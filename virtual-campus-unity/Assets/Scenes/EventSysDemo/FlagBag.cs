using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBag : MonoBehaviour
{
    static public FlagBag Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (bag.Count > 0)
        {
            StoryManager.Instance.refreshFlag = true;
        }
    }

    public List<string> bag = new List<string>();

    public void AddFlag(string name)
    {
        bag.Add(name);
        StoryManager.Instance.refreshFlag = true;
    }

    public bool HasFlag(string name)
    {
        return bag.Contains(name);
    }

    public bool HasFlags(List<string> list)
    {
        foreach (var flag in list)
        {
            if (!HasFlag(flag))
            {
                return false;
            }
        }
        return true;
    }

    public bool WithoutFlags(List<string> list)
    {
        foreach (var flag in list)
        {
            if (HasFlag(flag))
            {
                return false;
            }
        }
        return true;
    }

    public void DelFlag(string name)
    {
        bag.Remove(name);
        StoryManager.Instance.refreshFlag = true;
    }
}
