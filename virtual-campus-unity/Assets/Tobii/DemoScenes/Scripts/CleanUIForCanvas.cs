//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;

public class CleanUIForCanvas : CleanUI
{
	protected override Type ElementType
	{
		get { return typeof(CanvasGroup); }
	}

	protected override void SetElementOpacity(Component el, float opacity)
	{
		var element = el as CanvasGroup;
		element.alpha = opacity;
	}

	protected override void Init()
	{
		if (GetComponent<CanvasGroup>() == null)
		{
			gameObject.AddComponent<CanvasGroup>();
		}
	}

	protected override Rect GetViewportBounds()
	{
		var minMaxVector = RectTransformBounds(GetComponent<RectTransform>());
		Vector2 areaWorldSpaceMin = new Vector2(minMaxVector.x, minMaxVector.y);
		Vector2 areaWorldSpaceMax = new Vector2(minMaxVector.z, minMaxVector.w);

		return CalculateViewportBounds(GetComponentInParent<Canvas>(), AdditionalEyetrackingMarginScale, areaWorldSpaceMin, areaWorldSpaceMax);
	}

	public static Rect CalculateCompositeBounds(params GameObject[] objects)
	{
		if (objects.Length == 0)
			throw new ArgumentException("params GameObject objects missing");

		Vector2 areaWorldSpaceMin = Vector2.one * Single.MaxValue;
		Vector2 areaWorldSpaceMax = Vector2.zero;

		foreach (GameObject go in objects)
		{
			var minMaxVector = RectTransformBounds(go.GetComponent<RectTransform>());

			if (minMaxVector.x < areaWorldSpaceMin.x)
				areaWorldSpaceMin.x = minMaxVector.x;
			if (minMaxVector.y < areaWorldSpaceMin.y)
				areaWorldSpaceMin.y = minMaxVector.y;
			if (minMaxVector.z > areaWorldSpaceMax.x)
				areaWorldSpaceMax.x = minMaxVector.z;
			if (minMaxVector.w > areaWorldSpaceMax.y)
				areaWorldSpaceMax.y = minMaxVector.w;
		}

		return CalculateViewportBounds(objects[0].GetComponentInParent<Canvas>(), 0.3f, areaWorldSpaceMin, areaWorldSpaceMax);
	}

	private static Rect CalculateViewportBounds(Canvas canvas, float additionalEyetrackingMarginScale, Vector2 areaWorldSpaceMin, Vector2 areaWorldSpaceMax)
	{
		var unadjustedBounds = new Rect(areaWorldSpaceMin.x, areaWorldSpaceMin.y, areaWorldSpaceMax.x - areaWorldSpaceMin.x, areaWorldSpaceMax.y - areaWorldSpaceMin.y);

		//Convert to screen coordinates if needed
		if (canvas != null)
		{
			if (canvas.worldCamera != null)
			{
				var renderingCamera = canvas.worldCamera;
				var rightBottom = renderingCamera.WorldToScreenPoint(unadjustedBounds.position + unadjustedBounds.size);
				unadjustedBounds.position = renderingCamera.WorldToScreenPoint(unadjustedBounds.position);
				unadjustedBounds.size = new Vector2(rightBottom.x, rightBottom.y) - unadjustedBounds.position;
			}
		}

		var pixelScale = Mathf.Min(unadjustedBounds.width, unadjustedBounds.height) * additionalEyetrackingMarginScale;

		return new Rect((unadjustedBounds.position.x - pixelScale) / Screen.width, (unadjustedBounds.position.y - pixelScale) / Screen.height, (unadjustedBounds.width + pixelScale * 2.0f) / Screen.width, (unadjustedBounds.height + pixelScale * 2.0f) / Screen.height);
	}

	private static Vector4 RectTransformBounds(RectTransform rt)
	{
		var returnVector = new Vector4();
		var fourCorners = new Vector3[4];
		rt.GetWorldCorners(fourCorners);

		returnVector.x = Mathf.Min(Mathf.Min(Mathf.Min(fourCorners[0].x, fourCorners[1].x), fourCorners[2].x), fourCorners[3].x);
		returnVector.y = Mathf.Min(Mathf.Min(Mathf.Min(fourCorners[0].y, fourCorners[1].y), fourCorners[2].y), fourCorners[3].y);
		returnVector.z = Mathf.Max(Mathf.Max(Mathf.Max(fourCorners[0].x, fourCorners[1].x), fourCorners[2].x), fourCorners[3].x);
		returnVector.w = Mathf.Max(Mathf.Max(Mathf.Max(fourCorners[0].y, fourCorners[1].y), fourCorners[2].y), fourCorners[3].y);
		return returnVector;
	}
}