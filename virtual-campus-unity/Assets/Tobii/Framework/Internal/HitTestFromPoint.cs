//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// This Class contains some basic implementations of techniques for gaze selection for unity.
	/// </summary>
	internal class HitTestFromPoint
	{
		/// <summary>
		/// Performs a hit test in World space for the given point, sets the hitInfo 
		/// out parameter and returns true if an object was found, returns false otherwise.
		/// </summary>
		/// <param name="hitInfo">Out: If true is returned, hitInfo will contain more information about where the collider was hit.</param>
		/// <param name="point">Point to hit test from.</param>
		/// <param name="camera">Camera that the point is relative to.</param>
		/// <param name="distance">Optional: Maximum distance to hit test.</param>
		/// <param name="layerMask">Optional: Which layers to hit test on.</param>
		/// <returns>True if an object was found, false otherwise.</returns>
		public static bool FindObjectInWorld(out RaycastHit hitInfo, Vector2 point, Camera camera, float distance = Mathf.Infinity, int layerMask = 1)
		{
			Ray hitTestRay = camera.ScreenPointToRay(point);
			return Physics.Raycast(hitTestRay, out hitInfo, distance, layerMask);
		}

		/// <summary>
		/// Performs hit tests on all the provided points in World space. Sets
		/// the hitInfos out parameter and returns true if at least one object
		/// was hit, returns false otherwise.
		/// </summary>
		/// <param name="hitInfos">Out: If true is returned, hitInfos will contain information about where the colliders were hit.</param>
		/// <param name="points">The points to hit test.</param>
		/// <param name="camera">Camera that the point is relative to.</param>
		/// <param name="distance">Optional: Maximum distance to hit test</param>
		/// <param name="layerMask">Optional: Which layers to hit test on.</param>
		/// <returns></returns>
		public static bool FindMultipleObjectsInWorldFromMultiplePoints(out IEnumerable<RaycastHit> hitInfos, IEnumerable<Vector2> points, Camera camera, float distance = Mathf.Infinity, int layerMask = 1)
		{
			IEnumerable<Ray> rays = CreateRaysFromPoints(points, camera);
			var isAtLeastOneHit = false;
			var hitList = new List<RaycastHit>();

			foreach (Ray ray in rays)
			{
				RaycastHit localHitInfo;
				if (Physics.Raycast(ray, out localHitInfo, distance, layerMask))
				{
					isAtLeastOneHit = true;

					hitList.Add(localHitInfo);
				}
			}

			hitInfos = hitList;

			return isAtLeastOneHit;
		}

		/// <summary>
		/// Performs a hit test for the given point on the <see cref="UnityEngine.Canvas"/>,
		/// sets the hitObject out parameter and returns true if an object was found, returns 
		/// false otherwise.
		/// </summary>
		/// <param name="selectedObject">Out: the game object that was selected</param>
		/// <param name="point">The point to hit test from.</param>
		/// <param name="layerMask">Optional: Which layers to hit test on.</param>
		/// <param name="minDepth">Optional: Minimum depth to hit test.</param>
		/// <param name="maxDepth">Optional: Maximum depth to hit test.</param>
		/// <returns>True if an object was found, false otherwise.</returns>
		public static bool FindObjectOnCanvas(out GameObject selectedObject, Vector2 point, int layerMask = -1, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity)
		{
			var selectedCollider = Physics2D.OverlapPoint(point, layerMask, minDepth, maxDepth);
			if (selectedCollider)
			{
				selectedObject = selectedCollider.gameObject;
				return true;
			}

			selectedObject = null;
			return false;
		}

		/// <summary>
		/// Creates a collection of <see cref="Ray"/> objects from a collection
		/// of <see cref="Vector2"/> coordinates on <see cref="Screen"/>.
		/// </summary>
		/// <param name="points">The points to create rays from.</param>
		/// <param name="camera">The camera to use.</param>
		/// <returns></returns>
		private static IEnumerable<Ray> CreateRaysFromPoints(IEnumerable<Vector2> points, Camera camera)
		{
			var rays = new List<Ray>();

		    if (camera == null) return rays;

			foreach (var point in points)
			{
				rays.Add(camera.ScreenPointToRay(point));
			}

			return rays;
		}
	}

	public static class PatternGenerator
	{
		public static IEnumerable<Vector2> CreateCircularAreaUniformPattern(Vector2 centralPoint, int radiusInScreenPixels, int numberOfPoints)
		{
			var points = new List<Vector2>();
			for (int i = 0; i < numberOfPoints; ++i)
			{
				var point = Random.insideUnitCircle * radiusInScreenPixels + centralPoint;
				points.Add(point);
			}

			return points;
		}

		/// <summary>
		/// Creating a Circle Ray pattern arround the original Gaze Point
		/// </summary>
		/// <param name="centralPoint"></param>
		/// <param name="camera"></param>
		/// <param name="numberOfPoints"></param>
		/// <param name="patternRadius"></param>
		/// <returns></returns>
		public static Ray[] CreateCircleAroundCentralPoint(Vector2 centralPoint, Camera camera, int numberOfPoints, float patternRadius)
		{
			var positionsOfSecondRaysOnScreen = new Ray[numberOfPoints];
			var step = 0;

			for (var alpha = 0; alpha < 360; alpha += 360 / numberOfPoints)
			{
				float heading = Mathf.Deg2Rad * alpha;
				var offset = new Vector2(Mathf.Cos(heading) * patternRadius, Mathf.Sin(heading) * patternRadius);
				positionsOfSecondRaysOnScreen[step] = camera.ScreenPointToRay(centralPoint + offset);
				step++;
			}

			return positionsOfSecondRaysOnScreen;
		}
	}
}

