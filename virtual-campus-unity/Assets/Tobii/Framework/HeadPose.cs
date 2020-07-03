//-----------------------------------------------------------------------
// Copyright 2017 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using Tobii.Gaming.Internal;
using UnityEngine;

namespace Tobii.Gaming
{
	/// <summary>
	/// Holds a head pose with a timestamp.
	/// </summary>
	public struct HeadPose : ITimestamped
	{
		private const float MaxAge = 0.5f;
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="position">Head position: the x, y and z coordinate of the head of the user (in millimeters from the center of the display area)</param>
		/// <param name="rotation">Head rotation: the x, y and z rotation of the head of the user (expressed in Euler angles using the right-hand rule. The z rotation describes the rotation around the vector that points out from the screen in front of the user, towards the user)</param>
		/// <param name="timestamp">The timestamp when the head pose data point was created in the eye tracker, in seconds <see cref="Time.unscaledTime"/>.</param>
		public HeadPose(Vector3 position, Quaternion rotation, float timestamp, long preciseTimestamp) : this()
		{
			Position = position;
			Rotation = rotation;
			Timestamp = timestamp;
			PreciseTimestamp = preciseTimestamp;
		}

		/// <summary>
		/// Gets a value representing an invalid head pose data point.
		/// </summary>
		public static HeadPose Invalid
		{
			get
			{
				return new HeadPose(new Vector3(float.NaN, float.NaN, float.NaN), Quaternion.identity, -1.0f, -1);
			}
		}

		/// <summary>
		/// Head position: the x, y and z coordinate of the head of the user, in 
		/// millimeters from the center of the display area.
		/// </summary>
		public Vector3 Position { get; private set; }

		/// <summary>
		/// Head rotation: rotation of the head of the user. Expressed using Quaternion.
		/// </summary>
		public Quaternion Rotation { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the data point is valid or not.
		/// <remarks>
		/// This indicates if the point was created with valid data. To check
		/// if a point is stale, use <see cref="Timestamp"/> instead.
		/// </remarks>
		/// </summary>
		public bool IsValid { get { return !float.IsNaN(Position.x); } }

		/// <summary>
		/// Gets the <see cref="Time.time"/> timestamp for the data point in seconds.
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
	}
}
