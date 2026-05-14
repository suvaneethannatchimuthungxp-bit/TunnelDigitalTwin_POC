using System.Collections.Generic;

[System.Serializable]
public class ReplayFrame
{
    public float timeStamp;

    public List<WorkerFrameData> workerData =
        new List<WorkerFrameData>();
}