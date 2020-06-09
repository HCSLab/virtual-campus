using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBag : MonoBehaviour
{
    public static ItemBag Instance;

    public GameObject displayPrefab;

    public RectTransform layout;

    private List<ItemBox> itemBoxs = new List<ItemBox>();

    public Image detailImage;
    public Text detailName;
    public Text detailDescription;

    public List<GameObject> testItems = new List<GameObject>();

    protected Item currentItem;

    protected virtual void Awake()
    {
        Instance = this;
    }

    protected virtual void Start()
    {
        for (int i = 0; i< 10; i++)
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
        display.transform.parent = layout;
    }

    public void Select(Item item)
    {
        detailImage.sprite = item.image;
        detailName.text = item.itemName;
        detailDescription.text = item.description;
        currentItem = item;
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
    
    //old methord
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
    }

    public void Reload()
    {
        foreach (var box in itemBoxs)
        {
            Destroy(box.gameObject);
        }
        for (int i = 0; i < 10; i++)
        {
            foreach (var item in testItems)
            {
                Add(item);
            }
        }
    }
}
