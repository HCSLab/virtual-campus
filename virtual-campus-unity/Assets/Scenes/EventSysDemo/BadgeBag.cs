using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BadgeBag : MonoBehaviour
{
    public static BadgeBag Instance;

    public GameObject displayPrefab;

    public RectTransform layout;

    private List<ItemBox> itemBoxs = new List<ItemBox>();

    public List<GameObject> testItems = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
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
        display.transform.parent = layout;
    }

    public void Select(Item item)
    {
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
}
