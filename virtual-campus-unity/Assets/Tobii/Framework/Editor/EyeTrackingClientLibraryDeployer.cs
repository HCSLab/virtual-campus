//-----------------------------------------------------------------------
// Copyright 2014 Tobii Technology AB. All rights reserved.
//-----------------------------------------------------------------------
 
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Callbacks;
 
/// <summary>
/// Editor class for deployment of the Tobii client library
/// to where they need to be.
/// </summary>
[InitializeOnLoad]
public class EyeTrackingClientLibraryDeployer
{
    private const string ClientLibraryFileName32 = "tobii_gameintegration_x86.dll";
	private const string ClientLibraryFileName64 = "tobii_gameintegration_x64.dll";
	private static readonly string Source32BitDirectory  = "Assets" + Path.DirectorySeparatorChar + "Tobii" + Path.DirectorySeparatorChar + "Plugins";
    private static readonly string Source64BitDirectory  = "Assets" + Path.DirectorySeparatorChar + "Tobii" + Path.DirectorySeparatorChar + "Plugins";

	/// <summary>
	/// When loading the editor, copy the correct version of Tobii.GameIntegration.dll
	/// library to the project root folder to be able to run in the editor.
	/// </summary>
	static EyeTrackingClientLibraryDeployer()
    {
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64
            || EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ClientLibraryFileName64)))
            {
                Debug.Log("Initialize: Copying tobii_gameintegration_x64.dll");
                Copy64BitClientLibrary(Path.Combine(Directory.GetCurrentDirectory(), ClientLibraryFileName64));
            }

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), ClientLibraryFileName32)))
            {
                Debug.Log("Initialize: Copying tobii_gameintegration_x86.dll");
                Copy32BitClientLibrary(Path.Combine(Directory.GetCurrentDirectory(), ClientLibraryFileName32));
            }
        }

        if (IsActivePlatformLinuxOrOsx())
        {
            Debug.LogWarning("Tobii Unity SDK can only provide eye-gaze data on the Windows platform. Change platform in build settings to make the Eye Tracking features work.");
        }
        else if (!(EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows ||
              EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64))
        {
            Debug.LogWarning("Tobii Unity SDK only supports building for Standalone build targets. Eye-gaze data is only available on the Windows platform. Change platform in build settings to make the EyeTracking features work.");
        }                          
    }

	/// <summary>
	/// After a build, copy the correct Tobii.GameIntegration.dll library to the output directory.
	/// </summary>
	[PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
	{
		if (target == BuildTarget.StandaloneWindows)
		{
			Debug.Log("Build: Copying x86 native library.");
			Copy32BitClientLibrary(Path.Combine(Path.GetDirectoryName(pathToBuiltProject), ClientLibraryFileName32));
		}
		else if (target == BuildTarget.StandaloneWindows64)
		{
			Debug.Log("Build: Copying x64 native library.");
			Copy64BitClientLibrary(Path.Combine(Path.GetDirectoryName(pathToBuiltProject), ClientLibraryFileName64));
		}
		else
		{
			Debug.LogWarning("Tobii Unity SDK is only compatible with Windows Standalone builds.");
		}
	}

	private static void Copy32BitClientLibrary(string targetClientDllPath)
    {
        File.Copy(Path.Combine(Source32BitDirectory, ClientLibraryFileName32), targetClientDllPath, true);
    }
 
    private static void Copy64BitClientLibrary(string targetClientDllPath)
    {
        File.Copy(Path.Combine(Source64BitDirectory, ClientLibraryFileName64), targetClientDllPath, true);
    }

    private static bool IsActivePlatformLinuxOrOsx()
    {
        return IsActivePlatformLinux() || IsActivePlatformOsx();
    }

    private static bool IsActivePlatformLinux()
    {
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux ||
               EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinux64 ||
               EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneLinuxUniversal;
    }

    private static bool IsActivePlatformOsx()
    {
#if UNITY_2017_3_OR_NEWER
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSX;
#else
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel ||
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXIntel64 ||
            EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneOSXUniversal;
#endif
    }
}