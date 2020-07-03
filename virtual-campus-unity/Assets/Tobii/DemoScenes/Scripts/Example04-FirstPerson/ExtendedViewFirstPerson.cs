//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the specialization for Extended View when in first person.
/// This should be a direct child of the transform that is responsible for mouse yaw and pitch shifts, or it won't work.
/// The camera used for the first person controller should also be a child of the transform associated with this component.
/// </summary>
public class ExtendedViewFirstPerson : ExtendedView
{
	[Tooltip("Optional reference to a crosshair Image. If supplied the crosshair will be moved according to the character forward direction rather than stay at the center of screen.")]
	public Image OptionalCrosshair;

	private Camera _usedCamera;

	private Vector3 _crosshairScreenPosition;
	private WeaponController _weaponController;
	private SimpleMoveController SimpleMoveController { get; set; }

	/// <summary>
	/// Bind extended view sensitivity settings here
	/// </summary>
	//protected override void UpdateSettings()
	//{
	//	var sensitivitySlider = 0.5f; //min 0 - 1 max

	//	GazeViewResponsiveness = 0.25f + sensitivitySlider * 0.5f;
	//	HeadViewResponsiveness = 0.5f + sensitivitySlider * 0.5f;
	//}

	private void Awake()
	{
		SimpleMoveController = GetComponentInParent<SimpleMoveController>();
	}

	protected override void Start()
	{
        base.Start();
		// Make sure that our transform is in the identity state when we start.
		transform.localRotation = Quaternion.identity;
		transform.localPosition = Vector3.zero;

		_usedCamera = GetComponent<Camera>();
		_weaponController = GetComponentInParent<WeaponController>();
	}

	protected override void UpdateTransform()
	{
		if (_weaponController != null)
		{
			IsAiming = _weaponController.IsAiming;
		}
	}

	private void OnPreCull()
	{
		var crosshairWorldPosition = _usedCamera.transform.position + _usedCamera.transform.forward;
		var localRotation = transform.localRotation;
		UpdateCameraWithoutExtendedView(_usedCamera);
		Rotate(transform);
		UpdateCameraWithExtendedView(_usedCamera);

		_crosshairScreenPosition = _usedCamera.WorldToScreenPoint(crosshairWorldPosition);
		UpdateCrosshair();

		StartCoroutine(ResetCameraLocal(localRotation, transform));
	}

	private void UpdateCrosshair()
	{
		if (OptionalCrosshair == null) return;

		var canvas = OptionalCrosshair.GetComponentInParent<Canvas>();

		OptionalCrosshair.rectTransform.anchoredPosition =
			new Vector2((_crosshairScreenPosition.x - Screen.width * 0.5f) * (canvas.GetComponent<RectTransform>().sizeDelta.x / Screen.width),
			(_crosshairScreenPosition.y - Screen.height * 0.5f) * (canvas.GetComponent<RectTransform>().sizeDelta.y / Screen.height));
	}

	/// <summary>
	/// Aim at Gaze: ...
	/// If you are calling this function manually, make sure that IsAiming property is updated on Update()
	/// </summary>
	/// <param name="worldPostion"></param>
	public override void AimAtWorldPosition(Vector3 worldPostion)
	{
		if (_weaponController != null)
		{
			IsAiming = _weaponController.IsAiming;
		}

		if (SimpleMoveController == null)
		{
			return;
		}

		var direction = worldPostion - transform.position;
		var desiredRotation = Quaternion.LookRotation(direction);

		Yaw = Mathf.DeltaAngle(desiredRotation.eulerAngles.y, CameraWithExtendedView.transform.rotation.eulerAngles.y);
		Pitch = Mathf.DeltaAngle(desiredRotation.eulerAngles.x, CameraWithExtendedView.transform.rotation.eulerAngles.x);

		SimpleMoveController.SetRotation(desiredRotation);
	}
}