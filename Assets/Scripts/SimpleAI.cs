using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    [SerializeField] private float offsetSpeed = 1;
    [SerializeField] private float offsetAmount = 2;

    private CarInputHandler player = null;

    private TopDownCarController controller = null;

    private Vector3 chasePoint = Vector3.zero;

    private void Start()
    {
        player = FindObjectOfType<CarInputHandler>();

        controller = GetComponent<TopDownCarController>();    
    }

    private void Update()
    {
        chasePoint = GetChasePoint();

        Vector3 dir = chasePoint - transform.position;

        dir = transform.InverseTransformDirection(dir);

        controller.SetInputVector(dir.normalized);
    }

    private void FixedUpdate()
    {
        controller.OnFixedUpdate();
    }

    private Vector3 GetChasePoint()
    {
        Vector3 a = player.transform.position;

        float x = Mathf.Sin(Time.time * offsetSpeed) * offsetAmount;
        Vector3 dir = Vector3.right * x;
        dir = player.transform.TransformDirection(dir);

        Vector3 point = a + dir;

        return point;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(chasePoint, 0.5f);

        if (player == null) return;

        Vector2 dir = Vector2.right * offsetAmount;
        dir = player.transform.TransformDirection(dir);

        Vector2 a = player.transform.position;
        Gizmos.DrawLine(a - dir, a + dir);
    }
}
