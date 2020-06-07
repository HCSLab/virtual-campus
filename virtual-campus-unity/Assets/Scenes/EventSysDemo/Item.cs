using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName;
    public string description;
    public Sprite image;

    public Item(string itemName, string description, Sprite image)
    {
        this.itemName = itemName;
        this.description = description;
        this.image = image;
    }

    public Item()
    {
        this.itemName = "";
        this.description = "";
        this.image = null;
    }
}
