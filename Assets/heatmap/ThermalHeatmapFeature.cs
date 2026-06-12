using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

/// <summary>
/// URP Renderer Feature for the Thermal Heatmap post-process effect.
/// Add this to your URP Renderer asset (Project Settings > Graphics,
/// or directly on your UniversalRendererData asset).
/// </summary>
public sealed class ThermalHeatmapFeature : ScriptableRendererFeature
{
    // -------------------------------------------------------------------------
    // Settings exposed in the Renderer asset inspector
    // -------------------------------------------------------------------------
    [System.Serializable]
    public class Settings
    {
        [Tooltip("When in the frame the effect is injected.")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    public Settings settings = new Settings();

    ThermalHeatmapPass m_Pass;

    // -------------------------------------------------------------------------
    // ScriptableRendererFeature API
    // -------------------------------------------------------------------------
    public override void Create()
    {
        m_Pass = new ThermalHeatmapPass(settings.renderPassEvent);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Skip preview and reflection cameras
        if (renderingData.cameraData.cameraType == CameraType.Preview || 
            renderingData.cameraData.cameraType == CameraType.Reflection)
            return;

        // Skip Minimap and Gizmo cameras to prevent them from turning blue/distorting
        var camera = renderingData.cameraData.camera;
        if (camera != null)
        {
            string camName = camera.name.ToLower();
            if (camName.Contains("minimap") || camName.Contains("gizmo"))
                return;
        }

        // Only enqueue when the volume component is active
        var stack = VolumeManager.instance.stack;
        var component = stack.GetComponent<ThermalHeatmap>();
        if (component == null || !component.IsActive())
            return;

        renderer.EnqueuePass(m_Pass);
    }

    protected override void Dispose(bool disposing)
    {
        m_Pass?.Dispose();
    }

    // =========================================================================
    // Inner render pass
    // =========================================================================
    sealed class ThermalHeatmapPass : ScriptableRenderPass, System.IDisposable
    {
        const string k_ShaderName  = "Hidden/Shader/ThermalHeatmap";

        Material m_Material;

        public ThermalHeatmapPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;

            // Request the camera depth texture so the shader can reconstruct
            // world-space positions per pixel for accurate 3D heat projection.
            ConfigureInput(ScriptableRenderPassInput.Depth);

            var shader = Shader.Find(k_ShaderName);
            if (shader != null)
                m_Material = CoreUtils.CreateEngineMaterial(shader);
            else
                Debug.LogError($"[ThermalHeatmap] Shader '{k_ShaderName}' not found.");
        }

        // ------------------------------------------------------------------
        // Render Graph logic (Unity 6+)
        // ------------------------------------------------------------------
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (m_Material == null)
                return;

            // Grab the current Volume settings
            var stack = VolumeManager.instance.stack;
            var component = stack.GetComponent<ThermalHeatmap>();
            if (component == null || !component.IsActive())
                return;

            // Retrieve resource data
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            if (resourceData == null || cameraData == null)
                return;

            TextureHandle cameraColor = resourceData.activeColorTexture;
            if (!cameraColor.IsValid())
                return;

            // Upload volume parameters
            m_Material.SetFloat("_Blend",    component.blend.value);
            m_Material.SetFloat("_RangeMin", component.rangeMin.value);
            m_Material.SetFloat("_RangeMax", component.rangeMax.value);
            m_Material.SetInt  ("_Palette",  component.palette.value);

            // Upload ray-hit data from the heat source
            UploadRayHits(cameraData.camera);

            // Create temporary texture descriptor based on camera color target
            TextureDesc desc = renderGraph.GetTextureDesc(cameraColor);
            desc.name = "_ThermalHeatmapTemp";
            desc.clearBuffer = false;
            desc.depthBufferBits = 0;

            TextureHandle tempRT = renderGraph.CreateTexture(desc);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Thermal Heatmap Pass", out var passData))
            {
                passData.src = cameraColor;
                passData.material = m_Material;

                builder.UseTexture(cameraColor);
                
                TextureHandle depth = resourceData.activeDepthTexture;
                if (depth.IsValid())
                {
                    builder.UseTexture(depth);
                }

                builder.SetRenderAttachment(tempRT, 0);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
                {
                    Blitter.BlitTexture(context.cmd, data.src, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // Copy 2: tempRT -> cameraColor
            renderGraph.AddCopyPass(tempRT, cameraColor, "Copy Thermal Heatmap Back");
        }

        class PassData
        {
            public TextureHandle src;
            public Material material;
        }



        // ------------------------------------------------------------------
        // Upload raw world-space hit positions — the shader reconstructs
        // each pixel's world position from depth and computes 3D distances.
        // ------------------------------------------------------------------
        void UploadRayHits(Camera cam)
        {
            const int ShaderMaxHits = 1024;
            
            // Gather all active points from all sources
            var allPoints = new System.Collections.Generic.List<(Vector3 position, float timeCreated, float intensity, float radius)>();
            
            var instances = cube_ThermalHeatSource.Instances;
            for (int sourceIdx = 0; sourceIdx < instances.Count; sourceIdx++)
            {
                var source = instances[sourceIdx];
                if (source == null) continue;

                var pts = source.ActivePoints;
                for (int ptIdx = 0; ptIdx < pts.Count; ptIdx++)
                {
                    var pt = pts[ptIdx];
                    allPoints.Add((pt.position, pt.timeCreated, pt.intensity, source.heatRadius));
                }
            }

            // Sort points by timeCreated descending (newest first) to prioritize active trails
            allPoints.Sort((a, b) => b.timeCreated.CompareTo(a.timeCreated));

            int count = Mathf.Min(allPoints.Count, ShaderMaxHits);
            Vector4[] hitPositions = new Vector4[ShaderMaxHits];
            float[] hitRadii = new float[ShaderMaxHits];

            for (int i = 0; i < count; i++)
            {
                var pt = allPoints[i];
                hitPositions[i] = new Vector4(pt.position.x, pt.position.y, pt.position.z, pt.intensity);
                hitRadii[i] = pt.radius;
            }

            m_Material.SetInt         ("_RayHitCount",      count);
            m_Material.SetVectorArray ("_RayHitPositions",  hitPositions);
            m_Material.SetFloatArray  ("_RayHitRadii",      hitRadii);

            // Backwards compatibility for single-source settings
            float defaultRadius = instances.Count > 0 ? instances[0].heatRadius : 1f;
            float defaultIntensity = instances.Count > 0 ? instances[0].heatIntensity : 1f;
            m_Material.SetFloat       ("_RayHitRadius",     defaultRadius);
            m_Material.SetFloat       ("_RayHitIntensity",  defaultIntensity);
        }

        // ------------------------------------------------------------------
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            // RTHandle is reused; don't release here
        }

        public void Dispose()
        {
            CoreUtils.Destroy(m_Material);
        }
    }
}
