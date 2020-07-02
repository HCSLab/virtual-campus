using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
	public GraphicRaycaster canvasGraphicRaycaster;
	public GameObject descriptionPanel;
	public Image loadingCursor;
	public TextMeshProUGUI buildingName, buildingDescription;
	public float enableTime;

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

	private void Start()
	{
		descriptionPanel.SetActive(false);
		loadingCursor.gameObject.SetActive(false);
	}

	private void Update()
	{
		SetTargetByScreenPos(Input.mousePosition);
	}

	delegate bool Check();
	bool isHit, isUIHit;
	RaycastHit hit;
	bool hasDisableCoroutine = false, hasEnableCoroutine = false;
	public void SetTargetByScreenPos(Vector2 screenPos)
	{
		loadingCursor.gameObject.GetComponent<RectTransform>().position = screenPos;

		var ray = cam.ScreenPointToRay(screenPos);
		isHit = Physics.Raycast(ray, out hit, 10000, LayerMask.GetMask("Building"));

		var pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = screenPos;
		List<RaycastResult> results = new List<RaycastResult>();
		canvasGraphicRaycaster.Raycast(pointerEventData, results);
		foreach(RaycastResult result in results)
		{
			if(result.gameObject == loadingCursor.gameObject)
			{
				results.Remove(result);
				break;
			}
		}
		isUIHit = results.Count > 0;

		if (descriptionPanel.activeSelf)
		{
			var currentTarget = target.gameObject;
			if (!isHit || hit.collider.gameObject != currentTarget)
			{
				// Try to disable the panel.
				if (!hasDisableCoroutine)
				{
					hasDisableCoroutine = true;
					loadingCursor.gameObject.SetActive(true);
					loadingCursor.fillAmount = 0f;
					StartCoroutine(
						TryDelayedInvoke(
							enableTime,
							() => { return !isUIHit && (!isHit || hit.collider.gameObject != currentTarget); },
							() => { target = oldTarget = null; descriptionPanel.SetActive(false); },
							() => { hasDisableCoroutine = false; loadingCursor.gameObject.SetActive(false); }
							)
					);
				}
			}
		}
		else
		{
			if (isHit)
			{
				// Try to enable the panel.
				if(!hasEnableCoroutine)
				{
					hasEnableCoroutine = true;
					loadingCursor.gameObject.SetActive(true);
					loadingCursor.fillAmount = 0f;
					var tentiveTarget = hit.transform.GetComponent<Renderer>();
					StartCoroutine(
						TryDelayedInvoke(
							enableTime,
							() => { return isHit && hit.collider.gameObject == tentiveTarget.gameObject; },
							() =>
							{
								target = tentiveTarget;
								var buildingDescriptionComponent = target.gameObject.GetComponent<BuildingDescription>();
								descriptionPanel.SetActive(true);
								buildingName.text = buildingDescriptionComponent.GetName();
								buildingDescription.text = buildingDescriptionComponent.GetDescription();
							},
							() => { hasEnableCoroutine = false; loadingCursor.gameObject.SetActive(false); }
							)
						);
				}
			}
		}
	}

	IEnumerator TryDelayedInvoke(float time, Check checkFunction, System.Action successFunction, System.Action finalFunction)
	{
		float timer = 0f;
		yield return new WaitForEndOfFrame();

		while (timer < time)
		{
			if (!checkFunction())
			{
				finalFunction();
				yield break;
			}
			timer += Time.deltaTime;
			loadingCursor.fillAmount = timer / time;
			yield return new WaitForEndOfFrame();
		}

		successFunction();
		finalFunction();
	}


	private void OnDisable()
	{
		RenderTexture.ReleaseTemporary(outlineTex);
		RenderTexture.ReleaseTemporary(cullTex);
		RenderTexture.ReleaseTemporary(tempTex);
	}
}
