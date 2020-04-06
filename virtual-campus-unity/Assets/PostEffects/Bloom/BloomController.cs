using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomController : PostEffect
{
	public Shader bloomShader;
	protected Material bloomMaterial;

	public Material material
	{
		get
		{
			bloomMaterial = CheckShaderAndCreateMaterial(bloomShader, bloomMaterial);
			return bloomMaterial;
		}
	}

	[Range(0, 4)]
	public int inerations = 3;

	[Range(0.2f, 3)]
	public float blurSpread = 0.6f;

	[Range(1, 8)]
	public int downSample = 2;

	[Range(0, 4)]
	public float luminanceThreshold = 0.6f;

	[Range(0, 2)]
	public float bloomAmount;

	[Range(0, 2)]
	public int mode = 2;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (material != null && mode != 0)
		{
			int w = source.width / downSample;
			int h = source.height / downSample;
			RenderTexture buffer = RenderTexture.GetTemporary(w, h, 0);
			buffer.filterMode = FilterMode.Bilinear;
			RenderTexture buffer2 = RenderTexture.GetTemporary(w, h, 0);
			buffer2.filterMode = FilterMode.Bilinear;

			material.SetFloat("_LuminanceThreshold", luminanceThreshold);
			material.SetFloat("_BloomAmount", bloomAmount);

			Graphics.Blit(source, buffer, material, 0);

			for (int i = 0; i < inerations; i++)
			{
				material.SetFloat("_BlurSize", blurSpread * (1 + (i - 1) / 2.0f));
				Graphics.Blit(buffer, buffer2, material, 1);
				Graphics.Blit(buffer2, buffer, material, 2);
			}

			if (mode == 1)
			{
				Graphics.Blit(buffer, destination);
				RenderTexture.ReleaseTemporary(buffer);
				RenderTexture.ReleaseTemporary(buffer2);
				return;
			}

			material.SetTexture("_Bloom", buffer);
			Graphics.Blit(source, destination, material, 3);

			RenderTexture.ReleaseTemporary(buffer);
			RenderTexture.ReleaseTemporary(buffer2);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}
}
