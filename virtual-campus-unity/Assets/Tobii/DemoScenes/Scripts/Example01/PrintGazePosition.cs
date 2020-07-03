//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using Tobii.Gaming;

/// <summary>
/// Writes the position of the eye gaze point into a UI Text view
/// </summary>
/// <remarks>
/// Referenced by the Data View in the Eye Tracking Data example scene.
/// </remarks>
public class PrintGazePosition : MonoBehaviour
{
	public Text xCoord;
	public Text yCoord;
	public GameObject GazePoint;

	private float _pauseTimer;
	private Outline _xOutline;
	private Outline _yOutline;

	void Start()
	{
		_xOutline = xCoord.GetComponent<Outline>();
		_yOutline = yCoord.GetComponent<Outline>();
	}

	void Update()
	{
		if (_pauseTimer > 0)
		{
			_pauseTimer -= Time.deltaTime;
			return;
		}

		GazePoint.SetActive(false);
		_xOutline.enabled = false;
		_yOutline.enabled = false;

		GazePoint gazePoint = TobiiAPI.GetGazePoint();
		if (gazePoint.IsValid)
		{
			Vector2 gazePosition = gazePoint.Screen;
			yCoord.color = xCoord.color = Color.white;
			Vector2 roundedSampleInput = new Vector2(Mathf.RoundToInt(gazePosition.x), Mathf.RoundToInt(gazePosition.y));
			xCoord.text = "x (in px): " + roundedSampleInput.x;
			yCoord.text = "y (in px): " + roundedSampleInput.y;
		}

		if (Input.GetKeyDown(KeyCode.Space) && gazePoint.IsRecent())
		{
			_pauseTimer = 3f;
			GazePoint.transform.localPosition = (gazePoint.Screen - new Vector2(Screen.width, Screen.height) / 2f) / GetComponentInParent<Canvas>().scaleFactor;
			yCoord.color = xCoord.color = new Color(0 / 255f, 190 / 255f, 255 / 255f);
			GazePoint.SetActive(true);
			_xOutline.enabled = true;
			_yOutline.enabled = true;
		}
	}
}
