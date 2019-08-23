using UnityEngine;
using System.Collections;
using System.Linq;
using EasyRoads3Dv3;
using UnityEditor;

[CustomEditor(typeof(EasyRoadNodes))]
public class EasyRoadNodesEditor : Editor
{
    private EasyRoadNodes _easyRoadNodes;

    public override void OnInspectorGUI()
    {
        _easyRoadNodes = (EasyRoadNodes)target;

        ERRoadNetwork network = new ERRoadNetwork();
        _easyRoadNodes.ERRoads = network.GetRoads().ToList();

        if (_easyRoadNodes.DontBuild.Count == 0)
        {
            for (int i = 0; i < _easyRoadNodes.ERRoads.Count; i++)
            {
                _easyRoadNodes.DontBuild.Add(false);
            }
        }

        if (_easyRoadNodes.Inverted.Count == 0)
        {
            for (int i = 0; i < _easyRoadNodes.ERRoads.Count; i++)
            {
                _easyRoadNodes.Inverted.Add(false);
            }
        }

        if (_easyRoadNodes.LinesCount.Count == 0)
        {
            for (int i = 0; i < _easyRoadNodes.ERRoads.Count; i++)
            {
                _easyRoadNodes.LinesCount.Add(2);
            }
        }

        if (_easyRoadNodes.DontBuild.Count != _easyRoadNodes.ERRoads.Count)
        {
            bool[] bools = new bool[_easyRoadNodes.DontBuild.Count];
            _easyRoadNodes.DontBuild.CopyTo(bools);
            _easyRoadNodes.DontBuild.Clear();

            for (int i = 0; i < _easyRoadNodes.ERRoads.Count; i++)
            {
                if (bools.Length > i)
                    _easyRoadNodes.DontBuild.Add(bools[i]);
                else
                {
                    _easyRoadNodes.DontBuild.Add(false);
                }
            }
        }

        if (_easyRoadNodes.Inverted.Count != _easyRoadNodes.ERRoads.Count)
        {
            bool[] bools = new bool[_easyRoadNodes.Inverted.Count];
            _easyRoadNodes.Inverted.CopyTo(bools);
            _easyRoadNodes.Inverted.Clear();

            for (int i = 0; i < _easyRoadNodes.ERRoads.Count; i++)
            {
                if (bools.Length > i)
                    _easyRoadNodes.Inverted.Add(bools[i]);
                else
                {
                    _easyRoadNodes.Inverted.Add(false);
                }
            }
        }
        
        if (_easyRoadNodes.OneWay.Count != _easyRoadNodes.ERRoads.Count)
        {
            bool[] bools = new bool[_easyRoadNodes.OneWay.Count];
            _easyRoadNodes.OneWay.CopyTo(bools);
            _easyRoadNodes.OneWay.Clear();

            for (int i = 0; i < _easyRoadNodes.ERRoads.Count; i++)
            {
                if (bools.Length > i)
                    _easyRoadNodes.OneWay.Add(bools[i]);
                else
                {
                    _easyRoadNodes.OneWay.Add(false);
                }
            }
        }

        if (_easyRoadNodes.LinesCount.Count != _easyRoadNodes.ERRoads.Count)
        {
            int[] ints = new int[_easyRoadNodes.LinesCount.Count];
            _easyRoadNodes.LinesCount.CopyTo(ints);
            _easyRoadNodes.LinesCount.Clear();

            for (int i = 0; i < _easyRoadNodes.ERRoads.Count; i++)
            {
                if (ints.Length > i)
                    _easyRoadNodes.LinesCount.Add(ints[i]);
                else
                {
                    _easyRoadNodes.LinesCount.Add(2);
                }
            }
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nodes count:", GUILayout.MaxWidth(230f));
        _easyRoadNodes.Count = EditorGUILayout.IntField(_easyRoadNodes.Count, GUILayout.MaxWidth(200f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Range between nodes (in meters):", GUILayout.MaxWidth(230f));
        _easyRoadNodes.NodeDefinition = EditorGUILayout.FloatField(_easyRoadNodes.NodeDefinition, GUILayout.MaxWidth(200f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Count of Bezier curve's points:", GUILayout.MaxWidth(230f));
        _easyRoadNodes.Definition = EditorGUILayout.IntField(_easyRoadNodes.Definition, GUILayout.MaxWidth(200f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Range for connected nodes search:", GUILayout.MaxWidth(230f));
        _easyRoadNodes.MaximumDistance = EditorGUILayout.FloatField(_easyRoadNodes.MaximumDistance, GUILayout.MaxWidth(200f));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10f);

        if (_easyRoadNodes.CrossRoads != null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Crossroads: " + _easyRoadNodes.CrossRoads.Count, GUILayout.MaxWidth(230f));
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Regenerate crossroads"))
            {
                _easyRoadNodes.RegenerateCrossroads();
            }
        }

        GUILayout.Space(10f);

        for (int i = 0; i < _easyRoadNodes.ERRoads.Count; i++)
        {
            GUILayout.BeginHorizontal();
            if (Selection.activeGameObject == _easyRoadNodes.ERRoads[i].roadScript.transform.gameObject)
            {
                GUILayout.Label("=>");
            }
            GUILayout.Label(_easyRoadNodes.ERRoads[i].roadScript.transform.name, GUILayout.MaxWidth(200f));
            _easyRoadNodes.DontBuild[i] = GUILayout.Toggle(_easyRoadNodes.DontBuild[i], " Ignore", GUILayout.MaxWidth(70f));
            GUILayout.Label("Lines count: ");
            _easyRoadNodes.LinesCount[i] = EditorGUILayout.IntField(_easyRoadNodes.LinesCount[i], GUILayout.MaxWidth(50f));
            _easyRoadNodes.OneWay[i] = GUILayout.Toggle(_easyRoadNodes.OneWay[i], " One way", GUILayout.MaxWidth(70f));
            GUI.enabled = _easyRoadNodes.OneWay[i];
            _easyRoadNodes.Inverted[i] = GUILayout.Toggle(_easyRoadNodes.Inverted[i], " Invert", GUILayout.MaxWidth(70f));
            GUI.enabled = true;
            GUILayout.Space(5f);

            if (GUILayout.Button("Rebuild", GUILayout.MaxWidth(100f)))
            {
                _easyRoadNodes.GenerateOneRoad(i);
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        if (GUILayout.Button(_easyRoadNodes.Generate ? "Generating..." : "Build waypoint network"))
        {
            _easyRoadNodes.GenerateNetwork();
        }
    }

}
