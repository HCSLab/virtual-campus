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
    [CustomEditor(typeof(VoxelSkinnedAnimationObjectExplosion))]
    public class VoxelSkinnedAnimationObjectExplosionEditor : VoxelBaseExplosionEditor
    {
        public VoxelSkinnedAnimationObjectExplosion explosionObject { get; protected set; }
        public VoxelSkinnedAnimationObjectExplosionCore explosionObjectCore { get; protected set; }

        protected override void OnEnable()
        {
            base.OnEnable();

            explosionBase = explosionObject = target as VoxelSkinnedAnimationObjectExplosion;
            if (explosionObject == null) return;
            explosionCore = explosionObjectCore = new VoxelSkinnedAnimationObjectExplosionCore(explosionObject);
            if (explosionCore.voxelBase == null)
            {
                Undo.DestroyObjectImmediate(explosionBase);
                return;
            }
            OnEnableInitializeSet();
        }

        protected override void Inspector_MeshMaterial()
        {
            #region Mesh
            {
                EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                {
                    EditorGUI.indentLevel++;
                    {
                        if (explosionObject.meshes != null)
                        {
                            for (int i = 0; i < explosionObject.meshes.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUI.BeginDisabledGroup(true);
                                    EditorGUILayout.ObjectField(explosionObject.meshes[i].mesh, typeof(Mesh), false);
                                    EditorGUI.EndDisabledGroup();
                                }
                                if (explosionObject.meshes[i] != null && explosionObject.meshes[i].mesh != null)
                                {
                                    if (!EditorCommon.IsMainAsset(explosionObject.meshes[i].mesh))
                                    {
                                        if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                                        {
                                            #region Create Mesh
                                            string path = EditorUtility.SaveFilePanel("Save mesh", explosionCore.voxelBaseCore.GetDefaultPath(), string.Format("{0}_explosion_mesh{1}.asset", explosionObject.gameObject.name, i), "asset");
                                            if (!string.IsNullOrEmpty(path))
                                            {
                                                if (path.IndexOf(Application.dataPath) < 0)
                                                {
                                                    EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                                }
                                                else
                                                {
                                                    Undo.RecordObject(explosionObject, "Save Mesh");
                                                    path = FileUtil.GetProjectRelativePath(path);
                                                    AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].mesh), path);
                                                    explosionObject.meshes[i].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                                                    explosionCore.Generate();
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    {
                                        if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                        {
                                            #region Reset Mesh
                                            Undo.RecordObject(explosionObject, "Reset Mesh");
                                            explosionObject.meshes[i].mesh = null;
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

            #region Material
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

        protected override void Inspector_Bake()
        {
            explosionObject.edit_bakeFoldout = EditorGUILayout.Foldout(explosionObject.edit_bakeFoldout, "Bake", guiStyleFoldoutBold);
            if (explosionObject.edit_bakeFoldout)
            {
                EditorGUILayout.BeginHorizontal(guiStyleSkinBox);
                EditorGUILayout.BeginVertical();
                {
                    #region Mesh
                    {
                        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel++;
                            {
                                if (explosionObject.meshes != null)
                                {
                                    for (int i = 0; i < explosionObject.meshes.Count; i++)
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            EditorGUI.BeginDisabledGroup(true);
                                            EditorGUILayout.ObjectField(explosionObject.meshes[i].bakeMesh, typeof(Mesh), false);
                                            EditorGUI.EndDisabledGroup();
                                        }
                                        if (explosionObject.meshes[i] != null && explosionObject.meshes[i].bakeMesh != null)
                                        {
                                            if (!EditorCommon.IsMainAsset(explosionObject.meshes[i].bakeMesh))
                                            {
                                                if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                                                {
                                                    #region Create Mesh
                                                    string path = EditorUtility.SaveFilePanel("Save mesh", explosionCore.voxelBaseCore.GetDefaultPath(), string.Format("{0}_explosion_bake_mesh{1}.asset", explosionObject.gameObject.name, i), "asset");
                                                    if (!string.IsNullOrEmpty(path))
                                                    {
                                                        if (path.IndexOf(Application.dataPath) < 0)
                                                        {
                                                            EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                                        }
                                                        else
                                                        {
                                                            Undo.RecordObject(explosionObject, "Save Mesh");
                                                            path = FileUtil.GetProjectRelativePath(path);
                                                            AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].bakeMesh), path);
                                                            explosionObject.meshes[i].bakeMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                                                            explosionCore.Generate();
                                                            ForceRepaint();
                                                        }
                                                    }
                                                    #endregion
                                                }
                                            }
                                            {
                                                if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                                {
                                                    #region Reset Mesh
                                                    Undo.RecordObject(explosionObject, "Reset Mesh");
                                                    explosionObject.meshes[i].bakeMesh = null;
                                                    explosionCore.Generate();
                                                    ForceRepaint();
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
                    
                    EditorGUILayout.Space();

                    #region Bake
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Bake"))
                        {
                            Undo.RecordObject(explosionBase, "Bake Voxel Explosion");
                            explosionObjectCore.Bake();
                            ForceRepaint();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    #endregion
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
        
        [MenuItem("CONTEXT/VoxelSkinnedAnimationObjectExplosion/Save All Unsaved Assets")]
        private static void SaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var explosionObject = menuCommand.context as VoxelSkinnedAnimationObjectExplosion;
            if (explosionObject == null) return;

            var explosionCore = new VoxelSkinnedAnimationObjectExplosionCore(explosionObject);

            var folder = EditorUtility.OpenFolderPanel("Save all", explosionCore.voxelBaseCore.GetDefaultPath(), null);
            if (string.IsNullOrEmpty(folder)) return;
            if (folder.IndexOf(Application.dataPath) < 0)
            {
                EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                return;
            }

            Undo.RecordObject(explosionObject, "Save All Unsaved Assets");

            #region Mesh
            if (explosionObject.meshes != null)
            {
                for (int i = 0; i < explosionObject.meshes.Count; i++)
                {
                    if (explosionObject.meshes[i] != null && explosionObject.meshes[i].mesh != null && !EditorCommon.IsMainAsset(explosionObject.meshes[i].mesh))
                    {
                        var path = folder + "/" + string.Format("{0}_explosion_mesh{1}.asset", explosionObject.gameObject.name, i);
                        path = FileUtil.GetProjectRelativePath(path);
                        path = AssetDatabase.GenerateUniqueAssetPath(path);
                        AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].mesh), path);
                        explosionObject.meshes[i].mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    }
                    if (explosionObject.meshes[i] != null && explosionObject.meshes[i].bakeMesh != null && !EditorCommon.IsMainAsset(explosionObject.meshes[i].bakeMesh))
                    {
                        var path = folder + "/" + string.Format("{0}_explosion_bake_mesh{1}.asset", explosionObject.gameObject.name, i);
                        path = FileUtil.GetProjectRelativePath(path);
                        path = AssetDatabase.GenerateUniqueAssetPath(path);
                        AssetDatabase.CreateAsset(Mesh.Instantiate(explosionObject.meshes[i].bakeMesh), path);
                        explosionObject.meshes[i].bakeMesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
                    }
                }
            }
            #endregion

            #region Material
            if (explosionObject.materials != null)
            {
                for (int index = 0; index < explosionObject.materials.Count; index++)
                {
                    if (explosionObject.materials[index] == null) continue;
                    if (EditorCommon.IsMainAsset(explosionObject.materials[index])) continue;
                    var path = folder + "/" + string.Format("{0}_explosion_mat{1}.mat", explosionObject.gameObject.name, index);
                    path = FileUtil.GetProjectRelativePath(path);
                    path = AssetDatabase.GenerateUniqueAssetPath(path);
                    AssetDatabase.CreateAsset(Material.Instantiate(explosionObject.materials[index]), path);
                    explosionObject.materials[index] = AssetDatabase.LoadAssetAtPath<Material>(path);
                }
            }
            #endregion

            explosionCore.Generate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelSkinnedAnimationObjectExplosion/Save All Unsaved Assets", true)]
        private static bool IsValidateSaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObjectExplosion;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }

        [MenuItem("CONTEXT/VoxelSkinnedAnimationObjectExplosion/Reset All Assets")]
        private static void ResetAllSavedAssets(MenuCommand menuCommand)
        {
            var explosionObject = menuCommand.context as VoxelSkinnedAnimationObjectExplosion;
            if (explosionObject == null) return;

            var explosionCore = new VoxelSkinnedAnimationObjectExplosionCore(explosionObject);

            Undo.RecordObject(explosionObject, "Reset All Assets");

            explosionCore.ResetAllAssets();
            explosionCore.Generate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelSkinnedAnimationObjectExplosion/Reset All Assets", true)]
        private static bool IsValidateResetAllSavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObjectExplosion;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }
    }
}

