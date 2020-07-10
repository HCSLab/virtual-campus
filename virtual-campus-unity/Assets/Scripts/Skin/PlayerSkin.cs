using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkin : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;

    public void ChangeSkinTexture(Texture2D newTexture)
	{
		foreach (Material m in skinnedMeshRenderer.materials)
			m.SetTexture("_BaseMap", newTexture);
	}

	public Texture GetCurrentSkinTexture()
	{
		return skinnedMeshRenderer.material.GetTexture("_BaseMap");
	}
}
