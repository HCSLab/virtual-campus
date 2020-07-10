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
		foreach (var box in itemBoxs)
			Destroy(box.gameObject);
		itemBoxs.Clear();

		foreach (var item in testItems)
			Add(item);
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
}
