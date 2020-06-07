using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleCanvasBaseOnAnother : MonoBehaviour
{
    public RectTransform canvas;

    public int width;
    public int height;

    [Range(0, 1f)] public float fill;

    private void Update()
    {
        var scaler = GetComponent<CanvasScaler>();

        var ws = canvas.rect.width * fill / width;
        var hs = canvas.rect.height * fill / height;

        scaler.scaleFactor = Mathf.Min(ws, hs);
    }
}
