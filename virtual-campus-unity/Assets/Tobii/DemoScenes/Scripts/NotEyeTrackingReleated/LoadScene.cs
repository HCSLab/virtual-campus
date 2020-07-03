//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;

public class LoadScene : MonoBehaviour
{
	public void Load(string sceneName)
	{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
#else
		Application.LoadLevel(sceneName);
#endif
	}

	string GetCurrentScene()
	{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
#else
		return Application.loadedLevelName;
#endif

	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Load(false);
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			Load(true);
		}
	}

	public void Load(bool nextScene)
	{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
		var index = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
		if (nextScene)
		{
			index = (index + 1) % UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
		}
		else if (index > 0)
		{
			index -= 1;
		}
		else
		{
			index = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings - 1;
		}

		UnityEngine.SceneManagement.SceneManager.LoadScene(index);
#else
		var index = Application.loadedLevel;
        if (nextScene)
        {
            index = (index + 1) % Application.levelCount;
        }
        else if(index > 0)
        {
            index -= 1;
        }
        else
        {
            index = Application.levelCount - 1;
        }
        Application.LoadLevel(index);
#endif
	}

}
