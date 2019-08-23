using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Link
{
    public GraphNode Key;
    
    public GraphNode ConnectedTo;
    
    public RoadLine Value;

    public Link()
    {
        
    } 

    public Link(GraphNode key, GraphNode connectedTo, RoadLine value)
    {
        Key = key;
        Value = value;
        ConnectedTo = connectedTo;
    }
}

[ExecuteInEditMode]
public class Road : MonoBehaviour
{
    public int Id;
    public List<RoadLine> Lines = new List<RoadLine>();
    public bool IsCanBeBlocked;
    public bool IsBlocked;

	// Use this for initialization
	void Start ()
	{
	    //transform.name = "RoadLine_" + transform.parent.name;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void RecalculateIntersectionIndexes()
    {
        if (Lines != null)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                Lines[i].RoadIntersectionsIndexes.Clear();
                for (int j = 0; j < Lines[i].GraphNodes.Count; j++)
                {
                    if (Lines[i].GraphNodes[j].ConnectedNodes.Count > 1f || j == Lines[i].GraphNodes.Count - 1)
                    {
                        Lines[i].RoadIntersectionsIndexes.Add(j);

                        float distance = 0f;
                        for (int k = 0; k < j; k++)
                        {
                            distance += Lines[i].GraphNodes[k].GetDistanceToNode(Lines[i].GraphNodes[k + 1]);
                        }
                        Lines[i].DistanceFromStart.Add(distance);
                    }
                }
                Lines[i].Id = i;
            }
        }
    }

    public void RecalculateConnectedRoadLines()
    {
        for (int i = 0; i < Lines.Count; i++)
        {
            Lines[i].ConnectedRoads.Clear();
            for (int j = 0; j < Lines[i].RoadIntersectionsIndexes.Count; j++)
            {
                Lines[i].ConnectedRoads.AddRange(GetConnectedRoadLines(this, Lines[i], Lines[i].ConnectedRoads,
                    Lines[i].GraphNodes[Lines[i].RoadIntersectionsIndexes[j]]));
            }
            Lines[i].ConnectedRoads = Lines[i].ConnectedRoads.Distinct().ToList();
        }
    }

    public List<Link> GetConnectedRoadLines(Road road, RoadLine line, List<Link> connectedLines, GraphNode node)
    {
        for (int i = 0; i < node.ConnectedNodes.Count; i++)
        {
            if (road.GetRoadLineByNode(node.ConnectedNodes[i]) != line)
            {
                Road tempRoad = GlobalAINavigator.GetRoadByNode(node, node.ConnectedNodes[i]);
                if (tempRoad != null)
                {
                    RoadLine tempLine = tempRoad.GetRoadLineByNode(node.ConnectedNodes[i]);
                    if (tempLine != null)
                    {
                        connectedLines.Add(new Link(node, node.ConnectedNodes[i], tempLine));
                    }
                }
            }
                
        }
        
        return connectedLines;
    }

    public RoadLine GetRoadLineByNode(INode node, bool deep = false)
    {
        for (int i = 0; i < Lines.Count; i++)
        {
            for (int j = 0; j < Lines[i].GraphNodes.Count; j++)
            {
                if (node == (INode)Lines[i].GraphNodes[j])
                    return Lines[i];
            }
        }

        if (deep)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                for (int j = 0; j < Lines[i].RoadIntersectionsIndexes.Count; j++)
                {
                    for (int k = 0;
                        k < Lines[i].GraphNodes[Lines[i].RoadIntersectionsIndexes[j]].ConnectedNodes.Count;
                        k++)
                    {
                        if ((INode) Lines[i].GraphNodes[Lines[i].RoadIntersectionsIndexes[j]].ConnectedNodes[k] == node)
                        {
                            return Lines[i];
                        }
                    }
                }
            }
        }

        return null;
    }

    public void RecalculateLines()
    {
        List<GameObject> childObjects = new List<GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<GraphNode>())
            {
                childObjects.Add(transform.GetChild(i).gameObject);
            }
        }

        if (Lines.Count != 0)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i] != null)
                {
                    for (int j = 0; j < Lines[i].transform.childCount; j++)
                    {
                        Lines[i].transform.GetChild(j).SetParent(transform);
                    }

                    DestroyImmediate(Lines[i].gameObject);
                }
            }
        }

        Lines = new List<RoadLine>();
        GraphNode graphNode;

        for (int i = 0; i < childObjects.Count; i++)
        {
            graphNode = childObjects[i].GetComponent<GraphNode>();
            if (!IsNodeInLines(graphNode))
            {
                GameObject tempGo = new GameObject("Line"+(Lines.Count), typeof(RoadLine));
                tempGo.transform.SetParent(transform);
                Lines.Add(tempGo.GetComponent<RoadLine>());
                Lines[Lines.Count - 1].Id = (Lines.Count - 1);
                Lines[Lines.Count - 1].road = this;
                Lines[Lines.Count - 1].GraphNodes = GetAllConnectedChilds(graphNode, childObjects);
            }
        }

        for (int i = 0; i < Lines.Count; i++)
        {
            for (int j = 0; j < Lines[i].GraphNodes.Count; j++)
            {
                Lines[i].GraphNodes[j].Transform.SetParent(Lines[i].transform);
            }
        }

        RecalculateIntersectionIndexes();
    }

    public bool IsNodeInLines(GraphNode node)
    {
        for (int i = 0; i < Lines.Count; i++)
        {
            if (Lines[i].GraphNodes.Contains(node))
                return true;
        }

        return false;
    }

    public List<GraphNode> GetAllConnectedChilds(GraphNode node, List<GameObject> childs)
    {
        List<GraphNode> nodes = new List<GraphNode>();
        GraphNode curentNode = node, prevNode = null;

        while (childs.Contains(curentNode.gameObject))
        {
            nodes.Add(curentNode);

            prevNode = curentNode;

            for (int i = 0; i < curentNode.ConnectedNodes.Count; i++)
            {
                if (childs.Contains(curentNode.ConnectedNodes[i].gameObject) &&
                    curentNode.ConnectedNodes[i].Transform.name.Contains("NavNode"))
                {
                    curentNode = curentNode.ConnectedNodes[i];
                    break;
                }
            }

            if (curentNode == prevNode)
            {
                if(curentNode.ConnectedNodes.Count != 0)
                    curentNode = curentNode.ConnectedNodes[0];
                else
                    break;
            }
        }

        return nodes;
    } 
}
