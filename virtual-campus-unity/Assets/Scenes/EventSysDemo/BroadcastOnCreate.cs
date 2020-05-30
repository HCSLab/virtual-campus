using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroadcastOnCreate : MonoBehaviour
{
    private void Start()
    {
        EventCenter.Broadcast("create_" + gameObject.name, this.gameObject);
    }
}
