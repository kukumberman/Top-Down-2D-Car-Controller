using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=pVfj6mxhdMw

[System.Serializable]
public class NodeInfo
{
    public Transform Point;
    public Vector3 PositionOnLine;
    public PathNodeObject Node;
}


public class PathManager : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer = null;

    [SerializeField] private NodeInfo startNode = new NodeInfo();
    [SerializeField] private NodeInfo targetNode = new NodeInfo();

    [SerializeField] private List<PathNodeObject> nodes = new List<PathNodeObject>();

    [Header("Draw Options")]
    [SerializeField] private bool drawGizmos = false;
    [SerializeField] private bool drawCenter = false;
    [SerializeField] private bool drawProjections = false;

    private List<PathConnection> connections = new List<PathConnection>();

    public List<PathConnection> GetConnections() => connections;

    public List<PathNodeObject> Nodes => nodes;

    private void Start()
    {
        CreateConnections();

        Debug.Log($"Created {connections.Count} connections!");
    }

    private void Update()
    {
        GetClosestConnection(startNode.Point.position, startNode);
        GetClosestConnection(targetNode.Point.position, targetNode);

        //lineRenderer.positionCount = 3;
        //lineRenderer.SetPositions(new Vector3[] { a, pointOnLine, cornerPoint });
    }

    private void CreateConnections()
    {
        for (int i = 0, length = nodes.Count; i < length; i++)
        {
            PathNodeObject a = nodes[i];

            for (int j = 0; j < a.ConnectedNodes.Count; j++)
            {
                PathNodeObject b = a.ConnectedNodes[j];

                PathConnection connection = new PathConnection(a, b);

                if (!HasConnection(connection))
                {
                    connections.Add(connection);
                }
            }
        }
    }

    private bool HasConnection(PathConnection connection)
    {
        for (int i = 0, length = connections.Count; i < length; i++)
        {
            PathConnection c = connections[i];

            if (c.IsEqual(connection))
            {
                return true;
            }
        }

        return false;
    }

    private void GetClosestConnection(Vector3 position, NodeInfo nodeInfo)
    {
        PathConnection connection = null;

        float shortestDistance = int.MaxValue;

        for (int i = 0, length = connections.Count; i < length; i++)
        {
            PathConnection c = connections[i];

            Vector3 projected = c.Project(position, out bool isInside, out float t);

            float distance = Vector3.Distance(position, projected);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                connection = c;

                nodeInfo.PositionOnLine = projected;
                nodeInfo.Node = t < 0.5f ? connection.NodeA : connection.NodeB;

                //projectedPosition = projected;
                //cornerPosition = t < 0.5f ? connection.A : connection.B; 
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        DrawNodes();

        if (drawCenter)
        {
            DrawCenters();
        }

        //if (startPoint == null) return;

        float radius = 0.3f;

        DrawNodeInfo(startNode, radius);
        DrawNodeInfo(targetNode, radius);

        if (drawProjections)
        {
            Vector3 a = startNode.Point.position;
            DrawProjections(a);
        }
    }

    private void DrawNodeInfo(NodeInfo nodeInfo, float radius)
    {
        if (nodeInfo.Point == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nodeInfo.Point.position, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(nodeInfo.PositionOnLine, radius);

        if (nodeInfo.Node == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(nodeInfo.Node.transform.position, radius);
    }

    private void DrawNodes()
    {
        Color color = Color.black;
        //color.a = 0.2f;

        Gizmos.color = color;

        for (int i = 0; i < nodes.Count; i++)
        {
            PathNodeObject a = nodes[i];

            for (int j = 0; j < a.ConnectedNodes.Count; j++)
            {
                PathNodeObject b = a.ConnectedNodes[j];
                Gizmos.DrawLine(a.transform.position, b.transform.position);
            }
        }
    }

    private void DrawCenters()
    {
        for (int i = 0, length = connections.Count; i < length; i++)
        {
            PathConnection c = connections[i];
            Vector3 center = Vector3.Lerp(c.A, c.B, 0.5f);
            Gizmos.DrawSphere(center, 0.2f);
        }
    }

    private void DrawProjections(Vector3 position)
    {
        for (int i = 0, length = connections.Count; i < length; i++)
        {
            PathConnection connection = connections[i];

            Vector3 result = connection.Project(position, out bool isInside, out float t);

            Gizmos.color = isInside ? Color.yellow : Color.red;

            Gizmos.DrawLine(position, result);
            Gizmos.DrawSphere(result, 0.2f);
        }
    }
}
