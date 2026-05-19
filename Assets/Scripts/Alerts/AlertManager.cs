using UnityEngine;
using TMPro;


public enum AlertSeverity
{
    Info,
    Warning,
    Critical
}

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



    public void ShowAlert(
     string message,
     AlertSeverity severity,
     WorkerStatus workerStatus)
    {
        // SHOW POPUP

        alertPanel.SetActive(true);

        // MESSAGE

        alertText.text =
            "[" + severity.ToString().ToUpper() + "]\n" + message;

        // DEFAULT COLOR

        Color alertColor = Color.white;

        // SEVERITY COLORS

        switch (severity)
        {
            case AlertSeverity.Info:

                alertColor =
                    Color.cyan;

                break;

            case AlertSeverity.Warning:

                alertColor =
                    new Color(1f, 0.6f, 0f);

                break;

            case AlertSeverity.Critical:

                alertColor =
                    Color.red;

                break;
        }

        // APPLY COLOR

        alertText.color =
            alertColor;

        // TIMELINE

      
        

        // ACTIVE ALERTS

        if (activeAlertsUI != null)
        {
            activeAlertsUI.AddAlert(
                "[" + severity.ToString().ToUpper() + "] " + message
            );
        }

        int replayIndex = -1;

    //    if (ReplayManager.Instance != null &&
    //        workerStatus != null)
    //    {
    //        replayIndex =
    //            ReplayManager.Instance.RecordEvent(
    //                message,
    //                workerStatus.transform,
    //                workerStatus.currentState.ToString()
    //            );
        
    //}
        if (eventTimelineUI != null)
        {
            eventTimelineUI.AddEvent(
                "[" + severity.ToString().ToUpper() + "] " + message,
                alertColor,
                replayIndex
            );
        }
        // RESET TIMER

        CancelInvoke();

        Invoke(nameof(HideAlert), 3f);
    }

    void HideAlert()
    {
        alertPanel.SetActive(false);
    }
}