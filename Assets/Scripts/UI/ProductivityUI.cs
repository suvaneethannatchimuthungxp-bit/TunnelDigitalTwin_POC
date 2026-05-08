using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductivityUI : MonoBehaviour
{
    [Header("Center Efficiency")]
    public TMP_Text efficiencyText;

    [Header("Metric Texts")]
    public TMP_Text activeMetricText;

    public TMP_Text idleMetricText;

    public TMP_Text otherMetricText;

    [Header("Metric Dots")]
    public Image activeDot;

    public Image idleDot;

    public Image otherDot;

    [Header("Circle Rings")]
    public Image activeRing;

    public Image idleRing;

    public Image otherRing;

    // Dot/Ring Colors
    private Color activeColor =
        new Color32(50, 255, 126, 255);

    private Color idleColor =
        new Color32(255, 176, 32, 255);

    private Color otherColor =
        new Color32(0, 191, 255, 255);

    void Start()
    {
        // Dot Colors
        activeDot.color = activeColor;

        idleDot.color = idleColor;

        otherDot.color = otherColor;

        // Ring Colors
        activeRing.color = activeColor;

        idleRing.color = idleColor;

        otherRing.color = otherColor;
    }

    void Update()
    {
        // Selected Worker
        if (WorkerSelection.Instance == null)
            return;

        WorkerStatus workerStatus =
            WorkerSelection.Instance
            .selectedWorkerStatus;

        if (workerStatus == null)
            return;

        // Time Values
        float active =
            workerStatus.activeTime;

        float idle =
            workerStatus.idleTime;

        // Temporary Other Time
        float other = 10f;

        // Total Time
        float total =
            active + idle + other;

        if (total <= 0)
            return;

        // Percentages
        float activePercent =
            (active / total) * 100f;

        float idlePercent =
            (idle / total) * 100f;

        float otherPercent =
            (other / total) * 100f;

        // Main Efficiency
        float efficiency =
            activePercent;

        // Center Efficiency Text
        efficiencyText.text =
            efficiency.ToString("F0") + "%";

        // Ring Fill Amounts
        activeRing.fillAmount =
            activePercent / 100f;

        idleRing.fillAmount =
            idlePercent / 100f;

        otherRing.fillAmount =
            otherPercent / 100f;

        // Ring Rotations
        activeRing.rectTransform.rotation =
            Quaternion.Euler(0, 0, 0);

        idleRing.rectTransform.rotation =
            Quaternion.Euler(
                0,
                0,
                -(activePercent * 3.6f)
            );

        otherRing.rectTransform.rotation =
            Quaternion.Euler(
                0,
                0,
                -((activePercent + idlePercent) * 3.6f)
            );

        // Active Metric
        activeMetricText.text =
            "Active Time\n" +
            Mathf.RoundToInt(active) +
            "s (" +
            activePercent.ToString("F0") +
            "%)";

        // Idle Metric
        idleMetricText.text =
            "Idle Time\n" +
            Mathf.RoundToInt(idle) +
            "s (" +
            idlePercent.ToString("F0") +
            "%)";

        // Other Metric
        otherMetricText.text =
            "Other Time\n" +
            Mathf.RoundToInt(other) +
            "s (" +
            otherPercent.ToString("F0") +
            "%)";
    }
}