using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : Consumable
{
    public float jumpSpeedIncrement;

    public override void Apply()
    {
        base.Apply();
        GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>().jumpPower += jumpSpeedIncrement;
    }

    public override void End()
    {
        base.End();
        GameObject.FindGameObjectWithTag("Player").GetComponent<ScriptedFirstPersonAIO>().jumpPower -= jumpSpeedIncrement * level;
    }
}
