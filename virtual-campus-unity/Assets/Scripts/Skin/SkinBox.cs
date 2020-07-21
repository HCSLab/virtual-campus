using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinBox : ItemBox
{
    public override void OnClick()
    {
        SkinBag.Instance.Select(item, this);
    }

    public override void OnRightClick()
    {
        SkinBag.Instance.RightSelect(this);
    }
}
