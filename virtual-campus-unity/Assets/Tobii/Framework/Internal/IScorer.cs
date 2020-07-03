//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
	internal interface IScorer
	{
		/// <summary>
		/// Updates the internal score based on recent gaze point data and
		/// returns the <see cref="GameObject"/> with gaze focus.
		/// </summary>
		/// <param name="lastGazePoints">The most recent gaze point data.</param>
		/// <param name="camera">The camera that defines the user's current view point.</param>
		/// <returns>The <see cref="GameObject"/> with gaze focus if one found, null
		/// otherwise.</returns>
		FocusedObject GetFocusedObject(IEnumerable<GazePoint> lastGazePoints, Camera camera);

		/// <summary>
		/// Updates the internal score based on recent gaze point data and
		/// returns a list of <see cref="GameObject"/> within gaze.
		/// </summary>
		/// <param name="lastGazePoints">The most recent gaze point data.</param>
		/// <param name="camera">The camera that defines the user's current view point.</param>
		/// <returns></returns>
		IEnumerable<GameObject> GetObjectsInGaze(IEnumerable<GazePoint> lastGazePoints, Camera camera);

		/// <summary>
		/// Updates the internal score with no new gaze point data and returns 
		/// the <see cref="GameObject"/> with gaze focus.
		/// </summary>
		/// <returns>The <see cref="GameObject"/> with gaze focus if one found, null
		/// otherwise.</returns>
		FocusedObject GetFocusedObject();

		/// <summary>
		/// Reconfigure the gaze focus settings used.
		/// </summary>
		/// <param name="maximumDistance">The maximum distance to detect gaze focus on.</param>
		/// <param name="layerMask">Layers to detect gaze focus on.</param>
		/// <remarks>
		/// Calling this method will clear all scoring history.
		/// </remarks>
		void Reconfigure(float maximumDistance, int layerMask);

		/// <summary>
		/// Remove <see cref="GameObject"/> that is no longer gaze focusable.
		/// </summary>
		/// <param name="gameObject">Object to remove.</param>
		void RemoveObject(GameObject gameObject);

		/// <summary>
		/// Clear all scoring history.
		/// </summary>
		void Reset();
	}
}
