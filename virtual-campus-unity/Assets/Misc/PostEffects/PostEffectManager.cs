using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class PostEffectManager : MonoBehaviour
{
    public List<PostEffect> effects = new List<PostEffect>();

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture[] buffer = new RenderTexture[2]
        {
            RenderTexture.GetTemporary(source.width, source.height, 0),
            RenderTexture.GetTemporary(source.width, source.height, 0),
        };
        int ptr = 0;

        Graphics.Blit(source, buffer[ptr]);

        foreach (var e in effects)
        {
            if (e == null || !e.enabled) continue;

            e.OnRender(buffer[ptr], buffer[(ptr + 1) % 2]);
            ptr = (ptr + 1) % 2;
        }

        Graphics.Blit(buffer[ptr], destination);

        RenderTexture.ReleaseTemporary(buffer[0]);
        RenderTexture.ReleaseTemporary(buffer[1]);
    }
}
