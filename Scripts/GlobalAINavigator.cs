using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// ReSharper disable CheckNamespace

public class NodeParams
{
    public bool Mark;
    public bool DontTryOverrideWeight;
    public float Weight = float.MaxValue;
    public List<INode> Nodes = new List<INode>();
}

// ReSharper disable once InconsistentNaming
public class GlobalAINavigator
{
    public List<Unit> Ai;
    public List<INode> Nodes;
    public List<Road> Roads;
    public List<Road> BlockableRoads; 
    
    public GlobalAINavigator()
    {
        Ai = new List<Unit>();
        Nodes = new List<INode>();
        Roads = new List<Road>();

        // ReSharper disable once AccessToStaticMemberViaDerivedType
        Nodes = (GameObject.FindObjectsOfType(typeof(GraphNode)) as INode[]).ToList();
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].Transform.GetComponent<WarehouseWrapper>())
                Nodes[i].Transform.name = "_AINode_" + i + "_WareHouse";
            else
                Nodes[i].Transform.name = "_AINode_" + i;
        }

        Roads = (GameObject.FindObjectsOfType(typeof(Road)) as Road[]).ToList();
        for (int i = 0; i < Roads.Count; i++)
        {
            Roads[i].Id = i;
        }
        //TODO: remove from release
        #region road blocking debug
        Roads[1].IsCanBeBlocked = true;
        Roads[3].IsCanBeBlocked = true;
        Roads[5].IsCanBeBlocked = true;
        Roads[8].IsCanBeBlocked = true;
        Roads[11].IsCanBeBlocked = true;
        Roads[17].IsCanBeBlocked = true;
        #endregion road blocking debug
        BlockableRoads = Roads
            .Where(road => road.IsCanBeBlocked)
            .ToList();

        //TODO: for what purpose?
        //List<string> names = (from node in GameObject.FindObjectsOfType(typeof (GraphNode)) select node.name).ToList();

        foreach (Unit unit in ActualInfo.Instance.units.Where(unit => unit.Id != 0))
        {
            unit.TargetNode = GetNodeByLocationId(unit.TargetLocationId);
        }

        Debug.Log("Загружено " + Nodes.Count + " нод");

        //GenerateRouteToNode(nodes[names.IndexOf("1")], nodes[names.IndexOf("11")]);
    }

    /// <summary>
    /// Get waypoint's position by number
    /// </summary>
    /// <param name="current">Current node</param>
    /// <param name="previous">Previous node</param>
    /// <param name="waypoint">Waypoint's number</param>
    /// <returns>Waypoint's position</returns>
    public Vector3 GetWaypointPosition(INode current, INode previous, int waypoint)
    {
        try
        {
            //Debug.Log(current.Transform.name + " " + previous.Transform.name);
            if ((float)waypoint / (float)Bezier.BezierIterationsCount <= 1f)
            {
                return Bezier.GetPointPosition(previous.Transform.position, current.Transform.position,
                    previous.GetLinksBezierHelpers()[previous.GetConnectedNodes().IndexOf((GraphNode)current)],
                    ((float)waypoint / (float)Bezier.BezierIterationsCount));
            }
            return current.Transform.position;
        }
        catch
        {
            return current.Transform.position;
        }

    }

    /// <summary>
    /// Find the optimal path through the nodes network
    /// Dijkstra algorithm
    /// </summary>
    /// <param name="startNode">Start-node</param>
    /// <param name="targetNode">Target-node</param>
    /// <param name="result">Action, which accept the results</param>
    /// <returns></returns>
    public IEnumerator GenerateRouteToNode(INode startNode, INode targetNode, Action<List<INode>> result)
    {
        //Debug.Log(startNode.Transform.name + " " + targetNode.Transform.name);

        result(GetRouteToNode(startNode, targetNode));
        yield return null;
    }

    //public RoadLine GetRoadLineFromRoadPool(RoadLine line)
    //{
    //    return Roads[line.road.Id].Lines[line.Id];
    //}

    public float GetDistance(List<INode> nodes)
    {
        float distance = 0f;

        for (int i = 0; i < nodes.Count-1; i++)
        {
            distance += Vector3.Distance(nodes[i].Transform.position, nodes[i + 1].Transform.position);
        }

        return distance;
    }

    public List<INode> GetRouteToNode(INode start, INode end)
    {
        List<INode> nodePath = new List<INode>();
        List<RoadLine> nextRoads = new List<RoadLine>();
        
        bool enough = false;

        Road startNodeRoad = GetRoadByNode(start, start);
        Road endNodeRoad = GetRoadByNode(start, end);

        if (startNodeRoad == null)
        {
            startNodeRoad = GetRoadByNode(start, start, true);
        }

        if (endNodeRoad == null)
        {
            endNodeRoad = GetRoadByNode(start, end, true);
        }

        //Debug.Log(startNodeRoad + " " + start + " -> " + endNodeRoad + " " + end);

        RoadLine startNodeLine = startNodeRoad.GetRoadLineByNode(start);
        RoadLine endNodeLine = endNodeRoad.GetRoadLineByNode(end);

        if(startNodeLine == null)
            startNodeLine = startNodeRoad.GetRoadLineByNode(start, true);

        if(endNodeLine == null)
            endNodeLine = endNodeRoad.GetRoadLineByNode(end, true);

        Dictionary<RoadLine, float> Weights = new Dictionary<RoadLine, float>();
        

        Dictionary<RoadLine, List<RoadLine>> Paths = new Dictionary<RoadLine, List<RoadLine>>();
        Paths.Add(startNodeLine, new List<RoadLine>());
        //Paths[startNodeLine].Add(startNodeLine);

        List<RoadLine> Marked = new List<RoadLine>();

        RoadLine currentRoadLine = startNodeLine;
        Weights.Add(currentRoadLine, 0f);
        // алгоритм - строим по Дейкстре кратчайший путь, используя за логические узлы - RoadLine
        // далее по RoadLine в зависимости от пути собираем список нод и возвращаем

        while (!enough)
        {
            if(!Marked.Contains(currentRoadLine))
                Marked.Add(currentRoadLine);
            for (int i = 0; i < currentRoadLine.ConnectedRoads.Count; i++)
            {
                RoadLine line = currentRoadLine.ConnectedRoads[i].Value;

                if(line == null)
                    Debug.Log(currentRoadLine.road.transform.name + " " + i);

                if (!Weights.ContainsKey(line))
                {
                    Weights.Add(line, float.MaxValue);
                }

                float distanceToInters = currentRoadLine.GetDistanceForIntersectionNode(currentRoadLine.GetNodeForConnectedRoadLine(line));
                if ((distanceToInters + Weights[currentRoadLine]) < Weights[line])
                {
                    if (line != currentRoadLine)
                    {
                        Weights[line] = distanceToInters + Weights[currentRoadLine];

                        if (Paths.ContainsKey(line))
                            Paths[line].Clear();
                        else
                            Paths.Add(line, new List<RoadLine>());

                        if (Marked.Contains(line))
                            Marked.Remove(line);

                        if(!nextRoads.Contains(line))
                            nextRoads.Add(line);

                        Paths[line].AddRange(Paths[currentRoadLine]);
                        Paths[line].Add(currentRoadLine);
                    }
                }
            }


            RoadLine tempCurrentRoadLine = currentRoadLine;//currentRoadLine.ConnectedRoads[0].Value;
            float min = float.MaxValue;

            for (int i = 0; i < currentRoadLine.ConnectedRoads.Count; i++)
            {
                if (Weights[currentRoadLine.ConnectedRoads[i].Value] < min &&
                    !Marked.Contains(currentRoadLine.ConnectedRoads[i].Value))
                {
                    min = Weights[currentRoadLine.ConnectedRoads[i].Value];
                    tempCurrentRoadLine = currentRoadLine.ConnectedRoads[i].Value;
                }
            }

            if (currentRoadLine != tempCurrentRoadLine && currentRoadLine != endNodeLine)
            {
                currentRoadLine = tempCurrentRoadLine;
            }
            else
            {
                for (int i = 0; i < currentRoadLine.ConnectedRoads.Count; i++)
                {
                    if (currentRoadLine.ConnectedRoads[i].Value != tempCurrentRoadLine &&
                        !Marked.Contains(currentRoadLine.ConnectedRoads[i].Value))
                    {
                        if(!nextRoads.Contains(currentRoadLine.ConnectedRoads[i].Value))
                            nextRoads.Add(currentRoadLine.ConnectedRoads[i].Value);
                    }
                }

                if (nextRoads.Count > 0)
                {
                    tempCurrentRoadLine = nextRoads.FirstOrDefault();
                    nextRoads.Remove(tempCurrentRoadLine);
                    currentRoadLine = tempCurrentRoadLine; 
                }
                else
                {
                    enough = true;
                }
            }
        }

        if (Paths.ContainsKey(endNodeLine))
        {
            Paths[endNodeLine].Add(endNodeLine);
            List<RoadLine> path = Paths[endNodeLine];
            int endIndex, startIndex;
            
            for (int i = 0; i < path.Count; i++)
            {
                if (path[i] == startNodeLine)
                {
                    if(path.Count > 1)
                        endIndex = path[i].GetIndexOfNode(path[i].GetNodeForConnectedRoadLine(path[i + 1]));
                    else
                        endIndex = path[i].GetIndexOfNode((GraphNode)end);

                    startIndex = path[i].GetIndexOfNode((GraphNode) start);

                    if (startIndex == -1)
                    {
                        for (int j = 0; j < start.GetConnectedNodes().Count; j++)
                        {
                            if (GetRoadByNode(start, start.GetConnectedNodes()[j]) == path[i])
                            {
                                startIndex = path[i].GetIndexOfNode(start.GetConnectedNodes()[j]);
                            }
                        }
                    }

                    if (startIndex == -1)
                    {
                        startIndex = 0;
                    }

                    // start 29  end 6
                    if (startIndex > endIndex)
                    {
                        int temp = startIndex; //29
                        startIndex = (path[i].GraphNodes.Count - 1) - startIndex; //29-29=0
                        endIndex = temp - endIndex - 1; // 29 - 6 = 23

                        Road road = path[i].road;
                        for (int j = 0; j < road.Lines.Count; j++)
                        {
                            if (road.Lines[j] != path[i])
                            {
                                path[i] = road.Lines[j];
                                break;
                            }
                        }
                    }
                }
                else if (path[i] == endNodeLine)
                {
                    endIndex = path[i].GetIndexOfNode((GraphNode) end);

                    if(path.Count > 1)
                        startIndex = path[i].GetIndexOfNode(path[i - 1].GetConnectedToByLine(path[i]));
                    else
                        startIndex = path[i].GetIndexOfNode((GraphNode)start);

                    if (endIndex == -1)
                    {
                        for (int j = 0; j < path[i].RoadIntersectionsIndexes.Count; j++)
                        {
                            for (int k = 0;
                                k < path[i].GraphNodes[path[i].RoadIntersectionsIndexes[j]].ConnectedNodes.Count;
                                k++)
                            {
                                if (path[i].GraphNodes[path[i].RoadIntersectionsIndexes[j]].ConnectedNodes[k] == end)
                                {
                                    endIndex = path[i].RoadIntersectionsIndexes[j];
                                }
                            }
                        }
                    }

                    if (endIndex == -1)
                    {
                        endIndex = path[i].GraphNodes.Count - 1;
                    }
                }
                else
                {
                    endIndex = path[i].GetIndexOfNode(path[i].GetNodeForConnectedRoadLine(path[i + 1]));
                    startIndex = path[i].GetIndexOfNode(path[i - 1].GetConnectedToByLine(path[i]));
                }

                for (int j = startIndex; j <= endIndex; j++)
                {
                    nodePath.Add(path[i].GraphNodes[j]);
                }
            }
            
            //nodePath.Add(end);
        }
        else
        {
            //Debug.LogError("Failed to build path =(");
        }
        
        return nodePath;
    }

    /*public static Road GetRoadByNode(INode node, bool deep = false)
    {
        if (node.Transform.parent != null && node.Transform.parent.parent != null && node.Transform.parent.parent.GetComponent<Road>())
            return node.Transform.parent.parent.GetComponent<Road>();
        else if (deep)
        {
            List<KeyValuePair<Road, float>> roadDistance = new List<KeyValuePair<Road, float>>();
            List<Road> roads = ActualInfo.Instance.GlobalAINavigator.GetRoadsByDistance(node.Transform.position, 300, 2);
            for (int i = 0; i < roads.Count; i++)
            {
                for (int j = 0; j < roads[i].Lines.Count; j++)
                {
                    for (int k = 0; k < roads[i].Lines[j].RoadIntersectionsIndexes.Count; k++)
                    {
                        for (int l = 0;
                            l < roads[i].Lines[j].GraphNodes[roads[i].Lines[j].RoadIntersectionsIndexes[k]].ConnectedNodes.Count;
                            l++)
                        {
                            if ((INode)roads[i].Lines[j].GraphNodes[roads[i].Lines[j].RoadIntersectionsIndexes[k]]
                                    .ConnectedNodes[l] == node)
                            {
                                roadDistance.Add(new KeyValuePair<Road, float>(roads[i], Vector3.Distance(roads[i].Lines[j].GraphNodes[roads[i].Lines[j].RoadIntersectionsIndexes[k]].Transform.position, ActualInfo.Instance.GetPlayer().UnitPosition) +
                                    Vector3.Distance(roads[i].Lines[j].GraphNodes[0].Transform.position, ActualInfo.Instance.GetPlayer().UnitPosition)));
                                //return roads[i];
                            }
                        }
                    }
                }
            }

            float min = float.MaxValue;
            Road minimumIndexRoad = null;
            for (int i = 0; i < roadDistance.Count; i++)
            {
                if (roadDistance.ElementAt(i).Value < min)
                {
                    minimumIndexRoad = roadDistance.ElementAt(i).Key;
                    min = roadDistance.ElementAt(i).Value;
                }
            }

            return minimumIndexRoad;
        }


        return null;
    }*/

    public static Road GetRoadByNode(INode startNode, INode node, bool deep = false)
    {
        if (node.Transform.parent != null && node.Transform.parent.parent != null && node.Transform.parent.parent.GetComponent<Road>())
            return node.Transform.parent.parent.GetComponent<Road>();

        if (deep)
        {
            List<KeyValuePair<Road, float>> roadDistance = new List<KeyValuePair<Road, float>>();
            List<Road> roads = ActualInfo.Instance.GlobalAINavigator.GetRoadsByDistance(ActualInfo.Instance.GlobalAINavigator.Roads, 
                                                                                        node.Transform.position, 300, 2);
            for (int i = 0; i < roads.Count; i++)
            {
                for (int j = 0; j < roads[i].Lines.Count; j++)
                {
                    for (int k = 0; k < roads[i].Lines[j].RoadIntersectionsIndexes.Count; k++)
                    {
                        for (int l = 0;
                            l < roads[i].Lines[j].GraphNodes[roads[i].Lines[j].RoadIntersectionsIndexes[k]].ConnectedNodes.Count;
                            l++)
                        {
                            if ((INode)roads[i].Lines[j].GraphNodes[roads[i].Lines[j].RoadIntersectionsIndexes[k]]
                                    .ConnectedNodes[l] == node)
                            {
                                roadDistance.Add(new KeyValuePair<Road, float>(roads[i], Vector3.Distance(roads[i].Lines[j].GraphNodes[roads[i].Lines[j].RoadIntersectionsIndexes[k]].Transform.position, startNode.Transform.position) +
                                    Vector3.Distance(roads[i].Lines[j].GraphNodes[0].Transform.position, startNode.Transform.position)));
                                //return roads[i];
                            }
                        }
                    }
                }
            }

            float min = float.MaxValue;
            Road minimumIndexRoad = null;
            for (int i = 0; i < roadDistance.Count; i++)
            {
                if (roadDistance.ElementAt(i).Value < min)
                {
                    minimumIndexRoad = roadDistance.ElementAt(i).Key;
                    min = roadDistance.ElementAt(i).Value;
                }
            }

            return minimumIndexRoad;
        }


        return null;
    }

    /*public List<INode> GetRouteToNode(INode startNode, INode targetNode)
    {
        int targetNodeIndex = Nodes.IndexOf(targetNode);
        bool enough = false;
        INode currentNode = startNode;
        List<NodeParams> navParams = new List<NodeParams>();
        List<INode> nextNodes = new List<INode>();
        for (int i = 0; i < Nodes.Count; i++)
        {
            navParams.Add(new NodeParams());
        }
        //Debug.Log(startNode.Transform.name + " " + targetNode.Transform.name);
        navParams[Nodes.IndexOf(currentNode)].Weight = 0f;
        
        while (!enough)
        {
            int currentIndex = Nodes.IndexOf(currentNode);
            navParams[currentIndex].Mark = true;
            
            for (int i = 0; i < currentNode.GetConnectedNodes().Count; i++)
            {
                INode node = Nodes[Nodes.IndexOf(currentNode.GetConnectedNodes()[i])];
                if (currentNode.GetDistanceToNode(node) < navParams[Nodes.IndexOf(node)].Weight)
                {
                    int index = Nodes.IndexOf(node);

                    if (!((currentNode.GetDistanceToNode(node) + navParams[currentIndex].Weight) <
                          navParams[index].Weight)) continue;

                    //

                    if (!navParams[index].DontTryOverrideWeight)
                        navParams[index].Weight = 
                            currentNode.GetDistanceToNode(node) + navParams[currentIndex].Weight;

                    if (navParams[index].Nodes == null)
                        navParams[index].Nodes = new List<INode>();
                    else
                        navParams[index].Nodes.Clear();

                    for (int j = 0; j < navParams[currentIndex].Nodes.Count; j++)
                    {
                        navParams[index].Nodes.Add(navParams[currentIndex].Nodes[j]);
                    }
                    navParams[index].Nodes.Add(Nodes[currentIndex]);

                    if (currentNode == startNode)
                    {
                        navParams[index].DontTryOverrideWeight = true;
                    }

                    // Just debug
                    //if (node == targetNode)
                    //    Debug.Log("Запись в целевую ноду с ноды " + node.Transform.name);

                    ((GraphNode) Nodes[currentIndex]).path =
                        (from t in navParams[currentIndex].Nodes select t.Transform).ToArray();
                    ((GraphNode) Nodes[index]).path =
                        (from t in navParams[index].Nodes select t.Transform).ToArray();
                }
            }


            INode tempCurrentNode = currentNode;
            float min = float.MaxValue;

            for (int i = 0; i < currentNode.GetConnectedNodes().Count; i++)
            {
                if (navParams[Nodes.IndexOf((currentNode.GetConnectedNodes()[i]))].Weight < min && !navParams[Nodes.IndexOf((currentNode.GetConnectedNodes()[i]))].Mark)
                {
                    min = navParams[Nodes.IndexOf((currentNode.GetConnectedNodes()[i]))].Weight;
                    //Debug.Log("Думаю пойти с " + tempCurrentNode.Transform.name + " на " + currentNode.GetConnectedNodes()[i].name + " ноду");
                    tempCurrentNode = currentNode.GetConnectedNodes()[i];
                }
            }

            if (currentNode != tempCurrentNode)
            {
                nextNodes.Add(tempCurrentNode);
                for (int i = 0; i < currentNode.GetConnectedNodes().Count; i++)
                {
                    if (currentNode.GetConnectedNodes()[i] != (GraphNode) tempCurrentNode && !navParams[Nodes.IndexOf((currentNode.GetConnectedNodes()[i]))].Mark)
                    {
                        nextNodes.Add(currentNode.GetConnectedNodes()[i]);
                    }
                }
                
                tempCurrentNode = nextNodes.FirstOrDefault();
                //Debug.Log("Ставлю текущей нодой " + tempCurrentNode.Transform.name);
                nextNodes.Remove(tempCurrentNode);

                currentNode = tempCurrentNode;
            }
            else
            {
                 if (nextNodes.Count > 0)
                {
                    tempCurrentNode = nextNodes.FirstOrDefault();
                    nextNodes.Remove(tempCurrentNode);
                    currentNode = tempCurrentNode;
                }
                else if ((from param in navParams where param.Mark == false select param).ToArray().Length > 0)
                {
                    currentNode =
                        Nodes[
                            navParams.IndexOf((from param in navParams where param.Mark == false select param).First())];
                }
                else
                {
                    enough = true;
                }
            }
        }

        navParams[targetNodeIndex].Nodes.Add(targetNode);

        var tempParamsNodes = new List<INode>();

        // We catch-up path to remove strange circles and reduce the path if it is some kind of mistake
        // ReSharper disable once ForCanBeConvertedToForeach
        for (int i = 0; i < navParams[targetNodeIndex].Nodes.Count; i++)
        {
            tempParamsNodes.Add(navParams[targetNodeIndex].Nodes[i]);
            if (navParams[targetNodeIndex].Nodes[i] == targetNode)
                break;
        }

        return tempParamsNodes;
    }*/

    public List<Road> GetRoadsByDistance(List<Road> roads, Vector3 center, float distance, int checkLimit = 4)
    {
        List<Road> closestRoads = new List<Road>();
        int stepsChecked = 0;

        for (int i = 0; i < roads.Count; i++)
        {
            if (roads[i].Lines.Count != 0)
            {
                for (int j = 0;
                    j < roads[i].Lines[0].GraphNodes.Count;
                    j += (int) (roads[i].Lines[0].GraphNodes.Count/4f))
                {
                    if (Vector3.Distance(center, roads[i].Lines[0].GraphNodes[j].transform.position) < distance)
                    {
                        stepsChecked++;
                    }
                }

                if (Vector3.Distance(center, roads[i].Lines[0].GraphNodes[roads[i].Lines[0].GraphNodes.Count-1].transform.position) < distance)
                {
                    stepsChecked++;
                }
                
                if (stepsChecked >= checkLimit)
                {
                    closestRoads.Add(roads[i]);
                }
                stepsChecked = 0;
            }
        }

        return closestRoads;
    }

    public INode GetNodeByLocationId(int id)
    {
        return (from node in Nodes where node.LocationId == id select node).First();
    }
}
