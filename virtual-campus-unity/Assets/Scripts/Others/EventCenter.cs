using System;
using System.Collections.Generic;

public class EventCenter
{
    private static Dictionary<string, Action<object>> eventDict = new Dictionary<string, Action<object>>();

    public static void AddListener(string eventType, Action<object> callback)
    {
        if (! eventDict.ContainsKey(eventType))
        {
            eventDict.Add(eventType, null);
        }
        eventDict[eventType] += callback;
    }

    public static void RemoveListener(string eventType, Action<object> callback)
    {
        if (! eventDict.ContainsKey(eventType))
            return;

        if (eventDict[eventType] == null)
            return;

        eventDict[eventType] -= callback;
    }

    public static void Broadcast(string eventType, object data)
    {
        if (! eventDict.ContainsKey(eventType))
            return;

        eventDict[eventType]?.Invoke(data);
    }
}