using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class VoxelMaterialEditorWindow : EditorWindow
{
    static Color mainColor;
    static Color specularColor;
    static float gloss;
    static float emission;

    static bool targetActive;
    static GameObject targetObj;
    static string assetPathName;
    static Material material;
    static int mousePosUV_x;
    static Texture2D mainTex;
    static Texture2D propTex;

    const int mouseNeighbourhoodSize = 200;
    static Texture2D mouseNeighbourhoodTex;

    static SceneView sceneView;
    static Vector2 mousePos;

    private void OnEnable()
    {
        SceneView.duringSceneGui += DetectMouseClick;
        mouseNeighbourhoodTex = new Texture2D(mouseNeighbourhoodSize, mouseNeighbourhoodSize);

        targetActive = false;
    }

    // Add menu item named "My Window" to the Window menu
    [MenuItem("Tools/Voxel Material Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        GetWindow(typeof(VoxelMaterialEditorWindow));
    }

    static void DetectMouseClick(SceneView sv)
    {
        sceneView = sv;
        if (Event.current.type == EventType.MouseDown ||
            Event.current.type == EventType.MouseMove)
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

            bool active = OnClick();

            if (targetActive == true && active == false)
            {
                SaveTextures();
            }

            targetActive = active;

            if (targetActive)
                DrawMouseNighbourhood();
        }
    }

    static bool OnClick()
    {
        // get selected voxel object and its material and textures
        targetObj = Selection.activeGameObject;
        if (targetObj == null || targetObj.name != "default") 
            return false;

        MeshRenderer render = targetObj.GetComponent<MeshRenderer>();
        if (render == null) 
            return false;

        material = render.sharedMaterial;
        if (!material.HasProperty("_MainTex") || !material.HasProperty("_PropTex"))
            return false;

        mainTex = (Texture2D)material.GetTexture("_MainTex");
        propTex = (Texture2D)material.GetTexture("_PropTex");
        if (propTex == null)
        {
            propTex = CreatePropTex();
            SaveTextures();
            material.SetTexture("_PropTex", propTex);
        }
        mainTex.filterMode = FilterMode.Point;
        propTex.filterMode = FilterMode.Point;

        var path = AssetDatabase.GetAssetPath(mainTex);
        assetPathName = path.Split('.')[0];

        // get the UV at mouse position
        Texture2D UVTex = DrawWithShaderReplacement(Shader.Find("Voxel/VoxelUV"), "CustomType");
        Color col = UVTex.GetPixel((int)mousePos.x, (int)mousePos.y);
        mousePosUV_x = (int)( UVTex.GetPixel((int)mousePos.x, (int)mousePos.y).r * 256 );

        // set variables to data from material
        mainColor = mainTex.GetPixel(mousePosUV_x, 0);
        emission = propTex.GetPixel(mousePosUV_x, 1).r * 10;
        specularColor = propTex.GetPixel(mousePosUV_x, 0);
        gloss = propTex.GetPixel(mousePosUV_x, 1).g * 256;

        return true;
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
        Texture2D tex = new Texture2D(256, 4, mainTex.format, false);
        tex.filterMode = FilterMode.Point;

        for (int i = 0; i < 256; i++)
        {
            tex.SetPixel(i, 0, new Color(0.3f, 0.3f, 0.3f, 1));
            tex.SetPixel(i, 1, new Color(0, 128f / 256f, 0, 1));
        }

        return tex;
    }

    void OnGUI()
    {
        GUILayout.Label("Mouse Select View", EditorStyles.boldLabel);
        GUILayout.Label(mouseNeighbourhoodTex);
        GUILayout.Space(5);

        GUILayout.Label("Color", EditorStyles.boldLabel);
        mainColor = EditorGUILayout.ColorField("Main Color", mainColor);
        emission = EditorGUILayout.Slider("Emission", emission, 0, 10);
        GUILayout.Space(5);

        GUILayout.Label("Specular", EditorStyles.boldLabel);
        specularColor = EditorGUILayout.ColorField("Specular Color", specularColor);
        gloss = EditorGUILayout.Slider("Gloss", gloss, 8, 256);
        GUILayout.Space(5);

        SetDataToTextures();
    }

    void SetDataToTextures()
    {
        if (mainTex == null || propTex == null) 
            return;

        mainTex.SetPixel(mousePosUV_x, 0, mainColor);

        propTex.SetPixel(mousePosUV_x, 0, specularColor);

        Color prop_1 = new Color(emission / 10, gloss / 256, 0, 1);
        propTex.SetPixel(mousePosUV_x, 1, prop_1);

        mainTex.Apply();
        propTex.Apply();

        // SaveTextures();
    }

    static void SaveTextures()
    {
        SaveTexture(mainTex, assetPathName + ".png");
        SaveTexture(propTex, assetPathName + "_prop.png");
    }

    static void SaveTexture(Texture2D tex, string filename)
    {
        var pngdata = tex.EncodeToPNG();
        var file = File.Open(filename, System.IO.FileMode.Create);
        var writer = new BinaryWriter(file);
        writer.Write(pngdata);
        file.Close();
    }

    private void Update()
    {
        Repaint();
    }

    private void OnDestroy()
    {
        SaveTextures();

        SceneView.duringSceneGui -= DetectMouseClick;
        DestroyImmediate(mouseNeighbourhoodTex);
    }
}
