//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using Tobii.GameIntegration.Net;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Provider of gaze point data. When the provider has been started it
	/// will continuously update the Last property with the latest gaze point 
	/// value received from Tobii Engine.
	/// </summary>
	internal class GazePointDataProvider : DataProviderBase<GazePoint>
	{
		private readonly ITobiiHost _tobiiHost;

		/// <summary>
		/// Creates a new instance.
		/// Note: don't create instances of this class directly. Use the <see cref="TobiiHost.GetGazePointDataProvider"/> method instead.
		/// </summary>
		/// <param name="tobiiHost">Eye Tracking Host.</param>
		public GazePointDataProvider(ITobiiHost tobiiHost)
		{
			_tobiiHost = tobiiHost;
			Last = GazePoint.Invalid;
		}

		protected override void UpdateData()
		{
			var gazePoints = TobiiGameIntegrationApi.GetGazePoints();
			foreach (var gazePoint in gazePoints)
			{
				OnGazePoint(gazePoint);
			}
		}

		private void OnGazePoint(Tobii.GameIntegration.Net.GazePoint gazePoint)
		{
			long eyetrackerCurrentUs = gazePoint.TimeStampMicroSeconds; // TODO awaiting new API from tgi;
			float timeStampUnityUnscaled = Time.unscaledTime - ((eyetrackerCurrentUs - gazePoint.TimeStampMicroSeconds) / 1000000f);

			var bounds = _tobiiHost.GameViewInfo.NormalizedClientAreaBounds;

			if (float.IsNaN(bounds.x)
				|| float.IsNaN(bounds.y)
				|| float.IsNaN(bounds.width)
				|| float.IsNaN(bounds.height)
				|| bounds.width < float.Epsilon
				|| bounds.height < float.Epsilon)
				return;

			var x = (0.5f + gazePoint.X * 0.5f - bounds.x) / bounds.width;
			var y = (0.5f + gazePoint.Y * 0.5f - (1 - bounds.height - bounds.y)) / bounds.height;
			Last = new GazePoint(new Vector2(x, y), timeStampUnityUnscaled, gazePoint.TimeStampMicroSeconds);
		}
	}
}
#endif
