using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOperatorOnPlayerEnter : EventOperator
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ExecuteOnConditions();
        }
    }
}
