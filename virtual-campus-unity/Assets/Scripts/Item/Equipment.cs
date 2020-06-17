using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : Usable
{
    protected bool activated;           //是否启用

    public override void Use()
    {
        //remainingCooldown = cooldown + 1;
        //available = false;
        Apply();
    }

    public virtual void Apply()
    {

    }
    public virtual void End()
    {

    }

    protected override void Update()
    {
        base.Update();
    }
}
