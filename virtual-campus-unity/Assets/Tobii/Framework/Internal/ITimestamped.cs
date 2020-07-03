//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

namespace Tobii.Gaming.Internal
{
	internal interface ITimestamped
	{
		/// <summary>
		/// Returns a value indicating if the timestamped value is valid or not.
		/// </summary>
		bool IsValid { get; }

		/// <summary>
		/// Gets the <see cref="Time.unscaledTime"/> timestamp for the data point
		/// in seconds.
		/// <remarks>
		/// Time.unscaledTime timestamp of the data point. Every timestamp is unique.
		/// </remarks>
		/// </summary>
		float Timestamp { get; }

		/// <summary>
		/// Gets the precise timestamp of the data point in milliseconds.
		/// </summary>
		/// <remarks>
		/// This is the precise timestamp from the eye tracker when the data point 
		/// was created. Can be used to compare small deltas between data points,
		/// with a higher precision and without the floating point rounding error
		/// of the <see cref="ITimestamped.Timestamp"/>.
		/// </remarks>
		long PreciseTimestamp { get; }
	}
}
