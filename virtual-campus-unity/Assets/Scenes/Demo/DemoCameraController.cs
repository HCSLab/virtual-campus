using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoCameraController : MonoBehaviour
{
	class CameraState
	{
		public float yaw;
		public float pitch;
		public float roll;
		public float x;
		public float y;
		public float z;

		public void SetFromTransform(Transform t)
		{
			pitch = t.eulerAngles.x;
			yaw = t.eulerAngles.y;
			roll = t.eulerAngles.z;
			x = t.position.x;
			y = t.position.y;
			z = t.position.z;
		}

		public void LerpTowards(CameraState target, float positionLerpPct, float rotationLerpPct)
		{
			yaw = Mathf.Lerp(yaw, target.yaw, rotationLerpPct);
			pitch = Mathf.Lerp(pitch, target.pitch, rotationLerpPct);
			roll = Mathf.Lerp(roll, target.roll, rotationLerpPct);

			x = Mathf.Lerp(x, target.x, positionLerpPct);
			y = Mathf.Lerp(y, target.y, positionLerpPct);
			z = Mathf.Lerp(z, target.z, positionLerpPct);
		}

		public void UpdateTransform(Transform t)
		{
			t.eulerAngles = new Vector3(pitch, yaw, roll);
			t.position = new Vector3(x, y, z);
		}
	}

	CameraState m_TargetCameraState = new CameraState();
	CameraState m_InterpolatingCameraState = new CameraState();
	CinemachineBrain brain;
	GameObject currentVirtualCamera;

	[Header("Cinemachine")]
	public float cutTime;
	public CinemachineVirtualCamera[] virtualCameras;
	public GameObject descriptionPanel;
	public float maxFOV, minFOV, zoomSpeed;

	[Header("Movement Settings")]
	[Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
	public float positionLerpTime = 0.2f;

	[Header("Rotation Settings")]
	[Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
	public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

	[Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
	public float rotationLerpTime = 0.01f;

	private void Start()
	{
		brain = GetComponent<CinemachineBrain>();
		StartCoroutine(ChangeVirtualCamera());
	}

	private void Update()
	{
		if (!currentVirtualCamera)
			return;

		// Exit Sample
		if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}

		// Rotation
		var mouseMovement = GetCameraRotation();
		var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

		m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
		m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;

		// Framerate-independent interpolation
		// Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
		var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
		var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
		m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

		m_InterpolatingCameraState.UpdateTransform(currentVirtualCamera.transform);

		var cam = currentVirtualCamera.GetComponent<CinemachineVirtualCamera>();
		cam.m_Lens.FieldOfView += GetZoomInput() * zoomSpeed * Time.deltaTime;
		cam.m_Lens.FieldOfView = Mathf.Max(minFOV, cam.m_Lens.FieldOfView);
		cam.m_Lens.FieldOfView = Mathf.Min(maxFOV, cam.m_Lens.FieldOfView);
	}

	public void UpdateVirtualCameraState()
	{
		if (!(brain.ActiveVirtualCamera as CinemachineVirtualCamera))
			return;
		var newVirtualCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject;
		if (newVirtualCamera == currentVirtualCamera)
			return;
		currentVirtualCamera = newVirtualCamera;
		m_TargetCameraState.SetFromTransform(currentVirtualCamera.transform);
		m_InterpolatingCameraState.SetFromTransform(currentVirtualCamera.transform);
	}
	
	IEnumerator ChangeVirtualCamera()
	{
		int currentVirtualCameraIndex = 0;
		float initialFOVOfCurrentVirtualCamera;
		Quaternion initialRotationOfCurrentVirtualCamera;
		while (true)
		{
			initialFOVOfCurrentVirtualCamera = virtualCameras[currentVirtualCameraIndex].m_Lens.FieldOfView;
			initialRotationOfCurrentVirtualCamera = virtualCameras[currentVirtualCameraIndex].gameObject.transform.rotation;
			virtualCameras[currentVirtualCameraIndex].Priority++;
			
			yield return new WaitForSeconds(cutTime);

			while (true)
			{
				while(GetCameraRotation().magnitude > 0.1 || descriptionPanel.activeInHierarchy)
					yield return null;

				var timer = 0f;
				while(!(GetCameraRotation().magnitude > 0.1 || descriptionPanel.activeInHierarchy))
				{
					timer += Time.deltaTime;
					if (timer > 1f)
						break;
					yield return null;
				}
				if (timer > 1f)
					break;
			}

			virtualCameras[currentVirtualCameraIndex].Priority--;
			var lastTransform = virtualCameras[currentVirtualCameraIndex].gameObject.transform;
			var lastCamera = virtualCameras[currentVirtualCameraIndex];
			var lastFOV = initialFOVOfCurrentVirtualCamera;
			var lastInitialRotation = initialRotationOfCurrentVirtualCamera;
			currentVirtualCameraIndex = (currentVirtualCameraIndex + 1) % virtualCameras.Length;

			LeanTween.delayedCall(2f, () => { lastCamera.m_Lens.FieldOfView = lastFOV; lastTransform.rotation = lastInitialRotation; });
		}
	}

	public Vector2 GetCameraRotation()
	{
		return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") * -1f);
	}

	// 1 -> zoom out
	// 0 -> no change
	// -1 -> zoom in
	public int GetZoomInput()
	{
		int ret = 0;
		if (Input.GetKey(KeyCode.N))
			ret++;
		if (Input.GetKey(KeyCode.M))
			ret--;
		return ret;
	}
}
