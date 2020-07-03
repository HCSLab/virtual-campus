//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
	internal class MultiRaycastHistoricHitScore : IScorer
	{
		private static readonly float GainGazeDwellTime = 0.04f;
		private static readonly float LoseGazeDwellTime = 0.15f;
		private static readonly float Threshold = 0.06f;

		private readonly Dictionary<int, ScoredObject> _scoredObjects = new Dictionary<int, ScoredObject>();
		private ScoredObject _focusedObject = ScoredObject.Empty();
		private int _layerMask;

		public MultiRaycastHistoricHitScore()
		{
			MaximumDistance = GazeFocus.MaximumDistance;
			LayerMask = GazeFocus.LayerMask;
		}

		public MultiRaycastHistoricHitScore(float maximumDistance, int layerMask)
		{
			MaximumDistance = maximumDistance;
			LayerMask = layerMask;
		}

		private FocusedObject FocusedGameObject
		{
			get
			{
				if (_focusedObject.Equals(ScoredObject.Empty()))
				{
					return FocusedObject.Invalid;
				}

				return new FocusedObject(_focusedObject.GameObject);
			}
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
			var gazePoints = new List<GazePoint>();
			/*Note: Do not use LINQ here - too inefficient to be called every update.*/
			foreach (var gazePoint in lastGazePoints)
			{
				if (gazePoint.IsValid)
				{
					gazePoints.Add(gazePoint);
				}
			}

			foreach (var gazePoint in gazePoints)
			{
				var objectsInGaze = FindObjectsInGaze(gazePoint.Screen, camera);
				UpdateFocusConfidenceScore(objectsInGaze);
			}

			var focusChallenger = FindFocusChallenger();

			if (focusChallenger.GetScore() > _focusedObject.GetScore() + Threshold)
			{
				_focusedObject = focusChallenger;
			}

			return FocusedGameObject;
		}

		public IEnumerable<GameObject> GetObjectsInGaze(IEnumerable<GazePoint> lastGazePoints, Camera camera)
		{
			GetFocusedObject(lastGazePoints, camera);
			var objectsInGaze = new List<GameObject>();
			/*Note: Do not use LINQ here - too inefficient to be called every update.*/
			foreach (var scoredObject in _scoredObjects)
			{
				if (scoredObject.Value.GetScore() > 0.0f)
				{
					objectsInGaze.Add(scoredObject.Value.GameObject);
				}
			}

			return objectsInGaze;
		}

		public FocusedObject GetFocusedObject()
		{
			ClearFocusedObjectIfOld();
			return FocusedGameObject;
		}

		public void Reconfigure(float maximumDistance, int layerMask)
		{
			Reset();
			MaximumDistance = maximumDistance;
			LayerMask = layerMask;
		}

		public void RemoveObject(GameObject gameObject)
		{
			_scoredObjects.Remove(gameObject.GetInstanceID());
			if (_focusedObject.GameObject.GetInstanceID() == gameObject.GetInstanceID())
			{
				_focusedObject = ScoredObject.Empty();
			}
		}

		public void Reset()
		{
			_scoredObjects.Clear();
		}

		private IEnumerable<GameObject> FindObjectsInGaze(Vector2 gazePoint, Camera camera)
		{
			var objectsInGaze = new List<GameObject>();
			float fovealAngle = 2.0f;
			float distanceGazeOriginToScreen_inches = 24f; // ~ 60 cm
			var dpi = Screen.dpi > 0 ? Screen.dpi : 100;
			float fovealRadius_inches = Mathf.Tan(fovealAngle * Mathf.Deg2Rad) * distanceGazeOriginToScreen_inches;
			int fovealRadius_pixels = Mathf.RoundToInt(fovealRadius_inches * dpi);

			var points = PatternGenerator.CreateCircularAreaUniformPattern(gazePoint, fovealRadius_pixels, 4);
			IEnumerable<RaycastHit> hitInfos;
			if (HitTestFromPoint.FindMultipleObjectsInWorldFromMultiplePoints(out hitInfos, points, camera,
				MaximumDistance, LayerMask))
			{
				foreach (var raycastHit in hitInfos)
				{
					objectsInGaze.Add(raycastHit.collider.gameObject);
				}
			}

			return objectsInGaze;
		}

		private void UpdateFocusConfidenceScore(IEnumerable<GameObject> objectsInGaze)
		{
			foreach (var objectInGaze in objectsInGaze)
			{
				var instanceId = objectInGaze.GetInstanceID();
				if (!_scoredObjects.ContainsKey(instanceId))
				{
					if (!GazeFocus.IsFocusableObject(objectInGaze))
					{
						continue;
					}

					_scoredObjects.Add(objectInGaze.GetInstanceID(), new ScoredObject(objectInGaze, GainGazeDwellTime, LoseGazeDwellTime));
				}

				ScoredObject hitObject = _scoredObjects[instanceId];
				hitObject.AddHit(Time.unscaledTime, Time.unscaledDeltaTime);
			}

			ClearFocusedObjectIfOld();
		}

		private ScoredObject FindFocusChallenger()
		{
			ScoredObject topFocusChallenger = ScoredObject.Empty();
			float topScore = 0.0f;

			foreach (var key in _scoredObjects.Keys)
			{
				ScoredObject scoredObject = _scoredObjects[key];

				var score = scoredObject.GetScore(Time.unscaledTime - LoseGazeDwellTime, Time.unscaledTime - GainGazeDwellTime);

				if (score > topScore)
				{
					topScore = score;
					topFocusChallenger = scoredObject;
				}
			}

			return topFocusChallenger;
		}

		private void ClearFocusedObjectIfOld()
		{
			if (!_focusedObject.IsRecentlyHit())
			{
				_focusedObject = ScoredObject.Empty();
			}
		}
	}
}