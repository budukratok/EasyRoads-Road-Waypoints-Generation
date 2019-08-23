using System.Collections.Generic;
using UnityEngine;
// ReSharper disable CheckNamespace

public enum RoadStatus
{
    Move,
    Stop
}

public interface INode
{
    /// <summary>
    /// Specifies location Id for cities
    /// </summary>
    int LocationId { get; }

    /// <summary>
    /// Status
    /// </summary>
    RoadStatus Status { get; set; }

    /// <summary>
    /// Maximum speed
    /// </summary>
    float MaxSpeed { get; set; }

    /// <summary>
    /// Returns distance to connected node
    /// </summary>
    /// <param name="i">Index of connected node</param>
    /// <returns>Distance to connected node</returns>
    float GetDistanceToNode(int i);

    /// <summary>
    /// Returns distance to connected node
    /// </summary>
    /// <param name="node">Connected node</param>
    /// <returns>Distance to connected node</returns>
    float GetDistanceToNode(INode node);

    /// <summary>
    /// List of connected nodes
    /// </summary>
    List<INode> Nodes { get; set; }

    /// <summary>
    /// Positions of Bezier helper points
    /// </summary>
    List<Vector3> Helpers { get; set; }

    List<GraphNode> GetConnectedNodes();
    List<Vector3> GetLinksBezierHelpers();

    Vector3 Position { get; }
    Transform Transform { get; }
}
