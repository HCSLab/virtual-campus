using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DrawOutline : MonoBehaviour
{
    private RenderTexture outlineTex;
    private RenderTexture cullTex;
    private RenderTexture tempTex;
    private Material outlineMat;
    public Shader outlineShader;
    private CommandBuffer commandBuffer;

    public Color edgeColor;
    [Range(0.1f, 10)] public float blurSize;
    [Range(0, 1)] public float threshold;
    public bool hardEdge;

    public Renderer target;

    private void OnEnable()
    {
        outlineMat = new Material(outlineShader);
        outlineMat.hideFlags = HideFlags.DontSave;

        outlineTex = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.R8);
        cullTex = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.R8);
        tempTex = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.R8);

        commandBuffer = new CommandBuffer();
        commandBuffer.SetRenderTarget(cullTex);
        commandBuffer.ClearRenderTarget(true, true, Color.black);
        commandBuffer.DrawRenderer(target, outlineMat, 0, 0);

        commandBuffer.CopyTexture(cullTex, outlineTex);
        commandBuffer.Blit(outlineTex, tempTex, outlineMat, 1);
        commandBuffer.Blit(tempTex, outlineTex, outlineMat, 2);
        
        GetComponent<Camera>().AddCommandBuffer(CameraEvent.AfterForwardOpaque, commandBuffer);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        outlineMat.SetColor("_Color", edgeColor);
        outlineMat.SetFloat("_BlurSize", blurSize);
        outlineMat.SetFloat("_Threshold", threshold);
        outlineMat.SetFloat("_HardEdge", hardEdge ? 1 : -1);

        //Graphics.ExecuteCommandBuffer(commandBuffer);

        outlineMat.SetTexture("_OutlineTex", outlineTex);
        outlineMat.SetTexture("_CullTex", cullTex);
        Graphics.Blit(source, destination, outlineMat, 3);
        //Graphics.Blit(outlineTex, destination);
    }

    private void OnDisable()
    {
        commandBuffer.Clear();
        RenderTexture.ReleaseTemporary(outlineTex);
        RenderTexture.ReleaseTemporary(tempTex);
    }
}
