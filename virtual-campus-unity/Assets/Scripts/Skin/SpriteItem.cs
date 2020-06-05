using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteItem : Item
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public SpriteItem (Item item)
    {
        itemName = item.itemName;
        description = item.description;
        image = item.image;

    }
}
