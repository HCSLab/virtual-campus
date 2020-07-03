//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CloudPointVisualizer : MonoBehaviour
{
	private const float MaxVisibilityDuration = 0.35f; /* in seconds */

	private SpriteRenderer _renderer;
	private float _lastTimestamp;
	private Vector3 _newPosition = Vector3.zero;
	private float _scale = 0.1f;

	public float Scale
	{
		get { return _scale; }
		set { _scale = value; }
	}

	private float Age
	{
		get { return Time.time - _lastTimestamp; }
	}

	void Start()
	{
		_renderer = GetComponent<SpriteRenderer>();
		InitializeScale();
	}

	void Update()
	{
		MoveToNewPosition();
		FadeByAge();
	}

	/// <summary>
	/// Sets a new pending position of this sprite that it will be moved to in 
	/// the next Update() loop.
	/// </summary>
	/// <param name="timestamp"></param>
	/// <param name="position"></param>
	public void NewPosition(float timestamp, Vector3 position)
	{
		_lastTimestamp = timestamp;
		_newPosition = position;
	}

	private void InitializeScale()
	{
		transform.localScale = Vector3.one * Scale;
	}

	private void MoveToNewPosition()
	{
		transform.position = _newPosition;
	}

	private void FadeByAge()
	{
		UpdateTransparency(TransparencyByAge());
	}

	private float TransparencyByAge()
	{
		return 1.0f - Mathf.Clamp(Age / MaxVisibilityDuration, 0.0f, 1.0f);
	}

	private void UpdateTransparency(float alpha)
	{
		var color = _renderer.color;
		color.a = alpha;
		_renderer.color = color;
	}
}