using UnityEngine;

public class WorkerSelection : MonoBehaviour
{
    public static WorkerSelection Instance;

    [Header("Selected Worker")]
    public Transform selectedWorker;
    public WorkerStatus selectedWorkerStatus;
    private Camera mainCam;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectWorker();
        }
    }

    void SelectWorker()
    {
        Ray ray =
            mainCam.ScreenPointToRay(
                Input.mousePosition
            );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Worker"))
            {
                selectedWorker =
                    hit.collider.transform;

                selectedWorkerStatus =
    hit.collider.GetComponent<WorkerStatus>();

                Debug.Log(
                    "Selected Worker : " +
                    selectedWorker.name
                );
            }
        }
    }
}