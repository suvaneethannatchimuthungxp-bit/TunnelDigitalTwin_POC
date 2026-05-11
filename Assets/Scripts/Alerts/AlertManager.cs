using UnityEngine;
using TMPro;

public class AlertManager : MonoBehaviour
{
    public static AlertManager Instance;

    [Header("Popup Alert")]
    public GameObject alertPanel;

    public TMP_Text alertText;

    [Header("Dashboard Alerts")]
    public ActiveAlertsUI activeAlertsUI;

    public EventTimelineUI eventTimelineUI;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowAlert(string message)
    {
        // Top Popup Alert
        alertPanel.SetActive(true);


        alertText.text = message;

        if (eventTimelineUI != null)
        {
            eventTimelineUI.AddEvent(
                message,
                Color.red
            );
        }
        // Dashboard Alert History
        if (activeAlertsUI != null)
        {
            activeAlertsUI.AddAlert(message);
        }

        // Reset Hide Timer
        CancelInvoke();

        Invoke(nameof(HideAlert), 3f);
    }

    void HideAlert()
    {
        alertPanel.SetActive(false);
    }
}