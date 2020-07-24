using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoView : MonoBehaviour
{
	static public PhotoView Instance;

    RawImage rawImage;

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		rawImage = GetComponent<RawImage>();
		var rectTransform = GetComponent<RectTransform>();
		var newSizeDelta = rectTransform.sizeDelta;
		var asepctRatio = (float)Screen.width / Screen.height;
		newSizeDelta.y = newSizeDelta.x / asepctRatio;
		rectTransform.sizeDelta = newSizeDelta;

		gameObject.SetActive(false);
	}

	public void ShowPhoto(Texture2D photo)
	{
		gameObject.SetActive(true);
		rawImage.texture = photo;
	}
}
