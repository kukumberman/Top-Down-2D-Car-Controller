using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    private CarInputHandler player = null;

    private TopDownCarController controller = null;

    private void Start()
    {
        player = FindObjectOfType<CarInputHandler>();

        controller = GetComponent<TopDownCarController>();    
    }

    private void Update()
    {
        Vector3 dir = (player.transform.position + player.transform.up * 2) - transform.position;

        dir = transform.InverseTransformDirection(dir);

        controller.SetInputVector(dir.normalized);
    }
}
