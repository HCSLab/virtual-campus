using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
	// 物品三类：item、skin、real world photo
	// item只能看预览，skin可以预览+装备

	public enum ItemType
	{
		item,
		skin,
		realWorldPhoto
	};

	public static ItemPanel Instance;

	public Transform elementContainer;

	[Header("Item")]
	public GameObject itemDisplayPrefab;
	public GameObject itemRight;
	public TextMeshProUGUI itemNameText, itemDescriptionText;
	public Image itemIcon;

	[Header("Skin")]
	public GameObject skinDisplayPrefab;
	public GameObject skinRight;
	public TextMeshProUGUI skinName;
	public Button skinUseButton;

	private void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		itemRight.SetActive(false);
		skinRight.SetActive(false);
	}

	public void AddItem(ItemScriptableObject item)
	{
		var itemDisplay = Instantiate(itemDisplayPrefab);
		itemDisplay.transform.SetParent(elementContainer);
		itemDisplay.transform.localScale = Vector3.one;
		itemDisplay.GetComponent<ItemDisplay>().Initialize(item);
	}

	public void ShowItem(ItemScriptableObject item)
	{
		itemRight.SetActive(true);
		skinRight.SetActive(false);
		itemIcon.sprite = item.icon;
		itemNameText.text = item.name;
		itemDescriptionText.text = item.description;
	}

	public void RemoveItem(ItemScriptableObject item)
	{

	}
}
