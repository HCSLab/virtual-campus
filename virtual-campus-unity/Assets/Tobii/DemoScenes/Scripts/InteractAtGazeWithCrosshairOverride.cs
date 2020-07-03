//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

/// <summary>
/// This version of Interact at Gaze uses:
/// - Single Object Selection
/// - Crosshair Override
/// - Activation Time
/// 
/// ## Single Object Selection using eye-gaze
/// Only one object is focused for selection at the time. To be selected an object
/// goes through the following steps/states:
/// - Normal (not looked at)
/// - Focused for selection (looked at, compare with UI Button state 'Highlighted')
/// - Selected for interaction (key pressed while looked at, compare with UI Button state 'Pressed')
/// 
/// ## Crosshair Override
/// If the user is looking inside a circular override area around the 
/// crosshair and the crosshair is pointing at an object, this object
/// will be focused for selection - overriding if the user is looking
/// at another object within the circle.
/// 
/// ## Activation Time
/// The Activation Time is the time duration the interaction/selection key has
/// to be pressed before a selection occurs. If the key is released within this
/// time period, the selection will be cancelled. If the Activation Time is set
/// to a value higher than zero, an activation progress icon will be displayed
/// on top of the item the selection is in progress for.
/// 
/// ## Key bindings
/// To select a focused (highlighted) object:
/// - Use the 'F' key on the keyboard 
/// - Use the 'X' key on an XInput joystick
/// </summary>
public class InteractAtGazeWithCrosshairOverride : MonoBehaviour
{
	[Tooltip("Time to hold the interaction button before an object gets selected. Set to 0 to respond directly on button clicks.")]
	public float ActivationTime = 0f;
	[Tooltip("An icon to show the interaction progress (cancelation time)")]
	public ProgressButton ActivationProgressIcon;
	[Tooltip("Center crosshair UI element")]
	public Image CenterCrosshairImage;
	[Tooltip("Extended View reference (needed for player forward direction information)")]
	public ExtendedViewBase ExtendedView;
	[Tooltip("Post-process effect to visualize object outline")]
	public HighlightsPostEffect HighlightsPostEffect;
	[Tooltip("Outline color for objects in the gaze focus area")]
	public Color FocusedColor = new Color(0, 1, 1, 0.5f);
	[Tooltip("Outline color for selected object")]
	public Color SelectedColor = new Color(1, 1, 0, 0.5f);

	[Tooltip("Check this option to visualize the crosshair override zone for object selection")]
	public bool DebugCrosshairOverrideZone;
	[Tooltip("Crosshair override zone circle")]
	public Texture CrosshairOverrideZoneCircle;

	private AudioSource _audio;

	public float MaxInteractionDistance = 3f;

	/** Highlight settings **/
	private float FadeDelay = 0.015f;
	private float FadeDuration = 0.1f;

	/** Crosshair override settings - using rough estimation of foveal area as override area**/
	private const int RaycastLayerMask = ~0x24;//0b100100;                  // ignore "ignore raycast" and "ui" layers
	private readonly float FovealAngle = 2.0f;                       // slight overestimate of the foveal angle
	private readonly float GazeOriginToScreenDistanceInInches = 24f; // estimate of a typical viewing distance using eye tracking
	private float _fovealRadiusInInches;                             // calculated in Start()

	/** Interaction Key Bindings **/
	private readonly KeyCode InteractionKey = KeyCode.F;
	private readonly KeyCode JoystickInteractionKey = KeyCode.JoystickButton2; // 'X' on XInput joysticks

	/** Private members **/
	private GameObject _focusedObject;
	private GameObject _previouslyFocusedObject;
	private float _focusedLastSeen;
	private float _previouslyLastSeen;
	private bool _interactOnRelease;
	private float _interactionKeyPressedTime;
	private float _interactionKeyPressedDuration;
	private bool _wasJoystickInteractionKeyPressed;
	private Camera _usedCamera;

	// ========================================================================
	//  Properties
	// ========================================================================

	public bool IsSelecting { get; private set; }

	private bool HasObjectFocusedForSelection
	{
		get { return _focusedObject != null; }
	}

	private float ScreenDpi
	{
		get { return Screen.dpi > 0 ? Screen.dpi : 100; }
	}

	private int OverrideZoneRadiusInPixels
	{
		get { return Mathf.RoundToInt(_fovealRadiusInInches * ScreenDpi); }
	}

	// ========================================================================
	//  MonoBehavior event functions
	// ========================================================================

	void Start()
	{
		_usedCamera = Camera.main;
		_audio = GetComponent<AudioSource>();
		_fovealRadiusInInches = Mathf.Tan(FovealAngle * Mathf.Deg2Rad) * GazeOriginToScreenDistanceInInches;
	}

	void Update()
	{
		bool isInteractionKeyDown = Input.GetKeyDown(InteractionKey);
		bool isInteractionKeyPressed = Input.GetKey(InteractionKey);
		bool isJoystickInteractionKeyPressed = Input.GetKey(JoystickInteractionKey);
		bool isJoystickInteractionKeyDown = isJoystickInteractionKeyPressed && !_wasJoystickInteractionKeyPressed;
		bool isAnyInteractionKeyDown = isInteractionKeyDown || isJoystickInteractionKeyDown;
		bool isAnyInteractionKeyReleased = !(isInteractionKeyPressed || isJoystickInteractionKeyPressed);
		_wasJoystickInteractionKeyPressed = isJoystickInteractionKeyPressed;

		if (HasObjectFocusedForSelection && isAnyInteractionKeyDown)
		{
			StartSelecting();
		}

		if (IsSelecting)
		{
			if (isAnyInteractionKeyReleased)
			{
				if (_interactOnRelease)
				{
					InteractWithSelectedObject();
				}

				StopSelecting();
				return;
			}

			_interactionKeyPressedDuration = Time.time - _interactionKeyPressedTime;
		}
		else
		{
			UpdateObjectFocusedForSelection();
		}

		UpdateGraphics();
	}

	void OnGUI()
	{
		if (DebugCrosshairOverrideZone)
		{
			var crosshairPosition = GetGuiPositionOfPivotPoint(CenterCrosshairImage.GetComponent<RectTransform>());
			var buttonPosition = new Vector2(crosshairPosition.x - OverrideZoneRadiusInPixels, crosshairPosition.y - OverrideZoneRadiusInPixels);
			var buttonSize = new Vector2(OverrideZoneRadiusInPixels * 2, OverrideZoneRadiusInPixels * 2);
			GUI.Button(new Rect(buttonPosition.x, buttonPosition.y, buttonSize.x, buttonSize.y), CrosshairOverrideZoneCircle);
		}
	}

	// ========================================================================
	//  Private functions
	// ========================================================================

	private void StartSelecting()
	{
		_interactOnRelease = true;
		_interactionKeyPressedTime = Time.time;
		IsSelecting = true;
	}

	private void InteractWithSelectedObject()
	{
		if (!(_interactionKeyPressedDuration > ActivationTime)) { return; }

		if (_focusedObject != null)
		{
			AudioSource.PlayClipAtPoint(_audio.clip, _focusedObject.transform.position);

			_focusedObject.GetComponent<InteractableGazeAware>().Interact();
		}
	}

	private void StopSelecting()
	{
		_focusedObject = null;
		IsSelecting = false;
		_interactionKeyPressedDuration = 0;
	}

	private void UpdateObjectFocusedForSelection()
	{
		GameObject newlyFocusedObject = null;
		if (IsPlayerLookingAtCrosshair())
		{
			newlyFocusedObject = GetObjectAtCrosshair();
		}

		if (newlyFocusedObject == null)
		{
			newlyFocusedObject = TobiiAPI.GetFocusedObject();
			if (newlyFocusedObject != null)
			{
				if (Vector3.Distance(newlyFocusedObject.transform.position, ExtendedView.CameraWithoutExtendedView.transform.position) > MaxInteractionDistance)
				{
					newlyFocusedObject = null;
				}
			}
		}

		if (!IsInteractableGazeAware(newlyFocusedObject))
		{
			newlyFocusedObject = null;
		}

		UpdatePreviouslyFocusedObject(newlyFocusedObject, _focusedObject, _focusedLastSeen);

		_focusedObject = newlyFocusedObject;
		if (_focusedObject != null)
		{
			_focusedLastSeen = Time.time;
		}
	}

	private GameObject GetObjectAtCrosshair()
	{
		GameObject objectAtCrosshair = null;
		RaycastHit hitInfo;

		if (Physics.Raycast(ExtendedView.CameraWithoutExtendedView.transform.position,
							ExtendedView.CameraWithoutExtendedView.transform.forward,
							out hitInfo, MaxInteractionDistance, RaycastLayerMask))
		{
			if (IsInteractableGazeAware(hitInfo.collider.gameObject))
			{
				objectAtCrosshair = hitInfo.collider.gameObject;
			}
		}

		return objectAtCrosshair;
	}

	private bool IsPlayerLookingAtCrosshair()
	{
		var gazePoint = TobiiAPI.GetGazePoint();
		if (!gazePoint.IsRecent()) return true;

		if (CenterCrosshairImage == null) { return false; }

		var crosshairTransform = CenterCrosshairImage.GetComponent<RectTransform>();
		if (crosshairTransform == null) { return false; }

		var crosshairGuiPosition = GetGuiPositionOfPivotPoint(crosshairTransform);
		var currentGazePoint = gazePoint.GUI;

		return Vector2.Distance(currentGazePoint, crosshairGuiPosition) < OverrideZoneRadiusInPixels;
	}

	private void UpdatePreviouslyFocusedObject(GameObject newlyFocusedObject, GameObject recentlyFocusedObject, float recentlyFocusedLastSeen)
	{
		if (recentlyFocusedObject != null &&
			!Equals(newlyFocusedObject, recentlyFocusedObject))
		{
			_previouslyFocusedObject = recentlyFocusedObject;
			_previouslyLastSeen = recentlyFocusedLastSeen;
		}
	}

	private void UpdateGraphics()
	{
		UpdateObjectsOutline();
		UpdateActivationProgressIndication();
	}

	private void UpdateObjectsOutline()
	{
		if (_interactionKeyPressedDuration > ActivationTime)
		{
			OutlineSelectedObject();
		}
		else
		{
			OutlineFocusedObject();
		}
	}

	private void UpdateActivationProgressIndication()
	{
		if (ActivationProgressIcon == null) return;

		if ((_focusedObject != null)
			&& (_interactionKeyPressedDuration > 0)
			&& (_interactionKeyPressedDuration < ActivationTime))
		{
			ActivationProgressIcon.transform.position = GetScreenCoordinates(_focusedObject, _usedCamera);
			ActivationProgressIcon.gameObject.SetActive(true);
			ActivationProgressIcon.SetProgress(Mathf.Min(_interactionKeyPressedDuration / ActivationTime, 1));
		}
		else
		{
			ActivationProgressIcon.gameObject.SetActive(false);
			ActivationProgressIcon.SetProgress(0);
		}
	}

	private void OutlineFocusedObject()
	{
		OutlineObjects(FocusedColor);
	}

	private void OutlineSelectedObject()
	{
		OutlineObjects(SelectedColor);
	}

	private void OutlineObjects(Color outlineColor)
	{
		HighlightsPostEffect.HighlightColor = outlineColor;
		HighlightsPostEffect.SetHighlightedObjects(ObjectsToHighlight(_focusedObject, _previouslyFocusedObject, _previouslyLastSeen));
	}

	private GameObjectAndOpacity[] ObjectsToHighlight(GameObject objectToHighlight, GameObject objectWithFadedHighlight, float fadedObjectLastSeen)
	{
		return new[]
		{
			new GameObjectAndOpacity(objectToHighlight, 1.0f),
			new GameObjectAndOpacity(objectWithFadedHighlight, FadedOpacity(fadedObjectLastSeen))
		};
	}

	private float FadedOpacity(float lastSeenTime)
	{
		var timeDelta = Time.time - lastSeenTime - FadeDelay;
		if (timeDelta < 0) { timeDelta = 0; }

		return Mathf.Max(0, 1 - timeDelta / FadeDuration);
	}

	// ========================================================================
	//  Static utility functions
	// ========================================================================

	private static bool IsInteractableGazeAware(GameObject currentlyFocusedObject)
	{
		return currentlyFocusedObject != null
			   && currentlyFocusedObject.GetComponent<InteractableGazeAware>() != null;
	}

	/// <summary>
	/// Returns screen coordinates of the center of provided object
	/// </summary>
	/// <param name="gObject">Game object, must not be null.</param>
	/// <param name="cam">Camera to use</param>
	private static Vector2 GetScreenCoordinates(GameObject gObject, Camera cam)
	{
		var gameObjectRigidbody = gObject.GetComponent<Rigidbody>();
		if (gameObjectRigidbody)
		{
			return cam.WorldToScreenPoint(gameObjectRigidbody.worldCenterOfMass);
		}

		var gameObjectRenderer = gObject.GetComponent<Renderer>();
		if (gameObjectRenderer)
		{
			return cam.WorldToScreenPoint(gameObjectRenderer.bounds.center);
		}

		return cam.WorldToScreenPoint(gObject.transform.position);
	}

	/// <summary>
	/// Calculates the GUI positon of a RectTransform's pivot point.
	/// </summary>
	/// <param name="rectTransform">Rect transform, must not be null.</param>
	private static Vector2 GetGuiPositionOfPivotPoint(RectTransform rectTransform)
	{
		Vector2 screenPosition = rectTransform.anchoredPosition + new Vector2(Screen.width / 2f, Screen.height / 2f);
		var guiPosition = screenPosition;
		guiPosition.y = Screen.height - 1 - screenPosition.y;
		return guiPosition;
	}

	private static bool Equals(GameObject one, GameObject other)
	{
		if (one == null && other == null)
		{
			return true;
		}

		if (one == null || other == null)
		{
			return false;
		}

		return one.GetInstanceID() == other.GetInstanceID();
	}
}