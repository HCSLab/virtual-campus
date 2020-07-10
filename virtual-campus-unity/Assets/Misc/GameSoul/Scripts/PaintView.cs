//-----------------------------------------------------------------------
// <copyright file="PaintView.cs" company="Codingworks Game Development">
//     Copyright (c) codingworks. All rights reserved.
// </copyright>
// <author> codingworks </author>
// <email> coding2233@163.com </email>
// <time> 2017-12-10 </time>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PaintView : MonoBehaviour
{
    #region 属性

    //绘图shader&material
    [SerializeField]
    private Shader _paintBrushShader;
    private Material _paintBrushMat;
    //清理renderTexture的shader&material
    [SerializeField]
    private Shader _clearBrushShader;
    private Material _clearBrushMat;
    //默认笔刷RawImage
    [SerializeField]
    private RawImage _defaultBrushRawImage;
    //默认笔刷&笔刷合集
    [SerializeField]
    private Texture _defaultBrushTex;
    //renderTexture
    private RenderTexture _renderTex;
    //默认笔刷RawImage
    [SerializeField]
    private Image _defaultColorImage;
    //绘画的画布
    [SerializeField]
    private RawImage _paintCanvas;
    //笔刷的默认颜色&颜色合集
    [SerializeField]
    private Color _defaultColor;
    //笔刷大小的slider
    private Text _brushSizeText;
    //笔刷的大小
    private float _brushSize;
    //屏幕的宽高
    private int _screenWidth;
    private int _screenHeight;
    //笔刷的间隔大小
    private float _brushLerpSize;
    //默认上一次点的位置
    private Vector2 _lastPoint;
    public Slider slider;
    private GameObject skinBagObject;
    public GameObject painterHub;
    private SkinItem currentSkinItem;
    private Texture2D currentTex;
	#endregion

	void Awake()
	{
        //Debug.Log(gameObject);
        //Debug.Log(UIManager.Instance);
        skinBagObject = painterHub.GetComponent<PainterHub>().skinBagObject;
        InitData();
        Reload();
	}

	private void Update()
	{
        //Debug.Log(UIManager.Instance);
        Color clearColor = new Color(0, 0, 0, 0);
		if (Input.GetKeyDown(KeyCode.Space))
			_paintBrushMat.SetColor("_Color", clearColor);
    }


	#region 外部接口

	public void SetBrushSize(float size)
    {
        _brushSize = size/3;
       _paintBrushMat.SetFloat("_Size", _brushSize);
    }

    public void SetBrushTexture(Texture texture)
    {
        _defaultBrushTex = texture;
        _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
        _defaultBrushRawImage.texture = _defaultBrushTex;
    }

    public void SetBrushColor(Color color)
    {
        _defaultColor = color;
        _paintBrushMat.SetColor("_Color", _defaultColor);
        _defaultColorImage.color = _defaultColor;
    }
    /// <summary>
    /// 选择颜色
    /// </summary>
    /// <param name="image"></param>
    public void SelectColor(Image image)
    {
        SetBrushColor(image.color);
    }
    /// <summary>
    /// 选择笔刷
    /// </summary>
    /// <param name="rawImage"></param>
    public void SelectBrush(RawImage rawImage)
    {
        SetBrushTexture(rawImage.texture);
    }
    /// <summary>
    /// 设置笔刷大小
    /// </summary>
    /// <param name="value"></param>
    public void BrushSizeChanged(Slider slider)
    {
        //  float value = slider.maxValue + slider.minValue - slider.value;
        if (_paintBrushMat == null)
            return;
        SetBrushSize(Remap(slider.value,100.0f,10.0f));
        if (_brushSizeText == null)
        {
            _brushSizeText=slider.transform.Find("Background/Text").GetComponent<Text>();
        }
        _brushSizeText.text = slider.value.ToString("f2");
    }



    /// <summary>
    /// 拖拽
    /// </summary>
    public void DragUpdate()
    {
        if (_renderTex && _paintBrushMat)
        {
            if (Input.GetMouseButton(0))
            {
                LerpPaint(Input.mousePosition);
            }
        }
    }
    /// <summary>
    /// 拖拽结束
    /// </summary>
    public void DragEnd()
    {
        if (Input.GetMouseButtonUp(0))
            _lastPoint = Vector2.zero;
    }

    #endregion

    #region 内部函数
	
    //初始化数据
    public void InitData()
    {
        _brushSizeText = slider.transform.Find("Background/Text").GetComponent<Text>();
        _brushSizeText.text = 50.ToString("f2"); 
        _brushSize = 100.0f;
        _brushLerpSize = (_defaultBrushTex.width + _defaultBrushTex.height) / 2.0f / _brushSize;

        _lastPoint = Vector2.zero;

        Texture playerTexture = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkin>().GetCurrentSkinTexture();

        if (_paintBrushMat == null)
        {
            UpdateBrushMaterial();
        }
        if (_clearBrushMat == null)
        {
            _clearBrushMat = new Material(_clearBrushShader);
        }
        if (_renderTex == null)
        {

            float scale = Screen.width / GetComponent<CanvasScaler>().referenceResolution.x;
            //Debug.Log(Screen.width);
            //Debug.Log(scale);
            _screenWidth = (int) (transform.Find("Panel/Mask/RawImage_canvas").GetComponent<RectTransform>().rect.size.x * scale);
            _screenHeight = (int) (transform.Find("Panel/Mask/RawImage_canvas").GetComponent<RectTransform>().rect.size.y * scale);
            //Debug.Log(_screenWidth);

            /*
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            */
            /*
            _screenWidth = (int)(transform.Find("Panel/RawImage_canvas").GetComponent<RectTransform>().rect.size.x);
            _screenHeight = (int)(transform.Find("Panel/RawImage_canvas").GetComponent<RectTransform>().rect.size.y);
            */

            _renderTex = RenderTexture.GetTemporary(512, 512, 24);
            _paintCanvas.texture = _renderTex;
        }
        //Graphics.Blit(null, _renderTex, _clearBrushMat);
        Graphics.Blit(playerTexture, _renderTex);
        BrushSizeChanged(slider);
    }

    //更新笔刷材质
    private void UpdateBrushMaterial()
    {
        _paintBrushMat = new Material(_paintBrushShader);
        _paintBrushMat.SetTexture("_BrushTex", _defaultBrushTex);
        _paintBrushMat.SetColor("_Color", _defaultColor);
        _paintBrushMat.SetFloat("_Size", _brushSize);
    }

    //插点
    private void LerpPaint(Vector2 point)
    {

        RectTransform re = transform.Find("Panel/Mask/RawImage_canvas").gameObject.GetComponent<RectTransform>();
        //Debug.Log(point - ((Vector2)re.transform.position + new Vector2(-180, -300)));
        Paint(point);

        if (_lastPoint == Vector2.zero)
        {
            _lastPoint = point;
            return;
        }

        float dis = Vector2.Distance(point, _lastPoint);
        if (dis > _brushLerpSize)
        {
            Vector2 dir = (point - _lastPoint).normalized;
            int num = (int)(dis / _brushLerpSize);
            for (int i = 0; i < num; i++)
            {
                Vector2 newPoint = _lastPoint + dir * (i + 1) * _brushLerpSize;
                Paint(newPoint);
            }
        }
        _lastPoint = point;
    }

    //画点
    private void Paint(Vector2 point)
    {
        /*
        if (point.x < 0 || point.x > _screenWidth || point.y < 0 || point.y > _screenHeight)
        {
            Debug.Log(point);
            return;
        }
        */
        RectTransform re = transform.Find("Panel/Mask/RawImage_canvas").gameObject.GetComponent<RectTransform>();
        RectTransform mask = transform.Find("Panel/Mask").gameObject.GetComponent<RectTransform>();

        float scale = Screen.width / GetComponent<CanvasScaler>().referenceResolution.x;
        int minY = (int)(mask.rect.top*scale);
        int minX = (int)(mask.rect.left*scale);
        Vector2 point_mask = point - ((Vector2)mask.transform.position + new Vector2(minX, minY));

        minY = (int)(re.rect.top * scale);
        minX = (int)(re.rect.left * scale);
        point = point - ((Vector2)re.transform.position + new Vector2(minX, minY));
        //Debug.Log(0.5*Screen.width);
        //Debug.Log(re.transform.position.x);
        //Debug.Log(point);

        if (point_mask.x < 0 || point_mask.x > mask.rect.width || point_mask.y < 0 || point_mask.y > mask.rect.height)
        {
            return;
        }

        Vector2 uv = new Vector2(point.x / (float)_screenWidth,
            point.y / (float)_screenHeight);
        _paintBrushMat.SetVector("_UV", uv);
        Graphics.Blit(_renderTex, _renderTex, _paintBrushMat);
    }

    public void PointPaint()
    {

        Vector2 point = Input.mousePosition;

        RectTransform re = _paintCanvas.GetComponent<RectTransform>();

        float scale = Screen.width / GetComponent<CanvasScaler>().referenceResolution.x;
        int minY = (int)(re.rect.top * scale);
        int minX = (int)(re.rect.left * scale);


        point = point - ((Vector2)re.transform.position + new Vector2(minX, minY));
        //Debug.Log(0.5*Screen.width);
        //Debug.Log(re.transform.position.x);
        //Debug.Log(point);

        if (point.x < 0 || point.x > re.rect.width || point.y < 0 || point.y > re.rect.height)
        {
            return;
        }

        Vector2 uv = new Vector2(point.x / (float)_screenWidth,
            point.y / (float)_screenHeight);
        _paintBrushMat.SetVector("_UV", uv);
        Graphics.Blit(_renderTex, _renderTex, _paintBrushMat);
    }

    /// <summary>
    /// 重映射  默认  value 为1-100
    /// </summary>
    /// <param name="value"></param>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    /// <returns></returns>
    private float Remap(float value, float startValue, float enValue)
    {
        float returnValue = (value - 1.0f) / (100.0f - 1.0f);
        returnValue = (enValue - startValue) * returnValue + startValue;
        return returnValue;
    }

    #endregion

    public void OnPainterButtonClicked()
    {
        float alpha = GetComponent<CanvasGroup>().alpha;
        if (alpha == 1)
        {
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().interactable = false;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
        }
        else
        {
            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().interactable = true;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
    }

    public void Save()
    {
        if (currentSkinItem == null)
        {
            SaveAs();
            return;
        }
        if (!currentSkinItem.customized)
        {
            
            SaveAs();
            return;
        }
        else
        {
            Texture tex = _paintCanvas.GetComponent<RawImage>().texture;
            Sprite sprite = Sprite.Create(TextureToTexture2D(tex), new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1000);
            sprite.name = "sprite";
            currentSkinItem.texture = TextureToTexture2D(tex);
            currentSkinItem.image = sprite;
            var skinBag = skinBagObject.GetComponent<SkinBag>();
            //skinBag.Reload();
            skinBag.ReloadSprites();
            gameObject.SetActive(false);
        }
    }

    public void SaveAs()
    {
        Texture tex = _paintCanvas.GetComponent<RawImage>().texture;
        Sprite sprite = Sprite.Create(TextureToTexture2D(tex), new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1000);
        sprite.name = "Customized Sprite";
        painterHub.GetComponent<PainterHub>().SaveAs(TextureToTexture2D(tex), sprite);
        return;
        /*
        string name = "name";
        string description = "description";
        //TextureToPNG toPNG = new TextureToPNG();
        //toPNG.SaveRenderTextureToPNG(TextureToTexture2D(tex), outputShader, "Assets/Sprites/Skins", "1");
        Sprite sprite = Sprite.Create(TextureToTexture2D(tex), new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 1000);
        //Sprite sprite = Sprite.Create(TextureToTexture2D(tex), new Rect(0, 0, 150, 250), Vector2.zero);
        sprite.name = "sprite";
        //SpriteItem spriteItem = new SpriteItem(new Item(name, description, sprite));
        var skinBag = skinBagObject.GetComponent<SkinBag>();
        GameObject newSkin = new GameObject();
        newSkin.name = "Customized Sprite";
        newSkin.transform.SetParent(skinBag.transform);
        var spriteItem = newSkin.AddComponent<SkinItem>();
        spriteItem.itemName = name;
        spriteItem.description = description;
        spriteItem.image = sprite;
        spriteItem.texture = TextureToTexture2D(tex);
        skinBag.testItems.Add(newSkin);
        skinBag.Reload();
        UIManager.Instance.GetComponent<UIManager>().DeactivatePanel(gameObject);
        */
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Clear()
    {
        //Texture playerTexture = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkin>().playerTexture;;
        Texture tex = currentTex;
        RenderTexture old = (RenderTexture) _paintCanvas.texture;
        _renderTex = RenderTexture.GetTemporary(512, 512, 24);
        _paintCanvas.texture = _renderTex;
        Graphics.Blit(tex, _renderTex);
        old.Release();
    }

    public Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }

    public void Reload()
    {
        if (skinBagObject == null)
        {
            return;
        }
        SkinItem skinItem = (SkinItem)skinBagObject.GetComponent<SkinBag>().currentItem;
        Graphics.Blit(skinItem.texture, _renderTex);
        currentSkinItem = skinItem;
        currentTex = skinItem.texture;
    }

}
