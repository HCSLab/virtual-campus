//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Tobii.Gaming;
using Random = UnityEngine.Random;

public class RobotFaceControl : MonoBehaviour
{
	public enum EmotionStates
	{
		Happy,
		Neutral,
		Sad,
		Angry,
		Dollar,
		None
	}

	public EmotionStates _emotion = EmotionStates.Neutral;

	public Sprite eyeShape_Happy;
	public Sprite eyeShape_Neutral;
	public Sprite eyeShape_Sad;
	public Sprite eyeShape_Angry;
	public Sprite eyeShape_Dollar;
	public Image[] eyeImageRenderer = new Image[2];
	public Transform headAnchor;
	public GazeAware GazeAware;

	public Transform focusPointNormal;
	public Transform focusPointGazeAware;

	public float InteractionDistance = 10f;

	private Animator _animator;
	private bool _isInFocus;
	public float AnimatorDelayTime = 0.25f;
	private Quaternion _snapPosition;
	private Coroutine _rotateCoroutine;

	private AudioSource _audio;
	private bool _gazeFocus;

	void Start()
	{
		_animator = gameObject.GetComponent<Animator>();
		_audio = GetComponent<AudioSource>();

		headAnchor.transform.LookAt(focusPointNormal);

		UpdateFace();
	}

	private void OnGazeChanged(bool gazeOnTarget)
	{
		_isInFocus = gazeOnTarget;
		if (!_isInFocus)
		{
			if (_rotateCoroutine != null)
			{
				StopCoroutine(_rotateCoroutine);
				_rotateCoroutine = null;
			}
			_rotateCoroutine = StartCoroutine(RotateBack());
		}
		else
		{
			if (_rotateCoroutine != null)
			{
				StopCoroutine(_rotateCoroutine);
				_rotateCoroutine = null;
			}
			if (_animator.enabled)
			{
				_snapPosition = headAnchor.localRotation;
			}
			_rotateCoroutine = StartCoroutine(Rotate());
		}

		_animator.enabled = false;
	}

	void Update()
	{
		if (GazeAware.HasGazeFocus)
		{
			if (Input.GetKeyDown(KeyCode.F))
			{
				OnClick();
			}
		}

		var currentFocus = GazeAware.HasGazeFocus
			&& (Vector3.Distance(transform.position, Camera.main.transform.position) < InteractionDistance);
		if (_gazeFocus != currentFocus)
		{
			_gazeFocus = currentFocus;
			OnGazeChanged(_gazeFocus);
		}

		foreach (Image img in eyeImageRenderer)
		{
			img.sprite = UpdateFace();
		}
	}

	public IEnumerator Rotate()
	{
		var elapsedTime = 0f;

		_audio.Play();

		while (elapsedTime < 1f)
		{
			var focusPoint = focusPointGazeAware != null ? focusPointGazeAware : Camera.main.transform;

			var direction = focusPoint.position - headAnchor.position;
			headAnchor.rotation = Quaternion.Lerp(headAnchor.rotation, Quaternion.LookRotation(direction), (elapsedTime / 1f));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}

	public IEnumerator RotateBack()
	{
		yield return new WaitForSeconds(AnimatorDelayTime);
		var elapsedTime = 0f;
		while (elapsedTime < 1f)
		{
			headAnchor.localRotation = Quaternion.Lerp(headAnchor.localRotation, _snapPosition, (elapsedTime / 1f));
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		_animator.enabled = true;
	}

	private void OnClick()
	{
		float random = Random.Range(0, 10);

		if (random > 4)
		{
			_emotion = EmotionStates.Angry;
		}
		else
		{
			_emotion = EmotionStates.Happy;
		}
	}

	private Sprite UpdateFace()
	{
		var selectedSprite = eyeShape_Neutral;
		switch (_emotion)
		{
			case EmotionStates.Neutral: return eyeShape_Neutral;
			case EmotionStates.Angry: return eyeShape_Angry;
			case EmotionStates.Happy: return eyeShape_Happy;
			case EmotionStates.Sad:
				for (int i = 0; i < eyeImageRenderer.Length; i++)
				{
					var eyeImage = eyeImageRenderer[i];
					eyeImage.color = new Color(129 / 255f, 129 / 255f, 129 / 255f, 255 / 255f);

					eyeImage.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);

					if (i == 1)
					{
						eyeImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0);
					}
				}
				return eyeShape_Sad;
			case EmotionStates.Dollar:
				for (int i = 0; i < eyeImageRenderer.Length; i++)
				{
					var eyeImage = eyeImageRenderer[i];
					eyeImage.color = new Color(51 / 255f, 225 / 255f, 18 / 255f, 255 / 255f);

					eyeImage.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);


					if (i == 1)
					{
						eyeImage.rectTransform.localEulerAngles = new Vector3(0, -180, 0);
					}
				}
				return eyeShape_Dollar;
		}

		return selectedSprite;
	}

	public void SetFace(EmotionStates state)
	{
		_emotion = state;
		UpdateFace();
	}
}