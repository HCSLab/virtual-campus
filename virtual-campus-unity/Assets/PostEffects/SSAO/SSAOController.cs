using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SSAOController : PostEffect
{
    public Shader AOShader;
    private Material AOMaterial = null;

    public Material material
    {
        get
        {
            AOMaterial = CheckShaderAndCreateMaterial(AOShader, AOMaterial);
            return AOMaterial;
        }
    }

    public float sampleRadius = 1f;
    [Range(0, 1)] public float AOAmount = 1f;

    [Range(0.2f, 3)] public float blurSpread = 0.6f;
    [Range(1, 8)] public int downSample = 2;
    [Range(0, 4)] public int inerations = 3;

    [Range(0, 2)] public int mode;

    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
    }

    private void Start()
    {
        Matrix4x4 frustumCorners = Matrix4x4.identity;

        float fov = GetComponent<Camera>().fieldOfView;
        float near = GetComponent<Camera>().nearClipPlane;
        float aspect = GetComponent<Camera>().aspect;

        float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        Vector3 toRight = Vector3.right * halfHeight * aspect;
        Vector3 toTop = Vector3.up * halfHeight;

        Vector3 topLeft = Vector3.forward * near + toTop - toRight;
        Vector3 topRight = Vector3.forward * near + toRight + toTop;
        Vector3 bottomLeft = Vector3.forward * near - toTop - toRight;
        Vector3 bottomRight = Vector3.forward * near + toRight - toTop;

        bottomLeft /= near;
        bottomRight /= near;
        topRight /= near;
        topLeft /= near;

        frustumCorners.SetRow(0, bottomLeft);
        frustumCorners.SetRow(1, bottomRight);
        frustumCorners.SetRow(2, topRight);
        frustumCorners.SetRow(3, topLeft);

        material.SetMatrix("_FrustumCornersRay", frustumCorners);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null && mode != 0)
        {
            var sampleList = new List<Vector4>();
            for (int i = 0; i < 64; i++)
            {
                Vector4 dir = new Vector4(Random.Range(-1, 1), Random.Range(0, 1), Random.Range(-1, 1), 0);
                dir = Mathf.Pow(Random.Range(0f, 1f), 2) * sampleRadius * dir.normalized;
                sampleList.Add(dir);
            }
            material.SetVectorArray("_SampleList", sampleList);

            material.SetFloat("_AOAmount", AOAmount);
            material.SetFloat("_BlurSize", blurSpread);

            int w = src.width / downSample;
            int h = src.height / downSample;
            RenderTexture buffer = RenderTexture.GetTemporary(w, h, 0);
            buffer.filterMode = FilterMode.Bilinear;
            RenderTexture buffer2 = RenderTexture.GetTemporary(w, h, 0);
            buffer2.filterMode = FilterMode.Bilinear;

            Graphics.Blit(src, buffer, material, 0);

            for (int i = 0; i < inerations; i++)
            {
                Graphics.Blit(buffer, buffer2, material, 1);
                Graphics.Blit(buffer2, buffer, material, 2);
            }

            if (mode == 1)
            {
                Graphics.Blit(buffer, dest);
                RenderTexture.ReleaseTemporary(buffer);
                RenderTexture.ReleaseTemporary(buffer2);
                return;
            }

            material.SetTexture("_SSAOTex", buffer);
            Graphics.Blit(src, dest, material, 3);

            RenderTexture.ReleaseTemporary(buffer);
            RenderTexture.ReleaseTemporary(buffer2);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
