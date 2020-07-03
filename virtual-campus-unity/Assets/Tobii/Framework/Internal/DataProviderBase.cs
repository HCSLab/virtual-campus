//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using System;
using UnityEngine;
using System.Collections.Generic;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Base class for data streams.
	/// </summary>
	/// <typeparam name="T">Type of the provided data value object.</typeparam>
	internal abstract class DataProviderBase<T> : IDataProvider<T> where T : ITimestamped
	{
		private readonly List<T> _lastDataPoints = new List<T>();
		private const float PruneIntervalSecs = 2.0f;
		private float _pruneLastDataPointsTimer = 0;
		private T _last;
		private int _lastUpdatedFrame;

		// --------------------------------------------------------------------
		//  Implementation of IDataProvider<T>
		// --------------------------------------------------------------------

		/// <summary>
		/// Gets or sets the latest value of the data stream. The value is never null but 
		/// it might be invalid.
		/// </summary>
		public T Last
		{
			get
			{
				return _last;
			}

			protected set
			{
				_lastDataPoints.Add(value);
				_last = value;
			}
		}

		/// <summary>
		/// Gets all data points since the supplied data point. 
		/// Points older than 500 ms will not be included.
		/// </summary>
		public IEnumerable<T> GetDataPointsSince(ITimestamped dataPoint)
		{
			var dataPointTimestamp = dataPoint.IsValid ? dataPoint.Timestamp : 0.0;

			return _lastDataPoints.FindAll(point =>
				(point.Timestamp > dataPointTimestamp) &&
				(point.Timestamp > Time.unscaledTime - 0.5f));
		}

		public void Update()
		{
			if (Time.frameCount == _lastUpdatedFrame) return;

			_lastUpdatedFrame = Time.frameCount;

			UpdateData();

			Cleanup();
		}

		// --------------------------------------------------------------------
		//  Protected and private methods
		// --------------------------------------------------------------------		

		protected abstract void UpdateData();

		protected void Cleanup()
		{
			_pruneLastDataPointsTimer += Time.unscaledDeltaTime;
			if (_pruneLastDataPointsTimer > PruneIntervalSecs)
			{
				_lastDataPoints.RemoveAll(point => point.Timestamp < Time.unscaledTime - 0.5f);
				_pruneLastDataPointsTimer = 0;
			}
		}
	}
}
#endif
