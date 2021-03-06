using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [SerializeField] private Transform target = null;

    [SerializeField] private float amount = 1f;

    private void LateUpdate()
    {
        var pos = Vector3.Lerp(transform.position, target.position, amount * Time.deltaTime);
        transform.position = pos;
    }
}
