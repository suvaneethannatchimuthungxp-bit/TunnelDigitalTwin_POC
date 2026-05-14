using UnityEngine;
using TMPro;

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

    public Color safeColor = Color.green;

    public Color warningColor =
        new Color(1f, 0.6f, 0f);

    public Color criticalColor =
        Color.red;

    void Update()
    {
        SimulateEnvironment();
    }

    void SimulateEnvironment()
    {
        // LIVE MOCK VALUES

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

        // AIRFLOW STATE

        string airflowStatus = "NORMAL";

        if (methane > 3f)
        {
            airflowStatus = "HIGH";
        }

        // UPDATE TEXT

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

        // DEFAULT COLORS

        oxygenText.color =
            normalColor;

        co2Text.color =
            normalColor;

        methaneText.color =
            safeColor;

        temperatureText.color =
            normalColor;

        humidityText.color =
            normalColor;

        airflowText.color =
            safeColor;

        // WARNING

        if (methane > 2f)
        {
            methaneText.color =
                warningColor;

            airflowText.color =
                warningColor;
        }

        // CRITICAL

        if (methane > 4f)
        {
            methaneText.color =
                criticalColor;

            airflowText.color =
                criticalColor;

            airflowText.text =
                "AIRFLOW : CRITICAL";
        }

        // HIGH TEMPERATURE

        if (temperature > 32f)
        {
            temperatureText.color =
                warningColor;
        }

        // LOW OXYGEN

        if (oxygen < 19.5f)
        {
            oxygenText.color =
                criticalColor;
        }
    }
}