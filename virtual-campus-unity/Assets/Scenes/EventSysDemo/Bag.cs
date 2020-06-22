using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bag : MonoBehaviour
{

    public GameObject displayPrefab;

    protected List<ItemBox> itemBoxs = new List<ItemBox>();

    public RectTransform layout;


    public Image detailImage;
    public Text detailName;
    public Text detailDescription;

    public List<GameObject> testItems = new List<GameObject>();

    protected Item currentItem;

    protected virtual void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            foreach (var item in testItems)
            {
                Add(item);
            }
        }
    }

    public virtual void Add(GameObject obj, bool copy = true)
    {
        var display = Instantiate(displayPrefab);
        var itemBox = display.GetComponent<ItemBox>();

        Item item;
        if (copy)
        {
            item = Instantiate(obj).GetComponent<Item>();
        }
        else
        {
            item = obj.GetComponent<Item>();
        }
        item.transform.parent = transform;

        itemBox.Init(item);

        itemBoxs.Add(itemBox);
        display.transform.SetParent(layout);
    }

    public virtual void Select(Item item)
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


    public void Remove(Item item, bool destroyItem = true)
    {
        foreach (var box in itemBoxs)
        {
            if (box.item == item)
            {
                Destroy(box.gameObject);
            }
        }
        if (destroyItem)
        {
            Destroy(item.gameObject);
        }
    }

    public void BagButtonPressed()
    {
        float alpha = GetComponent<CanvasGroup>().alpha;
        if (alpha == 1)
        {
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().interactable = false;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().interactable = true;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        //gameObject.SetActive(!gameObject.activeSelf);
        GetComponent<CanvasScaler>().enabled = true;
    }

    public void Reload()
    {
        GetComponent<CanvasScaler>().enabled = false;
        foreach (var box in itemBoxs)
        {
            Destroy(box.gameObject);
        }
        itemBoxs.Clear();
        for (int i = 0; i < 10; i++)
        {
            foreach (var item in testItems)
            {
                Add(item);
            }
        }
        GetComponent<CanvasScaler>().enabled = true;
    }

    public virtual void Select()
    {
        if (itemBoxs.Count > 0)
        {
            Select(itemBoxs[0].item);
        }
        else
        {
            ClearSelection();
        }
    }

    protected virtual void ClearSelection()
    {
        detailImage.sprite = null;
        detailName.text = null;
        detailDescription.text = null;
        currentItem = null;
    }
}
