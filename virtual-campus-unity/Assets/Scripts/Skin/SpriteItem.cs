using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteItem : Item
{
    public SpriteItem(Item item)
    {
        itemName = item.itemName;
        description = item.description;
        image = item.image;
    }
}
