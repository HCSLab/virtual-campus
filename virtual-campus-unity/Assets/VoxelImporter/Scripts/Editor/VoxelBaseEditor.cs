using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

#if UNITY_2018_3_OR_NEWER
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif

namespace VoxelImporter
{
    public abstract class VoxelBaseEditor : Editor
    {
        public VoxelBase baseTarget { get; protected set; }
        public VoxelBaseCore baseCore { get; protected set; }

        protected ReorderableList materialList;

        protected VoxelEditorCommon editorCommon;

        protected bool drawEditorMesh = true;
        protected DataTable3<VoxelBase.Face> editVoxelList = new DataTable3<VoxelBase.Face>();

        protected VoxelData voxleDataBefore = null;

        protected static Rect editorConfigureEditorWindowRect = new Rect(8, 17 + 8, 0, 0);

        #region GUIStyle
        protected GUIStyle guiStyleMagentaBold;
        protected GUIStyle guiStyleRedBold;
        protected GUIStyle guiStyleFoldoutBold;
        protected GUIStyle guiStyleDropDown;
        protected GUIStyle guiStyleLabelMiddleLeftItalic;
        protected GUIStyle guiStyleTextFieldMiddleLeft;
        protected GUIStyle guiStyleEditorWindow;
        #endregion

        #region GUIContent
        public static readonly GUIContent CombineVoxelFacesContent = new GUIContent("Combine Voxel Faces", "When enabled, it reduces polygons as much as possible.\nNormally it is not necessary to disable it.");
        public static readonly GUIContent IgnoreCavityContent = new GUIContent("Ignore Cavity", "When enabled, it will not create internal cavities that can not be seen from the outside.\nNormally it is not necessary to disable it.\nFor example, it will invalidate it only when it is necessary, such as a treasure box.");
        public static readonly GUIContent ShareSameFaceContent = new GUIContent("Share same face", "When enabled, it shares the texture surface of the same design.\nNormally it is not necessary to disable it.\nIt disables this only when problems occur such as baking of the lightmapping.");
        public static readonly GUIContent RemoveUnusedPalettesContent = new GUIContent("Remove unused palettes", "This setting only affects imports from vox.");
        #endregion

        #region strings
        public static readonly string[] Edit_ConfigureMaterialMainModeString =
        {
            VoxelBase.Edit_ConfigureMaterialMainMode.Add.ToString(),
            VoxelBase.Edit_ConfigureMaterialMainMode.Remove.ToString(),
        };
        public static readonly string[] Edit_ConfigureMaterialSubModeString =
        {
            VoxelBase.Edit_ConfigureMaterialSubMode.Voxel.ToString(),
            VoxelBase.Edit_ConfigureMaterialSubMode.Fill.ToString(),
            VoxelBase.Edit_ConfigureMaterialSubMode.Rect.ToString(),
        };
        public static readonly string[] Edit_ConfigureDisableMainModeString =
        {
            VoxelBase.Edit_ConfigureDisableMainMode.Add.ToString(),
            VoxelBase.Edit_ConfigureDisableMainMode.Remove.ToString(),
        };
        public static readonly string[] Edit_ConfigureDisableSubModeString =
        {
            VoxelBase.Edit_ConfigureDisableSubMode.Face.ToString(),
            VoxelBase.Edit_ConfigureDisableSubMode.Fill.ToString(),
            VoxelBase.Edit_ConfigureDisableSubMode.Rect.ToString(),
        };
        public static readonly string[] Edit_AdvancedModeStrings =
        {
            "Simple",
            "Advanced",
        };
        #endregion

        #region ScaleTemplate
        public static readonly int[] ScaleDivisionTemplate =
        {
            1,
            2,
            4,
            8,
            16,
            32,
            64,
            128,
            256,
            512,
            1024,
            2048,
        };
        public static readonly float[] ScaleTemplateTemplate =
        {
            1f,
            0.75f,
            0.5f,
            0.25f,
            0.1f,
            0.075f,
            0.05f,
            0.025f,
            0.001f,
            0.0075f,
            0.005f,
            0.0025f,
            0.0001f,
        };
        #endregion

        #region Prefab
#if UNITY_2018_3_OR_NEWER
        protected PrefabAssetType prefabType { get { return PrefabUtility.GetPrefabAssetType(baseTarget.gameObject); } }
        protected bool prefabEnable { get { return (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant) || baseCore.isPrefabEditMode; } }
        protected bool isPrefab { get { return false; } }
#else
        protected PrefabType prefabType { get { return PrefabUtility.GetPrefabType(baseTarget.gameObject); } }
        protected bool prefabEnable { get { var type = prefabType; return type == PrefabType.Prefab || type == PrefabType.PrefabInstance || type == PrefabType.DisconnectedPrefabInstance; } }
        protected bool isPrefab { get { return prefabType == PrefabType.Prefab; } }
#endif
        #endregion

        protected virtual void OnEnable()
        {
            baseTarget = target as VoxelBase;
            if (baseTarget == null) return;

            Undo.undoRedoPerformed -= EditorUndoRedoPerformed;
            Undo.undoRedoPerformed += EditorUndoRedoPerformed;
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneCustomGUI;
            SceneView.duringSceneGui += OnSceneCustomGUI;
#endif
        }
        protected virtual void OnDisable()
        {
            if (baseTarget == null) return;

            AfterRefresh();

            EditEnableMeshDestroy();

            baseCore.SetSelectedWireframeHidden(false);

#if !UNITY_2018_3_OR_NEWER
            if (isPrefab)
            {
                baseCore.ClearVoxelData();
            }
#endif

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneCustomGUI;
#endif
            Undo.undoRedoPerformed -= EditorUndoRedoPerformed;
        }
        protected virtual void OnDestroy()
        {
            OnDisable();
        }

        protected virtual void OnEnableInitializeSet()
        {
            baseCore.Initialize();

            editorCommon = new VoxelEditorCommon(baseTarget, baseCore);

            UpdateMaterialList();
            UpdateConfigureEnableMesh();
        }

        protected virtual void GUIStyleReady()
        {
            //Styles
            if (guiStyleMagentaBold == null)
                guiStyleMagentaBold = new GUIStyle(EditorStyles.boldLabel);
            guiStyleMagentaBold.normal.textColor = Color.magenta;
            if (guiStyleRedBold == null)
                guiStyleRedBold = new GUIStyle(EditorStyles.boldLabel);
            guiStyleRedBold.normal.textColor = Color.red;
            if (guiStyleFoldoutBold == null)
                guiStyleFoldoutBold = new GUIStyle(EditorStyles.foldout);
            guiStyleFoldoutBold.fontStyle = FontStyle.Bold;
            if (guiStyleDropDown == null)
                guiStyleDropDown = new GUIStyle("DropDown");
            guiStyleDropDown.alignment = TextAnchor.MiddleCenter;
            if (guiStyleLabelMiddleLeftItalic == null)
                guiStyleLabelMiddleLeftItalic = new GUIStyle(EditorStyles.label);
            guiStyleLabelMiddleLeftItalic.alignment = TextAnchor.MiddleLeft;
            guiStyleLabelMiddleLeftItalic.fontStyle = FontStyle.Italic;
            if (guiStyleTextFieldMiddleLeft == null)
                guiStyleTextFieldMiddleLeft = new GUIStyle(EditorStyles.textField);
            guiStyleTextFieldMiddleLeft.alignment = TextAnchor.MiddleLeft;
            if (guiStyleEditorWindow == null)
            {
                if (EditorGUIUtility.isProSkin)
                    guiStyleEditorWindow = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).window);
                else
                    guiStyleEditorWindow = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).window);
            }
        }

        public override void OnInspectorGUI()
        {
            if (baseTarget == null || editorCommon == null)
            {
                DrawDefaultInspector();
                return;
            }

            baseCore.AutoSetSelectedWireframeHidden();

            serializedObject.Update();

            InspectorGUI();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void InspectorGUI()
        {
#if UNITY_2018_3_OR_NEWER
            {
                if (!baseCore.isPrefabEditable)
                {
                    EditorGUILayout.HelpBox("Prefab can only be edited in Prefab mode.", MessageType.Info);
                }
            }
#endif

            GUIStyleReady();
            editorCommon.GUIStyleReady();
            if (baseTarget.voxelData != voxleDataBefore)
            {
                UpdateMaterialList();
                voxleDataBefore = baseTarget.voxelData;
            }

            #region Simple
            {
                EditorGUI.BeginChangeCheck();
                var mode = GUILayout.Toolbar(baseTarget.advancedMode ? 1 : 0, Edit_AdvancedModeStrings);
                if (EditorGUI.EndChangeCheck())
                {
                    baseTarget.advancedMode = mode != 0 ? true : false;
                }
            }
            #endregion
        }

        protected void InspectorGUI_Import()
        {
            Event e = Event.current;

            baseTarget.edit_importFoldout = EditorGUILayout.Foldout(baseTarget.edit_importFoldout, "Import", guiStyleFoldoutBold);
            if (baseTarget.edit_importFoldout)
            {
                EditorGUI.BeginDisabledGroup(isPrefab);

                EditorGUILayout.BeginHorizontal(editorCommon.guiStyleSkinBox);
                EditorGUILayout.BeginVertical();
                #region Voxel File
                {
                    bool fileExists = baseCore.IsVoxelFileExists();
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (string.IsNullOrEmpty(baseTarget.voxelFilePath))
                            EditorGUILayout.LabelField("Voxel File", guiStyleMagentaBold);
                        else if (!fileExists)
                            EditorGUILayout.LabelField("Voxel File", guiStyleRedBold);
                        else
                            EditorGUILayout.LabelField("Voxel File", EditorStyles.boldLabel);

                        Action<string, UnityEngine.Object> OpenFile = (path, obj) =>
                        {
                            if (EditorCommon.IsSubAsset(obj))
                                return;
                            if (!baseCore.IsEnableFile(path))
                                return;
                            if (obj == null && path.Contains(Application.dataPath))
                            {
                                var assetPath = FileUtil.GetProjectRelativePath(path);
                                obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                            }
                            UndoRecordObject("Open Voxel File", true);
                            baseCore.Reset(path, obj);
                            baseCore.Create(path, obj);
                            UpdateMaterialList();
                        };

                        var rect = GUILayoutUtility.GetRect(new GUIContent("Open"), guiStyleDropDown, GUILayout.Width(64));
                        if (GUI.Button(rect, "Open", guiStyleDropDown))
                        {
                            InspectorGUI_ImportOpenBefore();
                            GenericMenu menu = new GenericMenu();
                            #region vox
                            menu.AddItem(new GUIContent("MagicaVoxel (*.vox)"), false, () =>
                            {
                                var path = EditorUtility.OpenFilePanel("Open MagicaVoxel File", !string.IsNullOrEmpty(baseTarget.voxelFilePath) ? Path.GetDirectoryName(baseTarget.voxelFilePath) : "", "vox");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    OpenFile(path, null);
                                }
                            });
                            #endregion
                            #region qb
                            menu.AddItem(new GUIContent("Qubicle Binary (*.qb)"), false, () =>
                            {
                                var path = EditorUtility.OpenFilePanel("Open Qubicle Binary File", !string.IsNullOrEmpty(baseTarget.voxelFilePath) ? Path.GetDirectoryName(baseTarget.voxelFilePath) : "", "qb");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    OpenFile(path, null);
                                }
                            });
                            #endregion
                            #region png
                            menu.AddItem(new GUIContent("Pixel Art (*.png)"), false, () =>
                            {
                                var path = EditorUtility.OpenFilePanel("Open Pixel Art File", !string.IsNullOrEmpty(baseTarget.voxelFilePath) ? Path.GetDirectoryName(baseTarget.voxelFilePath) : "", "png");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    OpenFile(path, null);
                                }
                            });
                            #endregion
                            menu.ShowAsContext();
                        }
                        #region Drag&Drop
                        {
                            switch (e.type)
                            {
                            case EventType.DragUpdated:
                            case EventType.DragPerform:
                                if (!rect.Contains(e.mousePosition)) break;
                                if (DragAndDrop.paths.Length != 1) break;
                                DragAndDrop.AcceptDrag();
                                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                                if (e.type == EventType.DragPerform)
                                {
                                    string path = DragAndDrop.paths[0];
                                    if (Path.GetPathRoot(path) == "")
                                        path = EditorCommon.GetProjectRelativePath2FullPath(DragAndDrop.paths[0]);
                                    OpenFile(path, DragAndDrop.objectReferences.Length > 0 ? DragAndDrop.objectReferences[0] : null);
                                    e.Use();
                                }
                                break;
                            }
                        }
                        #endregion
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel++;
                    {
                        if (fileExists)
                        {
                            EditorGUILayout.BeginHorizontal();
                            if (baseTarget.voxelFileObject == null)
                            {
                                EditorGUILayout.LabelField(Path.GetFileName(baseTarget.voxelFilePath));
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                var obj = EditorGUILayout.ObjectField(baseTarget.voxelFileObject, typeof(UnityEngine.Object), false);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (!EditorCommon.IsSubAsset(obj))
                                    {
                                        var path = EditorCommon.GetProjectRelativePath2FullPath(AssetDatabase.GetAssetPath(obj));
                                        if (baseCore.IsEnableFile(path))
                                        {
                                            UndoRecordObject("Open Voxel File", true);
                                            baseCore.Reset(path, obj);
                                            baseCore.Create(path, obj);
                                            UpdateMaterialList();
                                        }
                                    }
                                }
                            }
                            if (baseTarget.advancedMode)
                            {
                                EditorGUILayout.LabelField(baseTarget.voxelData != null ? "Loaded" : "Unloaded", GUILayout.Width(80));
                                if (baseTarget.voxelData == null)
                                {
                                    if (GUILayout.Button("Load", GUILayout.Width(54), GUILayout.Height(16)))
                                    {
                                        baseCore.ReadyVoxelData();
                                    }
                                }
                                else
                                {
                                    if (GUILayout.Button("UnLoad", GUILayout.Width(54), GUILayout.Height(16)))
                                    {
                                        baseCore.ClearVoxelData();
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        else
                        {
                            {
                                EditorGUI.BeginChangeCheck();
                                var obj = EditorGUILayout.ObjectField(baseTarget.voxelFileObject, typeof(UnityEngine.Object), false);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    if (!EditorCommon.IsSubAsset(obj))
                                    {
                                        var path = EditorCommon.GetProjectRelativePath2FullPath(AssetDatabase.GetAssetPath(obj));
                                        if (baseCore.IsEnableFile(path))
                                        {
                                            UndoRecordObject("Open Voxel File", true);
                                            baseCore.Reset(path, obj);
                                            baseCore.Create(path, obj);
                                            UpdateMaterialList();
                                        }
                                    }
                                }
                            }
                            EditorGUILayout.HelpBox("Voxel file not found. Please open file.", MessageType.Error);
                        }
                    }
                    EditorGUI.indentLevel--;
                }
                #endregion
                #region Settings
                {
                    EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    {
                        if (baseTarget.advancedMode)
                        {
                            #region LegacyVoxImport
                            if (baseTarget.fileType == VoxelBase.FileType.vox)
                            {
                                EditorGUI.BeginChangeCheck();
                                var legacyVoxImport = EditorGUILayout.Toggle(new GUIContent("Legacy Vox Import", "Import with legacy behavior up to Version 1.1.2.\nMultiple objects do not correspond.\nIt is deprecated for future use.\nThis is left for compatibility."), baseTarget.legacyVoxImport);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    UndoRecordObject("Inspector", true);
                                    baseTarget.legacyVoxImport = legacyVoxImport;
                                    baseCore.ReadyVoxelData(true);
                                    Refresh();
                                }
                            }
                            #endregion

                            #region Import Mode
                            {
                                EditorGUI.BeginChangeCheck();
                                var importMode = (VoxelObject.ImportMode)EditorGUILayout.EnumPopup("Import Mode", baseTarget.importMode);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    UndoRecordObject("Inspector");
                                    baseTarget.importMode = importMode;
                                    Refresh();
                                }
                            }
                            #endregion
                            #region Import Flag
                            {
                                EditorGUI.BeginChangeCheck();
#if UNITY_2017_3_OR_NEWER
                                var importFlags = (VoxelObject.ImportFlag)EditorGUILayout.EnumFlagsField("Import Flag", baseTarget.importFlags);
#else
                                var importFlags = (VoxelObject.ImportFlag)EditorGUILayout.EnumMaskField("Import Flag", baseTarget.importFlags);
#endif
                                if (EditorGUI.EndChangeCheck())
                                {
                                    UndoRecordObject("Inspector", true);
                                    baseTarget.importFlags = importFlags;
                                    baseCore.ReadyVoxelData(true);
                                    Refresh();
                                }
                            }
                            #endregion
                        }
                        #region Import Scale
                        {
                            InspectorGUI_ImportSettingsImportScale();
                        }
                        #endregion
                        #region Import Offset
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                InspectorGUI_ImportSettingsImportOffset();
                            }
                            {
                                if (GUILayout.Button("Set", guiStyleDropDown, GUILayout.Width(40), GUILayout.Height(14)))
                                {
                                    GenericMenu menu = new GenericMenu();
                                    #region Reset
                                    menu.AddItem(new GUIContent("Reset"), false, () =>
                                    {
                                        UndoRecordObject("Inspector", true);
                                        baseTarget.importOffset = Vector3.zero;
                                        Refresh();
                                    });
                                    #endregion
                                    #region Center
                                    menu.AddItem(new GUIContent("Center"), false, () =>
                                    {
                                        UndoRecordObject("Inspector", true);
                                        baseTarget.importOffset = Vector3.zero;
                                        baseTarget.importOffset = -baseCore.GetVoxelsCenter();
                                        Refresh();
                                    });
                                    #endregion
                                    InspectorGUI_ImportOffsetSetExtra(menu);
                                    menu.ShowAsContext();
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        #endregion
                        if (baseTarget.advancedMode)
                        {
                            InspectorGUI_ConfigureDisable();
                        }
                        InspectorGUI_ImportSettingsExtra();
                    }
                    EditorGUI.indentLevel--;
                }
                #endregion
                #region Optimize
                if (baseTarget.advancedMode)
                {
                    EditorGUILayout.LabelField("Optimize", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    {
                        #region combineFaces
                        {
                            EditorGUI.BeginChangeCheck();
                            var combineFaces = EditorGUILayout.Toggle(CombineVoxelFacesContent, baseTarget.combineFaces);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.combineFaces = combineFaces;
                                Refresh();
                            }
                        }
                        #endregion
                        #region Ignore the cavity
                        {
                            EditorGUI.BeginChangeCheck();
                            var ignoreCavity = EditorGUILayout.Toggle(IgnoreCavityContent, baseTarget.ignoreCavity);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.ignoreCavity = ignoreCavity;
                                Refresh();
                            }
                        }
                        #endregion
                        #region shareSameFace
                        {
                            EditorGUI.BeginChangeCheck();
                            var shareSameFace = EditorGUILayout.Toggle(ShareSameFaceContent, baseTarget.shareSameFace);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.shareSameFace = shareSameFace;
                                Refresh();
                            }
                        }
                        #endregion
                        #region removeUnusedPalettes
                        if (baseTarget.fileType == VoxelBase.FileType.vox)
                        {
                            EditorGUI.BeginChangeCheck();
                            var removeUnusedPalettes = EditorGUILayout.Toggle(RemoveUnusedPalettesContent, baseTarget.removeUnusedPalettes);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.removeUnusedPalettes = removeUnusedPalettes;
                                baseCore.ClearVoxelData();
                                Refresh();
                            }
                        }
                        #endregion
                    }
                    InspectorGUI_ImportOptimizeExtra();
                    EditorGUI.indentLevel--;
                }
                #endregion
                #region Output
                if (baseTarget.advancedMode)
                {
                    EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);
                    EditorGUI.indentLevel++;
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.ObjectField(new GUIContent("Voxel Structure", "Save the structure information."), baseTarget.voxelStructure, typeof(VoxelStructure), false);
                        EditorGUI.EndDisabledGroup();
                        if (baseTarget.voxelStructure == null)
                        {
                            if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                            {
                                #region Save
                                UndoRecordObject("Inspector");
                                string path = EditorUtility.SaveFilePanel("Save as", baseCore.GetDefaultPath(), string.Format("{0}.asset", Path.GetFileNameWithoutExtension(baseTarget.voxelFilePath)), "asset");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.IndexOf(Application.dataPath) < 0)
                                    {
                                        EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                    }
                                    else
                                    {
                                        path = FileUtil.GetProjectRelativePath(path);
                                        baseTarget.voxelStructure = ScriptableObject.CreateInstance<VoxelStructure>();
                                        baseTarget.voxelStructure.Set(baseCore.voxelData);
                                        AssetDatabase.CreateAsset(baseTarget.voxelStructure, path);
                                    }
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                            {
                                #region Reset
                                UndoRecordObject("Inspector");
                                baseTarget.voxelStructure = null;
                                #endregion
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
                #endregion
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();

                EditorGUI.EndDisabledGroup();
            }
        }
        protected virtual void UndoRecordObject(string text, bool reset = false)
        {
            if (baseTarget != null)
                Undo.RecordObject(baseTarget, text);
        }
        protected virtual void InspectorGUI_ImportOpenBefore() { }
        protected void InspectorGUI_ImportSettingsImportScale()
        {
            EditorGUILayout.BeginHorizontal();

            InspectorGUI_ImportSettingsImportScaleVector();

            if (GUILayout.Button("Set", guiStyleDropDown, GUILayout.Width(40), GUILayout.Height(14)))
            {
                GenericMenu menu = new GenericMenu();
                #region Division
                {
                    foreach (var value in ScaleDivisionTemplate)
                    {
                        menu.AddItem(new GUIContent(string.Format("Division/{0}", value)), false, () =>
                        {
                            var tmp = 1f / (float)value;
                            InspectorGUI_ImportSettingsImportScale_Set(new Vector3(tmp, tmp, tmp));
                        });
                    }
                }
                #endregion
                #region Template
                {
                    foreach (var value in ScaleTemplateTemplate)
                    {
                        menu.AddItem(new GUIContent(string.Format("Template/{0}", value)), false, () =>
                        {
                            InspectorGUI_ImportSettingsImportScale_Set(new Vector3(value, value, value));
                        });
                    }
                }
                #endregion
                menu.AddSeparator("");
                #region Default value
                {
                    menu.AddItem(new GUIContent("Default value/Save to default value"), false, () =>
                    {
                        EditorPrefs.SetFloat("VoxelImporter_DefaultScaleX", baseTarget.importScale.x);
                        EditorPrefs.SetFloat("VoxelImporter_DefaultScaleY", baseTarget.importScale.y);
                        EditorPrefs.SetFloat("VoxelImporter_DefaultScaleZ", baseTarget.importScale.z);
                    });
                    menu.AddItem(new GUIContent("Default value/Load from default value"), false, () =>
                    {
                        var x = EditorPrefs.GetFloat("VoxelImporter_DefaultScaleX", 1f);
                        var y = EditorPrefs.GetFloat("VoxelImporter_DefaultScaleY", 1f);
                        var z = EditorPrefs.GetFloat("VoxelImporter_DefaultScaleZ", 1f);
                        InspectorGUI_ImportSettingsImportScale_Set(new Vector3(x, y, z));
                    });
                }
                #endregion
                menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();
        }
        protected virtual void InspectorGUI_ImportSettingsImportScaleVector()
        {
            EditorGUI.BeginChangeCheck();
            var importScale = EditorGUILayout.Vector3Field("Import Scale", baseTarget.importScale);
            if (EditorGUI.EndChangeCheck())
            {
                InspectorGUI_ImportSettingsImportScale_Set(importScale);
            }
        }
        protected virtual void InspectorGUI_ImportSettingsImportScale_Set(Vector3 scale)
        {
            UndoRecordObject("Inspector", true);
            baseTarget.importScale = scale;
            Refresh();
        }
        protected virtual void InspectorGUI_ImportSettingsImportOffset()
        {
            EditorGUI.BeginChangeCheck();
            var importOffset = EditorGUILayout.Vector3Field("Import Offset", baseTarget.importOffset);
            if (EditorGUI.EndChangeCheck())
            {
                UndoRecordObject("Inspector", true);
                baseTarget.importOffset = importOffset;
                Refresh();
            }
        }
        protected virtual void InspectorGUI_ImportSettingsExtra() { }
        protected virtual void InspectorGUI_ImportOptimizeExtra() { }
        protected virtual void InspectorGUI_ImportOffsetSetExtra(GenericMenu menu) { }
        protected virtual void InspectorGUI_Object_Mesh_Settings()
        {
            #region Generate Lightmap UVs
            {
                EditorGUI.BeginChangeCheck();
                var generateLightmapUVs = EditorGUILayout.Toggle(new GUIContent("Generate Lightmap UVs", "Generate lightmap UVs into UV2."), baseTarget.generateLightmapUVs);
                if (EditorGUI.EndChangeCheck())
                {
                    UndoRecordObject("Inspector");
                    baseTarget.generateLightmapUVs = generateLightmapUVs;
                    Refresh();
                }
                if (baseTarget.generateLightmapUVs)
                {
                    EditorGUI.indentLevel++;
                    baseTarget.edit_generateLightmapUVsAdvancedFoldout = EditorGUILayout.Foldout(baseTarget.edit_generateLightmapUVsAdvancedFoldout, new GUIContent("Advanced"));
                    if (baseTarget.edit_generateLightmapUVsAdvancedFoldout)
                    {
                        {
                            EditorGUI.BeginChangeCheck();
                            var hardAngle = EditorGUILayout.Slider(new GUIContent("Hard Angle", "Angle between neighbor triangles that will generate seam."), baseTarget.generateLightmapUVsHardAngle, 0f, 180f);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.generateLightmapUVsHardAngle = Mathf.Round(hardAngle);
                                Refresh();
                            }
                        }
                        {
                            EditorGUI.BeginChangeCheck();
                            var packMargin = EditorGUILayout.Slider(new GUIContent("Pack Margin", "Measured in pixels, assuming mesh will cover an entire 1024x1024 lightmap."), baseTarget.generateLightmapUVsPackMargin, 1f, 64f);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.generateLightmapUVsPackMargin = Mathf.Round(packMargin);
                                Refresh();
                            }
                        }
                        {
                            EditorGUI.BeginChangeCheck();
                            var angleError = EditorGUILayout.Slider(new GUIContent("Angle Error", "Measured in percents. Angle error measures deviation of UV angles from geometry angles. Area error measures deviation of UV triangles area from geometry triangles if they were uniformly scaled."),
                                                                    baseTarget.generateLightmapUVsAngleError, 1f, 75f);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.generateLightmapUVsAngleError = Mathf.Round(angleError);
                                Refresh();
                            }
                        }
                        {
                            EditorGUI.BeginChangeCheck();
                            var areaError = EditorGUILayout.Slider(new GUIContent("Area Error"), baseTarget.generateLightmapUVsAreaError, 1f, 75f);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.generateLightmapUVsAreaError = Mathf.Round(areaError);
                                Refresh();
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            #endregion
            #region Generate Tangents
            {
                EditorGUI.BeginChangeCheck();
                var generateTangents = EditorGUILayout.Toggle(new GUIContent("Generate Tangents", "Generate Tangents."), baseTarget.generateTangents);
                if (EditorGUI.EndChangeCheck())
                {
                    UndoRecordObject("Inspector");
                    baseTarget.generateTangents = generateTangents;
                    Refresh();
                }
            }
            #endregion
            #region meshFaceVertexOffset
            {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayout.Slider(new GUIContent("Vertex Offset", "Increase this value if flickering of polygon gaps occurs at low resolution."), baseTarget.meshFaceVertexOffset, 0f, 0.01f);
                if (EditorGUI.EndChangeCheck())
                {
                    UndoRecordObject("Inspector");
                    baseTarget.meshFaceVertexOffset = value;
                    Refresh();
                }
            }
            #endregion
        }
        protected virtual void InspectorGUI_Refresh()
        {
            if (GUILayout.Button("Refresh"))
            {
                UndoRecordObject("Inspector");
                Refresh();
            }
        }
        protected virtual bool IsEnableConfigureDisable() { return true; }
        protected virtual void BeginConfigureDisable() { }
        protected void InspectorGUI_ConfigureDisable()
        {
            if (baseTarget.disableData != null)
            {
                EditorGUI.BeginDisabledGroup(!IsEnableConfigureDisable() || isPrefab);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                GUILayout.Toggle(baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable, "Configure Disable Face", GUI.skin.button);
                if (EditorGUI.EndChangeCheck())
                {
                    UndoRecordObject("Configure Disable Face");
                    if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
                    {
                        baseTarget.edit_configureMode = VoxelBase.Edit_ConfigureMode.None;
                        AfterRefresh();
                    }
                    else
                    {
                        baseTarget.edit_configureMode = VoxelBase.Edit_ConfigureMode.Disable;
                        BeginConfigureDisable();
                        UpdateConfigureEnableMesh();
                        editorConfigureEditorWindowRect.width = editorConfigureEditorWindowRect.height = 0;
                    }
                    InternalEditorUtility.RepaintAllViews();
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
            }
            else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
            {
                baseTarget.edit_configureMode = VoxelBase.Edit_ConfigureMode.None;
                AfterRefresh();
            }

            if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable &&
                !IsEnableConfigureDisable())
            {
                baseTarget.edit_configureMode = VoxelBase.Edit_ConfigureMode.None;
                AfterRefresh();
            }
        }
        protected virtual bool IsEnableConfigureMaterial() { return true; }
        protected virtual void BeginConfigureMaterial() { }
        protected void InspectorGUI_ConfigureMaterial()
        {
            if (baseTarget.materialData != null && baseTarget.materialData.Count > 1 && !baseTarget.loadFromVoxelFile)
            {
                EditorGUI.BeginDisabledGroup(!IsEnableConfigureMaterial() || isPrefab);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                GUILayout.Toggle(baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material, "Configure Material", GUI.skin.button);
                if (EditorGUI.EndChangeCheck())
                {
                    UndoRecordObject("Configure Material");
                    if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
                    {
                        baseTarget.edit_configureMode = VoxelBase.Edit_ConfigureMode.None;
                        AfterRefresh();
                    }
                    else
                    {
                        baseTarget.edit_configureMode = VoxelBase.Edit_ConfigureMode.Material;
                        BeginConfigureMaterial();
                        UpdateConfigureEnableMesh();
                        editorConfigureEditorWindowRect.width = editorConfigureEditorWindowRect.height = 0;
                    }
                    InternalEditorUtility.RepaintAllViews();
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                EditorGUILayout.Space();
            }
            else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
            {
                baseTarget.edit_configureMode = VoxelBase.Edit_ConfigureMode.None;
                AfterRefresh();
            }

            if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material &&
                !IsEnableConfigureMaterial())
            {
                baseTarget.edit_configureMode = VoxelBase.Edit_ConfigureMode.None;
                AfterRefresh();
            }
        }

#if UNITY_2019_1_OR_NEWER
        private void OnSceneCustomGUI(SceneView sceneView)
        {
            if (sceneView != SceneView.currentDrawingSceneView) return;
#else
        protected virtual void OnSceneGUI()
        {
#endif
            if (baseTarget == null || editorCommon == null) return;

            GUIStyleReady();
            editorCommon.GUIStyleReady();

            Event e = Event.current;
            bool repaint = false;

            #region Configure Material
            if (baseTarget.edit_configureMode != VoxelBase.Edit_ConfigureMode.None)
            {
                if (SceneView.currentDrawingSceneView == SceneView.lastActiveSceneView)
                {
                    if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
                    {
                        #region Material Editor
                        if (baseTarget.materialData != null && materialList != null &&
                            baseTarget.edit_configureMaterialIndex > 0 && baseTarget.edit_configureMaterialIndex < baseTarget.materialData.Count)
                        {
                            editorConfigureEditorWindowRect = GUILayout.Window(EditorGUIUtility.GetControlID(FocusType.Passive, editorConfigureEditorWindowRect), editorConfigureEditorWindowRect, (id) =>
                            {
                                #region MainMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_configureMaterialMainMode = (VoxelBase.Edit_ConfigureMaterialMainMode)GUILayout.Toolbar((int)baseTarget.edit_configureMaterialMainMode, Edit_ConfigureMaterialMainModeString);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(baseTarget, "Main Mode");
                                        baseTarget.edit_configureMaterialMainMode = edit_configureMaterialMainMode;
                                        ShowNotification();
                                    }
                                }
                                #endregion
                                #region SubMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_configureMaterialSubMode = (VoxelBase.Edit_ConfigureMaterialSubMode)GUILayout.Toolbar((int)baseTarget.edit_configureMaterialSubMode, Edit_ConfigureMaterialSubModeString);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(baseTarget, "Sub Mode");
                                        baseTarget.edit_configureMaterialSubMode = edit_configureMaterialSubMode;
                                        ShowNotification();
                                    }
                                }
                                #endregion
                                #region Transparent
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var transparent = EditorGUILayout.Toggle("Transparent", baseTarget.materialData[baseTarget.edit_configureMaterialIndex].transparent);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(baseTarget, "Transparent");
                                        baseTarget.materialData[baseTarget.edit_configureMaterialIndex].transparent = transparent;
                                        baseTarget.edit_afterRefresh = true;
                                    }
                                }
                                #endregion
                                #region Clear
                                {
                                    if (GUILayout.Button("Clear"))
                                    {
                                        Undo.RecordObject(baseTarget, "Clear");
                                        baseTarget.materialData[baseTarget.edit_configureMaterialIndex].ClearMaterial();
                                        UpdateConfigureEnableMesh();
                                        baseTarget.edit_afterRefresh = true;
                                    }
                                }
                                #endregion
                                #region PreviewMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_ConfigurePreviewMode = (VoxelBase.Edit_ConfigurePreviewMode)EditorGUILayout.EnumPopup("Preview", baseTarget.edit_ConfigurePreviewMode);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(baseTarget, "Preview Mode");
                                        baseTarget.edit_ConfigurePreviewMode = edit_ConfigurePreviewMode;
                                    }
                                }
                                #endregion
                                #region Help
                                if (!baseTarget.edit_helpEnable)
                                {
                                    #region "?"
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUI.BeginChangeCheck();
                                        GUILayout.Toggle(baseTarget.edit_helpEnable, "?", GUI.skin.button, GUILayout.Width(16));
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(baseTarget, "Help Enable");
                                            baseTarget.edit_helpEnable = !baseTarget.edit_helpEnable;
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    EditorGUI.BeginChangeCheck();
                                    baseTarget.edit_helpEnable = EditorGUILayout.Foldout(baseTarget.edit_helpEnable, "Help");
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        editorConfigureEditorWindowRect.width = editorConfigureEditorWindowRect.height = 0;
                                    }
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    EditorGUILayout.LabelField("F5 Key - Refresh");
                                    EditorGUILayout.LabelField("Press Space Key - Hide Preview");
                                    EditorGUILayout.EndVertical();
                                }
                                #endregion
                                GUI.DragWindow();

                            }, "Material Editor", guiStyleEditorWindow);
                            editorConfigureEditorWindowRect = editorCommon.ResizeSceneViewRect(editorConfigureEditorWindowRect);
                        }
                        #endregion
                    }
                    else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
                    {
                        #region Disable Editor
                        if (baseTarget.disableData != null)
                        {
                            editorConfigureEditorWindowRect = GUILayout.Window(EditorGUIUtility.GetControlID(FocusType.Passive, editorConfigureEditorWindowRect), editorConfigureEditorWindowRect, (id) =>
                            {
                                #region MainMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_configureDisableMainMode = (VoxelBase.Edit_ConfigureDisableMainMode)GUILayout.Toolbar((int)baseTarget.edit_configureDisableMainMode, Edit_ConfigureDisableMainModeString);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(baseTarget, "Main Mode");
                                        baseTarget.edit_configureDisableMainMode = edit_configureDisableMainMode;
                                        ShowNotification();
                                    }
                                }
                                #endregion
                                #region SubMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_configureDisableSubMode = (VoxelBase.Edit_ConfigureDisableSubMode)GUILayout.Toolbar((int)baseTarget.edit_configureDisableSubMode, Edit_ConfigureDisableSubModeString);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(baseTarget, "Sub Mode");
                                        baseTarget.edit_configureDisableSubMode = edit_configureDisableSubMode;
                                        ShowNotification();
                                    }
                                }
                                #endregion
                                #region Set
                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                {
                                    #region All
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PrefixLabel("All");
                                        if (GUILayout.Button("Add"))
                                        {
                                            Undo.RecordObject(baseTarget, "Add");
                                            foreach (var voxel in baseTarget.voxelData.voxels)
                                            {
                                                var visible = voxel.visible & VoxelBase.FaceAllFlags;
                                                if (visible == 0) continue;
                                                baseTarget.disableData.SetDisable(voxel.position, visible);
                                            }
                                            UpdateConfigureEnableMesh();
                                            baseTarget.edit_afterRefresh = true;
                                        }
                                        if (GUILayout.Button("Remove"))
                                        {
                                            Undo.RecordObject(baseTarget, "Remove");
                                            baseTarget.disableData.ClearDisable();
                                            UpdateConfigureEnableMesh();
                                            baseTarget.edit_afterRefresh = true;
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    #endregion
                                    #region Faces
                                    Action<string, VoxelBase.Face> FaceAction = (label, flag) =>
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.PrefixLabel(label);
                                        if (GUILayout.Button("Add"))
                                        {
                                            Undo.RecordObject(baseTarget, "Add");
                                            foreach (var voxel in baseTarget.voxelData.voxels)
                                            {
                                                var visible = voxel.visible & flag;
                                                if (visible == 0) continue;
                                                var face = baseTarget.disableData.GetDisable(voxel.position);
                                                baseTarget.disableData.SetDisable(voxel.position, face | flag);
                                            }
                                            UpdateConfigureEnableMesh();
                                            baseTarget.edit_afterRefresh = true;
                                        }
                                        if (GUILayout.Button("Remove"))
                                        {
                                            Undo.RecordObject(baseTarget, "Remove");
                                            baseTarget.disableData.AllAction((pos, face) =>
                                            {
                                                baseTarget.disableData.SetDisable(pos, face & ~flag);
                                            });
                                            UpdateConfigureEnableMesh();
                                            baseTarget.edit_afterRefresh = true;
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    };
                                    FaceAction("Right", VoxelBase.Face.right);
                                    FaceAction("Left", VoxelBase.Face.left);
                                    FaceAction("Up", VoxelBase.Face.up);
                                    FaceAction("Down", VoxelBase.Face.down);
                                    FaceAction("Forward", VoxelBase.Face.forward);
                                    FaceAction("Back", VoxelBase.Face.back);
                                    #endregion
                                }
                                EditorGUILayout.EndVertical();
                                #endregion
                                #region PreviewMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_ConfigurePreviewMode = (VoxelBase.Edit_ConfigurePreviewMode)EditorGUILayout.EnumPopup("Preview", baseTarget.edit_ConfigurePreviewMode);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(baseTarget, "Preview Mode");
                                        baseTarget.edit_ConfigurePreviewMode = edit_ConfigurePreviewMode;
                                    }
                                }
                                #endregion
                                #region Help
                                if (!baseTarget.edit_helpEnable)
                                {
                                    #region "?"
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUI.BeginChangeCheck();
                                        GUILayout.Toggle(baseTarget.edit_helpEnable, "?", GUI.skin.button, GUILayout.Width(16));
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(baseTarget, "Help Enable");
                                            baseTarget.edit_helpEnable = !baseTarget.edit_helpEnable;
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    EditorGUI.BeginChangeCheck();
                                    baseTarget.edit_helpEnable = EditorGUILayout.Foldout(baseTarget.edit_helpEnable, "Help");
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        editorConfigureEditorWindowRect.width = editorConfigureEditorWindowRect.height = 0;
                                    }
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    EditorGUILayout.LabelField("F5 Key - Refresh");
                                    EditorGUILayout.LabelField("Press Space Key - Hide Preview");
                                    EditorGUILayout.EndVertical();
                                }
                                #endregion
                                GUI.DragWindow();

                            }, "Disable Face Editor", guiStyleEditorWindow);
                            editorConfigureEditorWindowRect = editorCommon.ResizeSceneViewRect(editorConfigureEditorWindowRect);
                        }
                        #endregion
                    }
                }

                #region Event
                {
                    var controlID = GUIUtility.GetControlID(FocusType.Passive);
                    Tools.current = Tool.None;
                    switch (e.type)
                    {
                    case EventType.MouseMove:
                        editVoxelList.Clear();
                        editorCommon.selectionRect.Reset();
                        editorCommon.ClearPreviewMesh();
                        UpdateCursorMesh();
                        break;
                    case EventType.MouseDown:
                        if (editorCommon.CheckMousePositionEditorRects())
                        {
                            if (!e.alt && e.button == 0)
                            {
                                editorCommon.ClearCursorMesh();
                                EventMouseDrag(true);
                            }
                            else if (!e.alt && e.button == 1)
                            {
                                ClearMakeAddData();
                            }
                        }
                        break;
                    case EventType.MouseDrag:
                        {
                            if (!e.alt && e.button == 0)
                            {
                                EventMouseDrag(false);
                            }
                        }
                        break;
                    case EventType.MouseUp:
                        if (!e.alt && e.button == 0)
                        {
                            EventMouseApply();
                        }
                        ClearMakeAddData();
                        UpdateCursorMesh();
                        repaint = true;
                        break;
                    case EventType.Layout:
                        HandleUtility.AddDefaultControl(controlID);
                        break;
                    }
                    switch (e.type)
                    {
                    case EventType.KeyDown:
                        if (!e.alt)
                        {
                            if (e.keyCode == KeyCode.F5)
                            {
                                Refresh();
                            }
                            else if (e.keyCode == KeyCode.Space)
                            {
                                drawEditorMesh = false;
                            }
                        }
                        break;
                    case EventType.KeyUp:
                        {
                            if (e.keyCode == KeyCode.Space)
                            {
                                drawEditorMesh = true;
                            }
                        }
                        break;
                    }
                }
                #endregion

                if (drawEditorMesh)
                {
                    #region SilhouetteMesh
                    if (editorCommon.silhouetteMesh != null)
                    {
                        for (int i = 0; i < editorCommon.silhouetteMesh.Length; i++)
                        {
                            if (editorCommon.silhouetteMesh[i] == null) continue;
                            editorCommon.unlitColorMaterial.color = new Color(0, 0, 0, 1f);
                            editorCommon.unlitColorMaterial.SetPass(0);
                            Graphics.DrawMeshNow(editorCommon.silhouetteMesh[i], baseTarget.transform.localToWorldMatrix);
                        }
                    }
                    #endregion

                    #region EnableMesh
                    if (baseTarget.edit_enableMesh != null)
                    {
                        for (int i = 0; i < baseTarget.edit_enableMesh.Length; i++)
                        {
                            if (baseTarget.edit_enableMesh[i] == null) continue;
                            if (baseTarget.edit_ConfigurePreviewMode == VoxelBase.Edit_ConfigurePreviewMode.Transparent)
                            {
                                editorCommon.vertexColorTransparentMaterial.color = new Color(1, 0, 0, 0.75f);
                                editorCommon.vertexColorTransparentMaterial.SetPass(0);
                            }
                            else
                            {
                                editorCommon.vertexColorMaterial.color = new Color(1, 0, 0, 1);
                                editorCommon.vertexColorMaterial.SetPass(0);
                            }
                            Graphics.DrawMeshNow(baseTarget.edit_enableMesh[i], baseTarget.transform.localToWorldMatrix);
                        }
                    }
                    #endregion
                }

                if (SceneView.currentDrawingSceneView == SceneView.lastActiveSceneView)
                {

                    #region Preview Mesh
                    if (editorCommon.previewMesh != null)
                    {
                        Color color = Color.white;
                        if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
                        {
                            if (baseTarget.edit_configureMaterialMainMode == VoxelBase.Edit_ConfigureMaterialMainMode.Add)
                                color = new Color(1, 0, 0, 1);
                            else if (baseTarget.edit_configureMaterialMainMode == VoxelBase.Edit_ConfigureMaterialMainMode.Remove)
                                color = new Color(0, 0, 1, 1);
                        }
                        else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
                        {
                            if (baseTarget.edit_configureDisableMainMode == VoxelBase.Edit_ConfigureDisableMainMode.Add)
                                color = new Color(1, 0, 0, 1);
                            else if (baseTarget.edit_configureDisableMainMode == VoxelBase.Edit_ConfigureDisableMainMode.Remove)
                                color = new Color(0, 0, 1, 1);
                        }
                        else
                        {
                            Assert.IsTrue(false);
                        }
                        color.a = 0.5f + 0.5f * (1f - editorCommon.AnimationPower);
                        for (int i = 0; i < editorCommon.previewMesh.Length; i++)
                        {
                            if (editorCommon.previewMesh[i] == null) continue;
                            editorCommon.vertexColorTransparentMaterial.color = color;
                            editorCommon.vertexColorTransparentMaterial.SetPass(0);
                            Graphics.DrawMeshNow(editorCommon.previewMesh[i], baseTarget.transform.localToWorldMatrix);
                        }
                        repaint = true;
                    }
                    #endregion

                    #region CursorMesh
                    {
                        float color = 0.2f + 0.4f * (1f - editorCommon.AnimationPower);
                        if (editorCommon.cursorMesh != null)
                        {
                            for (int i = 0; i < editorCommon.cursorMesh.Length; i++)
                            {
                                if (editorCommon.cursorMesh[i] == null) continue;
                                editorCommon.vertexColorTransparentMaterial.color = new Color(1, 1, 1, color);
                                editorCommon.vertexColorTransparentMaterial.SetPass(0);
                                Graphics.DrawMeshNow(editorCommon.cursorMesh[i], baseTarget.transform.localToWorldMatrix);
                            }
                        }
                        repaint = true;
                    }
                    #endregion

                    #region Selection Rect
                    if (editorCommon.selectionRect.Enable)
                    {
                        bool enable = false;
                        if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
                        {
                            if (baseTarget.edit_configureMaterialSubMode == VoxelBase.Edit_ConfigureMaterialSubMode.Rect)
                                enable = true;
                        }
                        else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
                        {
                            if (baseTarget.edit_configureDisableSubMode == VoxelBase.Edit_ConfigureDisableSubMode.Rect)
                                enable = true;
                        }
                        else
                        {
                            Assert.IsTrue(false);
                        }
                        if (enable)
                        {
                            Handles.BeginGUI();
                            GUI.Box(editorCommon.selectionRect.rect, "", "SelectionRect");
                            Handles.EndGUI();
                            repaint = true;
                        }
                    }
                    #endregion
                }
            }
            #endregion

            if (repaint)
            {
                SceneView.currentDrawingSceneView.Repaint();
            }
        }

        protected void UpdateSilhouetteMeshMesh()
        {
            editorCommon.ClearSilhouetteMeshMesh();

            if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material ||
                baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
            {
                List<VoxelData.Voxel> voxels = new List<VoxelData.Voxel>(baseCore.voxelData.voxels.Length);
                for (int i = 0; i < baseCore.voxelData.voxels.Length; i++)
                {
                    var voxel = baseCore.voxelData.voxels[i];
                    voxel.palette = -1;
                    voxels.Add(voxel);
                }
                if (voxels.Count > 0)
                {
                    editorCommon.silhouetteMesh = baseCore.Edit_CreateMesh(voxels, null, true);
                }
            }
        }
        protected void UpdatePreviewMesh()
        {
            editorCommon.ClearPreviewMesh();

            if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
            {
                if (baseTarget.edit_configureMaterialIndex > 0 && baseTarget.edit_configureMaterialIndex < baseTarget.materialData.Count)
                {
                    List<VoxelData.Voxel> voxels = new List<VoxelData.Voxel>();
                    editVoxelList.AllAction((x, y, z, face) =>
                    {
                        var index = baseCore.voxelData.VoxelTableContains(x, y, z);
                        if (index < 0) return;
                        var voxel = baseCore.voxelData.voxels[index];
                        voxel.palette = -1;
                        voxels.Add(voxel);
                    });
                    if (voxels.Count > 0)
                    {
                        editorCommon.previewMesh = baseCore.Edit_CreateMesh(voxels, null, false);
                    }
                }
            }
            else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
            {
                List<VoxelData.Voxel> voxels = new List<VoxelData.Voxel>();
                editVoxelList.AllAction((x, y, z, face) =>
                {
                    var index = baseCore.voxelData.VoxelTableContains(x, y, z);
                    if (index < 0) return;
                    var voxel = baseCore.voxelData.voxels[index];
                    voxel.palette = -1;
                    voxel.visible = face;
                    voxels.Add(voxel);
                });
                if (voxels.Count > 0)
                {
                    editorCommon.previewMesh = baseCore.Edit_CreateMesh(voxels, null, false);
                }
            }
        }
        protected void UpdateCursorMesh()
        {
            editorCommon.ClearCursorMesh();

            if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
            {
                if (baseTarget.edit_configureMaterialIndex > 0 && baseTarget.edit_configureMaterialIndex < baseTarget.materialData.Count)
                {
                    switch (baseTarget.edit_configureMaterialSubMode)
                    {
                    case VoxelBase.Edit_ConfigureMaterialSubMode.Voxel:
                        {
                            var result = editorCommon.GetMousePositionVoxel();
                            if (result.HasValue)
                            {
                                editorCommon.cursorMesh = baseCore.Edit_CreateMesh(new List<VoxelData.Voxel>() { new VoxelData.Voxel(result.Value.x, result.Value.y, result.Value.z, -1) });
                            }
                        }
                        break;
                    case VoxelBase.Edit_ConfigureMaterialSubMode.Fill:
                        {
                            var pos = editorCommon.GetMousePositionVoxel();
                            if (pos.HasValue)
                            {
                                var faceAreaTable = editorCommon.GetFillVoxelFaceAreaTable(pos.Value);
                                if (faceAreaTable != null)
                                    editorCommon.cursorMesh = new Mesh[1] { baseCore.Edit_CreateMeshOnly_Mesh(faceAreaTable, null, null) };
                            }
                        }
                        break;
                    }
                }
            }
            else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
            {
                switch (baseTarget.edit_configureDisableSubMode)
                {
                case VoxelBase.Edit_ConfigureDisableSubMode.Face:
                    {
                        IntVector3 pos;
                        VoxelBase.Face face;
                        if (editorCommon.GetMousePositionVoxelFace(out pos, out face))
                        {
                            editorCommon.cursorMesh = baseCore.Edit_CreateMesh(new List<VoxelData.Voxel>() { new VoxelData.Voxel(pos.x, pos.y, pos.z, -1, face) });
                        }
                    }
                    break;
                case VoxelBase.Edit_ConfigureDisableSubMode.Fill:
                    {
                        IntVector3 pos;
                        VoxelBase.Face face;
                        if (editorCommon.GetMousePositionVoxelFace(out pos, out face))
                        {
                            var faceAreaTable = editorCommon.GetFillVoxelFaceFaceAreaTable(pos, face);
                            if (faceAreaTable != null)
                                editorCommon.cursorMesh = new Mesh[1] { baseCore.Edit_CreateMeshOnly_Mesh(faceAreaTable, null, null) };
                        }
                    }
                    break;
                }
            }
        }

        protected void ClearMakeAddData()
        {
            editVoxelList.Clear();
            editorCommon.selectionRect.Reset();
            editorCommon.ClearPreviewMesh();
            editorCommon.ClearCursorMesh();
        }

        private void EventMouseDrag(bool first)
        {
            if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
            {
                UpdateCursorMesh();
                switch (baseTarget.edit_configureMaterialSubMode)
                {
                case VoxelBase.Edit_ConfigureMaterialSubMode.Voxel:
                    {
                        var result = editorCommon.GetMousePositionVoxel();
                        if (result.HasValue)
                        {
                            editVoxelList.Set(result.Value, VoxelBase.FaceAllFlags);
                            UpdatePreviewMesh();
                        }
                    }
                    break;
                case VoxelBase.Edit_ConfigureMaterialSubMode.Fill:
                    {
                        var pos = editorCommon.GetMousePositionVoxel();
                        if (pos.HasValue)
                        {
                            var result = editorCommon.GetFillVoxel(pos.Value);
                            if (result != null)
                            {
                                for (int i = 0; i < result.Count; i++)
                                    editVoxelList.Set(result[i], VoxelBase.FaceAllFlags);
                                UpdatePreviewMesh();
                            }
                        }
                    }
                    break;
                case VoxelBase.Edit_ConfigureMaterialSubMode.Rect:
                    {
                        var pos = new IntVector2((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
                        if (first) { editorCommon.selectionRect.Reset(); editorCommon.selectionRect.SetStart(pos); }
                        else editorCommon.selectionRect.SetEnd(pos);
                        //
                        editVoxelList.Clear();
                        {
                            var list = editorCommon.GetSelectionRectVoxel();
                            for (int i = 0; i < list.Count; i++)
                                editVoxelList.Set(list[i], VoxelBase.FaceAllFlags);
                        }
                        UpdatePreviewMesh();
                    }
                    break;
                }
            }
            else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
            {
                UpdateCursorMesh();
                switch (baseTarget.edit_configureDisableSubMode)
                {
                case VoxelBase.Edit_ConfigureDisableSubMode.Face:
                    {
                        IntVector3 pos;
                        VoxelBase.Face face;
                        if (editorCommon.GetMousePositionVoxelFace(out pos, out face))
                        {
                            var combineFace = editVoxelList.Get(pos);
                            combineFace |= face;
                            editVoxelList.Set(pos, combineFace);
                            UpdatePreviewMesh();
                        }
                    }
                    break;
                case VoxelBase.Edit_ConfigureDisableSubMode.Fill:
                    {
                        IntVector3 pos;
                        VoxelBase.Face face;
                        if (editorCommon.GetMousePositionVoxelFace(out pos, out face))
                        {
                            var result = editorCommon.GetFillVoxelFace(pos, face);
                            if (result != null)
                            {
                                for (int i = 0; i < result.Count; i++)
                                {
                                    var combineFace = editVoxelList.Get(result[i]);
                                    combineFace |= face;
                                    editVoxelList.Set(result[i], combineFace);
                                }
                                UpdatePreviewMesh();
                            }
                        }
                    }
                    break;
                case VoxelBase.Edit_ConfigureDisableSubMode.Rect:
                    {
                        var pos = new IntVector2((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
                        if (first) { editorCommon.selectionRect.Reset(); editorCommon.selectionRect.SetStart(pos); }
                        else editorCommon.selectionRect.SetEnd(pos);
                        //
                        editVoxelList.Clear();
                        {
                            var list = editorCommon.GetSelectionRectVoxelFace();
                            foreach (var pair in list)
                            {
                                var combineFace = editVoxelList.Get(pair.Key);
                                combineFace |= pair.Value;
                                editVoxelList.Set(pair.Key, combineFace);
                            }
                        }
                        UpdatePreviewMesh();
                    }
                    break;
                }
            }
        }
        private void EventMouseApply()
        {
            if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
            {
                Undo.RecordObject(baseTarget, "Material");

                bool update = false;
                if (baseTarget.edit_configureMaterialMainMode == VoxelBase.Edit_ConfigureMaterialMainMode.Add)
                {
                    editVoxelList.AllAction((x, y, z, face) =>
                    {
                        if (!update)
                            DisconnectPrefabInstance();

                        for (int i = 0; i < baseTarget.materialData.Count; i++)
                        {
                            if (i == baseTarget.edit_configureMaterialIndex) continue;
                            if (baseTarget.materialData[i].GetMaterial(new IntVector3(x, y, z)))
                            {
                                baseTarget.materialData[i].RemoveMaterial(new IntVector3(x, y, z));
                            }
                        }
                        baseTarget.materialData[baseTarget.edit_configureMaterialIndex].SetMaterial(new IntVector3(x, y, z));
                        update = true;
                    });
                }
                else if (baseTarget.edit_configureMaterialMainMode == VoxelBase.Edit_ConfigureMaterialMainMode.Remove)
                {
                    editVoxelList.AllAction((x, y, z, face) =>
                    {
                        if (baseTarget.materialData[baseTarget.edit_configureMaterialIndex].GetMaterial(new IntVector3(x, y, z)))
                        {
                            if (!update)
                                DisconnectPrefabInstance();

                            baseTarget.materialData[baseTarget.edit_configureMaterialIndex].RemoveMaterial(new IntVector3(x, y, z));
                            update = true;
                        }
                    });
                }
                else
                {
                    Assert.IsTrue(false);
                }
                if (update)
                {
                    UpdateConfigureEnableMesh();
                    baseTarget.edit_afterRefresh = true;
                }
                editVoxelList.Clear();
            }
            else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
            {
                Undo.RecordObject(baseTarget, "Disable Face");

                if (baseTarget.edit_configureDisableMainMode == VoxelBase.Edit_ConfigureDisableMainMode.Add)
                {
                    bool update = false;
                    editVoxelList.AllAction((x, y, z, face) =>
                    {
                        if (!update)
                            DisconnectPrefabInstance();

                        var pos = new IntVector3(x, y, z);
                        var combineFace = baseTarget.disableData.GetDisable(pos);
                        combineFace |= face;
                        if ((combineFace & VoxelBase.FaceAllFlags) != 0)
                            baseTarget.disableData.SetDisable(pos, combineFace);
                        else
                            baseTarget.disableData.RemoveDisable(pos);
                        update = true;
                    });
                    if (update)
                    {
                        UpdateConfigureEnableMesh();
                        baseTarget.edit_afterRefresh = true;
                    }
                }
                else if (baseTarget.edit_configureDisableMainMode == VoxelBase.Edit_ConfigureDisableMainMode.Remove)
                {
                    bool update = false;
                    editVoxelList.AllAction((x, y, z, face) =>
                    {
                        if (!update)
                            DisconnectPrefabInstance();

                        var pos = new IntVector3(x, y, z);
                        var combineFace = baseTarget.disableData.GetDisable(pos);
                        combineFace &= ~face;
                        if ((combineFace & VoxelBase.FaceAllFlags) != 0)
                            baseTarget.disableData.SetDisable(pos, combineFace);
                        else
                            baseTarget.disableData.RemoveDisable(pos);
                        update = true;
                    });
                    if (update)
                    {
                        UpdateConfigureEnableMesh();
                        baseTarget.edit_afterRefresh = true;
                    }
                }
                else
                {
                    Assert.IsTrue(false);
                }
                editVoxelList.Clear();
            }
        }

        private void ShowNotification()
        {
            SceneView.currentDrawingSceneView.ShowNotification(new GUIContent(string.Format("{0} - {1}", baseTarget.edit_configureMaterialMainMode, baseTarget.edit_configureMaterialSubMode)));
        }

        protected abstract List<Material> GetMaterialListMaterials();
        protected virtual void AddMaterialData(string name)
        {
            baseTarget.materialData.Add(new MaterialData() { name = name });
        }
        protected virtual void RemoveMaterialData(int index)
        {
            baseTarget.materialData.RemoveAt(index);
        }
        protected void UpdateMaterialList()
        {
            materialList = null;
            if (baseTarget.materialData == null) return;
            materialList = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("materialData"),
                false, true,
                !baseTarget.loadFromVoxelFile,
                !baseTarget.loadFromVoxelFile
            );
            materialList.elementHeight = 20;
            materialList.drawHeaderCallback = (rect) =>
            {
                Rect r = rect;
                EditorGUI.LabelField(r, "Name", EditorStyles.boldLabel);
                r.x = 182;
                var materials = GetMaterialListMaterials();
                if (materials != null)
                    EditorGUI.LabelField(r, "Material", EditorStyles.boldLabel);
                #region LoadFormVoxelFile
                r.x = r.width - 128;
                {
                    EditorGUI.BeginDisabledGroup(baseTarget.voxelData != null && baseTarget.voxelData.materials == null);
                    EditorGUI.BeginChangeCheck();
                    string tooltip = null;
                    if (baseTarget.voxelData == null)
                        tooltip = "Voxel data is not loaded.";
                    else if (baseTarget.voxelData.materials == null)
                        tooltip = "Material is not included in the voxel data.";
                    var loadFromVoxelFile = EditorGUI.ToggleLeft(r, new GUIContent("Load From Voxel File", tooltip), baseTarget.loadFromVoxelFile);
                    if (EditorGUI.EndChangeCheck())
                    {
                        UndoRecordObject("Inspector");
                        baseTarget.loadFromVoxelFile = loadFromVoxelFile;
                        EditorApplication.delayCall += () =>
                        {
                            if (baseTarget.loadFromVoxelFile)
                            {
                                baseCore.ClearVoxelData();
                                Refresh();
                            }
                            else
                            {
                                UpdateMaterialList();
                            }
                        };
                    }
                    EditorGUI.EndDisabledGroup();
                }
                #endregion
            };
            materialList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.yMin += 2;
                rect.yMax -= 2;
                if (index < baseTarget.materialData.Count)
                {
                    #region Name
                    {
                        Rect r = rect;
                        r.width = 144;
                        if (index == 0)
                        {
                            EditorGUI.LabelField(r, "default", guiStyleLabelMiddleLeftItalic);
                        }
                        else if (!baseTarget.loadFromVoxelFile)
                        {
                            EditorGUI.BeginChangeCheck();
                            string name = EditorGUI.TextField(r, baseTarget.materialData[index].name, guiStyleTextFieldMiddleLeft);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.materialData[index].name = name;
                            }
                        }
                        else
                        {
                            EditorGUI.LabelField(r, baseTarget.materialData[index].name, guiStyleLabelMiddleLeftItalic);
                        }
                    }
                    #endregion
                    #region Material
                    var materials = GetMaterialListMaterials();
                    if (materials != null && index < materials.Count)
                    {
                        {
                            Rect r = rect;
                            r.xMin = 182;
                            r.width = rect.width - r.xMin;
                            if (baseTarget.advancedMode)
                                r.width -= 64;
                            if (baseTarget.advancedMode && materials[index] != null && !EditorCommon.IsMainAsset(materials[index]))
                                r.width -= 48;
                            EditorGUI.BeginDisabledGroup(true);
                            EditorGUI.ObjectField(r, materials[index], typeof(Material), false);
                            EditorGUI.EndDisabledGroup();
                        }
                        if (baseTarget.advancedMode && materials[index] != null)
                        {
                            Rect r = rect;
                            r.xMin += rect.width - 46;
                            r.width = 48;
                            {
                                if (GUI.Button(r, "Reset"))
                                {
                                    #region Reset Material
                                    EditorApplication.delayCall += () =>
                                    {
                                        UndoRecordObject("Reset Material");
                                        materials[index] = EditorCommon.Instantiate(materials[index]);
                                        Refresh();
                                    };
                                    #endregion
                                }
                            }
                            if (!EditorCommon.IsMainAsset(materials[index]))
                            {
                                r.xMin -= 52;
                                r.width = 48;
                                if (GUI.Button(r, "Save"))
                                {
                                    #region Create Material
                                    string path = EditorUtility.SaveFilePanel("Save material", baseCore.GetDefaultPath(), string.Format("{0}_mat{1}.mat", baseTarget.gameObject.name, index), "mat");
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        if (path.IndexOf(Application.dataPath) < 0)
                                        {
                                            EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                        }
                                        else
                                        {
                                            EditorApplication.delayCall += () =>
                                            {
                                                UndoRecordObject("Save Material");
                                                path = FileUtil.GetProjectRelativePath(path);
                                                AssetDatabase.CreateAsset(Material.Instantiate(materials[index]), path);
                                                materials[index] = AssetDatabase.LoadAssetAtPath<Material>(path);
                                                Refresh();
                                            };
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion
                }
            };
            materialList.onSelectCallback = (list) =>
            {
                UndoRecordObject("Inspector");
                baseTarget.edit_configureMaterialIndex = list.index;
                UpdateConfigureEnableMesh();
                InternalEditorUtility.RepaintAllViews();
            };
            materialList.onAddCallback = (list) =>
            {
                var index = list.index;
                EditorApplication.delayCall += () =>
                {
                    UndoRecordObject("Inspector");
                    AddMaterialData(baseTarget.materialData.Count.ToString());
                    var materials = GetMaterialListMaterials();
                    if (materials != null)
                        materials.Add(null);
                    baseTarget.edit_configureMaterialIndex = index;
                    list.index = baseTarget.edit_configureMaterialIndex;
                    Refresh();
                    InternalEditorUtility.RepaintAllViews();
                };
            };
            materialList.onRemoveCallback = (list) =>
            {
                if (list.index > 0 && list.index < baseTarget.materialData.Count)
                {
                    var index = list.index;
                    EditorApplication.delayCall += () =>
                    {
                        UndoRecordObject("Inspector");
                        RemoveMaterialData(index);
                        var materials = GetMaterialListMaterials();
                        if (materials != null)
                            materials.RemoveAt(index);
                        baseTarget.edit_configureMaterialIndex = -1;
                        Refresh();
                        InternalEditorUtility.RepaintAllViews();
                    };
                }
            };
            if (baseTarget.edit_configureMaterialIndex >= 0 && baseTarget.materialData != null && baseTarget.edit_configureMaterialIndex < baseTarget.materialData.Count)
                materialList.index = baseTarget.edit_configureMaterialIndex;
            else
                baseTarget.edit_configureMaterialIndex = 0;
        }

        protected virtual void UpdateConfigureEnableMesh()
        {
            UpdateSilhouetteMeshMesh();
            if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Material)
                UpdateMaterialEnableMesh();
            else if (baseTarget.edit_configureMode == VoxelBase.Edit_ConfigureMode.Disable)
                UpdateDisableEnableMesh();
        }
        protected void UpdateMaterialEnableMesh()
        {
            if (baseTarget.materialData == null || baseCore.voxelData == null)
            {
                EditEnableMeshDestroy();
                return;
            }

            List<VoxelData.Voxel> voxels = new List<VoxelData.Voxel>(baseCore.voxelData.voxels.Length);
            if (baseTarget.edit_configureMaterialIndex == 0)
            {
                for (int i = 0; i < baseCore.voxelData.voxels.Length; i++)
                {
                    {
                        bool enable = true;
                        for (int j = 0; j < baseTarget.materialData.Count; j++)
                        {
                            if (baseTarget.materialData[j].GetMaterial(baseCore.voxelData.voxels[i].position))
                            {
                                enable = false;
                                break;
                            }
                        }
                        if (!enable) continue;
                    }
                    var voxel = baseCore.voxelData.voxels[i];
                    voxel.palette = -1;
                    voxels.Add(voxel);
                }
            }
            else if (baseTarget.edit_configureMaterialIndex >= 0 && baseTarget.edit_configureMaterialIndex < baseTarget.materialData.Count)
            {
                baseTarget.materialData[baseTarget.edit_configureMaterialIndex].AllAction((pos) =>
                {
                    var index = baseCore.voxelData.VoxelTableContains(pos);
                    if (index < 0) return;

                    var voxel = baseCore.voxelData.voxels[index];
                    voxel.palette = -1;
                    voxels.Add(voxel);
                });
            }
            baseTarget.edit_enableMesh = baseCore.Edit_CreateMesh(voxels);
        }
        protected void UpdateDisableEnableMesh()
        {
            if (baseTarget.disableData == null || baseCore.voxelData == null)
            {
                EditEnableMeshDestroy();
                return;
            }

            List<VoxelData.Voxel> voxels = new List<VoxelData.Voxel>(baseCore.voxelData.voxels.Length);
            {
                baseTarget.disableData.AllAction((pos, face) =>
                {
                    var index = baseCore.voxelData.VoxelTableContains(pos);
                    if (index < 0) return;

                    var voxel = baseCore.voxelData.voxels[index];
                    voxel.palette = -1;
                    voxel.visible = face;
                    voxels.Add(voxel);
                });
            }
            baseTarget.edit_enableMesh = baseCore.Edit_CreateMesh(voxels);
        }

        public void EditEnableMeshDestroy()
        {
            if (baseTarget.edit_enableMesh != null)
            {
                for (int i = 0; i < baseTarget.edit_enableMesh.Length; i++)
                {
                    MonoBehaviour.DestroyImmediate(baseTarget.edit_enableMesh[i]);
                }
                baseTarget.edit_enableMesh = null;
            }
        }

        protected void AfterRefresh()
        {
            if (AnimationMode.InAnimationMode())
            {
                return;
            }

            if (baseTarget.edit_afterRefresh)
                Refresh();
        }
        protected virtual void Refresh()
        {
            baseCore.ReCreate();

            UpdateMaterialList();
            UpdateConfigureEnableMesh();
        }

        #region PrefabCreate
        protected void DisconnectPrefabInstance()
        {
#if !UNITY_2018_3_OR_NEWER
            if (PrefabUtility.GetPrefabType(baseTarget) == PrefabType.PrefabInstance)
            {
                PrefabUtility.DisconnectPrefabInstance(baseTarget);
            }
#endif
        }

        protected class PrefabCreateMonitoringAssetModificationProcessor : UnityEditor.AssetModificationProcessor
        {
            public static bool IsContains(string path)
            {
                return paths.Contains(path);
            }
            public static void Remove(string path)
            {
                paths.Remove(path);
            }

            private static List<string> paths = new List<string>();

            private static bool delayCheck = false;

            private static void OnWillCreateAsset(string path)
            {
                if (Path.GetExtension(path) == ".prefab")
                {
                    paths.Add(path);

                    if (!delayCheck)
                    {
                        PrefabUtility.prefabInstanceUpdated += EditorPrefabInstanceUpdated;
                        EditorApplication.delayCall += () =>
                        {
                            CheckVoxelObject();
                            PrefabUtility.prefabInstanceUpdated -= EditorPrefabInstanceUpdated;
                            delayCheck = false;
                        };
                        delayCheck = true;
                    }
                }
            }
            private static void CheckVoxelObject()
            {
                List<string> removeList = new List<string>();
                foreach (var path in paths)
                {
                    var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    if (prefab == null)
                    {
                        removeList.Add(path);
                        continue;
                    }
                    var voxelBase = prefab.GetComponent<VoxelBase>();
                    if (voxelBase == null)
                    {
                        removeList.Add(path);
                        continue;
                    }
                }
                foreach (var path in removeList)
                {
                    paths.Remove(path);
                }
            }
        }

        protected static void EditorPrefabInstanceUpdated(GameObject go)
        {
#if UNITY_2018_3_OR_NEWER
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
            var path = AssetDatabase.GetAssetPath(prefab);
            if (!PrefabCreateMonitoringAssetModificationProcessor.IsContains(path))
                return;
            PrefabCreateMonitoringAssetModificationProcessor.Remove(path);

            var prefabType = PrefabUtility.GetPrefabAssetType(go);
            if (prefabType != PrefabAssetType.Regular && prefabType != PrefabAssetType.Variant)
                return;
            if (prefab.GetComponent<VoxelBase>() == null)
                return;

            Func<Component, bool> IsSelfComponent = (comp) =>
            {
                var subPrefab = PrefabUtility.GetCorrespondingObjectFromSource(comp);
                var subOriginalPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(comp);
                return subPrefab == subOriginalPrefab;
            };

            #region Children
            {
                var childrenVoxelBases = go.GetComponentsInChildren<VoxelBase>(true);
                foreach (var vb in childrenVoxelBases)
                {
                    if (!IsSelfComponent(vb)) continue;
                    VoxelBaseCore.StaticForceReCreate(vb);
                }
            }
            #endregion
            #region Extra
            {
                var childrenVoxelBaseExplosions = go.GetComponentsInChildren<VoxelBaseExplosion>(true);
                foreach (var vb in childrenVoxelBaseExplosions)
                {
                    if (!IsSelfComponent(vb)) continue;
                    VoxelBaseExplosionCore.StaticForceGenerate(vb);
                }
            }
            #endregion

            EditorApplication.delayCall += () =>
            {
                PrefabUtility.ApplyPrefabInstance(go, InteractionMode.AutomatedAction);
            };

#else
#if UNITY_2018_2_OR_NEWER
            var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go) as GameObject;
#else
            var prefab = PrefabUtility.GetPrefabParent(go) as GameObject;
#endif
            var path = AssetDatabase.GetAssetPath(prefab);
            if (!PrefabCreateMonitoringAssetModificationProcessor.IsContains(path))
                return;
            PrefabCreateMonitoringAssetModificationProcessor.Remove(path);

            var prefabType = PrefabUtility.GetPrefabType(go);
            if (prefabType != PrefabType.PrefabInstance)
                return;
            if (prefab.GetComponent<VoxelBase>() == null)
                return;

            #region Children
            {
                var childrenVoxelBases = go.GetComponentsInChildren<VoxelBase>(true);
                foreach (var vb in childrenVoxelBases)
                {
                    VoxelBaseCore.StaticForceReCreate(vb);
                }
            }
            #endregion
            #region Extra
            {
                var childrenVoxelBaseExplosions = go.GetComponentsInChildren<VoxelBaseExplosion>(true);
                foreach (var vb in childrenVoxelBaseExplosions)
                {
                    VoxelBaseExplosionCore.StaticForceGenerate(vb);
                }
            }
            #endregion

            EditorApplication.delayCall += () =>
            {
                PrefabUtility.ReplacePrefab(go, prefab, ReplacePrefabOptions.ConnectToPrefab);
            };
#endif
        }
        #endregion

        protected virtual void EditorUndoRedoPerformed()
        {
            if (AnimationMode.InAnimationMode())
            {
                baseTarget.edit_afterRefresh = true;
                return;
            }

            if (baseTarget != null && baseCore != null)
            {
                if (baseCore.RefreshCheckerCheck())
                {
                    if (baseTarget.importFlags != baseTarget.refreshChecker.importFlags)
                        baseCore.ReadyVoxelData(true);
                    Refresh();
                }
                else
                {
                    UpdateConfigureEnableMesh();
                }
            }
            Repaint();
        }
    }
}
