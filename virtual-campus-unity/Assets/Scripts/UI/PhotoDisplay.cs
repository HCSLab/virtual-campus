using UnityEngine;
using UnityEngine.UI;

public class PhotoDisplay : MonoBehaviour
{
	Image image;

	private void Start()
	{
		image = GetComponent<Image>();
	}

	public void ShowPhoto()
	{
        PhotoView.Instance.ShowPhoto(image.sprite);
	}
}
