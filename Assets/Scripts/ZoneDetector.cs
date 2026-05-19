using UnityEngine;

public class ZoneDetector : MonoBehaviour
{
    public string zoneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Worker"))
        {
            Debug.Log(other.name + " entered " + zoneName);

            WorkerStatus workerStatus =
                other.GetComponent<WorkerStatus>();

            if (workerStatus != null)
            {
                // Update current zone
                workerStatus.currentZone = zoneName;

                // Update worker state
                if (zoneName == "Drilling Zone")
                {
                    workerStatus.currentState =
                        WorkerState.Working;
                }

                if (zoneName == "Restricted Zone")
                {
                    workerStatus.currentState =
                        WorkerState.Restricted;

                    AlertManager.Instance.ShowAlert(
                        other.name + " entered Restricted Zone!",
                         AlertSeverity.Warning,
                         workerStatus
                    );
                }

                if (zoneName == "Break Zone")
                {
                    workerStatus.currentState =
                        WorkerState.Idle;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Worker"))
        {
            Debug.Log(
                other.name + " exited " + zoneName
            );

            WorkerStatus workerStatus =
                other.GetComponent<WorkerStatus>();

            if (workerStatus != null)
            {
                // Reset Zone
                workerStatus.currentZone = "";

                // Set Idle State
                workerStatus.currentState =
                    WorkerState.Idle;
            }

            Animator animator =
                other.GetComponent<Animator>();

            if (animator != null)
            {
                animator.SetInteger(
                    "WorkerAction",
                    0
                );
            }
        }
    }
}