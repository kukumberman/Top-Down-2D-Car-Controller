using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [SerializeField] private Transform target = null;

    [SerializeField] private float amount = 1f;

    public void HandlePosition()
    {
        var pos = Vector3.Lerp(transform.position, target.position, amount * Time.deltaTime);
        transform.position = pos;
    }

    public void HandlePosition(Transform target)
    {
        transform.position = target.position;
    }
}
