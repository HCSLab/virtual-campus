using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinDisplay : MonoBehaviour
{
	public RawImage rawImage;

	SkinScriptableObject skin;

	public void Initialize(SkinScriptableObject newSkin)
	{
		skin = newSkin;
		rawImage.texture = newSkin.skinTexture;
	}

	public void OnButtonClicked()
	{
		ItemPanel.Instance.ShowSkin(skin);
	}
}
