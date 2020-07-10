using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadgeBag : Bag
{
    public static BadgeBag Instance;

    private void Awake()
    {
        Instance = this;
    }

    public override void Select(Item item, ItemBox itemBox)
    {

    }
}