using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

public class VoxelMaterialEditorWindow : EditorWindow
{
    string myString = "Hello World";
    bool groupEnabled;
    bool myBool = true;
    float myFloat = 1.23f;

    const int mouseNeighbourhoodSize = 200;
    static Texture2D mouseNeighbourhoodTex;

    static SceneView sceneView;
    static Vector2 mousePos;

    private void OnEnable()
    {
        SceneView.duringSceneGui += DetectMouseClick;
        mouseNeighbourhoodTex = new Texture2D(mouseNeighbourhoodSize, mouseNeighbourhoodSize);
    }

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/My Window")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(VoxelMaterialEditorWindow));
    }

    static void DetectMouseClick(SceneView sv)
    {
        sceneView = sv;
        if (Event.current.type == EventType.MouseDown)
        {
            GUIStyle style = "GV Gizmo DropDown";
            Vector2 ribbon = style.CalcSize(sceneView.titleContent);

            Vector2 sv_correctSize = sceneView.position.size;
            sv_correctSize.y -= ribbon.y; //exclude this nasty ribbon

            //flip the position:
            mousePos = Event.current.mousePosition;
            //mousePos.y = sceneView.camera.pixelHeight - mousePos.y;
            mousePos.y = sv_correctSize.y - mousePos.y;
            mousePos *= sceneView.camera.pixelHeight / sv_correctSize.y;
            // Debug.Log(mousePos);

            OnClick();
        }
    }

    static void OnClick()
    {
        // get selected voxel object and its material and textures
        GameObject obj = Selection.activeGameObject;
        if (obj == null || obj.name != "default") return;
        MeshRenderer render = obj.GetComponent<MeshRenderer>();
        if (render == null) return;
        Material mat = render.sharedMaterial;
        //if (!mat.HasProperty("_MainTex") || !mat.HasProperty("_PropTex")) return;
        //Texture2D mainTex = (Texture2D)mat.GetTexture("_MainTex");
        //Texture2D propTex = (Texture2D)mat.GetTexture("_PropTex");
        Texture2D propTex = null; // for debug
        if (propTex == null)
        {
            propTex = CreatePropTex();
        }

        // get the UV at mouse position
        Texture2D UVTex = DrawWithShaderReplacement(Shader.Find("Others/RenderUV"));
        Color col = UVTex.GetPixel((int)mousePos.x, (int)mousePos.y);
        int uv_x = (int)(UVTex.GetPixel((int)mousePos.x, (int)mousePos.y).r * 255);

        // draw data into properties texture and save it
        propTex.SetPixel(uv_x, 0, Color.red);
        propTex.Apply();
        var pngData = propTex.EncodeToPNG();
        System.IO.File.WriteAllBytes("./test.png", pngData);

        DrawMouseNighbourhood();
    }

    static void DrawMouseNighbourhood()
    {
        // draw the scene view immediately
        Texture2D sceneTex = DrawWithShaderReplacement(null);

        // copy the correct region to mouse neighbourhood texture
        for (int i = 0; i < mouseNeighbourhoodSize; i++)
        {
            for (int j = 0; j < mouseNeighbourhoodSize; j++)
            {
                int sx = (int)mousePos.x - mouseNeighbourhoodSize / 2 + i;
                int sy = (int)mousePos.y - mouseNeighbourhoodSize / 2 + j;
                if (sx >= 0 && sy >= 0 && sx < sceneTex.width && sy < sceneTex.height)
                {
                    mouseNeighbourhoodTex.SetPixel(i, j, sceneTex.GetPixel(sx, sy));
                }
                else
                {
                    mouseNeighbourhoodTex.SetPixel(i, j, Color.black);
                }
            }
        }

        // draw red reference line
        for (int i = 0; i < mouseNeighbourhoodSize; i++)
        {
            mouseNeighbourhoodTex.SetPixel(mouseNeighbourhoodSize / 2, i, Color.red);
            mouseNeighbourhoodTex.SetPixel(i, mouseNeighbourhoodSize / 2, Color.red);
        }

        // apply changes
        mouseNeighbourhoodTex.Apply();
    }

    static Texture2D DrawWithShaderReplacement(Shader shader, string replacementTag = "")
    {
        // create a new camera and place it as the scene view camera
        GameObject camObj = new GameObject();
        camObj.AddComponent<Camera>();
        Camera cam = camObj.GetComponent<Camera>();
        cam.CopyFrom(sceneView.camera);

        // create a temporary render texture and use it as the target
        var tempRT = RenderTexture.GetTemporary(cam.targetTexture.descriptor);
        cam.targetTexture = tempRT;
        if (shader != null)
        {
            // draw with shader replacement
            cam.RenderWithShader(shader, replacementTag);
        }
        else
        {
            // shader is null, draw with original materials
            cam.Render();
        }

        // read from render texture to a 2d texture
        RenderTexture.active = tempRT;
        Texture2D tex = new Texture2D(tempRT.width, tempRT.height);
        tex.ReadPixels(new Rect(0, 0, tempRT.width, tempRT.height), 0, 0);
        tex.Apply();

        // clean up
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tempRT);
        DestroyImmediate(camObj);

        return tex;
    }

    static Texture2D CreatePropTex()
    {
        Texture2D tex = new Texture2D(255, 4, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Point;

        for (int i = 0; i < 256; i++)
        {
            tex.SetPixel(i, 0, new Color(1, 1, 1, 1));
        }

        return tex;
    }

    void OnGUI()
    {
        GUILayout.Label(mouseNeighbourhoodTex);
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

    private void Update()
    {
        Repaint();
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= DetectMouseClick;
        DestroyImmediate(mouseNeighbourhoodTex);
    }
}
