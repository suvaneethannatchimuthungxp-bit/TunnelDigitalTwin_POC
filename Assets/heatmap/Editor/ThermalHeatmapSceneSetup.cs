#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

/// <summary>
/// Builds the thermal heatmap demo objects in SampleScene.
/// Runs automatically once after scripts compile, or via Tools > Thermal Heatmap > Setup Sample Scene.
/// </summary>
[InitializeOnLoad]
static class ThermalHeatmapSceneSetup
{
    const string SetupDoneKey = "ThermalHeatmap.SampleSceneSetupDone";

    static ThermalHeatmapSceneSetup()
    {
        EditorApplication.delayCall += TryAutoSetup;
    }

    static void TryAutoSetup()
    {
        if (SessionState.GetBool(SetupDoneKey, false))
            return;

        if (!System.IO.File.Exists("Assets/Scenes/SampleScene.unity"))
            return;

        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        SetupSampleScene();
    }

    [MenuItem("Tools/Thermal Heatmap/Setup Sample Scene")]
    public static void SetupSampleSceneMenu()
    {
        SessionState.EraseBool(SetupDoneKey);
        SetupSampleScene();
    }

    static void SetupSampleScene()
    {
        var scene = EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);

        if (Object.FindAnyObjectByType<cube_ThermalHeatSource>() != null)
        {
            SessionState.SetBool(SetupDoneKey, true);
            Debug.Log("[ThermalHeatmap] Sample scene already set up.");
            return;
        }

        CreateHeatSource();
        CreateRayTargetWall();
        CreateThermalUi();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        SessionState.SetBool(SetupDoneKey, true);

        Debug.Log("[ThermalHeatmap] Sample scene setup complete.");
    }

    static void CreateHeatSource()
    {
        var heatSource = GameObject.CreatePrimitive(PrimitiveType.Cube);
        heatSource.name = "ThermalHeatSource";
        heatSource.transform.position = new Vector3(0f, 1f, 0f);
        heatSource.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

        var renderer = heatSource.GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.sharedMaterial.color = new Color(1f, 0.35f, 0.1f);

        heatSource.AddComponent<cube_ThermalHeatSource>();
    }

    static void CreateRayTargetWall()
    {
        var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "ThermalRayTarget";
        wall.transform.position = new Vector3(0f, 1f, 4f);
        wall.transform.localScale = new Vector3(6f, 3f, 0.25f);

        var renderer = wall.GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.sharedMaterial.color = new Color(0.55f, 0.55f, 0.6f);
    }

    static void CreateThermalUi()
    {
        if (Object.FindAnyObjectByType<EventSystem>() == null)
        {
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            eventSystem.AddComponent<InputSystemUIInputModule>();
#else
            eventSystem.AddComponent<StandaloneInputModule>();
#endif
        }

        var volume = Object.FindAnyObjectByType<Volume>();
        if (volume == null)
        {
            Debug.LogError("[ThermalHeatmap] No Volume found in scene.");
            return;
        }

        var canvasGo = new GameObject("ThermalUI", typeof(RectTransform));
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGo.AddComponent<GraphicRaycaster>();

        var panel = new GameObject("Panel", typeof(RectTransform));
        panel.transform.SetParent(canvasGo.transform, false);
        var panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0f, 1f);
        panelRect.anchorMax = new Vector2(0f, 1f);
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.anchoredPosition = new Vector2(20f, -20f);
        panelRect.sizeDelta = new Vector2(260f, 40f);

        var toggleGo = new GameObject("ThermalToggle", typeof(RectTransform));
        toggleGo.transform.SetParent(panel.transform, false);
        var toggleRect = toggleGo.GetComponent<RectTransform>();
        toggleRect.anchorMin = Vector2.zero;
        toggleRect.anchorMax = Vector2.one;
        toggleRect.offsetMin = Vector2.zero;
        toggleRect.offsetMax = Vector2.zero;

        var background = toggleGo.AddComponent<Image>();
        background.color = new Color(0.15f, 0.15f, 0.15f, 0.85f);

        var toggle = toggleGo.AddComponent<Toggle>();

        var checkmarkGo = new GameObject("Checkmark", typeof(RectTransform));
        checkmarkGo.transform.SetParent(toggleGo.transform, false);
        var checkRect = checkmarkGo.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0f, 0.5f);
        checkRect.anchorMax = new Vector2(0f, 0.5f);
        checkRect.pivot = new Vector2(0.5f, 0.5f);
        checkRect.anchoredPosition = new Vector2(18f, 0f);
        checkRect.sizeDelta = new Vector2(20f, 20f);
        var checkImage = checkmarkGo.AddComponent<Image>();
        checkImage.color = new Color(0.2f, 0.85f, 0.35f);

        var labelGo = new GameObject("Label", typeof(RectTransform));
        labelGo.transform.SetParent(toggleGo.transform, false);
        var labelRect = labelGo.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(42f, 0f);
        labelRect.offsetMax = Vector2.zero;
        var label = labelGo.AddComponent<Text>();
        label.text = "Thermal Heatmap";
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 18;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleLeft;

        toggle.targetGraphic = background;
        toggle.graphic = checkImage;
        toggle.isOn = false;

        var controller = canvasGo.AddComponent<ThermalHeatmapToggle>();
        controller.thermalToggle = toggle;
        controller.volume = volume;
    }
}
#endif
