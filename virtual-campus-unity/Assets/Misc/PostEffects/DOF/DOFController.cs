using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DOFController : PostEffect
{
    public Shader DEFShader;
    private Material DEFMaterial;
    public Material material
    {
        get
        {
            DEFMaterial = CheckShaderAndCreateMaterial(DEFShader, DEFMaterial);
            return DEFMaterial;
        }
    }

    [Range(0.2f, 3f)]
    public float maxBlurSpread = 0.6f;

    [Range(0, 4)]
    public int inerations = 3;

    [Range(0.1f, 5f)]
    public float powOfDist = 3f;

    public Vector2 blurRange = new Vector2(10, 100);

    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
    }

    public override void OnRender(RenderTexture source, RenderTexture destination)
    {
        if (material != null)
        {
            material.SetFloat("_PowOfDist", powOfDist);
            material.SetVector("_BlurRange", blurRange);

            int w = source.width;
            int h = source.height;
            RenderTexture buffer = RenderTexture.GetTemporary(w, h, 0);
            buffer.filterMode = FilterMode.Bilinear;
            RenderTexture buffer2 = RenderTexture.GetTemporary(w, h, 0);
            buffer2.filterMode = FilterMode.Bilinear;

            Graphics.Blit(source, buffer);

            for (int i = 0; i < inerations; i++)
            {
                material.SetFloat("_MaxBlurSize", maxBlurSpread * (1 + (i - 1) / 2.0f));
                Graphics.Blit(buffer, buffer2, material, 0);
                Graphics.Blit(buffer2, buffer, material, 1);
            }

            Graphics.Blit(buffer, destination);

            RenderTexture.ReleaseTemporary(buffer);
            RenderTexture.ReleaseTemporary(buffer2);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}
