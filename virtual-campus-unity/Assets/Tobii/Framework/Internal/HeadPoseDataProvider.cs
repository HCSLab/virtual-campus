//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using Tobii.GameIntegration;
using Tobii.GameIntegration.Net;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Tobii.Gaming.Internal
{
	/// <summary>
	/// Provider of head pose data. When the provider has been started it
	/// will continuously update the Last property with the latest gaze point 
	/// value received from Tobii Engine.
	/// </summary>
	internal class HeadPoseDataProvider : DataProviderBase<HeadPose>
	{
		/// <summary>
		/// Creates a new instance.
		/// Note: don't create instances of this class directly. Use the <see cref="TobiiHost.GetGazePointDataProvider"/> method instead.
		/// </summary>
		public HeadPoseDataProvider()
		{
			Last = HeadPose.Invalid;
		}

		protected override void UpdateData()
		{
			var headPoses = TobiiGameIntegrationApi.GetHeadPoses();
			foreach (var headPose in headPoses)
			{
				OnHeadPose(headPose);
			}
		}

		private void OnHeadPose(Tobii.GameIntegration.Net.HeadPose headPose)
		{
			long eyetrackerCurrentUs = headPose.TimeStampMicroSeconds; // TODO awaiting new API from tgi

			float timeStampUnityUnscaled = Time.unscaledTime - ((eyetrackerCurrentUs - headPose.TimeStampMicroSeconds) / 1000000f);
			var rotation = Quaternion.Euler(-headPose.Rotation.Pitch * Mathf.Rad2Deg,
				headPose.Rotation.Yaw * Mathf.Rad2Deg,
				-headPose.Rotation.Roll * Mathf.Rad2Deg);
			Last = new HeadPose(
				new Vector3(headPose.Position.X, headPose.Position.Y, headPose.Position.Z),
				rotation,
				timeStampUnityUnscaled, headPose.TimeStampMicroSeconds);
		}
	}
}
#endif
