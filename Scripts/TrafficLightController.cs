using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// ReSharper disable CheckNamespace

[System.Serializable]
public class TrafficLightListWrapper
{
    public string Name;
    public List<TrafficLight> TrafficLights;

    public TrafficLightListWrapper(string name, int capacity)
    {
        Name = name;
        TrafficLights = new List<TrafficLight>(capacity);
        for (var i = 0; i < TrafficLights.Capacity; i++)
        {
            this[i] = new TrafficLight();
        }

    }

    public TrafficLight this[int index]
    {
        get
        {
            TrafficLight light = null;

            if (index >= 0 && index <= TrafficLights.Count - 1)
            {
                light = TrafficLights[index];
            }
            return light;
        }
        set
        {
            if (index >= 0 && index <= TrafficLights.Count - 1)
            {
                TrafficLights[index] = value;
            }
            if (index >= TrafficLights.Count)
            {
                TrafficLights.Add(value);
            }
        }
    }

    public int Count()
    {
        return TrafficLights.Count;
    }
}

[System.Serializable]
public class TrafficLightController : MonoBehaviour
{
    public List<TrafficLightListWrapper> TrafficLights = new List<TrafficLightListWrapper>(); 

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
