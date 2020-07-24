using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagBag : MonoBehaviour
{
    static public FlagBag Instance;

    public List<string> bag = new List<string>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddFlag(string name)
    {
        bag.Add(name);
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
    }

    public void DelFlagsWithPrefix(string prefix)
    {
        //var tmp = new List<string>();
        //foreach (var flag in bag)
        //{
        //    if (flag.StartsWith(prefix))
        //    {
        //        tmp.Add(flag);
        //    }
        //}
        //foreach (var flag in tmp)
        //{
        //    bag.Remove(flag);
        //}

        bag.RemoveAll((string flag) => { return flag.StartsWith(prefix); });
    }
}
