using UnityEngine;
using UnityEngine.AI;

public class WorkerStatus : MonoBehaviour
{
    // Current Worker State
    public WorkerState currentState;

    // Current Zone Name
    public string currentZone;

    // Productivity Timers
    public float activeTime;

    public float idleTime;

    // Zone Timers
    public float drillingZoneTime;

    public float breakZoneTime;

    public float restrictedZoneTime;

    public float materialZoneTime;

    // Idle Detection
    private float idleTimer;

    public float idleThreshold = 10f;

    private bool idleAlertTriggered;

    // NavMesh
    private NavMeshAgent agent;

    private Animator animator;

    public bool isReplayMode;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {

        if (isReplayMode)
        {
            agent.velocity = Vector3.zero;
            return;
        }
        // Update Zone Analytics
        UpdateZoneTimers();

        bool isMoving =
      agent.velocity.magnitude > 0.15f &&
      agent.remainingDistance > 0.3f;


        // WORKER MOVING
        if (isMoving)
        {
            idleTimer = 0f;

            idleAlertTriggered = false;

            // Walking Animation
            animator.SetInteger(
                "WorkerAction",
                1
            );

            // Active working time
            activeTime += Time.deltaTime;

            // Don't override restricted state
            if (currentState != WorkerState.Restricted)
            {
                currentState = WorkerState.Walking;
            }
        }
        // WORKER STOPPED
        else
        {
            // Immediately set stopped
            // Drilling Zone
            if (currentZone == "Drilling Zone")
            {
                currentState = WorkerState.Working;

                animator.SetInteger(
                    "WorkerAction",
                    2
                );
            }
            // Material Zone
            else if (currentZone == "Material Zone")
            {
                currentState = WorkerState.Working;

                animator.SetInteger(
                    "WorkerAction",
                    3
                );
            }
            // Break Zone
            else if (currentZone == "Break Zone")
            {
                currentState = WorkerState.Idle;

                animator.SetInteger(
                    "WorkerAction",
                    0
                );
            }
            // Restricted Zone
            else if (currentZone == "Restricted Zone")
            {
                currentState = WorkerState.Restricted;

                animator.SetInteger(
                    "WorkerAction",
                    6
                );
            }
            // Outside All Zones
            else
            {
                currentState = WorkerState.Idle;

                animator.SetInteger(
                    "WorkerAction",
                    0
                );
            }
            // Count idle timer
            idleTimer += Time.deltaTime;
            // Working Zones
            if (currentZone == "Drilling Zone" ||
               currentZone == "Material Zone")
            {
                activeTime += Time.deltaTime;
            }
            else
            {
                idleTime += Time.deltaTime;
            }
            //// Count idle duration
            //idleTime += Time.deltaTime;

            // Long inactivity
            if (idleTimer >= idleThreshold &&
     currentZone != "Drilling Zone" &&
     currentZone != "Material Zone")
            {
                currentState = WorkerState.Idle;

                // Prevent repeated alerts
                if (!idleAlertTriggered)
                {
                    idleAlertTriggered = true;

                    AlertManager.Instance.ShowAlert(
                        gameObject.name + " idle too long!",
                         AlertSeverity.Info,
                         this
                    );

                    Debug.Log(
                        gameObject.name + " idle too long!"
                    );
                }
            }
        }
    }

    // Zone Time Tracking
    void UpdateZoneTimers()
    {
        switch (currentZone)
        {
            case "Drilling Zone":
                drillingZoneTime += Time.deltaTime;
                break;

            case "Break Zone":
                breakZoneTime += Time.deltaTime;
                break;

            case "Restricted Zone":
                restrictedZoneTime += Time.deltaTime;
                break;

            case "Material Zone":
                materialZoneTime += Time.deltaTime;
                break;
        }
    }
}