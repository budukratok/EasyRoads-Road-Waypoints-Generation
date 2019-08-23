using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Node for AI implementation
/// Нода, содержащая в себе позицию и тип вейпоинта
/// </summary>

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class GraphNode : MonoBehaviour, INode
{
    // List of connected nodes
    public bool SelectCreatedNodeOnSpawn = false;

    public List<GraphNode> ConnectedNodes = new List<GraphNode>();
    public List<Vector3> LinksBezierHelpers = new List<Vector3>();
    public int locationId; // for Unity3d inspector must be public
    public float maxSpeed;
    public RoadStatus status;

    public bool OverrideDefinition = false;
    public int OverrideDefinitionValue = 1;

    public float MaxSpeed {
        get
        {
            return maxSpeed;
        }
        set
        {
            maxSpeed = value;
        }
    }

    public Transform[] path;

    public RoadStatus Status {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }

    public int LocationId // for INode interface, must be public too
    {
        get
        {
            return locationId;
        }
    }

    public List<INode> Nodes
    {
        get
        {
            return ConnectedNodes.Select((val) => (INode) val).ToList();
        }

        set
        {
            ConnectedNodes = value.Select((val) => (GraphNode)val).ToList();
        }
    }

    public List<Vector3> Helpers
    {
        get
        {
            return LinksBezierHelpers;
        }

        set
        {
            LinksBezierHelpers = value;
        }
    }

    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    public Transform Transform
    {
        get
        {
            return transform;
        }
    }

    public INode test;

    private GraphVizualizer vizualizer = new GraphVizualizer();

    public List<GraphNode> GetConnectedNodes()
    {
        return ConnectedNodes;
    }

    public List<Vector3> GetLinksBezierHelpers()
    {
        return LinksBezierHelpers;
    }

    /// <summary>
    /// Получить дистанцию до i-ой ноды из подключённых
    /// Не могу представить где это может понадобиться
    /// </summary>
    /// <param name="i">номер ноды в списке нод текущей ноды</param>
    /// <returns>Дистанция до ноды или -1f если такой ноды нет</returns>
    public float GetDistanceToNode(int i)
    {
        if (ConnectedNodes.Count < i)
            return GetDistanceToNode(ConnectedNodes[i]);
        else
            return -1f;
    }

    public float GetDistanceToNode(INode node)
    {
        float distance = 0f;
        int nodeNumber = ConnectedNodes.IndexOf((GraphNode)node);

        int definition = (OverrideDefinition ? OverrideDefinitionValue : Bezier.BezierIterationsCount);

        for (int i = 1; i < definition + 1; i++)
        {
            distance += Vector3.Distance(
                Bezier.GetPointPosition(transform.position, ConnectedNodes[nodeNumber].transform.position,
                    LinksBezierHelpers[nodeNumber],
                    (float)(i - 1) / (float)definition),
                Bezier.GetPointPosition(transform.position, ConnectedNodes[nodeNumber].transform.position,
                    LinksBezierHelpers[nodeNumber],
                    (float)i / (float)definition)
                );
        }

        return distance;
    }

	// Use this for initialization
    void Start()
    {
        if (locationId != 0 && !gameObject.GetComponent<WarehouseWrapper>())
        {
            gameObject.AddComponent<WarehouseWrapper>();
        }

        if (gameObject.GetComponent<WarehouseWrapper>() && gameObject.GetComponent<WarehouseWrapper>().Id != locationId)
            gameObject.GetComponent<WarehouseWrapper>().Id = locationId;

        if (locationId == 0 && gameObject.GetComponent<WarehouseWrapper>())
        {
            #if UNITY_EDITOR
                DestroyImmediate(gameObject.GetComponent<WarehouseWrapper>());
            #else
                Destroy(gameObject.GetComponent<WarehouseWrapper>());
            #endif
        }

        //ConnectedNodes = new List<INode>();
        //LinksBezierHelpers = new List<Vector3>();
    }
	// Update is called once per frame
	void Update () 
    {
	    // Just.. be

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
                                LinksBezierHelpers.Add(transform.position + Vector3.one * 5f);
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
	                for (int i = 0; i < ConnectedNodes.Count; i++)
	                {
                        LinksBezierHelpers = new List<Vector3>(1);
                        LinksBezierHelpers.Add(Vector3.Lerp(transform.position, ConnectedNodes[i].transform.position, 0.5f));
	                }
	            }
	        }
	    }
	    else
	    {
            LinksBezierHelpers = null;
	    }
    #endif
	}

    # if UNITY_EDITOR
    void OnDrawGizmos()
    {
        vizualizer.VizualizeGraphNode(transform.position, Status); //draw node
        vizualizer.VizualizeGraphLinks(transform.position, ConnectedNodes, LinksBezierHelpers);
        // draw line and helpers
    }
    #endif
}
