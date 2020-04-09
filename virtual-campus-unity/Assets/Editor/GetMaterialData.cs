using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.TerrainAPI;
using UnityEngine;

[CustomEditor(typeof(GameObject))]
public class GetMaterialData : Editor
{
    void OnSceneGUI()
    {
        if (Event.current.isMouse && Event.current.type == EventType.MouseDown)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
            //Debug.Log(mousePosition);
            //OnClick(mousePosition);
        }
    }

    static void OnClick(Vector2 mousePos)
    {
        GameObject obj = Selection.activeGameObject;
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render == null) return;
        Material mat = render.material;
        //Texture2D mainTex = (Texture2D)mat.GetTexture("_MainTex");
        //Texture2D propTex = (Texture2D)mat.GetTexture("_PropTex");
        Texture2D propTex = null;
        if (propTex == null)
        {
            propTex = newPropTex();
        }
        GameObject camObj = new GameObject();
        camObj.AddComponent<Camera>();
        Camera cam = camObj.GetComponent<Camera>();
        cam.CopyFrom(SceneView.currentDrawingSceneView.camera);
        RenderTexture UVTex = new RenderTexture(cam.targetTexture.descriptor);
        cam.targetTexture = UVTex;
        Shader UVShader = Shader.Find("Others/RenderUV");
        cam.SetReplacementShader(UVShader, "");
        RenderTexture.active = UVTex;
        Texture2D UVTexReader = new Texture2D(1, 1);
        Rect mouseRect = new Rect(mousePos.x, mousePos.y, 1, 1);
        UVTexReader.ReadPixels(mouseRect, 0, 0, false);
        UVTexReader.Apply();
        Color col = UVTexReader.GetPixel(0, 0);
        int uv_x = (int)(col.r * 256);
        propTex.SetPixel(uv_x, 0, Color.red);
        propTex.Apply();
        var pngData = propTex.EncodeToPNG();
        System.IO.File.WriteAllBytes("./test.png", pngData);
        Destroy(camObj);
    }

    static Texture2D newPropTex()
    {
        Texture2D tex = new Texture2D(256, 4, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        for (int i = 0; i < 256; i++)
        {
            tex.SetPixel(i, 0, new Color(1, 1, 1, 1));
        }

        return tex;
    }
}
