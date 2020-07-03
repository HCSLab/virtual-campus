//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

namespace Tobii.Gaming.Internal
{
	internal interface IGazeFocusInternal
	{
		/// <summary>
		/// Updates the gaze focus according to the latest gaze data.
		/// </summary>
		/// <remarks>
		/// Should only by called from a MonoBehaviour Update() method (and UI
		/// thread). Currently called once per frame from EyeTrackingHost.
		/// </remarks>
		void UpdateGazeFocus();
	}
}
