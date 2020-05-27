using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    public class VoxelHumanoidConfigreAvatar : EditorWindow
    {
        public static VoxelHumanoidConfigreAvatar instance;

        public VoxelSkinnedAnimationObject animationObject { get; private set; }
        public VoxelSkinnedAnimationObjectCore animationCore { get; private set; }

        #region Textures
        private Texture2D avatarIcon;
        private Texture2D avatarHead;
        private Texture2D avatarTorso;
        private Texture2D avatarLeftArm;
        private Texture2D avatarLeftFingers;
        private Texture2D avatarLeftLeg;
        private Texture2D avatarRightArm;
        private Texture2D avatarRightFingers;
        private Texture2D avatarRightLeg;
        private Texture2D avatarHeadZoom;
        private Texture2D avatarLeftHandZoom;
        private Texture2D avatarRightHandZoom;
        private Texture2D avatarBodysilhouette;
        private Texture2D avatarHeadzoomsilhouette;
        private Texture2D avatarLefthandzoomsilhouette;
        private Texture2D avatarRighthandzoomsilhouette;
        private Texture2D dotfill;
        private Texture2D dotframe;
        private Texture2D dotframedotted;
        private Texture2D dotselection;
        #endregion

        #region GUIStyles
        private GUIStyle guiStyleBackgroundBox;
        private GUIStyle guiStyleVerticalToolbar;
        private GUIStyle guiStyleBoneButton;
        private GUIStyle guiStyleOLTitle;
        private GUIStyle guiStyleGVToolbar;
        private GUIStyle guiStyleGVGizmoDropDown;
        #endregion

        #region Strings
        private readonly string[] HumanoidAvatarConfigreModeStrings =
        {
            "Mapping",
            "Muscles & Settings",
        };
        private readonly string[] HumanoidAvatarMappingModeStrings =
        {
            "Body",
            "Head",
            "Left Hand",
            "Right Hand",
        };

        #endregion

        #region Editor
        public enum HumanoidAvatarConfigreMode
        {
            Mapping,
            MusclesAndSettings,
        }
        public HumanoidAvatarConfigreMode humanoidAvatarConfigreMode { get; private set; }

        public enum HumanoidAvatarMappingMode
        {
            Body,
            Head,
            LeftHand,
            RightHand,
        }
        public HumanoidAvatarMappingMode humanoidAvatarMappingMode { get; private set; }

        private bool mappingBodyBodyFoldout = true;
        private bool mappingBodyLeftArmFoldout = true;
        private bool mappingBodyRightArmFoldout = true;
        private bool mappingBodyLeftLegFoldout = true;
        private bool mappingBodyRightLegFoldout = true;
        private bool mappingHeadFoldout = true;
        private bool mappingLeftFingersFoldout = true;
        private bool mappingRightFingersFoldout = true;

        private VoxelSkinnedAnimationObject.HumanoidBone selectBone = (VoxelSkinnedAnimationObject.HumanoidBone)(-1);

        private VoxelSkinnedAnimationObject.VoxelImporterHumanDescription saveHumanDescription;

        private int undoGroupID = -1;

        private Vector2 scrollPosition;
        #endregion

        public static void Create(VoxelSkinnedAnimationObject animationObject)
        {
            if (instance == null)
            {
                instance = CreateInstance<VoxelHumanoidConfigreAvatar>();
            }

            instance.Initialize(animationObject);

            instance.minSize = new Vector2(256, instance.minSize.y);

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
            undoGroupID = Undo.GetCurrentGroup();

            Undo.undoRedoPerformed -= EditorUndoRedoPerformed;
            Undo.undoRedoPerformed += EditorUndoRedoPerformed;

            InternalEditorUtility.RepaintAllViews();
        }
        void OnDisable()
        {
            if (animationObject != null)
            {
                if (animationObject.humanDescription.IsChanged(ref saveHumanDescription))
                {
                    if (EditorUtility.DisplayDialog("Unapplied settings", "Unapplied 'HumanDescription' settings for Avatar.", "Apply", "Revert"))
                    {
                        saveHumanDescription = new VoxelSkinnedAnimationObject.VoxelImporterHumanDescription(ref animationObject.humanDescription);
                        animationCore.UpdateBoneWeight();
                    }
                    else
                    {
                        animationObject.humanDescription = new VoxelSkinnedAnimationObject.VoxelImporterHumanDescription(ref saveHumanDescription);
                    }
                }
            }

            if (undoGroupID >= 0)
            {
                Undo.CollapseUndoOperations(undoGroupID);
                undoGroupID = -1;
            }

            Undo.undoRedoPerformed -= EditorUndoRedoPerformed;

            instance = null;

            InternalEditorUtility.RepaintAllViews();
        }
        void OnDestroy()
        {
            OnDisable();
        }
        private void EditorUndoRedoPerformed()
        {
            Repaint();
        }

        void OnSelectionChange()
        {
            var go = Selection.activeGameObject;
            if (go == null || go.GetComponentInParent<VoxelSkinnedAnimationObject>() != animationObject)
            {
                Close();
            }
        }

        private void Initialize(VoxelSkinnedAnimationObject animationObject)
        {
            this.animationObject = animationObject;
            animationCore = new VoxelSkinnedAnimationObjectCore(this.animationObject);
            animationCore.ResetBoneTransform();

            saveHumanDescription = new VoxelSkinnedAnimationObject.VoxelImporterHumanDescription(ref animationObject.humanDescription);

            titleContent = new GUIContent("Configure Avatar");

            avatarIcon = EditorGUIUtility.IconContent("avatar icon").image as Texture2D;
            avatarHead = EditorGUIUtility.IconContent("AvatarInspector/head").image as Texture2D; 
            avatarTorso = EditorGUIUtility.IconContent("AvatarInspector/torso").image as Texture2D;
            avatarLeftArm = EditorGUIUtility.IconContent("AvatarInspector/leftarm").image as Texture2D;
            avatarLeftFingers = EditorGUIUtility.IconContent("AvatarInspector/leftfingers").image as Texture2D;
            avatarLeftLeg = EditorGUIUtility.IconContent("AvatarInspector/leftleg").image as Texture2D;
            avatarRightArm = EditorGUIUtility.IconContent("AvatarInspector/rightarm").image as Texture2D;
            avatarRightFingers = EditorGUIUtility.IconContent("AvatarInspector/rightfingers").image as Texture2D;
            avatarRightLeg = EditorGUIUtility.IconContent("AvatarInspector/rightleg").image as Texture2D;
            avatarHeadZoom = EditorGUIUtility.IconContent("AvatarInspector/headzoom").image as Texture2D;
            avatarLeftHandZoom = EditorGUIUtility.IconContent("AvatarInspector/lefthandzoom").image as Texture2D;
            avatarRightHandZoom = EditorGUIUtility.IconContent("AvatarInspector/righthandzoom").image as Texture2D;
            avatarBodysilhouette = EditorGUIUtility.IconContent("AvatarInspector/bodysilhouette").image as Texture2D;
            avatarHeadzoomsilhouette = EditorGUIUtility.IconContent("AvatarInspector/headzoomsilhouette").image as Texture2D;
            avatarLefthandzoomsilhouette = EditorGUIUtility.IconContent("AvatarInspector/lefthandzoomsilhouette").image as Texture2D;
            avatarRighthandzoomsilhouette = EditorGUIUtility.IconContent("AvatarInspector/righthandzoomsilhouette").image as Texture2D;
            dotfill = EditorGUIUtility.IconContent("AvatarInspector/dotfill").image as Texture2D;
            dotframe = EditorGUIUtility.IconContent("AvatarInspector/dotframe").image as Texture2D;
            dotframedotted = EditorGUIUtility.IconContent("AvatarInspector/dotframedotted").image as Texture2D;
            dotselection = EditorGUIUtility.IconContent("AvatarInspector/dotselection").image as Texture2D;
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
            if (guiStyleBackgroundBox == null)
            {
                guiStyleBackgroundBox = new GUIStyle("CurveEditorBackground");
            }
            if (guiStyleVerticalToolbar == null)
                guiStyleVerticalToolbar = new GUIStyle(GUI.skin.button);
            guiStyleVerticalToolbar.margin = new RectOffset(0, 0, 0, 0);
            guiStyleVerticalToolbar.fontSize = 9;
            if (guiStyleBoneButton == null)
                guiStyleBoneButton = new GUIStyle(GUI.skin.button);
            guiStyleBoneButton.margin = new RectOffset(0, 0, 0, 0);
            guiStyleBoneButton.padding = new RectOffset(0, 0, 0, 0);
            guiStyleBoneButton.border = new RectOffset(0, 0, 0, 0);
            guiStyleBoneButton.active = guiStyleBoneButton.normal;
            if (guiStyleOLTitle == null)
                guiStyleOLTitle = new GUIStyle("OL Title");
            guiStyleOLTitle.fixedHeight = 18;
            if (guiStyleGVToolbar == null)
                guiStyleGVToolbar = new GUIStyle("TE Toolbar");
            if (guiStyleGVGizmoDropDown == null)
                guiStyleGVGizmoDropDown = new GUIStyle("TE ToolbarDropDown");
            #endregion

            #region Avatar Icon
            {
                EditorGUILayout.BeginHorizontal();
                {
                    var rect = EditorGUILayout.GetControlRect();
                    rect.width = avatarIcon.width * 0.5f;
                    rect.height = avatarIcon.height * 0.5f;
                    GUI.DrawTexture(rect, avatarIcon);
                    rect.position = new Vector2(rect.position.x + rect.width, rect.position.y);
                    rect.width = EditorGUILayout.GetControlRect().width + rect.width;
                    EditorGUI.LabelField(rect, animationObject.avatar != null ? animationObject.avatar.name : "");
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }
            #endregion

            #region HumanoidAvatarConfigreMode
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                {
                    EditorGUI.BeginChangeCheck();
                    humanoidAvatarConfigreMode = (HumanoidAvatarConfigreMode)GUILayout.Toolbar((int)humanoidAvatarConfigreMode, HumanoidAvatarConfigreModeStrings, "LargeButton");
                    if (EditorGUI.EndChangeCheck())
                    {
                        InternalEditorUtility.RepaintAllViews();
                    }
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            #endregion

            if (humanoidAvatarConfigreMode == HumanoidAvatarConfigreMode.Mapping)
            {
                #region Avatar Preview
                {
                    Rect backgroundRect;
                    {
                        backgroundRect = EditorGUILayout.GetControlRect();
                        backgroundRect.height = 379f;
                        GUI.Box(backgroundRect, "", guiStyleBackgroundBox);
                    }
                    GUILayout.Space(backgroundRect.height - 16);

                    var saveGUIColor = GUI.color;

                    Action<bool> SetGUIColor = (enable) =>
                    {
                        if (enable) GUI.color = new Color(0.2f, 0.8f, 0.2f);
                        else GUI.color = new Color(0.8f, 0.2f, 0.2f);
                    };
                    Action<bool> SetGUIColorLeftFingers = (enable) =>
                    {
                        if (IsLeftFingersHave())
                        {
                            if (enable) GUI.color = new Color(0.2f, 0.8f, 0.2f);
                            else GUI.color = new Color(0.8f, 0.2f, 0.2f);
                        }
                        else
                        {
                            GUI.color = Color.gray;
                        }
                    };
                    Action<bool> SetGUIColorRightFingers = (enable) =>
                    {
                        if (IsRightFingersHave())
                        {
                            if (enable) GUI.color = new Color(0.2f, 0.8f, 0.2f);
                            else GUI.color = new Color(0.8f, 0.2f, 0.2f);
                        }
                        else
                        {
                            GUI.color = Color.gray;
                        }
                    };

                    if (humanoidAvatarMappingMode == HumanoidAvatarMappingMode.Body)
                    {
                        #region Body
                        #region BackGround
                        //base
                        GUI.color = new Color(0.2f, 0.2f, 0.2f);
                        GUI.DrawTexture(backgroundRect, avatarBodysilhouette, ScaleMode.ScaleToFit);
                        {//Head
                            SetGUIColor(IsHeadEnable());
                            GUI.DrawTexture(backgroundRect, avatarHead, ScaleMode.ScaleToFit);
                        }
                        {//Torso
                            SetGUIColor(IsTorsoEnable());
                            GUI.DrawTexture(backgroundRect, avatarTorso, ScaleMode.ScaleToFit);
                        }
                        {//LeftArm
                            SetGUIColor(IsLeftArmEnable());
                            GUI.DrawTexture(backgroundRect, avatarLeftArm, ScaleMode.ScaleToFit);
                        }
                        {//LeftFingers
                            SetGUIColorLeftFingers(IsLeftFingersEnable());
                            GUI.DrawTexture(backgroundRect, avatarLeftFingers, ScaleMode.ScaleToFit);
                        }
                        {//LeftLeg
                            SetGUIColor(IsLeftLegEnable());
                            GUI.DrawTexture(backgroundRect, avatarLeftLeg, ScaleMode.ScaleToFit);
                        }
                        {//RightArm
                            SetGUIColor(IsRightArmEnable());
                            GUI.DrawTexture(backgroundRect, avatarRightArm, ScaleMode.ScaleToFit);
                        }
                        {//RightFingers
                            SetGUIColorRightFingers(IsRightFingersEnable());
                            GUI.DrawTexture(backgroundRect, avatarRightFingers, ScaleMode.ScaleToFit);
                        }
                        {//RightLeg
                            SetGUIColor(IsRightLegEnable());
                            GUI.DrawTexture(backgroundRect, avatarRightLeg, ScaleMode.ScaleToFit);
                        }
                        #endregion
                        #region Bone
                        {
                            var position = backgroundRect.center;
                            GUI_PreviewBone(new Vector2(position.x, 227), true, VoxelSkinnedAnimationObject.HumanoidBone.Hips);
                            GUI_PreviewBone(new Vector2(position.x, 205), true, VoxelSkinnedAnimationObject.HumanoidBone.Spine);
                            GUI_PreviewBone(new Vector2(position.x, 175), false, VoxelSkinnedAnimationObject.HumanoidBone.Chest);
                            GUI_PreviewBone(new Vector2(position.x, 148), false, VoxelSkinnedAnimationObject.HumanoidBone.UpperChest);
                            GUI_PreviewBone(new Vector2(position.x, 118), false, VoxelSkinnedAnimationObject.HumanoidBone.Neck);
                            GUI_PreviewBone(new Vector2(position.x, 99), true, VoxelSkinnedAnimationObject.HumanoidBone.Head);
                            GUI_PreviewBone(new Vector2(position.x + 12, 129), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftShoulder);
                            GUI_PreviewBone(new Vector2(position.x - 12, 129), false, VoxelSkinnedAnimationObject.HumanoidBone.RightShoulder);
                            GUI_PreviewBone(new Vector2(position.x + 27, 135), true, VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperArm);
                            GUI_PreviewBone(new Vector2(position.x - 27, 135), true, VoxelSkinnedAnimationObject.HumanoidBone.RightUpperArm);
                            GUI_PreviewBone(new Vector2(position.x + 43, 186), true, VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerArm);
                            GUI_PreviewBone(new Vector2(position.x - 43, 186), true, VoxelSkinnedAnimationObject.HumanoidBone.RightLowerArm);
                            GUI_PreviewBone(new Vector2(position.x + 59, 237), true, VoxelSkinnedAnimationObject.HumanoidBone.LeftHand);
                            GUI_PreviewBone(new Vector2(position.x - 59, 237), true, VoxelSkinnedAnimationObject.HumanoidBone.RightHand);
                            GUI_PreviewBone(new Vector2(position.x + 14, 241), true, VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperLeg);
                            GUI_PreviewBone(new Vector2(position.x - 14, 241), true, VoxelSkinnedAnimationObject.HumanoidBone.RightUpperLeg);
                            GUI_PreviewBone(new Vector2(position.x + 18, 318), true, VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerLeg);
                            GUI_PreviewBone(new Vector2(position.x - 18, 318), true, VoxelSkinnedAnimationObject.HumanoidBone.RightLowerLeg);
                            GUI_PreviewBone(new Vector2(position.x + 20, 394), true, VoxelSkinnedAnimationObject.HumanoidBone.LeftFoot);
                            GUI_PreviewBone(new Vector2(position.x - 20, 394), true, VoxelSkinnedAnimationObject.HumanoidBone.RightFoot);
                            GUI_PreviewBone(new Vector2(position.x + 23, 411), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftToes);
                            GUI_PreviewBone(new Vector2(position.x - 23, 411), false, VoxelSkinnedAnimationObject.HumanoidBone.RightToes);
                        }
                        #endregion
                        #endregion
                    }
                    else if (humanoidAvatarMappingMode == HumanoidAvatarMappingMode.Head)
                    {
                        #region Head
                        #region BackGround
                        GUI.color = new Color(0.2f, 0.2f, 0.2f);
                        GUI.DrawTexture(backgroundRect, avatarHeadzoomsilhouette, ScaleMode.ScaleToFit);
                        //base
                        {
                            SetGUIColor(IsHeadEnable());
                            GUI.DrawTexture(backgroundRect, avatarHeadZoom, ScaleMode.ScaleToFit);
                        }
                        #endregion
                        #region Bone
                        {
                            var position = backgroundRect.center;
                            GUI_PreviewBone(new Vector2(position.x - 14, 299), true, VoxelSkinnedAnimationObject.HumanoidBone.Head);
                            GUI_PreviewBone(new Vector2(position.x - 18, 360), false, VoxelSkinnedAnimationObject.HumanoidBone.Neck);
                            GUI_PreviewBone(new Vector2(position.x + 56, 212), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftEye);
                            GUI_PreviewBone(new Vector2(position.x + 13, 212), false, VoxelSkinnedAnimationObject.HumanoidBone.RightEye);
                            GUI_PreviewBone(new Vector2(position.x + 40, 318), false, VoxelSkinnedAnimationObject.HumanoidBone.Jaw);
                        }
                        #endregion
                        #endregion
                    }
                    else if (humanoidAvatarMappingMode == HumanoidAvatarMappingMode.LeftHand)
                    {
                        #region LeftHand
                        #region BackGround
                        GUI.color = new Color(0.2f, 0.2f, 0.2f);
                        GUI.DrawTexture(backgroundRect, avatarLefthandzoomsilhouette, ScaleMode.ScaleToFit);
                        //base
                        SetGUIColorLeftFingers(IsLeftFingersEnable());
                        GUI.DrawTexture(backgroundRect, avatarLeftHandZoom, ScaleMode.ScaleToFit);
                        #endregion
                        #region Bone
                        {
                            var position = backgroundRect.center;
                            GUI_PreviewBone(new Vector2(position.x - 42, 222), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbProximal);
                            GUI_PreviewBone(new Vector2(position.x - 20, 198), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbIntermediate);
                            GUI_PreviewBone(new Vector2(position.x - 4, 180), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbDistal);
                            GUI_PreviewBone(new Vector2(position.x + 22, 222), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexProximal);
                            GUI_PreviewBone(new Vector2(position.x + 54, 215), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexIntermediate);
                            GUI_PreviewBone(new Vector2(position.x + 78, 211), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexDistal);
                            GUI_PreviewBone(new Vector2(position.x + 26, 243), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleProximal);
                            GUI_PreviewBone(new Vector2(position.x + 62, 243), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleIntermediate);
                            GUI_PreviewBone(new Vector2(position.x + 88, 243), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleDistal);
                            GUI_PreviewBone(new Vector2(position.x + 19, 265), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftRingProximal);
                            GUI_PreviewBone(new Vector2(position.x + 54, 266), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftRingIntermediate);
                            GUI_PreviewBone(new Vector2(position.x + 79, 268), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftRingDistal);
                            GUI_PreviewBone(new Vector2(position.x + 10, 286), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleProximal);
                            GUI_PreviewBone(new Vector2(position.x + 35, 287), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleIntermediate);
                            GUI_PreviewBone(new Vector2(position.x + 54, 289), false, VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleDistal);
                        }
                        #endregion
                        #endregion
                    }
                    else if (humanoidAvatarMappingMode == HumanoidAvatarMappingMode.RightHand)
                    {
                        #region RightHand
                        #region BackGround
                        GUI.color = new Color(0.2f, 0.2f, 0.2f);
                        GUI.DrawTexture(backgroundRect, avatarRighthandzoomsilhouette, ScaleMode.ScaleToFit);
                        //base
                        SetGUIColorRightFingers(IsRightFingersEnable());
                        GUI.DrawTexture(backgroundRect, avatarRightHandZoom, ScaleMode.ScaleToFit);
                        #endregion
                        #region Bone
                        {
                            var position = backgroundRect.center;
                            GUI_PreviewBone(new Vector2(position.x + 42, 222), false, VoxelSkinnedAnimationObject.HumanoidBone.RightThumbProximal);
                            GUI_PreviewBone(new Vector2(position.x + 20, 198), false, VoxelSkinnedAnimationObject.HumanoidBone.RightThumbIntermediate);
                            GUI_PreviewBone(new Vector2(position.x + 4, 180), false, VoxelSkinnedAnimationObject.HumanoidBone.RightThumbDistal);
                            GUI_PreviewBone(new Vector2(position.x - 22, 222), false, VoxelSkinnedAnimationObject.HumanoidBone.RightIndexProximal);
                            GUI_PreviewBone(new Vector2(position.x - 54, 215), false, VoxelSkinnedAnimationObject.HumanoidBone.RightIndexIntermediate);
                            GUI_PreviewBone(new Vector2(position.x - 78, 211), false, VoxelSkinnedAnimationObject.HumanoidBone.RightIndexDistal);
                            GUI_PreviewBone(new Vector2(position.x - 26, 243), false, VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleProximal);
                            GUI_PreviewBone(new Vector2(position.x - 62, 243), false, VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleIntermediate);
                            GUI_PreviewBone(new Vector2(position.x - 88, 243), false, VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleDistal);
                            GUI_PreviewBone(new Vector2(position.x - 19, 265), false, VoxelSkinnedAnimationObject.HumanoidBone.RightRingProximal);
                            GUI_PreviewBone(new Vector2(position.x - 54, 266), false, VoxelSkinnedAnimationObject.HumanoidBone.RightRingIntermediate);
                            GUI_PreviewBone(new Vector2(position.x - 79, 268), false, VoxelSkinnedAnimationObject.HumanoidBone.RightRingDistal);
                            GUI_PreviewBone(new Vector2(position.x - 10, 286), false, VoxelSkinnedAnimationObject.HumanoidBone.RightLittleProximal);
                            GUI_PreviewBone(new Vector2(position.x - 35, 287), false, VoxelSkinnedAnimationObject.HumanoidBone.RightLittleIntermediate);
                            GUI_PreviewBone(new Vector2(position.x - 54, 289), false, VoxelSkinnedAnimationObject.HumanoidBone.RightLittleDistal);
                        }
                        #endregion
                        #endregion
                    }
                    GUI.color = saveGUIColor;

                    #region Toolbar
                    {
                        Rect rect = backgroundRect;
                        {
                            rect.position = new Vector2(backgroundRect.position.x + 5, backgroundRect.position.y + 308);
                            rect.width = 70;
                            rect.height = 64;
                        }
                        humanoidAvatarMappingMode = (HumanoidAvatarMappingMode)GUI.SelectionGrid(rect, (int)humanoidAvatarMappingMode, HumanoidAvatarMappingModeStrings, 1, guiStyleVerticalToolbar);
                    }
                    #endregion
                }
                #endregion

                EditorGUILayout.BeginVertical(GUI.skin.box);

                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                #region Optional Bone
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        var rect = EditorGUILayout.GetControlRect();
                        rect.position = new Vector2(rect.position.x + 6, rect.position.y);
                        rect.width = dotframedotted.width;
                        rect.height = dotframedotted.height;

                        var saveGUIColor = GUI.color;
                        GUI.color = Color.gray;
                        GUI.DrawTexture(rect, dotframedotted, ScaleMode.ScaleToFit);
                        GUI.color = saveGUIColor;

                        rect.position = new Vector2(rect.position.x + rect.width + 8, rect.position.y);
                        rect.width = EditorGUILayout.GetControlRect().width + rect.width;
                        EditorGUI.LabelField(rect, "Optional Bone");
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(EditorGUILayout.GetControlRect().height - 12);
                }
                #endregion

                #region Inspector
                if (humanoidAvatarMappingMode == HumanoidAvatarMappingMode.Body)
                {
                    #region Body
                    mappingBodyBodyFoldout = EditorGUILayout.Foldout(mappingBodyBodyFoldout, "Body");
                    if (mappingBodyBodyFoldout)
                    {
                        GUI_InspectorBone("Hips", true, VoxelSkinnedAnimationObject.HumanoidBone.Hips);
                        GUI_InspectorBone("Spine", true, VoxelSkinnedAnimationObject.HumanoidBone.Spine);
                        GUI_InspectorBone("Chest", false, VoxelSkinnedAnimationObject.HumanoidBone.Chest);
                        GUI_InspectorBone("Upper Chest", false, VoxelSkinnedAnimationObject.HumanoidBone.UpperChest);
                    }
                    #endregion
                    #region Left Arm
                    mappingBodyLeftArmFoldout = EditorGUILayout.Foldout(mappingBodyLeftArmFoldout, "Left Arm");
                    if (mappingBodyLeftArmFoldout)
                    {
                        GUI_InspectorBone("Shoulder", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftShoulder);
                        GUI_InspectorBone("Upper Arm", true, VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperArm);
                        GUI_InspectorBone("Lower Arm", true, VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerArm);
                        GUI_InspectorBone("Hand", true, VoxelSkinnedAnimationObject.HumanoidBone.LeftHand);
                    }
                    #endregion
                    #region Right Arm
                    mappingBodyRightArmFoldout = EditorGUILayout.Foldout(mappingBodyRightArmFoldout, "Right Arm");
                    if (mappingBodyRightArmFoldout)
                    {
                        GUI_InspectorBone("Shoulder", false, VoxelSkinnedAnimationObject.HumanoidBone.RightShoulder);
                        GUI_InspectorBone("Upper Arm", true, VoxelSkinnedAnimationObject.HumanoidBone.RightUpperArm);
                        GUI_InspectorBone("Lower Arm", true, VoxelSkinnedAnimationObject.HumanoidBone.RightLowerArm);
                        GUI_InspectorBone("Hand", true, VoxelSkinnedAnimationObject.HumanoidBone.RightHand);
                    }
                    #endregion
                    #region Left Leg
                    mappingBodyLeftLegFoldout = EditorGUILayout.Foldout(mappingBodyLeftLegFoldout, "Left Leg");
                    if (mappingBodyLeftLegFoldout)
                    {
                        GUI_InspectorBone("Upper Leg", true, VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperLeg);
                        GUI_InspectorBone("Lower Leg", true, VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerLeg);
                        GUI_InspectorBone("Foot", true, VoxelSkinnedAnimationObject.HumanoidBone.LeftFoot);
                        GUI_InspectorBone("Toes", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftToes);
                    }
                    #endregion
                    #region Right Leg
                    mappingBodyRightLegFoldout = EditorGUILayout.Foldout(mappingBodyRightLegFoldout, "Right Leg");
                    if (mappingBodyRightLegFoldout)
                    {
                        GUI_InspectorBone("Upper Leg", true, VoxelSkinnedAnimationObject.HumanoidBone.RightUpperLeg);
                        GUI_InspectorBone("Lower Leg", true, VoxelSkinnedAnimationObject.HumanoidBone.RightLowerLeg);
                        GUI_InspectorBone("Foot", true, VoxelSkinnedAnimationObject.HumanoidBone.RightFoot);
                        GUI_InspectorBone("Toes", false, VoxelSkinnedAnimationObject.HumanoidBone.RightToes);
                    }
                    #endregion
                }
                else if (humanoidAvatarMappingMode == HumanoidAvatarMappingMode.Head)
                {
                    #region Head
                    mappingHeadFoldout = EditorGUILayout.Foldout(mappingHeadFoldout, "Head");
                    if (mappingHeadFoldout)
                    {
                        GUI_InspectorBone("Neck", false, VoxelSkinnedAnimationObject.HumanoidBone.Neck);
                        GUI_InspectorBone("Head", true, VoxelSkinnedAnimationObject.HumanoidBone.Head);
                        GUI_InspectorBone("Left Eye", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftEye);
                        GUI_InspectorBone("Right Eye", false, VoxelSkinnedAnimationObject.HumanoidBone.RightEye);
                        GUI_InspectorBone("Jaw", false, VoxelSkinnedAnimationObject.HumanoidBone.Jaw);
                    }
                    #endregion
                }
                else if (humanoidAvatarMappingMode == HumanoidAvatarMappingMode.LeftHand)
                {
                    #region Left Fingers
                    mappingLeftFingersFoldout = EditorGUILayout.Foldout(mappingLeftFingersFoldout, "Left Fingers");
                    if (mappingLeftFingersFoldout)
                    {
                        GUI_InspectorBone("Thumb Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbProximal);
                        GUI_InspectorBone("Thumb Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbIntermediate);
                        GUI_InspectorBone("Thumb Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbDistal);
                        GUI_InspectorBone("Index Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexProximal);
                        GUI_InspectorBone("Index Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexIntermediate);
                        GUI_InspectorBone("Index Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexDistal);
                        GUI_InspectorBone("Middle Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleProximal);
                        GUI_InspectorBone("Middle Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleIntermediate);
                        GUI_InspectorBone("Middle Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleDistal);
                        GUI_InspectorBone("Ring Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftRingProximal);
                        GUI_InspectorBone("Ring Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftRingIntermediate);
                        GUI_InspectorBone("Ring Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftRingDistal);
                        GUI_InspectorBone("Little Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleProximal);
                        GUI_InspectorBone("Little Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleIntermediate);
                        GUI_InspectorBone("Little Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleDistal);
                    }
                    #endregion
                }
                else if (humanoidAvatarMappingMode == HumanoidAvatarMappingMode.RightHand)
                {
                    #region Right Fingers
                    mappingRightFingersFoldout = EditorGUILayout.Foldout(mappingRightFingersFoldout, "Right Fingers");
                    if (mappingRightFingersFoldout)
                    {
                        GUI_InspectorBone("Thumb Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightThumbProximal);
                        GUI_InspectorBone("Thumb Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.RightThumbIntermediate);
                        GUI_InspectorBone("Thumb Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightThumbDistal);
                        GUI_InspectorBone("Index Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightIndexProximal);
                        GUI_InspectorBone("Index Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.RightIndexIntermediate);
                        GUI_InspectorBone("Index Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightIndexDistal);
                        GUI_InspectorBone("Middle Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleProximal);
                        GUI_InspectorBone("Middle Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleIntermediate);
                        GUI_InspectorBone("Middle Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleDistal);
                        GUI_InspectorBone("Ring Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightRingProximal);
                        GUI_InspectorBone("Ring Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.RightRingIntermediate);
                        GUI_InspectorBone("Ring Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightRingDistal);
                        GUI_InspectorBone("Little Proximal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightLittleProximal);
                        GUI_InspectorBone("Little Intermediate", false, VoxelSkinnedAnimationObject.HumanoidBone.RightLittleIntermediate);
                        GUI_InspectorBone("Little Distal", false, VoxelSkinnedAnimationObject.HumanoidBone.RightLittleDistal);
                    }
                    #endregion
                }
                #endregion

                EditorGUILayout.EndScrollView();

                EditorGUILayout.EndVertical();

                #region Common Menu
                {
                    GUILayout.BeginHorizontal("", guiStyleGVToolbar, GUILayout.ExpandWidth(true));

                    #region Mapping
                    {
                        var mappingContent = new GUIContent("Mapping");
                        var rect = GUILayoutUtility.GetRect(mappingContent, guiStyleGVGizmoDropDown);
                        if (GUI.Button(rect, mappingContent, guiStyleGVGizmoDropDown))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Clear"), false, () => { animationCore.ResetHumanDescriptionHuman(); });
                            menu.AddItem(new GUIContent("Automap"), false, () => { animationCore.AutomapHumanDescriptionHuman(); });
                            menu.ShowAsContext();
                        }
                    }
                    #endregion

                    #region Pose
                    {
                        var posecontent = new GUIContent("Pose");
                        var rect = GUILayoutUtility.GetRect(posecontent, guiStyleGVGizmoDropDown);
                        if (GUI.Button(rect, posecontent, guiStyleGVGizmoDropDown))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Reset"), false, () => { animationCore.ResetBoneTransform(); });
                            menu.ShowAsContext();
                        }
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();
                }
                #endregion
            }
            else if (humanoidAvatarConfigreMode == HumanoidAvatarConfigreMode.MusclesAndSettings)
            {
                #region Additional Settings
                {
                    #region BackGround
                    {
                        var rect = EditorGUILayout.GetControlRect();
                        rect.position = new Vector2(rect.position.x, rect.position.y + 7);
                        rect.width -= 1;
                        var width = rect.width;
                        rect.width = 80f;
                        GUI.Box(rect, "", guiStyleOLTitle);
                        rect.position = new Vector2(rect.position.x + rect.width, rect.position.y);
                        rect.width = width - rect.width;
                        GUI.Box(rect, "Additional Settings", guiStyleOLTitle);
                        GUILayout.Space(rect.height + 10);
                    }
                    #endregion

                    #region Params
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);

                        GUI_AddsionalSettingsSlider("Upper Arm Twist", ref animationObject.humanDescription.upperArmTwist);
                        GUI_AddsionalSettingsSlider("Lower Arm Twist", ref animationObject.humanDescription.lowerArmTwist);
                        GUI_AddsionalSettingsSlider("Upper Leg Twist", ref animationObject.humanDescription.upperLegTwist);
                        GUI_AddsionalSettingsSlider("Lower Leg Twist", ref animationObject.humanDescription.lowerLegTwist);
                        GUI_AddsionalSettingsSlider("Arm Stretch", ref animationObject.humanDescription.armStretch);
                        GUI_AddsionalSettingsSlider("Leg Stretch", ref animationObject.humanDescription.legStretch);
                        GUI_AddsionalSettingsSlider("Feet Spacing", ref animationObject.humanDescription.feetSpacing);
                        {
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Space(88);
                            EditorGUILayout.LabelField("Translation DoF", GUILayout.Width(105));
                            EditorGUI.BeginChangeCheck();
                            var tmp = EditorGUILayout.Toggle(animationObject.humanDescription.hasTranslationDoF);
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(animationObject, "Additional Settings");
                                animationObject.humanDescription.hasTranslationDoF = tmp;
                            }
                            EditorGUILayout.EndHorizontal();
                        }

                        EditorGUILayout.EndVertical();
                    }
                    #endregion
                }
                #endregion

                #region Common Menu
                {
                    GUILayout.BeginHorizontal("", guiStyleGVToolbar, GUILayout.ExpandWidth(true));

                    #region Muscles
                    {
                        var musclesContent = new GUIContent("Muscles");
                        var rect = GUILayoutUtility.GetRect(musclesContent, guiStyleGVGizmoDropDown);
                        if (GUI.Button(rect, musclesContent, guiStyleGVGizmoDropDown))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Reset"), false, () => { animationObject.humanDescription.ResetAdditionalSettings(); });
                            menu.ShowAsContext();
                        }
                    }
                    #endregion

                    GUILayout.FlexibleSpace();

                    GUILayout.EndHorizontal();
                }
                #endregion
            }

            #region Button
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.FlexibleSpace();

                    EditorGUI.BeginDisabledGroup(!animationObject.humanDescription.IsChanged(ref saveHumanDescription));

                    #region Revert
                    if (GUILayout.Button("Revert"))
                    {
                        Undo.RecordObject(animationObject, "Configure Avatar Revert");
                        animationObject.humanDescription = new VoxelSkinnedAnimationObject.VoxelImporterHumanDescription(ref saveHumanDescription);
                    }
                    #endregion

                    #region Apply
                    if (GUILayout.Button("Apply"))
                    {
                        Undo.RecordObject(animationObject, "Configure Avatar Apply");
                        saveHumanDescription = new VoxelSkinnedAnimationObject.VoxelImporterHumanDescription(ref animationObject.humanDescription);
                        animationCore.UpdateBoneWeight();
                    }
                    #endregion

                    EditorGUI.EndDisabledGroup();

                    #region Done
                    if (GUILayout.Button("Done"))
                    {
                        Close();
                    }
                    #endregion
                }
                EditorGUILayout.EndHorizontal();
            }
            #endregion
        }

        private void GUI_InspectorBone(string name, bool required, VoxelSkinnedAnimationObject.HumanoidBone select)
        {
            string errorMessage = "";

            var bone = GetHumanDescriptionBone(select);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(19);
            {
                var saveGUIColor = GUI.color;

                Texture2D frameTex = required ? dotframe : dotframedotted;
                if (bone != null)
                {
                    if (IsBoneError(select, ref errorMessage))
                    {
                        GUI.color = new Color(0.8f, 0.2f, 0.2f);
                    }
                    else
                    {
                        GUI.color = new Color(0.2f, 0.8f, 0.2f);
                    }
                }
                else
                {
                    GUI.color = Color.gray;
                }
                guiStyleBoneButton.normal.background = frameTex;
                guiStyleBoneButton.normal.scaledBackgrounds = null;
                guiStyleBoneButton.active.background = frameTex;
                guiStyleBoneButton.active.scaledBackgrounds = null;
                if (GUILayout.Button(bone != null ? dotfill : null, guiStyleBoneButton, GUILayout.Width(frameTex.width), GUILayout.Height(frameTex.height)))
                {
                    selectBone = select;
                    if (bone != null)
                    {
                        Selection.activeGameObject = bone.gameObject;
                        EditorGUIUtility.PingObject(Selection.activeGameObject);
                    }
                }

                if (selectBone == select)
                {
                    GUILayout.Space(-frameTex.width);
                    guiStyleBoneButton.normal.background = null;
                    guiStyleBoneButton.active.background = null;
                    GUI.color = new Color32(102, 178, 255, 255);
                    GUILayout.Button(dotselection, guiStyleBoneButton, GUILayout.Width(dotselection.width), GUILayout.Height(dotselection.height));
                }

                GUI.color = saveGUIColor;
                GUILayout.Space(-4);
            }
            {
                EditorGUI.BeginChangeCheck();
                var boneTmp = (VoxelSkinnedAnimationObjectBone)EditorGUILayout.ObjectField(name, bone, typeof(VoxelSkinnedAnimationObjectBone), true);
                if (EditorGUI.EndChangeCheck())
                {
                    if (boneTmp == null || boneTmp.voxelObject == animationObject)
                    {
                        Undo.RecordObject(animationObject, "Inspector");
                        SetHumanDescriptionBone(select, boneTmp);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(errorMessage))
            {
                EditorGUI.indentLevel += 2;
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
                EditorGUI.indentLevel -= 2;
            }
        }
        private void GUI_PreviewBone(Vector2 position, bool required, VoxelSkinnedAnimationObject.HumanoidBone select)
        {
            var bone = GetHumanDescriptionBone(select);

            var saveGUIColor = GUI.color;

            Texture2D frameTex = required ? dotframe : dotframedotted;
            if (bone != null)
            {
                string errorMessage = "";
                if (IsBoneError(select, ref errorMessage))
                {
                    GUI.color = new Color(0.8f, 0.2f, 0.2f);
                }
                else
                {
                    GUI.color = new Color(0.2f, 0.8f, 0.2f);
                }
            }
            else
            {
                GUI.color = Color.gray;
            }
            Rect rect = new Rect(new Vector2(position.x - frameTex.width / 2f, position.y - frameTex.height / 2f), new Vector2(frameTex.width, frameTex.height));

            guiStyleBoneButton.normal.background = frameTex;
            guiStyleBoneButton.active.background = frameTex;
            if (GUI.Button(rect, bone != null ? dotfill : null, guiStyleBoneButton))
            {
                selectBone = select;
                if (bone != null)
                {
                    Selection.activeGameObject = bone.gameObject;
                    EditorGUIUtility.PingObject(Selection.activeGameObject);
                }
            }

            if (selectBone == select)
            {
                guiStyleBoneButton.normal.background = null;
                guiStyleBoneButton.active.background = null;
                GUI.color = new Color32(102, 178, 255, 255);
                GUI.Button(rect, dotselection, guiStyleBoneButton);
            }

            GUI.color = saveGUIColor;
        }
        private void GUI_AddsionalSettingsSlider(string label, ref float param)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(88);
            EditorGUILayout.LabelField(label, GUILayout.Width(105));
            EditorGUI.BeginChangeCheck();
            var tmp = EditorGUILayout.Slider(param, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(animationObject, "Additional Settings");
                param = tmp;
            }
            EditorGUILayout.EndHorizontal();
        }

        private bool IsChildTransformCheck(VoxelSkinnedAnimationObjectBone parentBone, VoxelSkinnedAnimationObjectBone childBone)
        {
            var bone = childBone.transform.parent;
            while (bone.parent != null)
            {
                if (bone == parentBone.transform)
                    return true;

                bone = bone.parent;
            }
            return false;
        }

        private bool IsHeadEnable()
        {
            if (GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.Head) == null) return false;

            string errorMessage = "";
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.Neck; i <= VoxelSkinnedAnimationObject.HumanoidBone.Jaw; i++)
            {
                if (IsBoneError(i, ref errorMessage))
                    return false;
            }

            return true;
        }
        private bool IsTorsoEnable()
        {
            if (GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.Hips) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.Spine) == null) return false;

            string errorMessage = "";
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.Hips; i <= VoxelSkinnedAnimationObject.HumanoidBone.Chest; i++)
            {
                if (IsBoneError(i, ref errorMessage))
                    return false;
            }

            return true;
        }
        private bool IsLeftArmEnable()
        {
            if (GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperArm) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerArm) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.LeftHand) == null) return false;

            string errorMessage = "";
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.LeftShoulder; i <= VoxelSkinnedAnimationObject.HumanoidBone.LeftHand; i++)
            {
                if (IsBoneError(i, ref errorMessage))
                    return false;
            }

            return true;
        }
        private bool IsLeftFingersHave()
        {
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbProximal; i <= VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleDistal; i++)
            {
                if (GetHumanDescriptionBone(i) != null)
                    return true;
            }

            return false;
        }
        private bool IsLeftFingersEnable()
        {
            string errorMessage = "";
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbProximal; i <= VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleDistal; i++)
            {
                if (IsBoneError(i, ref errorMessage))
                    return false;
            }

            return true;
        }
        private bool IsLeftLegEnable()
        {
            if (GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperLeg) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerLeg) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.LeftFoot) == null) return false;

            string errorMessage = "";
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperLeg; i <= VoxelSkinnedAnimationObject.HumanoidBone.LeftToes; i++)
            {
                if (IsBoneError(i, ref errorMessage))
                    return false;
            }

            return true;
        }
        private bool IsRightArmEnable()
        {
            if (GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.RightUpperArm) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.RightLowerArm) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.RightHand) == null) return false;

            string errorMessage = "";
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.RightShoulder; i <= VoxelSkinnedAnimationObject.HumanoidBone.RightHand; i++)
            {
                if (IsBoneError(i, ref errorMessage))
                    return false;
            }

            return true;
        }
        private bool IsRightFingersHave()
        {
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.RightThumbProximal; i <= VoxelSkinnedAnimationObject.HumanoidBone.RightLittleDistal; i++)
            {
                if (GetHumanDescriptionBone(i) != null)
                    return true;
            }

            return false;
        }
        private bool IsRightFingersEnable()
        {
            string errorMessage = "";
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.RightThumbProximal; i <= VoxelSkinnedAnimationObject.HumanoidBone.RightLittleDistal; i++)
            {
                if (IsBoneError(i, ref errorMessage))
                    return false;
            }

            return true;
        }
        private bool IsRightLegEnable()
        {
            if (GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.RightUpperLeg) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.RightLowerLeg) == null ||
                GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.RightFoot) == null) return false;

            string errorMessage = "";
            for (var i = VoxelSkinnedAnimationObject.HumanoidBone.RightUpperLeg; i <= VoxelSkinnedAnimationObject.HumanoidBone.RightToes; i++)
            {
                if (IsBoneError(i, ref errorMessage))
                    return false;
            }

            return true;
        }

        private bool IsBoneError(VoxelSkinnedAnimationObject.HumanoidBone boneIndex, ref string errorMessage)
        {
            if (IsBoneUniqueCheck(boneIndex, ref errorMessage)) return true;
            if (IsBoneConflict(boneIndex, ref errorMessage)) return true;
            if (IsBoneTransformParentError(boneIndex, ref errorMessage)) return true;
            if (IsBoneLengthZero(boneIndex, ref errorMessage)) return true;

            return false;
        }
        private bool IsBoneUniqueCheck(VoxelSkinnedAnimationObject.HumanoidBone boneIndex, ref string errorMessage)
        {
            var bone = GetHumanDescriptionBone(boneIndex);
            if (bone == null) return false;

            switch (boneIndex)
            {
            case VoxelSkinnedAnimationObject.HumanoidBone.Hips:
                if (bone.transform.parent == null)
                {
                    errorMessage = bone.name + " cannot be the root transform";
                    return true;
                }
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.UpperChest:
                if (GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone.Chest) == null)
                {
                    errorMessage = "Chest must be assigned before assigning UpperChest.";
                    return true;
                }
                break;
            }

            return false;
        }
        private bool IsBoneConflict(VoxelSkinnedAnimationObject.HumanoidBone boneIndex, ref string errorMessage)
        {
            var bone = GetHumanDescriptionBone(boneIndex);
            if (bone == null) return false;

            int count = 0;
            for (int i = 0; i < animationObject.humanDescription.bones.Length; i++)
            {
                if (animationObject.humanDescription.bones[i] == bone) count++;
            }

            if (count != 1 && errorMessage != null)
            {
                string names = "";
                for (int i = 0; i < animationObject.humanDescription.bones.Length; i++)
                {
                    if ((VoxelSkinnedAnimationObject.HumanoidBone)i != boneIndex && animationObject.humanDescription.bones[i] == bone)
                    {
                        if (string.IsNullOrEmpty(names)) names = ((VoxelSkinnedAnimationObject.HumanoidBone)i).ToString();
                        else names += ", " + ((VoxelSkinnedAnimationObject.HumanoidBone)i).ToString();
                    }
                }
                errorMessage = string.Format("{0} Transform '{1}' is also assigned to {2}.", boneIndex.ToString(), bone.name, names);
            }

            return count != 1;
        }
        private bool IsBoneTransformParentError(VoxelSkinnedAnimationObject.HumanoidBone boneIndex, ref string errorMessage)
        {
            var bone = GetHumanDescriptionBone(boneIndex);
            if (bone == null) return false;

            var boneParent = bone.transform.parent;
            VoxelSkinnedAnimationObject.HumanoidBone errorParent = (VoxelSkinnedAnimationObject.HumanoidBone)(-1);

            Func<VoxelSkinnedAnimationObject.HumanoidBone, bool> CheckTransform = (checkIndex) =>
            {
                Transform checkTransform = null;
                {
                    var checkBone = GetHumanDescriptionBone(checkIndex);
                    if (checkBone == null)
                    {
                        errorParent = checkIndex;
                        return false;
                    }
                    checkTransform = checkBone.transform;
                }

                if (!boneParent.IsChildOf(checkTransform))
                {
                    errorParent = checkIndex;
                    return false;
                }

                return true;
            };

            bool result = true;
            switch (boneIndex)
            {
            case VoxelSkinnedAnimationObject.HumanoidBone.Hips:
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.Spine:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Hips);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.Chest:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Spine);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftShoulder:
                if (!CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.UpperChest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Chest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Spine))
                    result = false;
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperArm:
                if (!CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftShoulder) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.UpperChest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Chest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Spine))
                    result = false;
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerArm:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperArm);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftHand:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerArm);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightShoulder:
                if (!CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.UpperChest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Chest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Spine))
                    result = false;
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightUpperArm:
                if (!CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightShoulder) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.UpperChest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Chest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Spine))
                    result = false;
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightLowerArm:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightUpperArm);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightHand:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightLowerArm);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperLeg:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Hips);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerLeg:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftUpperLeg);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftFoot:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftLowerLeg);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftToes:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftFoot);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightUpperLeg:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Hips);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightLowerLeg:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightUpperLeg);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightFoot:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightLowerLeg);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightToes:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightFoot);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.Neck:
                if (!CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.UpperChest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Chest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Spine))
                    result = false;
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.Head:
                if (!CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Neck) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.UpperChest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Chest) &&
                    !CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Spine))
                    result = false;
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftEye:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Head);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightEye:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Head);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.Jaw:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Head);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftThumbIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftIndexIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftMiddleIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftRingProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftRingIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftRingProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftRingDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftRingIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.LeftLittleIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightThumbProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightThumbIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightThumbProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightThumbDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightThumbIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightIndexProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightIndexIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightIndexProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightIndexDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightIndexIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightMiddleIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightRingProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightRingIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightRingProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightRingDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightRingIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightLittleProximal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightHand);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightLittleIntermediate:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightLittleProximal);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.RightLittleDistal:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.RightLittleIntermediate);
                break;
            case VoxelSkinnedAnimationObject.HumanoidBone.UpperChest:
                result = CheckTransform(VoxelSkinnedAnimationObject.HumanoidBone.Chest);
                break;
            }

            if (!result)
            {
                var boneError = GetHumanDescriptionBone(errorParent);
                if (boneError != null)
                    errorMessage = string.Format("{0} Transform '{1}' is not a child of {2} Transform '{3}'.", boneIndex.ToString(), bone.name, errorParent.ToString(), boneError.name);
                else
                    errorMessage = string.Format("{0} Transform '{1}' parent {2} Transform is null.", boneIndex.ToString(), bone.name, errorParent.ToString());
            }

            return !result;
        }
        private bool IsBoneLengthZero(VoxelSkinnedAnimationObject.HumanoidBone boneIndex, ref string errorMessage)
        {
            var bone = GetHumanDescriptionBone(boneIndex);

            if (bone == null) return false;

            var result = bone.transform.localPosition.sqrMagnitude == 0f;

            if (result)
            {
                errorMessage = string.Format("{0} Transform '{1}' has bone length of zero.", boneIndex.ToString(), bone.name);
            }

            return result;
        }
        private VoxelSkinnedAnimationObjectBone GetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone boneIndex)
        {
            if (animationObject.humanDescription.bones != null && (int)boneIndex >= 0 && (int)boneIndex < animationObject.humanDescription.bones.Length)
                return animationObject.humanDescription.bones[(int)boneIndex];
            else
                return null;
        }
        private void SetHumanDescriptionBone(VoxelSkinnedAnimationObject.HumanoidBone boneIndex, VoxelSkinnedAnimationObjectBone bone)
        {
            if (animationObject.humanDescription.bones == null)
                animationObject.humanDescription.ResetMapping();
            if ((int)boneIndex >= 0 && animationObject.humanDescription.bones.Length <= (int)boneIndex)
                ArrayUtility.AddRange(ref animationObject.humanDescription.bones, new VoxelSkinnedAnimationObjectBone[(int)boneIndex - animationObject.humanDescription.bones.Length + 1]);
            animationObject.humanDescription.bones[(int)boneIndex] = bone;
        }
    }
}
