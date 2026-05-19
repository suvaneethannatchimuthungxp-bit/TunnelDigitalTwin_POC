
using UnityEngine;
using TMPro;

public class WorkerLabelUI : MonoBehaviour
{
    public WorkerStatus workerStatus;

    public TMP_Text workerNameText;

    public TMP_Text workerStateText;


    public TMP_Text chainageText;

    public Transform worker;

    public float baseChainage = 2000f;

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
        if (workerStatus == null)
            return;

        // NAME
        workerNameText.text =
           workerStatus.gameObject.name;

        // STATE
        WorkerState state =
            workerStatus.currentState;

        workerStateText.text =
            "● " + state.ToString();

        if (worker != null)
        {
            float distance = Mathf.Abs(worker.position.z);

            float currentChainage = baseChainage + distance;

            int km = Mathf.FloorToInt(currentChainage / 1000);
            int meters = Mathf.FloorToInt(currentChainage % 1000);

            chainageText.text = $"CH {km}+{meters:000}";
        }

        // COLOR
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