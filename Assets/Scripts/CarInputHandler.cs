using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInputHandler : MonoBehaviour
{
    [SerializeField] private SimpleCameraController cameraController = null;

    private TopDownCarController topDownCarController = null;

    private Vector2 inputVector = Vector2.zero;

    private void Awake()
    {
        topDownCarController = GetComponent<TopDownCarController>();
    }

    private void Update()
    {
        inputVector.x = Input.GetAxis("Horizontal");
        inputVector.y = Input.GetAxis("Vertical");

        topDownCarController.SetInputVector(inputVector);
    }

    private void LateUpdate()
    {
        cameraController.HandlePosition(transform);
    }

    private void FixedUpdate()
    {
        topDownCarController.OnFixedUpdate();
    }
}
