using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetection : PostEffect
{
	public Shader edgeDetectionShader;
	private Material edgeDetectionMaterial;
	public Material material
	{
		get
		{
			edgeDetectionMaterial = CheckShaderAndCreateMaterial(edgeDetectionShader, edgeDetectionMaterial);
			return edgeDetectionMaterial;
		}
	}

	[Range(0, 1)]
	public float edgesOnly = 0;

	[Range(0.5f, 3)]
	public float powOfEdge = 1;

	public Color edgeColor = Color.black;
	public Color backgroundColor = Color.white;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (material != null)
		{
			material.SetFloat("_EdgeOnly", edgesOnly);
			material.SetColor("_EdgeColor", edgeColor);
			material.SetColor("_BackgroundColor", backgroundColor);
			material.SetFloat("_Pow", powOfEdge);

			Graphics.Blit(source, destination, material);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}
}
