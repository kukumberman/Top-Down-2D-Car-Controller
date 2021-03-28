using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// UNDO doesnt works correctly

// TODO: 
// 1. Insert points by clicking
// 2. Remove connections by clicking 
// (I should recalculate connections at start and each time after node-changed events)

[CustomEditor(typeof(PathManager))]
public class PathCreatorEditor : Editor
{
    private class Mouse
    {
        public Vector3 WorldPosition;

        public bool IsHoveringNode;
        public PathNodeObject HoverNode;

        public bool IsHoveringLine;
        public Vector3 PositionOnLine;
    }

    private PathManager pathManager = null;

    private Mouse mouse = new Mouse();

    private Event currentEvent = null;

    private List<PathNodeObject> selectedNodes = new List<PathNodeObject>();

    private float nodeRadius = 0.5f;

    private Color lineColor = Color.black;
    private Color nodeIdleColor = Color.white;
    private Color nodeHoverColor = Color.yellow;
    private Color nodeSelectedColor = Color.green;

    private void OnEnable()
    {
        pathManager = target as PathManager;
    }

    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();

    //    EditorGUILayout.Space();
    //    EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
    //    //clickedPoint = EditorGUILayout.Vector3Field("Clicked Point", clickedPoint);
    //}

    private void OnSceneGUI()
    {
        currentEvent = Event.current;

        mouse.IsHoveringNode = HandleMouseHoverPoint();

        HandleMouseHoverLine();


        if (currentEvent.type == EventType.MouseMove)
        {
            GUI.changed = true;
            //Debug.Log("mouse move");
        }

        if (currentEvent.type == EventType.Repaint)
        {
            if (Application.isPlaying)
            {
                DrawConnections();
            }
            else
            {
                DrawNodes();
            }
        }
        else if (currentEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        if (currentEvent.type == EventType.MouseDown)
        {
            if (currentEvent.button == 0)
            {
                if (mouse.IsHoveringNode)
                {
                    if (currentEvent.shift)
                    {
                        DeleteNode();
                    }
                    else if (currentEvent.control)
                    {
                        ConnectNodes();
                    }
                    else
                    {
                        HandleNodeSelection();
                    }
                }
                else
                {
                    CreateNode();
                }
            }
        }
        else if (currentEvent.type == EventType.MouseDrag)
        {
            if (currentEvent.button == 0)
            {
                mouse.HoverNode.transform.position = mouse.WorldPosition;
            }
        }
    }

    private void DrawGUI()
    {
        Rect rect = new Rect(0, 0, 200, 200);
        //Handles.BeginGUI(rect);

        GUILayout.BeginArea(rect);

        if (GUILayout.Button("Click me"))
        {
            Debug.Log("clicked");
        }

        GUILayout.EndArea();

        //Handles.EndGUI();
    }

    private void DrawConnections()
    {
        List<PathConnection> connections = pathManager.GetConnections();

        for (int i = 0, length = connections.Count; i < length; i++)
        {
            PathConnection c = connections[i];

            Handles.DrawLine(c.A, c.B);

            Vector3 center = Vector3.Lerp(c.A, c.B, 0.5f);
            Vector3 dir = c.B - c.A;

            Quaternion rotation = Quaternion.LookRotation(dir);
            Handles.ConeHandleCap(0, center, rotation, 1, EventType.Repaint);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Event e = Event.current;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        float z = 0;

        float distance = (z - ray.origin.z) / ray.direction.z;

        Vector3 pos = ray.GetPoint(distance);

        pos = RoundVector(pos);

        return pos;
    }
    
    private Vector3 RoundVector(Vector3 value)
    {
        value.x = Mathf.Round(value.x * 100) / 100;
        value.y = Mathf.Round(value.y * 100) / 100;

        return value;
    }

    private void DrawNodes()
    {
        for (int i = 0; i < pathManager.Nodes.Count; i++)
        {
            PathNodeObject a = pathManager.Nodes[i];

            // draw nodes

            bool isHoverNode = mouse.IsHoveringNode && a == mouse.HoverNode;
            bool isSelectedNode = selectedNodes.Contains(a);

            Color hoverColor = isHoverNode && currentEvent.shift ? Color.red : nodeHoverColor;
            Color nodeColor = isHoverNode ? hoverColor : (isSelectedNode ? nodeSelectedColor : nodeIdleColor);

            Handles.color = nodeColor;
            Handles.DrawSolidDisc(a.transform.position, Vector3.forward, nodeRadius);

            Handles.color = Color.black;
            Handles.DrawWireDisc(a.transform.position, Vector3.forward, nodeRadius);

            // draw lines

            Handles.color = lineColor;

            for (int j = 0; j < a.ConnectedNodes.Count; j++)
            {
                PathNodeObject b = a.ConnectedNodes[j];
                Handles.DrawLine(a.transform.position, b.transform.position);
            }
        }

        if (!mouse.IsHoveringNode && mouse.IsHoveringLine)
        {
            Handles.color = Color.blue;
            Handles.DrawSolidDisc(mouse.PositionOnLine, Vector3.forward, nodeRadius * 0.5f);
        }
    }

    private bool HandleMouseHoverPoint()
    {
        mouse.WorldPosition = GetMouseWorldPosition();

        for (int i = 0; i < pathManager.Nodes.Count; i++)
        {
            PathNodeObject a = pathManager.Nodes[i];

            if (Vector3.Distance(a.transform.position, mouse.WorldPosition) <= nodeRadius)
            {
                mouse.HoverNode = a;

                return true;
            }
        }

        return false;
    }

    private void HandleMouseHoverLine()
    {
        List<PathConnection> connections = CreateConnections();
        GetClosestConnection(connections);
    }

    private void HandleNodeSelection()
    {
        if (selectedNodes.Contains(mouse.HoverNode))
        {
            selectedNodes.Remove(mouse.HoverNode);
        }
        else
        {
            selectedNodes.Add(mouse.HoverNode);
        }
    }

    private void CreateNode()
    {
        GameObject gameObject = new GameObject("[Path Node Object]");
        gameObject.transform.position = mouse.WorldPosition;
        gameObject.transform.SetParent(pathManager.transform);

        PathNodeObject node = gameObject.AddComponent<PathNodeObject>();

        pathManager.Nodes.Add(node);

        for (int i = 0; i < selectedNodes.Count; i++)
        {
            var n = selectedNodes[i];

            node.ConnectedNodes.Add(n);
            n.ConnectedNodes.Add(node);
        }

        selectedNodes.Clear();
        selectedNodes.Add(node);
    }

    private void DeleteNode()
    {
        PathNodeObject a = mouse.HoverNode;

        for (int i = a.ConnectedNodes.Count - 1; i >= 0; i--)
        {
            PathNodeObject b = a.ConnectedNodes[i];

            a.ConnectedNodes.Remove(b);
            b.ConnectedNodes.Remove(a);
        }

        if (selectedNodes.Contains(a)) selectedNodes.Remove(a);

        pathManager.Nodes.Remove(a);
        DestroyImmediate(a.gameObject);
    }

    private void ConnectNodes()
    {
        PathNodeObject a = mouse.HoverNode;

        if (selectedNodes.Contains(a))
        {
            Debug.LogWarning("selected collection already contains hover node, can't add!");
            return;
        }

        for (int i = 0; i < selectedNodes.Count; i++)
        {
            PathNodeObject b = selectedNodes[i];

            a.ConnectedNodes.Add(b);
            b.ConnectedNodes.Add(a);
        }
    }

    //

    private List<PathConnection> CreateConnections()
    {
        List<PathConnection> connections = new List<PathConnection>();

        for (int i = 0, length = pathManager.Nodes.Count; i < length; i++)
        {
            PathNodeObject a = pathManager.Nodes[i];

            for (int j = 0; j < a.ConnectedNodes.Count; j++)
            {
                PathNodeObject b = a.ConnectedNodes[j];

                PathConnection c = new PathConnection(a, b);

                if (!HasConnection(c, connections))
                {
                    connections.Add(c);
                }
            }
        }

        return connections;
    }

    private bool HasConnection(PathConnection connection, List<PathConnection> connections)
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

    private void GetClosestConnection(List<PathConnection> connections)
    {
        //PathConnection connection = null;

        float shortestDistance = int.MaxValue;

        Vector3 position = mouse.WorldPosition;
        mouse.IsHoveringLine = false;

        for (int i = 0, length = connections.Count; i < length; i++)
        {
            PathConnection c = connections[i];

            Vector3 projected = c.Project(position, out bool isInside, out float t);

            float distance = Vector3.Distance(position, projected);

            if (distance < shortestDistance && isInside)
            {
                shortestDistance = distance;
                //connection = c;

                mouse.PositionOnLine = projected;
            }
        }

        if (shortestDistance < nodeRadius)
        {
            mouse.IsHoveringLine = true;
        }
    }
}
