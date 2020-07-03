//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

/*
* This is the specialization for Extended View when in first person.
*/
public class ExtendedViewSimple : ExtendedView
{
	private Camera _usedCamera;

	/// <summary>
	/// Bind extended view sensitivity settings here
	/// </summary>
	//protected override void UpdateSettings()
	//{
	//	var sensitivitySlider = 0.5f; //min 0 - 1 max

	//	GazeViewResponsiveness = 0.25f + sensitivitySlider * 0.5f;
	//	HeadViewResponsiveness = 0.5f + sensitivitySlider * 0.5f;
	//}

	protected override void Start()
	{
        base.Start();
		_usedCamera = GetComponentInChildren<Camera>();
	}

	protected override void UpdateTransform()
	{
		var localRotation = transform.localRotation;

		UpdateCameraWithoutExtendedView(_usedCamera);
		var worldUp = Vector3.up;
		Rotate(transform, up: worldUp);
		UpdateCameraWithExtendedView(_usedCamera);

		StartCoroutine(ResetCameraLocal(localRotation, transform));
	}
}