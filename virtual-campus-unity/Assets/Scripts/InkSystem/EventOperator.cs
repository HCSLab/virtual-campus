using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOperator : MonoBehaviour
{
    public List<string> hasFlags = new List<string>();

    public List<string> addFlags = new List<string>();
    public List<string> delFlags = new List<string>();

    public List<GameObject> createList = new List<GameObject>();
    public List<GameObject> enableList = new List<GameObject>();
    public List<GameObject> DestroyList = new List<GameObject>();

    public bool CheckConditions()
    {
        var fbag = FlagBag.Instance;
        foreach (var flag in hasFlags)
        {
            if (! fbag.HasFlag(flag))
            {
                return false;
            }
        }
        return true;
    }

    public void ExecuteOperations()
    {
        foreach (var flag in addFlags)
        {
            FlagBag.Instance.AddFlag(flag);
        }

        foreach (var flag in delFlags)
        {
            FlagBag.Instance.DelFlag(flag);
        }

        foreach (var obj in createList)
        {
            Instantiate(obj);
        }

        foreach (var obj in enableList)
        {
            obj.SetActive(true);
        }

        foreach (var obj in DestroyList)
        {
            if (obj)
            {
                Destroy(obj);
            }
        }
    }

    public bool ExecuteOnConditions()
    {
        if (CheckConditions())
        {
            ExecuteOperations();
            return true;
        }
        else
        {
            return false;
        }
    }
}
