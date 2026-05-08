using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
    public Transform target;

    public float height = 80f;

    void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 pos =
            target.position;

        pos.y = height;

        transform.position = pos;
    }
}