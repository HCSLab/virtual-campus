//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public enum InputDevice
{
	Keyboard,
	XBox
}
public class InputManager : MonoBehaviour
{
	private static InputDevice _lastUsedInputDevice = InputDevice.Keyboard;

	public static InputDevice LastUsedInputDevice
	{
		get { return _lastUsedInputDevice; }
		private set { _lastUsedInputDevice = value; }
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)
			|| Input.GetKeyDown(KeyCode.LeftControl)
			|| Input.GetKeyDown(KeyCode.W)
			|| Input.GetKeyDown(KeyCode.A)
			|| Input.GetKeyDown(KeyCode.S)
			|| Input.GetKeyDown(KeyCode.D)
			|| Input.GetKeyDown(KeyCode.Q)
			|| Input.GetKeyDown(KeyCode.F)
			|| Input.GetKeyDown(KeyCode.Mouse0)
			|| Input.GetKeyDown(KeyCode.Mouse1)
			|| Input.GetKeyDown(KeyCode.Mouse2)
			)
		{
			LastUsedInputDevice = InputDevice.Keyboard;
		}

		if (Input.GetKeyDown(KeyCode.JoystickButton0)
			|| Input.GetKeyDown(KeyCode.JoystickButton1)
			|| Input.GetKeyDown(KeyCode.JoystickButton2)
			|| Input.GetKeyDown(KeyCode.JoystickButton3)
			|| Input.GetKeyDown(KeyCode.JoystickButton8)
			|| (Input.GetAxis("LeftTrigger") > 0)
			|| (Input.GetAxis("RightTrigger") > 0)
			)
		{
			LastUsedInputDevice = InputDevice.XBox;
		}
	}
}
