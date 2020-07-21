using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bag : MonoBehaviour
{
	[Header("Debug")]
	public List<GameObject> testItems = new List<GameObject>();
	[Header("Prefab")]
	public GameObject elementDisplayPrefab;
	[Header("In the Scene")]
	public GameObject elementContainer;
	public Image detailImage;
	public TextMeshProUGUI detailName;
	public TextMeshProUGUI detailDescription;
	public GameObject useButton, amountText;

	[HideInInspector]
	public Item currentItem;

	protected ItemBox currentItemBox;
	protected List<ItemBox> itemBoxs = new List<ItemBox>();

    protected ItemBox itemBoxToSwap;
    protected bool toSwap = false;

    protected virtual void Start()
	{
		foreach (var item in testItems)
			Add(item);

		gameObject.SetActive(true);
	}

	/// <summary>Instantiate the prefab, and add it to this bag.</summary>
	public virtual void Add(GameObject itemPrefab)
	{
		var item = Instantiate(itemPrefab).GetComponent<Item>();
		item.transform.SetParent(transform);

		var display = Instantiate(elementDisplayPrefab);
		var itemBox = display.GetComponent<ItemBox>();
		itemBox.Init(item);
		itemBoxs.Add(itemBox);
		display.transform.SetParent(elementContainer.transform);
		display.transform.localScale = Vector3.one;
	}

    public virtual void Add(Item item)
    {
        var display = Instantiate(elementDisplayPrefab);
        var itemBox = display.GetComponent<ItemBox>();
        itemBox.Init(item);
        itemBoxs.Add(itemBox);
        display.transform.SetParent(elementContainer.transform);
        display.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Remove both the display and the GameObject of the item.
    /// </summary>
    /// <param name="item">The item to remove.</param>
    public void Remove(Item item)
	{
		ItemBox boxToRemove = null;
		foreach (var box in itemBoxs)
		{
			if (box.item == item)
			{
				Destroy(box.gameObject);
				boxToRemove = box;
			}
		}
		if (boxToRemove)
			itemBoxs.Remove(boxToRemove);
        for (int i = 0; i < elementContainer.transform.childCount; i++)
        {
            GameObject child = elementContainer.transform.GetChild(i).gameObject;
            if (child.GetComponent<ItemBox>().item == item)
            {
                Destroy(child.GetComponent<ItemBox>().gameObject);
            }
        }

        StartCoroutine(DelayedDestroyItem(300, item));
        //Destroy(item.gameObject);
	}

    private IEnumerator DelayedDestroyItem(int waitingTime, Item item)
    {
        yield return new WaitForSeconds(waitingTime);
        DestroyItem(item);
    }

    private void DestroyItem(Item item)
    {
        Destroy(item.gameObject);
    }
    public virtual void Reload()
	{
        Clear();

		foreach (var item in testItems)
			Add(item);
	}

    public virtual void Clear()
    {
        foreach (var box in itemBoxs)
            Destroy(box.gameObject);
        ClearLayout();
    }

    public void ClearLayout()
    {
        for (int i = elementContainer.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(elementContainer.transform.GetChild(i).gameObject);
        }
        itemBoxs.Clear();
    }

    public virtual void Select(Item item, ItemBox itemBox)
	{
        if (toSwap)
        {
            int n1 = -1;
            int n2 = -1;
            for (int i=0; i<itemBoxs.Count; i++)
            {
                if (itemBoxs[i] == itemBox)
                {
                    n1 = i;
                }
                if (itemBoxs[i] == itemBoxToSwap)
                {
                    n2 = i;
                }
                if (n1 != -1 && n2 != -1)
                {
                    break;
                }
            }
            itemBoxs[n1] = itemBoxToSwap;
            itemBoxs[n2] = itemBox;

            /*
            int n1 = -1;
            int n2 = -1;
            for (int i=0; i<elementContainer.transform.childCount; i++)
            {
                GameObject child = elementContainer.transform.GetChild(i).gameObject;
                if (child.GetComponent<ItemBox>() == itemBox)
                {
                    n1 = i;
                }
                if (child.GetComponent<ItemBox>() == itemBoxToSwap)
                {
                    n2 = i;
                }
                if (n1 != -1 && n2 != -1)
                {
                    break;
                }
            }
           */
            List<ItemBox> newBoxList = new List<ItemBox>(itemBoxs);
            ClearLayout();
            for (int i = 0; i < newBoxList.Count; i++)
            {
                Add(newBoxList[i].item);
            }
            toSwap = false;
        }
    }

	protected virtual void ClearSelection()
	{
		detailImage.sprite = null;
		detailName.text = null;
		detailDescription.text = null;
		currentItem = null;
	}

    public void SortByName()
    {
        for (int i=0; i<itemBoxs.Count; i++)
        {
            for (int j=i+1; j<itemBoxs.Count; j++)
            {
                if (string.Compare(itemBoxs[j].item.itemName, itemBoxs[i].item.itemName) == -1)
                {
                    var temp = itemBoxs[i];
                    itemBoxs[i] = itemBoxs[j];
                    itemBoxs[j] = temp;
                }
            }
        }
        List<ItemBox> newBoxList = new List<ItemBox>(itemBoxs);
        ClearLayout();
        for (int i=0; i<newBoxList.Count; i++)
        {
            Add(newBoxList[i].item);
        }
    }

    public void RightSelect(ItemBox itemBox)
    {
        for (int i = 0; i < elementContainer.transform.childCount; i++)
        {
            GameObject child = elementContainer.transform.GetChild(i).gameObject;
            if (child.GetComponent<ItemBox>() == itemBoxToSwap)
            {
                child.GetComponent<Image>().color = new Color((255 / 255f), (255 / 255f), (255 / 255f), (255 / 255f));
            }
            if (child.GetComponent<ItemBox>() == itemBox)
            {
                child.GetComponent<Image>().color = new Color((255 / 255f), (240 / 255f), (139 / 255f), (255 / 255f));
            }
        }
        itemBoxToSwap = itemBox;
        toSwap = true;
    }
}
