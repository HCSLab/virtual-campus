using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinDisplay : MonoBehaviour
{
	public Image image;
    public static bool busy = false;

	public SkinScriptableObject skin;

    Texture2D RenderTextureToTexture2D(RenderTexture rt)
    {
        /*
        int width = renderTexture.width;
        int height = renderTexture.height;
        Texture2D texture2D = new Texture2D(width, height);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();
        return texture2D;
        */
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, true);
        tex.Apply();
        return tex;
    }

	public void Initialize(SkinScriptableObject newSkin, PlayerSkin playerForSkinIcon, RenderTexture skinIconRenderTexture)
	{
        busy = true;
		skin = newSkin;
        playerForSkinIcon.ChangeSkinTexture(newSkin.skinTexture);

        StartCoroutine(GenerateIcon(skinIconRenderTexture));
        
        /*
        Texture2D skinTexture = RenderTextureToTexture2D(skinIconRenderTexture);
        Sprite sprite = Sprite.Create(skinTexture, new Rect(0, 0, skinTexture.width, skinTexture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
        */
    }

    public IEnumerator GenerateIcon(RenderTexture skinIconRenderTexture)
    {
        yield return new WaitForEndOfFrame();
        Texture2D skinTexture = RenderTextureToTexture2D(skinIconRenderTexture);
        Sprite sprite = Sprite.Create(skinTexture, new Rect(0, 0, skinTexture.width, skinTexture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
        busy = false;
        
    }

    public void OnButtonClicked()
	{
		ItemPanel.Instance.ShowSkin(skin);
	}
}
