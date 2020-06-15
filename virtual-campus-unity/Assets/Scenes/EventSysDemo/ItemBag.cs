using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBag : Bag
{
    public static ItemBag Instance;
    protected virtual void Awake()
    {
        Instance = this;
    }

    public void UseButtonClicked()
    {
        Consumable c = (Consumable)currentItem;
        c.Use();
        GameObject number = transform.Find("Panel/Number").gameObject;
        if (c.number == 0)
        {
            Remove(c);
        }
        number.GetComponent<Text>().text = c.number.ToString();
    }

    public override void Select(Item item)
    {
        detailImage.sprite = item.image;
        detailName.text = item.itemName;
        detailDescription.text = item.description;
        currentItem = item;
        GameObject useButton = transform.Find("Panel/UseButton").gameObject;
        GameObject number = transform.Find("Panel/Number").gameObject;
        if (typeof(Consumable).IsInstanceOfType(currentItem))
        {
            Consumable c = (Consumable)currentItem;
            useButton.SetActive(true);
            number.SetActive(true);
            number.GetComponent<Text>().text = c.number.ToString();
        }
        else
        {
            useButton.SetActive(false);
            number.SetActive(false);
        }
    }
}
