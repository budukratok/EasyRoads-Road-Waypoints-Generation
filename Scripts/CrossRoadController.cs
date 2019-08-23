using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// ReSharper disable CheckNamespace
// ReSharper disable ForCanBeConvertedToForeach

[System.Serializable]
public class GraphNodeListWrapper
{
    public string Name;
    public List<GraphNode> RightSideNodes;

    public GraphNodeListWrapper(string name, int capacity)
    {
        Name = name;
        RightSideNodes = new List<GraphNode>(capacity);
        for (var i = 0; i < RightSideNodes.Capacity; i++)
        {
            this[i] = new GraphNode();
        }
    }

    public GraphNode this[int index]
    {
        get
        {
            GraphNode node = null;

            if (index >= 0 && index <= RightSideNodes.Count - 1)
            {
                node = RightSideNodes[index];
            }
            return node;
        }
        set
        {
            if (index >= 0 && index <= RightSideNodes.Count - 1)
            {
                RightSideNodes[index] = value;
            }
            if (index >= RightSideNodes.Count)
            {
                RightSideNodes.Add(value);
            }
        }
    }

    public int Count()
    {
        return RightSideNodes.Count;
    }
}

public enum ByteShiftDirection
{
    Left,
    Right
}

[System.Serializable]
public class CrossRoadController : MonoBehaviour
{
    public float ActualLightsTimeout = 3;
    public float RedGreenLightsTimeout = 3;
    public float YellowLightsTimeout = 6;

    public TrafficLightState ActualState; // for front/back roads
    public TrafficLightState NextState = TrafficLightState.Red; // for front/back roads
    public ByteShiftDirection ShiftDirection = ByteShiftDirection.Left;

    public List<GraphNodeListWrapper> Nodes = new List<GraphNodeListWrapper>();

    private float _lastRegulateTime = 0f, _nextRegulateTime = 0f;

	// Use this for initialization
	void Start ()
	{
	    StartCoroutine(RegulateMovement());
    }

    private void MovementControl()
    {
        ActualState = NextState;
        for (var i = 0; i < Nodes.Count; i++)
        {
            if (i % 2 == 0)
            {
                MovementSwitcher(i);
            }
            else
            {
                MovementSwitcher(i, true);
            }
        }

        if (((byte)ActualState << 1) > (byte)TrafficLightState.Green)
        {
            ShiftDirection = ByteShiftDirection.Right;
        }
        else if(((byte)ActualState >> 1) < (byte)TrafficLightState.Red)
        {
            ShiftDirection = ByteShiftDirection.Left;
        }

        switch (ShiftDirection)
        {
            case ByteShiftDirection.Left:
                NextState = (TrafficLightState) ((byte)ActualState << 1);
                break;   
            case ByteShiftDirection.Right:
                NextState = (TrafficLightState) ((byte)ActualState >> 1);
                break;
        }

        _lastRegulateTime = Time.time;
        _nextRegulateTime = Time.time + ActualLightsTimeout;
    }

    private void MovementSwitcher(int index, bool needReverse = false)
    {
        var tmpState = NextState;

        if (needReverse)
        {
            tmpState = (TrafficLightState) ReverseByteBits((byte) tmpState, 3);
        }
        switch (tmpState)
        {
            case TrafficLightState.Red:
                StopMovement(Nodes[index]);
                ActualLightsTimeout = RedGreenLightsTimeout;
                break;
            case TrafficLightState.Yellow:
                StopMovement(Nodes[index]);
                ActualLightsTimeout = YellowLightsTimeout;
                break;
            case TrafficLightState.Green:
                StartMovement(Nodes[index]);
                ActualLightsTimeout = RedGreenLightsTimeout;
                break;
        }
    }

    private void StopMovement(GraphNodeListWrapper nodes)
    {
        for (var i = 0; i < nodes.Count(); i++)
        {
            nodes[i].Status = RoadStatus.Stop;
        }
    }

    private void StartMovement(GraphNodeListWrapper nodes)
    {
        for (var i = 0; i < nodes.Count(); i++)
        {
            nodes[i].Status = RoadStatus.Move;
        }
    }

    /// <summary>
    /// Some brutal stuff %)
    /// http://forums.codeguru.com/showthread.php?441771-Mirror-byte post #4
    /// </summary>
    /// <param name="value">Byte for mirroring</param>
    /// <param name="bitesCount">How many bits from this byte will be mirrored</param>
    /// <returns>Mirrored byte (partly or complete)</returns>
    private byte ReverseByteBits(byte value, int bitesCount = 8)
    {
        byte ret = 0;
        for (var i = 0; i < bitesCount; i++)
        {
            if ((value & (byte) (1 << i)) != 0)
            {
                ret += (byte)(1 << ((bitesCount - 1) - i));
            }
        }
        return ret;
    }

    private IEnumerator RegulateMovement()
    {
        while (Application.isPlaying)
        {
            MovementControl();
            yield return new WaitForSeconds(ActualLightsTimeout);
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position + Vector3.up * 6f, 1f);

        int count = (int) (_nextRegulateTime - Time.time);
        float offset = (count*0.6f)/2f;

        float offsetStep = -offset;

        Gizmos.color = Color.cyan;
        for (int i = 0; i < count; i++)
        {
            Gizmos.DrawSphere(transform.position + Vector3.up*7f + Vector3.left*(offsetStep), 0.25f);
            offsetStep += 0.6f;
        }

        for (int i = 0; i < Nodes.Count; i++)
        {
            for (int j = 0; j < Nodes[i].Count(); j++)
            {
                Gizmos.color = Nodes[i][j].Status == RoadStatus.Move ? Color.green : Color.red;
                Gizmos.DrawLine(transform.position + Vector3.up * 6f, Nodes[i][j].transform.position);
                Gizmos.DrawSphere(Nodes[i][j].transform.position+Vector3.up*6f, 1f);
            }
        }
    }
}
