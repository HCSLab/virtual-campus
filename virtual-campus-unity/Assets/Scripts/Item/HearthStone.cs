using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HearthStone : Equipment
{
    public Vector3 position;

    public override void Apply()
    {
        base.Apply();
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        player.position = position;
    }

    public void setPosition(Vector3 newPosition)
    {
        position = newPosition;
    }
}
