using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor
{
    private Road road;

    public override void OnInspectorGUI()
    {
        road = (Road) target;

        if (GUILayout.Button("Reset and recalculate lines"))
        {
            road.RecalculateLines();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Update road intersections and links"))
        {
            road.RecalculateIntersectionIndexes();
            road.RecalculateConnectedRoadLines();
        }

        EditorGUILayout.LabelField("Lines: " + road.Lines.Count);
        for (int i = 0; i < road.Lines.Count; i++)
        {
            EditorGUILayout.LabelField("Line " + i + ": " + road.Lines[i].GraphNodes.Count + " nodes");
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Connected road lines: ");
        for (int i = 0; i < road.Lines.Count; i++)
        {
            for (int j = 0; j < road.Lines[i].ConnectedRoads.Count; j++)
            {
                EditorGUILayout.LabelField("Line " + i + " by "+ road.Lines[i].ConnectedRoads[j].Key.Transform.name + " node <-> " + road.Lines[i].ConnectedRoads[j].Value.road.transform.parent.name + "." + road.Lines[i].ConnectedRoads[j].Value.GraphNodes.Count);
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Intersections and last nodes: ");
        for (int i = 0; i < road.Lines.Count; i++)
        {
            EditorGUILayout.LabelField("Line " + i + ": ");
            string s = "";
            for (int j = 0; j < road.Lines[i].RoadIntersectionsIndexes.Count; j++)
            {
                s += road.Lines[i].RoadIntersectionsIndexes[j] + " ("+ road.Lines[i].DistanceFromStart[j]+ "m.)" + (j < road.Lines[i].RoadIntersectionsIndexes.Count-1 ?  ", " : "");
            }
            EditorGUILayout.LabelField(s);
        }

        EditorGUILayout.Space();

        DrawDefaultInspector();
    }

    public void OnSceneGUI()
    {
        road = (Road)target;

        for (int j = 0;
                    j < road.Lines[0].GraphNodes.Count;
                    j += (int)(road.Lines[0].GraphNodes.Count / 4f))
        {
           Handles.color = Color.magenta;
           Handles.SphereCap(1, road.Lines[0].GraphNodes[j].transform.position, Quaternion.identity, 6f);
        }

        Handles.SphereCap(1, road.Lines[0].GraphNodes[road.Lines[0].GraphNodes.Count - 1].transform.position, Quaternion.identity, 6f);
    }
}
