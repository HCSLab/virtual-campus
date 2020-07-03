//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.Gaming;
using UnityEngine.UI;

/// <summary>
/// Various settings and utilities to manage calibrations and the presence of the user
/// </summary>
/// <remarks>
/// Referenced in the Eye Tracking Settings and Configuration example scene.
/// </remarks>
public class EyeTrackerSettingsMenu : MonoBehaviour
{
	public Text TextViewUserPresenceStatus;
	public Text TextViewIsUserPresent;
	public Text TextViewDeviceStatus;

	void Update()
	{
		UpdateUserPresenceView();
	}

	/// <summary>
	/// Print the User Presence status
	/// </summary>
	private void UpdateUserPresenceView()
	{
		var userPresence = TobiiAPI.GetUserPresence();
		TextViewUserPresenceStatus.text = userPresence.ToString();

		if (TobiiAPI.GetUserPresence().IsUserPresent())
		{
			TextViewIsUserPresent.text = "Yes";
		}
		else
		{
			TextViewIsUserPresent.text = "No";
		}
	}
}
