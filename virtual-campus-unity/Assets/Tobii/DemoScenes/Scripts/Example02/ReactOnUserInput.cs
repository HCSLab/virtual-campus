//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using Tobii.Gaming;

/// <summary>
/// If the object is in focus of the user's eye-gaze when the user presses a 
/// button the object wobbles up.
/// </summary>
/// <remarks>
/// Referenced by the Target game objects in the Simple Gaze Selection example scene.
/// </remarks>
public class ReactOnUserInput : MonoBehaviour
{
	public AnimationCurve blendingCurve;

	private Vector3 _startScale;
	private GazeAware _gazeAware;
	private string _buttonName = "Fire1";
	private float _waitingTime = 0.1f;
	private float _timeSinceButtonPressed = 0;
	private bool _useBlobEffect = false;

	/// <summary>
	/// Store the start scale of the object
	/// </summary>
	void Start()
	{
		_startScale = transform.localScale;
		_gazeAware = GetComponent<GazeAware>();
	}

	/// <summary>
	/// Reset the component when it gets disabled
	/// </summary>
	void OnDisable()
	{
		_useBlobEffect = false;
		transform.localScale = _startScale;
	}

	/// <summary>
	/// Do the wobble effect and react to the users input
	/// </summary>
	void Update()
	{

		if (_gazeAware.HasGazeFocus)
		{
			if (Input.GetButtonDown(_buttonName))
			{
				_timeSinceButtonPressed = 0;
				StartCoroutine(StartScaleEffect());
			}
		}

		if (_useBlobEffect)
		{
			float scaleFactor = blendingCurve.Evaluate(_timeSinceButtonPressed / _waitingTime);
			_timeSinceButtonPressed += Time.deltaTime;
			transform.localScale = _startScale + _startScale * scaleFactor;
		}
	}

	IEnumerator StartScaleEffect()
	{
		_useBlobEffect = true;
		yield return new WaitForSeconds(_waitingTime);
		_useBlobEffect = false;
	}
}
