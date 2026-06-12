using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Volume settings for the Thermal Heatmap post-process effect (URP).
/// Add this component to a Volume profile, then add ThermalHeatmapFeature
/// to your URP Renderer asset.
/// </summary>
[Serializable, VolumeComponentMenu("Custom/Thermal Heatmap")]
[SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
public sealed class ThermalHeatmap : VolumeComponent, IPostProcessComponent
{
    [Tooltip("0 = normal scene, 1 = full thermal remap.")]
    public ClampedFloatParameter blend = new ClampedFloatParameter(0f, 0f, 1f);

    [Tooltip("Scene brightness that maps to 'cold' (blue).")]
    public ClampedFloatParameter rangeMin = new ClampedFloatParameter(0.0f, 0f, 1f);

    [Tooltip("Scene brightness that maps to 'hot' (white).")]
    public ClampedFloatParameter rangeMax = new ClampedFloatParameter(0.8f, 0f, 2f);

    [Tooltip("0 = infrared  1 = grayscale  2 = isotherm  3 = plasma  4 = heat zones  5 = delta")]
    public ClampedIntParameter palette = new ClampedIntParameter(0, 0, 5);

    public bool IsActive() => blend.value > 0f;

    // URP requires this; return false if you never want tile/cluster rendering
    public bool IsTileCompatible() => false;
}
