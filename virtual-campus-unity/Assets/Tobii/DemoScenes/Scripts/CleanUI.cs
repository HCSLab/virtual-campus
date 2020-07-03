//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tobii.Gaming;

/*
 * CleanUI
 *
 * CleanUI helps reduce the amount of UI clutter that can quickly accumulate in a game.
 * It does this by simply fading out the UI objects if the user is not looking at them.
 * Please note that you need to regenerate using the static method if you change UI elements during runtime!
 * 
 * PLEASE NOTE:
 * Since we are essentially taking control over opacity for all child elements of this script, if you want to programmatically change the opacity for any component
 * governed by this script, you cannot do this directly, but instead add a <see cref="CleanUIOpacityOverride"/> component and change the opacity there.
 * 
 * Benefits include:
 * 1. Increases immersion since the user doesn't have to deal with as many simultaneous pieces of information unless specifically called upon
 * 2. Increases visibility, letting the user perceive details in the scene that might otherwise have been occluded either completely or to a greater degree.
 * 3. If the game setting is futuristic, it can complement the design since this is tech that will likely be integrated into any VR or AR HUD's we create in the future.
 */
public abstract class CleanUIOpacityOverride : MonoBehaviour
{
	//This will start out as the opacity you specified in your Graphic element's color, but you can change it programmatically as well.
	public float OpacityOverride { get; set; }
}

public abstract class CleanUI : MonoBehaviour
{
	protected abstract Type ElementType { get; }

	public Rect BoundsOverride;

	//For easy access and to avoid iteration. Supports runtime changes. Not thread safe.
	public static ReadOnlyCollection<CleanUI> GetCleanUIElements() { return _cleanUIList.AsReadOnly(); }
	private static List<CleanUI> _cleanUIList;
	static CleanUI()
	{
		HeightWidth = Screen.height / (float)Screen.width;
		_cleanUIList = new List<CleanUI>();
	}

	// Rects for use with BoundsOverride
	private static float HeightWidth;
	public static Rect TopLeftRect(float x = 3, float y = 3, float w = -1, float h = -1)
	{
		w = w == -1 ? x : w;
		h = h == -1 ? y : h;
		return new Rect(0, 1 - y, w * HeightWidth, h);
	}
	public static Rect TopRightRect(float x = 3, float y = 3, float w = -1, float h = -1)
	{
		w = w == -1 ? x : w;
		h = h == -1 ? y : h;
		return new Rect(1 - x * HeightWidth, 1 - y, w * HeightWidth, h);
	}
	public static Rect TopRect(float x = 3, float y = 3, float w = -1, float h = -1)
	{
		w = w == -1 ? x : w;
		h = h == -1 ? y : h;
		return new Rect(0.5f - (x * 0.5f * HeightWidth), 1 - y, w * HeightWidth, h);
	}
	public static Rect BottomLeftRect(float x = 3, float y = 3, float w = -1, float h = -1)
	{
		w = w == -1 ? x : w;
		h = h == -1 ? y : h;
		return new Rect(0, 0, w * HeightWidth, h);
	}
	public static Rect BottomRightRect(float x = 3, float y = 3, float w = -1, float h = -1)
	{
		w = w == -1 ? x : w;
		h = h == -1 ? y : h;
		return new Rect(1 - x * HeightWidth, 0, w * HeightWidth, h);
	}
	public static Rect BottomRect(float x = 3, float y = 3, float w = -1, float h = -1)
	{
		w = w == -1 ? x : w;
		h = h == -1 ? y : h;
		return new Rect(0.5f - (x * 0.5f * HeightWidth), 0, w * HeightWidth, h);
	}

	public static void AddCleanUI<T>(ref T var, Component inst, Rect? rect = null) where T : CleanUI
	{
		if (var == null && inst != null)
		{
			var = inst.gameObject.AddComponent<T>();
			if (rect.HasValue)
			{
				var.BoundsOverride = rect.Value;
			}
		}
	}

	/// <summary>
	/// If you add UI elements in runtime, call this to regenerate our stuff!
	/// </summary>
	public static void RegenerateCleanUIElements()
	{
		foreach (var cleanUIElement in _cleanUIList)
		{
			cleanUIElement.RegenerateElements();
		}
	}

	public bool HasFocus { get; set; }
	public Rect CurrentBounds { get; set; }

	//Deadzones are useful if we have other systems that want to know if we are looking at this object or not. One such example is infinitescreen, where we might want to pause its effect while the user is looking at certain objects. This helps reduce unintentional camera movements.
	public bool IsInfiniteScreenDeadZone = true;
	//If you don't want a certain UI element to fade out when not looked at, but still want to affect for example infinite screen movement, we support turning off the fade.
	public bool FadesOnGaze = true;
	//If you have children that you want to affect with this script!
	public bool AffectChildren = true;
	//The UI element we want to fade out
	public List<Component> UIElements;
	//The fade out/in will proceed according to this curve. We usually recommend sigmoids for everything, but it can still be useful to control this per element.
	public AnimationCurve OpacityCurve = AnimationCurve.EaseInOut(0.0f, 0.2f, 1.0f, 1.0f);
	//We need to increase the size of the objects so that glancing gaze points and instances with suboptimal accuracy still produce desired results.
	public float AdditionalEyetrackingMarginScale = 0.3f;
	//It is very important to have fast fade in time. Otherwise it will not feel responsive
	public float FadeInTimeSecs = 0.2f;
	//Conversely, you want to have a long fade out time since it will then be less likely to induce eye saccades due to peripheral movement.
	public float FadeOutTimeSecs = 1.0f;
	//Allow disabling
	public bool IsEnabled = true;

	private float _currentOpacityInput;

	protected void Start()
	{
		Init();

		UIElements = new List<Component>();
		RegenerateElements();

		//Make sure that we can get fast easy access to this instance even at runtime without having to pay iteration costs
		_cleanUIList.Add(this);
	}

	protected virtual void Init() { }

	protected void OnDestroy()
	{
		_cleanUIList.Remove(this);
	}

	protected void Update()
	{
		GazePoint currentGazePoint = TobiiAPI.GetGazePoint();
		if (!currentGazePoint.IsRecent())
		{
			return;
		}

		if (UIElements.Count == 0)
		{
			HasFocus = false;
			return;
		}

		UpdateFocus(currentGazePoint);

		if (!IsEnabled || HasFocus || !FadesOnGaze)
		{
			_currentOpacityInput = Mathf.Clamp01(_currentOpacityInput + Time.unscaledDeltaTime * (1.0f / FadeInTimeSecs));
		}
		else
		{
			_currentOpacityInput = Mathf.Clamp01(_currentOpacityInput - Time.unscaledDeltaTime * (1.0f / FadeOutTimeSecs));
		}

		//Now we need to update the opacity of all of our registered children
		var currentOpacity = OpacityCurve.Evaluate(_currentOpacityInput);
		for (var index = 0; index < UIElements.Count; ++index)
		{
			var element = UIElements[index];
			if (element == null) //If it has been deleted, we need to remove our reference to allow GC. Unity overloads == to allow us to test like this
			{
				UIElements.RemoveAt(index);
				--index;
			}

			//If we have an override command component, we need to consider it.
			var maxOpacity = 1.0f;
			try
			{
				var opacityOverride = element.GetComponent<CleanUIOpacityOverride>();
				if (opacityOverride != null)
				{
					maxOpacity = opacityOverride.OpacityOverride;
				}

				SetElementOpacity(element, currentOpacity * maxOpacity);
			}
			catch (Exception e)
			{
				Debug.Log("CleanUI: " + e.Message);
			}
		}
	}

	protected abstract void SetElementOpacity(Component element, float opacity);

	//When you regenerate elements through the static method, this will get called
	private void RegenerateElements()
	{
		UIElements.Clear();
		UIElements.AddRange(GetComponents(ElementType));

		//If we want children, we need to recursively walk the hierarchy one step at a time and look for candidates
		if (AffectChildren)
		{
			//We cannot send our own transform since we are a CleanUI script, so we would get passed over
			for (var index = 0; index < transform.childCount; ++index)
			{
				AddGraphicsFromTransform(transform.GetChild(index));
			}
		}
	}

	private void AddGraphicsFromTransform(Transform childTransform)
	{
		if (childTransform.GetComponent<CleanUI>() != null)
		{
			return; //If the child has its own CleanUI element, we don't want to mess with it
		}

		UIElements.AddRange(childTransform.GetComponents(ElementType));

		//Now walk our children
		for (var index = 0; index < childTransform.childCount; ++index)
		{
			AddGraphicsFromTransform(childTransform.GetChild(index));
		}
	}

	//This method will see if the user is looking at any part of the screenspace owned by this component
	private void UpdateFocus(GazePoint currentGazePoint)
	{
		if (BoundsOverride != new Rect())
		{
			CurrentBounds = BoundsOverride;
		}
		else
		{
			CurrentBounds = GetViewportBounds();
		}
		HasFocus = CurrentBounds.Contains(currentGazePoint.Viewport);
	}

	protected abstract Rect GetViewportBounds();

	internal static void AddCleanUI<T>(ref object _hudLeftStatBarsCleanUI, Transform transform, Rect topLeftRect)
	{
		throw new NotImplementedException();
	}
}