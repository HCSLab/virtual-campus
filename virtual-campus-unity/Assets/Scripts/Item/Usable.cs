using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Usable : Item
{
    public float cooldown;              //冷却时间
    //public bool available;              //可用性
    //protected float remainingCooldown;  //剩余冷却时间

    public virtual void Use()
    {

    }
    protected virtual void Update()
    {
        /*
        if (remainingCooldown > 0)
        {
            remainingCooldown -= Time.deltaTime * 1000;
            if (remainingCooldown <= 0)
            {
                remainingCooldown = 0;
                available = true;
            }
        }
        */
    }

}
