//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.Gaming;

/*
 * Mark At Gaze
 *
 */

public class MarkAtGaze : MonoBehaviour
{
	public Transform MarkIcon;

	public float MaxDistance = 40;
	public float DeactivationTime = 1f;

	private GazeAware _gazeAware;
	private float _timeActivated;
	private AudioSource _audio;

	protected void Start()
	{
		_gazeAware = GetComponent<GazeAware>();
		_audio = GetComponent<AudioSource>();
		_timeActivated = -1000f;
		MarkIcon.localScale = Vector3.one * 0.1f;
		MarkIcon.gameObject.SetActive(false);
	}

	protected void Update()
	{
		if (_gazeAware.HasGazeFocus && Vector3.Distance(transform.position, Camera.main.transform.position) < MaxDistance)
		{
			_timeActivated = Time.unscaledTime;
		}

		if (Time.unscaledTime - _timeActivated > DeactivationTime)
		{
			// Remove icon
			if (MarkIcon.localScale.x > 0.1f)
			{
				MarkIcon.gameObject.SetActive(true);
				MarkIcon.localScale = Vector3.Lerp(MarkIcon.localScale, Vector3.zero, Time.unscaledDeltaTime * 3);
			}
			else
			{
				MarkIcon.localScale = Vector3.one * 0.1f;
				MarkIcon.gameObject.SetActive(false);
			}
		}
		else
		{
			// Show icon
			if (!MarkIcon.gameObject.activeInHierarchy)
			{
				_audio.Play();
			}

			MarkIcon.gameObject.SetActive(true);
			MarkIcon.localScale = Vector3.Lerp(MarkIcon.localScale, Vector3.one, Time.unscaledDeltaTime * 3);
			if (MarkIcon.localScale.x > 1)
			{
				MarkIcon.localScale = Vector3.one;
			}
		}
	}
}