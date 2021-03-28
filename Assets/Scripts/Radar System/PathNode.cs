using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private bool visitedAtOnce = false;

    public float Distance = 0;
    public PathNodeObject Node;

    public void SetDistance(float distance)
    {
        if (!visitedAtOnce)
        {
            Distance = distance;
            visitedAtOnce = true;
        }
        else
        {
            if (distance < Distance)
            {
                Distance = distance;
            }
        }
    }
}
