//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.Gaming;

public class HeadMovement : MonoBehaviour
{
	public bool LeftEyeClosed { get; private set; }
	public Vector2 LeftEyePosition { get; private set; }
	public bool RightEyeClosed { get; private set; }
	public Vector2 RightEyePosition { get; private set; }

	public Transform Head;
	public float Responsiveness = 10f;

	void Update()
	{
		var headPose = TobiiAPI.GetHeadPose();
		if (headPose.IsRecent())
		{
			Head.transform.localRotation = Quaternion.Lerp(Head.transform.localRotation, headPose.Rotation, Time.unscaledDeltaTime * Responsiveness);
		}

		var gazePoint = TobiiAPI.GetGazePoint();
		if (gazePoint.IsRecent() && Camera.main != null)
		{
			var eyeRotation = Quaternion.Euler((gazePoint.Viewport.y - 0.5f) * Camera.main.fieldOfView, (gazePoint.Viewport.x - 0.5f) * Camera.main.fieldOfView * Camera.main.aspect, 0);

			var eyeLocalRotation = Quaternion.Inverse(Head.transform.localRotation) * eyeRotation;

			var pitch = eyeLocalRotation.eulerAngles.x;
			if (pitch > 180) pitch -= 360;
			var yaw = eyeLocalRotation.eulerAngles.y;
			if (yaw > 180) yaw -= 360;

			LeftEyePosition = new Vector2(Mathf.Sin(yaw * Mathf.Deg2Rad), Mathf.Sin(pitch * Mathf.Deg2Rad));
			RightEyePosition = new Vector2(Mathf.Sin(yaw * Mathf.Deg2Rad), Mathf.Sin(pitch * Mathf.Deg2Rad));
		}

		LeftEyeClosed = RightEyeClosed = TobiiAPI.GetUserPresence().IsUserPresent() && (Time.unscaledTime - gazePoint.Timestamp) > 0.15f || !gazePoint.IsRecent();
	}
}