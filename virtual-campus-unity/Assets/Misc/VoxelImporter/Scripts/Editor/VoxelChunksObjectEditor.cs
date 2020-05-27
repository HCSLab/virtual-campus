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
    [CustomEditor(typeof(VoxelChunksObject))]
    public class VoxelChunksObjectEditor : VoxelBaseEditor
    {
        public VoxelChunksObject objectTarget { get; protected set; }
        public VoxelChunksObjectCore objectCore { get; protected set; }

        #region strings
        private static string[] SplitModeQBStrings =
        {
            "Chunk Size",
            "Qubicle Matrix",
        };
        private static string[] SplitModeVOXStrings =
        {
            "Chunk Size",
            "MagicaVoxel World Editor",
        };
        private static string[] SplitModeDefaultStrings =
        {
            "Chunk Size",
        };
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            objectTarget = target as VoxelChunksObject;
            if (objectTarget == null) return;
            baseCore = objectCore = new VoxelChunksObjectCore(objectTarget);
            OnEnableInitializeSet();
        }

        protected override void InspectorGUI()
        {
            base.InspectorGUI();

#if UNITY_2018_3_OR_NEWER
            {
                if (!baseCore.isPrefabEditable)
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
                    EditorGUI.BeginDisabledGroup(isPrefab);

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
                        #region Material Mode
                        if (baseTarget.advancedMode)
                        {
                            EditorGUI.BeginChangeCheck();
                            var materialMode = (VoxelChunksObject.MaterialMode)EditorGUILayout.EnumPopup("Material Mode", objectTarget.materialMode);
                            if (EditorGUI.EndChangeCheck())
                            {
                                UndoRecordObject("Inspector");
                                {
                                    var chunkObjects = objectTarget.chunks;
                                    if (objectTarget.materialMode == VoxelChunksObject.MaterialMode.Combine)
                                    {
                                        objectTarget.materials = null;
                                        objectTarget.atlasTexture = null;
                                    }
                                    else if (objectTarget.materialMode == VoxelChunksObject.MaterialMode.Individual)
                                    {
                                        for (int i = 0; i < chunkObjects.Length; i++)
                                        {
                                            if (chunkObjects[i] == null) continue;
                                            chunkObjects[i].materials = null;
                                            chunkObjects[i].atlasTexture = null;
                                        }
                                    }
                                }
                                objectTarget.materialMode = materialMode;
                                Refresh();
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
                        if (objectTarget.materialMode == VoxelChunksObject.MaterialMode.Combine)
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
                        if (objectTarget.materialMode == VoxelChunksObject.MaterialMode.Combine)
                        {
                            EditorGUILayout.LabelField("Texture Size", objectTarget.atlasTexture != null ? string.Format("{0} x {1}", objectTarget.atlasTexture.width, objectTarget.atlasTexture.height) : "");
                        }
                        #endregion
                        EditorGUI.indentLevel--;
                    }
                    #endregion
                    EditorGUILayout.EndVertical();

                    EditorGUI.EndDisabledGroup();
                }
            }
            #endregion

            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Reset All Chunks Transform"))
                {
                    for (int i = 0; i < objectTarget.chunks.Length; i++)
                    {
                        if (objectTarget.chunks[i] == null) continue;
                        var t = objectTarget.chunks[i].transform;
                        Undo.RecordObject(t, "Reset All Chunks Transform");
                        t.localPosition = objectTarget.chunks[i].basicOffset;
                        t.localRotation = Quaternion.identity;
                        t.localScale = Vector3.one;
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginDisabledGroup(isPrefab);

                InspectorGUI_Refresh();

                #region Refresh all chunks
                if (GUILayout.Button(new GUIContent("Refresh all chunks", "This will be created completely from Chunk's GameObject again.\nInformation maintained in normal Refresh will also be updated.")))
                {
                    UndoRecordObject("Inspector", true);
                    Refresh();
                }
                #endregion

                EditorGUI.EndDisabledGroup();
                EditorGUILayout.EndHorizontal();
            }

#if UNITY_2018_3_OR_NEWER
            {
                if (!baseCore.isPrefabEditable)
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
#endif
        }
        protected override void UndoRecordObject(string text, bool reset = false)
        {
            base.UndoRecordObject(text);

            for (int i = 0; i < objectTarget.chunks.Length; i++)
            {
                if (objectTarget.chunks[i] == null) continue;
                Undo.RecordObject(objectTarget.chunks[i], text);
            }

            if (reset)
            {
                objectCore.RemoveAllChunk();
            }
        }
        protected override void InspectorGUI_ImportSettingsImportScaleVector()
        {
            EditorGUI.BeginChangeCheck();
            var importScale = EditorGUILayout.Vector3Field("Import Scale", objectTarget.edit_importScale);
            if (EditorGUI.EndChangeCheck())
            {
                InspectorGUI_ImportSettingsImportScale_Set(importScale);
            }
        }
        protected override void InspectorGUI_ImportSettingsImportScale_Set(Vector3 scale)
        {
            UndoRecordObject("Inspector");
            objectTarget.edit_importScale = scale;
        }
        protected override void InspectorGUI_ImportSettingsImportOffset()
        {
            EditorGUI.BeginChangeCheck();
            var importOffset = EditorGUILayout.Vector3Field("Import Offset", objectTarget.edit_importOffset);
            if (EditorGUI.EndChangeCheck())
            {
                UndoRecordObject("Inspector");
                objectTarget.edit_importOffset = importOffset;
            }
        }
        protected override void InspectorGUI_ImportSettingsExtra()
        {
            #region Split Mode
            {
                EditorGUI.BeginChangeCheck();
                int splitMode = 0;
                switch (objectTarget.fileType)
                {
                case VoxelBase.FileType.vox:
                    splitMode = EditorGUILayout.Popup("Split Mode", objectTarget.splitMode == VoxelChunksObject.SplitMode.ChunkSize ? 0 : 1, SplitModeVOXStrings);
                    break;
                case VoxelBase.FileType.qb:
                    splitMode = EditorGUILayout.Popup("Split Mode", objectTarget.splitMode == VoxelChunksObject.SplitMode.ChunkSize ? 0 : 1, SplitModeQBStrings);
                    break;
                default:
                    splitMode = EditorGUILayout.Popup("Split Mode", objectTarget.splitMode == VoxelChunksObject.SplitMode.ChunkSize ? 0 : 1, SplitModeDefaultStrings);
                    break;
                }
                if (EditorGUI.EndChangeCheck())
                {
                    UndoRecordObject("Inspector", true);
                    if (splitMode == 0)
                        objectTarget.splitMode = VoxelChunksObject.SplitMode.ChunkSize;
                    else
                        objectTarget.splitMode = VoxelChunksObject.SplitMode.QubicleMatrix;
                    Refresh();
                }
            }
            #endregion
            {
                EditorGUI.indentLevel++;
                #region Chunk Size
                if (objectTarget.splitMode == VoxelChunksObject.SplitMode.ChunkSize)
                {
                    EditorGUI.BeginChangeCheck();
                    var chunkSize = EditorGUILayout.Vector3Field("Chunk Size", new Vector3(objectTarget.edit_chunkSize.x, objectTarget.edit_chunkSize.y, objectTarget.edit_chunkSize.z));
                    if (EditorGUI.EndChangeCheck())
                    {
                        UndoRecordObject("Inspector");
                        objectTarget.edit_chunkSize.x = Mathf.RoundToInt(chunkSize.x);
                        objectTarget.edit_chunkSize.y = Mathf.RoundToInt(chunkSize.y);
                        objectTarget.edit_chunkSize.z = Mathf.RoundToInt(chunkSize.z);
                    }
                }
                #endregion
                EditorGUI.indentLevel--;
            }
            #region Changed
            {
                if (objectTarget.IsEditorChanged())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Change Settings", guiStyleRedBold);
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Revert", GUILayout.Width(64f)))
                    {
                        UndoRecordObject("Inspector");
                        objectTarget.RevertEditorParam();
                    }
                    if (GUILayout.Button("Apply", GUILayout.Width(64f)))
                    {
                        UndoRecordObject("Inspector", true);
                        objectTarget.ApplyEditorParam();
                        Refresh();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            #endregion
        }
        protected override void InspectorGUI_ImportOptimizeExtra()
        {
            #region Create contact faces of chunks            
            if (baseTarget.advancedMode)
            {
                EditorGUI.BeginChangeCheck();
                var createContactChunkFaces = EditorGUILayout.Toggle(new GUIContent("Create contact faces of chunks", "Generate faces of adjacent part of Chunk"), objectTarget.createContactChunkFaces);
                if (EditorGUI.EndChangeCheck())
                {
                    UndoRecordObject("Inspector");
                    objectTarget.createContactChunkFaces = createContactChunkFaces;
                    Refresh();
                }
            }
            #endregion
        }

        protected override void Refresh()
        {
            base.Refresh();

            objectTarget.RevertEditorParam();
        }

        protected override List<Material> GetMaterialListMaterials()
        {
            return objectTarget.materialMode == VoxelChunksObject.MaterialMode.Combine ? objectTarget.materials : null;
        }

        [MenuItem("CONTEXT/VoxelChunksObject/Save All Unsaved Assets")]
        private static void SaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelChunksObjectCore(objectTarget);

            var folder = EditorUtility.OpenFolderPanel("Save all", objectCore.GetDefaultPath(), null);
            if (string.IsNullOrEmpty(folder)) return;
            if (folder.IndexOf(Application.dataPath) < 0)
            {
                EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                return;
            }

            Undo.RecordObject(objectTarget, "Save All Unsaved Assets");
            if (objectTarget.chunks != null)
                Undo.RecordObjects(objectTarget.chunks, "Save All Unsaved Assets");

            if (objectTarget.materialMode == VoxelChunksObject.MaterialMode.Combine)
            {
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

                if (objectTarget.chunks != null)
                {
                    for (int i = 0; i < objectTarget.chunks.Length; i++)
                    {
                        if (objectTarget.chunks[i] == null) continue;
                        #region Mesh
                        if (objectTarget.chunks[i].mesh != null && !EditorCommon.IsMainAsset(objectTarget.chunks[i].mesh))
                        {
                            var path = folder + "/" + string.Format("{0}_{1}_mesh.asset", objectTarget.gameObject.name, objectTarget.chunks[i].chunkName);
                            path = FileUtil.GetProjectRelativePath(path);
                            path = AssetDatabase.GenerateUniqueAssetPath(path);
                            AssetDatabase.CreateAsset(Mesh.Instantiate(objectTarget.chunks[i].mesh), path);
                            objectTarget.chunks[i].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                        }
                        #endregion
                    }
                }
            }
            else if (objectTarget.materialMode == VoxelChunksObject.MaterialMode.Individual)
            {
                if (objectTarget.chunks != null)
                {
                    for (int i = 0; i < objectTarget.chunks.Length; i++)
                    {
                        if (objectTarget.chunks[i] == null) continue;
                        #region Mesh
                        if (objectTarget.chunks[i].mesh != null && !EditorCommon.IsMainAsset(objectTarget.chunks[i].mesh))
                        {
                            var path = folder + "/" + string.Format("{0}_{1}_mesh.asset", objectTarget.gameObject.name, objectTarget.chunks[i].chunkName);
                            path = FileUtil.GetProjectRelativePath(path);
                            path = AssetDatabase.GenerateUniqueAssetPath(path);
                            AssetDatabase.CreateAsset(Mesh.Instantiate(objectTarget.chunks[i].mesh), path);
                            objectTarget.chunks[i].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                        }
                        #endregion

                        #region Material
                        if (objectTarget.chunks[i].materials != null)
                        {
                            for (int index = 0; index < objectTarget.chunks[i].materials.Count; index++)
                            {
                                if (objectTarget.chunks[i].materials[index] == null || EditorCommon.IsMainAsset(objectTarget.chunks[i].materials[index])) continue;
                                var path = folder + "/" + string.Format("{0}_{1}_mat{2}.mat", objectTarget.gameObject.name, objectTarget.chunks[i].chunkName, index);
                                path = FileUtil.GetProjectRelativePath(path);
                                path = AssetDatabase.GenerateUniqueAssetPath(path);
                                AssetDatabase.CreateAsset(Material.Instantiate(objectTarget.chunks[i].materials[index]), path);
                                objectTarget.chunks[i].materials[index] = AssetDatabase.LoadAssetAtPath<Material>(path);
                            }
                        }
                        #endregion

                        #region Texture
                        if (objectTarget.chunks[i].atlasTexture != null && !EditorCommon.IsMainAsset(objectTarget.chunks[i].atlasTexture))
                        {
                            var path = folder + "/" + string.Format("{0}_{1}_tex.png", objectTarget.gameObject.name, objectTarget.chunks[i].chunkName);
                            path = EditorCommon.GenerateUniqueAssetFullPath(path);
                            File.WriteAllBytes(path, objectTarget.chunks[i].atlasTexture.EncodeToPNG());
                            path = FileUtil.GetProjectRelativePath(path);
                            AssetDatabase.ImportAsset(path);
                            objectCore.SetTextureImporterSetting(path, objectTarget.chunks[i].atlasTexture);
                            objectTarget.chunks[i].atlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                        }
                        #endregion
                    }
                }
            }
            else
            {
                Assert.IsTrue(false);
            }

            objectCore.ReCreate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelChunksObject/Save All Unsaved Assets", true)]
        private static bool IsValidateSaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }

        [MenuItem("CONTEXT/VoxelChunksObject/Reset All Assets")]
        private static void ResetAllSavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelChunksObjectCore(objectTarget);

            Undo.RecordObject(objectTarget, "Reset All Assets");
            if (objectTarget.chunks != null)
                Undo.RecordObjects(objectTarget.chunks, "Save All Unsaved Assets");

            objectCore.ResetAllAssets();
            objectCore.ReCreate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelChunksObject/Reset All Assets", true)]
        private static bool IsValidateResetAllSavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }

        [MenuItem("CONTEXT/VoxelChunksObject/Export COLLADA(dae) File", false, 10000)]
        private static void ExportDaeFile(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelChunksObjectCore(objectTarget);

            string path = EditorUtility.SaveFilePanel("Export COLLADA(dae) File", objectCore.GetDefaultPath(), string.Format("{0}.dae", Path.GetFileNameWithoutExtension(objectTarget.voxelFilePath)), "dae");
            if (string.IsNullOrEmpty(path)) return;

            if (!objectCore.ExportDaeFile(path))
            {
                Debug.LogErrorFormat("<color=green>[Voxel Importer]</color> Export COLLADA(dae) File error. file:{0}", path);
            }
        }
        [MenuItem("CONTEXT/VoxelChunksObject/Export COLLADA(dae) File", true)]
        private static bool IsValidateExportDaeFile(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObject;
            if (objectTarget == null) return false;

#if UNITY_2018_3_OR_NEWER
            return true;
#else
            return PrefabUtility.GetPrefabType(objectTarget) != PrefabType.Prefab;
#endif
        }

        [MenuItem("CONTEXT/VoxelChunksObject/Remove All Voxel Importer Compornent", false, 10100)]
        private static void RemoveAllVoxelImporterCompornent(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObject;
            if (objectTarget == null) return;

            if (objectTarget.chunks != null)
            {
                for (int i = 0; i < objectTarget.chunks.Length; i++)
                {
                    Undo.DestroyObjectImmediate(objectTarget.chunks[i]);
                }
            }
            Undo.DestroyObjectImmediate(objectTarget);
        }
        [MenuItem("CONTEXT/VoxelChunksObject/Remove All Voxel Importer Compornent", true)]
        private static bool IsValidateRemoveAllVoxelImporterCompornent(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }
    }
}

