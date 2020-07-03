using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

public class GameObjectAndOpacity
{
	public GameObject GameObject { get; private set; }
	public float Opacity { get; set; }

	public GameObjectAndOpacity(GameObject gameObject, float opacity)
	{
		GameObject = gameObject;
		Opacity = opacity;
	}
}

[RequireComponent(typeof(Camera))]
public class HighlightsPostEffect : MonoBehaviour
{
	public bool UseDepthBuffer = true;
	public int ResolutionDownsampleScale = 1;

	public string OccludersTag = "Occluder";
	public Color HighlightColor = new Color(1f, 1f, 1f, 0.9f);

	public Shader HighlightShader;
	public Shader HighlightBlurShader;

	private List<GameObjectAndOpacity> _highlightObjectsWithOpacity;
	private List<GameObject> _occluders;
	
	private Material _highlightMaterial;
	
	private CommandBuffer _renderBuffer;

	private int _renderTextureWidth = 512;
	private int _renderTextureHeight = 512;

	//Blur
	public int Downsample = 1;
	public int BlurSize = 3;
	public int BlurIterations = 2;
	private Material _blurMaterial;

	private const int ShaderPassGlow = 0;
	private const int ShaderPassOccluders = 1;
	private const int ShaderPassHighlightsOverlay = 2;
	private const int ShaderPassHighlightsDepthFilter = 3;

	private void Awake()
	{
		_renderBuffer = new CommandBuffer();

		SetOccluderObjects();

		_highlightObjectsWithOpacity = new List<GameObjectAndOpacity>();
		_occluders = new List<GameObject>();

		_renderTextureWidth = (int)(Screen.width / (float)ResolutionDownsampleScale);
		_renderTextureHeight = (int)(Screen.height / (float)ResolutionDownsampleScale);
	}

	public void SetHighlightedObjects(IEnumerable<GameObjectAndOpacity> gameObjects)
	{
		if (gameObjects == null)
		{
			_highlightObjectsWithOpacity = new List<GameObjectAndOpacity>();
			return;
		}

		_highlightObjectsWithOpacity = gameObjects.ToList();
	}

	private void SetOccluderObjects()
	{
		if( string.IsNullOrEmpty(OccludersTag) )
			return;

		_occluders = new List<GameObject>();

		if (OccludersTag != "")
		{
			var occluderGOs = GameObject.FindGameObjectsWithTag(OccludersTag);

			_occluders = occluderGOs.Where(go => go.GetComponent<Renderer>() != null).ToList();
		}
		else
		{
			_occluders = new List<GameObject>();
		}
	}
	
	private void RenderHighlights(RenderTexture rt)
	{
		if ( _highlightObjectsWithOpacity == null )
			return;

		var rtid = new RenderTargetIdentifier(rt);
		_renderBuffer.SetRenderTarget( rtid );

		foreach (var it in _highlightObjectsWithOpacity)
		{
			if( it == null )
				continue;

			if( it.GameObject == null )
				continue;

			var mat = new Material(_highlightMaterial);
			mat.SetColor("_Color", new Color(0, 0, 0, it.Opacity));

			var renderers = new List<Renderer>();
			renderers.AddRange(it.GameObject.GetComponents<Renderer>());
			renderers.AddRange(it.GameObject.GetComponentsInChildren<Renderer>(true));
			
			foreach (var goRenderer in renderers)
			{
				if (goRenderer.gameObject.activeInHierarchy && goRenderer.enabled)
				{
					_renderBuffer.DrawRenderer(goRenderer, mat, 0, UseDepthBuffer ? ShaderPassHighlightsDepthFilter : ShaderPassHighlightsOverlay);
				}
			}
		}

		RenderTexture.active = rt;
		Graphics.ExecuteCommandBuffer(_renderBuffer);
		RenderTexture.active = null;
	}
	
	private void RenderOccluders(RenderTexture rt)
	{
		var rtid = new RenderTargetIdentifier(rt);
		_renderBuffer.SetRenderTarget(rtid);

		_renderBuffer.Clear();

		var mat = new Material(_highlightMaterial);
		mat.SetColor("_Color", new Color(0, 0, 0, 1));

		foreach (var go in _occluders)
		{
			var goRenderer = go.GetComponent<Renderer>();
			if (goRenderer != null)
			{
				_renderBuffer.DrawRenderer(goRenderer, mat, 0, UseDepthBuffer ? ShaderPassHighlightsDepthFilter : ShaderPassHighlightsOverlay);
			}
		}

		RenderTexture.active = rt;
		Graphics.ExecuteCommandBuffer(_renderBuffer);
		RenderTexture.active = null;
	}

	public void RenderBlur(RenderTexture source, RenderTexture destination)
	{
		if (_blurMaterial == null)
		{
			_blurMaterial = new Material(HighlightBlurShader);
		}

		float num1 = (float)(1.0 / (1.0 * (double)(1 << Downsample)));
		_blurMaterial.SetVector("_Parameter", new Vector4(BlurSize * num1, -BlurSize * num1, 0.0f, 0.0f));
		source.filterMode = FilterMode.Bilinear;
		int width = source.width >> Downsample;
		int height = source.height >> Downsample;

		RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 0, source.format);
		renderTexture.filterMode = FilterMode.Bilinear;

		Graphics.Blit(source, renderTexture, _blurMaterial, 0);
		int num2 = 0;//// 2;
		for (int index = 0; index < BlurIterations; ++index)
		{
			float num3 = index * 1f;
			_blurMaterial.SetVector("_Parameter", new Vector4(BlurSize * num1 + num3, -BlurSize * num1 - num3, 0.0f, 0.0f));
			RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary1.filterMode = FilterMode.Bilinear;
			Graphics.Blit(renderTexture, temporary1, _blurMaterial, 1 + num2);
			RenderTexture.ReleaseTemporary(renderTexture);
			RenderTexture temp = temporary1;
			RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, source.format);
			temporary2.filterMode = FilterMode.Bilinear;
			Graphics.Blit(temp, temporary2, _blurMaterial, 2 + num2);
			RenderTexture.ReleaseTemporary(temp);
			renderTexture = temporary2;
		}
		Graphics.Blit(renderTexture, destination);
		RenderTexture.ReleaseTemporary(renderTexture);
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (_highlightMaterial == null)
		{
			_highlightMaterial = new Material(HighlightShader);
		}

		var highlightRt = RenderTexture.GetTemporary(_renderTextureWidth, _renderTextureHeight, 0, RenderTextureFormat.R8);

		RenderTexture.active = highlightRt;
		GL.Clear(true, true, Color.clear);
		RenderTexture.active = null;

		_renderBuffer.Clear();

		// Render all the highlight objects either with Overlay shader or DepthFilter
		RenderHighlights(highlightRt);
		var blurredRt = RenderTexture.GetTemporary(_renderTextureWidth, _renderTextureHeight, 0, RenderTextureFormat.R8);
		// Downsample and blur the result image using standard BlurOptimized image effect
		RenderBlur(highlightRt, blurredRt);

		// Render occluders to the same render texture
		RenderOccluders(highlightRt);
		var occludedRt = RenderTexture.GetTemporary(_renderTextureWidth, _renderTextureHeight, 0, RenderTextureFormat.R8);

		// Excluding the original image from the blurred image, leaving out the areal alone
		_highlightMaterial.SetTexture("_OccludeMap", highlightRt);

		Graphics.Blit(blurredRt, occludedRt, _highlightMaterial, ShaderPassOccluders);

		_highlightMaterial.SetTexture("_OccludeMap", occludedRt);

		RenderTexture.ReleaseTemporary(occludedRt);

		_highlightMaterial.SetColor("_Color", HighlightColor);

		// Renders the result image over the main camera's G-Buffer
		Graphics.Blit(source, destination, _highlightMaterial, ShaderPassGlow);

		RenderTexture.ReleaseTemporary(blurredRt);
		RenderTexture.ReleaseTemporary(highlightRt);
	}
}