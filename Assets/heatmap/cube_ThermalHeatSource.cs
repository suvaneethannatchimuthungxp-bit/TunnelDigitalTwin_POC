using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class cube_ThermalHeatSource : MonoBehaviour
{
    [System.Serializable]
    public struct HeatPoint
    {
        public Vector3 position;
        public float timeCreated;
        public float intensity;
    }

    [Header("Raycast")]
    public int gridPerFace = 3;
    public float rayDistance = 20f;
    public LayerMask hitMask = ~0;

    [Header("Thermal Heat")]
    public float heatRadius = 0.5f;
    public float heatIntensity = 1f;
    public float trailDuration = 10f;

    [Header("Debug")]
    public bool drawRuntimeRays = true;

    private static readonly List<cube_ThermalHeatSource> s_Instances = new List<cube_ThermalHeatSource>();
    public static IReadOnlyList<cube_ThermalHeatSource> Instances => s_Instances;

    // List of active heat points tracked in world space
    private readonly List<HeatPoint> m_ActivePoints = new List<HeatPoint>();
    public List<HeatPoint> ActivePoints => m_ActivePoints;

    const int MaxHits = 1024;

    BoxCollider box;

    void OnEnable()
    {
        s_Instances.Add(this);
    }

    void OnDisable()
    {
        s_Instances.Remove(this);
    }

    void Awake()
    {
        box = GetComponent<BoxCollider>();
    }

    void Update()
    {
        // Temporarily disable our own colliders to prevent self-collision
        var colliders = GetComponentsInChildren<Collider>();
        var disabledColliders = new List<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].enabled)
            {
                colliders[i].enabled = false;
                disabledColliders.Add(colliders[i]);
            }
        }

        // 2. Perform raycasting and collect hits into our list
        CastFace(Vector3.right);
        CastFace(Vector3.left);
        CastFace(Vector3.up);
        CastFace(Vector3.down);
        CastFace(Vector3.forward);
        CastFace(Vector3.back);

        // Re-enable our own colliders
        for (int i = 0; i < disabledColliders.Count; i++)
        {
            disabledColliders[i].enabled = true;
        }
    }

    void CastFace(Vector3 localNormal)
    {
        Vector3 half = box.size * 0.5f;
        Vector3 faceCenter = box.center + Vector3.Scale(localNormal, half);

        Vector3 tangentA;
        Vector3 tangentB;

        if (Mathf.Abs(localNormal.x) > 0.5f)
        {
            tangentA = Vector3.up;
            tangentB = Vector3.forward;
        }
        else if (Mathf.Abs(localNormal.y) > 0.5f)
        {
            tangentA = Vector3.right;
            tangentB = Vector3.forward;
        }
        else
        {
            tangentA = Vector3.right;
            tangentB = Vector3.up;
        }

        int grid = Mathf.Max(1, gridPerFace);

        for (int y = 0; y < grid; y++)
        {
            for (int x = 0; x < grid; x++)
            {
                float u = grid == 1 ? 0f : Mathf.Lerp(-1f, 1f, x / (float)(grid - 1));
                float v = grid == 1 ? 0f : Mathf.Lerp(-1f, 1f, y / (float)(grid - 1));

                Vector3 localPoint = faceCenter;
                localPoint += Vector3.Scale(tangentA, half) * u;
                localPoint += Vector3.Scale(tangentB, half) * v;

                Vector3 origin = transform.TransformPoint(localPoint);
                Vector3 direction = transform.TransformDirection(localNormal).normalized;

                // Ensure the raycast origin is always at least 0.2f units above the character's feet (transform.position.y)
                // so it starts above the floor and doesn't get stuck inside/below it.
                if (origin.y < transform.position.y + 0.2f)
                {
                    origin.y = transform.position.y + 0.2f;
                }

                origin += direction * 0.02f;

                if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance, hitMask, QueryTriggerInteraction.Ignore))
                {
                    AddOrUpdateHeatPoint(hit.point);

                    if (drawRuntimeRays)
                        Debug.DrawRay(origin, direction * hit.distance, Color.red);
                }
                else
                {
                    if (drawRuntimeRays)
                        Debug.DrawRay(origin, direction * rayDistance, Color.green);
                }
            }
        }
    }

    void AddOrUpdateHeatPoint(Vector3 pos)
    {
        // Check if there is an existing point within distance threshold to merge/update
        float threshold = heatRadius * 0.6f;
        int existingIndex = -1;
        for (int i = 0; i < m_ActivePoints.Count; i++)
        {
            if (Vector3.Distance(m_ActivePoints[i].position, pos) < threshold)
            {
                existingIndex = i;
                break;
            }
        }

        if (existingIndex >= 0)
        {
            // Refresh the lifetime and intensity of the existing point
            var pt = m_ActivePoints[existingIndex];
            pt.timeCreated = Time.time;
            pt.intensity = heatIntensity;
            m_ActivePoints[existingIndex] = pt;
        }
        else
        {
            // Add a new point if capacity permits, otherwise reuse the oldest one
            if (m_ActivePoints.Count < MaxHits)
            {
                m_ActivePoints.Add(new HeatPoint
                {
                    position = pos,
                    timeCreated = Time.time,
                    intensity = heatIntensity
                });
            }
            else
            {
                // Find oldest point to replace
                int oldestIndex = 0;
                float oldestTime = m_ActivePoints[0].timeCreated;
                for (int i = 1; i < m_ActivePoints.Count; i++)
                {
                    if (m_ActivePoints[i].timeCreated < oldestTime)
                    {
                        oldestTime = m_ActivePoints[i].timeCreated;
                        oldestIndex = i;
                    }
                }

                m_ActivePoints[oldestIndex] = new HeatPoint
                {
                    position = pos,
                    timeCreated = Time.time,
                    intensity = heatIntensity
                };
            }
        }
    }
}