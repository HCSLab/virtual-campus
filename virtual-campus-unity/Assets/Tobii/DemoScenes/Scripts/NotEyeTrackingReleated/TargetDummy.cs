//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TargetDummy : MonoBehaviour
{
	public float FallTimeSecs = 0.1f;
	public float StayDownTime = 2.0f;
	public float StandUpTime = 1.0f;

	public float StandUpAngle = 270;
	public float FallDownAngle = 0;
	public Transform affectedObject;

	public AudioClip HitSound;

	private DateTime _lastFallDownTime;
	private DateTime _lastStandingUpTime;
	private TargetDummyState _state;
	private AudioSource _audio;

	private void Awake()
	{
		if (null == affectedObject)
			affectedObject = transform;
	}

	private void Start()
	{
		_audio = GetComponent<AudioSource>();
	}

	protected void Update()
	{
		switch (_state)
		{
			case TargetDummyState.GoingDown:
				{
					var alpha = (DateTime.UtcNow - _lastFallDownTime).TotalSeconds / FallTimeSecs;
					var euler = affectedObject.eulerAngles;

					if (alpha >= 1.0f) //We're down
					{
						euler.x = FallDownAngle;
						_state = TargetDummyState.Down;
					}
					else //Going down
					{
						euler.x = Mathf.LerpAngle(StandUpAngle, FallDownAngle, (float)alpha);
					}

					affectedObject.eulerAngles = euler;
					break;
				}

			case TargetDummyState.Down:
				if ((DateTime.UtcNow - _lastFallDownTime).TotalSeconds > StayDownTime)
				{
					_state = TargetDummyState.GoingUp;
					_lastStandingUpTime = DateTime.UtcNow;
				}
				break;

			case TargetDummyState.GoingUp:
				{
					var alpha = (DateTime.UtcNow - _lastStandingUpTime).TotalSeconds / StandUpTime;
					var euler = affectedObject.eulerAngles;

					if (alpha >= 1.0f) //We're up
					{
						euler.x = StandUpAngle;
						_state = TargetDummyState.Up;
					}
					else //Going up
					{
						euler.x = Mathf.LerpAngle(FallDownAngle, StandUpAngle, (float)alpha);
					}

					affectedObject.eulerAngles = euler;
					break;
				}

			default:
				break;
		}
	}

	public void Hit()
	{
		if (_state != TargetDummyState.Up)
		{
			return;
		}
		if (HitSound != null)
		{
			_audio.clip = HitSound;
			_audio.Play();
		}
		_state = TargetDummyState.GoingDown;
		_lastFallDownTime = DateTime.UtcNow;
	}


	private enum TargetDummyState
	{
		Up,
		GoingDown,
		Down,
		GoingUp
	}
}
