//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEditor;
using System.Collections.Generic;
public static class DemoScenesToolsMenu
{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
	public static readonly string[] SceneList = {
		"01_GazePointData",
		"02a_SimpleGazeSelection",
		"02b_SimpleGazeSelection",
		"02c_SimpleGazeSelection",
		"03_UserPresenceAndHeadPose",
		"04_FirstPersonExample",
		"05_ThirdPersonExample",
		"06_StrategyExample",
		"07_DynamicLightAdaptationExample"
	};

	[MenuItem("Tools/Add Tobii SDK Demo Scenes to Build")]
	static void AddDemoScenesToBuild()
	{
		EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
		HashSet<string> hashSetScenes = new HashSet<string>();
		foreach (EditorBuildSettingsScene scene in scenes)
		{
			hashSetScenes.Add(scene.path);
		}

		foreach (string sceneName in SceneList)
		{
			string scenePath = string.Format("Assets/Tobii/DemoScenes/{0}.unity", sceneName);
			if (hashSetScenes.Contains(scenePath))
				continue;

			ArrayUtility.Add(ref scenes, new EditorBuildSettingsScene(scenePath, true));
			hashSetScenes.Add(scenePath);
		}

		EditorBuildSettings.scenes = scenes;
	}
#else
	static DemoScenesToolsMenu()
	{
		UnityEngine.Debug.LogError ("Tobii SDK Demo Scenes require Unity 5.3.7f1 or higher.");
	}
#endif
}