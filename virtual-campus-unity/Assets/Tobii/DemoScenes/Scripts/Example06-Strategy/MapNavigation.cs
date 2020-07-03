//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class MapNavigation : MapNavigationBase
{
	private Vector3? _lastGroundPosition;

	private float _cameraSpeed = 10f;

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

	//--------------------------------------------------------------------
	// Overridden abstract functions
	//--------------------------------------------------------------------

	protected override float GetCurrentZoomLevel()
	{

		return Vector3.Magnitude(ZoomVector);
	}

	protected override bool IsInteractionAllowed()
	{
		return true;
	}

	protected override bool ZoomOut(float currentZoomLevel, float targetZoomLevel)
	{
		var reachedMaxZoomedOutLevel = false;
		var zoomStepAmount = Time.unscaledDeltaTime * _cameraSpeed;
		if (currentZoomLevel < targetZoomLevel - zoomStepAmount)
		{
			currentZoomLevel = Mathf.Lerp(currentZoomLevel, targetZoomLevel, zoomStepAmount);
			CameraTransform.position = GetCurrentGroundPositionAtScreenCenter() - Vector3.Normalize(ZoomVector) * currentZoomLevel;
		}
		else
		{
			reachedMaxZoomedOutLevel = true;
		}

		return reachedMaxZoomedOutLevel;
	}

	protected override bool MoveTo(Vector3 targetGroundPosition, float targetZoomLevel)
	{
		var targetCameraPosition = targetGroundPosition - Vector3.Normalize(ZoomVector) * targetZoomLevel;

		var currentGroundPosition = CameraTransform.position;
		var delta = currentGroundPosition - targetCameraPosition;

		var reachedTargetPosition = delta.magnitude < 0.05f;

		if (_lastGroundPosition != null)
		{
			var lastDelta = _lastGroundPosition.Value - targetCameraPosition;
			reachedTargetPosition = reachedTargetPosition || (delta.magnitude >= lastDelta.magnitude);
		}

		if (!reachedTargetPosition)
		{
			float speed = Time.unscaledDeltaTime * _cameraSpeed;
			CameraTransform.position = Vector3.Lerp(CameraTransform.position, targetCameraPosition, speed);
		}

		_lastGroundPosition = currentGroundPosition;

		return reachedTargetPosition;
	}

	protected override Vector3 ProjectScreenPointToGroundPosition(Vector2 screenPoint)
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

	//--------------------------------------------------------------------
	// Overridden virtual functions
	//--------------------------------------------------------------------

	protected override void DoOptionalInitializeInteractionWork()
	{
		_lastGroundPosition = null;
	}
}