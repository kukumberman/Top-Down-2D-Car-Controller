using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointFollowerAI : SimpleAI
{
    [SerializeField] private WaypointPath m_Path = null;

    [SerializeField] private int m_CurrentIndex = 0;

    [SerializeField] private float m_DistanceThreshold = 1;

    protected override Vector3 GetChasePoint()
    {
        Vector3 point = m_Path.GetPoint(m_CurrentIndex);

        if (Vector3.Distance(transform.position, point) < m_DistanceThreshold)
        {
            if (m_CurrentIndex + 1 == m_Path.Count())
            {
                m_CurrentIndex = 0;
            }
            else
            {
                m_CurrentIndex++;
            }
        }

        return point;
    }
}
