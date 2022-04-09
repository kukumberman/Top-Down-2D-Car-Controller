using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleAI : MonoBehaviour
{
    private TopDownCarController controller = null;

    protected Vector3 chasePoint = Vector3.zero;

    private void Start()
    {
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

    protected abstract Vector3 GetChasePoint();
}
