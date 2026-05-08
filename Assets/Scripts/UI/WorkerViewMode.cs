using System.Collections;
using UnityEngine;

public class WorkerViewMode : MonoBehaviour
{
    [Header("Tunnel")]
    public Renderer[] tunnelRenderers;

    public Material tunnelWireframe;

    // STORE ORIGINAL MATERIALS
    private Material[][] originalTunnelMaterials;

    [Header("Workers")]
    public Renderer[] workerRenderers;

    public Material workerNormal;

    public Material workerHighlight;

    [Header("Zones")]
    public Renderer[] zoneRenderers;

    private Material[][] originalZoneMaterials;

    [Header("Transition")]
    public float transitionDuration = 0.6f;

    public AnimationCurve transitionCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Tunnel Visual")]
    public Color tunnelBaseColor =
        new Color(0f, 0.85f, 1f);

    [Range(0f, 1f)]
    public float tunnelTargetAlpha = 0.03f;

    [Header("Worker Glow")]
    public Color workerGlowColor =
        new Color(0.2f, 1f, 0.8f);

    public float workerGlowIntensity = 2.5f;

    [Header("Zone Visual")]
    [Range(0f, 1f)]
    public float hiddenZoneAlpha = 0f;

  
    [Range(0f, 1f)]
    public float visibleZoneAlpha = 0.28f;

    [Header("Alerts")]
    public Renderer[] alertWorkers;

    public Renderer restrictedZoneRenderer;

    public Color alertGlowColor =
        new Color(1f, 0.2f, 0.1f);

    public float alertGlowIntensity = 5f;

    public float restrictedZoneAlertAlpha = 0.18f;

    [Header("Analytics")]
    public RightPanelUI rightPanelUI;

    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        // STORE ORIGINAL TUNNEL MATERIALS
        originalTunnelMaterials =
            new Material[tunnelRenderers.Length][];

        for (int i = 0; i < tunnelRenderers.Length; i++)
        {
            originalTunnelMaterials[i] =
                tunnelRenderers[i].materials;
        }

        // STORE ORIGINAL ZONE MATERIALS
        originalZoneMaterials =
            new Material[zoneRenderers.Length][];

        for (int i = 0; i < zoneRenderers.Length; i++)
        {
            originalZoneMaterials[i] =
                zoneRenderers[i].materials;
        }

        // HIDE ZONES INITIALLY
        SetZoneAlpha(hiddenZoneAlpha);

        // INITIAL MODE
        ShowOverview();
    }

    // ==================================================
    // ZONE ALPHA CONTROL
    // ==================================================

    void SetZoneAlpha(float alpha)
    {
        foreach (Renderer r in zoneRenderers)
        {
            foreach (Material mat in r.materials)
            {
                // BASE COLOR
                if (mat.HasProperty("_BaseColor"))
                {
                    Color c =
                        mat.GetColor("_BaseColor");

                    c.a = alpha;

                    mat.SetColor(
                        "_BaseColor",
                        c
                    );
                }

                // EMISSION
                if (mat.HasProperty("_EmissionColor"))
                {
                    Color emission =
                        mat.GetColor("_BaseColor");

                    mat.SetColor(
                        "_EmissionColor",
                        emission * alpha * 8f
                    );
                }
            }
        }
    }

    // ==================================================
    // SHOW ALERTS MODE
    // ==================================================

    public void ShowAlertsMode()
    {
        rightPanelUI.HidePanel();
        StopAllCoroutines();

        StartCoroutine(
            TransitionToAlertsMode()
        );
    }
    // ANALYTICS MODE
    // ==================================================

    public void ShowAnalyticsMode()
    {
        // SHOW RIGHT PANEL
        rightPanelUI.ShowPanel();

        // USE NORMAL OVERVIEW STATE
        ShowOverview();
    }
    // ==================================================
    // ALERTS TRANSITION
    // ==================================================

    IEnumerator TransitionToAlertsMode()
    {
        float time = 0;

        // APPLY TUNNEL WIREFRAME
        for (int i = 0; i < tunnelRenderers.Length; i++)
        {
            Material[] mats =
                new Material[
                    tunnelRenderers[i].materials.Length
                ];

            for (int j = 0; j < mats.Length; j++)
            {
                mats[j] =
                    new Material(tunnelWireframe);
            }

            tunnelRenderers[i].materials =
                mats;
        }

        // RESET TUNNEL ALPHA
        foreach (Renderer r in tunnelRenderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    Color c =
                        tunnelBaseColor;

                    c.a = 0f;

                    mat.SetColor(
                        "_BaseColor",
                        c
                    );
                }
            }
        }

        // HIDE ALL ZONES FIRST
        SetZoneAlpha(hiddenZoneAlpha);

        // SHOW ONLY RESTRICTED ZONE
        foreach (Material mat in restrictedZoneRenderer.materials)
        {
            // BASE COLOR
            if (mat.HasProperty("_BaseColor"))
            {
                Color c =
                    alertGlowColor;

                c.a = restrictedZoneAlertAlpha;

                mat.SetColor(
                    "_BaseColor",
                    c
                );
            }

            // EMISSION
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");

                mat.SetColor(
                    "_EmissionColor",
                    alertGlowColor *
                    alertGlowIntensity
                );
            }
        }

        // RESET ALERT WORKERS
        foreach (Renderer r in alertWorkers)
        {
            Material mat =
                r.material;

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor(
                    "_EmissionColor",
                    Color.black
                );
            }
        }

        // SMOOTH TRANSITION
        while (time < transitionDuration)
        {
            time += Time.deltaTime;

            float t =
                transitionCurve.Evaluate(
                    time / transitionDuration
                );

            // TUNNEL TRANSITION
            foreach (Renderer r in tunnelRenderers)
            {
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color c =
                            tunnelBaseColor;

                        c.a =
                            Mathf.Lerp(
                                0f,
                                tunnelTargetAlpha,
                                t
                            );

                        mat.SetColor(
                            "_BaseColor",
                            c
                        );
                    }
                }
            }

            // ALERT WORKER GLOW
            foreach (Renderer r in alertWorkers)
            {
                Material mat =
                    r.material;

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");

                    mat.SetColor(
                        "_EmissionColor",
                        Color.Lerp(
                            Color.black,
                            alertGlowColor *
                            alertGlowIntensity,
                            t
                        )
                    );
                }
            }

            yield return null;
        }
    }

    // ==================================================
    // OVERVIEW MODE
    // ==================================================

    public void ShowOverview()
    {
        rightPanelUI.ShowPanel();
        StopAllCoroutines();

        StartCoroutine(
            TransitionToOverview()
        );
    }

    IEnumerator TransitionToOverview()
    {
        float time = 0;

        // RESTORE TUNNEL MATERIALS
        for (int i = 0; i < tunnelRenderers.Length; i++)
        {
            tunnelRenderers[i].materials =
                originalTunnelMaterials[i];
        }

        // RESTORE ZONE MATERIALS
        for (int i = 0; i < zoneRenderers.Length; i++)
        {
            zoneRenderers[i].materials =
                originalZoneMaterials[i];
        }

        // HIDE ZONES
        SetZoneAlpha(hiddenZoneAlpha);

        // STORE CURRENT WORKER EMISSION
        Color[] startWorkerEmission =
            new Color[workerRenderers.Length];

        for (int i = 0; i < workerRenderers.Length; i++)
        {
            Material mat =
                workerRenderers[i].material;

            if (mat.HasProperty("_EmissionColor"))
            {
                startWorkerEmission[i] =
                    mat.GetColor("_EmissionColor");
            }
        }

        while (time < transitionDuration)
        {
            time += Time.deltaTime;

            float t =
                transitionCurve.Evaluate(
                    time / transitionDuration
                );

            // REMOVE WORKER GLOW
            for (int i = 0; i < workerRenderers.Length; i++)
            {
                Material mat =
                    workerRenderers[i].material;

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor(
                        "_EmissionColor",
                        Color.Lerp(
                            startWorkerEmission[i],
                            Color.black,
                            t
                        )
                    );
                }
            }

            yield return null;
        }

        // FINAL NORMAL MATERIAL
        foreach (Renderer r in workerRenderers)
        {
            r.material =
                workerNormal;
        }
    }

    // ==================================================
    // WORKERS MODE
    // ==================================================

    public void ShowWorkersMode()
    {
        rightPanelUI.HidePanel();
        StopAllCoroutines();

        StartCoroutine(
            TransitionToWorkersMode()
        );
    }

    IEnumerator TransitionToWorkersMode()
    {
        float time = 0;

        // APPLY WIREFRAME MATERIALS
        for (int i = 0; i < tunnelRenderers.Length; i++)
        {
            Material[] mats =
                new Material[
                    tunnelRenderers[i].materials.Length
                ];

            for (int j = 0; j < mats.Length; j++)
            {
                mats[j] =
                    new Material(tunnelWireframe);
            }

            tunnelRenderers[i].materials =
                mats;
        }

        // RESTORE ZONE MATERIALS
        for (int i = 0; i < zoneRenderers.Length; i++)
        {
            zoneRenderers[i].materials =
                originalZoneMaterials[i];
        }

        // HIDE ZONES
        SetZoneAlpha(hiddenZoneAlpha);

        // RESET TUNNEL ALPHA
        foreach (Renderer r in tunnelRenderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    Color c =
                        tunnelBaseColor;

                    c.a = 0f;

                    mat.SetColor(
                        "_BaseColor",
                        c
                    );
                }
            }
        }

        // APPLY WORKER MATERIAL
        foreach (Renderer r in workerRenderers)
        {
            r.material =
                workerHighlight;
        }

        // RESET WORKER EMISSION
        foreach (Renderer r in workerRenderers)
        {
            Material mat =
                r.material;

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor(
                    "_EmissionColor",
                    Color.black
                );
            }
        }

        while (time < transitionDuration)
        {
            time += Time.deltaTime;

            float t =
                transitionCurve.Evaluate(
                    time / transitionDuration
                );

            // TUNNEL TRANSITION
            foreach (Renderer r in tunnelRenderers)
            {
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color c =
                            tunnelBaseColor;

                        c.a =
                            Mathf.Lerp(
                                0f,
                                tunnelTargetAlpha,
                                t
                            );

                        mat.SetColor(
                            "_BaseColor",
                            c
                        );
                    }
                }
            }

            // WORKER TRANSITION
            foreach (Renderer r in workerRenderers)
            {
                Material mat =
                    r.material;

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.SetColor(
                        "_EmissionColor",
                        Color.Lerp(
                            Color.black,
                            workerGlowColor *
                            workerGlowIntensity,
                            t
                        )
                    );
                }
            }

            yield return null;
        }
    }

    // ==================================================
    // ZONES MODE
    // ==================================================

    public void ShowZonesMode()
    {
        rightPanelUI.HidePanel();
        StopAllCoroutines();

        StartCoroutine(
            TransitionToZonesMode()
        );
    }

    IEnumerator TransitionToZonesMode()
    {
        float time = 0;

        // APPLY WIREFRAME MATERIALS
        for (int i = 0; i < tunnelRenderers.Length; i++)
        {
            Material[] mats =
                new Material[
                    tunnelRenderers[i].materials.Length
                ];

            for (int j = 0; j < mats.Length; j++)
            {
                mats[j] =
                    new Material(tunnelWireframe);
            }

            tunnelRenderers[i].materials =
                mats;
        }

        // RESET TUNNEL ALPHA
        foreach (Renderer r in tunnelRenderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_BaseColor"))
                {
                    Color c =
                        tunnelBaseColor;

                    c.a = 0f;

                    mat.SetColor(
                        "_BaseColor",
                        c
                    );
                }
            }
        }

        // RESTORE ORIGINAL ZONE MATERIALS
        for (int i = 0; i < zoneRenderers.Length; i++)
        {
            zoneRenderers[i].materials =
                originalZoneMaterials[i];
        }

        while (time < transitionDuration)
        {
            time += Time.deltaTime;

            float t =
                transitionCurve.Evaluate(
                    time / transitionDuration
                );

            // TUNNEL TRANSITION
            foreach (Renderer r in tunnelRenderers)
            {
                foreach (Material mat in r.materials)
                {
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color c =
                            tunnelBaseColor;

                        c.a =
                            Mathf.Lerp(
                                0f,
                                tunnelTargetAlpha,
                                t
                            );

                        mat.SetColor(
                            "_BaseColor",
                            c
                        );
                    }
                }
            }

            // ZONE TRANSITION
            foreach (Renderer r in zoneRenderers)
            {
                foreach (Material mat in r.materials)
                {
                    // ALPHA
                    if (mat.HasProperty("_BaseColor"))
                    {
                        Color c =
                            mat.GetColor("_BaseColor");

                        c.a =
                            Mathf.Lerp(
                                hiddenZoneAlpha,
                                visibleZoneAlpha,
                                t
                            );

                        mat.SetColor(
                            "_BaseColor",
                            c
                        );
                    }

                    // EMISSION
                    if (mat.HasProperty("_EmissionColor"))
                    {
                        Color emission =
                            mat.GetColor("_BaseColor");

                        mat.SetColor(
                            "_EmissionColor",
                            emission * t * 2f
                        );
                    }
                }
            }
           
            yield return null;
        }
    }
}