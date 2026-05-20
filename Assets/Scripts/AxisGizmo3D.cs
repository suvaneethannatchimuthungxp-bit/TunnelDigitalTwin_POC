using UnityEngine;

public class AxisGizmo3D : MonoBehaviour
{
    public Transform targetCamera;

    void LateUpdate()
    {
        transform.rotation =
            Quaternion.Inverse(
                targetCamera.rotation
            );
    }
}