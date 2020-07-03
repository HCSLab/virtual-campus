//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class MouseNavigation : MonoBehaviour
{
	public int MaxZoomLevel = 100;
	public int MinZoomLevel = 10;

	private Vector3 _lastMousePosition;
	private Vector3 _lastWorldPosition;

	private Transform CameraTransform
	{
		get { return Camera.main.transform; }
	}

	private Camera Camera
	{
		get { return Camera.main; }
	}

	private Vector3 ZoomVector
	{
		get
		{
			return GetCurrentGroundPositionAtScreenCenter() - CameraTransform.position;
		}
	}

	private void Update()
	{
		//MouseScroll
		var scrollScale = 1f;
		var scrollDelta = Input.mouseScrollDelta.y * scrollScale;
		if (Mathf.Abs(scrollDelta) > 0)
		{
			var zoomLevel = GetCurrentZoomLevel() - scrollDelta;
			zoomLevel = Mathf.Clamp(zoomLevel, MinZoomLevel, MaxZoomLevel);
			CameraTransform.position = GetCurrentGroundPositionAtScreenCenter() - Vector3.Normalize(ZoomVector) * zoomLevel;
		}

		//MousePan
		if (Input.GetKey(KeyCode.Mouse0))
		{
			var mouseDelta = Input.mousePosition - _lastMousePosition;
			if ((mouseDelta.magnitude > 0) && !Input.GetKeyDown(KeyCode.Mouse0))
			{
				var worldPosition = ProjectScreenPointToGroundPosition(Input.mousePosition);
				var delta = worldPosition - _lastWorldPosition;
				CameraTransform.position = CameraTransform.position - delta;
			}
			_lastMousePosition = Input.mousePosition;
			_lastWorldPosition = ProjectScreenPointToGroundPosition(Input.mousePosition);
		}
	}

	protected Vector3 GetCurrentGroundPositionAtScreenCenter()
	{
		return ProjectScreenPointToGroundPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
	}

	//--------------------------------------------------------------------
	// Overridden abstract functions
	//--------------------------------------------------------------------

	protected float GetCurrentZoomLevel()
	{

		return Vector3.Magnitude(ZoomVector);
	}

	protected Vector3 ProjectScreenPointToGroundPosition(Vector2 screenPoint)
	{
		var ray = Camera.ScreenPointToRay(screenPoint);
		var intersectionPoint = CameraTransform.position;
		float enter;
		if (new Plane(Vector3.up, Vector3.zero).Raycast(ray, out enter))
		{
			intersectionPoint = ray.GetPoint(enter);
		}
		return intersectionPoint;
	}
}