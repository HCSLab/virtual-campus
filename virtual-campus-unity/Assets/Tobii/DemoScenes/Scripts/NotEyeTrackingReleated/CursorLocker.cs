//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class CursorLocker : MonoBehaviour
{
	public bool LockCursor = true;

	protected void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			LockCursor = !LockCursor;
		}

		UpdateCursor();
	}

	private void UpdateCursor()
	{
		if (LockCursor)
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		else if (!LockCursor)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}
