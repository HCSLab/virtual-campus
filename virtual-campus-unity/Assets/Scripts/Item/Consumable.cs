using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item
{
    public int number;
    public int maxPerStack;
    public float duration;
    public float cooldown;
    public bool available;
    protected float remainingTime;

    public virtual void Use()
    {
        number--;
        remainingTime += duration;
    }

    public virtual void End()
    {

    }

    void Update()
    {
        if (remainingTime <= 0)
            return;
        remainingTime -= Time.deltaTime * 1000;
        //Debug.Log(remainingTime);
        if (remainingTime <= 0)
        {
            End();
        }
    }
}
