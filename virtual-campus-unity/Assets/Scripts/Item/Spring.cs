using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : Consumable
{
    public float jumpSpeedIncrement;
    public override void Use()
    {
        base.Use();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().jumpSpeed += jumpSpeedIncrement;
    }

    public override void End()
    {
        base.End();
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().jumpSpeed -= jumpSpeedIncrement;
    }
}
