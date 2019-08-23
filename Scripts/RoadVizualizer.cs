using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor; // only for editor
#endif
using System.Collections;

public class RoadVizualiser
{
    public List<Vector3> vertices = new List<Vector3>();

    private Vector3 move;
    private RaycastHit hit;

    private Mesh[] roadMesh;

    private Vector3[] crossRoadVectors = new Vector3[4];

#if UNITY_EDITOR

    /// <summary>
    /// Call this only in OnDrawGizmos() MonoBehaviour method
    /// </summary>
    public void VizualizeRoadNode(Vector3 position)
    {
        Gizmos.color = Color.white;
        Gizmos.DrawSphere(position, 2f);
    }

    public void VizualizeRoadLinks(RoadNode node, Vector3 transformPosition, List<RoadNode> nodes, List<Vector3> helpers)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (i + 1 <= helpers.Count)
            {
                for (int j = 1; j < node.IterationsCount[i] + 1; j++)
                {
                    if (nodes[i] != null && helpers[i] != null)
                    {
                        Gizmos.color = Color.white;

                        Gizmos.DrawLine(
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                (float)(j - 1) / (float)node.IterationsCount[i]),
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                ((float)j / (float)node.IterationsCount[i]))
                            );

                        move = (Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                helpers[i],
                                (float)(j - 1) / (float)node.IterationsCount[i]) -
                                    Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                        helpers[i],
                                        (float)(j) / (float)node.IterationsCount[i])).normalized * 6f;

                        Gizmos.DrawLine(
                                Rotate(
                                    Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                        (float)(j) / (float)node.IterationsCount[i]),
                                    move,
                                    90, 1),
                                Rotate(
                                    Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                        (float)(j) / (float)node.IterationsCount[i]),
                                    move,
                                    -90, 1));

                    }
                }
            }
            else
            {
                for (int j = 1; j < node.IterationsCount[i] + 1; j++)
                {
                    if (nodes[i] != null)
                    {
                        Gizmos.DrawLine(
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                Bezier.CenterOf(transformPosition, (nodes[i]).transform.position),
                                (float)(j - 1) / (float)node.IterationsCount[i]),
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                Bezier.CenterOf(transformPosition, (nodes[i]).transform.position),
                                (float)j / (float)node.IterationsCount[i])
                            );



                        move = (Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                helpers[i],
                                (float)(j - 1) / (float)node.IterationsCount[i]) -
                                    Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                        helpers[i],
                                        (float)(j) / (float)node.IterationsCount[i])).normalized * 6f;

                        Gizmos.DrawLine(
                                Rotate(
                                    Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                        (float)(j) / (float)node.IterationsCount[i]),
                                    move,
                                    90, 1),
                                Rotate(
                                    Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                        (float)(j) / (float)node.IterationsCount[i]),
                                    move,
                                    -90, 1));
                    }
                }
            }
        }
    }

    private Vector3 GetDirection(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }

    public void GenerateRoadMesh(RoadNode node, Vector3 transformPosition, List<RoadNode> nodes, List<Vector3> helpers)
    {
        Vector3 oneDirection = Vector3.zero, twoDirection = Vector3.zero;
        roadMesh = new Mesh[nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            roadMesh[i] = new Mesh();
            roadMesh[i].name = node.transform.name;
            if (i + 1 <= helpers.Count)
            {
                move = (Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                            helpers[i],
                            (float)(0) / (float)node.IterationsCount[i]) -
                                Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                    helpers[i],
                                    (float)(1) / (float)node.IterationsCount[i])).normalized * 6f;

                vertices.Add(node.transform.InverseTransformPoint(Rotate(
                    Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                        (float)(0) / (float)node.IterationsCount[i]),
                    move,
                    90, 1)));

                vertices.Add(node.transform.InverseTransformPoint(Rotate(
                    Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                        (float)(0) / (float)node.IterationsCount[i]),
                    move,
                    -90, 1)));

                for (int j = 1; j < node.IterationsCount[i] + 1; j++)
                {
                    if (nodes[i] != null && helpers[i] != null)
                    {
                        move = (Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                            helpers[i],
                            (float)(j - 1) / (float)node.IterationsCount[i]) -
                                Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                    helpers[i],
                                    (float)(j) / (float)node.IterationsCount[i])).normalized * 6f;

                        vertices.Add(node.transform.InverseTransformPoint(Rotate(
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                (float)(j) / (float)node.IterationsCount[i]),
                            move,
                            90, 1)));
                        vertices.Add(node.transform.InverseTransformPoint(Rotate(
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                (float)(j) / (float)node.IterationsCount[i]),
                            move,
                            -90, 1)));
                    }
                }
            }

            switch (nodes[i].roadConnectType)
            {
                case RoadConnectType.Crossroad3:
                    throw new Exception("Перекрёстки не могут быть целевыми нодами");
                    break;
                case RoadConnectType.Crossroad4:
                    throw new Exception("Перекрёстки не могут быть целевыми нодами");
                    break;
                case RoadConnectType.Normal:
                    if (nodes[i].ConnectedNodes.Count > 0)
                    {
                        move = (Bezier.GetPointPosition(nodes[i].transform.position,
                            (nodes[i].ConnectedNodes[0]).transform.position,
                            nodes[i].LinksBezierHelpers[0],
                            (float)(0) / (float)nodes[i].IterationsCount[0]) -
                                Bezier.GetPointPosition(nodes[i].transform.position,
                                    (nodes[i].ConnectedNodes[0]).transform.position,
                                    nodes[i].LinksBezierHelpers[0],
                                    (float)(1) / (float)nodes[i].IterationsCount[0])).normalized * 6f;

                        if (Vector3.Angle(move, ((Bezier.GetPointPosition(node.transform.position,
                            (nodes[i]).transform.position,
                            helpers[i],
                            (float)(node.IterationsCount[i] - 1) / (float)node.IterationsCount[i]) -
                                Bezier.GetPointPosition(node.transform.position,
                                    (nodes[i]).transform.position,
                                    helpers[i],
                                    (float)(node.IterationsCount[i] - 2) / (float)node.IterationsCount[i])).normalized * 6f)) < 90f)
                        {
                            move = move * -1f;
                        }

                        vertices[vertices.Count - 2] = node.transform.InverseTransformPoint(Rotate(
                            Bezier.GetPointPosition(nodes[i].transform.position,
                                (nodes[i].ConnectedNodes[0]).transform.position,
                                nodes[i].LinksBezierHelpers[0],
                                0f),
                            move,
                            90, 1));

                        vertices[vertices.Count - 1] = node.transform.InverseTransformPoint(Rotate(
                            Bezier.GetPointPosition(nodes[i].transform.position,
                                (nodes[i].ConnectedNodes[0]).transform.position,
                                nodes[i].LinksBezierHelpers[0],
                                0f),
                            move,
                            -90, 1));
                        }
                    break;
                case RoadConnectType.Stop:

                    break;
            }

            switch (node.roadConnectType)
            {
                case RoadConnectType.Crossroad3:
                    
                    if (i == 0)
                    {
                        crossRoadVectors[i] = node.transform.TransformPoint(vertices[2]);
                        crossRoadVectors[i+1] = node.transform.TransformPoint(vertices[3]);
                    }

                    if (i == 1)
                    {
                        crossRoadVectors[i] = Vector3.Lerp(crossRoadVectors[i], node.transform.TransformPoint(vertices[2]), 0.5f);
                        oneDirection = (crossRoadVectors[i] - node.transform.position).normalized;
                        crossRoadVectors[i] = crossRoadVectors[i] + (crossRoadVectors[i] - node.transform.position).normalized * 0f;
                        crossRoadVectors[i + 1] = node.transform.TransformPoint(vertices[3]);
                    }

                    if (i == 2)
                    {
                        crossRoadVectors[i] = Vector3.Lerp(crossRoadVectors[i], node.transform.TransformPoint(vertices[2]), 0.5f);
                        twoDirection = (crossRoadVectors[i] - node.transform.position).normalized;
                        crossRoadVectors[i] = crossRoadVectors[i] + (crossRoadVectors[i] - node.transform.position).normalized * 0f;
                        crossRoadVectors[i + 1] = node.transform.TransformPoint(vertices[3]);

                    }

                    vertices[1] = vertices[3]; 
                    vertices[0] = vertices[2];



                break;
                case RoadConnectType.Crossroad4:
                    if (i == 0)
                    {
                        crossRoadVectors[i] = node.transform.TransformPoint(vertices[2]);
                        crossRoadVectors[i + 1] = node.transform.TransformPoint(vertices[3]);
                    }

                    if (i == 1)
                    {
                        crossRoadVectors[i] = Vector3.Lerp(crossRoadVectors[i], node.transform.TransformPoint(vertices[2]), 0.5f);
                        oneDirection = (crossRoadVectors[i] - node.transform.position).normalized;
                        crossRoadVectors[i] = crossRoadVectors[i] + (crossRoadVectors[i] - node.transform.position).normalized * 0f;
                        crossRoadVectors[i + 1] = node.transform.TransformPoint(vertices[3]);
                    }

                    if (i == 2)
                    {
                        crossRoadVectors[i] = Vector3.Lerp(crossRoadVectors[i], node.transform.TransformPoint(vertices[2]), 0.5f);
                        twoDirection = (crossRoadVectors[i] - node.transform.position).normalized;
                        crossRoadVectors[i] = crossRoadVectors[i] + (crossRoadVectors[i] - node.transform.position).normalized * 0f;
                        crossRoadVectors[i + 1] = node.transform.TransformPoint(vertices[3]);

                    }

                    vertices[1] = vertices[3];
                    vertices[0] = vertices[2];
                    break;
                case RoadConnectType.Normal:
                    if (nodes[i].ConnectedNodes.Count > 0)
                    {
                        move = (Bezier.GetPointPosition(nodes[i].transform.position,
                            (nodes[i].ConnectedNodes[0]).transform.position,
                            nodes[i].LinksBezierHelpers[0],
                            (float) (0)/(float) nodes[i].IterationsCount[0]) -
                                Bezier.GetPointPosition(nodes[i].transform.position,
                                    (nodes[i].ConnectedNodes[0]).transform.position,
                                    nodes[i].LinksBezierHelpers[0],
                                    (float) (1)/(float) nodes[i].IterationsCount[0])).normalized * 6f;

                        if (Vector3.Angle(move, ((Bezier.GetPointPosition(node.transform.position,
                            (nodes[i]).transform.position,
                            helpers[i],
                            (float)(node.IterationsCount[i] - 1) / (float)node.IterationsCount[i]) -
                                Bezier.GetPointPosition(node.transform.position,
                                    (nodes[i]).transform.position,
                                    helpers[i],
                                    (float)(node.IterationsCount[i] - 2) / (float)node.IterationsCount[i])).normalized * 6f)) < 90f)
                        {
                            move = move * -1f;
                        }

                        vertices[vertices.Count - 2] = node.transform.InverseTransformPoint(Rotate(
                            Bezier.GetPointPosition(nodes[i].transform.position,
                                (nodes[i].ConnectedNodes[0]).transform.position,
                                nodes[i].LinksBezierHelpers[0],
                                0f),
                            move,
                            90, 1));

                        vertices[vertices.Count - 1] = node.transform.InverseTransformPoint(Rotate(
                            Bezier.GetPointPosition(nodes[i].transform.position,
                                (nodes[i].ConnectedNodes[0]).transform.position,
                                nodes[i].LinksBezierHelpers[0],
                                0f),
                            move,
                            -90, 1));
                        
                    }

                    if (nodes.Count > 1 && i == 1)
                    {
                        move = (Bezier.GetPointPosition(node.transform.position,
                            (nodes[0]).transform.position,
                            helpers[0],
                            (float)(0) / (float)node.IterationsCount[0]) -
                                Bezier.GetPointPosition(node.transform.position,
                                    (nodes[0]).transform.position,
                                    helpers[0],
                                    (float)(1) / (float)node.IterationsCount[0])).normalized * 6f;

                        vertices[0] = node.transform.InverseTransformPoint(Rotate(
                            Bezier.GetPointPosition(node.transform.position,
                                (nodes[0]).transform.position,
                                helpers[0],
                                0f),
                            move,
                            -90, 1));

                        vertices[1] = node.transform.InverseTransformPoint(Rotate(
                            Bezier.GetPointPosition(node.transform.position,
                                (nodes[0]).transform.position,
                                helpers[0],
                                0f),
                            move,
                            90, 1));
                    }
                    break;
                case RoadConnectType.Stop:
                    throw new Exception("Для тупиковых нод не нужно просчитывать дорогу");
                    break;
            }
                
            for (int k = 0; k < vertices.Count; k++)
            {
                Vector3 hitPoint = Vector3.zero, hitPoint2 = Vector3.zero;

                Terrain first = new Terrain(), second = new Terrain();

                if (Physics.Raycast(node.transform.TransformPoint(vertices[k]) + Vector3.up*15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        hitPoint = node.transform.InverseTransformPoint(hit.point);
                        if (hit.collider.GetComponent<Terrain>())
                        {
                            first = hit.collider.GetComponent<Terrain>();
                        }
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(vertices[k+1]) + Vector3.up * 15f, -Vector3.up, out hit, 400f, 1))
                {
                    hitPoint2 = node.transform.InverseTransformPoint(hit.point);
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        if (hit.collider.GetComponent<Terrain>())
                        {
                            second = hit.collider.GetComponent<Terrain>();
                        }

                        if (node.transform.InverseTransformPoint(hit.point).y >= hitPoint.y)
                        {
                            vertices[k] = new Vector3(vertices[k].x,
                                node.transform.InverseTransformPoint(hit.point).y + 0.1f, vertices[k].z);
                            vertices[k + 1] = new Vector3(vertices[k + 1].x,
                                node.transform.InverseTransformPoint(hit.point).y + 0.1f, vertices[k + 1].z);

                            Vector2 terrainPosLow = TerrainCoords(node.transform.TransformPoint(hitPoint), first);
                            Vector2 terrainPosHight = TerrainCoords(node.transform.TransformPoint(hitPoint2), second);

                            float height = second.terrainData.GetInterpolatedHeight((int)terrainPosHight.x, (int)terrainPosHight.y);
                            float[,] heights = new float[(int)Mathf.Abs(terrainPosHight.x - terrainPosLow.x), (int)Mathf.Abs(terrainPosHight.y - terrainPosLow.y)];

                            for (int x = 0; x < (int)Mathf.Abs(terrainPosHight.x - terrainPosLow.x); x++)
                            {
                                for (int y = 0; y < (int)Mathf.Abs(terrainPosHight.y - terrainPosLow.y); y++)
                                {
                                    heights[x, y] = height;
                                }
                            }

                            Debug.Log(height + " " + (int)Mathf.Abs(terrainPosHight.x - terrainPosLow.x) + " " + (int)Mathf.Abs(terrainPosHight.y - terrainPosLow.y));
                            first.terrainData.SetHeights((int)terrainPosLow.x, (int)terrainPosLow.y, heights);
                        }
                        else
                        {
                            vertices[k] = new Vector3(vertices[k].x, hitPoint.y + 0.1f, vertices[k].z);
                            vertices[k + 1] = new Vector3(vertices[k + 1].x, hitPoint.y + 0.1f, vertices[k + 1].z);
                        }
                    }
                }

                if (k > 1)
                {
                    if (Mathf.Abs(vertices[k - 2].y - vertices[k].y) > 2f)
                    {

                        vertices[k] = new Vector3(vertices[k].x, vertices[k - 2].y + ((vertices[k].y - vertices[k - 2].y) > 0 ? 0.26f : -0.26f), vertices[k].z);
                        vertices[k + 1] = new Vector3(vertices[k + 1].x, vertices[k - 1].y + ((vertices[k + 1].y - vertices[k - 1].y) > 0 ? 0.26f : -0.26f), vertices[k + 1].z);
                    }
                }

                k++;
            }

            //Debug.Log(vertices.ToArray().Length + " " + GenTris(vertices.Count - 1, node.IterationsCount[i]).Length + " " + GenNormals(node.IterationsCount[i]).Length + " " + GenUVs(node.roadConnectType, node.IterationsCount[i]).Length);
            roadMesh[i].vertices = vertices.ToArray();
            roadMesh[i].triangles = GenTris(vertices.Count - 1, node.IterationsCount[i]);
            roadMesh[i].normals = GenNormals(node.IterationsCount[i]);
            roadMesh[i].uv = GenUVs(nodes[i].roadConnectType, node.IterationsCount[i]);
            roadMesh[i].RecalculateNormals();
            
            vertices.Clear();

        }

        switch (node.roadConnectType)
        {
            case RoadConnectType.Crossroad4:
            case RoadConnectType.Crossroad3:
                List<Vector3> verts = roadMesh[0].vertices.ToList();
                List<Vector3> normals = roadMesh[0].normals.ToList();
                List<Vector2> uvs = roadMesh[0].uv.ToList();
                List<int> tris = roadMesh[0].triangles.ToList();

                int count = verts.Count;

                verts.Add(node.transform.InverseTransformPoint(node.transform.position + twoDirection * 8.5f));
                verts.Add(node.transform.InverseTransformPoint(node.transform.position + oneDirection * 8.5f));
                verts.Add(node.transform.InverseTransformPoint(node.transform.position - oneDirection * 8.5f));
                verts.Add(node.transform.InverseTransformPoint(node.transform.position - twoDirection * 8.5f));

                if (node.roadConnectType == RoadConnectType.Crossroad3)
                {
                    uvs.Add(new Vector2(0.99f, 0.99f));
                    uvs.Add(new Vector2(0.51f, 0.99f));
                    uvs.Add(new Vector2(0.99f, 0.51f));
                    uvs.Add(new Vector2(0.51f, 0.51f));
                }
                else
                {
                    uvs.Add(new Vector2(0.491f, 0.99f));
                    uvs.Add(new Vector2(0.011f, 0.99f));
                    uvs.Add(new Vector2(0.491f, 0.51f));
                    uvs.Add(new Vector2(0.011f, 0.51f));
                }

                tris.Add(count + 0);
                tris.Add(count + 2);
                tris.Add(count + 1);
                tris.Add(count + 2);
                tris.Add(count + 3);
                tris.Add(count + 1);

                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);

                
                roadMesh[0].vertices = verts.ToArray();
                roadMesh[0].uv = uvs.ToArray();
                roadMesh[0].triangles = tris.ToArray();
                roadMesh[0].normals = normals.ToArray();
                roadMesh[0].RecalculateNormals();



                // меш 1
                List<Vector3> tempVerts = roadMesh[0].vertices.ToList();
                tempVerts[1] = node.transform.InverseTransformPoint(node.transform.position + oneDirection * 8.5f);
                tempVerts[3] = node.transform.InverseTransformPoint(node.transform.position + oneDirection * 8.5f);
                tempVerts[5] = node.transform.InverseTransformPoint(node.transform.position + oneDirection * 8.5f);
                tempVerts[0] = node.transform.InverseTransformPoint(node.transform.position - twoDirection * 8.5f);
                tempVerts[2] = node.transform.InverseTransformPoint(node.transform.position - twoDirection * 8.5f);
                tempVerts[4] = node.transform.InverseTransformPoint(node.transform.position - twoDirection * 8.5f);

                List<Terrain> vertTerrains = new List<Terrain>(6);
                for (int i = 0; i < 6; i++)
                {
                    vertTerrains.Add(new Terrain());
                }

                
                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[1]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[1] = new Vector3(tempVerts[1].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[1].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[3]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[3] = new Vector3(tempVerts[3].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[3].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[0]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[0] = new Vector3(tempVerts[0].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[0].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[2]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[2] = new Vector3(tempVerts[2].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[2].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[5]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[5] = new Vector3(tempVerts[5].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[5].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[4]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[4] = new Vector3(tempVerts[4].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[4].z);
                    }
                }

                if (tempVerts[1].y >= tempVerts[0].y)
                {
                    tempVerts[0] = new Vector3(tempVerts[0].x, tempVerts[1].y, tempVerts[0].z);
                }
                else
                {
                    tempVerts[1] = new Vector3(tempVerts[1].x, tempVerts[0].y, tempVerts[1].z);
                }

                if (tempVerts[3].y >= tempVerts[2].y)
                {

                    tempVerts[2] = new Vector3(tempVerts[2].x, tempVerts[3].y, tempVerts[2].z);
                }
                else
                {
                    tempVerts[3] = new Vector3(tempVerts[3].x, tempVerts[2].y, tempVerts[3].z);
                }

                if (tempVerts[5].y >= tempVerts[4].y)
                {
                    tempVerts[4] = new Vector3(tempVerts[4].x, tempVerts[5].y, tempVerts[4].z);
                }
                else
                {
                    tempVerts[5] = new Vector3(tempVerts[5].x, tempVerts[4].y, tempVerts[5].z);
                }

                roadMesh[0].vertices = tempVerts.ToArray();

                // меш 2
                tempVerts = roadMesh[1].vertices.ToList();
                tempVerts[0] = node.transform.InverseTransformPoint(node.transform.position + oneDirection * 8.5f);
                tempVerts[1] = node.transform.InverseTransformPoint(node.transform.position + twoDirection * 8.5f);
                tempVerts[2] = node.transform.InverseTransformPoint(node.transform.position + oneDirection * 8.5f);
                tempVerts[3] = node.transform.InverseTransformPoint(node.transform.position + twoDirection * 8.5f);
                tempVerts[4] = node.transform.InverseTransformPoint(node.transform.position + oneDirection * 8.5f);
                tempVerts[5] = node.transform.InverseTransformPoint(node.transform.position + twoDirection * 8.5f);
                
                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[1]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[1] = new Vector3(tempVerts[1].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[1].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[3]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[3] = new Vector3(tempVerts[3].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[3].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[0]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[0] = new Vector3(tempVerts[0].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[0].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[2]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[2] = new Vector3(tempVerts[2].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[2].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[5]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[5] = new Vector3(tempVerts[5].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[5].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[4]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[4] = new Vector3(tempVerts[4].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[4].z);
                    }
                }

                if (tempVerts[1].y >= tempVerts[0].y)
                    tempVerts[0] = new Vector3(tempVerts[0].x, tempVerts[1].y, tempVerts[0].z);
                else
                    tempVerts[1] = new Vector3(tempVerts[1].x, tempVerts[0].y, tempVerts[1].z);

                if (tempVerts[3].y >= tempVerts[2].y)
                    tempVerts[2] = new Vector3(tempVerts[2].x, tempVerts[3].y, tempVerts[2].z);
                else
                    tempVerts[3] = new Vector3(tempVerts[3].x, tempVerts[2].y, tempVerts[3].z);

                if (tempVerts[5].y >= tempVerts[4].y)
                    tempVerts[4] = new Vector3(tempVerts[4].x, tempVerts[5].y, tempVerts[4].z);
                else
                    tempVerts[5] = new Vector3(tempVerts[5].x, tempVerts[4].y, tempVerts[5].z);

                roadMesh[1].vertices = tempVerts.ToArray();

                // меш 3
                tempVerts = roadMesh[2].vertices.ToList();
                tempVerts[0] = node.transform.InverseTransformPoint(node.transform.position + twoDirection * 8.5f);
                tempVerts[2] = node.transform.InverseTransformPoint(node.transform.position + twoDirection * 8.5f);
                tempVerts[4] = node.transform.InverseTransformPoint(node.transform.position + twoDirection * 8.5f);
                tempVerts[1] = node.transform.InverseTransformPoint(node.transform.position - oneDirection * 8.5f);
                tempVerts[3] = node.transform.InverseTransformPoint(node.transform.position - oneDirection * 8.5f);
                tempVerts[5] = node.transform.InverseTransformPoint(node.transform.position - oneDirection * 8.5f);

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[1]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[1] = new Vector3(tempVerts[1].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[1].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[3]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[3] = new Vector3(tempVerts[3].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[3].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[0]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[0] = new Vector3(tempVerts[0].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[0].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[2]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[2] = new Vector3(tempVerts[2].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[2].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[5]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[5] = new Vector3(tempVerts[5].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[5].z);
                    }
                }

                if (Physics.Raycast(node.transform.TransformPoint(tempVerts[4]) + Vector3.up * 15f, -Vector3.up,
                    out hit, 400f, 1))
                {
                    if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Roads"))
                    {
                        tempVerts[4] = new Vector3(tempVerts[4].x, node.transform.InverseTransformPoint(hit.point).y + 0.1f, tempVerts[4].z);
                    }
                }

                if (tempVerts[1].y >= tempVerts[0].y)
                    tempVerts[0] = new Vector3(tempVerts[0].x, tempVerts[1].y, tempVerts[0].z);
                else
                    tempVerts[1] = new Vector3(tempVerts[1].x, tempVerts[0].y, tempVerts[1].z);

                if (tempVerts[3].y >= tempVerts[2].y)
                    tempVerts[2] = new Vector3(tempVerts[2].x, tempVerts[3].y, tempVerts[2].z);
                else
                    tempVerts[3] = new Vector3(tempVerts[3].x, tempVerts[2].y, tempVerts[3].z);

                if (tempVerts[5].y >= tempVerts[4].y)
                    tempVerts[4] = new Vector3(tempVerts[4].x, tempVerts[5].y, tempVerts[4].z);
                else
                    tempVerts[5] = new Vector3(tempVerts[5].x, tempVerts[4].y, tempVerts[5].z);

                roadMesh[2].vertices = tempVerts.ToArray();
                
                roadMesh[1].RecalculateNormals();
                roadMesh[2].RecalculateNormals();

                // сшивание

                verts = roadMesh[0].vertices.ToList();
                verts[roadMesh[0].vertexCount - 1] = roadMesh[0].vertices[4];
                verts[roadMesh[0].vertexCount - 3] = roadMesh[0].vertices[1];
                verts[roadMesh[0].vertexCount - 2] = roadMesh[2].vertices[1];
                verts[roadMesh[0].vertexCount - 4] = roadMesh[1].vertices[1];
                roadMesh[0].vertices = verts.ToArray();

                verts = roadMesh[1].vertices.ToList();
                verts[0] = roadMesh[0].vertices[1];
                verts[2] = roadMesh[0].vertices[3];
                verts[4] = roadMesh[0].vertices[5];
                roadMesh[1].vertices = verts.ToArray();

                verts = roadMesh[2].vertices.ToList();
                verts[0] = roadMesh[1].vertices[1];
                verts[2] = roadMesh[1].vertices[3];
                verts[4] = roadMesh[1].vertices[5];
                roadMesh[2].vertices = verts.ToArray();

                roadMesh[0].RecalculateNormals();
                roadMesh[1].RecalculateNormals();
                roadMesh[2].RecalculateNormals();


                if (node.roadConnectType == RoadConnectType.Crossroad4)
                {
                    tempVerts = roadMesh[3].vertices.ToList();
                    tempVerts[0] = roadMesh[2].vertices[1];
                    tempVerts[1] = roadMesh[0].vertices[0];
                    tempVerts[2] = roadMesh[2].vertices[1];
                    tempVerts[3] = roadMesh[0].vertices[0];
                    tempVerts[4] = roadMesh[2].vertices[1];
                    tempVerts[5] = roadMesh[0].vertices[0];

                    roadMesh[3].vertices = tempVerts.ToArray();

                    roadMesh[3].RecalculateNormals();


                }


                break;
        }



        CombineInstance[] combine = new CombineInstance[nodes.Count];
        string meshName = node.transform.position.ToString() + node.roadConnectType.ToString();
        Mesh resultMesh = new Mesh();

        for (int j = 0; j < nodes.Count; j++)
        {
            meshName += "_" + nodes[j].transform.position.ToString() + nodes[j].roadConnectType.ToString();
            combine[j].mesh = roadMesh[j];
        }

        resultMesh.CombineMeshes(combine, true, false);

        AssetDatabase.CreateAsset(resultMesh, "Assets/GenRoads/" + meshName + ".asset");
        AssetDatabase.SaveAssets();

        node.GetComponent<MeshFilter>().sharedMesh = resultMesh;
        node.GetComponent<MeshCollider>().sharedMesh = resultMesh;

        node.gameObject.layer = LayerMask.NameToLayer("Roads");
    }

    Vector2 TerrainCoords(Vector3 worldPos, Terrain terrain)
    {
        Vector3 terrainLocalPos = worldPos - terrain.transform.position;
        Vector3 scale = terrain.terrainData.heightmapScale;
        Debug.Log(terrain.transform.position + " - " + worldPos + " = " + terrainLocalPos);
        return new Vector2((int) (terrainLocalPos.x/scale.x), (int) (terrainLocalPos.y/scale.z));
    }

    int[] GenTris(int vertCount, int iterations)
    {
        if (vertCount > 3)
        {
            int[] triangles = new int[iterations * 6];

            int index = 0;
            for (int y = 0; y < iterations; y++)
            {
                triangles[index] = (y * 2);
                triangles[index + 1] = ((y + 1) * 2);
                triangles[index + 2] = (y * 2) + 1;

                triangles[index + 3] = ((y + 1) * 2);
                triangles[index + 4] = ((y + 1) * 2) + 1;
                triangles[index + 5] = (y * 2) + 1;
                index += 6;
            }

            return triangles;
        }
        else
        {
            throw new ArgumentException("vertCount can't be less than 3");
        }
    }

    Vector2[] GenUVs(RoadConnectType type, int iterations)
    {
        Vector2[] uv = new Vector2[(iterations+1) * 2];
        int index = 0;
        switch (type)
        {
            case RoadConnectType.Crossroad4:
            case RoadConnectType.Crossroad3:
                index = 0;
                for (int j = 0; j < iterations + 1; j++)
                {
                    if (j%2 == 0)
                    {
                        uv[index] = new Vector2(0.51f, 0.38f);
                        uv[index + 1] = new Vector2(0.99f, 0.38f);
                    }
                    else
                    {
                        uv[index] = new Vector2(0.51f, 0.49f);
                        uv[index + 1] = new Vector2(0.99f, 0.49f);
                    }
                    index += 2;
                }
                break;
            case RoadConnectType.Normal:
                index = 0;
                for (int j = 0; j < iterations + 1; j++)
                {
                    if (j%2 == 0)
                    {
                        uv[index] = new Vector2(0.51f, 0.38f);
                        uv[index + 1] = new Vector2(0.99f, 0.38f);
                    }
                    else
                    {
                        uv[index] = new Vector2(0.51f, 0.49f);
                        uv[index + 1] = new Vector2(0.99f, 0.49f);
                    }
                    index += 2;
                }
                break;
            case RoadConnectType.Stop:
                index = 0;
                for (int j = 0; j < iterations-1; j++)
                {
                    if (j % 2 == 0)
                    {
                        uv[index] = new Vector2(0.51f, 0.38f);
                        uv[index + 1] = new Vector2(0.99f, 0.38f);
                    }
                    else
                    {
                        uv[index] = new Vector2(0.51f, 0.49f);
                        uv[index + 1] = new Vector2(0.99f, 0.49f);
                    }
                    index += 2;
                }

                uv[index] = new Vector2(0.51f, 0.12f);
                uv[index + 1] = new Vector2(0.99f, 0.12f);
                index += 2;
                uv[index] = new Vector2(0.51f, 0f);
                uv[index + 1] = new Vector2(0.99f, 0f);
                break;
        }
        

        return uv;
    }

    Vector3[] GenNormals(int iterations)
    {
        Vector3[] normals = new Vector3[(iterations+1) * 2];

        int index = 0;
        for (int j = 0; j < iterations+1; j++)
        {
            normals[index] = -Vector3.forward;
            normals[index + 1] = -Vector3.forward;
            index += 2;
        }

        return normals;
    }

    Vector3 Rotate(Vector3 vector, Vector3 helper, float angle, float scale)
    {
        return
            Matrix4x4.TRS(vector, Quaternion.Euler(0, angle, 0), new Vector3(scale, scale, scale))
                .MultiplyPoint3x4(helper);
    }
#else
    public void VizualizeRoadNode(Vector3 position)
    {
        // nothing here
    }

    public void VizualizeRoadLinks(Vector3 transformPosition, List<RoadNode> nodes, List<Vector3> helpers)
    {
        // and here
    }
#endif

    
}
