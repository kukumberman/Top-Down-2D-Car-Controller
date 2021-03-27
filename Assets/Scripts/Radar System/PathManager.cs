using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] private Transform startPoint = null;
    [SerializeField] private List<PathNodeObject> nodes = new List<PathNodeObject>();

    [Header("Draw Options")]
    [SerializeField] private bool drawCenter = false;
    [SerializeField] private bool drawProjections = false;

    private Vector3 pointOnLine = Vector3.zero;
    private Vector3 cornerPoint = Vector3.zero;

    private List<PathConnection> connections = new List<PathConnection>();

    private void Start()
    {
        CreateConnections();

        Debug.Log($"Created {connections.Count} connections!");
    }

    private void Update()
    {
        Vector3 a = startPoint.position;

        GetClosestConnection(a, ref pointOnLine, ref cornerPoint);
    }

    private void CreateConnections()
    {
        for (int i = 0, length = nodes.Count; i < length; i++)
        {
            PathNodeObject a = nodes[i];

            for (int j = 0; j < a.ConnectedNodes.Count; j++)
            {
                PathNodeObject b = a.ConnectedNodes[j];

                PathConnection connection = new PathConnection(a.transform.position, b.transform.position);

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

    private void GetClosestConnection(Vector3 position, ref Vector3 projectedPosition, ref Vector3 cornerPosition)
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

                projectedPosition = projected;
                cornerPosition = t < 0.5f ? connection.A : connection.B; 
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (drawCenter)
        {
            DrawCenters();
        }

        if (startPoint == null) return;

        Vector3 a = startPoint.position;
        float radius = 0.3f;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(a, radius);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(pointOnLine, radius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(cornerPoint, radius);

        if (drawProjections)
        {
            DrawProjections(a);
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
