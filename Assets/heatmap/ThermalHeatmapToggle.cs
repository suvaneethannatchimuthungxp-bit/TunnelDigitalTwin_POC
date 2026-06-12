using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

/// <summary>
/// Drives the ThermalHeatmap Volume component from a UI Toggle.
///
/// HOW TO SET UP IN UNITY:
/// 1. Create a UI Canvas (GameObject > UI > Canvas)
/// 2. Add a Toggle inside the Canvas (GameObject > UI > Toggle)
/// 3. Add this script to any GameObject (e.g. the Canvas or a Manager object)
/// 4. In the Inspector, assign:
///    - "Thermal Toggle" -> your UI Toggle
///    - "Volume"         -> the Volume in your scene that has ThermalHeatmap
///      (if left empty, it auto-finds the first active Volume in the scene)
/// 5. Make sure ThermalHeatmapFeature is added to your URP Renderer asset
/// </summary>
public sealed class ThermalHeatmapToggle : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("Drag the UI Toggle here from the Canvas.")]
    public Toggle thermalToggle;

    [Header("Volume Reference")]
    [Tooltip("Drag the Volume that contains ThermalHeatmap. Leave empty to auto-find.")]
    public Volume volume;

    [Header("Blend Settings")]
    [Tooltip("Blend value when heatmap is ON.")]
    [Range(0f, 1f)]
    public float blendOnValue = 0.8f;

    ThermalHeatmap thermalHeatmap;

    // -------------------------------------------------------------------------
    void Start()
    {
        // Auto-find Volume if not assigned
        if (volume == null)
        {
            volume = FindAnyObjectByType<Volume>();
            if (volume == null)
            {
                Debug.LogError("[ThermalHeatmapToggle] No Volume found in scene. " +
                    "Please create a Volume and add ThermalHeatmap to its profile.");
                return;
            }
        }

        // Try to get the ThermalHeatmap component from the Volume profile
        if (!volume.profile.TryGet(out thermalHeatmap))
        {
            Debug.LogError("[ThermalHeatmapToggle] ThermalHeatmap component not found in Volume Profile. " +
                "Open the Volume's profile asset and click 'Add Override > Custom > Thermal Heatmap'.");
            return;
        }

        // Validate toggle reference
        if (thermalToggle == null)
        {
            Debug.LogError("[ThermalHeatmapToggle] No UI Toggle assigned. " +
                "Drag a Toggle from your Canvas into the 'Thermal Toggle' field.");
            return;
        }

        // Register listener and apply initial state
        thermalToggle.onValueChanged.AddListener(SetThermalMode);
        SetThermalMode(thermalToggle.isOn);

        Debug.Log("[ThermalHeatmapToggle] Initialized. Toggle is currently: " +
            (thermalToggle.isOn ? "ON" : "OFF"));
    }

    // -------------------------------------------------------------------------
    /// <summary>
    /// Called automatically when the Toggle value changes.
    /// Can also be called manually from other scripts.
    /// </summary>
    public void SetThermalMode(bool isEnabled)
    {
        if (thermalHeatmap == null) return;

        thermalHeatmap.blend.overrideState = true;
        thermalHeatmap.blend.value = isEnabled ? blendOnValue : 0f;

        Debug.Log($"[ThermalHeatmapToggle] Heatmap {(isEnabled ? "ENABLED" : "DISABLED")} " +
            $"(blend = {thermalHeatmap.blend.value})");
    }

    // -------------------------------------------------------------------------
    void OnDestroy()
    {
        if (thermalToggle != null)
            thermalToggle.onValueChanged.RemoveListener(SetThermalMode);
    }
}
