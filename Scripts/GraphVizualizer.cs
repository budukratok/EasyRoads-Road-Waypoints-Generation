using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor; // only for editor
#endif
using UnityEngine;
using System.Collections;

public class GraphVizualizer {
    
#if UNITY_EDITOR
    private readonly Color orange = new Color(255f / 255f, 117f / 255f, 24f / 255f);
    private readonly Color green = new Color(52f / 255f, 201f / 255f, 36f / 255f);


    /// <summary>
    /// Call this only in OnDrawGizmos() MonoBehaviour method
    /// </summary>
    public void VizualizeGraphNode(Vector3 position, RoadStatus status)
    {
        Gizmos.color = (status == RoadStatus.Move) 
            ? Color.cyan 
            : Color.red;
        Gizmos.DrawSphere(position, 1f);
    }

    public void VizualizeGraphLinks(Vector3 transformPosition, List<GraphNode> nodes, List<Vector3> helpers)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            int definition = (nodes[i].OverrideDefinition ? nodes[i].OverrideDefinitionValue : Bezier.BezierIterationsCount);
            if (i+1 <= helpers.Count)
            {
                for (int j = 1; j < definition+1; j++)
                {
                    if (nodes[i] != null && helpers[i] != null)
                    {
                        
                        if (nodes[i].MaxSpeed != 0)
                        {
                            Handles.color = Color.Lerp(Color.red, Color.green,
                                (nodes[i].MaxSpeed/35f) > 1f
                                    ? ((nodes[i].MaxSpeed/35f) < 2 ? (nodes[i].MaxSpeed/35f) - 1f : 0)
                                    : (nodes[i].MaxSpeed/35f));
                            Gizmos.color = Handles.color;
                        }
                        else
                        {
                            Handles.color = Color.red;
                            Gizmos.color = Handles.color;
                        }

                        if (!((helpers[i].y >= transformPosition.y - 1f &&
                             helpers[i].y <= (nodes[i]).transform.position.y + 1f) ||
                            (helpers[i].y <= transformPosition.y + 1f &&
                             helpers[i].y >= (nodes[i]).transform.position.y - 1f)))
                        {
                            Gizmos.color = orange;
                            Handles.color = Color.red;
                        }

                        //Arrows
                        if (j > 0)
                        {
                            Handles.ArrowCap(0,
                                Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                    (float) (j - 1)/(float)definition),
                                Quaternion.LookRotation((
                                Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                            helpers[i],
                                            ((float) j/(float)definition)) - 
                                        Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                            helpers[i],
                                            (float) (j - 1)/(float)definition)
                                        ).normalized),
                                        2f
                                );
                        }

                        Gizmos.DrawLine(
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                (float) (j - 1)/(float)definition),
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position, helpers[i],
                                ((float) j/(float)definition))
                            );
                    }
                }
            }
            else
            {
                for (int j = 1; j < definition+1; j++)
                {
                    if (nodes[i] != null)
                    {
                        Gizmos.DrawLine(
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                Bezier.CenterOf(transformPosition, (nodes[i]).transform.position),
                                (float) (j - 1)/(float)definition),
                            Bezier.GetPointPosition(transformPosition, (nodes[i]).transform.position,
                                Bezier.CenterOf(transformPosition, (nodes[i]).transform.position),
                                (float) j/(float)definition)
                            );
                    }
                }      
            }
        }
    }

    private Vector3 GetDirection(Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }
#else
    public void VizualizeGraphNode(Vector3 position)
    {
        // nothing here
    }

    public void VizualizeGraphLinks(Vector3 transformPosition, List<GraphNode> nodes, List<Vector3> helpers)
    {
        // and here
    }
#endif
}
