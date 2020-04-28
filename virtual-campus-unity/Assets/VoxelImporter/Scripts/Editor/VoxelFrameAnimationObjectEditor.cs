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
    [CustomEditor(typeof(VoxelFrameAnimationObject))]
    public class VoxelFrameAnimationObjectEditor : VoxelBaseEditor
    {
        public VoxelFrameAnimationObject objectTarget { get; protected set; }
        public VoxelFrameAnimationObjectCore objectCore { get; protected set; }

        protected ReorderableList frameList;

        protected override void OnEnable()
        {
            base.OnEnable();

            objectTarget = target as VoxelFrameAnimationObject;
            if (objectTarget == null) return;
            baseCore = objectCore = new VoxelFrameAnimationObjectCore(objectTarget);
            OnEnableInitializeSet();

            editorCommon.InitializeIcon();
            UpdateFrameList();

            AnimationUtility.onCurveWasModified -= EditorOnCurveWasModified;
            AnimationUtility.onCurveWasModified += EditorOnCurveWasModified;
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            AnimationUtility.onCurveWasModified -= EditorOnCurveWasModified;
        }

        protected override void InspectorGUI()
        {
            base.InspectorGUI();

            Event e = Event.current;

#if UNITY_2018_3_OR_NEWER
            {
                if (!objectCore.isPrefabEditable)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }
            }
#endif

            InspectorGUI_Import();

            #region Object
            if (!string.IsNullOrEmpty(baseTarget.voxelFilePath))
            {
                //Object
                baseTarget.edit_objectFoldout = EditorGUILayout.Foldout(baseTarget.edit_objectFoldout, "Object", guiStyleFoldoutBold);
                if (baseTarget.edit_objectFoldout)
                {
                    EditorGUILayout.BeginVertical(editorCommon.guiStyleSkinBox);
                    #region Mesh
                    if (baseTarget.advancedMode)
                    {
                        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        InspectorGUI_Object_Mesh_Settings();
                        EditorGUI.indentLevel--;
                    }
                    #endregion
                    #region Material
                    {
                        EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        #region updateMeshRendererMaterials
                        if (baseTarget.advancedMode)
                        {
                            EditorGUI.BeginChangeCheck();
                            var updateMeshRendererMaterials = EditorGUILayout.ToggleLeft("Update the Mesh Renderer Materials", baseTarget.updateMeshRendererMaterials);
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (EditorUtility.DisplayDialog("Update the Mesh Renderer Materials", "It will be changed.\nAre you sure?", "ok", "cancel"))
                                {
                                    UndoRecordObject("Inspector");
                                    baseTarget.updateMeshRendererMaterials = updateMeshRendererMaterials;
                                    baseCore.SetRendererCompornent();
                                }
                            }
                        }
                        #endregion
                        if (materialList != null)
                        {
                            materialList.DoLayoutList();
                        }
                        InspectorGUI_ConfigureMaterial();
                        EditorGUI.indentLevel--;
                    }
                    #endregion
                    #region Texture
                    if (baseTarget.advancedMode)
                    {
                        EditorGUILayout.LabelField("Texture", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        #region updateMaterialTexture
                        {
                            EditorGUI.BeginChangeCheck();
                            var updateMaterialTexture = EditorGUILayout.ToggleLeft("Update the Material Texture", baseTarget.updateMaterialTexture);
                            if (EditorGUI.EndChangeCheck())
                            {
                                if (EditorUtility.DisplayDialog("Update the Material Texture", "It will be changed.\nAre you sure?", "ok", "cancel"))
                                {
                                    UndoRecordObject("Inspector");
                                    baseTarget.updateMaterialTexture = updateMaterialTexture;
                                    baseCore.SetRendererCompornent();
                                }
                            }
                        }
                        #endregion
                        #region Texture
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.ObjectField(objectTarget.atlasTexture, typeof(Texture2D), false);
                                EditorGUI.EndDisabledGroup();
                            }
                            if (objectTarget.atlasTexture != null)
                            {
                                if (!EditorCommon.IsMainAsset(objectTarget.atlasTexture))
                                {
                                    if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                                    {
                                        #region Create Texture
                                        string path = EditorUtility.SaveFilePanel("Save atlas texture", baseCore.GetDefaultPath(), string.Format("{0}_tex.png", baseTarget.gameObject.name), "png");
                                        if (!string.IsNullOrEmpty(path))
                                        {
                                            if (path.IndexOf(Application.dataPath) < 0)
                                            {
                                                EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                            }
                                            else
                                            {
                                                UndoRecordObject("Save Atlas Texture");
                                                File.WriteAllBytes(path, objectTarget.atlasTexture.EncodeToPNG());
                                                path = FileUtil.GetProjectRelativePath(path);
                                                AssetDatabase.ImportAsset(path);
                                                objectCore.SetTextureImporterSetting(path, objectTarget.atlasTexture);
                                                objectTarget.atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                                                Refresh();
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                {
                                    if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                    {
                                        #region Reset Texture
                                        UndoRecordObject("Reset Atlas Texture");
                                        objectTarget.atlasTexture = null;
                                        Refresh();
                                        #endregion
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        #endregion
                        #region Generate Mip Maps
                        {
                            EditorGUI.BeginChangeCheck();
                            var generateMipMaps = EditorGUILayout.Toggle("Generate Mip Maps", baseTarget.generateMipMaps);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                baseTarget.generateMipMaps = generateMipMaps;
                                Refresh();
                            }
                        }
                        #endregion
                        #region Texture Size
                        {
                            EditorGUILayout.LabelField("Texture Size", objectTarget.atlasTexture != null ? string.Format("{0} x {1}", objectTarget.atlasTexture.width, objectTarget.atlasTexture.height) : "");
                        }
                        #endregion
                        EditorGUI.indentLevel--;
                    }
                    #endregion
                    EditorGUILayout.EndVertical();
                }
            }
            #endregion

            #region Animation
            if (!string.IsNullOrEmpty(baseTarget.voxelFilePath))
            {
                objectTarget.edit_animationFoldout = EditorGUILayout.Foldout(objectTarget.edit_animationFoldout, "Animation", guiStyleFoldoutBold);
                if (objectTarget.edit_animationFoldout)
                {
                    EditorGUILayout.BeginVertical(editorCommon.guiStyleSkinBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            {
                                if (objectTarget.frames == null || objectTarget.frames.Count == 0)
                                    EditorGUILayout.LabelField("Frame", guiStyleMagentaBold);
                                else
                                {
                                    bool contains = true;
                                    for (int i = 0; i < objectTarget.frames.Count; i++)
                                    {
                                        if (objectTarget.frames[i] == null || objectTarget.frames[i].mesh == null || !AssetDatabase.Contains(objectTarget.frames[i].mesh))
                                        {
                                            contains = false;
                                            break;
                                        }
                                    }
                                    EditorGUILayout.LabelField("Frame", contains ? EditorStyles.boldLabel : guiStyleRedBold);
                                }
                            }

                            Action<string> OpenFile = (path) =>
                            {
                                if (!baseCore.IsEnableFile(path))
                                    return;
                                UndoRecordObject("Open Voxel File", true);
                                UnityEngine.Object obj = null;
                                if (path.Contains(Application.dataPath))
                                {
                                    var assetPath = FileUtil.GetProjectRelativePath(path);
                                    var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                                    obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                                    if (obj != null)
                                    {
                                        bool done = false;
                                        if (obj is Texture2D)
                                        {
                                            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                                            if (importer != null && importer.spriteImportMode == SpriteImportMode.Multiple)
                                            {
                                                for (int j = 0; j < sprites.Length; j++)
                                                {
                                                    if (sprites[j] is Sprite)
                                                        objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = sprites[j], name = objectTarget.Edit_GetUniqueFrameName(sprites[j].name) });
                                                }
                                                done = true;
                                            }
                                        }
                                        else if (objectCore.GetFileType(path) == VoxelBase.FileType.vox)
                                        {
                                            var subCount = objectCore.GetVoxelFileSubCount(path);
                                            if (subCount > 1)
                                            {
                                                for (int i = 0; i < subCount; i++)
                                                {
                                                    objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = obj, voxelFileSubIndex = i, name = objectTarget.Edit_GetUniqueFrameName(string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(path), i)) });
                                                }
                                                done = true;
                                            }
                                        }
                                        if (!done)
                                        {
                                            objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = obj, name = objectTarget.Edit_GetUniqueFrameName(obj.name) });
                                        }
                                    }
                                }
                                else
                                {
                                    bool done = false;
                                    if (objectCore.GetFileType(path) == VoxelBase.FileType.vox)
                                    {
                                        var subCount = objectCore.GetVoxelFileSubCount(path);
                                        if (subCount > 1)
                                        {
                                            for (int i = 0; i < subCount; i++)
                                            {
                                                objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileSubIndex = i, name = objectTarget.Edit_GetUniqueFrameName(string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(path), i)) });
                                            }
                                            done = true;
                                        }
                                    }
                                    if (!done)
                                    {
                                        objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, name = objectTarget.Edit_GetUniqueFrameName(Path.GetFileNameWithoutExtension(path)) });
                                    }
                                }
                                objectCore.ReCreate();
                            };

                            var rect = GUILayoutUtility.GetRect(new GUIContent("Open"), guiStyleDropDown, GUILayout.Width(64));
                            if (GUI.Button(rect, "Open", guiStyleDropDown))
                            {
                                GenericMenu menu = new GenericMenu();
                                #region vox
                                menu.AddItem(new GUIContent("MagicaVoxel (*.vox)"), false, () =>
                                {
                                    var path = EditorUtility.OpenFilePanel("Open MagicaVoxel File", !string.IsNullOrEmpty(baseTarget.voxelFilePath) ? Path.GetDirectoryName(baseTarget.voxelFilePath) : "", "vox");
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        OpenFile(path);
                                    }
                                });
                                #endregion
                                #region qb
                                menu.AddItem(new GUIContent("Qubicle Binary (*.qb)"), false, () =>
                                {
                                    var path = EditorUtility.OpenFilePanel("Open Qubicle Binary File", !string.IsNullOrEmpty(baseTarget.voxelFilePath) ? Path.GetDirectoryName(baseTarget.voxelFilePath) : "", "qb");
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        OpenFile(path);
                                    }
                                });
                                #endregion
                                #region png
                                menu.AddItem(new GUIContent("Pixel Art (*.png)"), false, () =>
                                {
                                    var path = EditorUtility.OpenFilePanel("Open Pixel Art File", !string.IsNullOrEmpty(baseTarget.voxelFilePath) ? Path.GetDirectoryName(baseTarget.voxelFilePath) : "", "png");
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        OpenFile(path);
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
                                    if (DragAndDrop.paths.Length == 0) break;
                                    DragAndDrop.AcceptDrag();
                                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                                    if (e.type == EventType.DragPerform)
                                    {
                                        UndoRecordObject("Open Voxel File", true);
                                        if (DragAndDrop.objectReferences.Length > 0)
                                        {
                                            for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                                            {
                                                var obj = DragAndDrop.objectReferences[i];
                                                var assetPath = AssetDatabase.GetAssetPath(obj);
                                                var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                                                string path = assetPath;
                                                if (!baseCore.IsEnableFile(path))
                                                    continue;
                                                if (Path.GetPathRoot(path) == "")
                                                    path = EditorCommon.GetProjectRelativePath2FullPath(assetPath);
                                                bool done = false;
                                                if (obj is Texture2D)
                                                {
                                                    TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                                                    if (importer != null && importer.spriteImportMode == SpriteImportMode.Multiple)
                                                    {
                                                        for (int j = 0; j < sprites.Length; j++)
                                                        {
                                                            if (sprites[j] is Sprite)
                                                                objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = sprites[j], name = objectTarget.Edit_GetUniqueFrameName(sprites[j].name) });
                                                        }
                                                        done = true;
                                                    }
                                                }
                                                else if (objectCore.GetFileType(path) == VoxelBase.FileType.vox)
                                                {
                                                    var subCount = objectCore.GetVoxelFileSubCount(path);
                                                    if (subCount > 1)
                                                    {
                                                        for (int j = 0; j < subCount; j++)
                                                        {
                                                            objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = obj, voxelFileSubIndex = j, name = objectTarget.Edit_GetUniqueFrameName(string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(path), j)) });
                                                        }
                                                        done = true;
                                                    }
                                                }
                                                if (!done)
                                                {
                                                    objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = obj, name = objectTarget.Edit_GetUniqueFrameName(obj.name) });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (int i = 0; i < DragAndDrop.paths.Length; i++)
                                            {
                                                string path = DragAndDrop.paths[i];
                                                if (Path.GetPathRoot(path) == "")
                                                    path = EditorCommon.GetProjectRelativePath2FullPath(DragAndDrop.paths[i]);
                                                if (!baseCore.IsEnableFile(path))
                                                    continue;
                                                bool done = false;
                                                if (objectCore.GetFileType(path) == VoxelBase.FileType.vox)
                                                {
                                                    var subCount = objectCore.GetVoxelFileSubCount(path);
                                                    if (subCount > 1)
                                                    {
                                                        for (int j = 0; j < subCount; j++)
                                                        {
                                                            objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileSubIndex = j, name = objectTarget.Edit_GetUniqueFrameName(string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(path), j)) });
                                                        }
                                                        done = true;
                                                    }
                                                }
                                                if (!done)
                                                {
                                                    objectTarget.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, name = objectTarget.Edit_GetUniqueFrameName(Path.GetFileNameWithoutExtension(path)) });
                                                }
                                            }
                                        }
                                        objectCore.ReCreate();
                                        e.Use();
                                    }
                                    break;
                                }
                            }
                            #endregion
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    {
                        EditorGUI.indentLevel++;
                        {
                            IconRender();
                            if (frameList != null)
                            {
                                frameList.DoLayoutList();
                            }
#if UNITY_2018_3_OR_NEWER
                            {
                                if (!objectCore.isPrefabEditable)
                                {
                                    EditorGUI.EndDisabledGroup();
                                }
                            }
#endif
                            FrameListWindowGUI();
#if UNITY_2018_3_OR_NEWER
                            {
                                if (!objectCore.isPrefabEditable)
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                }
                            }
#endif
                        }
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.Space();
                    #region HelpBox
                    {
                        {
                            HashSet<string> helpList = new HashSet<string>();
                            {
                                if (objectTarget.frames != null)
                                {
                                    for (int i = 0; i < objectTarget.frames.Count; i++)
                                    {
                                        if (objectTarget.frames[i] == null || objectTarget.frames[i].mesh == null || !AssetDatabase.Contains(objectTarget.frames[i].mesh))
                                        {
                                            helpList.Add("Mesh");
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    helpList.Add("Mesh");
                                }
                            }
                            if (helpList.Count > 0)
                            {
                                EditorGUILayout.HelpBox(EditorCommon.GetHelpStrings(new List<string>(helpList)), MessageType.Error);
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.Space();
                                if (GUILayout.Button("Save All Unsaved Assets"))
                                {
                                    SaveAllUnsavedAssets(new MenuCommand(baseTarget));
                                }
                                EditorGUILayout.Space();
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.Space();
                            }
                        }
                    }
                    #endregion
                    EditorGUILayout.EndVertical();
                }
            }
            #endregion

            InspectorGUI_Refresh();

#if UNITY_2018_3_OR_NEWER
            {
                if (!objectCore.isPrefabEditable)
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
#endif
        }
        protected override bool IsEnableConfigureDisable()
        {
            return objectTarget.edit_frameIndex >= 0;
        }
        protected override void BeginConfigureDisable()
        {
            objectCore.ReadyIndividualVoxelData();
        }
        protected override bool IsEnableConfigureMaterial()
        {
            return objectTarget.edit_frameIndex >= 0;
        }
        protected override void BeginConfigureMaterial()
        {
            objectCore.ReadyIndividualVoxelData();
        }
        protected void FrameListWindowGUI()
        {
            try
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUI.BeginChangeCheck();
                GUILayout.Toggle(VoxelFrameAnimationListWindow.instance != null, "Frame List Window", GUI.skin.button);
                if (EditorGUI.EndChangeCheck())
                {
                    if (VoxelFrameAnimationListWindow.instance == null)
                    {
                        VoxelFrameAnimationListWindow.Create(objectTarget);
                        VoxelFrameAnimationListWindow.instance.frameIndexChanged += () =>
                        {
                            if (objectTarget.edit_frameEnable)
                                frameList.index = objectTarget.edit_frameIndex;
                            else
                                objectTarget.edit_frameIndex = -1;
                            if(frameList != null)
                            {
                                frameList.index = objectTarget.edit_frameIndex;
                            }
                            UpdateConfigureEnableMesh();
                            objectCore.SetCurrentMesh();
                            EditorApplication.delayCall += () =>
                            {
                                InternalEditorUtility.RepaintAllViews();
                            };
                        };
                        VoxelFrameAnimationListWindow.instance.previewCameraModeChanged += () =>
                        {
                            objectCore.ClearFramesIcon();
                        };
                    }
                    else
                    {
                        VoxelFrameAnimationListWindow.instance.Close();
                    }
                }
                EditorGUILayout.Space();
            }
            catch
            {

            }
            finally
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        protected override List<Material> GetMaterialListMaterials()
        {
            return objectTarget.materials;
        }
        protected override void AddMaterialData(string name)
        {
            for (int i = 0; i < objectTarget.frames.Count; i++)
            {
                objectTarget.frames[i].materialData.Add(new MaterialData() { name = name });
            }
        }
        protected override void RemoveMaterialData(int index)
        {
            for (int i = 0; i < objectTarget.frames.Count; i++)
            {
                objectTarget.frames[i].materialData.RemoveAt(index);
            }
        }

        protected void SetPreviewCameraTransform(Bounds bounds)
        {
            var transform = editorCommon.iconCamera.transform;
            var sizeMax = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
            switch (objectTarget.edit_previewCameraMode)
            {
            case VoxelFrameAnimationObject.Edit_CameraMode.forward:
                {
                    var rot = Quaternion.AngleAxis(180f, Vector3.up);
                    transform.localRotation = rot;
                    sizeMax = Mathf.Max(bounds.size.x, bounds.size.y);
                    transform.localPosition = new Vector3(bounds.center.x, bounds.center.y, bounds.max.z) - transform.forward;
                }
                break;
            case VoxelFrameAnimationObject.Edit_CameraMode.back:
                {
                    transform.localRotation = Quaternion.identity;
                    sizeMax = Mathf.Max(bounds.size.x, bounds.size.y);
                    transform.localPosition = new Vector3(bounds.center.x, bounds.center.y, bounds.min.z) - transform.forward;
                }
                break;
            case VoxelFrameAnimationObject.Edit_CameraMode.up:
                {
                    var rot = Quaternion.AngleAxis(90f, Vector3.right);
                    transform.localRotation = rot;
                    sizeMax = Mathf.Max(bounds.size.x, bounds.size.z);
                    transform.localPosition = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z) - transform.forward;
                }
                break;
            case VoxelFrameAnimationObject.Edit_CameraMode.down:
                {
                    var rot = Quaternion.AngleAxis(-90f, Vector3.right);
                    transform.localRotation = rot;
                    sizeMax = Mathf.Max(bounds.size.x, bounds.size.z);
                    transform.localPosition = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z) - transform.forward;
                }
                break;
            case VoxelFrameAnimationObject.Edit_CameraMode.right:
                {
                    var rot = Quaternion.AngleAxis(-90f, Vector3.up);
                    transform.localRotation = rot;
                    sizeMax = Mathf.Max(bounds.size.y, bounds.size.z);
                    transform.localPosition = new Vector3(bounds.max.x, bounds.center.y, bounds.center.z) - transform.forward;
                }
                break;
            case VoxelFrameAnimationObject.Edit_CameraMode.left:
                {
                    var rot = Quaternion.AngleAxis(90f, Vector3.up);
                    transform.localRotation = rot;
                    sizeMax = Mathf.Max(bounds.size.y, bounds.size.z);
                    transform.localPosition = new Vector3(bounds.min.x, bounds.center.y, bounds.center.z) - transform.forward;
                }
                break;
            }

            var camera = editorCommon.iconCamera.GetComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = sizeMax * 0.6f;
            camera.farClipPlane = 1f + sizeMax * 5f;
        }

        protected void UpdateFrameList()
        {
            frameList = null;
            if (objectTarget.frames == null) return;
            frameList = new ReorderableList(
                serializedObject,
                serializedObject.FindProperty("frames"),
                true, true, false, true
            );
            frameList.elementHeight = 40;
            frameList.drawHeaderCallback = (rect) =>
            {
                {
                    Rect r = rect;
                    {
                        r.x -= 16;
                        r.width = 80;
                        EditorGUI.BeginChangeCheck();
                        var edit_previewCameraMode = (VoxelFrameAnimationObject.Edit_CameraMode)EditorGUI.EnumPopup(r, objectTarget.edit_previewCameraMode);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(objectTarget, "Camera Mode");
                            objectTarget.edit_previewCameraMode = edit_previewCameraMode;
                            objectCore.ClearFramesIcon();
                            if (VoxelFrameAnimationListWindow.instance != null)
                                VoxelFrameAnimationListWindow.instance.Repaint();
                        }
                    }
                    r.x += 16 + frameList.elementHeight + 12;
                    r.width = rect.width - r.width;
                    EditorGUI.LabelField(r, "Object & Mesh", EditorStyles.boldLabel);
                }
                {
                    Rect r = rect;
                    r.x += r.width - 100;
                    r.width = 100;
                    if (GUI.Button(r, "Select None"))
                    {
                        frameList.index = objectTarget.edit_frameIndex = -1;
                        UpdateConfigureEnableMesh();
                        objectCore.SetCurrentMesh();
                    }
                }
            };
            frameList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                rect.yMin += 2;
                rect.yMax -= 2;
                if (index < objectTarget.frames.Count && objectTarget.frames[index] != null)
                {
                    Rect r = rect;
                    #region Icon
                    r.width = frameList.elementHeight - 2;
                    r.height = frameList.elementHeight - 2;
                    if (objectTarget.frames[index].icon != null)
                        GUI.DrawTexture(r, objectTarget.frames[index].icon);
                    r.x += r.width + 2;
                    #endregion
                    r.width = rect.width - (r.x - rect.x);
                    r.height = 16;
                    #region Name
                    {
                        EditorGUI.BeginChangeCheck();
                        var name = EditorGUI.TextField(r, objectTarget.frames[index].name);
                        if (EditorGUI.EndChangeCheck())
                        {
                            UndoRecordObject("Change Frame Name");
                            objectTarget.frames[index].name = objectTarget.Edit_GetUniqueFrameName(name);
                        }
                    }
                    #endregion
                    #region Object
                    {
                        const int FrameIndexWidth = 32;
                        Rect rs = r;
                        rs.width /= 2;
                        rs.y += 18 + 2;
                        rs.width -= FrameIndexWidth;
                        if (objectTarget.frames[index].voxelFileObject != null)
                        {
                            EditorGUI.BeginChangeCheck();
                            var obj = EditorGUI.ObjectField(rs, objectTarget.frames[index].voxelFileObject, typeof(UnityEngine.Object), false);
                            if (EditorGUI.EndChangeCheck())
                            {
                                var path = EditorCommon.GetProjectRelativePath2FullPath(AssetDatabase.GetAssetPath(obj));
                                if (baseCore.IsEnableFile(path))
                                {
                                    UndoRecordObject("Change Frame Voxel File");
                                    objectTarget.frames[index].voxelFileObject = obj;
                                    objectTarget.frames[index].voxelFileSubIndex = 0;
                                    objectCore.ClearFrameVoxelData(index);
                                    Refresh();
                                }
                            }
                        }
                        else
                        {
                            EditorGUI.LabelField(rs, Path.GetFileName(objectTarget.frames[index].voxelFilePath));
                        }
                        rs.x += rs.width;
                        rs.width = FrameIndexWidth;
                        EditorGUI.LabelField(rs, new GUIContent(objectTarget.frames[index].voxelFileSubIndex.ToString(), "Frame Index"));
                    }
                    #endregion
                    #region Mesh
                    {
                        Rect rs = r;
                        rs.width /= 2;
                        rs.x += rs.width;
                        rs.y += 18 + 2;
                        if (baseTarget.advancedMode)
                            rs.width -= 104;
                        if (baseTarget.advancedMode && EditorCommon.IsMainAsset(objectTarget.frames[index].mesh))
                            rs.width += 48;
                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUI.ObjectField(rs, objectTarget.frames[index].mesh, typeof(Mesh), false);
                        EditorGUI.EndDisabledGroup();
                        rs.x += rs.width + 4;
                        if (baseTarget.advancedMode && EditorCommon.IsMainAsset(objectTarget.frames[index].mesh))
                            rs.x -= 48;
                        if (baseTarget.advancedMode && objectTarget.frames[index].mesh != null)
                        {
                            rs.width = 48;
                            rs.height = 16;
                            if (!EditorCommon.IsMainAsset(objectTarget.frames[index].mesh))
                            {
                                if (GUI.Button(rs, "Save"))
                                {
                                    #region Create Mesh
                                    var name = objectTarget.frames[index].voxelFileObject != null ? objectTarget.frames[index].voxelFileObject.name : Path.GetFileNameWithoutExtension(objectTarget.frames[index].voxelFilePath);
                                    var path = EditorUtility.SaveFilePanel("Save mesh", objectCore.GetDefaultPath(), string.Format("{0}_mesh_{1}_{2}.asset", baseTarget.gameObject.name, name, objectTarget.frames[index].voxelFileSubIndex), "asset");
                                    if (!string.IsNullOrEmpty(path))
                                    {
                                        if (path.IndexOf(Application.dataPath) < 0)
                                        {
                                            EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                        }
                                        else
                                        {
                                            UndoRecordObject("Save Mesh");
                                            path = FileUtil.GetProjectRelativePath(path);
                                            var oldObj = objectTarget.frames[index].mesh;
                                            AssetDatabase.CreateAsset(Mesh.Instantiate(objectTarget.frames[index].mesh), path);
                                            objectTarget.frames[index].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                                            if (objectTarget.mesh == oldObj)
                                                objectTarget.mesh = objectTarget.frames[index].mesh;
                                            Refresh();
                                            objectCore.SwapAnimationObjectReference(oldObj, objectTarget.frames[index].mesh);
                                            InternalEditorUtility.RepaintAllViews();
                                        }
                                    }
                                    #endregion
                                }
                            }
                            rs.x += rs.width + 4;
                            if (GUI.Button(rs, "Reset"))
                            {
                                #region Reset Mesh
                                UndoRecordObject("Reset Mesh");
                                var oldObj = objectTarget.frames[index].mesh;
                                objectTarget.frames[index].mesh = null;
                                if (objectTarget.mesh == oldObj)
                                    objectTarget.mesh = null;
                                Refresh();
                                objectCore.SwapAnimationObjectReference(oldObj, objectTarget.frames[index].mesh);
                                InternalEditorUtility.RepaintAllViews();
                                #endregion
                            }
                        }
                    }
                    #endregion
                }
            };
            frameList.onSelectCallback = (list) =>
            {
                objectTarget.edit_frameIndex = list.index;
                UpdateConfigureEnableMesh();
                objectCore.SetCurrentMesh();
                if (VoxelFrameAnimationListWindow.instance != null)
                    VoxelFrameAnimationListWindow.instance.FrameIndexChanged();
                EditorApplication.delayCall += () =>
                {
                    InternalEditorUtility.RepaintAllViews();
                };
            };
            frameList.onChangedCallback = (list) =>
            {
                objectCore.ReadyVoxelData(true);
                objectCore.ClearFramesIcon();
                if (VoxelFrameAnimationListWindow.instance != null)
                    VoxelFrameAnimationListWindow.instance.Repaint();
            };
            frameList.onRemoveCallback = (list) =>
            {
                if (list.index >= 0)
                {
                    UndoRecordObject("Remove Frame");
                    objectTarget.frames.RemoveAt(list.index);
                    if (list.index < objectTarget.edit_frameIndex)
                        objectTarget.edit_frameIndex--;
                    objectCore.SetCurrentMesh();
                    Refresh();
                    if (VoxelFrameAnimationListWindow.instance != null)
                        VoxelFrameAnimationListWindow.instance.Repaint();
                }
            };

            objectTarget.edit_frameIndex = -1;
            for (int i = 0; i < objectTarget.frames.Count; i++)
            {
                if(objectTarget.mesh == objectTarget.frames[i].mesh)
                {
                    objectTarget.edit_frameIndex = i;
                    break;
                }
            }
            frameList.index = objectTarget.edit_frameIndex;
        }

        public void IconRender()
        {
            if (objectTarget.frames == null) return;
            foreach (var frame in objectTarget.frames)
            {
                if (frame.icon == null && frame.materialIndexes != null)
                {
                    Material[] materials = new Material[frame.materialIndexes.Count];
                    for (int j = 0; j < frame.materialIndexes.Count; j++)
                    {
                        var mindex = frame.materialIndexes[j];
                        if (objectTarget.materials == null || mindex >= objectTarget.materials.Count) continue;
                        materials[j] = objectTarget.materials[mindex];
                    }
                    var mesh = frame.mesh;
                    if (mesh != null && mesh.subMeshCount > 0)
                    {
                        var bounds = mesh.bounds;
                        {
                            var size = bounds.size;
                            for (int i = 0; i < 3; i++)
                            {
                                if (size[i] <= 0f)
                                    size[i] = 1f;
                            }
                            bounds.size = size;
                        }
                        editorCommon.CreateIconObject(objectTarget.transform, mesh, materials);
                        SetPreviewCameraTransform(bounds);
                        frame.icon = editorCommon.IconObjectRender();
                    }
                }
            }
        }

        protected override void UpdateConfigureEnableMesh()
        {
            if (baseTarget.edit_configureMode != VoxelBase.Edit_ConfigureMode.None)
            {
                if (objectTarget.voxelData == null)
                {
                    objectCore.ReadyVoxelData();
                }
                objectTarget.Edit_SetFrameCurrentVoxelOtherData();
            }
            base.UpdateConfigureEnableMesh();
        }

        private int editorOnCurveWasModifiedCount;
        private void EditorOnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted)
        {
            if (editorOnCurveWasModifiedCount++ == 0)
            {
                if (deleted == AnimationUtility.CurveModifiedType.CurveModified)
                {
                    if (binding.type == typeof(MeshRenderer))
                    {
                        //AnimationUtility.SetObjectReferenceCurve(clip, binding, null);    So it will be back in this useless.
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, new ObjectReferenceKeyframe[0]);
                    }
                }
            }
            editorOnCurveWasModifiedCount--;
        }

        protected override void Refresh()
        {
            base.Refresh();

            UpdateFrameList();
        }

        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Save All Unsaved Assets")]
        private static void SaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelFrameAnimationObjectCore(objectTarget);

            var folder = EditorUtility.OpenFolderPanel("Save all", objectCore.GetDefaultPath(), null);
            if (string.IsNullOrEmpty(folder)) return;
            if (folder.IndexOf(Application.dataPath) < 0)
            {
                EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                return;
            }

            Undo.RecordObject(objectTarget, "Save All Unsaved Assets");

            #region Material
            if (objectTarget.materials != null)
            {
                for (int index = 0; index < objectTarget.materials.Count; index++)
                {
                    if (objectTarget.materials[index] == null || EditorCommon.IsMainAsset(objectTarget.materials[index])) continue;
                    var path = folder + "/" + string.Format("{0}_mat{1}.mat", objectTarget.gameObject.name, index);
                    path = FileUtil.GetProjectRelativePath(path);
                    path = AssetDatabase.GenerateUniqueAssetPath(path);
                    AssetDatabase.CreateAsset(Material.Instantiate(objectTarget.materials[index]), path);
                    objectTarget.materials[index] = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
            }
            #endregion

            #region Texture
            if (objectTarget.atlasTexture != null && !EditorCommon.IsMainAsset(objectTarget.atlasTexture))
            {
                var path = folder + "/" + string.Format("{0}_tex.png", objectTarget.gameObject.name);
                path = EditorCommon.GenerateUniqueAssetFullPath(path);
                File.WriteAllBytes(path, objectTarget.atlasTexture.EncodeToPNG());
                path = FileUtil.GetProjectRelativePath(path);
                AssetDatabase.ImportAsset(path);
                objectCore.SetTextureImporterSetting(path, objectTarget.atlasTexture);
                objectTarget.atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            #endregion

            #region Mesh
            if (objectTarget.frames != null)
            {
                for (int i = 0; i < objectTarget.frames.Count; i++)
                {
                    if (objectTarget.frames[i].mesh != null && !EditorCommon.IsMainAsset(objectTarget.frames[i].mesh))
                    {
                        var name = objectTarget.frames[i].voxelFileObject != null ? objectTarget.frames[i].voxelFileObject.name : Path.GetFileNameWithoutExtension(objectTarget.frames[i].voxelFilePath);
                        var path = folder + "/" + string.Format("{0}_mesh_{1}_{2}.asset", objectTarget.gameObject.name, name, objectTarget.frames[i].voxelFileSubIndex);
                        path = FileUtil.GetProjectRelativePath(path);
                        path = AssetDatabase.GenerateUniqueAssetPath(path);
                        var oldObj = objectTarget.frames[i].mesh;
                        AssetDatabase.CreateAsset(Mesh.Instantiate(objectTarget.frames[i].mesh), path);
                        objectTarget.frames[i].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                        if (objectTarget.mesh == oldObj)
                            objectTarget.mesh = objectTarget.frames[i].mesh;
                        objectCore.SwapAnimationObjectReference(oldObj, objectTarget.frames[i].mesh);
                    }
                }
            }
            #endregion

            objectCore.ReCreate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Save All Unsaved Assets", true)]
        private static bool IsValidateSaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }

        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Reset All Assets")]
        private static void ResetAllSavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelFrameAnimationObjectCore(objectTarget);

            Undo.RecordObject(objectTarget, "Reset All Assets");
            
            #region Mesh
            List<UnityEngine.Object> oldFramesMesh = null;
            if (objectTarget.frames != null)
            {
                oldFramesMesh = new List<UnityEngine.Object>(objectTarget.frames.Count);
                for (int i = 0; i < objectTarget.frames.Count; i++)
                {
                    oldFramesMesh.Add(objectTarget.frames[i].mesh);
                }
            }
            #endregion

            objectCore.ResetAllAssets();
            objectCore.ReCreate();

            #region Mesh
            if (oldFramesMesh != null)
            {
                for (int i = 0; i < oldFramesMesh.Count; i++)
                {
                    objectCore.SwapAnimationObjectReference(oldFramesMesh[i], objectTarget.frames[i].mesh);
                }
            }
            #endregion

            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Reset All Assets", true)]
        private static bool IsValidateResetAllSavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }

        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Export COLLADA(dae) File", false, 10000)]
        private static void ExportDaeFile(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelFrameAnimationObjectCore(objectTarget);

            string path = EditorUtility.SaveFilePanel("Export COLLADA(dae) File", objectCore.GetDefaultPath(), string.Format("{0}.dae", Path.GetFileNameWithoutExtension(objectTarget.voxelFilePath)), "dae");
            if (string.IsNullOrEmpty(path)) return;

            if (!objectCore.ExportDaeFile(path))
            {
                Debug.LogErrorFormat("<color=green>[Voxel Importer]</color> Export COLLADA(dae) File error. file:{0}", path);
            }
        }
        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Export COLLADA(dae) File", true)]

        private static bool IsValidateExportDaeFile(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return false;

#if UNITY_2018_3_OR_NEWER
            return true;
#else
            return PrefabUtility.GetPrefabType(objectTarget) != PrefabType.Prefab;
#endif
        }
        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Export Animation Curve (Clip)", false, 10001)]
        private static void ExportAnimationClip(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelFrameAnimationObjectCore(objectTarget);

            string path = EditorUtility.SaveFilePanel("If you specify an existing Clip as an overwrite, only AnimationCurve is updated and other information is retained.", objectCore.GetDefaultPath(), string.Format("{0}.anim", Path.GetFileNameWithoutExtension(objectTarget.voxelFilePath)), "anim");
            if (string.IsNullOrEmpty(path)) return;
            if (path.IndexOf(Application.dataPath) < 0)
            {
                EditorCommon.SaveInsideAssetsFolderDisplayDialog();
            }
            else
            {
                AnimationClip clip = null;
                if (File.Exists(path))
                {
                    //Edit
                    var assetPath = FileUtil.GetProjectRelativePath(path);
                    clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                }
                else
                {
                    //New
                    clip = new AnimationClip();
                    clip.frameRate = 10;
                    {
                        var settings = AnimationUtility.GetAnimationClipSettings(clip);
                        settings.loopTime = true;
                        AnimationUtility.SetAnimationClipSettings(clip, settings);
                    }
                }
                Func<int, float> FrameToTime = (frame) =>
                {
                    return Mathf.Round((frame * (1f / clip.frameRate)) * clip.frameRate) / clip.frameRate;
                };
                #region Mesh
                {
                    var keys = new ObjectReferenceKeyframe[objectTarget.frames.Count];
                    for (int i = 0; i < objectTarget.frames.Count; i++)
                    {
                        keys[i] = new ObjectReferenceKeyframe()
                        {
                            time = FrameToTime(i),
                            value = objectTarget.frames[i].mesh,
                        };
                    }
                    var binding = new EditorCurveBinding()
                    {
                        type = typeof(VoxelFrameAnimationObject),
                        path = "",
                        propertyName = "mesh",
                    };
                    AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
                }
                #endregion
                #region Material
                {
                    for (int mat = 0; mat < 8; mat++)
                    {
                        var binding = new EditorCurveBinding()
                        {
                            type = typeof(VoxelFrameAnimationObject),
                            path = "",
                            propertyName = "playMaterial" + mat.ToString(),
                        };
                        AnimationUtility.SetObjectReferenceCurve(clip, binding, null);
                    }
                    if (objectTarget.materials.Count > 1)
                    {
                        for (int mat = 0; mat < objectTarget.materials.Count; mat++)
                        {
                            ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[objectTarget.frames.Count];
                            for (int i = 0; i < objectTarget.frames.Count; i++)
                            {
                                keys[i] = new ObjectReferenceKeyframe()
                                {
                                    time = FrameToTime(i),
                                };
                                if (mat < objectTarget.frames[i].materialIndexes.Count)
                                {
                                    var index = objectTarget.frames[i].materialIndexes[mat];
                                    keys[i].value = objectTarget.materials[index];
                                }
                                else
                                {
                                    keys[i].value = null;
                                }
                            }
                            var binding = new EditorCurveBinding()
                            {
                                type = typeof(VoxelFrameAnimationObject),
                                path = "",
                                propertyName = "playMaterial" + mat.ToString(),
                            };
                            AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
                        }
                    }
                }
                #endregion
                if (File.Exists(path))
                {
                    AssetDatabase.SaveAssets();
                    path = FileUtil.GetProjectRelativePath(path);
                    AssetDatabase.ImportAsset(path);
                }
                else
                {
                    path = FileUtil.GetProjectRelativePath(path);
                    AssetDatabase.CreateAsset(clip, path);
                }
                AssetDatabase.Refresh();
                InternalEditorUtility.RepaintAllViews();
            }
        }
        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Export Animation Curve (Clip)", true)]
        private static bool IsValidateExportAnimationClip(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return false;

#if UNITY_2018_3_OR_NEWER
            return true;
#else
            return PrefabUtility.GetPrefabType(objectTarget) != PrefabType.Prefab;
#endif
        }

        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Remove All Voxel Importer Compornent", false, 10100)]
        private static void RemoveAllVoxelImporterCompornent(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return;

            Undo.DestroyObjectImmediate(objectTarget);
        }
        [MenuItem("CONTEXT/VoxelFrameAnimationObject/Remove All Voxel Importer Compornent", true)]
        private static bool IsValidateRemoveAllVoxelImporterCompornent(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelFrameAnimationObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }
    }
}
