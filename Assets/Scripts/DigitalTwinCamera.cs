using UnityEngine;

public enum CameraViewMode
{
    TopView,
    CCTVView,
    AutoView,
    FreeCam
    
}
public class DigitalTwinCamera : MonoBehaviour
{
    [Header("UI")]
    public CameraUIManager cameraUIManager;
    [Header("Movement")]
    public float moveSpeed = 12f;

    public float sprintSpeed = 20f;

    public float smoothSpeed = 8f;

    [Header("Rotation")]
    public float rotationSpeed = 120f;

    public float rotationSmoothSpeed = 6f;

    [Header("Zoom")]
    public float zoomSpeed = 80f;

    public float minZoom = 6f;

    public float maxZoom = 16f;

    [Header("Bounds")]
    public float minX = -200f;

    public float maxX = 200f;

    public float minZ = -200f;

    public float maxZ = 200f;

    [Header("Worker Focus")]
    public Vector3 focusOffset =
        new Vector3(0, 0, -4);

    public float followSpeed = 4f;

    [Header("Focus Refinement")]
    public float focusedZoom = -7f;

    public float normalZoom = -10f;

    public float focusHeight = 2f;

    public float focusTransitionSpeed = 4f;

    [Header("Camera Modes")]
    public CameraViewMode currentMode;

    [Header("Mode Targets")]
    public Transform topViewTarget;

    public Transform cctvViewTarget;

    public Transform AutoLookTarget;

    public Transform freeCameLook;

    // CAMERA
    private Camera cam;

    // POSITION
    private Vector3 targetPosition;

    private float fixedCameraY;

    // ROTATION
    private Quaternion targetRotation;

    private float rotationY;

    private float targetYaw;

    private float rotationX = 28f;

    // ZOOM
    private float currentZoomZ;

    // FOLLOW
    private Transform followTarget;

    private bool isFollowingWorker;
    private bool workerFocusTransition;
    private bool lockFixedHeight = false;

    [Header("Auto View")]
    public Transform[] autoViewPoints;

    public float autoViewMoveSpeed = 2f;

    public float autoViewWaitTime = 2f;

    private int currentAutoPoint;
    private bool isAutoViewing;

    void Start()
    {
        cam = Camera.main;

        // POSITION
        targetPosition =
            transform.position;

        fixedCameraY =
            transform.position.y;

        // ROTATION
        rotationY =
            transform.eulerAngles.y;

        targetYaw =
            rotationY;

        targetRotation =
            Quaternion.Euler(
                rotationX,
                rotationY,
                0
            );

        // ZOOM
        currentZoomZ =
            cam.transform.localPosition.z;

        transform.rotation =
            targetRotation;
    }

    void Update()
    {
        HandleMovement();

        HandleRotation();

        HandleZoom();

        HandleWorkerFocus();

        FollowWorker();

        ApplyCamera();

        HandleAutoView();
    }

    void HandleAutoView()
    {
        if (!isAutoViewing)
            return;

        if (autoViewPoints.Length == 0)
            return;

        Transform target =
            autoViewPoints[currentAutoPoint];

        // MOVE
        targetPosition =
            Vector3.Lerp(
                targetPosition,
                target.position,
                autoViewMoveSpeed *
                Time.deltaTime
            );

        // ROTATE
        targetYaw =
            Mathf.LerpAngle(
                targetYaw,
                target.eulerAngles.y,
                autoViewMoveSpeed *
                Time.deltaTime
            );

        // REACHED
        float distance =
            Vector3.Distance(
                transform.position,
                target.position
            );

        if (distance < 1f)
        {
            currentAutoPoint++;

            if (currentAutoPoint >= autoViewPoints.Length)
            {
                currentAutoPoint = 0;
            }
        }
    }
    // =====================================================
    // MOVEMENT
    // =====================================================

    void HandleMovement()
    {
        if (currentMode != CameraViewMode.FreeCam)
            return;
        // EXIT FOCUS
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D))
        {
            isFollowingWorker = false;

            currentZoomZ =
                normalZoom;
        }

        float h =
            Input.GetAxisRaw("Horizontal");

        float v =
            Input.GetAxisRaw("Vertical");

        Vector3 forward =
            transform.forward;

        Vector3 right =
            transform.right;

        // REMOVE HEIGHT
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 move =
            (forward * v +
             right * h).normalized;

        float speed =
            Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : moveSpeed;

        targetPosition +=
            move *
            speed *
            Time.deltaTime;

        // FIXED HEIGHT
        targetPosition.y =
            fixedCameraY;

        // LIMIT AREA
        targetPosition.x =
            Mathf.Clamp(
                targetPosition.x,
                minX,
                maxX
            );

        targetPosition.z =
            Mathf.Clamp(
                targetPosition.z,
                minZ,
                maxZ
            );
    }

    // =====================================================
    // ROTATION
    // =====================================================

    void HandleRotation()
    {
        //if (currentMode != CameraViewMode.FreeCam)
        //    return;
        // EXIT FOLLOW
        if (Input.GetMouseButton(1))
        {
            isFollowingWorker = false;

            currentZoomZ =
                normalZoom;
        }

        // FREE RIGHT CLICK ROTATION
        if (Input.GetMouseButton(1))
        {
            targetYaw +=
                Input.GetAxis("Mouse X") *
                rotationSpeed *
                Time.deltaTime;
        }

        // Q = -90
        if (Input.GetKeyDown(KeyCode.Q))
        {
            targetYaw -= 90f;
        }

        // E = +90
        if (Input.GetKeyDown(KeyCode.E))
        {
            targetYaw += 90f;
        }

        // SMOOTH ROTATION
        rotationY =
            Mathf.LerpAngle(
                rotationY,
                targetYaw,
                rotationSmoothSpeed *
                Time.deltaTime
            );

        // FIXED INDUSTRY ANGLE
        rotationX = 28f;

        targetRotation =
            Quaternion.Euler(
                rotationX,
                rotationY,
                0
            );
    }

    // =====================================================
    // ZOOM
    // =====================================================

    void HandleZoom()
    {
        if (currentMode != CameraViewMode.FreeCam)
            return;
        float scroll =
            Input.GetAxis(
                "Mouse ScrollWheel"
            );

        // DEAD ZONE
        if (Mathf.Abs(scroll) < 0.001f)
            return;

        isFollowingWorker = false;

        // INDUSTRY STYLE ZOOM
        currentZoomZ -=
            scroll *
            zoomSpeed *
            Time.deltaTime;

        // LIMIT
        currentZoomZ =
            Mathf.Clamp(
                currentZoomZ,
                -maxZoom,
                -minZoom
            );
    }

    // =====================================================
    // WORKER FOCUS
    // =====================================================

    void HandleWorkerFocus()
    {
        if (WorkerSelection.Instance == null)
            return;

        if (WorkerSelection.Instance.selectedWorker == null)
            return;

        followTarget =
            WorkerSelection.Instance.selectedWorker;

        workerFocusTransition = true;
        isFollowingWorker = true;

        // REFINED ZOOM
        currentZoomZ =
            focusedZoom;

        WorkerSelection.Instance.selectedWorker =
            null;
    }

    void FollowWorker()
    {
        if (!workerFocusTransition)
            return;
        if (!isFollowingWorker)
            return;

        if (followTarget == null)
            return;

        Vector3 workerPos =
            followTarget.position;

        // INDUSTRY FOCUS POSITION
        Vector3 desiredPosition =
            new Vector3(
                workerPos.x,
                fixedCameraY,
                workerPos.z
            ) +
            new Vector3(
                0,
                focusHeight,
                -4
            );

        // SMOOTH FOLLOW
        targetPosition =
            Vector3.Lerp(
                targetPosition,
                desiredPosition,
                focusTransitionSpeed *
                Time.deltaTime
            );

        // LOOK AT WORKER
        Vector3 direction =
            workerPos -
            transform.position;

        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation =
                Quaternion.LookRotation(
                    direction
                );

            targetYaw =
                Mathf.LerpAngle(
                    targetYaw,
                    lookRotation.eulerAngles.y,
                    focusTransitionSpeed *
                    Time.deltaTime
                );

            rotationY =
                Mathf.LerpAngle(
                    rotationY,
                    targetYaw,
                    focusTransitionSpeed *
                    Time.deltaTime
                );

            targetRotation =
                Quaternion.Euler(
                    rotationX,
                    rotationY,
                    0
                );
        }

        float distance =
    Vector3.Distance(
        transform.position,
        desiredPosition
    );

        if (distance < 1.5f)
        {
            workerFocusTransition = false;

            currentMode =
                CameraViewMode.FreeCam;

            lockFixedHeight = true;

            cameraUIManager.SelectFreeCam();
        }
    }

    // =====================================================
    // APPLY CAMERA
    // =====================================================

    void ApplyCamera()
    {
        // FIX HEIGHT
        if (lockFixedHeight)
        {
            targetPosition.y =
                fixedCameraY;
        }

        // POSITION
        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed *
                Time.deltaTime
            );

        // ROTATION
        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmoothSpeed *
                Time.deltaTime
            );

        // ZOOM
        Vector3 camLocal =
            cam.transform.localPosition;

        camLocal.z =
            Mathf.SmoothStep(
                camLocal.z,
                currentZoomZ,
                smoothSpeed *
                Time.deltaTime
            );

        cam.transform.localPosition =
            camLocal;
    }


    public void SwitchMode(CameraViewMode mode)
    {
        currentMode = mode;
        isAutoViewing = false;
        isFollowingWorker = false;

        currentZoomZ = normalZoom;

        switch (mode)
        {
            case CameraViewMode.TopView:

                MoveToTarget(
                    topViewTarget
                );

                break;

            case CameraViewMode.CCTVView:

                MoveToTarget(
                    cctvViewTarget
                );

                break;

            case CameraViewMode.AutoView:

                StartAutoView();
                MoveToTarget(
                    AutoLookTarget
                );

                break;

            case CameraViewMode.FreeCam:

                MoveToTarget(
                    freeCameLook
                );

                break;
        }
    }

    void StartAutoView()
    {
        isAutoViewing = true;

        currentAutoPoint = 0;
    }
    public void TopViewMode()
    {
        SwitchMode(
            CameraViewMode.TopView
        );
    }

    public void CCTVViewMode()
    {
        SwitchMode(
            CameraViewMode.CCTVView
        );
    }

    public void AutoLookMode()
    {
        SwitchMode(
            CameraViewMode.AutoView
        );
    }

    public void FreeCamMode()
    {
        lockFixedHeight = true;
        SwitchMode(
            CameraViewMode.FreeCam
        );
    }
    void MoveToTarget(Transform target)
    {
        if (target == null)
            return;

        lockFixedHeight = false;

        targetPosition =
            target.position;

        targetYaw =
            target.eulerAngles.y;
    }
}