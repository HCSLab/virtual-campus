//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using Tobii.Gaming;
using UnityEngine;

public abstract class MapNavigationBase : MonoBehaviour
{
	public KeyCode NavigationKey = KeyCode.LeftShift;
	public float InitiateBungeeZoomThreshold = 0.2f;
	public float MediumZoomLevel = 3f;
	public float TargetZoomOutLevel = 4f;

	//--------------------------------------------------------------------
	// Private Properties
	//--------------------------------------------------------------------

	private bool IsZoomDirectionPositive
	{
		get { return TargetZoomOutLevel > MediumZoomLevel; }
	}

	private float KeyPressDuration
	{
		get { return UnityEngine.Input.GetKey(NavigationKey) ? Time.unscaledTime - InteractionKeyPressTime : 0f; }
	}

	private bool IsZoomingOut
	{
		get { return KeyPressDuration > InitiateBungeeZoomThreshold && !IsInMaxZoomOut; }
	}

	private float CurrentZoomLevel
	{
		get { return GetCurrentZoomLevel(); }
	}

	private bool IsPassedMaxZoomOutLevel
	{
		get
		{
			return (IsZoomDirectionPositive && (CurrentZoomLevel > TargetZoomOutLevel))
				   || (!IsZoomDirectionPositive && (CurrentZoomLevel < TargetZoomOutLevel));
		}
	}

	protected virtual bool ShouldZoomInToMediumLevel
	{
		get
		{
			return ((IsZoomDirectionPositive && (InitialZoomLevel > MediumZoomLevel))
					|| (!IsZoomDirectionPositive && (InitialZoomLevel < MediumZoomLevel)));
		}
	}

	private float InitialZoomLevel { get; set; }
	private Vector3 InitialGroundPosition { get; set; }
	private Vector3 TargetGroundPosition { get; set; }
	private float InteractionKeyPressTime { get; set; }
	private bool IsInteractionOngoing { get; set; }
	private bool IsInMaxZoomOut { get; set; }

	//--------------------------------------------------------------------
	// MonoBehaviour event functions (messages)
	//--------------------------------------------------------------------
	protected virtual void Start()
	{
	}

	protected virtual void LateUpdate()
	{
		if (!IsInteractionAllowed())
		{
			return;
		}

		/** Different kinds of navigation interaction:
			* - tap: center at gaze
			* - bungy zoom: press until full zoom out, then release and zoom at gaze
			* - cancel bungy zoom: release key before fully zoomed out
	        */
		var isNavigationKeyJustPressedDown = UnityEngine.Input.GetKeyDown(NavigationKey);
		var isNavigationKeyJustReleased = UnityEngine.Input.GetKeyUp(NavigationKey);
		var isNavigationKeyPressedAndHeld = UnityEngine.Input.GetKey(NavigationKey);

		if (isNavigationKeyJustPressedDown) /* Allow a new interaction to start while one is ongoing.*/
		{
			InitializeInteraction();
			IsInteractionOngoing = true;
		}

		if (IsInteractionOngoing)
		{
			if (isNavigationKeyJustReleased
				&& TobiiAPI.GetGazePoint().IsRecent())
			{
				TargetGroundPosition = FindGazeIntersectionWithWorld();
			}

			var reachedTargetPositionAndZoomLevel = false;
			if (isNavigationKeyPressedAndHeld)
			{
				if (IsZoomingOut)
				{
					if (IsPassedMaxZoomOutLevel)
					{
						IsInMaxZoomOut = true;
					}
					else
					{
						IsInMaxZoomOut = ZoomOut(CurrentZoomLevel, TargetZoomOutLevel);
					}
				}
			}
			else if (IsBungyZoomCancelled())
			{
				IsInMaxZoomOut = false;
				TargetGroundPosition = InitialGroundPosition;
			}
			else /** Navigation key released **/
			{
				if (IsInMaxZoomOut && ShouldZoomInToMediumLevel)
				{
					InitialZoomLevel = MediumZoomLevel;
				}

				reachedTargetPositionAndZoomLevel = MoveTo(TargetGroundPosition, InitialZoomLevel);
			}

			IsInteractionOngoing = !reachedTargetPositionAndZoomLevel;
		}
	}

	//--------------------------------------------------------------------
	// Protected functions
	//--------------------------------------------------------------------

	protected Vector3 GetCurrentGroundPositionAtScreenCenter()
	{
		return ProjectScreenPointToGroundPosition(new Vector2(Screen.width * 0.5f, Screen.height * 0.5f));
	}

	//--------------------------------------------------------------------
	// Protected abstract functions
	//--------------------------------------------------------------------

	/// <summary>
	/// Gets or calculates the current zoom level as a float value. Could be an actual
	/// height value or a percentage. Should correspond to how the value is set
	/// for the TargetZoomOutLevel property.
	/// </summary>
	/// <returns>Value representing the current zoom level.</returns>
	protected abstract float GetCurrentZoomLevel();

	/// <summary>
	/// Checks game state to evaluate if map navigation should be allowed or not.
	/// </summary>
	/// <returns>True if map navigation is allowed in this game state, false otherwise.</returns>
	protected abstract bool IsInteractionAllowed();

	/// <summary>
	/// Zooms out towards a target max zoom out level. Used for Bungy Zoom.
	/// </summary>
	/// <param name="currentZoomLevel">The value returned by CalculateCurrentZoomLevel().</param>
	/// <param name="targetZoomLevel">The value of the TargetZoomOutLevel property.</param>
	/// <returns>True if the target zoom level has been reached, false otherwise.</returns>
	protected abstract bool ZoomOut(float currentZoomLevel, float targetZoomLevel);

	/// <summary>
	/// Moves the view to a target ground position and zoom level.
	/// </summary>
	/// <param name="targetGroundPosition">Target ground position to center camera on.</param>
	/// <param name="targetZoomLevel">Target zoom level</param>
	/// <returns>True if the target camera position has been reached, false otherwise.</returns>
	protected abstract bool MoveTo(Vector3 targetGroundPosition, float targetZoomLevel);

	/// <summary>
	/// Gets the projected ground position in world coordinates from a screen point.
	/// </summary>
	/// <param name="screenPoint"></param>
	/// <returns></returns>
	protected abstract Vector3 ProjectScreenPointToGroundPosition(Vector2 screenPoint);

	//--------------------------------------------------------------------
	// Virtual functions
	//--------------------------------------------------------------------

	/// <summary>
	/// Optionally override this function to do work when a map navigation 
	/// interaction is initialized. Called from InitializeInteraction()
	/// before any other initialization work is done.
	/// </summary>
	protected virtual void DoOptionalInitializeInteractionWork()
	{
		/** empty default implementation **/
	}

	//--------------------------------------------------------------------
	// Private functions
	//--------------------------------------------------------------------

	private void InitializeInteraction()
	{
		DoOptionalInitializeInteractionWork();

		InitialZoomLevel = GetCurrentZoomLevel();
		InitialGroundPosition = GetCurrentGroundPositionAtScreenCenter();
		InteractionKeyPressTime = Time.unscaledTime;
		IsInMaxZoomOut = false;
	}

	private bool IsBungyZoomCancelled()
	{
		return IsZoomingOut
				&& !UnityEngine.Input.GetKey(NavigationKey)
				&& CurrentZoomLevel < (TargetZoomOutLevel - float.Epsilon);
	}

	private Vector3 FindGazeIntersectionWithWorld()
	{
		Vector3 target = InitialGroundPosition;
		var gazePoint = TobiiAPI.GetGazePoint();
		if (gazePoint.IsRecent())
		{
			target = ProjectScreenPointToGroundPosition(gazePoint.Screen);
		}

		return target;
	}
}