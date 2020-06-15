using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    public Item item;
    public Image image;
    public Text text;

    public void Init(Item i)
    {
        item = i;

        image.sprite = item.image;
        text.text = item.itemName;
    }

    public virtual void OnClick()
    {
        ItemBag.Instance.Select(item);
    }
}
