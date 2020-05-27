using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace VoxelImporter
{
    [CustomEditor(typeof(VoxelChunksObjectChunkExplosion))]
    public class VoxelChunksObjectChunkExplosionEditor : Editor
    {
        public VoxelChunksObjectChunkExplosion explosionObject { get; protected set; }
        public VoxelChunksObjectChunkExplosionCore explosionObjectCore { get; protected set; }

        public VoxelChunksObjectChunk chunkObject { get; protected set; }
        public VoxelChunksObjectChunkCore chunkCore { get; protected set; }

        public VoxelChunksObject voxelObject { get; protected set; }
        public VoxelChunksObjectExplosion voxelExplosionObject { get; protected set; }
        public VoxelChunksObjectExplosionCore voxelExplosionCore { get; protected set; }

        #region GuiStyle
        private GUIStyle guiStyleSkinBox;
        private GUIStyle guiStyleMagentaBold;
        private GUIStyle guiStyleRedBold;
        private GUIStyle guiStyleFoldoutBold;
        #endregion

        #region Prefab
#if UNITY_2018_3_OR_NEWER
        protected PrefabAssetType prefabType { get { return PrefabUtility.GetPrefabAssetType(explosionObject.gameObject); } }
        protected bool prefabEnable { get { return (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant) || isPrefabEditMode; } }
        protected bool isPrefab { get { return false; } }
        protected bool isPrefabEditMode { get { return PrefabStageUtility.GetCurrentPrefabStage() != null && PrefabStageUtility.GetCurrentPrefabStage().prefabContentsRoot != null; } }
        protected bool isPrefabEditable { get { return EditorCommon.IsComponentEditable(explosionObject); } }
#else
        protected PrefabType prefabType { get { return PrefabUtility.GetPrefabType(explosionObject.gameObject); } }
        protected bool prefabEnable { get { var type = prefabType; return type == PrefabType.Prefab || type == PrefabType.PrefabInstance || type == PrefabType.DisconnectedPrefabInstance; } }
        protected bool isPrefab { get { return prefabType == PrefabType.Prefab; } }
#endif
        #endregion

        protected void OnEnable()
        {
            explosionObject = target as VoxelChunksObjectChunkExplosion;
            if (explosionObject == null) return;
            explosionObjectCore = new VoxelChunksObjectChunkExplosionCore(explosionObject);
            chunkObject = explosionObject.GetComponent<VoxelChunksObjectChunk>();
            if (chunkObject == null) return;
            chunkCore = new VoxelChunksObjectChunkCore(chunkObject);
            if (explosionObject.transform.parent == null) return;
            voxelObject = explosionObject.transform.parent.GetComponent<VoxelChunksObject>();
            if (voxelObject == null) return;
            voxelExplosionObject = voxelObject.GetComponent<VoxelChunksObjectExplosion>();
            if (voxelExplosionObject == null) return;
            voxelExplosionCore = new VoxelChunksObjectExplosionCore(voxelExplosionObject);
        }
        protected void OnDisable()
        {
            if (explosionObject == null || chunkObject == null || voxelObject == null) return;
        }

        public override void OnInspectorGUI()
        {
            if (explosionObject == null || chunkObject == null || voxelObject == null)
            {
                DrawDefaultInspector();
                return;
            }

#if UNITY_2018_3_OR_NEWER
            {
                if (!isPrefabEditable)
                {
                    EditorGUILayout.HelpBox("Prefab can only be edited in Prefab mode.", MessageType.Info);
                    EditorGUI.BeginDisabledGroup(true);
                }
            }
#endif

            #region GuiStyle
            if (guiStyleSkinBox == null)
            {
                guiStyleSkinBox = new GUIStyle(GUI.skin.box);
                var olBox = new GUIStyle("OL box");
                guiStyleSkinBox.normal = olBox.normal;
                guiStyleSkinBox.hover = olBox.hover;
                guiStyleSkinBox.focused = olBox.focused;
                guiStyleSkinBox.active = olBox.active;
            }
            if (guiStyleMagentaBold == null)
                guiStyleMagentaBold = new GUIStyle(EditorStyles.boldLabel);
            guiStyleMagentaBold.normal.textColor = Color.magenta;
            if (guiStyleRedBold == null)
                guiStyleRedBold = new GUIStyle(EditorStyles.boldLabel);
            guiStyleRedBold.normal.textColor = Color.red;
            if (guiStyleFoldoutBold == null)
                guiStyleFoldoutBold = new GUIStyle(EditorStyles.foldout);
            guiStyleFoldoutBold.fontStyle = FontStyle.Bold;
            #endregion

            serializedObject.Update();

            InspectorGUI();

            serializedObject.ApplyModifiedProperties();

#if UNITY_2018_3_OR_NEWER
            {
                if (!isPrefabEditable)
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
#endif
        }

        protected void InspectorGUI()
        {
            #region Object
            {
                explosionObject.edit_objectFoldout = EditorGUILayout.Foldout(explosionObject.edit_objectFoldout, "Object", guiStyleFoldoutBold);
                if (explosionObject.edit_objectFoldout)
                {
                    EditorGUILayout.BeginVertical(guiStyleSkinBox);
                    #region Mesh
                    {
                        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        #region Mesh
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
                                if (explosionObject.meshes[i].mesh != null)
                                {
                                    if (!EditorCommon.IsMainAsset(explosionObject.meshes[i].mesh))
                                    {
                                        if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                                        {
                                            #region Create Mesh
                                            string path = EditorUtility.SaveFilePanel("Save mesh", chunkCore.GetDefaultPath(), string.Format("{0}_{1}_explosion_mesh{2}.asset", voxelObject.gameObject.name, chunkObject.chunkName, i), "asset");
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
                                                    voxelExplosionCore.Generate();
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    {
                                        if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                        {
                                            #region Reset Mesh
                                            Undo.RecordObject(voxelExplosionObject, "Reset Mesh");
                                            Undo.RecordObjects(voxelExplosionObject.chunksExplosion, "Reset Mesh");
                                            explosionObject.meshes[i].mesh = null;
                                            voxelExplosionCore.Generate();
                                            #endregion
                                        }
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        #endregion
                        EditorGUI.indentLevel--;
                    }
                    #endregion
                    #region Material
                    if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
                    {
                        EditorGUILayout.LabelField("Material", EditorStyles.boldLabel);
                        EditorGUI.indentLevel++;
                        #region Material
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
                                        string defaultName = string.Format("{0}_{1}_explosion_mat{2}.mat", voxelObject.gameObject.name, chunkObject.chunkName, i);
                                        string path = EditorUtility.SaveFilePanel("Save material", chunkCore.GetDefaultPath(), defaultName, "mat");
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
                                                voxelExplosionCore.Generate();
                                            }
                                        }

                                        #endregion
                                    }
                                }
                                {
                                    if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                    {
                                        #region Reset Material
                                        Undo.RecordObject(voxelExplosionObject, "Reset Material");
                                        Undo.RecordObjects(voxelExplosionObject.chunksExplosion, "Reset Material");
                                        explosionObject.materials[i] = EditorCommon.Instantiate(explosionObject.materials[i]);
                                        voxelExplosionCore.Generate();
                                        #endregion
                                    }
                                }
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        #endregion
                        EditorGUI.indentLevel--;
                    }
                    #endregion
                    EditorGUILayout.EndVertical();
                }
            }
            #endregion

            #region Generate
            {
                if (GUILayout.Button("Generate"))
                {
                    Undo.RecordObject(voxelExplosionObject, "Generate Voxel Explosion");
                    Undo.RecordObjects(voxelExplosionObject.chunksExplosion, "Generate Voxel Explosion");
                    voxelExplosionCore.Generate();
                }
            }
            #endregion
        }
    }
}
