//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Tobii.Gaming.Internal;

namespace Tobii.Gaming
{
    public class EyeTrackingSettingsWindow : EditorWindow
    {
        [SerializeField]
        private static readonly List<int> LayerNumbers = new List<int>();
        [SerializeField]
        private GazeFocusSettings _gazeFocusSettings;
        [SerializeField]
        private GazeFocusSettings _lastSavedSettings;
        [SerializeField]
        private GazeFocusSettings _defaultSettings;
        [SerializeField]
        private bool _isInitialized;

        [MenuItem("Edit/Eye Tracking Settings...")]
        private static void Initialize()
        {
            EyeTrackingSettingsWindow window = (EyeTrackingSettingsWindow)EditorWindow.GetWindow(
                typeof(EyeTrackingSettingsWindow), true, "Eye Tracking Settings");
            window.Show();
        }

		void Awake()
		{
			_lastSavedSettings = GazeFocusSettings.CreateDefault();
			_defaultSettings = GazeFocusSettings.CreateDefault();
		}

		void OnGUI()
        {
            if (!_isInitialized)
            {
                _gazeFocusSettings = RetrieveSettings();
                CacheAsLastSavedSettings(_gazeFocusSettings);
                _isInitialized = true;
            }

            EditorGUILayout.Space();

            var headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.font.material.color = Color.white;
            EditorGUILayout.LabelField(
                new GUIContent("Gaze Focus Settings"), headerStyle);

            EditorGUILayout.Space();

            _gazeFocusSettings.MaximumDistance =
                EditorGUILayout.FloatField(
                    new GUIContent("Maximum Distance", "The maximum depth of the gaze focus detection"),
                    _gazeFocusSettings.MaximumDistance);

            _gazeFocusSettings.LayerMask =
                DrawLayerMaskField(
                    new GUIContent("Gaze Focus Layers", "Layers that should be used for the gaze focus detection."),
                    _gazeFocusSettings.LayerMask);

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            GUI.enabled = !IsDefault();
            if (GUILayout.Button("Default Values", GUILayout.MaxWidth(105)))
            {
                UseDefaultValues();
            }
            GUI.enabled = HasUnsavedChanges();
            if (GUILayout.Button("Revert", GUILayout.MaxWidth(100)))
            {
                RestoreToLastSavedSettings();
            }
            if (GUILayout.Button("Apply", GUILayout.MaxWidth(100)))
            {
                SaveSettings();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }

        void OnDestroy()
        {
            if (HasUnsavedChanges() && EditorUtility.DisplayDialog("There are unsaved changes",
                "Do you want to save your changes before you close the window?", "Save", "Discard Changes"))
            {
                SaveSettings();
            }
        }

        private bool HasUnsavedChanges()
        {
            return !_gazeFocusSettings.Equals(_lastSavedSettings);
        }

        private bool IsDefault()
        {
            return _defaultSettings.Equals(_gazeFocusSettings);
        }

        private void SaveSettings()
        {
            GazeFocusSettings.Set(_gazeFocusSettings);
            CacheAsLastSavedSettings(_gazeFocusSettings);
            NotifySettingsChanged();
        }

        private static GazeFocusSettings RetrieveSettings()
        {
            return GazeFocusSettings.Get();
        }

        private static void NotifySettingsChanged()
        {
            GazeFocus.SettingsUpdated();
        }

        private void CacheAsLastSavedSettings(GazeFocusSettings gazeFocusSettings)
        {
            _lastSavedSettings.LayerMask = gazeFocusSettings.LayerMask;
            _lastSavedSettings.MaximumDistance = gazeFocusSettings.MaximumDistance;
        }

        private void UseDefaultValues()
        {
            _gazeFocusSettings.LayerMask = _defaultSettings.LayerMask;
            _gazeFocusSettings.MaximumDistance = _defaultSettings.MaximumDistance;
        }

        private void RestoreToLastSavedSettings()
        {
            _gazeFocusSettings.LayerMask = _lastSavedSettings.LayerMask;
            _gazeFocusSettings.MaximumDistance = _lastSavedSettings.MaximumDistance;
        }

        /// <summary>
        /// Draws a MaskField of only the named layers in Unity, where the user
        /// can select what layers to include/exclude in the gaze focus detection.
        /// </summary>
        /// <returns>A mask value including all Unity layers.</returns>
        private static int DrawLayerMaskField(GUIContent content, int layerMask)
        {
            string[] layerNames = UnityEditorInternal.InternalEditorUtility.layers;
            LayerNumbers.Clear();

            for (int i = 0; i < layerNames.Length; i++)
            {
                LayerNumbers.Add(LayerMask.NameToLayer(layerNames[i]));
            }

            int maskWithoutEmpty = 0;
            for (int i = 0; i < LayerNumbers.Count; i++)
            {
                if (((1 << LayerNumbers[i]) & layerMask) > 0)
                    maskWithoutEmpty |= (1 << i);
            }

            maskWithoutEmpty = EditorGUILayout.MaskField(content, maskWithoutEmpty, layerNames);

            int mask = 0;
            for (int i = 0; i < LayerNumbers.Count; i++)
            {
                if ((maskWithoutEmpty & (1 << i)) > 0)
                {
                    mask |= (1 << LayerNumbers[i]);
                }
            }

            return mask;
        }
    }
}
