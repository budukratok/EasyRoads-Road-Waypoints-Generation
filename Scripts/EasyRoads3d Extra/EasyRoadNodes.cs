using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EasyRoads3Dv3;

[ExecuteInEditMode]
public class EasyRoadNodes : MonoBehaviour
{
    public bool Generate = false;

    public Road Road;
    //public List<GraphNode> Nodes = new List<GraphNode>();
    public int Count = 10;
    public float NodeDefinition = 10f;
    public int Definition = 1;
    public float MaximumDistance = 15f;

    public List<Road> Roads = new List<Road>();
    public List<CrossRoadController> CrossRoads = new List<CrossRoadController>();

    public List<ERRoad> ERRoads = new List<ERRoad>(); 
    public List<bool> DontBuild = new List<bool>();
    public List<int> LinesCount = new List<int>();
    public List<bool> Inverted = new List<bool>();
    public List<bool> OneWay = new List<bool>();

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateNetwork()
    {
        Generate = true;

        for (int i = 0; i < Roads.Count; i++)
        {
            if(Roads[i] != null)
                DestroyImmediate(Roads[i].gameObject);
        }

        Roads.Clear();

        ERRoad[] roads = ERRoads.ToArray();

        for (int i = 0; i < roads.Length; i++)
        {
            if (!DontBuild[i])
            {
                GameObject tempGo = new GameObject("Road Lines", typeof (Road));
                tempGo.transform.position = transform.position;
                tempGo.transform.SetParent(transform);
                tempGo.transform.name = "RoadLine_" + ERRoads[i].roadScript.transform.name;

                Road = tempGo.GetComponent<Road>();

                if (roads[i].GetSplinePointsCenter().Length != 0)
                    Road.Lines =
                        GenerateRoadNodes(roads[i], Road, NodeDefinition, Definition, Count, LinesCount[i], Inverted[i],
                            OneWay[i])
                            .ToList();

                Road.RecalculateIntersectionIndexes();
                Road.RecalculateConnectedRoadLines();

                Roads.Add(Road);
            }
            else
            {
                Roads.Add(null);
            }
        }

        List<RoadLine> tempLines;
        List<Vector3> centers = new List<Vector3>();
        Vector3 center = new Vector3();
        Vector3 vectorPlus = new Vector3();

        for (int i = 0; i < Roads.Count; i++)
        {
            if (Roads[i] != null)
            {
                for (int j = 0; j < Roads[i].Lines.Count; j++)
                {
                    centers.Clear();

                    vectorPlus =
                        (Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].Transform.position -
                         Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 2].Transform.position)
                            .normalized*
                        2f;

                    tempLines =
                        GetClosestRoadLinesByFirstNodes(
                            Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].Transform.position,
                            Roads[i], vectorPlus, MaximumDistance);

                    for (int k = 0; k < tempLines.Count; k++)
                    {
                        Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].ConnectedNodes.Add(
                            tempLines[k].GraphNodes[0]);
                        centers.Add(Vector3.Lerp(
                            Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].Transform.position,
                            tempLines[k].GraphNodes[0].Transform.position, 0.5f));
                    }

                    if (centers.Count > 1f)
                    {
                        for (int k = 1; k < centers.Count; k++)
                        {
                            center = Vector3.Lerp(centers[k - 1], centers[k], 0.5f);
                        }
                    }
                    else
                    {
                        if (centers.Count == 1)
                        {
                            center = centers[0];
                        }
                    }

                    for (int k = 0; k < tempLines.Count; k++)
                    {
                        Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].LinksBezierHelpers.Add(
                            center + vectorPlus);
                    }
                }

                Roads[i].RecalculateIntersectionIndexes();
                Roads[i].RecalculateConnectedRoadLines();
            }
        }
        
        Generate = false;
    }

    public void GenerateOneRoad(int i)
    {
        if (!DontBuild[i])
        {
            DestroyImmediate(Roads[i].gameObject);

            GameObject tempGo = new GameObject("Road Lines", typeof (Road));
            tempGo.transform.position = transform.position;
            tempGo.transform.SetParent(transform);
            tempGo.transform.name = "RoadLine_" + ERRoads[i].roadScript.transform.name;

            Road = tempGo.GetComponent<Road>();

            if (ERRoads[i].GetSplinePointsCenter().Length != 0)
                Road.Lines =
                    GenerateRoadNodes(ERRoads[i], Road, NodeDefinition, Definition, Count, LinesCount[i], Inverted[i],
                        OneWay[i])
                        .ToList();

            Road.RecalculateIntersectionIndexes();
            Road.RecalculateConnectedRoadLines();

            Roads[i] = Road;

            List<RoadLine> tempLines;
            List<Vector3> centers = new List<Vector3>();
            Vector3 center = new Vector3();
            Vector3 vectorPlus = new Vector3();

            for (int j = 0; j < Roads[i].Lines.Count; j++)
            {
                centers.Clear();

                vectorPlus =
                    (Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].Transform.position -
                     Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 2].Transform.position).normalized*
                    2f;

                tempLines =
                    GetClosestRoadLinesByFirstNodes(
                        Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].Transform.position,
                        Roads[i], vectorPlus, MaximumDistance);

                for (int k = 0; k < tempLines.Count; k++)
                {
                    Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].ConnectedNodes.Add(
                        tempLines[k].GraphNodes[0]);
                    centers.Add(Vector3.Lerp(
                        Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].Transform.position,
                        tempLines[k].GraphNodes[0].Transform.position, 0.5f));
                }

                if (centers.Count > 1f)
                {
                    for (int k = 1; k < centers.Count; k++)
                    {
                        center = Vector3.Lerp(centers[k - 1], centers[k], 0.5f);
                    }
                }
                else
                {
                    if (centers.Count == 1)
                    {
                        center = centers[0];
                    }
                }

                for (int k = 0; k < tempLines.Count; k++)
                {
                    Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1].LinksBezierHelpers.Add(
                        center + vectorPlus);
                }
            }

            Roads[i].RecalculateIntersectionIndexes();
            Roads[i].RecalculateConnectedRoadLines();
        }
    }


    public List<RoadLine> GetClosestRoadLinesByFirstNodes(Vector3 pos, Road road, Vector3 forward, float maxDistance)
    {
        List<RoadLine> Lines = new List<RoadLine>();

        List<GraphNode> firstNodes = new List<GraphNode>();
        for (int i = 0; i < Roads.Count; i++)
        {
            if (Roads[i] != road && Roads[i] != null)
            {
                for (int j = 0; j < Roads[i].Lines.Count; j++)
                {
                    firstNodes.Add(Roads[i].Lines[j].GraphNodes[0]);
                }
            }
        }

        firstNodes = firstNodes.OrderBy(x => (Vector3.Distance(pos, x.Transform.position)*200f + 360f-Mathf.Abs(Vector3.Angle(forward,
                    (x.Transform.parent.parent.GetComponent<Road>().GetRoadLineByNode(x).GraphNodes[1].Transform.position - x.Transform.position))))).ToList();

        for (int i = 0; i < firstNodes.Count; i++)
        {
            if (Vector3.Distance(firstNodes[i].Transform.position, pos) <= maxDistance)
            {
                Lines.Add(firstNodes[i].Transform.parent.parent.GetComponent<Road>().GetRoadLineByNode(firstNodes[i]));
            }
        }

        if (Lines.Count == 0)
        {
            Lines.Add(firstNodes[0].Transform.parent.parent.GetComponent<Road>().GetRoadLineByNode(firstNodes[0]));
        }

        for (int i = 0; i < Lines.Count; i++)
        {
            if (
                Mathf.Abs(Vector3.Angle(forward,
                    (Lines[i].GraphNodes[1].Transform.position - Lines[i].GraphNodes[0].Transform.position))) > 180 ||
                Vector3.Distance(pos, Lines[i].GraphNodes[0].Transform.position) >
                Vector3.Distance(pos, Lines[i].GraphNodes[1].Transform.position))
            {
                Lines.Remove(Lines[i]);
            }
        }

        return Lines;
    } 

    public RoadLine[] GenerateRoadNodes(ERRoad road, Road roadLines, float nodeDefinition, int definition, int count, int linesCount, bool invertFirst, bool oneWay)
    {
        RoadLine[] lines = new RoadLine[linesCount];//(count*(linesCount/2));
        float plusMinus = 1f;
        int elCount = (int) (road.roadScript.distances[road.roadScript.distances.Count-1]/nodeDefinition);

        if (elCount == 0)
            elCount = 2;

        if ((float) elCount%2 != 0f)
        {
            elCount += 1;
        }
        
        for (int i = 0; i < linesCount; i++)
        {
            GameObject tempGo = new GameObject("Line" + (i), typeof(RoadLine));
            tempGo.transform.SetParent(roadLines.transform);
            lines[i] = (tempGo.GetComponent<RoadLine>());

            lines[i].GraphNodes = new List<GraphNode>(elCount/ linesCount);
            lines[i].road = roadLines;
            plusMinus *= -1f;

            lines[i].GraphNodes.AddRange(GenerateOneLine(road, roadLines, (linesCount > 1 ? (road.roadScript.roadWidth / 2f) : 0f) * plusMinus, definition, elCount, oneWay ? !invertFirst : (plusMinus < 0f)));

            if (plusMinus > 0f)
                plusMinus += (road.roadScript.roadWidth / 4f);
        }

        for (int i = 0; i < linesCount; i++)
        {
            for (int j = 0; j < lines[i].GraphNodes.Count; j++)
            {
                lines[i].GraphNodes[j].Transform.SetParent(lines[i].transform);
            }
        }

        return lines;
    }

    public List<GraphNode> GenerateOneLine(ERRoad road, Road roadLines, float horizontalSep, int definition, int count, bool inverted)
    {
        List<GraphNode> nodes = new List<GraphNode>(count*2);

        Vector3 vect1 = Vector3.zero, POS = default(Vector3), pos, tempVect = Vector3.zero, prevTempVect = Vector3.zero;
        GameObject temp;
        float percent = 0f;
        GraphNode node;

        road.roadScript.distances.Clear();
        road.SetDistances();

        float max = road.roadScript.distances[road.roadScript.distances.Count-1], min = road.roadScript.distances[0];

        int counter = 0;
        int currentElement = 0;


        for (int i = 0; i < count; i += 1)
        {
            percent = (float)i / (float)count;

            if (inverted)
                percent = 1f - ((float)i / (float)count);
            
            percent = Mathf.Lerp(min, max, percent);
            
            if (i == 0)
            {
                if (!inverted)
                {
                    percent = min;
                }
                else
                {
                    percent = max;
                }
            }

            if (i == count - 1)
            {
                if (!inverted)
                {
                    percent = max;
                }
                else
                {
                    percent = min;
                }
            }

           //Debug.Log(road.roadScript.transform.name + " " + currentElement + " " + percent + " "+ road.roadScript.distances.Count);
            
            vect1 = GetPosition(road, percent);
            tempVect =
                (Quaternion.AngleAxis(90, Vector3.up) *
                    (GetPosition(road, percent + 0.01f) - vect1).normalized) *
                (horizontalSep * 0.4f);

            if (inverted && i == 0)
            {
                tempVect =
                (Quaternion.AngleAxis(90, Vector3.up) *
                    (GetPosition(road, percent - 1f) - vect1).normalized * -1f) *
                (horizontalSep * 0.4f);
            }

            if ((!inverted && i != count - 1) || (inverted))
            {
                prevTempVect = tempVect;
            }
            else
            {
                tempVect = prevTempVect;
            }

            vect1 += tempVect;
            

            pos = vect1;//(Vector3.Lerp(splinesAtLeft[currentElement], splinesAtRight[currentElement], horizontalSep/Vector3.Distance(splinesAtLeft[currentElement], splinesAtRight[currentElement])));
            pos += Vector3.up * 2f;

            temp = new GameObject("NavNode " + counter, typeof(GraphNode));

            nodes.Add(temp.GetComponent<GraphNode>());

            node = nodes[nodes.Count - 1];
            node.OverrideDefinition = true;
            node.OverrideDefinitionValue = definition;

            node.MaxSpeed = 35f;

            node.Transform.position = pos;

            temp.transform.SetParent(roadLines.transform);
            
            //GameObject tempGo = new GameObject(tempVect.ToString());
            //tempGo.transform.position = pos;
            //tempGo.transform.SetParent(node.transform);

            counter++;
        }

        currentElement = 0;
        for (int i = 1; i < nodes.Count; i++)
        {
            nodes[i-1].ConnectedNodes.Add(nodes[i]);

            percent = (float)(i) / (float)count - (1f/(float)count)/2f;

            if (inverted)
                percent = 1f - (((float)(i) / (float)count) - (1f / (float)count) / 2f);

            percent = Mathf.Lerp(min, max, percent);

            if (i == 1)
            {
                if (!inverted)
                {
                    percent = min;
                }
                else
                {
                    percent = max;
                }
            }

            //if (i == count - 1)
            //{
            //    if (!inverted)
            //    {
            //        currentElement = road.GetSplinePointsCenter().Length - 1;
            //        percent = max;
            //    }
            //    else
            //    {
            //        currentElement = 0;
            //        percent = min;
            //    }
            //}

            if (inverted && i == 1)
            {
                vect1 = Vector3.Lerp(nodes[i - 1].transform.position, nodes[i].transform.position, 0.5f);
            }
            else
            {

                vect1 = GetPosition(road, percent);
                vect1 +=
                    (Quaternion.AngleAxis(90, Vector3.up)*
                     (GetPosition(road, percent + 0.01f) - vect1).normalized)*
                    (horizontalSep*0.4f);
                vect1 += Vector3.up*2f;
            }



            nodes[i-1].LinksBezierHelpers.Add(vect1);
        }

        return nodes;
    }

    public Vector3 GetPosition(ERRoad road, float distance)
    {
        //if (road.roadScript.distances != null && road.roadScript.distances.Count < 3)
        //{
        //    Debug.Log(road.roadScript.transform.name);

            road.roadScript.distances.Clear();
            road.SetDistances();
        //}

        Vector3 result = Vector3.zero;
        int prevIndex = 0, nextIndex = 0;
        float prevIndexDistance = 0f, nextIndexDistance = 0f;

        if(road.roadScript.distances.Count < 2)
            return result;

        if(distance < road.roadScript.distances[0])
            return road.roadScript.soSplinePoints[0];

        if (road.roadScript.distances.Count == 2)
        {
            prevIndex = 0;
            nextIndex = 1;
        }
        else if (distance > road.roadScript.distances[road.roadScript.distances.Count - 1])
        {
            return road.roadScript.soSplinePoints[road.roadScript.distances.Count - 1];
        }


        if (road.roadScript.distances.Count > 2)
        {
            for (int i = 0; i < road.roadScript.distances.Count - 1; i++)
            {
                if (road.roadScript.distances[i] <= distance && road.roadScript.distances[i + 1] >= distance)
                {
                    prevIndex = i;
                    nextIndex = i + 1;
                }
            }
        }

        prevIndexDistance = road.roadScript.distances[prevIndex];
        nextIndexDistance = road.roadScript.distances[nextIndex];

        result = Vector3.Lerp(road.roadScript.soSplinePoints[prevIndex], road.roadScript.soSplinePoints[nextIndex], ((distance - prevIndexDistance) / (nextIndexDistance-prevIndexDistance)));

        return result;
    }

    public void RegenerateCrossroads()
    {
        for (int i = 0; i < CrossRoads.Count; i++)
        {
            if (CrossRoads[i] != null)
            {
                DestroyImmediate(CrossRoads[i].gameObject);
            }
        }
        CrossRoads.Clear();
        CrossRoads = GenerateCrossRoadsLogic();
    }

    public List<CrossRoadController> GenerateCrossRoadsLogic()
    {
        List<CrossRoadController> crossRoads = new List<CrossRoadController>();

        ERCrossings[] crossings = GameObject.FindObjectsOfType<ERCrossings>();

        for (int i = 0; i < crossings.Length; i++)
        {
            crossRoads.Add(new GameObject(crossings[i].transform.name+"_logic", typeof(CrossRoadController)).GetComponent<CrossRoadController>());
            var crossroad = crossRoads[crossRoads.Count - 1];

            crossroad.transform.position = crossings[i].transform.position;
            crossroad.transform.SetParent(transform);

            crossroad.Nodes = GetNodesForCrossroad(crossings[i], crossroad);
        }

        return crossRoads;
    }

    public List<GraphNodeListWrapper> GetNodesForCrossroad(ERCrossings crossing, CrossRoadController controller)
    {
        List<GraphNodeListWrapper> nodeListWrapper = new List<GraphNodeListWrapper>();

        List<GraphNodeListWrapper> xListWrapper = new List<GraphNodeListWrapper>();
        List<GraphNodeListWrapper> zListWrapper = new List<GraphNodeListWrapper>();

        List<GraphNodeListWrapper> resultListWrapper = new List<GraphNodeListWrapper>();

        List<RoadLine> roadLines = new List<RoadLine>();
        int roadCount = 0;

        for (int i = 0; i < Roads.Count; i++)
        {
            roadLines.Clear();

            for (int j = 0; j < Roads[i].Lines.Count; j++)
            {
                var tempNode = Roads[i].Lines[j].GraphNodes[Roads[i].Lines[j].GraphNodes.Count - 1];
                if (Vector3.Distance(controller.transform.position, tempNode.transform.position) < 25f)
                {
                    roadLines.Add(Roads[i].Lines[j]);
                }
            }

            if (roadLines.Count != 0 && roadLines[roadLines.Count - 1].road == Roads[i])
            {
                nodeListWrapper.Add(new GraphNodeListWrapper(Roads[i].transform.name, roadLines.Count));
                var tempWrapper = nodeListWrapper[nodeListWrapper.Count - 1];

                List<GraphNode> nodes = new List<GraphNode>();

                for (int j = 0; j < roadLines.Count; j++)
                {
                    nodes.Add(roadLines[j].GraphNodes[roadLines[j].GraphNodes.Count - 1]);
                }
                tempWrapper.RightSideNodes = nodes;
            }
        }

        // передние / задние
        for (int i = 0; i < nodeListWrapper.Count; i++)
        {
            float angle = Vector3.Angle(crossing.transform.forward, nodeListWrapper[i][0].transform.position - crossing.transform.position);
            if (angle < 45f || angle > 135f)
            {
                xListWrapper.Add(nodeListWrapper[i]);
            }
        }

        // правые / левые
        for (int i = 0; i < nodeListWrapper.Count; i++)
        {
            float angle = Vector3.Angle(crossing.transform.forward, nodeListWrapper[i][0].transform.position - crossing.transform.position);
            if (angle > 45f && angle < 135f)
            {
                zListWrapper.Add(nodeListWrapper[i]);
            }
        }

        int countX=0, countZ=0;
        for (int i = 0; i < xListWrapper.Count+zListWrapper.Count; i++)
        {
            if (i%2 == 0)
            {
                if (countX < xListWrapper.Count)
                {
                    resultListWrapper.Add(xListWrapper[countX]);
                    countX++;
                }
            }
            else
            {
                if (countZ < zListWrapper.Count)
                {
                    resultListWrapper.Add(zListWrapper[countZ]);
                    countZ++;
                }
            }
        }

        return resultListWrapper;
    }
}
