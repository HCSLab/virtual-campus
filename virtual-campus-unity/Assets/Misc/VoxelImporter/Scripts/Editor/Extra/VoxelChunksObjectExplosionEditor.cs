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
    [CustomEditor(typeof(VoxelChunksObjectExplosion))]
    public class VoxelChunksObjectExplosionEditor : VoxelBaseExplosionEditor
    {
        public VoxelChunksObjectExplosion explosionObject { get; protected set; }
        public VoxelChunksObjectExplosionCore explosionObjectCore { get; protected set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            explosionBase = explosionObject = target as VoxelChunksObjectExplosion;
            if (explosionObject == null) return;
            explosionCore = explosionObjectCore = new VoxelChunksObjectExplosionCore(explosionObject);
            if (explosionCore.voxelBase == null)
            {
                if (explosionObject.chunksExplosion != null)
                {
                    for (int i = 0; i < explosionObject.chunksExplosion.Length; i++)
                    {
                        Undo.DestroyObjectImmediate(explosionObject.chunksExplosion[i]);
                    }
                }
                Undo.DestroyObjectImmediate(explosionBase);
                return;
            }
            OnEnableInitializeSet();
        }

        protected override void Inspector_MeshMaterial()
        {
            #region Material
            if (explosionObjectCore.voxelObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
            {
                EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
                {
                    EditorGUI.indentLevel++;
                    {
                        if (explosionObject.materials != null)
                        {
                            for (int i = 0; i < explosionObject.materials.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.ObjectField(explosionObject.materials[i], typeof(Material), false);
                                    EditorGUI.EndDisabledGroup();
                                }
                                if (explosionObject.materials[i] != null)
                                {
                                    if (!EditorCommon.IsMainAsset(explosionObject.materials[i]))
                                    {
                                        if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                                        {
                                            #region Create Material
                                            string path = EditorUtility.SaveFilePanel("Save material", explosionCore.voxelBaseCore.GetDefaultPath(), string.Format("{0}_explosion_mat{1}.mat", explosionObject.gameObject.name, i), "mat");
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                if (path.IndexOf(Application.dataPath) < 0)
                                                {
                                                    EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                                }
                                                else
                                                {
                                                    Undo.RecordObject(explosionObject, "Save Material");
                                                    path = FileUtil.GetProjectRelativePath(path);
                                                    AssetDatabase.CreateAsset(Material.Instantiate(explosionObject.materials[i]), path);
                                                    explosionObject.materials[i] = AssetDatabase.LoadAssetAtPath<Material>(path);
                                                    explosionCore.Generate();
                                                }
                                            }

                                            #endregion
                                        }
                                    }
                                    {
                                        if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                        {
                                            #region Reset Material
                                            Undo.RecordObject(explosionObject, "Reset Material");
                                            explosionObject.materials[i] = EditorCommon.Instantiate(explosionObject.materials[i]);
                                            explosionCore.Generate();
                                            #endregion
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    EditorGUI.indentLevel--;
                }
            }
            #endregion
        }

        [MenuItem("CONTEXT/VoxelChunksObjectExplosion/Save All Unsaved Assets")]
        private static void SaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var explosionObject = menuCommand.context as VoxelChunksObjectExplosion;
            if (explosionObject == null) return;

            var explosionCore = new VoxelChunksObjectExplosionCore(explosionObject);

            var folder = EditorUtility.OpenFolderPanel("Save all", explosionCore.voxelBaseCore.GetDefaultPath(), null);
            if (string.IsNullOrEmpty(folder)) return;
            if (folder.IndexOf(Application.dataPath) < 0)
            {
                EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                return;
            }

            Undo.RecordObject(explosionObject, "Save All Unsaved Assets");
            if (explosionObject.chunksExplosion != null)
                Undo.RecordObjects(explosionObject.chunksExplosion, "Save All Unsaved Assets");

            if (explosionObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
            {
                if (explosionObject.chunksExplosion != null)
                {
                    for (int i = 0; i < explosionObject.chunksExplosion.Length; i++)
                    {
                        if (explosionObject.chunksExplosion[i] == null) continue;
                        #region Mesh
                        for (int j = 0; j < explosionObject.chunksExplosion[i].meshes.Count; j++)
                        {
                            if (explosionObject.chunksExplosion[i].meshes[j] != null && explosionObject.chunksExplosion[i].meshes[j].mesh != null && !EditorCommon.IsMainAsset(explosionObject.chunksExplosion[i].meshes[j].mesh))
                            {
                                var chunkObject = explosionObject.chunksExplosion[i].GetComponent<VoxelChunksObjectChunk>();
                                if (chunkObject == null) continue;
                                var path = folder + "/" + string.Format("{0}_{1}_explosion_mesh{2}.asset", explosionObject.gameObject.name, chunkObject.chunkName, j);
                                path = FileUtil.GetProjectRelativePath(path);
                                AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.chunksExplosion[i].meshes[j].mesh), path);
                                explosionObject.chunksExplosion[i].meshes[j].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                            }
                        }
                        #endregion
                    }
                }

                #region Material
                if (explosionObject.materials != null)
                {
                    for (int index = 0; index < explosionObject.materials.Count; index++)
                    {
                        if (explosionObject.materials[index] == null) continue;
                        if (EditorCommon.IsMainAsset(explosionObject.materials[index])) continue;
                        var path = folder + "/" + string.Format("{0}_explosion_mat{1}.mat", explosionObject.gameObject.name, index);
                        path = FileUtil.GetProjectRelativePath(path);
                        AssetDatabase.CreateAsset(Material.Instantiate(explosionObject.materials[index]), path);
                        explosionObject.materials[index] = AssetDatabase.LoadAssetAtPath<Material>(path);
                    }
                }
                #endregion

            }
            else if (explosionObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
            {
                if (explosionObject.chunksExplosion != null)
                {
                    for (int i = 0; i < explosionObject.chunksExplosion.Length; i++)
                    {
                        if (explosionObject.chunksExplosion[i] == null) continue;
                        var chunkObject = explosionObject.chunksExplosion[i].GetComponent<VoxelChunksObjectChunk>();
                        if (chunkObject == null) continue;
                        #region Mesh
                        for (int j = 0; j < explosionObject.chunksExplosion[i].meshes.Count; j++)
                        {
                            if (explosionObject.chunksExplosion[i].meshes[j] != null && explosionObject.chunksExplosion[i].meshes[j].mesh != null && !EditorCommon.IsMainAsset(explosionObject.chunksExplosion[i].meshes[j].mesh))
                            {
                                var path = folder + "/" + string.Format("{0}_{1}_explosion_mesh{2}.asset", explosionObject.gameObject.name, chunkObject.chunkName, j);
                                path = FileUtil.GetProjectRelativePath(path);
                                AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.chunksExplosion[i].meshes[j].mesh), path);
                                explosionObject.chunksExplosion[i].meshes[j].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                            }
                        }
                        #endregion

                        #region Material
                        if (explosionObject.chunksExplosion[i].materials != null)
                        {
                            for (int index = 0; index < explosionObject.chunksExplosion[i].materials.Count; index++)
                            {
                                if (explosionObject.chunksExplosion[i].materials[index] == null) continue;
                                if (EditorCommon.IsMainAsset(explosionObject.chunksExplosion[i].materials[index])) continue;
                                var path = folder + "/" + string.Format("{0}_{1}_explosion_mat{2}.mat", explosionObject.gameObject.name, chunkObject.chunkName, index);
                                path = FileUtil.GetProjectRelativePath(path);
                                AssetDatabase.CreateAsset(Material.Instantiate(explosionObject.chunksExplosion[i].materials[index]), path);
                                explosionObject.chunksExplosion[i].materials[index] = AssetDatabase.LoadAssetAtPath<Material>(path);
                            }
                        }
                        #endregion
                    }
                }
            }
            else
            {
                Assert.IsTrue(false);
            }

            explosionCore.Generate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelChunksObjectExplosion/Save All Unsaved Assets", true)]
        private static bool IsValidateSaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObjectExplosion;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }

        [MenuItem("CONTEXT/VoxelChunksObjectExplosion/Reset All Assets")]
        private static void ResetAllSavedAssets(MenuCommand menuCommand)
        {
            var explosionObject = menuCommand.context as VoxelChunksObjectExplosion;
            if (explosionObject == null) return;

            var explosionCore = new VoxelChunksObjectExplosionCore(explosionObject);

            Undo.RecordObject(explosionObject, "Reset All Assets");
            if (explosionObject.chunksExplosion != null)
                Undo.RecordObjects(explosionObject.chunksExplosion, "Reset All Assets");

            explosionCore.ResetAllAssets();
            explosionCore.Generate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelChunksObjectExplosion/Reset All Assets", true)]
        private static bool IsValidateResetAllSavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelChunksObjectExplosion;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }
    }
}

