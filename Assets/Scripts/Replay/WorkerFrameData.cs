using UnityEngine;

[System.Serializable]
public class WorkerFrameData
{
    public string workerName;

    public Vector3 position;

    public Quaternion rotation;

    public WorkerState state;
}