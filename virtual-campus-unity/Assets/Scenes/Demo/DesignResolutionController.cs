using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ResolutionAdaptationMode
{
    FillHeight
};

[ExecuteInEditMode]
public class DesignResolutionController : MonoBehaviour
{
    public Vector2 designResolution = new Vector2(1920, 1080);

    public ResolutionAdaptationMode mode = ResolutionAdaptationMode.FillHeight;

    public CanvasScaler canvasScaler;

    private void Update()
    {
        var w = Screen.width;
        var h = Screen.height;

        if (mode == ResolutionAdaptationMode.FillHeight)
        {
            canvasScaler.scaleFactor = h / designResolution.y;
        }
    }
}
