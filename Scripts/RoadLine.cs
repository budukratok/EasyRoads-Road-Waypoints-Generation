using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RoadLine : MonoBehaviour
{
    public int Id;
    public Road road;
    public List<GraphNode> GraphNodes = new List<GraphNode>();
    public List<int> RoadIntersectionsIndexes = new List<int>();
    public List<float> DistanceFromStart = new List<float>();
    public List<Link> ConnectedRoads = new List<Link>();

    public float GetDistanceForIntersectionNode(GraphNode node)
    {
        for (int i = 0; i < RoadIntersectionsIndexes.Count; i++)
        {
            if (GraphNodes[RoadIntersectionsIndexes[i]] == node)
            {
                return DistanceFromStart[i];
            }
        }

        return float.MaxValue;
    }

    public GraphNode GetNodeForConnectedRoadLine(RoadLine line)
    {
        for (int i = 0; i < ConnectedRoads.Count; i++)
        {
            if (ConnectedRoads[i].Value == line)
                return ConnectedRoads[i].Key;
        }

        return null;
    }

    public GraphNode GetNodeConnectedTo(GraphNode node)
    {
        for (int i = 0; i < ConnectedRoads.Count; i++)
        {
            if (ConnectedRoads[i].ConnectedTo == node)
                return ConnectedRoads[i].Key;
        }

        return null;
    }

    public GraphNode GetConnectedToByNode(GraphNode node)
    {
        for (int i = 0; i < ConnectedRoads.Count; i++)
        {
            if (ConnectedRoads[i].Key == node)
                return ConnectedRoads[i].ConnectedTo;
        }

        return null;
    }

    public GraphNode GetConnectedToByLine(RoadLine line)
    {
        for (int i = 0; i < ConnectedRoads.Count; i++)
        {
            if (ConnectedRoads[i].Value == line)
                return ConnectedRoads[i].ConnectedTo;
        }

        return null;
    }

    public List<RoadLine> GetLinesByLinkNode(GraphNode node)
    {
        List<RoadLine> lines = new List<RoadLine>();
        for (int i = 0; i < ConnectedRoads.Count; i++)
        {
            if (ConnectedRoads[i].Key == node)
            {
                lines.Add(ConnectedRoads[i].Value);
            }
        }

        return lines;
    }

    public int GetIndexOfNode(GraphNode node)
    {
        for (int i = 0; i < GraphNodes.Count; i++)
        {
            if (GraphNodes[i] == node)
                return i;
        }

        return -1;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
