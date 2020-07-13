using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ResizeButtonByText: MonoBehaviour
{
	public RectTransform childRectTransform;
	public float margin;

	RectTransform rectTransform;

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	private void Update()
	{
		var newSizeDelta = childRectTransform.sizeDelta;
		newSizeDelta.x += margin;
		rectTransform.sizeDelta = newSizeDelta;
	}
}
