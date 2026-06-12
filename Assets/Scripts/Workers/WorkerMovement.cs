using UnityEngine;
using UnityEngine.AI;

public class WorkerMovement : MonoBehaviour
{
    public Transform target;

    private NavMeshAgent agent;

  //  private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        //animator = GetComponent<Animator>();

        if (agent == null)
        {
            Debug.LogError(
                "NavMeshAgent Missing!"
            );
        }
    }

    void Update()
    {
        if (target != null)
        {
            agent.SetDestination(
                target.position
            );
        }

        //// Smooth movement check
        //bool isMoving =
        //    agent.velocity.magnitude > 0.15f;

        bool isMoving =
agent.velocity.magnitude > 0.05f;


    }
}