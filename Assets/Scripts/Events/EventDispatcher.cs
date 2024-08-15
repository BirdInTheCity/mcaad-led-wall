using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventDispatcher : MonoBehaviour
{
    private static EventDispatcher instance;
    private Dictionary<string, List<Action<object>>> subscribers = new Dictionary<string, List<Action<object>>>();

    public static EventDispatcher Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<EventDispatcher>();

                if (instance == null)
                {
                    instance = new GameObject("EventDispatcher").AddComponent<EventDispatcher>();
                }
            }

            return instance;
        }
    }

    public void Subscribe(string eventName, Action<object> callback)
    {
        if (!subscribers.ContainsKey(eventName))
        {
            subscribers[eventName] = new List<Action<object>>();
        }
        subscribers[eventName].Add(callback);
    }

    public void Unsubscribe(string eventName, Action<object> callback)
    {
        if (subscribers.ContainsKey(eventName))
        {
            subscribers[eventName].Remove(callback);
        }
    }

    public void Dispatch(string eventName, object data)
    {
        if (subscribers.ContainsKey(eventName))
        {
            foreach (Action<object> callback in subscribers[eventName])
            {
                callback(data);
            }
        }
    }
}