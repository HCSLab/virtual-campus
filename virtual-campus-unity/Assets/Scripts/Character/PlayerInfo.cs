using Ink.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static Dictionary<string, string> info = new Dictionary<string, string>();

    private void Awake()
    {
        info["name"] = "test player";
        info["id"] = "119010000";
        info["gender"] = "female";
        info["school"] = "SME";
        info["collage"] = "Shaw";
    }

    public static void WriteToInkStory(Story story)
    {
        foreach (var i in info)
        {
            if (story.variablesState.Contains(i.Key))
            {
                story.variablesState[i.Key] = i.Value;
            }
        }
    }

    public static void UpdateFromInkStory(Story story)
    {
        foreach (var key in info.Keys)
        {
            if (story.variablesState.Contains(key))
            {
                info[key] = (string)story.variablesState[key];
            }
        }
    }
}
