using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
        new Color(0.2f, 1f, 0.7f, 1f);

    public Color warningColor =
        new Color(1f, 0.7f, 0f, 1f);

    public Color criticalColor =
        new Color(1f, 0.2f, 0.2f, 1f);

    [Header("Premium Radial Colors")]

    public Color rpmColor =
        new Color(0.2f, 1f, 1f, 0.75f);

    public Color pressureColor =
        new Color(1f, 0.5f, 0.1f, 0.75f);

    public Color speedColor =
        new Color(0.3f, 1f, 0.5f, 0.75f);

    [Header("Radial UI")]

    public Image rpmRadialFill;

    public Image pressureRadialFill;

    public Image speedRadialFill;

    [Header("Smooth Speed")]

    public float smoothSpeed = 3f;

    private float rpmTarget;

    private float pressureTarget;

    private float speedTarget;

    void Update()
    {
        SimulateTelemetry();

        UpdateSmoothRadials();
    }

    void SimulateTelemetry()
    {
        // =========================
        // LIVE MOCK VALUES
        // =========================

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

        // =========================
        // TARGET FILLS
        // =========================

        rpmTarget =
            rpm / 5f;

        pressureTarget =
            groutPressure / 3f;

        speedTarget =
            advanceRate / 15f;

        // =========================
        // STATUS
        // =========================

        statusText.text =
            "STATUS : ACTIVE";

        statusText.color =
            activeColor;

        // =========================
        // ADVANCE RATE
        // =========================

        advanceRateText.text =
            "ADVANCE RATE : " +
            advanceRate.ToString("F1") +
            " m/day";

        advanceRateText.color =
            normalColor;

        // =========================
        // RPM
        // =========================

        rpmText.text =
            "CUTTER RPM : " +
            rpm.ToString("F1");

        rpmText.color =
            normalColor;

        // =========================
        // TORQUE
        // =========================

        torqueText.text =
            "TORQUE : " +
            torque.ToString("F0") +
            " kNm";

        torqueText.color =
            normalColor;

        // =========================
        // PRESSURE
        // =========================

        groutPressureText.text =
            "GROUT PRESSURE : " +
            groutPressure.ToString("F1") +
            " bar";

        groutPressureText.color =
            normalColor;

        // =========================
        // RING BUILD
        // =========================

        ringBuildText.text =
            "RING BUILD TIME : " +
            ringBuild.ToString("F0") +
            " min";

        ringBuildText.color =
            normalColor;

        // =========================
        // PREMIUM RADIAL COLORS
        // =========================

        rpmRadialFill.color =
            rpmColor;

        pressureRadialFill.color =
            pressureColor;

        speedRadialFill.color =
            speedColor;

        // =========================
        // WARNING STATE
        // =========================

        if (torque > 3100f)
        {
            torqueText.color =
                warningColor;

            pressureRadialFill.color =
                warningColor;

            statusText.text =
                "STATUS : WARNING";

            statusText.color =
                warningColor;
        }

        // =========================
        // CRITICAL STATE
        // =========================

        if (torque > 3250f)
        {
            torqueText.color =
                criticalColor;

            rpmRadialFill.color =
                criticalColor;

            pressureRadialFill.color =
                criticalColor;

            speedRadialFill.color =
                criticalColor;

            statusText.text =
                "STATUS : CRITICAL";

            statusText.color =
                criticalColor;
        }
    }

    void UpdateSmoothRadials()
    {
        rpmRadialFill.fillAmount =
            Mathf.Lerp(
                rpmRadialFill.fillAmount,
                rpmTarget,
                Time.deltaTime * smoothSpeed);

        pressureRadialFill.fillAmount =
            Mathf.Lerp(
                pressureRadialFill.fillAmount,
                pressureTarget,
                Time.deltaTime * smoothSpeed);

        speedRadialFill.fillAmount =
            Mathf.Lerp(
                speedRadialFill.fillAmount,
                speedTarget,
                Time.deltaTime * smoothSpeed);
    }
}