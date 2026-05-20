using UnityEngine;
using TMPro;
using System.Collections;

public class DashboardModeManager : MonoBehaviour
{
    public GameObject adminDashboardPanel;

    public CanvasGroup adminCanvasGroup;

    public TMP_Dropdown modeDropdown;

    [Header("Report")]
    public BottomPanelUI bottomPanelUI;

    [Header("Animation")]
    public float fadeSpeed = 4f;

    void Start()
    {
        modeDropdown.onValueChanged.AddListener(OnModeChanged);

        adminDashboardPanel.SetActive(false);
    }

    void OnModeChanged(int index)
    {
        // NORMAL MODE
        if (index == 0)
        {
            StartCoroutine(HideAdminDashboard());
        }

        // ADMIN DASHBOARD
        else if (index == 1)
        {
            StartCoroutine(ShowAdminDashboard());

            bottomPanelUI.ShowPanel();
        }
    }

    IEnumerator ShowAdminDashboard()
    {
        adminDashboardPanel.SetActive(true);

        adminCanvasGroup.alpha = 0;

        Vector3 startScale =
            Vector3.one * 0.95f;

        Vector3 targetScale =
            Vector3.one;

        adminDashboardPanel.transform.localScale =
            startScale;

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;

            adminCanvasGroup.alpha =
                Mathf.Lerp(0, 1, t);

            adminDashboardPanel.transform.localScale =
                Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        adminCanvasGroup.alpha = 1;

        adminDashboardPanel.transform.localScale =
            Vector3.one;
    }

    IEnumerator HideAdminDashboard()
    {
        float t = 0;

        Vector3 startScale =
            Vector3.one;

        Vector3 targetScale =
            Vector3.one * 0.95f;

        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;

            adminCanvasGroup.alpha =
                Mathf.Lerp(1, 0, t);

            adminDashboardPanel.transform.localScale =
                Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        adminCanvasGroup.alpha = 0;

        adminDashboardPanel.SetActive(false);
    }
}