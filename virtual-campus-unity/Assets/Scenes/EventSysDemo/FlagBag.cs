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

    public List<string> bag = new List<string>();

    public void AddFlag(string name)
    {
        bag.Add(name);
    }

    public bool HasFlag(string name)
    {
        return bag.Contains(name);
    }

    public void DelFlag(string name)
    {
        bag.Remove(name);
    }
}
