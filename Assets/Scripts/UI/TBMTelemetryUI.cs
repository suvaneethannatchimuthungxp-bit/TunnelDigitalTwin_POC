using UnityEngine;
using TMPro;

public class TBMTelemetryUI : MonoBehaviour
{
    [Header("Telemetry Texts")]

    public TMP_Text statusText;

    public TMP_Text advanceRateText;

    public TMP_Text rpmText;

    public TMP_Text torqueText;

    public TMP_Text groutPressureText;

    public TMP_Text ringBuildText;

    [Header("Colors")]

    public Color normalColor =
        Color.white;

    public Color activeColor =
        Color.green;

    public Color warningColor =
        new Color(1f, 0.6f, 0f);

    public Color criticalColor =
        Color.red;

    void Update()
    {
        SimulateTelemetry();
    }

    void SimulateTelemetry()
    {
        // LIVE MOCK VALUES

        float advanceRate =
            10f + Mathf.PingPong(Time.time * 0.5f, 5f);

        float rpm =
            3f + Mathf.PingPong(Time.time * 0.3f, 2f);

        float torque =
            2800f + Mathf.PingPong(Time.time * 50f, 500f);

        float groutPressure =
            1.5f + Mathf.PingPong(Time.time * 0.1f, 1f);

        float ringBuild =
            30f + Mathf.PingPong(Time.time * 0.2f, 10f);

        // STATUS

        statusText.text =
            "STATUS : ACTIVE";

        statusText.color =
            activeColor;

        // ADVANCE RATE

        advanceRateText.text =
            "ADVANCE RATE : " +
            advanceRate.ToString("F1") +
            " m/day";

        advanceRateText.color =
            normalColor;

        // RPM

        rpmText.text =
            "CUTTER RPM : " +
            rpm.ToString("F1");

        rpmText.color =
            normalColor;

        // TORQUE

        torqueText.text =
            "TORQUE : " +
            torque.ToString("F0") +
            " kNm";

        torqueText.color =
            normalColor;

        // PRESSURE

        groutPressureText.text =
            "GROUT PRESSURE : " +
            groutPressure.ToString("F1") +
            " bar";

        groutPressureText.color =
            normalColor;

        // RING BUILD

        ringBuildText.text =
            "RING BUILD TIME : " +
            ringBuild.ToString("F0") +
            " min";

        ringBuildText.color =
            normalColor;

        // WARNING

        if (torque > 3100f)
        {
            torqueText.color =
                warningColor;
        }

        // CRITICAL

        if (torque > 3250f)
        {
            torqueText.color =
                criticalColor;

            statusText.text =
                "STATUS : WARNING";

            statusText.color =
                criticalColor;
        }
    }
}