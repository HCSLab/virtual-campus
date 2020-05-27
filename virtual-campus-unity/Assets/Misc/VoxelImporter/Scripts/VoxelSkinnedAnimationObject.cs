using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{

    [AddComponentMenu("Voxel Importer/Voxel Skinned Animation Object")]
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class VoxelSkinnedAnimationObject : VoxelBase
    {
#if !UNITY_EDITOR   
        void Awake()
        {
            Destroy(this);
        }
#else
        public override bool EditorInitialize()
        {
            var result = base.EditorInitialize();

            //ver1.021 -> ver1.0.3
            if (material != null)
            {
                materials = new List<Material>();
                materials.Add(material);
                materialData = new List<MaterialData>();
                materialData.Add(new MaterialData());
                materialIndexes = new List<int>();
                materialIndexes.Add(0);
                material = null;
                result = true;
            }
            //ver1.0.6 -> ver1.0.6.p1
            {
                var meshFilter = GetComponent<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh == mesh)
                {
                    DestroyImmediate(meshFilter);
                }
            }

            return result;
        }

        public Mesh mesh;
        [SerializeField]
        protected Material material;        //ver1.021 old
        public List<Material> materials;    //ver1.0.3 new
        public Texture2D atlasTexture;

        public enum RigAnimationType
        {
            None = -1,
            Legacy,
            Generic,
            Humanoid,
        }
        public RigAnimationType rigAnimationType = RigAnimationType.None;
        public Avatar avatar;
        public bool updateAnimatorAvatar = true;

        public enum HumanoidBone
        {
            Hips,
            Spine,
            Chest,
            LeftShoulder,
            LeftUpperArm,
            LeftLowerArm,
            LeftHand,
            RightShoulder,
            RightUpperArm,
            RightLowerArm,
            RightHand,
            LeftUpperLeg,
            LeftLowerLeg,
            LeftFoot,
            LeftToes,
            RightUpperLeg,
            RightLowerLeg,
            RightFoot,
            RightToes,
            Neck,
            Head,
            LeftEye,
            RightEye,
            Jaw,
            LeftThumbProximal,
            LeftThumbIntermediate,
            LeftThumbDistal,
            LeftIndexProximal,
            LeftIndexIntermediate,
            LeftIndexDistal,
            LeftMiddleProximal,
            LeftMiddleIntermediate,
            LeftMiddleDistal,
            LeftRingProximal,
            LeftRingIntermediate,
            LeftRingDistal,
            LeftLittleProximal,
            LeftLittleIntermediate,
            LeftLittleDistal,
            RightThumbProximal,
            RightThumbIntermediate,
            RightThumbDistal,
            RightIndexProximal,
            RightIndexIntermediate,
            RightIndexDistal,
            RightMiddleProximal,
            RightMiddleIntermediate,
            RightMiddleDistal,
            RightRingProximal,
            RightRingIntermediate,
            RightRingDistal,
            RightLittleProximal,
            RightLittleIntermediate,
            RightLittleDistal,
            UpperChest,
            Total
        }
        public static readonly HumanoidBone[] HumanTraitBoneNameTable =
        {
            HumanoidBone.Hips,
            HumanoidBone.LeftUpperLeg,
            HumanoidBone.RightUpperLeg,
            HumanoidBone.LeftLowerLeg,
            HumanoidBone.RightLowerLeg,
            HumanoidBone.LeftFoot,
            HumanoidBone.RightFoot,
            HumanoidBone.Spine,
            HumanoidBone.Chest,
            HumanoidBone.Neck,
            HumanoidBone.Head,
            HumanoidBone.LeftShoulder,
            HumanoidBone.RightShoulder,
            HumanoidBone.LeftUpperArm,
            HumanoidBone.RightUpperArm,
            HumanoidBone.LeftLowerArm,
            HumanoidBone.RightLowerArm,
            HumanoidBone.LeftHand,
            HumanoidBone.RightHand,
            HumanoidBone.LeftToes,
            HumanoidBone.RightToes,
            HumanoidBone.LeftEye,
            HumanoidBone.RightEye,
            HumanoidBone.Jaw,
            HumanoidBone.LeftThumbProximal,
            HumanoidBone.LeftThumbIntermediate,
            HumanoidBone.LeftThumbDistal,
            HumanoidBone.LeftIndexProximal,
            HumanoidBone.LeftIndexIntermediate,
            HumanoidBone.LeftIndexDistal,
            HumanoidBone.LeftMiddleProximal,
            HumanoidBone.LeftMiddleIntermediate,
            HumanoidBone.LeftMiddleDistal,
            HumanoidBone.LeftRingProximal,
            HumanoidBone.LeftRingIntermediate,
            HumanoidBone.LeftRingDistal,
            HumanoidBone.LeftLittleProximal,
            HumanoidBone.LeftLittleIntermediate,
            HumanoidBone.LeftLittleDistal,
            HumanoidBone.RightThumbProximal,
            HumanoidBone.RightThumbIntermediate,
            HumanoidBone.RightThumbDistal,
            HumanoidBone.RightIndexProximal,
            HumanoidBone.RightIndexIntermediate,
            HumanoidBone.RightIndexDistal,
            HumanoidBone.RightMiddleProximal,
            HumanoidBone.RightMiddleIntermediate,
            HumanoidBone.RightMiddleDistal,
            HumanoidBone.RightRingProximal,
            HumanoidBone.RightRingIntermediate,
            HumanoidBone.RightRingDistal,
            HumanoidBone.RightLittleProximal,
            HumanoidBone.RightLittleIntermediate,
            HumanoidBone.RightLittleDistal,
            HumanoidBone.UpperChest,
        };

        [Serializable]
        public struct VoxelImporterHumanDescription
        {
            public VoxelImporterHumanDescription(bool reset = true)
            {
                firstAutomapDone = false;
                bones = null;
                upperArmTwist = 0.5f;
                lowerArmTwist = 0.5f;
                upperLegTwist = 0.5f;
                lowerLegTwist = 0.5f;
                armStretch = 0.05f;
                legStretch = 0.05f;
                feetSpacing = 0f;
                hasTranslationDoF = false;

                ResetMapping();
            }
            public VoxelImporterHumanDescription(ref VoxelImporterHumanDescription src)
            {
                firstAutomapDone = src.firstAutomapDone;
                bones = new VoxelSkinnedAnimationObjectBone[src.bones.Length];
                src.bones.CopyTo(bones, 0);
                upperArmTwist = src.upperArmTwist;
                lowerArmTwist = src.lowerArmTwist;
                upperLegTwist = src.upperLegTwist;
                lowerLegTwist = src.lowerLegTwist;
                armStretch = src.armStretch;
                legStretch = src.legStretch;
                feetSpacing = src.feetSpacing;
                hasTranslationDoF = src.hasTranslationDoF;
            }
            public bool IsChanged(ref VoxelImporterHumanDescription src)
            {
                if (firstAutomapDone != src.firstAutomapDone) return true;
                if (bones == null && src.bones != null) return true;
                if (bones != null && src.bones == null) return true;
                if (bones != null && src.bones != null)
                {
                    if (bones.Length != src.bones.Length) return true;
                    for (int i = 0; i < bones.Length; i++)
                    {
                        if (bones[i] != src.bones[i]) return true;
                    }
                }
                if(upperArmTwist != src.upperArmTwist) return true;
                if (lowerArmTwist != src.lowerArmTwist) return true;
                if (upperLegTwist != src.upperLegTwist) return true;
                if (lowerLegTwist != src.lowerLegTwist) return true;
                if (armStretch != src.armStretch) return true;
                if (legStretch != src.legStretch) return true;
                if (feetSpacing != src.feetSpacing) return true;
                if (hasTranslationDoF != src.hasTranslationDoF) return true;
                return false;
            }

            public void ResetMapping()
            {
                bones = new VoxelSkinnedAnimationObjectBone[(int)HumanoidBone.Total];
            }
            public void ResetAdditionalSettings()
            {
                upperArmTwist = 0.5f;
                lowerArmTwist = 0.5f;
                upperLegTwist = 0.5f;
                lowerLegTwist = 0.5f;
                armStretch = 0.05f;
                legStretch = 0.05f;
                feetSpacing = 0f;
                hasTranslationDoF = false;
            }

            //Mapping
            public bool firstAutomapDone;
            public VoxelSkinnedAnimationObjectBone[] bones;

            //Pre-Muscle Settings

            //Additional Settings
            public float upperArmTwist;
            public float lowerArmTwist;
            public float upperLegTwist;
            public float lowerLegTwist;
            public float armStretch;
            public float legStretch;
            public float feetSpacing;
            public bool hasTranslationDoF;
        }
        public VoxelImporterHumanDescription humanDescription = new VoxelImporterHumanDescription(true);

        public bool skinnedMeshBoundsUpdate = true;
        public Vector3 skinnedMeshBoundsUpdateScale = new Vector3(1.5f, 1.5f, 1.5f);

        #region BoneWeight
        public VoxelSkinnedAnimationObjectBone[] bones;
        public Transform rootBone { get { return (bones != null && bones.Length > 0 && bones[0] != null) ? bones[0].transform : null; } }
        public Matrix4x4[] bindposes;
        #endregion

        #region Editor
        public enum Edit_Mode
        {
            None,
            BoneAnimation,
            BonePosition,
            BoneWeight,
        }
        public Edit_Mode editMode;
        public Edit_Mode editLastMode;

        public enum Edit_VoxelMode
        {
            Voxel,
            Vertex,
        }
        public Edit_VoxelMode edit_voxelMode;

        public enum Edit_VoxelWeightMode
        {
            Voxel,
            Fill,
            Rect,
        }
        public Edit_VoxelWeightMode edit_voxelWeightMode;

        public enum Edit_VertexWeightMode
        {
            Brush,
            Rect,
        }
        public Edit_VertexWeightMode edit_vertexWeightMode;

        public enum Edit_BlendMode
        {
            Replace,
            Add,
            Subtract,
        }
        public Edit_BlendMode edit_blendMode;

        public float edit_weight = 1f;
        public bool edit_autoNormalize = true;
        public float edit_brushRadius = 10f;
        public AnimationCurve edit_brushCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

        public enum Edit_WeightPreviewMode
        {
            Opaque,
            Transparent,
        }
        public Edit_WeightPreviewMode edit_WeightPreviewMode;

        public enum Edit_MirrorSetMode
        {
            None,
            Positive,
            Negative,
        }
        public Edit_MirrorSetMode[] edit_mirrorPosition = new Edit_MirrorSetMode[3] { Edit_MirrorSetMode.Negative, Edit_MirrorSetMode.Positive, Edit_MirrorSetMode.Positive };
        public Edit_MirrorSetMode[] edit_mirrorRotation = new Edit_MirrorSetMode[3] { Edit_MirrorSetMode.Negative, Edit_MirrorSetMode.Positive, Edit_MirrorSetMode.Negative };
        public Edit_MirrorSetMode[] edit_mirrorScale = new Edit_MirrorSetMode[3] { Edit_MirrorSetMode.Positive, Edit_MirrorSetMode.Positive, Edit_MirrorSetMode.Positive };

        public bool edit_animationFoldout = true;

        #endregion

        #region Asset
        public override bool IsUseAssetObject(UnityEngine.Object obj)
        {
            if (mesh == obj) return true;
            if (materials != null)
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i] == obj) return true;
                }
            }
            if (atlasTexture == obj) return true;
            if (avatar == obj) return true;
            return false;
        }
        #endregion

        #region Undo
        public class RefreshCheckerSkinnedAnimation : RefreshChecker
        {
            public RefreshCheckerSkinnedAnimation(VoxelSkinnedAnimationObject voxelObject) : base(voxelObject)
            {
                controllerSkinnedAnimation = voxelObject;
            }

            public VoxelSkinnedAnimationObject controllerSkinnedAnimation;

            public VoxelSkinnedAnimationObjectBone[] bones;
            public Avatar avatar;

            public override void Save()
            {
                base.Save();

                if (controllerSkinnedAnimation.bones != null)
                {
                    bones = new VoxelSkinnedAnimationObjectBone[controllerSkinnedAnimation.bones.Length];
                    controllerSkinnedAnimation.bones.CopyTo(bones, 0);
                }
                else
                {
                    bones = null;
                }
                avatar = controllerSkinnedAnimation.avatar;
            }
            public override bool Check()
            {
                if (base.Check())
                    return true;

                if (bones == null && controllerSkinnedAnimation.bones == null) return false;
                if (bones == null || controllerSkinnedAnimation.bones == null) return true;
                if (bones.Length != controllerSkinnedAnimation.bones.Length) return true;
                for (int i = 0; i < bones.Length; i++)
                {
                    if (bones[i] != controllerSkinnedAnimation.bones[i]) return true;
                }

                if (avatar != controllerSkinnedAnimation.avatar) return true;

                return false;
            }
        }
        #endregion
#endif
    }

}
