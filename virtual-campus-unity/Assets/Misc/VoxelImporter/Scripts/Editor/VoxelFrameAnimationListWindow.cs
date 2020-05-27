using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    public class VoxelFrameAnimationListWindow : EditorWindow
    {
        public static VoxelFrameAnimationListWindow instance;

        public VoxelFrameAnimationObject objectTarget { get; private set; }

        public event Action frameIndexChanged;
        public event Action previewCameraModeChanged;

        private GUIStyle guiStyleButton;
        private GUIStyle guiStyleNameLabel;

        private static float frameIconSize = 64f;

        private Vector3 scrollPosition;

        public static void Create(VoxelFrameAnimationObject objectTarget)
        {
            if (instance == null)
            {
                instance = CreateInstance<VoxelFrameAnimationListWindow>();
            }

            instance.Initialize(objectTarget);
            
            instance.ShowUtility();
        }
        public static void Destroy()
        {
            if (instance != null)
            {
                instance.Close();
            }
        }

        void OnEnable()
        {
            InternalEditorUtility.RepaintAllViews();
        }
        void OnDisable()
        {
            instance = null;

            InternalEditorUtility.RepaintAllViews();
        }
        void OnDestroy()
        {
            OnDisable();
        }

        void OnSelectionChange()
        {
            var go = Selection.activeGameObject;
            if (go != objectTarget)
            {
                Close();
            }
        }

        private void Initialize(VoxelFrameAnimationObject objectTarget)
        {
            this.objectTarget = objectTarget;

            UpdateTitle();
        }

        void Update()
        {
            if (instance == null)
            {
                Close();
            }
        }

        void OnGUI()
        {
            #region GUIStyle
            if (guiStyleButton == null)
                guiStyleButton = new GUIStyle(GUI.skin.button);
            guiStyleButton.margin = new RectOffset(0, 0, 0, 0);
            guiStyleButton.overflow = new RectOffset(0, 0, 0, 0);
            guiStyleButton.padding = new RectOffset(0, 0, 0, 0);
            if (guiStyleNameLabel == null)
                guiStyleNameLabel = new GUIStyle(GUI.skin.label);
            guiStyleNameLabel.alignment = TextAnchor.LowerCenter;
            #endregion
            
            EditorGUILayout.BeginHorizontal();
            {
                #region PreviewCameraMode
                {
                    EditorGUI.BeginChangeCheck();
                    var edit_previewCameraMode = (VoxelFrameAnimationObject.Edit_CameraMode)EditorGUILayout.EnumPopup(objectTarget.edit_previewCameraMode, GUILayout.Width(64));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(objectTarget, "Camera Mode");
                        objectTarget.edit_previewCameraMode = edit_previewCameraMode;
                        if (previewCameraModeChanged != null)
                            previewCameraModeChanged.Invoke();
                        EditorApplication.delayCall += () =>
                        {
                            InternalEditorUtility.RepaintAllViews();
                        };
                    }
                }
                #endregion
                EditorGUILayout.Space();
                #region Size
                {
                    frameIconSize = EditorGUILayout.Slider(frameIconSize, 32f, 128f);
                }
                #endregion
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                int countX = Math.Max(1, Mathf.FloorToInt(position.width / frameIconSize));
                int countY = Mathf.CeilToInt(objectTarget.frames.Count / (float)countX);
                for (int i = 0; i < countY; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < countX; j++)
                    {
                        var index = i * countX + j;
                        if (index >= objectTarget.frames.Count) break;
                        var rect = EditorGUILayout.GetControlRect(false, frameIconSize, guiStyleButton, GUILayout.Width(frameIconSize), GUILayout.Height(frameIconSize));
                        EditorGUI.BeginChangeCheck();
                        GUI.Toggle(rect, index == objectTarget.edit_frameIndex, objectTarget.frames[index].icon, guiStyleButton);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(objectTarget, "Select Frame");
                            objectTarget.edit_frameIndex = index;
                            if (frameIndexChanged != null)
                                frameIndexChanged.Invoke();
                            UpdateTitle();
                            InternalEditorUtility.RepaintAllViews();
                        }
                        GUI.Label(rect, objectTarget.frames[index].name, guiStyleNameLabel);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                {
                    var index = -1;
                    var rect = EditorGUILayout.GetControlRect(false, frameIconSize, guiStyleButton, GUILayout.Width(frameIconSize), GUILayout.Height(frameIconSize));
                    EditorGUI.BeginChangeCheck();
                    GUI.Toggle(rect, index == objectTarget.edit_frameIndex, "", guiStyleButton);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(objectTarget, "Select Frame");
                        objectTarget.edit_frameIndex = index;
                        if (frameIndexChanged != null)
                            frameIndexChanged.Invoke();
                        UpdateTitle();
                        InternalEditorUtility.RepaintAllViews();
                    }
                    GUI.Label(rect, "None", guiStyleNameLabel);
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public void FrameIndexChanged()
        {
            UpdateTitle();
            Repaint();
        }

        private void UpdateTitle()
        {
            if (objectTarget.edit_frameEnable)
            {
                var frame = objectTarget.edit_currentFrame;
                instance.titleContent = new GUIContent(string.Format("Frame List ({0}) - ({1} / {2})", frame.name, objectTarget.edit_frameIndex, objectTarget.frames.Count));
            }
            else
            {
                instance.titleContent = new GUIContent("Frame List");
            }
        }
    }
}
