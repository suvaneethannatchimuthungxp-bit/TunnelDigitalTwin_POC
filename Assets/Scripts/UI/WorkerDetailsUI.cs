//using UnityEngine;
//using TMPro;

//public class WorkerDetailsUI : MonoBehaviour
//{
//   // [Header("Worker Data")]
//   // public WorkerStatus workerStatus;

//    [Header("Worker Details")]
//    public TMP_Text workerNameText;

//    public TMP_Text workerStateText;

//    public TMP_Text workerZoneText;

//    [Header("Time Analytics")]
//    public TMP_Text activeTimeText;

//    public TMP_Text idleTimeText;

//    public TMP_Text totalTimeText;

//    void Update()
//    {
//        if (WorkerSelection.Instance == null)
//            return;

//        WorkerStatus workerStatus =
//            WorkerSelection.Instance
//            .selectedWorkerStatus;

//        if (workerStatus == null)
//            return;

//        // Rounded Times
//        int active =
//            Mathf.RoundToInt(workerStatus.activeTime);

//        int idle =
//            Mathf.RoundToInt(workerStatus.idleTime);

//        int total =
//            active + idle;

//        // Worker Name
//        workerNameText.text =
//            "Worker : " +
//            workerStatus.gameObject.name;

//        // Worker State
//        workerStateText.text =
//            "State : " +
//            workerStatus.currentState.ToString();

//        // Worker Zone
//        workerZoneText.text =
//            "Zone : " +
//            workerStatus.currentZone;

//        // Time Analytics
//        activeTimeText.text =
//            "Time Active : " +
//            active + "s";

//        idleTimeText.text =
//            "Idle Time : " +
//            idle + "s";

//        totalTimeText.text =
//            "Total Time : " +
//            total + "s";
//    }
//}

using UnityEngine;
using TMPro;

public class WorkerDetailsUI : MonoBehaviour
{
    [Header("Worker Details")]
    public TMP_Text workerNameText;

    public TMP_Text workerStateText;

    public TMP_Text workerZoneText;

    [Header("Time Analytics")]
    public TMP_Text activeTimeText;

    public TMP_Text idleTimeText;

    public TMP_Text totalTimeText;

    [Header("State Colors")]
    public Color workingColor =
        Color.green;

    public Color walkingColor =
        new Color(0.1f, 0.6f, 1f);

    public Color idleColor =
        new Color(1f, 0.6f, 0f);

    public Color restrictedColor =
        Color.red;

    void Update()
    {
        if (WorkerSelection.Instance == null)
            return;

        WorkerStatus workerStatus =
            WorkerSelection.Instance
            .selectedWorkerStatus;

        if (workerStatus == null)
            return;

        // TIMES
        int active =
            Mathf.RoundToInt(
                workerStatus.activeTime
            );

        int idle =
            Mathf.RoundToInt(
                workerStatus.idleTime
            );

        int total =
            active + idle;

        // NAME
        workerNameText.text =
            "Worker : " +
            workerStatus.gameObject.name;

        // STATE
        WorkerState state =
            workerStatus.currentState;

        workerStateText.text =
            "State : " +
            state.ToString();

        // ZONE
        workerZoneText.text =
            "Zone : " +
            workerStatus.currentZone;

        // TIME
        activeTimeText.text =
            "Time Active : " +
            active + "s";

        idleTimeText.text =
            "Idle Time : " +
            idle + "s";

        totalTimeText.text =
            "Total Time : " +
            total + "s";

        // =========================================
        // STATE COLOR
        // =========================================

        switch (state)
        {
            case WorkerState.Working:

                workerStateText.color =
                    workingColor;

                break;

            case WorkerState.Walking:

                workerStateText.color =
                    walkingColor;

                break;

            case WorkerState.Idle:

                workerStateText.color =
                    idleColor;

                break;

            case WorkerState.Restricted:

                workerStateText.color =
                    restrictedColor;

                break;
        }
    }
}