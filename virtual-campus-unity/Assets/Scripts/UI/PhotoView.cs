using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoView : MonoBehaviour
{
	static public PhotoView Instance;

    Image image;
	RectTransform rectTransform;
	Vector2 defaultSizeDelta;
	AspectRatioFitter aspectRatioFitter;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		image = GetComponent<Image>();
		rectTransform = GetComponent<RectTransform>();
		defaultSizeDelta = rectTransform.sizeDelta;

		aspectRatioFitter = GetComponent<AspectRatioFitter>();

		gameObject.SetActive(false);
	}

	public void ShowPhoto(Sprite photo)
	{
		gameObject.SetActive(true);
		image.sprite = photo;
		// Reset the aspect of the preview image container
		// to fit the aspect ratio of the photo.
		aspectRatioFitter.aspectRatio = (float)image.sprite.rect.width / image.sprite.rect.height;
	}
}
