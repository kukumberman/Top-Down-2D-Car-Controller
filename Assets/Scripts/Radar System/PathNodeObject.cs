using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNodeObject : MonoBehaviour
{
    [SerializeField] private List<PathNodeObject> connectedNodes = new List<PathNodeObject>();

    public List<PathNodeObject> ConnectedNodes => connectedNodes;

    //private void OnDrawGizmosSelected()
    //{
    //    Vector3 a = transform.position;

    //    Gizmos.DrawSphere(a, 0.5f);

    //    Gizmos.color = Color.green;

    //    for (int i = 0, length = connectedNodes.Count; i < length; i++)
    //    {
    //        Vector3 b = connectedNodes[i].transform.position;
    //        Gizmos.DrawLine(a, b);

    //        Gizmos.DrawSphere(b, 0.5f);
    //    }
    //}
}
