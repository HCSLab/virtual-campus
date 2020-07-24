using UnityEngine;
using UnityEngine.UI;

public class PhotoDisplay : MonoBehaviour
{
    public void ShowPhoto()
	{
        PhotoView.Instance.ShowPhoto((Texture2D)GetComponent<RawImage>().texture);
	}
}
