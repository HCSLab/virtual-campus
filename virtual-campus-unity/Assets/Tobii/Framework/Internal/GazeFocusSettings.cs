//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System;
using System.IO;
using SimpleJSON;
using UnityEngine;

namespace Tobii.Gaming.Internal
{
	[Serializable]
	public struct GazeFocusSettings
	{
		private const string ResourcePath = "GazeFocusSettings";
#if UNITY_EDITOR
		private static readonly string DirectoryPath = "Assets" + Path.DirectorySeparatorChar + "Tobii" + Path.DirectorySeparatorChar + "Resources";
		private static readonly string FilePath = DirectoryPath + Path.DirectorySeparatorChar + ResourcePath + ".json";
#endif
		private const int MaximumLayersInUnity = 32;

		/// <summary>
		/// Layers to detect gaze focus on.
		/// </summary>
		public int LayerMask;

		/// <summary>
		/// Maximum distance to detect gaze focus within.
		/// </summary>
		public float MaximumDistance;

		/// <summary>
		/// Gets the stored gaze focus settings.
		/// </summary>
		/// <returns></returns>
		public static GazeFocusSettings Get()
		{
#if UNITY_EDITOR
			if (!IsInitialized())
			{
				Initialize();
			}

#if UNITY_STANDALONE
			string settingsAsJson = File.ReadAllText(FilePath);
			return GetSettingsFromObject(JSON.Parse(settingsAsJson));
#else
			return CreateDefault();
#endif

#else
            TextAsset settings = Resources.Load<TextAsset>(ResourcePath);

            if (null == settings)
            {
                return CreateDefault();
            }

	        return GetSettingsFromObject(SimpleJSON.JSON.Parse(settings.text));
#endif
		}

		private static GazeFocusSettings GetSettingsFromObject(SimpleJSON.JSONNode json)
		{
			var gfs = new GazeFocusSettings();
			gfs.LayerMask = json["LayerMask"].AsInt;
			gfs.MaximumDistance = json["MaximumDistance"].AsFloat;
			return gfs;
		}

		/// <summary>
		/// Overwrites the currently stored settings with the supplied
		/// gazeFocusSettings. This call will only affect the stored settings,
		/// it will not update the settings loaded by the gaze focus system.
		/// </summary>
		/// <remarks>Will only store settings if called in Unity Editor.</remarks>
		/// <param name="gazeFocusSettings"></param>
		public static void Set(GazeFocusSettings gazeFocusSettings)
		{
#if UNITY_EDITOR && UNITY_STANDALONE
			var json = new JSONClass();
			json["LayerMask"].AsInt = gazeFocusSettings.LayerMask;
			json["MaximumDistance"].AsFloat = gazeFocusSettings.MaximumDistance;
			string settingsAsJson = json.ToString();
			File.WriteAllText(FilePath, settingsAsJson);
#endif
		}

		public static GazeFocusSettings CreateDefault()
		{
			return new GazeFocusSettings
			{
				LayerMask = CreateDefaultLayerMask(),
				MaximumDistance = Mathf.Infinity
			};
		}

		public bool Equals(GazeFocusSettings otherSettings)
		{
			return this.LayerMask == otherSettings.LayerMask &&
				(float.IsInfinity(this.MaximumDistance) && float.IsInfinity(otherSettings.MaximumDistance) ||
				 Math.Abs(this.MaximumDistance - otherSettings.MaximumDistance) < float.Epsilon);
		}

#if UNITY_EDITOR
		private static bool IsInitialized()
		{
			return File.Exists(FilePath);
		}

		private static void Initialize()
		{
			Directory.CreateDirectory(DirectoryPath);
			Set(CreateDefault());
		}
#endif

		/// <summary>
		/// Creates a default layer mask with all non-empty layers selected.
		/// </summary>
		private static int CreateDefaultLayerMask()
		{
			int layerMask = 0;
			for (int i = 0; i < MaximumLayersInUnity; ++i)
			{
				if (!string.Empty.Equals(UnityEngine.LayerMask.LayerToName(i)))
				{
					layerMask |= (1 << i);
				}
			}

			return layerMask;
		}
	}
}
