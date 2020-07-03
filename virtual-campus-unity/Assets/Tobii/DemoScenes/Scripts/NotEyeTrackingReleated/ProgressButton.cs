//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

public class ProgressButton : MonoBehaviour
{
	private Transform _xBoxButton;
	private Transform _keyboardButton;
	private Transform _currentButton;

	public void SetProgress(float progress)
	{
		if (_currentButton == null) return;

		var plane = _currentButton.Find("ProgressBar");
		plane.GetComponent<Image>().fillAmount = progress;
	}

	protected void Start()
	{
		_xBoxButton = transform.Find("ButtonXBox");
		_keyboardButton = transform.Find("ButtonKeyboard");
		_currentButton = _keyboardButton;
	}

	protected void Update()
	{
		if (InputManager.LastUsedInputDevice == InputDevice.Keyboard)
		{
			_currentButton = _keyboardButton;
			_xBoxButton.gameObject.SetActive(false);
			_keyboardButton.gameObject.SetActive(true);
		}
		else
		{
			_currentButton = _xBoxButton;
			_xBoxButton.gameObject.SetActive(true);
			_keyboardButton.gameObject.SetActive(false);
		}
	}
}