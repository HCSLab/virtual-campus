//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

/*
 * Aim At Gaze
 *
 * Aim At Gaze helps to decouple the aim direction from the view direction, allowing the player to fire in non-forward directions. 
 * This also helps to reduce screen movements which can be less disorienting, especially when you need to align your gun quickly for your next shot.
 * This component requies Extended View Component in order to do camera transitions.
 */
public class WeaponController : MonoBehaviour
{
	//This is where we are currently aiming in world space (the location we are aiming at)
	public Vector3 AimedAtLocation
	{
		get
		{
			return IsShootingAtGaze ? WorldGazeCrosshairTransformProjected.position : WorldCenterCrosshairTransformProjected.position;
		}
	}

	public bool IsAiming { get; protected set; }
	public bool IsShooting { get; protected set; }
	public bool IsShootingAtGaze { get; protected set; }
	//This is where we will shoot from. Usually the camera, but can be overridden. If overridden, it also enables the collision crosshair
	//since if the fire ray is not coincident with the camera normal, we can run into collision problems
	public Transform OptionalWeaponFireOriginOverride;

	//This is where any fired projectiles will hit
	public bool IsWeaponHitObject { get; private set; }
	public RaycastHit WeaponHitData { get; private set; }

	//If the path to our aimed position is blocked, we should display this crosshair
	public Image AimBlockedCrosshairImage;
	//This margin prevents the crosshair from being placed outside the screen, or too close to the edge.
	public float ScreenEdgeAimMarginPx = 40;
	//We need this to compensate for pre-saccade activation (the player pressed the aim button before the eye has started moving).
	public float AimDelaySecs = 0.02f;
	//If we have not gotten a new fixation before this time, simply use the center
	public float AimTimeoutTimeSecs = 0.7f;
	//If the gaze is within this deadzone in the center of the screen, we will not shift the crosshair
	public float NoAimShiftCenterDeadZoneRadius = 0.05f;
	//Filter gaze data for primary fire at gaze
	public float GazeFilterStrength = 1.0f;
	//Allow disabling
	public bool IsEnabled = true;

	public const float MaxProjectionDistance = 100000f;
	public const int RaycastLayerMask = ~0x24;//0b100100;           // ignore "ignore raycast" and "ui" layers
	public ExtendedViewBase ExtendedView;

	public Camera MainCamera;
	public Camera WeaponCamera;
	public float AimedFieldOfView = 40;
	public float NormalFieldOfView = 50;

	protected Transform WorldCenterCrosshairTransformProjected;
	protected Transform WorldCenterCrosshairTransformFixed;

	protected Transform WorldGazeCrosshairTransformProjected;
	protected Transform WorldGazeCrosshairTransformFixed;

	private Vector2 _screenCenterCrosshairPosition;
	private float _aimRequestTime;
	private bool _isRequestingAim;
	private Vector2 _filteredGazePoint;
	private bool _calculatedThisFrame;



	protected virtual void Start()
	{
		if (ExtendedView == null)
		{
			Debug.LogError("Missing Extended view component!");
		}

		WorldCenterCrosshairTransformProjected = new GameObject("WorldCenterCrosshairProjected").transform;
		WorldCenterCrosshairTransformProjected.transform.parent = null;

		WorldCenterCrosshairTransformFixed = new GameObject("WorldCenterCrosshairFixed").transform;
		WorldCenterCrosshairTransformFixed.transform.parent = ExtendedView.CameraWithoutExtendedView.transform;
		WorldCenterCrosshairTransformFixed.transform.localPosition = Vector3.forward;

		WorldGazeCrosshairTransformProjected = new GameObject("WorldGazeCrosshairProjected").transform;
		WorldGazeCrosshairTransformProjected.transform.parent = null;

		WorldGazeCrosshairTransformFixed = new GameObject("WorldGazeCrosshairFixed").transform;
		WorldGazeCrosshairTransformFixed.transform.parent = ExtendedView.CameraWithoutExtendedView.transform;
		WorldGazeCrosshairTransformFixed.transform.localPosition = Vector3.forward;

		_screenCenterCrosshairPosition = new Vector2(0.5f, 0.5f);

		_filteredGazePoint = new Vector2(Screen.width, Screen.height) * 0.5f;
	}

	private void Update()
	{
		_calculatedThisFrame = false;

		if (IsAiming)
		{
			MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, AimedFieldOfView, Time.unscaledDeltaTime * 5);
		}
		else
		{
			MainCamera.fieldOfView = Mathf.Lerp(MainCamera.fieldOfView, NormalFieldOfView, Time.unscaledDeltaTime * 5);
		}

		if (WeaponCamera != null)
		{
			WeaponCamera.fieldOfView = MainCamera.fieldOfView;
		}
	}

	public void Calculate()
	{
		if (_calculatedThisFrame) return;
		_calculatedThisFrame = true;

		if (IsEnabled)
		{
			var gazePoint = TobiiAPI.GetGazePoint();
			if (gazePoint.IsRecent())
			{
				var w = (float)(1 - GazeFilterStrength * 0.9);
				_filteredGazePoint = _filteredGazePoint + (gazePoint.Screen - _filteredGazePoint) * w;
			}
			else
			{
				_filteredGazePoint = new Vector2(Screen.width, Screen.height) * 0.5f;
			}
		}
		//If the user wants to start aiming, see if we can accommodate
		if (_isRequestingAim)
		{
			//If we still haven't gotten any gaze data, don't wait anymore and start the aim in the center instead
			if (Time.unscaledTime - _aimRequestTime > AimTimeoutTimeSecs)
			{
				IsAiming = true;
				_isRequestingAim = false;
				ExtendedView.AimAtWorldPosition(WorldCenterCrosshairTransformProjected.position);
			}
			else if (Time.unscaledTime - _aimRequestTime > AimDelaySecs)
			{
				IsAiming = true;
				_isRequestingAim = false;

				var gazePoint = TobiiAPI.GetGazePoint();

				if (gazePoint.IsRecent())
				{
					var normalizedCenteredGazeCoordinates = (gazePoint.Viewport - new Vector2(0.5f, 0.5f)) * 2;

					//Don't aim at gaze if the user is looking close to the center of the screen to avoid the crosshair jumping just a tiny amount
					if (normalizedCenteredGazeCoordinates.magnitude < NoAimShiftCenterDeadZoneRadius)
					{
						ExtendedView.AimAtWorldPosition(WorldCenterCrosshairTransformProjected.position);
					}
					else
					{
						var viewportCoordinates = ExtendedView.CameraWithExtendedView.WorldToViewportPoint(WorldCenterCrosshairTransformProjected.position);
						var normalizedCoordinatesDelta = (gazePoint.Viewport - new Vector2(viewportCoordinates.x, viewportCoordinates.y)) * 2;

						if (normalizedCoordinatesDelta.magnitude < NoAimShiftCenterDeadZoneRadius)
						{
							ExtendedView.AimAtWorldPosition(WorldCenterCrosshairTransformProjected.position);
						}
						else
						{
							var aimAtGazeTargetPosition = ScreenToWorldProjection(gazePoint.Screen);
							ExtendedView.AimAtWorldPosition(aimAtGazeTargetPosition);
						}
					}
				}
				else
				{
					ExtendedView.AimAtWorldPosition(WorldCenterCrosshairTransformProjected.position);
				}
			}
		}

		CalculateWorldCenterCrosshairPosition();
		UpdateCenterCrosshairScreenPosition();

		if (IsShootingAtGaze)
		{
			CalculateWorldGazeCrosshairPosition();
		}

		FindHitDataForPotentialOverride();

		UpdateAimBlockedCrosshair();
	}

	private void CalculateWorldGazeCrosshairPosition()
	{
		WorldGazeCrosshairTransformFixed.position = ScreenToWorldProjection(_filteredGazePoint);

		RaycastHit hitInfo;
		var direction = WorldGazeCrosshairTransformFixed.position - ExtendedView.CameraWithExtendedView.transform.position;
		direction.Normalize();
		if (Physics.Raycast(ExtendedView.CameraWithExtendedView.transform.position, direction, out hitInfo, MaxProjectionDistance, RaycastLayerMask))
		{
			WorldGazeCrosshairTransformProjected.position = hitInfo.point;
			WeaponHitData = hitInfo;
			IsWeaponHitObject = true;
		}
		else
		{
			WorldGazeCrosshairTransformProjected.position = WorldGazeCrosshairTransformFixed.position;

			IsWeaponHitObject = false;
		}
	}

	public void StartAiming()
	{
		//We want to wait for the next fixation before we actually start calculating the new aim direction
		_aimRequestTime = Time.unscaledTime;
		_isRequestingAim = true;
	}

	public void StopAiming()
	{
		_isRequestingAim = false;
		IsAiming = false;
	}

	//Since we allow extended view to shift the camera position when using the center crosshair (not gaze-aiming)
	//we need to figure out where to put the actual crosshair on screen every frame.
	private void UpdateCenterCrosshairScreenPosition()
	{
		_screenCenterCrosshairPosition = ExtendedView.CameraWithExtendedView.WorldToScreenPoint(WorldCenterCrosshairTransformProjected.position);
		//Clamp to pixel
		_screenCenterCrosshairPosition = new Vector2(Mathf.FloorToInt(_screenCenterCrosshairPosition.x + 0.25f), Mathf.FloorToInt(_screenCenterCrosshairPosition.y + 0.25f));
	}

	private void CalculateWorldCenterCrosshairPosition()
	{
		//When looking around with extended view, you don't want the crosshair to move away from what you were aiming at.
		//To make this happen, we have to use a fixed reference point that only moves when you move your aiming device (e.g. mouse)
		//This is why we exclude the extended view extra shift if it is present when doing these calculations

		//First figure out 3d position
		RaycastHit hitInfo;
		if (Physics.Raycast(ExtendedView.CameraWithoutExtendedView.transform.position,
			ExtendedView.CameraWithoutExtendedView.transform.forward, out hitInfo, MaxProjectionDistance, RaycastLayerMask))
		{
			WorldCenterCrosshairTransformProjected.position = hitInfo.point;

			if (OptionalWeaponFireOriginOverride == null)
			//If we don't have an override, we can just use this directly! Fantastic!
			{
				WeaponHitData = hitInfo;
				IsWeaponHitObject = true;
			}
		}
		else
		{
			WorldCenterCrosshairTransformProjected.position = ExtendedView.CameraWithoutExtendedView.transform.position +
															ExtendedView.CameraWithoutExtendedView.transform.forward * 100f;
			WeaponHitData = new RaycastHit
			{
				point = WorldCenterCrosshairTransformProjected.position,
				normal =
					(WorldCenterCrosshairTransformProjected.position - ExtendedView.CameraWithoutExtendedView.transform.position)
						.normalized,
				distance =
					(WorldCenterCrosshairTransformProjected.position - ExtendedView.CameraWithoutExtendedView.transform.position)
						.magnitude
			};
			IsWeaponHitObject = false;
		}
	}

	//If we have overridden the fire origin, we need to test this ray as well
	//since if the fire ray is not coincident with the camera normal, we can run into collision problems
	private void FindHitDataForPotentialOverride()
	{
		if (OptionalWeaponFireOriginOverride == null)
		{
			return;
		}

		RaycastHit hitInfo;
		var direction = AimedAtLocation - OptionalWeaponFireOriginOverride.position;
		direction.Normalize();
		if (Physics.Raycast(OptionalWeaponFireOriginOverride.position, direction, out hitInfo, MaxProjectionDistance, RaycastLayerMask))
		{
			WeaponHitData = hitInfo;
			IsWeaponHitObject = true;
		}
		else
		{
			IsWeaponHitObject = false;
		}
	}

	public void StartShooting()
	{
		IsShooting = true;
	}

	public void StartShootingAtGaze()
	{
		IsShooting = true;
		IsShootingAtGaze = true;
	}

	public void StopShooting()
	{
		IsShooting = false;
		IsShootingAtGaze = false;
	}

	private Vector3 ScreenToWorldProjection(Vector2 screenPosition)
	{
		//Raycast to find a hit point
		var worldPositionFixed = ExtendedView.CameraWithExtendedView.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 100));
		var direction = worldPositionFixed - ExtendedView.CameraWithExtendedView.transform.position;

		Vector3 worldPosition;
		RaycastHit hitInfo;
		if (Physics.Raycast(ExtendedView.CameraWithExtendedView.transform.position, direction, out hitInfo, MaxProjectionDistance, RaycastLayerMask))
		{
			worldPosition = hitInfo.point;
		}
		else
		{
			worldPosition = ExtendedView.CameraWithExtendedView.transform.position + direction * 100f;
		}
		return worldPosition;
	}

	private void UpdateAimBlockedCrosshair()
	{
		if (AimBlockedCrosshairImage != null)
		{
			//Show partially blocked crosshair if our aim and hit positions differ. This can only happen if we have some special weapon origin.
			if ((OptionalWeaponFireOriginOverride != null)
				&& (Vector3.Distance(AimedAtLocation, WeaponHitData.point) > 0.01f))
			{
				AimBlockedCrosshairImage.enabled = true;
				var screenPosition = ExtendedView.CameraWithExtendedView.WorldToScreenPoint(WeaponHitData.point);
				AimBlockedCrosshairImage.rectTransform.anchoredPosition = new Vector2(screenPosition.x - Screen.width / 2.0f, screenPosition.y - Screen.height / 2.0f);
			}
			else
			{
				AimBlockedCrosshairImage.enabled = false;
			}
		}
	}
}