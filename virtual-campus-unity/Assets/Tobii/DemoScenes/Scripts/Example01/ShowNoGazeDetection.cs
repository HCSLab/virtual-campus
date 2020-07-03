//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.Gaming;

/// <summary>
/// Enable a set of UI elements if there is no gaze detected
/// </summary>
/// <remarks>
/// Referenced by the No Gaze Tracked visualization in the Eye Tracking Data example scene.
/// </remarks>
public class ShowNoGazeDetection : MonoBehaviour
{
	public GameObject Icon;
	public GameObject Text;

	void Update()
	{
		if (!TobiiAPI.GetGazePoint().IsRecent())
		{
			ShowGraphic(true);
		}
		else
		{
			ShowGraphic(false);
		}
	}

	private void ShowGraphic(bool isVisible)
	{
		Icon.SetActive(isVisible);
		Text.SetActive(isVisible);
	}
}
