using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostEffect : MonoBehaviour
{
	protected Material CheckShaderAndCreateMaterial(Shader shader, Material material)
	{
		if (shader == null || !shader.isSupported)
		{
			return null;
		}
		if (shader.isSupported && material && material.shader == shader)
		{
			return material;
		}
		material = new Material(shader);
		material.hideFlags = HideFlags.DontSave;

		if (material) return material;
		else return null;
	}

	public virtual void OnRender(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(source, destination);
	}
}
