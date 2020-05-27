using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEditorInternal;
using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace VoxelImporter
{
    [CustomEditor(typeof(VoxelSkinnedAnimationObject))]
    public class VoxelSkinnedAnimationObjectEditor : VoxelObjectEditor
    {
        public VoxelSkinnedAnimationObject animationTarget { get; private set; }
        public VoxelSkinnedAnimationObjectCore animationCore { get; protected set; }

        public override Mesh mesh { get { return animationTarget.mesh; } set { animationTarget.mesh = value; } }
        public override List<Material> materials { get { return animationTarget.materials; } set { animationTarget.materials = value; } }
        public override Texture2D atlasTexture { get { return animationTarget.atlasTexture; } set { animationTarget.atlasTexture = value; } }

        protected override void OnEnable()
        {
            base.OnEnable();

            animationTarget = target as VoxelSkinnedAnimationObject;
            if (animationTarget == null) return;
            baseCore = objectCore = animationCore = new VoxelSkinnedAnimationObjectCore(animationTarget);
            OnEnableInitializeSet();
        }

        protected override void InspectorGUI()
        {
            if (animationTarget == null) return;

            base.InspectorGUI();

#if UNITY_2018_3_OR_NEWER
            {
                if (!baseCore.isPrefabEditable)
                {
                    EditorGUI.BeginDisabledGroup(true);
                }
            }
#endif
            
            #region Animation
            if (!string.IsNullOrEmpty(baseTarget.voxelFilePath))
            {
                animationTarget.edit_animationFoldout = EditorGUILayout.Foldout(animationTarget.edit_animationFoldout, "Animation", guiStyleFoldoutBold);
                if (animationTarget.edit_animationFoldout)
                {
                    EditorGUILayout.BeginVertical(editorCommon.guiStyleSkinBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUIStyle style = EditorStyles.boldLabel;
                            if (animationTarget.rootBone == null)
                                style = guiStyleMagentaBold;
                            EditorGUILayout.LabelField("Bone", style);
                        }
                        {
                            EditorGUI.BeginDisabledGroup(animationTarget.rootBone == null);
                            if (GUILayout.Button("Save as template", GUILayout.Width(128)))
                            {
                                #region Save as template
                                string BoneTemplatesPath = Application.dataPath + "/VoxelImporter/Scripts/Editor/BoneTemplates";
                                if (!Directory.Exists(BoneTemplatesPath))
                                {
                                    BoneTemplatesPath = Application.dataPath;
                                }
                                string path = EditorUtility.SaveFilePanel("Save as template", BoneTemplatesPath, string.Format("{0}.asset", baseTarget.gameObject.name), "asset");
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (path.IndexOf(Application.dataPath) < 0)
                                    {
                                        EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                                    }
                                    else
                                    {
                                        path = FileUtil.GetProjectRelativePath(path);
                                        var boneTemplate = ScriptableObject.CreateInstance<BoneTemplate>();
                                        boneTemplate.Set(animationTarget.rootBone);
                                        AssetDatabase.CreateAsset(boneTemplate, path);
                                    }
                                }
                                #endregion
                            }
                            EditorGUI.EndDisabledGroup();
                            if (GUILayout.Button("Create", guiStyleDropDown, GUILayout.Width(64)))
                            {
                                #region Create
                                VoxelHumanoidConfigreAvatar.Destroy();

                                Dictionary<string, BoneTemplate> boneTemplates = new Dictionary<string, BoneTemplate>();
                                {
                                    {
                                        var boneTemplate = ScriptableObject.CreateInstance<BoneTemplate>();
                                        boneTemplate.boneInitializeData.Add(new BoneTemplate.BoneInitializeData() { name = "Root" });
                                        boneTemplate.boneInitializeData.Add(new BoneTemplate.BoneInitializeData() { name = "Bone", parentName = "Root", position = new Vector3(0f, 2f, 0f) });
                                        boneTemplates.Add("Default", boneTemplate);
                                    }
                                    {
                                        var guids = AssetDatabase.FindAssets("t:bonetemplate");
                                        for (int i = 0; i < guids.Length; i++)
                                        {
                                            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                                            var boneTemplate = AssetDatabase.LoadAssetAtPath<BoneTemplate>(path);
                                            if (boneTemplate == null) continue;
                                            var name = path.Remove(0, "Assets/".Length);
                                            boneTemplates.Add(name, boneTemplate);
                                        }
                                    }
                                }

                                Action<BoneTemplate> MenuCallback = (boneTemplate) =>
                                {
                                    GameObject goRoot = baseTarget.gameObject;
                                    VoxelBase clRoot = baseTarget;

#if !UNITY_2018_3_OR_NEWER
                                    if (isPrefab)
                                    {
                                        goRoot = (GameObject)PrefabUtility.InstantiatePrefab(baseTarget.gameObject);
                                        clRoot = goRoot.GetComponent<VoxelBase>();
                                    }
#endif
                                    {
                                        var bones = clRoot.GetComponentsInChildren<VoxelSkinnedAnimationObjectBone>();
                                        for (int i = 0; i < bones.Length; i++)
                                        {
                                            for (int j = 0; j < bones[i].transform.childCount; j++)
                                            {
                                                var child = bones[i].transform.GetChild(j);
                                                if (child.GetComponent<VoxelSkinnedAnimationObjectBone>() == null)
                                                {
                                                    Undo.SetTransformParent(child, animationTarget.transform, "Create Bone");
                                                    i--;
                                                }
                                            }
                                        }
                                        for (int i = 0; i < bones.Length; i++)
                                        {
                                            if (bones[i] == null || bones[i].gameObject == null) continue;
                                            Undo.DestroyObjectImmediate(bones[i].gameObject);
                                        }
                                    }

                                    {
                                        List<GameObject> createList = new List<GameObject>();
                                        for (int i = 0; i < boneTemplate.boneInitializeData.Count; i++)
                                        {
                                            var tp = boneTemplate.boneInitializeData[i];
                                            GameObject go = new GameObject(tp.name);
                                            Undo.RegisterCreatedObjectUndo(go, "Create Bone");
                                            var bone = Undo.AddComponent<VoxelSkinnedAnimationObjectBone>(go);
                                            {
                                                bone.edit_disablePositionAnimation = tp.disablePositionAnimation;
                                                bone.edit_disableRotationAnimation = tp.disableRotationAnimation;
                                                bone.edit_disableScaleAnimation = tp.disableScaleAnimation;
                                                bone.edit_mirrorSetBoneAnimation = tp.mirrorSetBoneAnimation;
                                                bone.edit_mirrorSetBonePosition = tp.mirrorSetBonePosition;
                                                bone.edit_mirrorSetBoneWeight = tp.mirrorSetBoneWeight;
                                            }
                                            if (string.IsNullOrEmpty(tp.parentName))
                                            {
                                                Undo.SetTransformParent(go.transform, goRoot.transform, "Create Bone");
                                            }
                                            else
                                            {
                                                int parentIndex = createList.FindIndex(a => a.name == tp.parentName);
                                                Debug.Assert(parentIndex >= 0);
                                                GameObject parent = createList[parentIndex];
                                                Assert.IsNotNull(parent);
                                                Undo.SetTransformParent(go.transform, parent.transform, "Create Bone");
                                            }
                                            go.transform.localPosition = tp.position;
                                            go.transform.localRotation = Quaternion.identity;
                                            go.transform.localScale = Vector3.one;
                                            createList.Add(go);
                                        }
                                    }
                                    animationTarget.humanDescription.firstAutomapDone = false;
                                    Refresh();

#if !UNITY_2018_3_OR_NEWER
                                    if (isPrefab)
                                    {
#if UNITY_2018_2_OR_NEWER
                                        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(goRoot);
#else
                                        var prefab = PrefabUtility.GetPrefabParent(goRoot);
#endif
                                        PrefabUtility.ReplacePrefab(goRoot, prefab, ReplacePrefabOptions.ConnectToPrefab);
                                        DestroyImmediate(goRoot);
                                    }
#endif
                                };
                                GenericMenu menu = new GenericMenu();
                                {
                                    var enu = boneTemplates.GetEnumerator();
                                    while (enu.MoveNext())
                                    {
                                        var value = enu.Current.Value;
                                        menu.AddItem(new GUIContent(enu.Current.Key), false, () =>
                                        {
                                            MenuCallback(value);
                                        });
                                    }
                                }
                                menu.ShowAsContext();
                                #endregion
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        if (baseTarget.advancedMode)
                        {
                            EditorGUI.indentLevel++;
                            if (animationTarget.rootBone != null)
                            {
                                #region Root
                                {
                                    EditorGUI.BeginDisabledGroup(isPrefab);
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("Root");
                                        #region Add Root Bone
                                        {
                                            if (GUILayout.Button("Add Root Bone"))
                                            {
                                                var beforeRoot = animationTarget.rootBone.GetComponent<VoxelSkinnedAnimationObjectBone>();
                                                Undo.RecordObject(beforeRoot, "Add Root Bone");
                                                GameObject go = new GameObject("Root");
                                                Undo.RegisterCreatedObjectUndo(go, "Add Root Bone");
                                                Undo.AddComponent<VoxelSkinnedAnimationObjectBone>(go);
                                                Undo.SetTransformParent(go.transform, animationTarget.transform, "Add Root Bone");
                                                go.transform.localPosition = Vector3.zero;
                                                go.transform.localRotation = Quaternion.identity;
                                                go.transform.localScale = Vector3.one;
                                                Undo.SetTransformParent(animationTarget.rootBone, go.transform, "Add Root Bone");
                                                EditorGUIUtility.PingObject(go);
                                                animationCore.UpdateBoneWeight();
                                                animationCore.FixMissingAnimation();
                                                #region FixBoneWeight
                                                for (int i = 0; i < animationTarget.voxelData.voxels.Length; i++)
                                                {
                                                    var pos = animationTarget.voxelData.voxels[i].position;
                                                    for (var vindex = (VoxelBase.VoxelVertexIndex)0; vindex < VoxelBase.VoxelVertexIndex.Total; vindex++)
                                                    {
                                                        var weight = animationCore.GetBoneWeight(pos, vindex);
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
                                                        var weights = beforeRoot.weightData.GetWeight(pos);
                                                        if (weights == null)
                                                            weights = new WeightData.VoxelWeight();
                                                        weights.SetWeight(vindex, power);
                                                        beforeRoot.weightData.SetWeight(pos, weights);
                                                    }
                                                }
                                                #endregion
                                                Refresh();
                                                InternalEditorUtility.RepaintAllViews();
                                            }
                                        }
                                        #endregion
                                        #region Remove Root Bone
                                        {
                                            bool disabled = false;
                                            {
                                                int count = 0;
                                                for (int i = 0; i < animationTarget.rootBone.childCount; i++)
                                                {
                                                    var child = animationTarget.rootBone.GetChild(i);
                                                    if (child.GetComponent<VoxelSkinnedAnimationObjectBone>() != null)
                                                        count++;
                                                }
                                                disabled = count != 1;
                                            }
                                            EditorGUI.BeginDisabledGroup(disabled);
                                            if (GUILayout.Button("Remove Root Bone"))
                                            {
                                                for (int i = 0; i < animationTarget.rootBone.childCount; i++)
                                                {
                                                    var child = animationTarget.rootBone.GetChild(i);
                                                    if (child.GetComponent<VoxelSkinnedAnimationObjectBone>() != null)
                                                        Undo.RecordObject(animationTarget.rootBone, "Remove Root Bone");
                                                    Undo.SetTransformParent(child, animationTarget.transform, "Remove Root Bone");
                                                    i--;
                                                }
                                                Undo.DestroyObjectImmediate(animationTarget.rootBone.gameObject);
                                                animationCore.UpdateBoneBindposes();
                                                EditorGUIUtility.PingObject(animationTarget.rootBone.gameObject);
                                                animationCore.FixMissingAnimation();
                                                Refresh();
                                                InternalEditorUtility.RepaintAllViews();
                                            }
                                            EditorGUI.EndDisabledGroup();
                                        }
                                        #endregion
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUI.EndDisabledGroup();
                                }
                                #endregion
                                #region Reset
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("Reset");
                                    {
                                        if (GUILayout.Button("All"))
                                        {
                                            for (int i = 0; i < animationTarget.bones.Length; i++)
                                            {
                                                Undo.RecordObject(animationTarget.bones[i].transform, "Reset Bone Transform");
                                                if (animationTarget.bones[i].bonePositionSave)
                                                {
                                                    animationTarget.bones[i].transform.localPosition = animationTarget.bones[i].bonePosition;
                                                    animationTarget.bones[i].transform.localRotation = animationTarget.bones[i].boneRotation;
                                                }
                                                animationTarget.bones[i].transform.localScale = Vector3.one;
                                            }
                                        }
                                        if (GUILayout.Button("Position"))
                                        {
                                            for (int i = 0; i < animationTarget.bones.Length; i++)
                                            {
                                                Undo.RecordObject(animationTarget.bones[i].transform, "Reset Bone Position");
                                                if (animationTarget.bones[i].bonePositionSave)
                                                    animationTarget.bones[i].transform.localPosition = animationTarget.bones[i].bonePosition;
                                            }
                                        }
                                        if (GUILayout.Button("Rotation"))
                                        {
                                            for (int i = 0; i < animationTarget.bones.Length; i++)
                                            {
                                                Undo.RecordObject(animationTarget.bones[i].transform, "Reset Bone Rotation");
                                                if (animationTarget.bones[i].bonePositionSave)
                                                    animationTarget.bones[i].transform.localRotation = animationTarget.bones[i].boneRotation;
                                            }
                                        }
                                        if (GUILayout.Button("Scale"))
                                        {
                                            for (int i = 0; i < animationTarget.bones.Length; i++)
                                            {
                                                Undo.RecordObject(animationTarget.bones[i].transform, "Reset Bone Scale");
                                                animationTarget.bones[i].transform.localScale = Vector3.one;
                                            }
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                #endregion
                                #region Count
                                {
                                    EditorGUILayout.LabelField("Count", animationTarget.rootBone != null ? animationTarget.bones.Length.ToString() : "");
                                }
                                #endregion
                            }
                            EditorGUI.indentLevel--;
                        }
                        if (animationTarget.mesh != null)
                        {
                            if (animationTarget.rootBone == null)
                            {
                                EditorGUILayout.HelpBox("Bone not found. Please create bone.", MessageType.Error);
                            }
                        }
                    }
                    if (animationTarget.rootBone != null)
                    {
                        EditorGUILayout.LabelField("Rig", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel++;
                            {
                                #region Update the Animator Avatar
                                if (baseTarget.advancedMode)
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var updateAnimatorAvatar = EditorGUILayout.ToggleLeft("Update the Animator Avatar", animationTarget.updateAnimatorAvatar);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (EditorUtility.DisplayDialog("Update the Animator Avatar", "It will be changed.\nAre you sure?", "ok", "cancel"))
                                        {
                                            UndoRecordObject("Inspector");
                                            animationTarget.updateAnimatorAvatar = updateAnimatorAvatar;
                                            baseCore.SetRendererCompornent();
                                        }
                                    }
                                }
                                #endregion
                                #region AnimationType
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var rigAnimationType = (VoxelSkinnedAnimationObject.RigAnimationType)EditorGUILayout.EnumPopup("Animation Type", animationTarget.rigAnimationType);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        UndoRecordObject("Inspector");
                                        #region ChangeAnimationType
                                        Action RemoveAnimation = () =>
                                        {
                                            EditorApplication.delayCall += () =>
                                            {
                                                if (animationTarget == null || animationTarget.gameObject == null) return;
                                                var animation = animationTarget.gameObject.GetComponent<Animation>();
                                                if (animation != null)
                                                    Undo.DestroyObjectImmediate(animation);
                                            };
                                        };
                                        Action RemoveAnimator = () =>
                                        {
                                            EditorApplication.delayCall += () =>
                                            {
                                                if (animationTarget == null || animationTarget.gameObject == null) return;
                                                var animator = animationTarget.gameObject.GetComponent<Animator>();
                                                if (animator != null)
                                                    Undo.DestroyObjectImmediate(animator);
                                            };
                                        };
                                        Action CreateAnimation = () =>
                                        {
                                            var animation = animationTarget.gameObject.GetComponent<Animation>();
                                            if (animation == null)
                                            {
                                                animation = animationTarget.gameObject.AddComponent<Animation>();
                                                Undo.RegisterCreatedObjectUndo(animation, "Inspector");
                                            }
                                        };
                                        Action CreateAnimator = () =>
                                        {
                                            var animator = animationTarget.gameObject.GetComponent<Animator>();
                                            if (animator == null)
                                            {
                                                animator = animationTarget.gameObject.AddComponent<Animator>();
                                                Undo.RegisterCreatedObjectUndo(animator, "Inspector");
                                            }
                                        };
                                        switch (rigAnimationType)
                                        {
                                        case VoxelSkinnedAnimationObject.RigAnimationType.None:
                                            RemoveAnimation();
                                            RemoveAnimator();
                                            break;
                                        case VoxelSkinnedAnimationObject.RigAnimationType.Legacy:
                                            RemoveAnimator();
                                            CreateAnimation();
                                            break;
                                        case VoxelSkinnedAnimationObject.RigAnimationType.Generic:
                                        case VoxelSkinnedAnimationObject.RigAnimationType.Humanoid:
                                            RemoveAnimation();
                                            CreateAnimator();
                                            break;
                                        }
                                        #endregion
                                        VoxelHumanoidConfigreAvatar.Destroy();
                                        animationTarget.rigAnimationType = rigAnimationType;
                                        animationTarget.humanDescription.firstAutomapDone = false;
                                        Refresh();
                                    }
                                }
                                #endregion
                                #region Avatar
                                if (baseTarget.advancedMode &&
                                    (animationTarget.rigAnimationType == VoxelSkinnedAnimationObject.RigAnimationType.Generic || animationTarget.rigAnimationType == VoxelSkinnedAnimationObject.RigAnimationType.Humanoid))
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUI.BeginDisabledGroup(true);
                                        EditorGUILayout.ObjectField("Avatar", animationTarget.avatar, typeof(Avatar), false);
                                        EditorGUI.EndDisabledGroup();
                                    }
                                    if (animationTarget.avatar != null)
                                    {
                                        if (!EditorCommon.IsMainAsset(animationTarget.avatar))
                                        {
                                            if (GUILayout.Button("Save", GUILayout.Width(48), GUILayout.Height(16)))
                                            {
                                                #region Create Avatar
                                                string path = EditorUtility.SaveFilePanel("Save avatar", objectCore.GetDefaultPath(), string.Format("{0}_avatar.asset", baseTarget.gameObject.name), "asset");
                                                if (!string.IsNullOrEmpty(path))
                                                {
                                                    if (path.IndexOf(Application.dataPath) < 0)
                                                    {
                                                        EditorUtility.DisplayDialog("Error!", "Please save a lower than \"Assets\"", "ok");
                                                    }
                                                    else
                                                    {
                                                        UndoRecordObject("Save Avatar");
                                                        path = FileUtil.GetProjectRelativePath(path);
                                                        AssetDatabase.CreateAsset(Avatar.Instantiate(animationTarget.avatar), path);
                                                        animationTarget.avatar = AssetDatabase.LoadAssetAtPath<Avatar>(path);
                                                        Refresh();
                                                    }
                                                }
                                                #endregion
                                            }
                                        }
                                        {
                                            if (GUILayout.Button("Reset", GUILayout.Width(48), GUILayout.Height(16)))
                                            {
                                                #region Reset Avatar
                                                UndoRecordObject("Reset Avatar");
                                                animationTarget.avatar = null;
                                                Refresh();
                                                #endregion
                                            }
                                        }
                                    }
                                    EditorGUILayout.EndHorizontal();
                                }
                                if ((animationTarget.rigAnimationType == VoxelSkinnedAnimationObject.RigAnimationType.Generic || animationTarget.rigAnimationType == VoxelSkinnedAnimationObject.RigAnimationType.Humanoid))
                                {
                                    EditorGUI.indentLevel++;
                                    if (animationTarget.avatar != null && !animationTarget.avatar.isValid)
                                    {
                                        EditorGUILayout.HelpBox("Invalid mecanim avatar.\nCheck the bone please.", MessageType.Error);
                                    }
                                    #region AvatarSetWarning
                                    if (animationTarget.updateAnimatorAvatar)
                                    {
                                        var animator = animationTarget.GetComponent<Animator>();
                                        if (animator != null && animator.avatar != animationTarget.avatar)
                                        {
                                            EditorGUILayout.HelpBox("Animator's Avatar is not set.\nIt needs to be updated.\nPlease press 'Refresh'.", MessageType.Warning);
                                        }
                                    }
                                    #endregion
                                    EditorGUI.indentLevel--;
                                }
                                #endregion
                                #region Configre Avatar
                                if (animationTarget.rigAnimationType == VoxelSkinnedAnimationObject.RigAnimationType.Humanoid)
                                {
                                    EditorGUI.BeginDisabledGroup(isPrefab);
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.Space();
                                    EditorGUI.BeginChangeCheck();
                                    GUILayout.Toggle(VoxelHumanoidConfigreAvatar.instance != null, "Configure Avatar", GUI.skin.button);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (VoxelHumanoidConfigreAvatar.instance == null)
                                            VoxelHumanoidConfigreAvatar.Create(animationTarget);
                                        else
                                            VoxelHumanoidConfigreAvatar.instance.Close();
                                    }
                                    EditorGUILayout.Space();
                                    EditorGUILayout.EndHorizontal();
                                    EditorGUI.EndDisabledGroup();
                                    EditorGUILayout.Space();
                                }
                                #endregion
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    if (baseTarget.advancedMode && animationTarget.rootBone != null)
                    {
                        EditorGUILayout.LabelField("Mesh", EditorStyles.boldLabel);
                        {
                            EditorGUI.indentLevel++;
                            {
                                #region skinnedMeshBoundsUpdate
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var skinnedMeshBoundsUpdate = EditorGUILayout.ToggleLeft("Update the Skinned Mesh Renderer Bounds", animationTarget.skinnedMeshBoundsUpdate);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (EditorUtility.DisplayDialog("Update the Skinned Mesh Renderer Bounds", "It will be changed.\nAre you sure?", "ok", "cancel"))
                                        {
                                            UndoRecordObject("Inspector");
                                            animationTarget.skinnedMeshBoundsUpdate = skinnedMeshBoundsUpdate;
                                            animationCore.UpdateSkinnedMeshBounds();
                                        }
                                    }
                                }
                                #endregion
                                #region skinnedMeshBoundsUpdateScale
                                if (animationTarget.skinnedMeshBoundsUpdate)
                                {
                                    EditorGUI.indentLevel++;
                                    EditorGUI.BeginChangeCheck();
                                    var skinnedMeshBoundsUpdateScale = EditorGUILayout.Vector3Field("Scale", animationTarget.skinnedMeshBoundsUpdateScale);
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        UndoRecordObject("Inspector");
                                        animationTarget.skinnedMeshBoundsUpdateScale = skinnedMeshBoundsUpdateScale;
                                        animationCore.UpdateSkinnedMeshBounds();
                                    }
                                    EditorGUI.indentLevel--;
                                }
                                #endregion
                            }
                            EditorGUI.indentLevel--;
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }
            #endregion

            base.InspectorGUI_Refresh();

#if UNITY_2018_3_OR_NEWER
            {
                if (!baseCore.isPrefabEditable)
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
#endif
        }
        protected override void InspectorGUI_ImportOpenBefore()
        {
            base.InspectorGUI_ImportOpenBefore();

            VoxelHumanoidConfigreAvatar.Destroy();
        }
        protected override void InspectorGUI_ImportOffsetSetExtra(GenericMenu menu)
        {
            #region Feet
            menu.AddItem(new GUIContent("Feet"), false, () =>
            {
                UndoRecordObject("Inspector", true);
                baseTarget.importOffset = -animationCore.GetVoxelsFeet();
                Refresh();
            });
            #endregion
        }
        protected override void InspectorGUI_Refresh() { }

        protected override void SaveAllUnsavedAssets()
        {
            SaveAllUnsavedAssets(new MenuCommand(baseTarget));
        }

        [MenuItem("CONTEXT/VoxelSkinnedAnimationObject/Save All Unsaved Assets")]
        private static void SaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelSkinnedAnimationObjectCore(objectTarget);

            var folder = EditorUtility.OpenFolderPanel("Save all", objectCore.GetDefaultPath(), null);
            if (string.IsNullOrEmpty(folder)) return;
            if (folder.IndexOf(Application.dataPath) < 0)
            {
                EditorCommon.SaveInsideAssetsFolderDisplayDialog();
                return;
            }

            Undo.RecordObject(objectTarget, "Save All Unsaved Assets");

            #region Mesh
            if (objectTarget.mesh != null && !EditorCommon.IsMainAsset(objectTarget.mesh))
            {
                var path = folder + "/" + string.Format("{0}_mesh.asset", objectTarget.gameObject.name);
                path = FileUtil.GetProjectRelativePath(path);
                path = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CreateAsset(Mesh.Instantiate(objectTarget.mesh), path);
                objectTarget.mesh = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            }
            #endregion

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

            #region Avatar
            if (objectTarget.avatar != null && !EditorCommon.IsMainAsset(objectTarget.avatar))
            {
                var path = folder + "/" + string.Format("{0}_avatar.asset", objectTarget.gameObject.name);
                path = FileUtil.GetProjectRelativePath(path);
                path = AssetDatabase.GenerateUniqueAssetPath(path);
                AssetDatabase.CreateAsset(Avatar.Instantiate(objectTarget.avatar), path);
                objectTarget.avatar = AssetDatabase.LoadAssetAtPath<Avatar>(path);
            }
            #endregion

            objectCore.ReCreate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelSkinnedAnimationObject/Save All Unsaved Assets", true)]
        private static bool IsValidateSaveAllUnsavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }

        [MenuItem("CONTEXT/VoxelSkinnedAnimationObject/Reset All Assets")]
        private static void ResetAllSavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelSkinnedAnimationObjectCore(objectTarget);

            Undo.RecordObject(objectTarget, "Reset All Assets");

            objectCore.ResetAllAssets();
            objectCore.ReCreate();
            InternalEditorUtility.RepaintAllViews();
        }
        [MenuItem("CONTEXT/VoxelSkinnedAnimationObject/Reset All Assets", true)]
        private static bool IsValidateResetAllSavedAssets(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }

        [MenuItem("CONTEXT/VoxelSkinnedAnimationObject/Export COLLADA(dae) File", false, 10000)]
        private static void ExportDaeFile(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObject;
            if (objectTarget == null) return;

            var objectCore = new VoxelSkinnedAnimationObjectCore(objectTarget);

            DaeExporterWindow.Open(objectTarget.rigAnimationType == VoxelSkinnedAnimationObject.RigAnimationType.Humanoid, () =>
            {
                string path = EditorUtility.SaveFilePanel("Export COLLADA(dae) File", objectCore.GetDefaultPath(), string.Format("{0}.dae", Path.GetFileNameWithoutExtension(objectTarget.voxelFilePath)), "dae");
                if (string.IsNullOrEmpty(path)) return;

                if (!objectCore.ExportDaeFileWithAnimation(path, DaeExporterWindow.exportMesh, DaeExporterWindow.exportAnimation, DaeExporterWindow.enableFootIK))
                {
                    Debug.LogErrorFormat("<color=green>[Voxel Importer]</color> Export COLLADA(dae) File error. file:{0}", path);
                }
            });
        }
        [MenuItem("CONTEXT/VoxelSkinnedAnimationObject/Export COLLADA(dae) File", true)]
        private static bool IsValidateExportDaeFile(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObject;
            if (objectTarget == null) return false;

#if UNITY_2018_3_OR_NEWER
            return true;
#else
            return PrefabUtility.GetPrefabType(objectTarget) != PrefabType.Prefab;
#endif
        }

        [MenuItem("CONTEXT/VoxelSkinnedAnimationObject/Remove All Voxel Importer Compornent", false, 10100)]
        private static void RemoveAllVoxelImporterCompornent(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObject;
            if (objectTarget == null) return;

            if (objectTarget.bones != null)
            {
                for (int i = 0; i < objectTarget.bones.Length; i++)
                {
                    Undo.DestroyObjectImmediate(objectTarget.bones[i]);
                }
            }
            Undo.DestroyObjectImmediate(objectTarget);
        }
        [MenuItem("CONTEXT/VoxelSkinnedAnimationObject/Remove All Voxel Importer Compornent", true)]
        private static bool IsValidateRemoveAllVoxelImporterCompornent(MenuCommand menuCommand)
        {
            var objectTarget = menuCommand.context as VoxelSkinnedAnimationObject;
            if (objectTarget == null) return false;

            return EditorCommon.IsComponentEditable(objectTarget);
        }
    }
}
