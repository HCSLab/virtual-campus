using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBag : Bag
{
    public static ItemBag Instance;
    protected Dictionary<System.Type, bool> availability = new Dictionary<System.Type, bool>();
    protected Dictionary<System.Type, float> cooldown = new Dictionary<System.Type, float>();
    protected virtual void Awake()
    {
        Instance = this;
    }

    public void UseButtonClicked()
    {
        Usable u = (Usable)currentItem;
        u.Use();
        cooldown[u.GetType()] = u.cooldown + 1;
        availability[u.GetType()] = false;
        if (typeof(Consumable).IsInstanceOfType(currentItem))
        {
            Consumable c = (Consumable)currentItem;
            GameObject number = transform.Find("Panel/Number").gameObject;
            if (c.number == 0)
            {
                Remove(c, false);
                Select();
            }
            number.GetComponent<Text>().text = c.number.ToString();
        }
    }

    public override void Select(Item item, ItemBox itemBox)
    {
        detailImage.sprite = item.image;
        detailName.text = item.itemName;
        detailDescription.text = item.description;
        currentItem = item;
        GameObject useButton = transform.Find("Panel/UseButton").gameObject;
        GameObject number = transform.Find("Panel/Number").gameObject;
        if (typeof(Usable).IsInstanceOfType(currentItem))
        {
            
            Usable u = (Usable)currentItem;
            if (!availability.ContainsKey(u.GetType()))
            {
                availability[u.GetType()] = true;
            }
            if (!useButton.active)
                useButton.SetActive(true);
            number.SetActive(false);
            if (availability[u.GetType()])
            {
                useButton.GetComponent<Button>().enabled = false;
                useButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                useButton.GetComponent<Button>().enabled = true;
                useButton.GetComponent<Button>().interactable = true;
            }
            if (typeof(Consumable).IsInstanceOfType(currentItem))
            {
                Consumable c = (Consumable)currentItem;
                number.SetActive(true);
                number.GetComponent<Text>().text = c.number.ToString();
            }
        }
        else
        {
            useButton.SetActive(false);
            number.SetActive(false);
        }
    }

    protected override void ClearSelection()
    {
        base.ClearSelection();
        GameObject useButton = transform.Find("Panel/UseButton").gameObject;
        GameObject number = transform.Find("Panel/Number").gameObject;
        useButton.SetActive(false);
        number.SetActive(false);
    }

    private void Update()
    {
        Dictionary<System.Type, float> newCD = new Dictionary<System.Type, float>();

        foreach (System.Type type in cooldown.Keys)
        {
            float remainingCD = cooldown[type];
            if (remainingCD > 0)
            {
                remainingCD -= Time.deltaTime * 1000;
                if (remainingCD <= 0)
                {
                    remainingCD = 0;
                    availability[type] = true;
                }
            }
            newCD[type] = remainingCD;
        }

        cooldown = newCD;

        if (typeof(Consumable).IsInstanceOfType(currentItem))
        {
            GameObject useButton = transform.Find("Panel/UseButton").gameObject;
            Consumable c = (Consumable)currentItem;
            if (!availability[c.GetType()])
            {
                useButton.GetComponent<Button>().enabled = false;
                useButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                useButton.GetComponent<Button>().enabled = true;
                useButton.GetComponent<Button>().interactable = true;
            }
        }
        if (typeof(Equipment).IsInstanceOfType(currentItem))
        {
            GameObject useButton = transform.Find("Panel/UseButton").gameObject;
            Equipment e = (Equipment)currentItem;
            if (!availability[e.GetType()])
            {
                useButton.GetComponent<Button>().enabled = false;
                useButton.GetComponent<Button>().interactable = false;
            }
            else
            {
                useButton.GetComponent<Button>().enabled = true;
                useButton.GetComponent<Button>().interactable = true;
            }
        }
    }
}
