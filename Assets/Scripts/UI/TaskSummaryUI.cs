using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TaskCircleUI : MonoBehaviour
{
    [Header("Rings")]
    public Image completedRing;

    public Image progressRing;

    public Image pendingRing;

    [Header("Center Text")]
    public TMP_Text totalTaskText;

    [Header("Task Counts")]
    public int completedTasks = 2;

    public int inProgressTasks = 1;

    public int pendingTasks = 0;

    void Start()
    {
        UpdateCircle();
    }

    void UpdateCircle()
    {
        // TOTAL
        int total =
            completedTasks +
            inProgressTasks +
            pendingTasks;

        if (total <= 0)
            return;

        // CENTER TEXT
        totalTaskText.text =
            total.ToString() +
            "\nTotal Tasks";

        // PERCENTAGES
        float completedPercent =
            (float)completedTasks / total;

        float progressPercent =
            (float)inProgressTasks / total;

        float pendingPercent =
            (float)pendingTasks / total;

        // FILL AMOUNTS
        completedRing.fillAmount =
            completedPercent;

        progressRing.fillAmount =
            progressPercent;

        pendingRing.fillAmount =
            pendingPercent;

        // ROTATIONS
        completedRing.rectTransform.rotation =
            Quaternion.Euler(0, 0, 0);

        progressRing.rectTransform.rotation =
            Quaternion.Euler(
                0,
                0,
                -(completedPercent * 360f)
            );

        pendingRing.rectTransform.rotation =
            Quaternion.Euler(
                0,
                0,
                -((completedPercent +
                progressPercent) * 360f)
            );
    }
}