using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cola : Consumable
{
    public float walkSpeedIncrement;
    public float SprintSpeedIncrement;
    public override void Apply()
    {
        base.Apply();
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonAIO>().walkSpeed += walkSpeedIncrement;
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonAIO>().sprintSpeed += SprintSpeedIncrement;
    }

    public override void End()
    {
        base.End();
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonAIO>().walkSpeed -= walkSpeedIncrement * level;
        GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonAIO>().sprintSpeed -= SprintSpeedIncrement * level;
    }
}
