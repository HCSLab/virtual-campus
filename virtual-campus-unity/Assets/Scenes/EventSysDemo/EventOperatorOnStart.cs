using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventOperatorOnStart : EventOperator
{
    private void Start()
    {
        ExecuteOnConditions();
    }
}
