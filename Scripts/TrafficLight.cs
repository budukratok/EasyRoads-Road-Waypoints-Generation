using UnityEngine;
using System.Collections;
// ReSharper disable CheckNamespace

public enum TrafficLightState : byte
{
    Red = (1 << 0),
    Yellow = (1 << 1),
    Green = (1 << 2)
}

[System.Serializable]
public class TrafficLight : MonoBehaviour
{
    public TrafficLightState State;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
