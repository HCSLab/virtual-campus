using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBox : MonoBehaviour
{
    public Item item;
    public Image image;
    public TextMeshProUGUI text;

    public void Init(Item i)
    {
        item = i;

        image.sprite = item.image;
        text.text = item.itemName;
    }

    public virtual void OnClick()
    {
        ItemBag.Instance.Select(item, this);
    }
}
