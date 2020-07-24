using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealWorldPhotoDisplay : MonoBehaviour
{
	public Image image;

	RealWorldPhotoScriptableObject photo;

	public void Initialize(RealWorldPhotoScriptableObject newPhoto)
	{
		photo = newPhoto;
		image.sprite = newPhoto.photo;

		// Reset the aspect of the preview image container
		// to fit the aspect ratio of the photo.
		var rectTransform = image.gameObject.GetComponent<RectTransform>();
		var newSizeDelta = rectTransform.sizeDelta;
		var aspect = (float)newPhoto.photo.rect.width / newPhoto.photo.rect.height;
		if(newPhoto.photo.rect.width > newPhoto.photo.rect.height)
		{
			newSizeDelta.y = newSizeDelta.x / aspect;
		}
		else
		{
			newSizeDelta.x = newSizeDelta.y * aspect;
		}
		rectTransform.sizeDelta = newSizeDelta;
	}

	public void OnButtonClicked()
	{
		ItemPanel.Instance.ShowPhoto(photo);
	}
}
