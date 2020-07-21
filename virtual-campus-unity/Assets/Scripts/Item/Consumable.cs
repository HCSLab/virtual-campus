using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Usable
{
    public int amount;                  //物品数量
    public int maxPerStack;             //堆叠数量
    public float duration;              //持续时间
    public bool superposable;           //可叠加性
    public int maxLevel;                //最高层数
    protected float remainingTime = 0;  //剩余时间
    protected int level = 0;            //叠加层数

    public override void Use()
    {
        amount--;
        remainingTime += duration;

        if (!superposable)
        {
            if (level == 0)
            {
                level = 1;
                Apply();
            }
        }

        else if (superposable)
        {
            level++;
            if (level > maxLevel)
            {
                level = maxLevel;
                return;
            }
            Apply();
        }
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

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime * 1000;
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                End();
                level = 0;
                if (amount == 0)
                {
                    Destroy(gameObject);
                    //Debug.Log("DESTORY");
                }
            }
        }

    }
}
