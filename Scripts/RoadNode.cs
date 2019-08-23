using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public enum RoadStatusType
{
    Asphalte,
    OldRoad,
    Dirt
}

public enum RoadType
{
    Normal,
    OneWay
}

public enum RoadConnectType
{
    Normal,
    Crossroad3,
    Crossroad4,
    Stop
}

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class RoadNode : MonoBehaviour {

    public List<RoadNode> ConnectedNodes = new List<RoadNode>();
    public List<Vector3> LinksBezierHelpers = new List<Vector3>();
    public List<int>  IterationsCount = new List<int>();
    private RoadVizualiser vizualizer = new RoadVizualiser();
    public RoadConnectType roadConnectType = RoadConnectType.Normal;
    public RoadType roadType = RoadType.Normal;
    public RoadStatusType roadStatus = RoadStatusType.Asphalte;


    public bool rebuildRoadPath = true;

	// Use this for initialization
	void Start ()
	{

	}

    public void RebuildMesh()
    {
        
#if UNITY_EDITOR
        gameObject.GetComponent<MeshFilter>().sharedMesh = null;
        vizualizer.GenerateRoadMesh(GetComponent<RoadNode>(), transform.position, ConnectedNodes, LinksBezierHelpers);
        rebuildRoadPath = false;
#endif
    }
	
	// Update is called once per frame
	void Update () 
    {
#if UNITY_EDITOR
        if (ConnectedNodes.Count != 0)
        {
            if (LinksBezierHelpers.Count != ConnectedNodes.Count)
            {
                if (LinksBezierHelpers.Count != 0)
                {
                    List<Vector3> helpersBefore = new List<Vector3>(LinksBezierHelpers.Count);
                    for (int i = 0; i < LinksBezierHelpers.Count; i++)
                    {
                        helpersBefore.Add(LinksBezierHelpers[i]);
                    }

                    LinksBezierHelpers = new List<Vector3>(ConnectedNodes.Count);
                    for (int i = 0; i < ConnectedNodes.Count; i++)
                    {
                        if (i > helpersBefore.Count - 1)
                        {
                            if (ConnectedNodes[i] != null)
                            {
                                LinksBezierHelpers.Add(Vector3.Lerp(transform.position, ConnectedNodes[i].transform.position, 0.5f));
                            }
                            else
                            {
                                LinksBezierHelpers.Add(Vector3.Lerp(transform.position, ConnectedNodes[i].transform.position, 0.5f));
                            }
                        }
                        else
                        {
                            LinksBezierHelpers.Add(helpersBefore[i]);
                        }
                    }
                }
                else
                {
                    LinksBezierHelpers = new List<Vector3>(1);
                    for (int i = 0; i < ConnectedNodes.Count; i++)
                    {
                        LinksBezierHelpers.Add(Vector3.Lerp(transform.position, ConnectedNodes[i].transform.position, 0.5f));
                    }
                }
            }

            if (IterationsCount.Count != ConnectedNodes.Count)
            {
                if (IterationsCount.Count != 0)
                {
                    List<int> IterationsCountBefore = new List<int>(IterationsCount.Count);
                    for (int i = 0; i < IterationsCount.Count; i++)
                    {
                        IterationsCountBefore.Add(IterationsCount[i]);
                    }

                    IterationsCount = new List<int>(ConnectedNodes.Count);
                    for (int i = 0; i < ConnectedNodes.Count; i++)
                    {
                        if (i > IterationsCount.Count - 1)
                        {
                            if (ConnectedNodes[i] != null)
                            {
                                IterationsCount.Add(30);
                            }
                            else
                            {
                                IterationsCount.Add(30);
                            }
                        }
                        else
                        {
                            IterationsCount.Add(IterationsCountBefore[i]);
                        }
                    }
                }
                else
                {
                    IterationsCount = new List<int>(1);
                    IterationsCount.Add(30);
                }
            }
        }
        else
        {
            LinksBezierHelpers = null;
            IterationsCount = null;
        }
#endif
	}

# if UNITY_EDITOR
    void OnDrawGizmos()
    {
        vizualizer.VizualizeRoadNode(transform.position); //draw node
        vizualizer.VizualizeRoadLinks(GetComponent<RoadNode>(), transform.position, ConnectedNodes, LinksBezierHelpers);
        // draw line and helpers
    }
#endif
}
