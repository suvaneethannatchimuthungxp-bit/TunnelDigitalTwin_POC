using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnvironmentMonitoringUI : MonoBehaviour
{
    [Header("Environment Texts")]

    public TMP_Text oxygenText;
    public TMP_Text co2Text;
    public TMP_Text methaneText;
    public TMP_Text temperatureText;
    public TMP_Text humidityText;
    public TMP_Text airflowText;

    [Header("Colors")]

    public Color normalColor = Color.white;

    public Color safeColor =
        new Color(0.2f, 1f, 0.7f, 1f);

    public Color warningColor =
        new Color(1f, 0.7f, 0f, 1f);

    public Color criticalColor =
        new Color(1f, 0.2f, 0.2f, 1f);

    [Header("Premium Radial Colors")]

    public Color oxygenColor =
        new Color(0.2f, 1f, 1f, 0.75f);

    public Color co2Color =
        new Color(0.7f, 0.4f, 1f, 0.75f);

    public Color temperatureColor =
        new Color(1f, 0.5f, 0.1f, 0.75f);

    public Color humidityColor =
        new Color(0.2f, 0.7f, 1f, 0.75f);

    public Color airflowColor =
        new Color(0.3f, 1f, 0.5f, 0.75f);

    [Header("Quick View Radials")]

    public Image o2Fill;
    public Image co2Fill;
    public Image temperatureFill;
    public Image humidityFill;
    public Image airflowFill;

    [Header("Quick View Texts")]

    public TMP_Text quickO2Text;
    public TMP_Text quickCO2Text;
    public TMP_Text quickTemperatureText;
    public TMP_Text quickHumidityText;
    public TMP_Text quickAirflowText;

    [Header("Smooth Speed")]

    public float smoothSpeed = 3f;

    private float o2Target;
    private float co2Target;
    private float tempTarget;
    private float humidityTarget;
    private float airflowTarget;

    void Update()
    {
        SimulateEnvironment();
        UpdateSmoothRadials();
    }

    void SimulateEnvironment()
    {
        // =========================
        // LIVE MOCK VALUES
        // =========================

        float oxygen =
            20f + Mathf.PingPong(Time.time * 0.05f, 1f);

        float co2 =
            400f + Mathf.PingPong(Time.time * 10f, 200f);

        float methane =
            Mathf.PingPong(Time.time * 0.02f, 5f);

        float temperature =
            28f + Mathf.PingPong(Time.time * 0.2f, 6f);

        float humidity =
            60f + Mathf.PingPong(Time.time * 0.1f, 15f);

        float airflow =
            2f + Mathf.PingPong(Time.time * 0.2f, 2f);

        // =========================
        // TARGET FILLS
        // =========================

        o2Target = oxygen / 21f;
        co2Target = co2 / 1000f;
        tempTarget = temperature / 50f;
        humidityTarget = humidity / 100f;
        airflowTarget = airflow / 5f;

        // =========================
        // AIRFLOW STATE
        // =========================

        string airflowStatus = "NORMAL";

        if (methane > 3f)
        {
            airflowStatus = "HIGH";
        }

        // =========================
        // DETAILED TEXT UPDATE
        // =========================

        oxygenText.text =
            "O2 LEVEL : " +
            oxygen.ToString("F1") + "%";

        co2Text.text =
            "CO2 LEVEL : " +
            co2.ToString("F0") + " ppm";

        methaneText.text =
            "CH4 STATUS : " +
            methane.ToString("F1") + "%";

        temperatureText.text =
            "TEMPERATURE : " +
            temperature.ToString("F1") + "°C";

        humidityText.text =
            "HUMIDITY : " +
            humidity.ToString("F0") + "%";

        airflowText.text =
            "AIRFLOW : " + airflowStatus;

        // =========================
        // QUICK VIEW TEXTS
        // =========================

        quickO2Text.text =
            oxygen.ToString("F1") + "%";

        quickCO2Text.text =
            co2.ToString("F0");

        quickTemperatureText.text =
            temperature.ToString("F1") + "°";

        quickHumidityText.text =
            humidity.ToString("F0") + "%";

        quickAirflowText.text =
            airflowStatus;

        // =========================
        // DEFAULT TEXT COLORS
        // =========================

        oxygenText.color = normalColor;
        co2Text.color = normalColor;
        methaneText.color = safeColor;
        temperatureText.color = normalColor;
        humidityText.color = normalColor;
        airflowText.color = safeColor;

        // =========================
        // PREMIUM RADIAL COLORS
        // =========================

        o2Fill.color = oxygenColor;
        co2Fill.color = co2Color;
        temperatureFill.color = temperatureColor;
        humidityFill.color = humidityColor;
        airflowFill.color = airflowColor;

        // =========================
        // WARNING
        // =========================

        if (methane > 2f)
        {
            methaneText.color =
                warningColor;

            airflowText.color =
                warningColor;

            airflowFill.color =
                warningColor;
        }

        // =========================
        // CRITICAL
        // =========================

        if (methane > 4f)
        {
            methaneText.color =
                criticalColor;

            airflowText.color =
                criticalColor;

            airflowText.text =
                "AIRFLOW : CRITICAL";

            airflowFill.color =
                criticalColor;

            co2Fill.color =
                criticalColor;
        }

        // =========================
        // HIGH TEMPERATURE
        // =========================

        if (temperature > 32f)
        {
            temperatureText.color =
                warningColor;

            temperatureFill.color =
                warningColor;
        }

        // =========================
        // LOW OXYGEN
        // =========================

        if (oxygen < 19.5f)
        {
            oxygenText.color =
                criticalColor;

            o2Fill.color =
                criticalColor;
        }
    }

    void UpdateSmoothRadials()
    {
        o2Fill.fillAmount =
            Mathf.Lerp(
                o2Fill.fillAmount,
                o2Target,
                Time.deltaTime * smoothSpeed);

        co2Fill.fillAmount =
            Mathf.Lerp(
                co2Fill.fillAmount,
                co2Target,
                Time.deltaTime * smoothSpeed);

        temperatureFill.fillAmount =
            Mathf.Lerp(
                temperatureFill.fillAmount,
                tempTarget,
                Time.deltaTime * smoothSpeed);

        humidityFill.fillAmount =
            Mathf.Lerp(
                humidityFill.fillAmount,
                humidityTarget,
                Time.deltaTime * smoothSpeed);

        airflowFill.fillAmount =
            Mathf.Lerp(
                airflowFill.fillAmount,
                airflowTarget,
                Time.deltaTime * smoothSpeed);
    }
}