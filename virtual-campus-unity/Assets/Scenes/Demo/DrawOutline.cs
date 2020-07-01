using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

public class DrawOutline : MonoBehaviour
{
    private Camera cam;
    private RenderTexture outlineTex;
    private RenderTexture cullTex;
    private RenderTexture tempTex;
    private Material outlineMat;
    private CommandBuffer commandBuffer;

    [Header("Rendering")]
    public Shader outlineShader;
    public Color edgeColor;
    [Range(0.1f, 10)] public float blurSize;
    [Range(0.5f, 3)] public float intensity;
    [Range(0, 1)] public float threshold;
    public bool hardEdge;

    [Header("Building Description")]
    public GameObject descriptionPanel;
    public TextMeshProUGUI buildingName, buildingDescription;

    private Renderer target;
    private Renderer oldTarget;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        outlineMat = new Material(outlineShader);
        outlineMat.hideFlags = HideFlags.DontSave;

        commandBuffer = new CommandBuffer();
    }

    private void OnEnable()
    {
        outlineTex = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.R8);
        cullTex = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.R8);
        tempTex = RenderTexture.GetTemporary(Screen.width, Screen.height, 0, RenderTextureFormat.R8);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!target)
        {
            Graphics.Blit(source, destination);
            return;
        }

        outlineMat.SetColor("_Color", edgeColor);
        outlineMat.SetFloat("_BlurSize", blurSize);
        outlineMat.SetFloat("_Threshold", threshold);
        outlineMat.SetFloat("_HardEdge", hardEdge ? 1 : -1);
        outlineMat.SetFloat("_Intensity", intensity);

        if (target != oldTarget)
        {
            oldTarget = target;

            commandBuffer.Clear();
            commandBuffer.SetRenderTarget(cullTex);
            commandBuffer.ClearRenderTarget(true, true, Color.black);
            var mf = target.GetComponent<MeshFilter>();
            for (int i = 0; i < mf.sharedMesh.subMeshCount; i++)
            {
                commandBuffer.DrawRenderer(target, outlineMat, i, 0);
            }
            
            commandBuffer.CopyTexture(cullTex, outlineTex);
            commandBuffer.Blit(outlineTex, tempTex, outlineMat, 1);
            commandBuffer.Blit(tempTex, outlineTex, outlineMat, 2);
        }

        Graphics.ExecuteCommandBuffer(commandBuffer);

        outlineMat.SetTexture("_OutlineTex", outlineTex);
        outlineMat.SetTexture("_CullTex", cullTex);
        Graphics.Blit(source, destination, outlineMat, 3);
        //Graphics.Blit(outlineTex, destination);
    }

    private void Update()
    {
        SetTargetByScreenPos(Input.mousePosition);
    }

    public void SetTargetByScreenPos(Vector2 screenPos)
    {
        var ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 10000, LayerMask.GetMask("Building")))
        {
            target = hit.transform.GetComponent<Renderer>();

            var buildingDescriptionComponent = hit.collider.gameObject.GetComponent<BuildingDescription>();
            descriptionPanel.SetActive(true);
            buildingName.text = buildingDescriptionComponent.GetName();
            buildingDescription.text = buildingDescriptionComponent.GetDescription();
        }
        else
        {
            target = oldTarget = null;

            descriptionPanel.SetActive(false);
        }
    }

    private void OnDisable()
    {
        RenderTexture.ReleaseTemporary(outlineTex);
        RenderTexture.ReleaseTemporary(cullTex);
        RenderTexture.ReleaseTemporary(tempTex);
    }
}
