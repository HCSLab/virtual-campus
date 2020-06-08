using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinBag : ItemBag
{
    new public static SkinBag Instance;

    public int index;

    public void OnSaveClicked()
    {
        if (currentItem != null)
        {
            SpriteItem spriteItem = new SpriteItem(currentItem);
            GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Mapping>().ChangeMapping(spriteItem);
        }
        BagButtonPressed();
    }

    public void OnCancelClicked()
    {
        BagButtonPressed();
    }
}