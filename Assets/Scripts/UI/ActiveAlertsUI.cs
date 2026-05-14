using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActiveAlertsUI : MonoBehaviour
{
    [Header("Alert Texts")]
    public TMP_Text alertText1;

    public TMP_Text alertText2;

    public TMP_Text alertText3;

    [Header("Alert Icons")]
    public Image alertIcon1;

    public Image alertIcon2;

    public Image alertIcon3;

    private string[] alerts =
        new string[3];

    private Color[] colors =
        new Color[3];

    void Start()
    {
        alertText1.text = "No Active Alerts";

        alertText2.text = "";

        alertText3.text = "";
    }

    // Add New Alert
    public void AddAlert(string message)
    {
        Color alertColor = Color.white;

        // INFO
        if (message.Contains("[INFO]"))
        {
            alertColor = Color.cyan;
        }

        // WARNING
        else if (message.Contains("[WARNING]"))
        {
            alertColor =
                new Color32(255, 176, 32, 255);
        }

        // CRITICAL
        else if (message.Contains("[CRITICAL]"))
        {
            alertColor =
                new Color32(255, 59, 48, 255);
        }

        // Shift old alerts
        alerts[2] = alerts[1];
        alerts[1] = alerts[0];
        alerts[0] = message;

        colors[2] = colors[1];
        colors[1] = colors[0];
        colors[0] = alertColor;

        UpdateUI();
    }

    void UpdateUI()
    {
        alertText1.text =
            string.IsNullOrEmpty(alerts[0]) ? "" : alerts[0];

        alertText2.text =
            string.IsNullOrEmpty(alerts[1]) ? "" : alerts[1];

        alertText3.text =
            string.IsNullOrEmpty(alerts[2]) ? "" : alerts[2];

        alertIcon1.color = colors[0];
        alertIcon2.color = colors[1];
        alertIcon3.color = colors[2];
    }
}