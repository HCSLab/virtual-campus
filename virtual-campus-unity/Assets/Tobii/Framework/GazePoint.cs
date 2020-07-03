//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using Tobii.Gaming.Internal;
using UnityEngine;

namespace Tobii.Gaming
{
	/// <summary>
	/// Holds a gaze point with a timestamp and converts to either Screen space, Viewport, or GUI space coordinates.
	/// </summary>
	public struct GazePoint : ITimestamped
	{
		private const float MaxAge = 0.5f;

		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="unityScreenSpacePoint">Gaze point in unity screen coordinates.</param>
		/// <param name="timestamp">The timestamp when the gaze point was created in the eye tracker, in seconds <see cref="Time.unscaledTime"/>.</param>
		public GazePoint(Vector2 viewportCoordinates, float timestamp, long preciseTimestamp) : this()
		{
			Viewport = viewportCoordinates;
			Timestamp = timestamp;
			PreciseTimestamp = preciseTimestamp;
		}

		/// <summary>
		/// Gets a value representing an invalid gaze point.
		/// </summary>
		public static GazePoint Invalid
		{
			get
			{
				return new GazePoint(new Vector2(float.NaN, float.NaN), -1.0f, -1);
			}
		}

		/// <summary>
		/// Gets the gaze point in the viewport coordinate system.
		/// <para>
		/// The bottom-left of the screen/camera is (0, 0); the top-right is (1, 1).
		/// </para>
		/// </summary>
		public Vector2 Viewport { get; private set; }

		/// <summary>
		/// Gets the gaze point in (Unity) screen space pixels.
		/// <para>
		/// The bottom-left of the screen/camera is (0, 0); the right-top is (pixelWidth, pixelHeight).
		/// </para>
		/// </summary>
		public Vector2 Screen
		{
			get
			{
				var point = Viewport;
				point.x *= UnityEngine.Screen.width;
				point.y *= UnityEngine.Screen.height;
				return point;
			}
		}

		/// <summary>
		/// Gets the gaze point in GUI space pixels.
		/// <para>
		/// The top-left of the screen is (0, 0); the bottom-right is (pixelWidth, pixelHeight).
		/// </para>
		/// </summary>
		public Vector2 GUI
		{
			get
			{
				var point = Screen;
				point.y = UnityEngine.Screen.height - 1 - point.y;
				return point;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the data point is valid or not.
		/// <remarks>
		/// This indicates if the point was created with valid data. To check
		/// if a point is stale, use <see cref="Timestamp"/> instead.
		/// </remarks>
		/// </summary>
		public bool IsValid
		{
			get { return !float.IsNaN(Viewport.x) && !float.IsNaN(Viewport.y); }

		}

		/// <summary>
		/// Gets the <see cref="Time.unscaledTime"/> timestamp for the data point in seconds.
		/// <remarks>
		/// This timestamp closely corresponds to the Time.unscaledTime when the data
		/// point was created in the eye tracker. Every timestamp is unique.
		/// </remarks>
		/// </summary>
		public float Timestamp { get; private set; }

		/// <summary>
		/// Gets the precise timestamp of the data point in milliseconds.
		/// </summary>
		/// <remarks>
		/// This is the precise timestamp from the eye tracker when the data point 
		/// was created. Can be used to compare small deltas between data points,
		/// with a higher precision and without the floating point rounding error
		/// of the <see cref="ITimestamped.Timestamp"/>.
		/// </remarks>
		public long PreciseTimestamp { get; private set; }
		
		/// <summary>
		/// Checks that data point is both recent and valid.
		/// </summary>
		public bool IsRecent()
		{
			return IsRecent(MaxAge);
		}

		/// <summary>
		/// Checks that data point is valid and not older than maxAge.
		/// </summary>
		public bool IsRecent(float maxAge)
		{
			return IsValid && ((Time.unscaledTime - Timestamp) < maxAge);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return string.Format("{0} {1} {2}", Screen, Timestamp);
		}
	}
}
