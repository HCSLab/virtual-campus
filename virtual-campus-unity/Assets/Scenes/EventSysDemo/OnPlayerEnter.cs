using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPlayerEnter : EventOperator
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (CheckPreconditions())
            {
                ExecuteOperations();
            }
        }
    }
}
