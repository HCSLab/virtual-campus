//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Interface of an EyeTracking data provider.
	/// </summary>
	/// <typeparam name="T">Type of the provided data value object.</typeparam>
	internal interface IDataProvider<T> where T : ITimestamped
	{
		/// <summary>
		/// Gets the latest value of the data stream. The value is never null but 
		/// it might be invalid.
		/// </summary>
		T Last { get; }

		/// <summary>
		/// Gets all data points since the supplied data point. 
		/// Points older than 500 ms will not be included.
		/// </summary>
		IEnumerable<T> GetDataPointsSince(ITimestamped dataPoint);
	}
}
