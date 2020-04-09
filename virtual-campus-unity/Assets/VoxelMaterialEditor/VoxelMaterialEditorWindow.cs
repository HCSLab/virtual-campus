using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class VoxelMaterialEditorWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(VoxelMaterialEditorWindow));
        SceneView.duringSceneGui += DetectMouseClick;
    }

    static void DetectMouseClick(SceneView sceneView)
    {
        if (Event.current.type == EventType.MouseDown)
        {
            Vector3 mousePosition = Event.current.mousePosition;
            mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y;
            Debug.Log(mousePosition);
            OnClick(mousePosition);
        }
    }

    static void OnClick(Vector2 mousePos)
    {
        // get selected voxel object and its material and textures
        GameObject obj = Selection.activeGameObject;
        if (obj == null || obj.name != "default") return;
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render == null) return;
        Material mat = render.material;
        //if (!mat.HasProperty("_MainTex") || !mat.HasProperty("_PropTex")) return;
        //Texture2D mainTex = (Texture2D)mat.GetTexture("_MainTex");
        //Texture2D propTex = (Texture2D)mat.GetTexture("_PropTex");
        Texture2D propTex = null; // for debug
        if (propTex == null)
        {
            propTex = newPropTex();
        }

        // get the UV at mouse position
        GameObject camObj = new GameObject();
        camObj.AddComponent<Camera>();
        Camera cam = camObj.GetComponent<Camera>();
        cam.CopyFrom(SceneView.currentDrawingSceneView.camera);
        var UVTex = RenderTexture.GetTemporary(cam.targetTexture.descriptor);
        cam.targetTexture = UVTex;
        Shader UVShader = Shader.Find("Others/RenderUV");
        cam.RenderWithShader(UVShader, "");
        RenderTexture.active = UVTex;
        Texture2D UVTexReader = new Texture2D(1, 1);
        Rect mouseRect = new Rect(mousePos.x, mousePos.y, 1, 1);
        UVTexReader.ReadPixels(mouseRect, 0, 0, false);
        UVTexReader.Apply();
        Color col = UVTexReader.GetPixel(0, 0);
        int uv_x = (int)(col.r * 256);
        RenderTexture.ReleaseTemporary(UVTex);
        DestroyImmediate(UVTexReader);
        //cam.SetReplacementShader(UVShader, "");
        DestroyImmediate(camObj);

        // draw data into properties texture and save it
        propTex.SetPixel(uv_x, 0, Color.red);
        propTex.Apply();
        var pngData = propTex.EncodeToPNG();
        System.IO.File.WriteAllBytes("./test.png", pngData);
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

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        myString = EditorGUILayout.TextField("Text Field", myString);

        groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
        myBool = EditorGUILayout.Toggle("Toggle", myBool);
        myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
        EditorGUILayout.EndToggleGroup();

        var o = GameObject.Find("Cube");
        o.transform.localScale = new Vector3(myFloat, myFloat, myFloat);

        if (myFloat < 0) myFloat = -myFloat;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= DetectMouseClick;
    }
}
