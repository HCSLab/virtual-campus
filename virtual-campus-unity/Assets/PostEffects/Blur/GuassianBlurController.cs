using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuassianBlurController : PostEffect
{
	public Shader guassianBlurShader;
	protected Material guassianBlurMaterial;
	public Material material
	{
		get
		{
			guassianBlurMaterial = CheckShaderAndCreateMaterial(guassianBlurShader, guassianBlurMaterial);
			return guassianBlurMaterial;
		}
	}

	[Range(0, 4)]
	public int inerations = 3;

	[Range(0.2f, 3)]
	public float blurSpread = 0.6f;

	[Range(1, 8)]
	public int downSample = 2;

	public override void OnRender(RenderTexture source, RenderTexture destination)
	{
		if (material != null)
		{
			int w = source.width / downSample;
			int h = source.height / downSample;
			RenderTexture buffer = RenderTexture.GetTemporary(w, h, 0);
			buffer.filterMode = FilterMode.Bilinear;
			RenderTexture buffer_tp = RenderTexture.GetTemporary(w, h, 0);

			Graphics.Blit(source, buffer);

			for (int i = 0; i < inerations; i++)
			{
				material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

				Graphics.Blit(buffer, buffer_tp, material, 0);
				Graphics.Blit(buffer_tp, buffer, material, 1);
			}

			Graphics.Blit(buffer, destination);

			RenderTexture.ReleaseTemporary(buffer);
			RenderTexture.ReleaseTemporary(buffer_tp);
		}
		else
		{
			Graphics.Blit(source, destination);
		}
	}
}
