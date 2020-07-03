//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
	internal class SingleRayCastNoScore : IScorer
	{
		private int _layerMask;

		public SingleRayCastNoScore()
		{
			MaximumDistance = GazeFocus.MaximumDistance;
			LayerMask = GazeFocus.LayerMask;
		}

		public SingleRayCastNoScore(float maximumDistance, int layerMask)
		{
			MaximumDistance = maximumDistance;
			LayerMask = layerMask;
		}

		/// <summary>
		/// Maximum distance to detect gaze focus within.
		/// </summary>
		private float MaximumDistance { get; set; }

		/// <summary>
		/// Layers to detect gaze focus on.
		/// </summary>
		private LayerMask LayerMask
		{
			get { return _layerMask; }
			set { _layerMask = value.value; }
		}

		public FocusedObject GetFocusedObject(IEnumerable<GazePoint> lastGazePoints, Camera camera)
		{
			var gazePoint = lastGazePoints.Last();
			if (!gazePoint.IsValid)
			{
				return FocusedObject.Invalid;
			}

			GameObject focusedObject = null;
			RaycastHit hitInfo;
			if (HitTestFromPoint.FindObjectInWorld(out hitInfo, gazePoint.Screen, camera, MaximumDistance, LayerMask))
			{
				if (GazeFocus.IsFocusableObject(hitInfo.collider.gameObject))
				{
					focusedObject = hitInfo.collider.gameObject;
				}
			}

			return new FocusedObject(focusedObject);
		}

		public IEnumerable<GameObject> GetObjectsInGaze(IEnumerable<GazePoint> lastGazePoints, Camera camera)
		{
			var focusedObject = GetFocusedObject(lastGazePoints, camera);
			if (!focusedObject.IsValid)
			{
				return new List<GameObject>();
			}

			return new List<GameObject> { focusedObject.GameObject };
		}

		public FocusedObject GetFocusedObject()
		{
			return FocusedObject.Invalid;
		}

		public void Reconfigure(float maximumDistance, int layerMask)
		{
			Reset();
			MaximumDistance = maximumDistance;
			LayerMask = layerMask;
		}

		public void RemoveObject(GameObject gameObject)
		{
			// no implementation
		}

		public void Reset()
		{
			// no implementation
		}
	}
}
