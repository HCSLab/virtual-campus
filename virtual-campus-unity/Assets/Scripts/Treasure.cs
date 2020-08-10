using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public string type;
    public string itemName;
    
    public void Open()
    {
        if (type == "skin")
        {
            ItemPanel.Instance.AddSkin(itemName);
            FlagBag.Instance.AddFlag("skin_" + itemName);
        }
        else if (type == "item")
        {
            ItemPanel.Instance.AddSkin(itemName);
            FlagBag.Instance.AddFlag("item_" + itemName);
        }
        gameObject.SetActive(false);
    }
}
