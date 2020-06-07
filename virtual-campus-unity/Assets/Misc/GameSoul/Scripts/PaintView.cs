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
	#endregion

	void Start()
	{
        InitData();
	}

	private void Update()
	{
		Color clearColor = new Color(0, 0, 0, 0);
		if (Input.GetKeyDown(KeyCode.Space))
			_paintBrushMat.SetColor("_Color", clearColor);
    }


	#region 外部接口

	public void SetBrushSize(float size)
    {
        _brushSize = size;
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
        SetBrushSize(Remap(slider.value,300.0f,30.0f));
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
    void InitData()
    {
        _brushSize = 300.0f;
        _brushLerpSize = (_defaultBrushTex.width + _defaultBrushTex.height) / 2.0f / _brushSize;
        _lastPoint = Vector2.zero;

        if (_paintBrushMat == null)
        {
            UpdateBrushMaterial();
        }
        if(_clearBrushMat==null)
        _clearBrushMat = new Material(_clearBrushShader);
        if (_renderTex == null)
        {
            _screenWidth = (int) GameObject.FindGameObjectWithTag("Painter").GetComponent<RectTransform>().rect.size.x;
            _screenHeight = (int) GameObject.FindGameObjectWithTag("Painter").GetComponent<RectTransform>().rect.size.y;
            /*
            _screenWidth = Screen.width;
            _screenHeight = Screen.height;
            */

            _renderTex = RenderTexture.GetTemporary(_screenWidth, _screenHeight, 24);
            _paintCanvas.texture = _renderTex;
        }
        Graphics.Blit(null, _renderTex, _clearBrushMat);
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
        RectTransform re = GameObject.FindGameObjectWithTag("Painter").GetComponent<RectTransform>();
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
        RectTransform re = GameObject.FindGameObjectWithTag("Painter").GetComponent<RectTransform>();

        int minY = (int)re.rect.top;
        int maxY = (int)re.rect.bottom;
        int minX = (int)re.rect.left;
        int maxX = (int)re.rect.right;


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
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void Save()
    {
        string name = "name";
        string description = "description";
        Texture tex = GameObject.FindGameObjectWithTag("Painter").GetComponent<RawImage>().texture;
        Sprite sprite = Sprite.Create(TextureToTexture2D(tex), new Rect(0, 0, 0, 0), Vector2.zero);
        //SpriteItem spriteItem = new SpriteItem(new Item(name, description, sprite));
        GameObject newSprite = new GameObject();
        newSprite.AddComponent<SpriteItem>();
        newSprite.GetComponent<SpriteItem>().itemName = name;
        newSprite.GetComponent<SpriteItem>().description = description;
        newSprite.GetComponent<SpriteItem>().image = sprite;
        GameObject.FindGameObjectWithTag("SkinBag").GetComponent<SkinBag>().testItems.Add(newSprite);
    }

    private Texture2D TextureToTexture2D(Texture texture)
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
}
