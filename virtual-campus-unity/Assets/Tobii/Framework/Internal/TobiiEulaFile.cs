namespace Tobii.Gaming.Internal
{
#if UNITY_EDITOR
	using System.IO;
#else
    using UnityEngine;
#endif

	public static class TobiiEulaFile
	{
		private const string ResourcePath = "TobiiSDKEulaAccepted";
#if UNITY_EDITOR && UNITY_STANDALONE
		private static readonly string DirectoryPath = "Assets" + Path.DirectorySeparatorChar + "Tobii" + Path.DirectorySeparatorChar + "Resources";
		private static readonly string FilePath = DirectoryPath + Path.DirectorySeparatorChar + ResourcePath + ".json";
#endif

#if UNITY_STANDALONE
		private static bool _eulaAccepted;
#endif
		public static bool IsEulaAccepted()
		{
#if UNITY_STANDALONE
			if (_eulaAccepted) return true;

#if UNITY_EDITOR
			if (File.Exists(FilePath))
			{
				var text = File.ReadAllText(FilePath);
				_eulaAccepted = text == "{\"EulaAccepted\": \"true\"}";
			}
#else
            TextAsset text = Resources.Load<TextAsset>(ResourcePath);
			_eulaAccepted = (null != text);
#endif
			return _eulaAccepted;
#else
			return false;            
#endif
		}

#if UNITY_EDITOR
		public static void SetEulaAccepted()
		{
#if UNITY_STANDALONE
			if (Directory.Exists(DirectoryPath) == false)
			{
				Directory.CreateDirectory(DirectoryPath);
			}
			File.WriteAllText(FilePath, "{\"EulaAccepted\": \"true\"}");
#endif
		}
#endif
	}
}