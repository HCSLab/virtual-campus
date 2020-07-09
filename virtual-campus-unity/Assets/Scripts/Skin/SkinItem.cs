using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinItem : Item
{
    public Texture2D texture;
    public bool customized;
    public int level;
    public SkinItem(Item item)
    {
        itemName = item.itemName;
        description = item.description;
        image = item.image;
    }
}
