using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBag : Bag
{
    public static ItemBag Instance;
    protected Dictionary<System.Type, bool> availability = new Dictionary<System.Type, bool>();
    protected Dictionary<System.Type, float> cooldown = new Dictionary<System.Type, float>();

    private void Awake()
    {
        Instance = this;
        //currentItem = itemBoxs[0].item;
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
            if (c.amount == 0)
            {
                Remove(c);
                ClearSelection();
            }
            amountText.GetComponent<TextMeshProUGUI>().text = c.amount.ToString();
        }
    }

    public void Reselect()
    {
        if (currentItem != null && currentItemBox != null)
        {
            Select(currentItem, currentItemBox);
        }
    }
    public override void Select(Item item, ItemBox itemBox)
    {
        base.Select(item, itemBox);
        detailImage.sprite = item.image;
        detailName.text = item.itemName;
        detailDescription.text = item.description;
        currentItem = item;
        currentItemBox = itemBox;
        if (typeof(Usable).IsInstanceOfType(currentItem))
        {
            
            Usable u = (Usable)currentItem;
            if (!availability.ContainsKey(u.GetType()))
            {
                availability[u.GetType()] = true;
            }
            if (!useButton.activeSelf)
                useButton.SetActive(true);
            amountText.SetActive(false);
            if (availability[u.GetType()] == false)
            {
                useButton.GetComponent<Button>().interactable = true;
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
                amountText.SetActive(true);
                amountText.GetComponent<TextMeshProUGUI>().text = c.amount.ToString();
            }
        }
        else
        {
            useButton.SetActive(false);
            amountText.SetActive(false);
        }
    }

    protected override void ClearSelection()
    {
        base.ClearSelection();
        useButton.SetActive(false);
        amountText.SetActive(false);
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
