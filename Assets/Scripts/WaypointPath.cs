using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [SerializeField] private List<Transform> m_Waypoints = new List<Transform>();

    [Header("Debug")]
    [SerializeField] private bool m_DrawGizmo = true;

    public Vector3 GetPoint(int index)
    {
        return m_Waypoints[index].position;
    }

    public int Count()
    {
        return m_Waypoints.Count;
    }

    private void OnDrawGizmos()
    {
        if (!m_DrawGizmo)
        {
            return;
        }

        for (int i = 0; i < m_Waypoints.Count; i++)
        {
            var a = m_Waypoints[i];
            var b = m_Waypoints[(i + 1) % m_Waypoints.Count];
            Gizmos.DrawLine(a.position, b.position);
        }
    }
}
