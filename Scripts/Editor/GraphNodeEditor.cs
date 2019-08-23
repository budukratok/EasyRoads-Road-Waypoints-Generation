using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GraphNode))]
[CanEditMultipleObjects]
public class GraphNodeEditor : Editor
{
    private const string AppTitle = "GraphNodeEditor";
    private float _gizmoRadius = 1f;

    private GraphNode node;
    private GameObject newSelected = null;

    void OnSceneGUI()
    {
        node = target as GraphNode;

        if (node && node.ConnectedNodes != null)
        {
            Handles.color = Color.white;
            for (int i = 0; i < node.ConnectedNodes.Count; i++)
            {
                if (node.LinksBezierHelpers[i] != null)
                {
                    node.LinksBezierHelpers[i] = Handles.FreeMoveHandle(node.LinksBezierHelpers[i], Quaternion.identity,
                        2f, Vector3.zero, Handles.SphereCap);
                }
            }

            Event e = Event.current;
            DrawHandles(true);
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.I)
                {
                    SpawnOrConnectGraphNode();
                }

                if (e.keyCode == KeyCode.O)
                {
                    DeleteTargetNode();
                }

                if (e.keyCode == KeyCode.P)
                {
                    DisconnectTargetNode();
                }
            }
        }
        
        if (GUI.changed)
            EditorUtility.SetDirty(target);

        if (newSelected != null)
        {
            Selection.activeGameObject = newSelected;
            newSelected = null;
        }
    }

    public void SpawnOrConnectGraphNode()
    {
        RaycastHit hit;
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            GraphNode newNode = null;
            GraphNode[] objects = GameObject.FindObjectsOfType<GraphNode>();

            for (int i = 0; i < objects.Length; i++)
            {
                if (Vector3.Distance(hit.point, objects[i].transform.position) < 3f)
                {
                    newNode = objects[i];
                    break;
                }
            }

            if (newNode == null)
            {
                GameObject newGo = new GameObject("GraphNode", typeof (GraphNode));
                newNode = newGo.GetComponent<GraphNode>();

                newGo.transform.position = hit.point + Vector3.up;
                newGo.transform.SetParent(node.Transform.parent);

                newNode.MaxSpeed = node.MaxSpeed;
            }

            if (node.LinksBezierHelpers == null)
                node.LinksBezierHelpers = new List<Vector3>();

            if (node.ConnectedNodes == null)
                node.ConnectedNodes = new List<GraphNode>();

            node.LinksBezierHelpers.Add(Bezier.CenterOf(newNode.transform.position, node.transform.position));
            node.ConnectedNodes.Add(newNode);

            if (node.SelectCreatedNodeOnSpawn)
            {
                newNode.SelectCreatedNodeOnSpawn = true;
                newSelected = newNode.gameObject;
            }
        }
    }

    public void DeleteTargetNode()
    {
        RaycastHit hit;
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            for (int i = 0; i < node.ConnectedNodes.Count; i++)
            {
                if (Vector3.Distance(hit.point, node.ConnectedNodes[i].Transform.position) < 3f)
                {
                    GraphNode delNode = node.ConnectedNodes[i];
                    node.LinksBezierHelpers.RemoveAt(node.ConnectedNodes.IndexOf(delNode));
                    node.ConnectedNodes.Remove(delNode);

                    DestroyImmediate(delNode.gameObject);
                }
            }
        }
    }

    public void DisconnectTargetNode()
    {
        RaycastHit hit;
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            for (int i = 0; i < node.ConnectedNodes.Count; i++)
            {
                if (Vector3.Distance(hit.point, node.ConnectedNodes[i].Transform.position) < 3f)
                {
                    GraphNode delNode = node.ConnectedNodes[i];
                    node.LinksBezierHelpers.RemoveAt(node.ConnectedNodes.IndexOf(delNode));
                    node.ConnectedNodes.Remove(delNode);
                }
            }
        }
    }

    public void DrawHandles(bool add)
    {
        RaycastHit hit;
        var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            Handles.color = add ? Color.green : Color.red;
            Handles.CircleCap(1, hit.point, Quaternion.LookRotation(hit.normal), _gizmoRadius);
        }
    }
}
