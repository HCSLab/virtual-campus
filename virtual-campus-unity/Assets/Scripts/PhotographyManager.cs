using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityTemplateProjects;

public class PhotographyManager : MonoBehaviour
{
	[Header("Photography Settings")]
	public Camera photographyCamera;
	public Camera mainCamera;
	public float maxPositionError, maxAngleError;
	public AudioClip shutterSFX;
	public AudioSource audioSourceForSFX;
	public GameObject missionCompleteHint;

	[Header("Camera Controll Settings")]
	[Tooltip("Time it takes to interpolate camera position 99% of the way to the target."), Range(0.001f, 1f)]
	public float positionLerpTime = 0.2f;

	[Tooltip("X = Change in mouse position.\nY = Multiplicative factor for camera rotation.")]
	public AnimationCurve mouseSensitivityCurve = new AnimationCurve(new Keyframe(0f, 0.5f, 0f, 5f), new Keyframe(1f, 2.5f, 0f, 0f));

	[Tooltip("Time it takes to interpolate camera rotation 99% of the way to the target."), Range(0.001f, 1f)]
	public float rotationLerpTime = 0.01f;

	[Tooltip("Whether or not to invert our Y axis for mouse input to rotation.")]
	public bool invertY = false;


	bool isTakingPhoto = false;
	ScriptedFirstPersonAIO playerFirstPersonAIO;
	PlayerController playerController;
	float cameraInitialY;

	SimpleCameraController.CameraState m_TargetCameraState =
		new SimpleCameraController.CameraState();
	SimpleCameraController.CameraState m_InterpolatingCameraState =
		new SimpleCameraController.CameraState();

	int photoIndex = 0;

	private void Start()
	{
		var player = GameObject.FindGameObjectWithTag("Player");
		playerFirstPersonAIO = player.GetComponent<ScriptedFirstPersonAIO>();
		playerController = player.GetComponent<PlayerController>();

		cameraInitialY = photographyCamera.transform.localPosition.y;

		StartCoroutine(AddExistingPhotos());
	}

	IEnumerator AddExistingPhotos()
	{
		// Wait one or two frames to ensure that
		// the photo bag is initialized.
		yield return null;
		yield return null;
		while(Capture.DoesScreenshotExist(photoIndex))
		{
			PhotoPanel.Instance.Add(Capture.GetScreenShot_Sprite(photoIndex));
			photoIndex++;
		}
	}

	private void Update()
	{
		if (isTakingPhoto)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				StopTakingPhoto();
				return;
			}

			UpdateCamera();

			if (Input.GetKeyDown(KeyCode.Space))
				StartCoroutine(TakePhotoCoroutine());
		}
	}

	void UpdateCamera()
	{
		// Hide and lock cursor when right mouse button pressed
		if (Input.GetMouseButtonDown(1))
		{
			Cursor.lockState = CursorLockMode.Locked;

			// To prevent the screen splashing,
			// stop updating at the initial frame
			// when the right button is pressed
			return;
		}

		// Unlock and show cursor when right mouse button released
		if (Input.GetMouseButtonUp(1))
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		// Rotation
		if (Input.GetMouseButton(1))
		{
			var mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y") * (invertY ? 1 : -1));

			var mouseSensitivityFactor = mouseSensitivityCurve.Evaluate(mouseMovement.magnitude);

			m_TargetCameraState.yaw += mouseMovement.x * mouseSensitivityFactor;
			m_TargetCameraState.pitch += mouseMovement.y * mouseSensitivityFactor;
		}

		// Framerate-independent interpolation
		// Calculate the lerp amount, such that we get 99% of the way to our target in the specified time
		var positionLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / positionLerpTime) * Time.deltaTime);
		var rotationLerpPct = 1f - Mathf.Exp((Mathf.Log(1f - 0.99f) / rotationLerpTime) * Time.deltaTime);
		m_InterpolatingCameraState.LerpTowards(m_TargetCameraState, positionLerpPct, rotationLerpPct);

		m_InterpolatingCameraState.UpdateTransform(photographyCamera.transform);

		// Keep the camera at the top of the player precisely
		photographyCamera.transform.localPosition = Vector3.up * cameraInitialY;
	}

	IEnumerator TakePhotoCoroutine()
	{
		// Disable camera update
		isTakingPhoto = false;

		// Recover the cursor
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;

		UIManager.Instance.photographyHint.SetActive(false);

		yield return null;

		var filePath = Capture.TakeScreenShot(photoIndex++);

		yield return null;

		LeanTween.color(
			UIManager.Instance.splashWhenTakingPhoto,
			Color.white,
			0.2f
			)
			.setEase(LeanTweenType.easeOutQuad)
			.setLoopPingPong(1)
			.setOnComplete(
				() => { UIManager.Instance.photographyHint.SetActive(true); }
			);

		audioSourceForSFX.Stop();
		audioSourceForSFX.clip = shutterSFX;
		audioSourceForSFX.Play();

		while (!File.Exists(filePath))
			yield return null;

		PhotoPanel.Instance.Add(Capture.GetScreenShot_Sprite(photoIndex - 1));

		// Check is target photo taken in storys
		var taskTargets = GameObject.FindGameObjectsWithTag("PhotoTarget");
		int undoneTargetCount = 0;
		bool isSuccess = false;
		foreach (var tar in taskTargets)
		{
			if (!FlagBag.Instance.HasFlag("photo_" + tar.name))
			{
				undoneTargetCount++;
				if (IsTargetPhotoTaken(tar.transform))
				{
					FlagBag.Instance.AddFlag("photo_" + tar.name);
					isSuccess = true;
				}
			}
		}

		// Show hint box when success
		missionCompleteHint.SetActive(undoneTargetCount > 0 && isSuccess);

		isTakingPhoto = true;
	}

	public void StartTakingPhoto()
	{
		playerController.isTakingPhoto = true;

		// Set the flag
		isTakingPhoto = true;

		// Update the player
		playerFirstPersonAIO.playerCanMove = false;

		// Disable all UI
		UIManager.Instance.hudCanvas.SetActive(false);
		UIManager.Instance.photographyCanvas.SetActive(true);

		// Initialize the camera
		mainCamera.enabled = false;
		photographyCamera.gameObject.SetActive(true);
		m_TargetCameraState.SetFromTransform(photographyCamera.transform);
		m_InterpolatingCameraState.SetFromTransform(photographyCamera.transform);
	}

	public void StopTakingPhoto()
	{
		LeanTween.delayedCall(0.1f, () => { playerController.isTakingPhoto = false; });

		// Set the flag
		isTakingPhoto = false;

		// Update the player
		playerFirstPersonAIO.playerCanMove = true;

		// UI
		UIManager.Instance.hudCanvas.SetActive(true);
		UIManager.Instance.photographyCanvas.SetActive(false);
		missionCompleteHint.SetActive(false);

		// Initialize the camera
		mainCamera.enabled = true;
		photographyCamera.gameObject.SetActive(false);
	}

	public bool IsTargetPhotoTaken(Transform target)
	{
		bool posCheck = (photographyCamera.transform.position - target.position).magnitude < maxPositionError;
		bool angCheck = Vector3.Dot(photographyCamera.transform.forward, target.forward) > Mathf.Cos(Mathf.Deg2Rad * maxAngleError);

		return posCheck && angCheck;
	}
}
