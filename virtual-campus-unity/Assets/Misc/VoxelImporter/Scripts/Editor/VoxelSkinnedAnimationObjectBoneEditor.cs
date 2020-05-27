using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections.Generic;

#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace VoxelImporter
{
    [CustomEditor(typeof(VoxelSkinnedAnimationObjectBone))]
    public class VoxelSkinnedAnimationObjectBoneEditor : Editor
    {
        public VoxelSkinnedAnimationObjectBone boneTarget { get; private set; }
        public VoxelSkinnedAnimationObjectBoneCore boneCore { get; protected set; }

        public VoxelSkinnedAnimationObjectBone rootTarget { get; private set; }

        public VoxelSkinnedAnimationObject objectTarget { get; private set; }
        public VoxelSkinnedAnimationObjectCore objectCore { get; protected set; }

        private VoxelEditorCommon editorCommon;

        private class EditWeight
        {
            public EditWeight()
            {
                this.flags = (VoxelBase.VoxelVertexFlags)(-1);
                this.power = new float[(int)VoxelBase.VoxelVertexIndex.Total];
                for (int i = 0; i < this.power.Length; i++)
                {
                    this.power[i] = 1f;
                }
            }
            public EditWeight(VoxelBase.VoxelVertexFlags flags, float power = 1f)
            {
                this.flags = flags;
                this.power = new float[(int)VoxelBase.VoxelVertexIndex.Total];
                for (int i = 0; i < this.power.Length; i++)
                {
                    this.power[i] = power;
                }
            }

            public VoxelBase.VoxelVertexFlags flags;
            public float[] power = new float[(int)VoxelBase.VoxelVertexIndex.Total];
        }
        private DataTable3<EditWeight> editWeightList = new DataTable3<EditWeight>();

        //Editor
        private float positionScaleFactor = 1f;
        private bool drawEditorMesh = true;
        private Vector3[] skeletonLines;

        private static Rect editorBoneEditorWindowRect = new Rect(8, 17 + 8, 0, 0);

        //GUIStyle
        private GUIStyle guiStyleBoldButton;
        private GUIStyle guiStyleFoldoutBold;
        private GUIStyle guiStyleCircleButton;
        private GUIStyle guiStyleEditorWindow;

        #region Texture
        private Texture2D circleNormalTex;
        private Texture2D circleActiveTex;
        #endregion

        #region strings
        public static readonly string[] Edit_VoxelModeString =
        {
            VoxelSkinnedAnimationObject.Edit_VoxelMode.Voxel.ToString(),
            VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex.ToString(),
        };
        public static readonly string[] Edit_VoxelWeightModeString =
        {
            VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Voxel.ToString(),
            VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Fill.ToString(),
            VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Rect.ToString(),
        };
        public static readonly string[] Edit_VertexWeightModeString =
        {
            VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Brush.ToString(),
            VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Rect.ToString(),
        };
        public static readonly GUIContent[] Edit_BlendModeString =
        {
            new GUIContent("=", VoxelSkinnedAnimationObject.Edit_BlendMode.Replace.ToString()),
            new GUIContent("+", VoxelSkinnedAnimationObject.Edit_BlendMode.Add.ToString()),
            new GUIContent("-", VoxelSkinnedAnimationObject.Edit_BlendMode.Subtract.ToString()),
        };
        public static readonly string[] Edit_MirrorSetModeString =
        {
            " ",
            "+",
            "-",
        };
        #endregion

        #region Prefab
#if UNITY_2018_3_OR_NEWER
        protected PrefabAssetType prefabType { get { return PrefabUtility.GetPrefabAssetType(objectTarget.gameObject); } }
        protected bool prefabEnable { get { return (prefabType == PrefabAssetType.Regular || prefabType == PrefabAssetType.Variant) || objectCore.isPrefabEditMode; } }
        protected bool isPrefab { get { return false; } }
#else
        protected PrefabType prefabType { get { return PrefabUtility.GetPrefabType(objectTarget.gameObject); } }
        protected bool prefabEnable { get { var type = prefabType; return type == PrefabType.Prefab || type == PrefabType.PrefabInstance || type == PrefabType.DisconnectedPrefabInstance; } }
        protected bool isPrefab { get { return prefabType == PrefabType.Prefab; } }
#endif
        #endregion

        void OnEnable()
        {
            boneTarget = target as VoxelSkinnedAnimationObjectBone;
            if (boneTarget == null) return;
            objectTarget = boneTarget.voxelObject;
            if (objectTarget == null) return;

            objectCore = new VoxelSkinnedAnimationObjectCore(objectTarget);
            objectCore.Initialize();
            boneCore = new VoxelSkinnedAnimationObjectBoneCore(boneTarget, objectCore);
            boneCore.Initialize();

            editorCommon = new VoxelEditorCommon(objectTarget, objectCore);

            #region rootTarget
            rootTarget = objectTarget.GetComponentInChildren<VoxelSkinnedAnimationObjectBone>();
            {
                var trans = objectTarget.transform;
                for (int i = 0; i < trans.childCount; i++)
                {
                    rootTarget = trans.GetChild(i).GetComponent<VoxelSkinnedAnimationObjectBone>();
                    if (rootTarget != null) break;
                }
            }
            #endregion

            if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition)
            {
                objectCore.ResetBoneTransform();
            }
            else if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
            {
                objectCore.UpdateBoneBindposes();
                objectCore.UpdateBoneWeightTable();
                UpdateWeightPreviewMesh();
            }
            else if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation)
            {
                objectCore.ReadyVoxelData();
                #region DisableAnimation
                {
                    if (boneTarget.edit_disablePositionAnimation && boneCore.IsHaveEraseDisablePositionAnimation())
                        boneTarget.edit_disablePositionAnimation = false;
                    if (boneTarget.edit_disableRotationAnimation && boneCore.IsHaveEraseDisableRotationAnimation())
                        boneTarget.edit_disableRotationAnimation = false;
                    if (boneTarget.edit_disableScaleAnimation && boneCore.IsHaveEraseDisableScaleAnimation())
                        boneTarget.edit_disableScaleAnimation = false;
                }
                #endregion
            }
            UpdateSilhouetteMeshMesh();

            boneTarget.transform.hasChanged = false;
            if (boneTarget.mirrorBone != null)
                boneTarget.mirrorBone.transform.hasChanged = false;

            #region Texture
            circleNormalTex = editorCommon.LoadTexture2DAssetAtPath("Assets/VoxelImporter/Textures/Editor/Circle_normal.psd");
            circleActiveTex = editorCommon.LoadTexture2DAssetAtPath("Assets/VoxelImporter/Textures/Editor/Circle_active.psd");
            #endregion

            editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;

            AnimationUtility.onCurveWasModified -= EditorOnCurveWasModified;
            AnimationUtility.onCurveWasModified += EditorOnCurveWasModified;
            Undo.undoRedoPerformed -= EditorUndoRedoPerformed;
            Undo.undoRedoPerformed += EditorUndoRedoPerformed;
#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneCustomGUI;
            SceneView.duringSceneGui += OnSceneCustomGUI;
#endif
        }
        void OnDisable()
        {
            if (boneTarget == null || objectTarget == null) return;

            if (!AnimationMode.InAnimationMode())
            {
                if (objectTarget.edit_afterRefresh)
                {
                    objectCore.ReCreate();
                }
                else
                {
                    objectCore.SetRendererCompornent();
                }
            }

            if (boneTarget.edit_weightMesh != null)
            {
                for (int i = 0; i < boneTarget.edit_weightMesh.Length; i++)
                {
                    MonoBehaviour.DestroyImmediate(boneTarget.edit_weightMesh[i]);
                }
                boneTarget.edit_weightMesh = null;
            }
            boneTarget.edit_weightColorTexture = null;

            objectCore.SetSelectedWireframeHidden(false);

            Tools.current = VoxelEditorCommon.lastTool;

#if UNITY_2019_1_OR_NEWER
            SceneView.duringSceneGui -= OnSceneCustomGUI;
#endif
            AnimationUtility.onCurveWasModified -= EditorOnCurveWasModified;
            Undo.undoRedoPerformed -= EditorUndoRedoPerformed;
        }
        void OnDestroy()
        {
            OnDisable();
        }

        public override void OnInspectorGUI()
        {
            if (boneTarget == null || objectTarget == null || editorCommon == null)
            {
                DrawDefaultInspector();
                return;
            }

            if (!objectCore.IsVoxelFileExists())
            {
                EditorGUILayout.HelpBox("Voxel file not found. Please open file.", MessageType.Error);
                return;
            }

#if UNITY_2018_3_OR_NEWER
            {
                if (!objectCore.isPrefabEditable)
                {
                    EditorGUILayout.HelpBox("Prefab can only be edited in Prefab mode.", MessageType.Info);
                    EditorGUI.BeginDisabledGroup(true);
                }
            }
#endif

            serializedObject.Update();

            #region GuiStyle
            if (guiStyleBoldButton == null)
                guiStyleBoldButton = new GUIStyle(GUI.skin.button);
            guiStyleBoldButton.fontStyle = FontStyle.Bold;
            if (guiStyleFoldoutBold == null)
                guiStyleFoldoutBold = new GUIStyle(EditorStyles.foldout);
            guiStyleFoldoutBold.fontStyle = FontStyle.Bold;
            editorCommon.GUIStyleReady();
            #endregion

            #region Simple
            {
                EditorGUI.BeginChangeCheck();
                var mode = GUILayout.Toolbar(objectTarget.advancedMode ? 1 : 0, VoxelBaseEditor.Edit_AdvancedModeStrings);
                if (EditorGUI.EndChangeCheck())
                {
                    objectTarget.advancedMode = mode != 0 ? true : false;
                }
            }
            #endregion

            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(isPrefab);

            //Edit
            {
                if (AnimationMode.InAnimationMode())
                {
                    EditorGUILayout.HelpBox("You can not change while animation is being recorded.\nTo change, please stop recording.", MessageType.Warning);
                }
                #region BoneAnimation
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                {
                    EditorGUI.BeginDisabledGroup(AnimationMode.InAnimationMode());
                    EditorGUI.BeginChangeCheck();
                    GUILayout.Toggle(objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation, "Edit Bone Animation", guiStyleBoldButton, GUILayout.Height(32));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(objectTarget, "Inspector");
                        Undo.RecordObject(boneTarget, "Inspector");
                        if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation)
                        {
                            objectTarget.editLastMode = objectTarget.editMode;
                            objectTarget.editMode = VoxelSkinnedAnimationObject.Edit_Mode.None;
                            Tools.current = VoxelEditorCommon.lastTool;
                        }
                        else
                        {
                            objectTarget.editLastMode = VoxelSkinnedAnimationObject.Edit_Mode.None;
                            objectTarget.editMode = VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation;
                            Tools.current = VoxelEditorCommon.lastTool;
                            if (objectTarget.edit_afterRefresh)
                            {
                                objectCore.ReCreate();
                            }
                            else
                            {
                                objectCore.ReadyVoxelData();
                            }
                            #region DisableAnimation
                            {
                                if (boneTarget.edit_disablePositionAnimation && boneCore.IsHaveEraseDisablePositionAnimation())
                                    boneTarget.edit_disablePositionAnimation = false;
                                if (boneTarget.edit_disableRotationAnimation && boneCore.IsHaveEraseDisableRotationAnimation())
                                    boneTarget.edit_disableRotationAnimation = false;
                                if (boneTarget.edit_disableScaleAnimation && boneCore.IsHaveEraseDisableScaleAnimation())
                                    boneTarget.edit_disableScaleAnimation = false;
                            }
                            #endregion
                            editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                        }
                        InternalEditorUtility.RepaintAllViews();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                #endregion
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                #region BonePosition
                {
                    EditorGUI.BeginDisabledGroup(AnimationMode.InAnimationMode());
                    EditorGUI.BeginChangeCheck();
                    GUILayout.Toggle(objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition, "Edit Bone Position", guiStyleBoldButton, GUILayout.Height(24));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(objectTarget, "Inspector");
                        if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition)
                        {
                            objectTarget.editLastMode = objectTarget.editMode;
                            objectTarget.editMode = VoxelSkinnedAnimationObject.Edit_Mode.None;
                            Tools.current = VoxelEditorCommon.lastTool;
                            UpdateEnableVoxel();
                        }
                        else
                        {
                            objectTarget.editLastMode = VoxelSkinnedAnimationObject.Edit_Mode.None;
                            objectTarget.editMode = VoxelSkinnedAnimationObject.Edit_Mode.BonePosition;
                            Tools.current = Tool.None;
                            objectCore.ResetBoneTransform();
                            UpdateEnableVoxel();
                            editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                        }
                        InternalEditorUtility.RepaintAllViews();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                #endregion
                EditorGUILayout.Space();
                #region BoneWeight
                {
                    EditorGUI.BeginDisabledGroup(AnimationMode.InAnimationMode());
                    EditorGUI.BeginChangeCheck();
                    GUILayout.Toggle(objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight, "Edit Bone Weight", guiStyleBoldButton, GUILayout.Height(24));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(objectTarget, "Inspector");
                        if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
                        {
                            objectTarget.editLastMode = objectTarget.editMode;
                            objectTarget.editMode = VoxelSkinnedAnimationObject.Edit_Mode.None;
                            Tools.current = VoxelEditorCommon.lastTool;
                            UpdateEnableVoxel();
                        }
                        else
                        {
                            objectTarget.editLastMode = VoxelSkinnedAnimationObject.Edit_Mode.None;
                            objectTarget.editMode = VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight;
                            Tools.current = Tool.None;
                            UpdateEnableVoxel();
                            editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                        }
                        InternalEditorUtility.RepaintAllViews();
                    }
                    EditorGUI.EndDisabledGroup();
                }
                #endregion
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();

                #region AnimationModeRecordReset
                if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition ||
                    objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
                {
                    if (AnimationMode.InAnimationMode())
                    {
                        objectTarget.editLastMode = objectTarget.editMode;
                        objectTarget.editMode = VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation;
                        Tools.current = VoxelEditorCommon.lastTool;
                    }
                }
                #endregion
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            {
                var rect = EditorGUILayout.GetControlRect();
                rect.height = 2;
                GUI.Box(rect, "");
                GUILayout.Space(-rect.height);
            }

            {
                var disable = objectTarget.rigAnimationType != VoxelSkinnedAnimationObject.RigAnimationType.None && objectTarget.editMode != VoxelSkinnedAnimationObject.Edit_Mode.None;
                EditorGUI.BeginDisabledGroup(disable);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                #region Add Child Bone
                {
                    if (GUILayout.Button("Add Child Bone", GUILayout.Height(20)))
                    {
                        GameObject go = new GameObject("Bone");
                        Undo.RegisterCreatedObjectUndo(go, "Add Child Bone");
                        Undo.SetTransformParent(go.transform, boneTarget.transform, "Add Child Bone");
                        Undo.AddComponent<VoxelSkinnedAnimationObjectBone>(go);
                        go.transform.localPosition = Vector3.zero;
                        go.transform.localRotation = Quaternion.identity;
                        go.transform.localScale = Vector3.one;
                        UpdateEnableVoxel();
                        //
                        Selection.activeGameObject = go;
                        EditorGUIUtility.PingObject(Selection.activeGameObject);
                    }
                }
                #endregion
                EditorGUILayout.Space();
                #region Remove This Bone
                {
                    EditorGUI.BeginDisabledGroup(boneTarget == rootTarget);
                    if (GUILayout.Button("Remove This Bone", GUILayout.Height(20)))
                    {
                        DisconnectPrefabInstance();
                        Selection.activeObject = boneTarget.transform.parent;
                        EditorApplication.delayCall += () =>
                        {
                            Undo.RecordObject(objectTarget, "Remove This Bone");
                            if (objectTarget.humanDescription.bones != null)
                            {
                                for (int i = 0; i < objectTarget.humanDescription.bones.Length; i++)
                                {
                                    if (objectTarget.humanDescription.bones[i] == boneTarget)
                                        objectTarget.humanDescription.bones[i] = null;
                                }
                            }
                            while (boneTarget.transform.childCount > 0)
                            {
                                var go = boneTarget.transform.GetChild(0).gameObject;
                                Undo.SetTransformParent(boneTarget.transform.GetChild(0), boneTarget.transform.parent, "Remove This Bone");
                                var bone = go.GetComponent<VoxelSkinnedAnimationObjectBone>();
                                if (bone != null && boneTarget.bonePositionSave)
                                {
                                    Undo.RecordObject(bone, "Remove This Bone");
                                    bone.bonePosition += boneTarget.boneRotation * boneTarget.bonePosition;
                                }
                            }
                            Undo.DestroyObjectImmediate(boneTarget.gameObject);

                            objectCore.ReCreate();
                            objectCore.FixMissingAnimation();
                            InternalEditorUtility.RepaintAllViews();
                        };
                        return;
                    }
                    EditorGUI.EndDisabledGroup();
                }
                #endregion
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();
                if (disable)
                {
                    EditorGUILayout.HelpBox("If Animation Type is not None, you can not operate while editing.", MessageType.Info);
                }
            }

            EditorGUILayout.Separator();

            #region Object
            if (objectTarget.advancedMode)
            {
                boneTarget.edit_objectFoldout = EditorGUILayout.Foldout(boneTarget.edit_objectFoldout, "Object", guiStyleFoldoutBold);
                if (boneTarget.edit_objectFoldout)
                {
                    EditorGUILayout.BeginVertical(editorCommon.guiStyleSkinBox);
                    {
                        EditorGUI.BeginChangeCheck();
                        var mirrorBone = (VoxelSkinnedAnimationObjectBone)EditorGUILayout.ObjectField("Mirror Bone", boneTarget.mirrorBone, typeof(VoxelSkinnedAnimationObjectBone), true);
                        if (EditorGUI.EndChangeCheck())
                        {
                            if (mirrorBone == null || boneTarget.voxelObject == mirrorBone.voxelObject)
                            {
                                Undo.RecordObject(boneTarget, "Disable Animation");
                                boneTarget.mirrorBone = mirrorBone;
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            #endregion

            if (GUILayout.Button("Refresh"))
            {
                Undo.RecordObject(objectTarget, "Refresh");
                UpdateEnableVoxel();
            }

            EditorGUI.EndDisabledGroup();

            #region Mirror
            {
                switch (objectTarget.editMode)
                {
                case VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation:
                    boneCore.MirrorBoneAnimation();
                    break;
                case VoxelSkinnedAnimationObject.Edit_Mode.BonePosition:
                    boneCore.MirrorBonePosition();
                    break;
                case VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight:
                    //boneCore.MirrorBoneWeight();
                    break;
                }
            }
            #endregion

            #region Changed
            if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition)
            {
                if (boneTarget.transform.hasChanged || (boneTarget.mirrorBone != null && boneTarget.mirrorBone.transform.hasChanged))
                {
                    objectCore.UpdateBoneBindposes();
                    objectTarget.edit_afterRefresh = true;
                    boneTarget.transform.hasChanged = false;
                    if (boneTarget.mirrorBone != null)
                        boneTarget.mirrorBone.transform.hasChanged = false;
                }
            }
            else if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
            {
                if (boneTarget.transform.hasChanged || (boneTarget.mirrorBone != null && boneTarget.mirrorBone.transform.hasChanged))
                {
                    UpdateEnableVoxel(false);
                    boneTarget.transform.hasChanged = false;
                    if (boneTarget.mirrorBone != null)
                        boneTarget.mirrorBone.transform.hasChanged = false;
                }
            }
            #endregion

            serializedObject.ApplyModifiedProperties();

#if UNITY_2018_3_OR_NEWER
            {
                if (!objectCore.isPrefabEditable)
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
#endif
        }

#if UNITY_2019_1_OR_NEWER
        private void OnSceneCustomGUI(SceneView sceneView)
        {
            if (sceneView != SceneView.currentDrawingSceneView) return;
#else
        protected virtual void OnSceneGUI()
        {
#endif
            if (boneTarget == null || objectTarget == null || rootTarget == null || editorCommon == null) return;
            if (objectTarget.voxelData == null) return;

            if (guiStyleCircleButton == null)
            {
                guiStyleCircleButton = new GUIStyle(GUI.skin.button);
                if (circleNormalTex != null)
                {
                    guiStyleCircleButton.normal.background = circleNormalTex;
                    guiStyleCircleButton.normal.scaledBackgrounds = null;
                }
                if (circleActiveTex != null)
                {
                    guiStyleCircleButton.active.background = circleActiveTex;
                    guiStyleCircleButton.active.scaledBackgrounds = null;
                }
                guiStyleCircleButton.border = new RectOffset(0, 0, 0, 0);
                guiStyleCircleButton.margin = new RectOffset(0, 0, 0, 0);
                guiStyleCircleButton.overflow = new RectOffset(0, 0, 0, 0);
                guiStyleCircleButton.padding = new RectOffset(0, 0, 0, 0);
                guiStyleCircleButton.imagePosition = ImagePosition.ImageOnly;
            }
            if (guiStyleEditorWindow == null)
            {
                if (EditorGUIUtility.isProSkin)
                    guiStyleEditorWindow = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).window);
                else
                    guiStyleEditorWindow = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).window);
            }
            editorCommon.GUIStyleReady();

            Event e = Event.current;
            bool repaint = false;

            #region Event
            if (SceneView.currentDrawingSceneView == SceneView.lastActiveSceneView)
            {
                if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation)
                {
                    #region BoneAnimation
                    var controlID = GUIUtility.GetControlID(FocusType.Passive);
                    VoxelEditorCommon.lastTool = Tools.current;
                    switch (e.type)
                    {
                    case EventType.Layout:
                        HandleUtility.AddDefaultControl(controlID);
                        break;
                    case EventType.KeyDown:
                        if (!e.alt)
                        {
                            if (e.keyCode == KeyCode.F5)
                            {
                                UpdateEnableVoxel();
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
                    #endregion
                }
                else if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition)
                {
                    #region BonePosition
                    var controlID = GUIUtility.GetControlID(FocusType.Passive);
                    Tools.current = Tool.None;
                    switch (e.type)
                    {
                    case EventType.Layout:
                        HandleUtility.AddDefaultControl(controlID);
                        break;
                    case EventType.KeyDown:
                        if (!e.alt)
                        {
                            if (e.keyCode == KeyCode.F5)
                            {
                                UpdateEnableVoxel();
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
                    #endregion
                }
                else if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
                {
                    #region BoneWeight
                    var controlID = GUIUtility.GetControlID(FocusType.Passive);
                    Tools.current = Tool.None;
                    if (boneTarget == rootTarget)
                    {
                        switch (e.type)
                        {
                        case EventType.Layout:
                            HandleUtility.AddDefaultControl(controlID);
                            break;
                        case EventType.KeyDown:
                            if (!e.alt)
                            {
                                if (e.keyCode == KeyCode.F5)
                                {
                                    UpdateEnableVoxel();
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
                    else
                    {
                        switch (e.type)
                        {
                        case EventType.Layout:
                            HandleUtility.AddDefaultControl(controlID);
                            break;
                        case EventType.MouseMove:
                            editWeightList.Clear();
                            editorCommon.selectionRect.Reset();
                            editorCommon.ClearPreviewMesh();
                            UpdateCursorMesh();
                            break;
                        case EventType.MouseDown:
                            if (editorCommon.CheckMousePositionEditorRects())
                            {
                                if (!e.alt && e.button == 0)
                                {
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
                        case EventType.KeyDown:
                            if (!e.alt)
                            {
                                if (e.keyCode == KeyCode.F5)
                                {
                                    UpdateEnableVoxel();
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
                }
                else
                {
                    #region None
                    drawEditorMesh = true;
                    VoxelEditorCommon.lastTool = Tools.current;
                    switch (e.type)
                    {
                    case EventType.KeyDown:
                        if (!e.alt)
                        {
                            #region Refresh
                            if (e.keyCode == KeyCode.F5)
                            {
                                UpdateEnableVoxel();
                            }
                            #endregion
                        }
                        break;
                    }
                    #endregion
                }
            }
            #endregion

            if (drawEditorMesh)
            {
                #region DrawBaseMesh
                if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition)
                {
                    if (objectTarget.mesh != null && objectTarget.atlasTexture != null)
                    {
                        editorCommon.unlitTextureMaterial.mainTexture = objectTarget.atlasTexture;
                        editorCommon.unlitTextureMaterial.color = new Color(1, 1, 1, 0.5f);
                        editorCommon.unlitTextureMaterial.SetPass(0);
                        Graphics.DrawMeshNow(objectTarget.mesh, objectTarget.transform.localToWorldMatrix);
                    }
                }
                #endregion

                #region SilhouetteMesh
                if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
                {
                    if (editorCommon.silhouetteMesh != null)
                    {
                        for (int i = 0; i < editorCommon.silhouetteMesh.Length; i++)
                        {
                            if (editorCommon.silhouetteMesh[i] == null) continue;
                            editorCommon.unlitColorMaterial.color = new Color(0, 0, 0, 1f);
                            editorCommon.unlitColorMaterial.SetPass(0);
                            Graphics.DrawMeshNow(editorCommon.silhouetteMesh[i], objectTarget.transform.localToWorldMatrix);
                        }
                    }
                }
                #endregion

                if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
                {
                    #region WeightMesh
                    if (boneTarget.edit_weightMesh != null)
                    {
                        for (int i = 0; i < boneTarget.edit_weightMesh.Length; i++)
                        {
                            if (boneTarget.edit_weightMesh[i] == null) continue;
                            if (objectTarget.edit_WeightPreviewMode == VoxelSkinnedAnimationObject.Edit_WeightPreviewMode.Transparent)
                            {
                                editorCommon.vertexColorTransparentMaterial.color = new Color(1, 1, 1, 0.75f);
                                editorCommon.vertexColorTransparentMaterial.SetPass(0);
                            }
                            else
                            {
                                editorCommon.vertexColorMaterial.color = new Color(1, 1, 1, 1);
                                editorCommon.vertexColorMaterial.SetPass(0);
                            }
                            Graphics.DrawMeshNow(boneTarget.edit_weightMesh[i], objectTarget.transform.localToWorldMatrix);
                        }
                    }
                    #endregion
                }

                #region DrawArrow
                if (objectTarget.editMode != VoxelSkinnedAnimationObject.Edit_Mode.None)
                {
                    DrawBoneArrow(rootTarget.transform);
                }
                #endregion
            }

            if (SceneView.currentDrawingSceneView == SceneView.lastActiveSceneView)
            {
                editorCommon.editorRectList.Clear();

                if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation)
                {
                    if (drawEditorMesh)
                        GuiBoneButton();

                    #region Bone Animation Editor
                    {
                        editorBoneEditorWindowRect = GUILayout.Window(EditorGUIUtility.GetControlID(FocusType.Passive, editorBoneEditorWindowRect), editorBoneEditorWindowRect, (id) =>
                        {
                            #region Disable
                            {
                                EditorGUI.BeginDisabledGroup(objectTarget.GetComponent<Animator>() == null);
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_disablePositionAnimation = EditorGUILayout.ToggleLeft("Disable Position Animation", boneTarget.edit_disablePositionAnimation);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        bool enable = false;
                                        if (edit_disablePositionAnimation && boneCore.IsHaveEraseDisablePositionAnimation())
                                            enable = EditorUtility.DisplayDialog("Warning", "All position animation curve will be deleted.\nAre you sure?", "ok", "cancel");
                                        else
                                            enable = true;
                                        if (enable)
                                        {
                                            Undo.RecordObject(boneTarget, "Disable Animation");
                                            {
                                                var animator = objectTarget.GetComponent<Animator>();
                                                if (animator != null && animator.runtimeAnimatorController != null)
                                                    Undo.RecordObjects(animator.runtimeAnimatorController.animationClips, "Disable Animation");
                                            }
                                            boneTarget.edit_disablePositionAnimation = edit_disablePositionAnimation;
                                            boneCore.EraseDisableAnimation();
                                            InternalEditorUtility.RepaintAllViews();
                                        }
                                        editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                    }
                                }
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_disableRotationAnimation = EditorGUILayout.ToggleLeft("Disable Rotation Animation", boneTarget.edit_disableRotationAnimation);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        bool enable = false;
                                        if (edit_disableRotationAnimation && boneCore.IsHaveEraseDisableRotationAnimation())
                                            enable = EditorUtility.DisplayDialog("Warning", "All rotation animation curve will be deleted.\nAre you sure?", "ok", "cancel");
                                        else
                                            enable = true;
                                        if (enable)
                                        {
                                            Undo.RecordObject(boneTarget, "Disable Animation");
                                            {
                                                var animator = objectTarget.GetComponent<Animator>();
                                                if (animator != null && animator.runtimeAnimatorController != null)
                                                    Undo.RecordObjects(animator.runtimeAnimatorController.animationClips, "Disable Animation");
                                            }
                                            boneTarget.edit_disableRotationAnimation = edit_disableRotationAnimation;
                                            boneCore.EraseDisableAnimation();
                                            InternalEditorUtility.RepaintAllViews();
                                        }
                                        editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                    }
                                }
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_disableScaleAnimation = EditorGUILayout.ToggleLeft("Disable Scale Animation", boneTarget.edit_disableScaleAnimation);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        bool enable = false;
                                        if (edit_disableScaleAnimation && boneCore.IsHaveEraseDisableScaleAnimation())
                                            enable = EditorUtility.DisplayDialog("Warning", "All scale animation curve will be deleted.\nAre you sure?", "ok", "cancel");
                                        else
                                            enable = true;
                                        if (enable)
                                        {
                                            Undo.RecordObject(boneTarget, "Disable Animation");
                                            {
                                                var animator = objectTarget.GetComponent<Animator>();
                                                if (animator != null && animator.runtimeAnimatorController != null)
                                                    Undo.RecordObjects(animator.runtimeAnimatorController.animationClips, "Disable Animation");
                                            }
                                            boneTarget.edit_disableScaleAnimation = edit_disableScaleAnimation;
                                            boneCore.EraseDisableAnimation();
                                            InternalEditorUtility.RepaintAllViews();
                                        }
                                        editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                    }
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                            #endregion
                            #region Mirror
                            {
                                EditorGUI.BeginDisabledGroup(boneTarget.mirrorBone == null);
                                EditorGUI.BeginChangeCheck();
                                var edit_mirrorSetBoneAnimation = EditorGUILayout.ToggleLeft(new GUIContent(string.Format("Set to mirror bone\n ({0})", boneTarget.mirrorBone != null ? boneTarget.mirrorBone.name : "none"), "Mirroring is auto enabled if it contains the name of the GameObject is \"Left\" and \"Right\".\nIt is also possible to set it manually with the inspector."),
                                                                                                boneTarget.edit_mirrorSetBoneAnimation, GUILayout.Height(32));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(boneTarget, "Mirror");
                                    boneTarget.edit_mirrorSetBoneAnimation = edit_mirrorSetBoneAnimation;
                                    InternalEditorUtility.RepaintAllViews();
                                    editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                            #endregion
                            if (boneTarget.edit_mirrorSetBoneAnimation && boneTarget.mirrorBone != null)
                            {
                                #region Mode
                                {
                                    #region Position
                                    if (!boneTarget.edit_disablePositionAnimation)
                                    {
                                        editorCommon.guiStyleLabel.normal.textColor = new Color(0.5f, 0, 0);
                                        for (int i = 0; i < objectTarget.edit_mirrorPosition.Length; i++)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            {
                                                string text = "";
                                                switch (i)
                                                {
                                                case 0: text = "Position X"; break;
                                                case 1: text = "Position Y"; break;
                                                case 2: text = "Position Z"; break;
                                                }
                                                EditorGUILayout.LabelField(text, editorCommon.guiStyleLabel, GUILayout.Width(100));
                                            }
                                            EditorGUI.BeginChangeCheck();
                                            var mode = (VoxelSkinnedAnimationObject.Edit_MirrorSetMode)GUILayout.Toolbar((int)objectTarget.edit_mirrorPosition[i], Edit_MirrorSetModeString);
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                Undo.RecordObject(objectTarget, "Mirror Mode");
                                                objectTarget.edit_mirrorPosition[i] = mode;
                                                InternalEditorUtility.RepaintAllViews();
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                    #endregion
                                    #region Rotation
                                    if (!boneTarget.edit_disableRotationAnimation)
                                    {
                                        editorCommon.guiStyleLabel.normal.textColor = new Color(0, 0.5f, 0);
                                        for (int i = 0; i < objectTarget.edit_mirrorRotation.Length; i++)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            {
                                                string text = "";
                                                switch (i)
                                                {
                                                case 0: text = "Rotation X"; break;
                                                case 1: text = "Rotation Y"; break;
                                                case 2: text = "Rotation Z"; break;
                                                }
                                                EditorGUILayout.LabelField(text, editorCommon.guiStyleLabel, GUILayout.Width(100));
                                            }
                                            EditorGUI.BeginChangeCheck();
                                            var mode = (VoxelSkinnedAnimationObject.Edit_MirrorSetMode)GUILayout.Toolbar((int)objectTarget.edit_mirrorRotation[i], Edit_MirrorSetModeString);
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                Undo.RecordObject(objectTarget, "Mirror Mode");
                                                objectTarget.edit_mirrorRotation[i] = mode;
                                                InternalEditorUtility.RepaintAllViews();
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                    #endregion
                                    #region Scale
                                    if (!boneTarget.edit_disableScaleAnimation)
                                    {
                                        editorCommon.guiStyleLabel.normal.textColor = new Color(0, 0, 0.5f);
                                        for (int i = 0; i < objectTarget.edit_mirrorScale.Length; i++)
                                        {
                                            EditorGUILayout.BeginHorizontal();
                                            {
                                                string text = "";
                                                switch (i)
                                                {
                                                case 0: text = "Scale X"; break;
                                                case 1: text = "Scale Y"; break;
                                                case 2: text = "Scale Z"; break;
                                                }
                                                EditorGUILayout.LabelField(text, editorCommon.guiStyleLabel, GUILayout.Width(100));
                                            }
                                            EditorGUI.BeginChangeCheck();
                                            var mode = (VoxelSkinnedAnimationObject.Edit_MirrorSetMode)GUILayout.Toolbar((int)objectTarget.edit_mirrorScale[i], Edit_MirrorSetModeString);
                                            if (EditorGUI.EndChangeCheck())
                                            {
                                                Undo.RecordObject(objectTarget, "Mirror Mode");
                                                objectTarget.edit_mirrorScale[i] = mode;
                                                InternalEditorUtility.RepaintAllViews();
                                            }
                                            EditorGUILayout.EndHorizontal();
                                        }
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #region Help
                            if (!objectTarget.edit_helpEnable)
                            {
                                #region "?"
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Space();
                                    EditorGUI.BeginChangeCheck();
                                    GUILayout.Toggle(objectTarget.edit_helpEnable, "?", GUI.skin.button, GUILayout.Width(16));
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(objectTarget, "Help Enable");
                                        objectTarget.edit_helpEnable = !objectTarget.edit_helpEnable;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                #endregion
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                objectTarget.edit_helpEnable = EditorGUILayout.Foldout(objectTarget.edit_helpEnable, "Help");
                                if (EditorGUI.EndChangeCheck())
                                {
                                    editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                }
                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                EditorGUILayout.LabelField("F5 Key - Refresh");
                                EditorGUILayout.LabelField("Press Space Key - Hide Preview");
                                EditorGUILayout.EndVertical();
                            }
                            #endregion

                            GUI.DragWindow();

                        }, "Bone Animation Editor", guiStyleEditorWindow);
                        editorBoneEditorWindowRect = editorCommon.ResizeSceneViewRect(editorBoneEditorWindowRect);
                    }
                    #endregion

                    if (boneTarget.transform.hasChanged)
                    {
                        boneTarget.transform.hasChanged = false;
                        boneCore.MirrorBoneAnimation();
                    }
                }
                else if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition)
                {
                    if (drawEditorMesh)
                        GuiBoneButton();

                    #region Bone Position Editor
                    {
                        editorBoneEditorWindowRect = GUILayout.Window(EditorGUIUtility.GetControlID(FocusType.Passive, editorBoneEditorWindowRect), editorBoneEditorWindowRect, (id) =>
                        {
                            #region Snap to half-voxel
                            {
                                EditorGUI.BeginChangeCheck();
                                var edit_snapToHalfVoxel = EditorGUILayout.ToggleLeft("Snap to half-voxel", objectTarget.edit_snapToHalfVoxel);
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(objectTarget, "Snap to half-voxel");
                                    objectTarget.edit_snapToHalfVoxel = edit_snapToHalfVoxel;
                                }
                            }
                            #endregion
                            #region Mirror
                            {
                                EditorGUI.BeginDisabledGroup(boneTarget.mirrorBone == null);
                                EditorGUI.BeginChangeCheck();
                                var edit_mirrorSetBonePosition = EditorGUILayout.ToggleLeft(new GUIContent(string.Format("Set to mirror bone\n ({0})", boneTarget.mirrorBone != null ? boneTarget.mirrorBone.name : "none"), "Mirroring is auto enabled if it contains the name of the GameObject is \"Left\" and \"Right\".\nIt is also possible to set it manually with the inspector."),
                                                                                            boneTarget.edit_mirrorSetBonePosition, GUILayout.Height(32));
                                if (EditorGUI.EndChangeCheck())
                                {
                                    Undo.RecordObject(boneTarget, "Set to mirror bone");
                                    boneTarget.edit_mirrorSetBonePosition = edit_mirrorSetBonePosition;
                                }
                                EditorGUI.EndDisabledGroup();
                            }
                            #endregion
                            #region Scaling
                            {
                                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                                {
                                    positionScaleFactor = EditorGUILayout.FloatField(new GUIContent("Scaling All"), positionScaleFactor);
                                }
                                {
                                    EditorGUILayout.Space();
                                    if (GUILayout.Button(new GUIContent("Apply")))
                                    {
                                        Undo.RecordObject(objectTarget, "Scaling All");
                                        for (int i = 0; i < objectTarget.bones.Length; i++)
                                        {
                                            var t = objectTarget.bones[i].transform;
                                            Undo.RecordObject(t, "Scaling");
                                            t.localPosition *= positionScaleFactor;
                                            if (objectTarget.edit_snapToHalfVoxel)
                                            {
                                                var tmp = new IntVector3(Mathf.RoundToInt(t.localPosition.x / (0.5f * objectTarget.importScale.x)), Mathf.RoundToInt(t.localPosition.y / (0.5f * objectTarget.importScale.y)), Mathf.RoundToInt(t.localPosition.z / (0.5f * objectTarget.importScale.z)));
                                                t.localPosition = new Vector3(tmp.x * (0.5f * objectTarget.importScale.x), tmp.y * (0.5f * objectTarget.importScale.y), tmp.z * (0.5f * objectTarget.importScale.z));
                                            }
                                            t.hasChanged = false;
                                        }
                                        objectCore.UpdateBoneBindposes();
                                        UpdateEnableVoxel();
                                    }
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            #endregion
                            #region Help
                            if (!objectTarget.edit_helpEnable)
                            {
                                #region "?"
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Space();
                                    EditorGUI.BeginChangeCheck();
                                    GUILayout.Toggle(objectTarget.edit_helpEnable, "?", GUI.skin.button, GUILayout.Width(16));
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(objectTarget, "Help Enable");
                                        objectTarget.edit_helpEnable = !objectTarget.edit_helpEnable;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                #endregion
                            }
                            else
                            {
                                EditorGUI.BeginChangeCheck();
                                objectTarget.edit_helpEnable = EditorGUILayout.Foldout(objectTarget.edit_helpEnable, "Help");
                                if (EditorGUI.EndChangeCheck())
                                {
                                    editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                }
                                EditorGUILayout.BeginVertical(GUI.skin.box);
                                EditorGUILayout.LabelField("F5 Key - Refresh");
                                EditorGUILayout.LabelField("Press Space Key - Hide Preview");
                                EditorGUILayout.EndVertical();
                            }
                            #endregion

                            GUI.DragWindow();

                        }, "Bone Position Editor", guiStyleEditorWindow);
                        editorBoneEditorWindowRect = editorCommon.ResizeSceneViewRect(editorBoneEditorWindowRect);
                    }
                    #endregion

                    #region Handle
                    {
                        Vector3 pos = (objectTarget.bindposes[boneTarget.boneIndex] * objectTarget.transform.worldToLocalMatrix).inverse.GetColumn(3);
                        {
                            EditorGUI.BeginChangeCheck();
                            var worldResult = Handles.PositionHandle(pos, objectTarget.transform.rotation);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(boneTarget.transform, "Position Move");
                                Undo.RecordObject(boneTarget, "Position Move");
                                boneTarget.transform.position += worldResult - pos;
                                if (objectTarget.edit_snapToHalfVoxel)
                                {
                                    var tmp = new IntVector3(Mathf.RoundToInt(boneTarget.transform.localPosition.x / (0.5f * objectTarget.importScale.x)), Mathf.RoundToInt(boneTarget.transform.localPosition.y / (0.5f * objectTarget.importScale.y)), Mathf.RoundToInt(boneTarget.transform.localPosition.z / (0.5f * objectTarget.importScale.z)));
                                    boneTarget.transform.localPosition = new Vector3(tmp.x * (0.5f * objectTarget.importScale.x), tmp.y * (0.5f * objectTarget.importScale.y), tmp.z * (0.5f * objectTarget.importScale.z));
                                }
                                boneTarget.transform.hasChanged = false;
                                boneCore.MirrorBonePosition();
                                objectCore.UpdateBoneBindposes();
                                objectTarget.edit_afterRefresh = true;
                            }
                        }
                    }
                    #endregion
                }
                else if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
                {
                    if (boneTarget == rootTarget)
                    {
                        if (drawEditorMesh)
                            GuiBoneButton();

                        #region Bone Weight Editor
                        {
                            editorBoneEditorWindowRect = GUILayout.Window(EditorGUIUtility.GetControlID(FocusType.Passive, editorBoneEditorWindowRect), editorBoneEditorWindowRect, (id) =>
                            {
                                #region WeightClear
                                {
                                    if (GUILayout.Button("Clear All Bones Weight"))
                                    {
                                        Undo.RecordObject(boneTarget, "Clear All Bones Weight");
                                        Undo.RecordObjects(objectTarget.bones, "Clear All Bones Weight");
                                        for (int i = 0; i < objectTarget.bones.Length; i++)
                                        {
                                            objectTarget.bones[i].weightData.ClearWeight();
                                        }
                                        UpdateEnableVoxel(false);
                                    }
                                }
                                #endregion
                                #region WeightPreviewMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_WeightPreviewMode = (VoxelSkinnedAnimationObject.Edit_WeightPreviewMode)EditorGUILayout.EnumPopup("Preview", objectTarget.edit_WeightPreviewMode);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(objectTarget, "Weight Preview Mode");
                                        objectTarget.edit_WeightPreviewMode = edit_WeightPreviewMode;
                                    }
                                }
                                #endregion
                                #region Help
                                if (!objectTarget.edit_helpEnable)
                                {
                                    #region "?"
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUI.BeginChangeCheck();
                                        GUILayout.Toggle(objectTarget.edit_helpEnable, "?", GUI.skin.button, GUILayout.Width(16));
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(objectTarget, "Help Enable");
                                            objectTarget.edit_helpEnable = !objectTarget.edit_helpEnable;
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    EditorGUI.BeginChangeCheck();
                                    objectTarget.edit_helpEnable = EditorGUILayout.Foldout(objectTarget.edit_helpEnable, "Help");
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                    }
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    EditorGUILayout.LabelField("F5 Key - Refresh");
                                    EditorGUILayout.LabelField("Press Space Key - Hide Preview");
                                    EditorGUILayout.EndVertical();
                                }
                                #endregion

                                GUI.DragWindow();

                            }, "Bone Weight Editor", guiStyleEditorWindow);
                            editorBoneEditorWindowRect = editorCommon.ResizeSceneViewRect(editorBoneEditorWindowRect);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Preview Mesh
                        if (editorCommon.previewMesh != null)
                        {
                            Color color = Color.white;
                            color.a = 0.5f + 0.5f * (1f - editorCommon.AnimationPower);
                            for (int i = 0; i < editorCommon.previewMesh.Length; i++)
                            {
                                if (editorCommon.previewMesh[i] == null) continue;
                                editorCommon.vertexColorTransparentMaterial.color = color;
                                editorCommon.vertexColorTransparentMaterial.SetPass(0);
                                Graphics.DrawMeshNow(editorCommon.previewMesh[i], objectTarget.transform.localToWorldMatrix);
                            }
                            repaint = true;
                        }
                        #endregion

                        #region Cursor Mesh
                        {
                            float color = 0.2f + 0.4f * (1f - editorCommon.AnimationPower);
                            if (editorCommon.cursorMesh != null)
                            {
                                for (int i = 0; i < editorCommon.cursorMesh.Length; i++)
                                {
                                    if (editorCommon.cursorMesh[i] == null) continue;
                                    editorCommon.vertexColorTransparentMaterial.color = new Color(1, 1, 1, color);
                                    editorCommon.vertexColorTransparentMaterial.SetPass(0);
                                    Graphics.DrawMeshNow(editorCommon.cursorMesh[i], objectTarget.transform.localToWorldMatrix);
                                }
                            }
                            repaint = true;
                        }
                        #endregion

                        #region Selection Rect
                        if ((objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Voxel && objectTarget.edit_voxelWeightMode == VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Rect) ||
                            (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex && objectTarget.edit_vertexWeightMode == VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Rect))
                        {
                            if (editorCommon.selectionRect.Enable)
                            {
                                Handles.BeginGUI();
                                GUI.Box(editorCommon.selectionRect.rect, "", "SelectionRect");
                                Handles.EndGUI();
                                repaint = true;
                            }
                        }
                        #endregion

                        if (drawEditorMesh)
                            GuiBoneButton();

                        #region Bone Weight Editor
                        {
                            editorBoneEditorWindowRect = GUILayout.Window(EditorGUIUtility.GetControlID(FocusType.Passive, editorBoneEditorWindowRect), editorBoneEditorWindowRect, (id) =>
                            {
                                #region VoxelMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var voxelMode = (VoxelSkinnedAnimationObject.Edit_VoxelMode)GUILayout.Toolbar((int)objectTarget.edit_voxelMode, Edit_VoxelModeString);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(objectTarget, "Voxel Mode");
                                        objectTarget.edit_voxelMode = voxelMode;
                                        ShowNotification();
                                        editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                    }
                                }
                                #endregion
                                #region Voxel
                                if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Voxel)
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var weightMode = (VoxelSkinnedAnimationObject.Edit_VoxelWeightMode)GUILayout.Toolbar((int)objectTarget.edit_voxelWeightMode, Edit_VoxelWeightModeString);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(objectTarget, "Weight Mode");
                                        objectTarget.edit_voxelWeightMode = weightMode;
                                        ShowNotification();
                                        editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                    }
                                }
                                else if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex)
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var weightMode = (VoxelSkinnedAnimationObject.Edit_VertexWeightMode)GUILayout.Toolbar((int)objectTarget.edit_vertexWeightMode, Edit_VertexWeightModeString);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(objectTarget, "Weight Mode");
                                        objectTarget.edit_vertexWeightMode = weightMode;
                                        ShowNotification();
                                        editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                    }
                                }
                                #endregion
                                #region BlendMode
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var blendMode = (VoxelSkinnedAnimationObject.Edit_BlendMode)GUILayout.Toolbar((int)objectTarget.edit_blendMode, Edit_BlendModeString);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(objectTarget, "Blend Mode");
                                        objectTarget.edit_blendMode = blendMode;
                                    }
                                }
                                #endregion
                                #region Weight
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        if (boneTarget.edit_weightColorTexture == null)
                                            boneTarget.edit_weightColorTexture = editorCommon.CreateColorTexture(GetWeightColor(objectTarget.edit_weight));
                                        editorCommon.guiStyleLabel.normal.background = boneTarget.edit_weightColorTexture;
                                        editorCommon.guiStyleLabel.normal.scaledBackgrounds = null;
                                        EditorGUILayout.LabelField("Weight", editorCommon.guiStyleLabel, GUILayout.Width(48));
                                        editorCommon.guiStyleLabel.normal.background = null;
                                    }
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var weight = GUILayout.HorizontalSlider(objectTarget.edit_weight, 0f, 1f);
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            weight = Mathf.Clamp(weight, 0f, 1f);
                                            Undo.RecordObject(objectTarget, "Weight");
                                            Undo.RecordObject(boneTarget, "Weight");
                                            objectTarget.edit_weight = weight;
                                            boneTarget.edit_weightColorTexture = editorCommon.CreateColorTexture(GetWeightColor(objectTarget.edit_weight));
                                        }
                                    }
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var weight = EditorGUILayout.FloatField(objectTarget.edit_weight, GUILayout.Width(57));
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            weight = Mathf.Clamp(weight, 0f, 1f);
                                            Undo.RecordObject(objectTarget, "Weight");
                                            Undo.RecordObject(boneTarget, "Weight");
                                            objectTarget.edit_weight = weight;
                                            boneTarget.edit_weightColorTexture = editorCommon.CreateColorTexture(GetWeightColor(objectTarget.edit_weight));
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                #endregion
                                #region Auto Normalize
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var edit_autoNormalize = EditorGUILayout.Toggle("Auto Normalize", objectTarget.edit_autoNormalize);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(objectTarget, "Auto Normalize");
                                        objectTarget.edit_autoNormalize = edit_autoNormalize;
                                    }
                                }
                                #endregion
                                #region BrushRadius
                                if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex &&
                                    objectTarget.edit_vertexWeightMode == VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Brush)
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Radius", GUILayout.Width(48));
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var edit_brushRadius = GUILayout.HorizontalSlider(objectTarget.edit_brushRadius, 1f, 100f);
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(objectTarget, "Radius");
                                            objectTarget.edit_brushRadius = edit_brushRadius;
                                        }
                                    }
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var edit_brushRadius = EditorGUILayout.FloatField(objectTarget.edit_brushRadius, GUILayout.Width(57));
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(objectTarget, "Radius");
                                            objectTarget.edit_brushRadius = edit_brushRadius;
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                #endregion
                                #region BrushCurve
                                if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex &&
                                    objectTarget.edit_vertexWeightMode == VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Brush)
                                {
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var curve = EditorGUILayout.CurveField("Curve", objectTarget.edit_brushCurve);
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(objectTarget, "Curve");
                                            objectTarget.edit_brushCurve = curve;
                                        }
                                    }
                                }
                                #endregion
                                #region Mirror
                                {
                                    EditorGUI.BeginDisabledGroup(boneTarget.mirrorBone == null);
                                    EditorGUI.BeginChangeCheck();
                                    var edit_mirrorSetBoneWeight = EditorGUILayout.ToggleLeft(string.Format("Set to mirror bone\n ({0})", boneTarget.mirrorBone != null ? boneTarget.mirrorBone.name : "none"),
                                                                                                boneTarget.edit_mirrorSetBoneWeight, GUILayout.Height(32));
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        Undo.RecordObject(boneTarget, "Mirror");
                                        boneTarget.edit_mirrorSetBoneWeight = edit_mirrorSetBoneWeight;
                                    }
                                    EditorGUI.EndDisabledGroup();
                                }
                                #endregion
                                #region WeightClear
                                {
                                    if (GUILayout.Button("Clear Bone Weight"))
                                    {
                                        Undo.RecordObject(boneTarget, "Clear");
                                        boneTarget.weightData.ClearWeight();
                                        boneCore.MirrorBoneWeight();
                                        UpdateEnableVoxel(false);
                                    }
                                }
                                #endregion
                                #region WeightPreviewMode
                                {
                                    {
                                        EditorGUI.BeginChangeCheck();
                                        var edit_WeightPreviewMode = (VoxelSkinnedAnimationObject.Edit_WeightPreviewMode)EditorGUILayout.EnumPopup("Preview", objectTarget.edit_WeightPreviewMode);
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(objectTarget, "Weight Preview Mode");
                                            objectTarget.edit_WeightPreviewMode = edit_WeightPreviewMode;
                                        }
                                    }
                                }
                                #endregion
                                #region Help
                                if (!objectTarget.edit_helpEnable)
                                {
                                    #region "?"
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        EditorGUILayout.Space();
                                        EditorGUI.BeginChangeCheck();
                                        GUILayout.Toggle(objectTarget.edit_helpEnable, "?", GUI.skin.button, GUILayout.Width(16));
                                        if (EditorGUI.EndChangeCheck())
                                        {
                                            Undo.RecordObject(objectTarget, "Help Enable");
                                            objectTarget.edit_helpEnable = !objectTarget.edit_helpEnable;
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    #endregion
                                }
                                else
                                {
                                    EditorGUI.BeginChangeCheck();
                                    objectTarget.edit_helpEnable = EditorGUILayout.Foldout(objectTarget.edit_helpEnable, "Help");
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                                    }
                                    EditorGUILayout.BeginVertical(GUI.skin.box);
                                    EditorGUILayout.LabelField("F5 Key - Refresh");
                                    EditorGUILayout.LabelField("Press Space Key - Hide Preview");
                                    EditorGUILayout.EndVertical();
                                }
                                #endregion

                                #region ToolTip
                                {
                                    if (!string.IsNullOrEmpty(GUI.tooltip))
                                    {
                                        var stringSize = GUI.skin.box.CalcSize(new GUIContent(GUI.tooltip));
                                        EditorGUI.LabelField(new Rect(e.mousePosition.x + 16, e.mousePosition.y, stringSize.x, stringSize.y), GUI.tooltip, GUI.skin.box);
                                    }
                                }
                                #endregion

                                GUI.DragWindow();

                            }, "Bone Weight Editor", guiStyleEditorWindow);
                            editorBoneEditorWindowRect = editorCommon.ResizeSceneViewRect(editorBoneEditorWindowRect);
                        }
                        #endregion

                        #region Cursor
                        if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex &&
                            objectTarget.edit_vertexWeightMode == VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Brush)
                        {
                            const float OneRadius = 162 / 2f;
                            Vector3 pos;
                            {
                                var ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                                pos = ray.origin + ray.direction;
                            }
                            float radius = objectTarget.edit_brushRadius / OneRadius * HandleUtility.GetHandleSize(pos);
                            Handles.color = GetWeightColor(objectTarget.edit_weight);
                            Handles.DrawWireDisc(pos, SceneView.currentDrawingSceneView.camera.transform.forward, radius);
                        }
                        #endregion
                    }
                }
            }

            if (repaint)
            {
                SceneView.currentDrawingSceneView.Repaint();
            }
        }

        protected void UpdateSilhouetteMeshMesh()
        {
            editorCommon.ClearSilhouetteMeshMesh();

            if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
            {
                List<VoxelData.Voxel> voxels = new List<VoxelData.Voxel>(objectCore.voxelData.voxels.Length);
                for (int i = 0; i < objectCore.voxelData.voxels.Length; i++)
                {
                    var voxel = objectCore.voxelData.voxels[i];
                    voxel.palette = -1;
                    voxels.Add(voxel);
                }
                if (voxels.Count > 0)
                {
                    editorCommon.silhouetteMesh = objectCore.Edit_CreateMesh(voxels, null, true);
                }
            }
        }
        private void UpdatePreviewMesh()
        {
            editorCommon.ClearPreviewMesh();
            {
                List<VoxelData.Voxel> voxels = new List<VoxelData.Voxel>();
                {
                    editWeightList.AllAction((x, y, z, w) =>
                    {
                        if (objectCore.voxelData.VoxelTableContains(x, y, z) < 0) return;
                        voxels.Add(new VoxelData.Voxel(x, y, z, -1));
                    });
                }
                List<VoxelObjectCore.Edit_VerticesInfo> infoList = new List<VoxelObjectCore.Edit_VerticesInfo>();
                editorCommon.previewMesh = objectCore.Edit_CreateMesh(voxels, infoList, false);
                for (int i = 0; i < editorCommon.previewMesh.Length; i++)
                {
                    Func<IntVector3, VoxelBase.VoxelVertexIndex, float[], float> GetWeight = (pos, index, power) =>
                    {
                        var w = objectTarget.edit_weight;
                        if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex &&
                            objectTarget.edit_vertexWeightMode == VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Brush)
                        {
                            w *= objectTarget.edit_brushCurve.Evaluate(power[(int)index]);
                        }
                        switch (objectTarget.edit_blendMode)
                        {
                        case VoxelSkinnedAnimationObject.Edit_BlendMode.Replace:
                            break;
                        case VoxelSkinnedAnimationObject.Edit_BlendMode.Add:
                            {
                                var tmp = boneTarget.weightData.GetWeight(pos);
                                if (tmp != null) w = tmp.GetWeight(index) + w;
                            }
                            break;
                        case VoxelSkinnedAnimationObject.Edit_BlendMode.Subtract:
                            {
                                var tmp = boneTarget.weightData.GetWeight(pos);
                                if (tmp != null) w = tmp.GetWeight(index) - w;
                                else w = 0f;
                            }
                            break;
                        default:
                            Assert.IsTrue(false);
                            break;
                        }
                        return Mathf.Clamp(w, 0f, 1f);
                    };
                    Color[] colors = new Color[editorCommon.previewMesh[i].vertexCount];
                    for (int j = 0; j < infoList.Count; j++)
                    {
                        var editWeight = editWeightList.Get(infoList[j].position);
                        float weight = -1f;
                        switch (infoList[j].vertexIndex)
                        {
                        case VoxelBase.VoxelVertexIndex.XYZ:
                            if ((editWeight.flags & VoxelBase.VoxelVertexFlags.XYZ) != 0)
                                weight = GetWeight(infoList[j].position, infoList[j].vertexIndex, editWeight.power);
                            break;
                        case VoxelBase.VoxelVertexIndex._XYZ:
                            if ((editWeight.flags & VoxelBase.VoxelVertexFlags._XYZ) != 0)
                                weight = GetWeight(infoList[j].position, infoList[j].vertexIndex, editWeight.power);
                            break;
                        case VoxelBase.VoxelVertexIndex.X_YZ:
                            if ((editWeight.flags & VoxelBase.VoxelVertexFlags.X_YZ) != 0)
                                weight = GetWeight(infoList[j].position, infoList[j].vertexIndex, editWeight.power);
                            break;
                        case VoxelBase.VoxelVertexIndex.XY_Z:
                            if ((editWeight.flags & VoxelBase.VoxelVertexFlags.XY_Z) != 0)
                                weight = GetWeight(infoList[j].position, infoList[j].vertexIndex, editWeight.power);
                            break;
                        case VoxelBase.VoxelVertexIndex._X_YZ:
                            if ((editWeight.flags & VoxelBase.VoxelVertexFlags._X_YZ) != 0)
                                weight = GetWeight(infoList[j].position, infoList[j].vertexIndex, editWeight.power);
                            break;
                        case VoxelBase.VoxelVertexIndex._XY_Z:
                            if ((editWeight.flags & VoxelBase.VoxelVertexFlags._XY_Z) != 0)
                                weight = GetWeight(infoList[j].position, infoList[j].vertexIndex, editWeight.power);
                            break;
                        case VoxelBase.VoxelVertexIndex.X_Y_Z:
                            if ((editWeight.flags & VoxelBase.VoxelVertexFlags.X_Y_Z) != 0)
                                weight = GetWeight(infoList[j].position, infoList[j].vertexIndex, editWeight.power);
                            break;
                        case VoxelBase.VoxelVertexIndex._X_Y_Z:
                            if ((editWeight.flags & VoxelBase.VoxelVertexFlags._X_Y_Z) != 0)
                                weight = GetWeight(infoList[j].position, infoList[j].vertexIndex, editWeight.power);
                            break;
                        }
                        if (weight >= 0f)
                            colors[j] = GetWeightColor(weight);
                        else
                            colors[j] = Color.clear;
                    }
                    editorCommon.previewMesh[i].colors = colors;
                }
            }
        }
        private void UpdateCursorMesh()
        {
            editorCommon.ClearCursorMesh();
            if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Voxel)
            {
                switch (objectTarget.edit_voxelWeightMode)
                {
                case VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Voxel:
                    {
                        var result = editorCommon.GetMousePositionVoxel();
                        if (result.HasValue)
                        {
                            editorCommon.cursorMesh = objectCore.Edit_CreateMesh(new List<VoxelData.Voxel>() { new VoxelData.Voxel(result.Value.x, result.Value.y, result.Value.z, -1) });
                        }
                    }
                    break;
                case VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Fill:
                    {
                        var pos = editorCommon.GetMousePositionVoxel();
                        if (pos.HasValue)
                        {
                            var faceAreaTable = editorCommon.GetFillVoxelFaceAreaTable(pos.Value);
                            if (faceAreaTable != null)
                                editorCommon.cursorMesh = new Mesh[1] { objectCore.Edit_CreateMeshOnly_Mesh(faceAreaTable, null, null) };
                        }
                    }
                    break;
                }
            }
        }

        private void ClearMakeAddData()
        {
            editWeightList.Clear();
            editorCommon.selectionRect.Reset();
            editorCommon.ClearPreviewMesh();
            editorCommon.ClearCursorMesh();
        }

        private void EventMouseDrag(bool first)
        {
            UpdateCursorMesh();
            if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Voxel)
            {
                #region Voxel
                switch (objectTarget.edit_voxelWeightMode)
                {
                case VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Voxel:
                    {
                        var result = editorCommon.GetMousePositionVoxel();
                        if (result.HasValue)
                        {
                            if (!editWeightList.Contains(result.Value))
                            {
                                editWeightList.Set(result.Value, new EditWeight());
                                UpdatePreviewMesh();
                            }
                        }
                    }
                    break;
                case VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Fill:
                    {
                        var pos = editorCommon.GetMousePositionVoxel();
                        if (pos.HasValue)
                        {
                            var result = editorCommon.GetFillVoxel(pos.Value);
                            if (result != null)
                            {
                                for (int i = 0; i < result.Count; i++)
                                {
                                    if (!editWeightList.Contains(result[i]))
                                    {
                                        editWeightList.Set(result[i], new EditWeight());
                                    }
                                }
                                UpdatePreviewMesh();
                            }
                        }
                    }
                    break;
                case VoxelSkinnedAnimationObject.Edit_VoxelWeightMode.Rect:
                    {
                        var pos = new IntVector2((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
                        if (first) { editorCommon.selectionRect.Reset(); editorCommon.selectionRect.SetStart(pos); }
                        else editorCommon.selectionRect.SetEnd(pos);
                        //
                        editWeightList.Clear();
                        {
                            var list = editorCommon.GetSelectionRectVoxel();
                            for (int i = 0; i < list.Count; i++)
                            {
                                editWeightList.Set(list[i], new EditWeight());
                            }
                        }
                        UpdatePreviewMesh();
                    }
                    break;
                }
                #endregion
            }
            else if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex)
            {
                #region Vertex
                Action<IntVector3, float> AddEditWeightList = (basePos, power) =>
                {
                    Action<IntVector3, VoxelBase.VoxelVertexFlags> AddEditWeightPosition = (pos, flags) =>
                    {
                        int index = -1;
                        switch (flags)
                        {
                        case VoxelBase.VoxelVertexFlags.XYZ: index = (int)VoxelBase.VoxelVertexIndex.XYZ; break;
                        case VoxelBase.VoxelVertexFlags._XYZ: index = (int)VoxelBase.VoxelVertexIndex._XYZ; break;
                        case VoxelBase.VoxelVertexFlags.X_YZ: index = (int)VoxelBase.VoxelVertexIndex.X_YZ; break;
                        case VoxelBase.VoxelVertexFlags.XY_Z: index = (int)VoxelBase.VoxelVertexIndex.XY_Z; break;
                        case VoxelBase.VoxelVertexFlags._X_YZ: index = (int)VoxelBase.VoxelVertexIndex._X_YZ; break;
                        case VoxelBase.VoxelVertexFlags._XY_Z: index = (int)VoxelBase.VoxelVertexIndex._XY_Z; break;
                        case VoxelBase.VoxelVertexFlags.X_Y_Z: index = (int)VoxelBase.VoxelVertexIndex.X_Y_Z; break;
                        case VoxelBase.VoxelVertexFlags._X_Y_Z: index = (int)VoxelBase.VoxelVertexIndex._X_Y_Z; break;
                        default: Assert.IsTrue(false); break;
                        }
                        if (objectCore.voxelData.VoxelTableContains(pos) >= 0)
                        {
                            if (editWeightList.Contains(pos))
                            {
                                var editWeight = editWeightList.Get(pos);
                                editWeight.flags |= flags;
                                editWeight.power[index] = power;
                            }
                            else
                            {
                                var editWeight = new EditWeight(flags, 0f);
                                editWeight.power[index] = power;
                                editWeightList.Set(pos, editWeight);
                            }
                        }
                    };

                    AddEditWeightPosition(new IntVector3(basePos.x - 1, basePos.y - 1, basePos.z - 1), VoxelBase.VoxelVertexFlags.XYZ);
                    AddEditWeightPosition(new IntVector3(basePos.x, basePos.y - 1, basePos.z - 1), VoxelBase.VoxelVertexFlags._XYZ);
                    AddEditWeightPosition(new IntVector3(basePos.x - 1, basePos.y, basePos.z - 1), VoxelBase.VoxelVertexFlags.X_YZ);
                    AddEditWeightPosition(new IntVector3(basePos.x - 1, basePos.y - 1, basePos.z), VoxelBase.VoxelVertexFlags.XY_Z);
                    AddEditWeightPosition(new IntVector3(basePos.x, basePos.y, basePos.z - 1), VoxelBase.VoxelVertexFlags._X_YZ);
                    AddEditWeightPosition(new IntVector3(basePos.x, basePos.y - 1, basePos.z), VoxelBase.VoxelVertexFlags._XY_Z);
                    AddEditWeightPosition(new IntVector3(basePos.x - 1, basePos.y, basePos.z), VoxelBase.VoxelVertexFlags.X_Y_Z);
                    AddEditWeightPosition(basePos, VoxelBase.VoxelVertexFlags._X_Y_Z);
                };
                switch (objectTarget.edit_vertexWeightMode)
                {
                case VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Brush:
                    {
                        var vertexList = editorCommon.GetMousePositionVertex(objectTarget.edit_brushRadius);
                        for (int i = 0; i < vertexList.Count; i++)
                        {
                            AddEditWeightList(vertexList[i].position, vertexList[i].power);
                        }
                        EventMouseApply();
                    }
                    break;
                case VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Rect:
                    {
                        var pos = new IntVector2((int)Event.current.mousePosition.x, (int)Event.current.mousePosition.y);
                        if (first) { editorCommon.selectionRect.Reset(); editorCommon.selectionRect.SetStart(pos); }
                        else editorCommon.selectionRect.SetEnd(pos);
                        //
                        editWeightList.Clear();
                        {
                            var list = editorCommon.GetSelectionRectVertex();
                            for (int i = 0; i < list.Count; i++)
                            {
                                AddEditWeightList(list[i], 1f);
                            }
                        }
                        UpdatePreviewMesh();
                    }
                    break;
                }
                #endregion
            }
        }
        private void EventMouseApply()
        {
            if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
            {
                bool update = false;

                {
                    Undo.RecordObject(objectTarget, "Weight");
                    Undo.RecordObjects(objectTarget.bones, "Weight");

                    WeightData.VoxelWeight weight = new WeightData.VoxelWeight();
                    Action<IntVector3, VoxelBase.VoxelVertexIndex, float[]> SetWeight = (pos, index, power) =>
                    {
                        var w = objectTarget.edit_weight;
                        var powerR = power[(int)index];
                        if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex &&
                            objectTarget.edit_vertexWeightMode == VoxelSkinnedAnimationObject.Edit_VertexWeightMode.Brush)
                        {
                            powerR = objectTarget.edit_brushCurve.Evaluate(power[(int)index]);
                            w *= powerR;
                        }
                        switch (objectTarget.edit_blendMode)
                        {
                        case VoxelSkinnedAnimationObject.Edit_BlendMode.Replace:
                            break;
                        case VoxelSkinnedAnimationObject.Edit_BlendMode.Add:
                            w = weight.GetWeight(index) + w;
                            break;
                        case VoxelSkinnedAnimationObject.Edit_BlendMode.Subtract:
                            w = weight.GetWeight(index) - w;
                            break;
                        default:
                            Assert.IsTrue(false);
                            break;
                        }
                        w = Mathf.Clamp(w, 0f, 1f);
                        if (objectTarget.edit_autoNormalize)
                        {
                            for (int m = 0; m < 2; m++)
                            {
                                IntVector3 posTmp = IntVector3.zero;
                                VoxelBase.VoxelVertexIndex indexTmp = 0;
                                if (m == 0)
                                {
                                    posTmp = pos;
                                    indexTmp = index;
                                }
                                else
                                {
                                    if (!boneTarget.edit_mirrorSetBoneWeight || boneTarget.mirrorBone == null) break;
                                    posTmp = boneCore.GetMirrorVoxelPosition(pos);
                                    switch (index)
                                    {
                                    case VoxelBase.VoxelVertexIndex.XYZ: indexTmp = VoxelBase.VoxelVertexIndex._XYZ; break;
                                    case VoxelBase.VoxelVertexIndex._XYZ: indexTmp = VoxelBase.VoxelVertexIndex.XYZ; break;
                                    case VoxelBase.VoxelVertexIndex.X_YZ: indexTmp = VoxelBase.VoxelVertexIndex._X_YZ; break;
                                    case VoxelBase.VoxelVertexIndex.XY_Z: indexTmp = VoxelBase.VoxelVertexIndex._XY_Z; break;
                                    case VoxelBase.VoxelVertexIndex._X_YZ: indexTmp = VoxelBase.VoxelVertexIndex.X_YZ; break;
                                    case VoxelBase.VoxelVertexIndex._XY_Z: indexTmp = VoxelBase.VoxelVertexIndex.XY_Z; break;
                                    case VoxelBase.VoxelVertexIndex.X_Y_Z: indexTmp = VoxelBase.VoxelVertexIndex._X_Y_Z; break;
                                    case VoxelBase.VoxelVertexIndex._X_Y_Z: indexTmp = VoxelBase.VoxelVertexIndex.X_Y_Z; break;
                                    default: Assert.IsTrue(false); break;
                                    }
                                }

                                float subPower = 0f;
                                for (int i = 0; i < objectTarget.bones.Length; i++)
                                {
                                    if (objectTarget.bones[i] == boneTarget) continue;
                                    var subWeight = objectTarget.bones[i].weightData.GetWeight(posTmp);
                                    if (subWeight == null) continue;
                                    subPower += subWeight.GetWeight(indexTmp);
                                }
                                if (subPower > 0f)
                                {
                                    float subRate = (1f - w) / subPower;
                                    for (int i = 0; i < objectTarget.bones.Length; i++)
                                    {
                                        if (objectTarget.bones[i] == boneTarget) continue;
                                        var subWeight = objectTarget.bones[i].weightData.GetWeight(posTmp);
                                        if (subWeight == null) continue;
                                        subWeight.SetWeight(indexTmp, subWeight.GetWeight(indexTmp) * subRate);
                                    }
                                }
                            }
                        }
                        weight.SetWeight(index, w);
                    };
                    editWeightList.AllAction((x, y, z, w) =>
                    {
                        if (!update)
                            DisconnectPrefabInstance();

                        var pos = new IntVector3(x, y, z);
                        {
                            var tmp = boneTarget.weightData.GetWeight(pos);
                            if (tmp != null) weight = tmp;
                            else weight = new WeightData.VoxelWeight();
                        }
                        if ((w.flags & VoxelBase.VoxelVertexFlags.XYZ) != 0)
                            SetWeight(pos, VoxelBase.VoxelVertexIndex.XYZ, w.power);
                        if ((w.flags & VoxelBase.VoxelVertexFlags._XYZ) != 0)
                            SetWeight(pos, VoxelBase.VoxelVertexIndex._XYZ, w.power);
                        if ((w.flags & VoxelBase.VoxelVertexFlags.X_YZ) != 0)
                            SetWeight(pos, VoxelBase.VoxelVertexIndex.X_YZ, w.power);
                        if ((w.flags & VoxelBase.VoxelVertexFlags.XY_Z) != 0)
                            SetWeight(pos, VoxelBase.VoxelVertexIndex.XY_Z, w.power);
                        if ((w.flags & VoxelBase.VoxelVertexFlags._X_YZ) != 0)
                            SetWeight(pos, VoxelBase.VoxelVertexIndex._X_YZ, w.power);
                        if ((w.flags & VoxelBase.VoxelVertexFlags._XY_Z) != 0)
                            SetWeight(pos, VoxelBase.VoxelVertexIndex._XY_Z, w.power);
                        if ((w.flags & VoxelBase.VoxelVertexFlags.X_Y_Z) != 0)
                            SetWeight(pos, VoxelBase.VoxelVertexIndex.X_Y_Z, w.power);
                        if ((w.flags & VoxelBase.VoxelVertexFlags._X_Y_Z) != 0)
                            SetWeight(pos, VoxelBase.VoxelVertexIndex._X_Y_Z, w.power);
                        boneTarget.weightData.SetWeight(pos, weight);
                        update = true;
                    });
                    editWeightList.Clear();
                    if (update)
                    {
                        boneCore.MirrorBoneWeight();
                        UpdateEnableVoxel(false);
                    }
                }
            }
        }

        private void ShowNotification()
        {
            if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Voxel)
                SceneView.currentDrawingSceneView.ShowNotification(new GUIContent(string.Format("{0} - {1}", objectTarget.edit_voxelMode, objectTarget.edit_voxelWeightMode)));
            else if (objectTarget.edit_voxelMode == VoxelSkinnedAnimationObject.Edit_VoxelMode.Vertex)
                SceneView.currentDrawingSceneView.ShowNotification(new GUIContent(string.Format("{0} - {1}", objectTarget.edit_voxelMode, objectTarget.edit_vertexWeightMode)));
        }

        private void GuiBoneButton()
        {
            Handles.BeginGUI();
            for (int i = 0; i < objectTarget.bones.Length; i++)
            {
                if (objectTarget.bones[i] == null) continue;
                if (Selection.activeGameObject == objectTarget.bones[i].gameObject) continue;

                Vector3 pos;
                if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition ||
                    objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
                {
                    var tr = objectTarget.bones[i];
                    pos = (objectTarget.bindposes[tr.boneIndex] * objectTarget.transform.worldToLocalMatrix).inverse.GetColumn(3);
                }
                else
                {
                    pos = objectTarget.bones[i].transform.position;
                }

                var screen = HandleUtility.WorldToGUIPoint(pos);

                const int Size = 16;
                EditorGUI.BeginChangeCheck();
                {
                    var rect = new Rect(screen.x + 2 - Size / 2f, screen.y - 2 - Size / 2f, Size, Size);    //Why is it shifted. I do not know the cause 2
                    GUI.Button(rect, "", guiStyleCircleButton);
                    editorCommon.editorRectList.Add(rect);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Selection.activeGameObject = objectTarget.bones[i].gameObject;
                    EditorGUIUtility.PingObject(Selection.activeGameObject);
                    editorBoneEditorWindowRect.width = editorBoneEditorWindowRect.height = 0;
                    break;
                }
            }
            Handles.EndGUI();
        }

        private void DrawBoneArrow(Transform t)
        {
            for (int i = 0; i < t.childCount; i++)
            {
                var ct = t.GetChild(i);
                var ctr = ct.GetComponent<VoxelSkinnedAnimationObjectBone>();
                if (ctr != null)
                {
                    Vector3 posA, posB;
                    if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition ||
                        objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
                    {
                        var tr = t.GetComponent<VoxelSkinnedAnimationObjectBone>();
                        posA = (objectTarget.bindposes[tr.boneIndex] * objectTarget.transform.worldToLocalMatrix).inverse.GetColumn(3);
                        posB = (objectTarget.bindposes[ctr.boneIndex] * objectTarget.transform.worldToLocalMatrix).inverse.GetColumn(3);
                    }
                    else
                    {
                        posA = t.position;
                        posB = ct.position;
                    }
                    var vec = posB - posA;
                    if (vec.sqrMagnitude > 0f)
                    {
                        var cam = UnityEditor.SceneView.currentDrawingSceneView.camera.transform.forward;
                        var cross = Vector3.Cross(vec, cam);
                        cross.Normalize();
                        vec.Normalize();
                        float radius = HandleUtility.GetHandleSize(posA) * (16f / 200f);
                        var saveColor = Handles.color;
                        Handles.color = boneTarget == ctr ? Color.yellow : Color.green;
                        if (skeletonLines == null || skeletonLines.Length != 5)
                            skeletonLines = new Vector3[5];
                        skeletonLines[0] = posA;
                        skeletonLines[1] = posA + cross * radius + vec * radius;
                        skeletonLines[2] = posB;
                        skeletonLines[3] = posA - cross * radius + vec * radius;
                        skeletonLines[4] = skeletonLines[0];
                        Handles.DrawPolyLine(skeletonLines);
                        Handles.color = saveColor;
                    }
                    DrawBoneArrow(ct);
                }
            }
        }

        private void UpdateEnableVoxel(bool updateMesh = true)
        {
            if (boneTarget == null || objectTarget == null || rootTarget == null) return;

            if (updateMesh)
            {
                objectCore.ReCreate();
            }
            else
            {
                var boneCount = objectTarget.bones.Length;
                objectCore.UpdateBoneWeight();
                if (boneCount != objectTarget.bones.Length)
                {
                    objectCore.ReCreate();
                }
                else
                {
                    objectTarget.edit_afterRefresh = true;
                }
            }

            UpdateSilhouetteMeshMesh();
            UpdateWeightPreviewMesh();
        }
        private void UpdateWeightPreviewMesh()
        {
            #region WeightMesh
            if (objectTarget.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BoneWeight)
            {
                List<VoxelObjectCore.Edit_VerticesInfo> infoList = new List<VoxelObjectCore.Edit_VerticesInfo>();
                {
                    List<VoxelData.Voxel> voxels = new List<VoxelData.Voxel>();
                    if (boneTarget == rootTarget)
                    {
                        for (int i = 0; i < objectCore.voxelData.voxels.Length; i++)
                        {
                            var pos = objectCore.voxelData.voxels[i].position;
                            bool hasWeight = false;
                            for (var vindex = (VoxelBase.VoxelVertexIndex)0; vindex < VoxelBase.VoxelVertexIndex.Total; vindex++)
                            {
                                var weight = objectCore.GetBoneWeight(pos, vindex);
                                var power = 0f;
                                if (weight.boneIndex0 == 0 && weight.weight0 > 0f)
                                    power = weight.weight0;
                                else if (weight.boneIndex1 == 0 && weight.weight1 > 0f)
                                    power = weight.weight1;
                                else if (weight.boneIndex2 == 0 && weight.weight2 > 0f)
                                    power = weight.weight2;
                                else if (weight.boneIndex3 == 0 && weight.weight3 > 0f)
                                    power = weight.weight3;
                                if (power <= 0f) continue;
                                hasWeight = true;
                                break;
                            }
                            if (hasWeight)
                                voxels.Add(new VoxelData.Voxel(pos.x, pos.y, pos.z, -1));
                        }
                    }
                    else
                    {
                        boneTarget.weightData.AllAction((pos, weights) =>
                        {
                            if (objectCore.voxelData.VoxelTableContains(pos) < 0)
                                return;
                            voxels.Add(new VoxelData.Voxel(pos.x, pos.y, pos.z, -1));
                        });
                    }

                    boneTarget.edit_weightMesh = objectCore.Edit_CreateMesh(voxels, infoList);
                }
                for (int i = 0; i < boneTarget.edit_weightMesh.Length; i++)
                {
                    Color[] colors = new Color[boneTarget.edit_weightMesh[i].vertexCount];
                    if (boneTarget == rootTarget)
                    {
                        for (int j = 0; j < infoList.Count; j++)
                        {
                            var weight = objectCore.GetBoneWeight(infoList[j].position, infoList[j].vertexIndex);
                            var power = 0f;
                            if (weight.boneIndex0 == 0 && weight.weight0 > 0f)
                                power = weight.weight0;
                            else if (weight.boneIndex1 == 0 && weight.weight1 > 0f)
                                power = weight.weight1;
                            else if (weight.boneIndex2 == 0 && weight.weight2 > 0f)
                                power = weight.weight2;
                            else if (weight.boneIndex3 == 0 && weight.weight3 > 0f)
                                power = weight.weight3;
                            colors[j] = GetWeightColor(power);
                        }
                    }
                    else
                    {
                        for (int j = 0; j < infoList.Count; j++)
                        {
                            var weight = boneTarget.weightData.GetWeight(infoList[j].position);
                            Assert.IsNotNull(weight);
                            colors[j] = GetWeightColor(weight.GetWeight(infoList[j].vertexIndex));
                        }
                    }
                    boneTarget.edit_weightMesh[i].colors = colors;
                }
            }
            else
            {
                if (boneTarget.edit_weightMesh != null)
                {
                    for (int i = 0; i < boneTarget.edit_weightMesh.Length; i++)
                    {
                        DestroyImmediate(boneTarget.edit_weightMesh[i]);
                    }
                    boneTarget.edit_weightMesh = null;
                }
            }
            #endregion
        }

        private Color GetWeightColor(float weight, float BaseColor = 0.7f)
        {
            if (weight >= 0.75f)
            {
                return Color.Lerp(new Color(BaseColor, BaseColor, 0, 1), new Color(BaseColor, 0, 0, 1), (weight - 0.75f) / 0.25f);
            }
            else if (weight >= 0.5f)
            {
                return Color.Lerp(new Color(0, BaseColor, 0, 1), new Color(BaseColor, BaseColor, 0, 1), (weight - 0.5f) / 0.25f);
            }
            else if (weight >= 0.25f)
            {
                return Color.Lerp(new Color(0, BaseColor, BaseColor, 1), new Color(0, BaseColor, 0, 1), (weight - 0.25f) / 0.25f);
            }
            else
            {
                return Color.Lerp(new Color(0, 0, BaseColor, 1), new Color(0, BaseColor, BaseColor, 1), (weight) / 0.25f);
            }
        }

        private int EditorOnCurveWasModifiedStack = 0;
        private void EditorOnCurveWasModified(AnimationClip clip, EditorCurveBinding binding, AnimationUtility.CurveModifiedType deleted)
        {
            if (boneTarget == null)
                return;

            if (objectTarget.editMode != VoxelSkinnedAnimationObject.Edit_Mode.BoneAnimation) return;

            if (boneTarget != null && boneTarget.voxelObject != null && boneTarget.voxelObject.bones != null)
            {
                if (EditorOnCurveWasModifiedStack++ == 0)
                {
                    VoxelSkinnedAnimationObjectBone boneTmp = null;
                    VoxelSkinnedAnimationObjectBoneCore boneCoreTmp = null;
                    for (int i = 0; i < boneTarget.voxelObject.bones.Length; i++)
                    {
                        boneCoreTmp = new VoxelSkinnedAnimationObjectBoneCore(boneTarget.voxelObject.bones[i], objectCore);
                        if (boneCoreTmp.fullPathBoneName == binding.path)
                        {
                            boneTmp = boneTarget.voxelObject.bones[i];
                            break;
                        }
                    }
                    if (boneTmp != null && boneCoreTmp != null)
                    {
                        if (deleted == AnimationUtility.CurveModifiedType.CurveModified)
                        {
                            if (boneTmp.edit_disablePositionAnimation || boneTmp.edit_disableRotationAnimation || boneTmp.edit_disableScaleAnimation)
                            {
                                if ((boneTmp.edit_disablePositionAnimation && binding.propertyName.StartsWith("m_LocalPosition.")) ||
                                    (boneTmp.edit_disableRotationAnimation && binding.propertyName.StartsWith("localEulerAnglesRaw.")) ||
                                    (boneTmp.edit_disableScaleAnimation && binding.propertyName.StartsWith("m_LocalScale.")))
                                {
                                    AnimationUtility.SetEditorCurve(clip, binding, null);
                                }
                            }
                        }

                        boneCoreTmp.MirroringAnimation();
                    }
                }
                EditorOnCurveWasModifiedStack--;
            }
        }

        private void EditorUndoRedoPerformed()
        {
            UpdateWeightPreviewMesh();
        }

        private void DisconnectPrefabInstance()
        {
#if !UNITY_2018_3_OR_NEWER
            if (PrefabUtility.GetPrefabType(boneTarget) == PrefabType.PrefabInstance)
            {
                PrefabUtility.DisconnectPrefabInstance(boneTarget);
            }
#endif
        }
    }
}
