using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class ChangeCursorOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Texture2D hoverTexture, downTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    bool isCursorInRect = false;

	private void Update()
	{
		if (isCursorInRect)
		{
			if (Input.GetMouseButton(0))
			{
				SetPanCursor();
			}
			else
				SetHoverCursor();
		}
		else
			ResetCursor();
	}

	void SetHoverCursor()
	{
        Cursor.SetCursor(hoverTexture, hotSpot, cursorMode);
    }
    void SetPanCursor()
	{
        if (downTexture)
            Cursor.SetCursor(downTexture, hotSpot, cursorMode);
    }
	void ResetCursor()
	{
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
	{
        isCursorInRect = true;
	}

    public void OnPointerExit(PointerEventData pointerEventData)
	{
        isCursorInRect = false;
	}
}
