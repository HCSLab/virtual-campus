using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RealWorldPhotoDisplay : MonoBehaviour
{
	public Image image;
	public AspectRatioFitter aspectRatioFitter;

	RealWorldPhotoScriptableObject photo;

	public void Initialize(RealWorldPhotoScriptableObject newPhoto)
	{
		photo = newPhoto;
		image.sprite = newPhoto.photo;
		aspectRatioFitter.aspectRatio =
			(float)newPhoto.photo.rect.width / (float)newPhoto.photo.rect.height;
	}

	public void OnButtonClicked()
	{
		ItemPanel.Instance.ShowPhoto(photo);
	}
}
