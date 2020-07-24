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

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		image = GetComponent<Image>();
		rectTransform = GetComponent<RectTransform>();
		defaultSizeDelta = rectTransform.sizeDelta;

		gameObject.SetActive(false);
	}

	public void ShowPhoto(Sprite photo)
	{
		gameObject.SetActive(true);
		image.sprite = photo;
		// Reset the aspect of the preview image container
		// to fit the aspect ratio of the photo.
		var newSizeDelta = defaultSizeDelta;
		var aspect = (float)photo.rect.width / photo.rect.height;
		if (aspect > (float)Screen.width / Screen.height)
		{
			newSizeDelta.y = newSizeDelta.x / aspect;
		}
		else
		{
			newSizeDelta.x = newSizeDelta.y * aspect;
		}
		rectTransform.sizeDelta = newSizeDelta;
	}
}
