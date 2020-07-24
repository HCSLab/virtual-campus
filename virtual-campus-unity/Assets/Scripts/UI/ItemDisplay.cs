using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
	public Image iconImage;

	[HideInInspector]
	public ItemScriptableObject item;

	public void Initialize(ItemScriptableObject newItem)
	{
		item = newItem;
		iconImage.sprite = newItem.icon;
	}

	public void OnButtonClicked()
	{
		ItemPanel.Instance.ShowItem(item);
	}
}
