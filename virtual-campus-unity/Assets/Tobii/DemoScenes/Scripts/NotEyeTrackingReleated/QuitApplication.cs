//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class QuitApplication : MonoBehaviour
{
	/// <summary>
	/// Stops or quits the application.
	/// </summary>
	/// <remarks>
	/// Referenced in the Inspector by OnClick() event of the stop button.
	/// </remarks>
	public void StopApplication()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); 
#endif
	}
}
