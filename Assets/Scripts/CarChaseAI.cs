using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarChaseAI : SimpleAI
{
    [SerializeField] private Transform m_Target = null;

    [SerializeField] private float offsetSpeed = 1;
    [SerializeField] private float offsetAmount = 2;

    protected override Vector3 GetChasePoint()
    {
        Vector3 a = m_Target.position;

        float x = Mathf.Sin(Time.time * offsetSpeed) * offsetAmount;
        Vector3 dir = Vector3.right * x;
        dir = m_Target.transform.TransformDirection(dir);

        Vector3 point = a + dir;

        return point;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(chasePoint, 0.5f);

        if (m_Target == null) return;

        Vector2 dir = Vector2.right * offsetAmount;
        dir = m_Target.transform.TransformDirection(dir);

        Vector2 a = m_Target.transform.position;
        Gizmos.DrawLine(a - dir, a + dir);
    }
}
