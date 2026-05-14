using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalReplayManager : MonoBehaviour
{
    public static GlobalReplayManager Instance;

    [Header("Workers")]
    public List<WorkerStatus> workers =
        new List<WorkerStatus>();

    [Header("Replay Frames")]
    public List<ReplayFrame> replayFrames =
        new List<ReplayFrame>();

    [Header("Recording")]
    public float recordInterval = 0.1f;

    private Coroutine replayCoroutine;

    private bool isReplayPlaying;

    [Header("Replay State")]
    public int currentReplayFrame;

    public bool isPaused;

    [Header("Optimization")]
    public int maxReplayFrames = 3000;
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartCoroutine(RecordFrames());
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!isReplayPlaying)
            {
                replayCoroutine =
                    StartCoroutine(
                        PlayTimelineReplay()
                    );
            }
        }
    }

    // ==================================================
    // RECORD TIMELINE
    // ==================================================

    IEnumerator RecordFrames()
    {
        while (true)
        {
            // DON'T RECORD DURING REPLAY
            if (isReplayPlaying)
            {
                yield return null;
                continue;
            }

            ReplayFrame frame =
                new ReplayFrame();

            frame.timeStamp =
                Time.time;

            // RECORD ALL WORKERS
            foreach (WorkerStatus worker in workers)
            {
                WorkerFrameData data =
                    new WorkerFrameData();

                data.workerName =
                    worker.gameObject.name;

                data.position =
                    worker.transform.position;

                data.rotation =
                    worker.transform.rotation;

                data.state =
                    worker.currentState;

                frame.workerData.Add(data);
            }

            replayFrames.Add(frame);
            if (replayFrames.Count >
    maxReplayFrames)
            {
                replayFrames.RemoveAt(0);
            }

            yield return new WaitForSeconds(
                recordInterval
            );
        }
    }

    public void RestoreFrame(int frameIndex)
    {
        if (frameIndex < 0 ||
            frameIndex >= replayFrames.Count)
        {
            return;
        }

        ReplayFrame frame =
            replayFrames[frameIndex];

        // RESTORE ALL WORKERS
        foreach (WorkerFrameData data in frame.workerData)
        {
            GameObject workerObj =
                GameObject.Find(data.workerName);

            if (workerObj != null)
            {

                WorkerStatus status =
    workerObj.GetComponent<WorkerStatus>();

                if (status != null)
                {
                    status.isReplayMode = true;
                }
                // POSITION
                workerObj.transform.position =
     data.position;

                // ROTATION
                workerObj.transform.rotation =
                    data.rotation;

                // STATE
                if (status != null)
                {
                    status.currentState =
                        data.state;

                    Animator anim =
    workerObj.GetComponent<Animator>();

                    if (anim != null)
                    {
                        switch (data.state)
                        {
                            case WorkerState.Walking:

                                anim.SetInteger(
                                    "WorkerAction",
                                    1
                                );

                                break;

                            case WorkerState.Working:

                                // DRILLING / WORK
                                anim.SetInteger(
                                    "WorkerAction",
                                    2
                                );

                                break;

                            case WorkerState.Idle:

                                anim.SetInteger(
                                    "WorkerAction",
                                    0
                                );

                                break;

                            case WorkerState.Restricted:

                                anim.SetInteger(
                                    "WorkerAction",
                                    6
                                );

                                break;
                        }
                    }
                }
            }
        }
    }
    IEnumerator PlayTimelineReplay()
    {
        isReplayPlaying = true;

        isPaused = false;

        currentReplayFrame = 0;

        // =========================================
        // SMOOTH MOVE TO FIRST FRAME
        // =========================================

        ReplayFrame firstFrame =
            replayFrames[0];

        foreach (WorkerFrameData data in firstFrame.workerData)
        {
            GameObject workerObj =
                GameObject.Find(data.workerName);

            if (workerObj != null)
            {
                yield return StartCoroutine(
    SmoothMoveToStart(
        workerObj.transform,
        data.position
    )
);
            }
        }
        yield return new WaitForSeconds(1f);
        // =========================================
        // MAIN REPLAY LOOP
        // =========================================

        while (currentReplayFrame <
               replayFrames.Count - 1)
        {
            // PAUSE
            if (isPaused)
            {
                yield return null;
                continue;
            }

            ReplayFrame currentFrame =
                replayFrames[currentReplayFrame];

            ReplayFrame nextFrame =
                replayFrames[currentReplayFrame + 1];

            float timer = 0f;

            while (timer < recordInterval)
            {
                float t =
                    timer / recordInterval;

                // INTERPOLATE ALL WORKERS
                for (int i = 0;
                     i < currentFrame.workerData.Count;
                     i++)
                {
                    WorkerFrameData currentData =
                        currentFrame.workerData[i];

                    WorkerFrameData nextData =
                        nextFrame.workerData[i];

                    GameObject workerObj =
                        GameObject.Find(
                            currentData.workerName
                        );

                    if (workerObj != null)
                    {
                        // REPLAY MODE
                        WorkerStatus status =
                            workerObj.GetComponent<WorkerStatus>();

                        if (status != null)
                        {
                            status.isReplayMode = true;
                        }

                        // SMOOTH POSITION
                        workerObj.transform.position =
                            Vector3.Lerp(
                                currentData.position,
                                nextData.position,
                                t
                            );

                        // SMOOTH ROTATION
                        workerObj.transform.rotation =
                            Quaternion.Lerp(
                                currentData.rotation,
                                nextData.rotation,
                                t
                            );

                        // ANIMATION STATE
                        Animator anim =
                            workerObj.GetComponent<Animator>();

                        if (anim != null)
                        {
                            switch (nextData.state)
                            {
                                case WorkerState.Walking:

                                    anim.SetInteger(
                                        "WorkerAction",
                                        1
                                    );

                                    break;

                                case WorkerState.Working:

                                    anim.SetInteger(
                                        "WorkerAction",
                                        2
                                    );

                                    break;

                                case WorkerState.Idle:

                                    anim.SetInteger(
                                        "WorkerAction",
                                        0
                                    );

                                    break;

                                case WorkerState.Restricted:

                                    anim.SetInteger(
                                        "WorkerAction",
                                        6
                                    );

                                    break;
                            }
                        }
                    }
                }

                timer += Time.deltaTime;

                yield return null;
            }

            // FINALIZE FRAME
            RestoreFrame(
                currentReplayFrame + 1
            );

            currentReplayFrame++;
        }

        // =========================================
        // RETURN TO LIVE MODE
        // =========================================

        foreach (WorkerStatus worker in workers)
        {
            worker.isReplayMode = false;

            Animator anim =
                worker.GetComponent<Animator>();

            if (anim != null)
            {
                anim.SetInteger(
                    "WorkerAction",
                    0
                );
            }
        }

        isReplayPlaying = false;
    }
    IEnumerator SmoothMoveToStart(
    Transform worker,
    Vector3 targetPos)
    {
        Vector3 startPos =
            worker.position;

        float duration = 1f;

        float timer = 0f;

        while (timer < duration)
        {
            worker.position =
                Vector3.Lerp(
                    startPos,
                    targetPos,
                    timer / duration
                );

            timer += Time.deltaTime;

            yield return null;
        }

        worker.position = targetPos;
    }
    

    public bool IsReplayPlaying()
    {
        return isReplayPlaying;
    }

    public void PlayReplayFromUI()
    {
        if (!isReplayPlaying)
        {
            replayCoroutine =
                StartCoroutine(
                    PlayTimelineReplay()
                );
        }
    }

    public void StopReplay()
    {
        if (replayCoroutine != null)
        {
            StopCoroutine(replayCoroutine);
        }

        isReplayPlaying = false;

        isPaused = false;

        foreach (WorkerStatus worker in workers)
        {
            worker.isReplayMode = false;

            Animator anim =
                worker.GetComponent<Animator>();

            if (anim != null)
            {
                anim.SetInteger(
                    "WorkerAction",
                    0
                );
            }
        }
    }

    public void PauseReplay()
    {
        isPaused = !isPaused;
    }
}