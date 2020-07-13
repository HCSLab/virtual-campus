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

	protected virtual void Start()
	{
		foreach (var item in testItems)
			Add(item);

		gameObject.SetActive(false);
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
		detailImage.sprite = item.image;
		detailName.text = item.itemName;
		detailDescription.text = item.description;
		currentItem = item;
		if (typeof(Consumable).IsInstanceOfType(currentItem))
		{
			Consumable c = (Consumable)currentItem;
			useButton.SetActive(true);
			amountText.SetActive(true);
			amountText.GetComponent<TextMeshProUGUI>().text = c.amount.ToString();
		}
		else
		{
			useButton.SetActive(false);
			amountText.SetActive(false);
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
}
