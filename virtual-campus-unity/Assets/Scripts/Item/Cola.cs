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
        GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>().walkSpeed += walkSpeedIncrement;
        GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>().sprintSpeed += SprintSpeedIncrement;
        GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>().Reload();
    }

    public override void End()
    {
        base.End();
        GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>().walkSpeed -= walkSpeedIncrement * level;
        GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>().sprintSpeed -= SprintSpeedIncrement * level;
        GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>().Reload();
    }
}
