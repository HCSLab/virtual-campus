using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public GameObject item;

    public void OnClick()
    {
        ItemBag.Instance.Add(item);
    }
}
