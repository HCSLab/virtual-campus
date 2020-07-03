//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using Tobii.Gaming;

/// <summary>
/// Changes the color of the game object's material, when the the game object 
/// is in focus of the user's eye-gaze.
/// </summary>
/// <remarks>
/// Referenced by the Target game objects in the Simple Gaze Selection example scene.
/// </remarks>
[RequireComponent(typeof(GazeAware))]
[RequireComponent(typeof(MeshRenderer))]
public class ChangeColor : MonoBehaviour
{

	public Color selectionColor;

	private GazeAware _gazeAwareComponent;
	private MeshRenderer _meshRenderer;

	private Color _deselectionColor;
	private Color _lerpColor;
	private float _fadeSpeed = 0.1f;

	/// <summary>
	/// Set the lerp color
	/// </summary>
	void Start()
	{
		_gazeAwareComponent = GetComponent<GazeAware>();
		_meshRenderer = GetComponent<MeshRenderer>();
		_lerpColor = _meshRenderer.material.color;
		_deselectionColor = Color.white;
	}

	/// <summary>
	/// Lerping the color
	/// </summary>
	void Update()
	{

		if (_meshRenderer.material.color != _lerpColor)
		{
			_meshRenderer.material.color = Color.Lerp(_meshRenderer.material.color, _lerpColor, _fadeSpeed);
		}

		// Change the color of the cube
		if (_gazeAwareComponent.HasGazeFocus)
		{
			SetLerpColor(selectionColor);
		}
		else
		{
			SetLerpColor(_deselectionColor);
		}
	}

	/// <summary>
	/// Update the color, which should used for the lerping
	/// </summary>
	/// <param name="lerpColor"></param>
	public void SetLerpColor(Color lerpColor)
	{
		this._lerpColor = lerpColor;
	}
}
