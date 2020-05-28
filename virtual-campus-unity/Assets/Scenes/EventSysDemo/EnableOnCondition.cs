using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class EnableOnCondition : MonoBehaviour
{
    public List<string> hasFlag = new List<string>();

    private void Start()
    {
        if (CheckConditions())
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public bool CheckConditions()
    {
        foreach (var flag in hasFlag)
        {
            if (! FlagBag.Instance.HasFlag(flag))
            {
                return false;
            }
        }
        return true;
    }
}
