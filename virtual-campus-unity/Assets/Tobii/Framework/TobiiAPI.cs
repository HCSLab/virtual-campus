//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using Tobii.Gaming.Internal;
using UnityEngine;

namespace Tobii.Gaming
{
	/// <summary>
	/// Static access point for Tobii eye tracker data.
	/// </summary>
	public static class TobiiAPI
	{
		// --------------------------------------------------------------------
		//  Public properties
		// --------------------------------------------------------------------

		/// <summary>
		/// Checks if Tobii software is installed and device is connected,
		/// configured and running.
		/// </summary>
		public static bool IsConnected
		{
			get { return Host.IsConnected; }
		}

		// --------------------------------------------------------------------
		//  Public methods
		// --------------------------------------------------------------------
	
		/// <summary>
		/// Gets the gaze point. Subsequent calls within the same frame will
		/// return the same value.
		/// <para>
		/// The first time this function is called it will return an invalid 
		/// data point.
		/// </para>
		/// </summary>
		/// <returns>The last (newest) <see cref="GazePoint"/>.</returns>
		public static GazePoint GetGazePoint()
		{
			return Host.GetGazePointDataProvider().Last;
		}

		/// <summary>
		/// Gets the head pose. Subsequent calls within the same frame will
		/// return the same value.
		/// <para>
		/// The first time this function is called it will return an invalid
		/// data point.
		/// </para>
		/// </summary>
		/// <returns>The last (newest) <see cref="HeadPose"/>.</returns>
		public static HeadPose GetHeadPose()
		{
			return Host.GetHeadPoseDataProvider().Last;
		}

		/// <summary>
		/// Get the user presence, which indicates if there is a user present 
		/// in front of the screen.
		/// </summary>
		public static UserPresence GetUserPresence()
		{
			return Host.UserPresence;
		}


		/// <summary>
		/// Gets the <see cref="FocusedObject"/> with gaze focus. Only game 
		/// objects with a <see cref="GazeAware"/> component can be focused 
		/// using gaze.
		/// </summary>
		/// <returns>The gaze-aware game object that has gaze focus, 
		/// or null if no gaze-aware object is focused.</returns>
		public static GameObject GetFocusedObject()
		{
			var focusedObject = Host.GazeFocus.FocusedObject;
			if (!focusedObject.IsValid)
			{
				return null;
			}

			return focusedObject.GameObject;
		}

		/// <summary>
		/// Sets the camera that defines the user's current view point.
		/// </summary>
		/// <param name="camera"></param>
		public static void SetCurrentUserViewPointCamera(Camera camera)
		{
			Host.GazeFocus.Camera = camera;
		}

		/// <summary>
		/// Gets all gaze points since the supplied gaze point. 
		/// Points older than 500 ms will not be included.
		/// </summary>
		public static IEnumerable<GazePoint> GetGazePointsSince(GazePoint gazePoint)
		{
			return Host.GetGazePointDataProvider().GetDataPointsSince(gazePoint);
		}

		/// <summary>
		/// Gets all head pose data points since the supplied head pose. 
		/// Data points older than 500 ms will not be included.
		/// </summary>
		public static IEnumerable<HeadPose> GetHeadPosesSince(HeadPose headPose)
		{
			return Host.GetHeadPoseDataProvider().GetDataPointsSince(headPose);
		}

		/// <summary>
		/// Gets information about the eye-tracked display monitor.
		/// </summary>
		public static DisplayInfo GetDisplayInfo()
		{
			return Host.DisplayInfo;
		}

		// --------------------------------------------------------------------
		//  Private properties and methods
		// --------------------------------------------------------------------

		private static ITobiiHost Host
		{
			get
			{
				return TobiiHost.GetInstance();
			}
		}
	}
}
