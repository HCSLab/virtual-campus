using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeDetectNormalAndDepth : PostEffect
{
	public Shader edgeDetectShader;
	private Material edgeDetectMateral;
	public Material material
	{
		get
		{
			edgeDetectMateral = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMateral);
			return edgeDetectMateral;
		}
	}

	[Range(0, 1)]
	public float edgeOnly = 0.5f;

	public Color edgeColor = Color.black;
	public Color backgroundColor = Color.white;

	public float sampleDistance = 1.0f;
	public float sensitivityDepth = 1.0f;
	public float sensitivityNormal = 1.0f;

	private Camera myCamera;

	private void OnEnable()
	{
		myCamera = GetComponent<Camera>();
		myCamera.depthTextureMode |= DepthTextureMode.DepthNormals;
	}

	[ImageEffectOpaque]
	public override void OnRender(RenderTexture source, RenderTexture destination)
	{
		if (material != null)
		{
			material.SetFloat("_EdgeOnly", edgeOnly);
			material.SetColor("_EdgeColor", edgeColor);
			material.SetColor("_BackgroundColor", backgroundColor);
			material.SetFloat("_SampleDistance", sampleDistance);
			material.SetFloat("_SensitivityNormal", sensitivityNormal);
			material.SetFloat("_SensitivityDepth", sensitivityDepth);

			Graphics.Blit(source, destination, material);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}
}
