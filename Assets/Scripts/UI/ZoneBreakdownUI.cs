using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ZoneBreakdownUI : MonoBehaviour
{
    [Header("Zone Sliders")]
    public Slider drillingSlider;

    public Slider materialSlider;

    public Slider breakSlider;

    public Slider restrictedSlider;

    [Header("Zone Time Texts")]
    public TMP_Text drillingTimeText;

    public TMP_Text materialTimeText;

    public TMP_Text breakTimeText;

    public TMP_Text restrictedTimeText;

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

        // Zone Times
        float drilling =
            workerStatus.drillingZoneTime;

        float material =
            workerStatus.materialZoneTime;

        float breakZone =
            workerStatus.breakZoneTime;

        float restricted =
            workerStatus.restrictedZoneTime;

        // Total Time
        float total =
            drilling +
            material +
            breakZone +
            restricted;

        if (total <= 0)
            return;

        // Percentages
        float drillingPercent =
            (drilling / total) * 100f;

        float materialPercent =
            (material / total) * 100f;

        float breakPercent =
            (breakZone / total) * 100f;

        float restrictedPercent =
            (restricted / total) * 100f;

        // Slider Fill Values
        drillingSlider.value =
            drillingPercent / 100f;

        materialSlider.value =
            materialPercent / 100f;

        breakSlider.value =
            breakPercent / 100f;

        restrictedSlider.value =
            restrictedPercent / 100f;

        // Time Texts
        drillingTimeText.text =
            Mathf.RoundToInt(drilling) +
            "s (" +
            drillingPercent.ToString("F0") +
            "%)";

        materialTimeText.text =
            Mathf.RoundToInt(material) +
            "s (" +
            materialPercent.ToString("F0") +
            "%)";

        breakTimeText.text =
            Mathf.RoundToInt(breakZone) +
            "s (" +
            breakPercent.ToString("F0") +
            "%)";

        restrictedTimeText.text =
            Mathf.RoundToInt(restricted) +
            "s (" +
            restrictedPercent.ToString("F0") +
            "%)";
    }
}