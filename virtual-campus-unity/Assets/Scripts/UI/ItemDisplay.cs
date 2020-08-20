using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplay : MonoBehaviour
{
	public Image iconImage;
	public AspectRatioFitter aspectRatioFitter;

	ItemScriptableObject item;

	public void Initialize(ItemScriptableObject newItem)
	{
		item = newItem;
		iconImage.sprite = newItem.icon;
		aspectRatioFitter.aspectRatio =
			(float)iconImage.sprite.rect.width / (float)iconImage.sprite.rect.height;
	}

	public void OnButtonClicked()
	{
		ItemPanel.Instance.ShowItem(item);
	}
}
