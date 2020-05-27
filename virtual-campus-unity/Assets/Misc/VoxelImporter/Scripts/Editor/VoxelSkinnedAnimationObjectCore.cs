using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif


namespace VoxelImporter
{
    public class VoxelSkinnedAnimationObjectCore : VoxelObjectCore
    {
        public VoxelSkinnedAnimationObject animationObject { get; protected set; }

        public VoxelSkinnedAnimationObjectCore(VoxelBase target) : base(target)
        {
            voxelObject = null;
            animationObject = target as VoxelSkinnedAnimationObject;
        }

        public override Mesh mesh { get { return animationObject.mesh; } set { animationObject.mesh = value; } }
        public override List<Material> materials { get { return animationObject.materials; } set { animationObject.materials = value; } }
        public override Texture2D atlasTexture { get { return animationObject.atlasTexture; } set { animationObject.atlasTexture = value; } }

        public override void Initialize()
        {
            base.Initialize();
            if (animationObject == null) return;

            #region BoneChangeRefresh
            if(IsVoxelFileExists())
            {
                bool boneChange = false;
                {
                    var bones = GetBones();
                    if (bones != null && animationObject.bones == null)
                        boneChange = true;
                    else if (bones == null && animationObject.bones != null)
                        boneChange = true;
                    else if (bones.Length != animationObject.bones.Length)
                        boneChange = true;
                    else
                    {
                        for (int i = 0; i < bones.Length; i++)
                        {
                            if (bones[i] != animationObject.bones[i])
                            {
                                boneChange = true;
                                break;
                            }
                        }
                    }
                }
                if (boneChange)
                {
                    EditorApplication.delayCall += () =>
                    {
                        ReCreate();
                    };
                }
            }
            #endregion

            if (animationObject.voxelData == null)
            {
                animationObject.editMode = VoxelSkinnedAnimationObject.Edit_Mode.None;
            }

            UpdateBoneWeightTable();
        }
        
        #region CreateMesh
        protected override bool IsCombineVoxelFace(IntVector3 basePos, IntVector3 combinePos, VoxelBase.Face face)
        {
            if (!base.IsCombineVoxelFace(basePos, combinePos, face))
                return false;

            var baseWeights = boneWeightTable.Get(basePos);
            if (baseWeights == null)
            {
                return (boneWeightTable.Get(combinePos) == null) ? true : false;
            }
            else
            {
                var combineWeights = boneWeightTable.Get(combinePos);
                if (combineWeights == null)
                    return false;

                Assert.IsTrue(baseWeights.Length == (int)VoxelBase.VoxelVertexIndex.Total && combineWeights.Length == (int)VoxelBase.VoxelVertexIndex.Total);

                switch (face)
                {
                case VoxelBase.Face.forward:
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ])
                        return false;
                    break;
                case VoxelBase.Face.up:
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z])
                        return false;
                    break;
                case VoxelBase.Face.right:
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z])
                        return false;
                    break;
                case VoxelBase.Face.left:
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z])
                        return false;
                    break;
                case VoxelBase.Face.down:
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z])
                        return false;
                    break;
                case VoxelBase.Face.back:
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z] ||
                        baseWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z])
                        return false;
                    break;
                default:
                    break;
                }

                return true;
            }
        }
        protected override bool IsHiddenVoxelFace(IntVector3 basePos, VoxelBase.Face faceFlag)
        {
            Assert.IsTrue(faceFlag == VoxelBase.Face.forward || faceFlag == VoxelBase.Face.up || faceFlag == VoxelBase.Face.right || faceFlag == VoxelBase.Face.left || faceFlag == VoxelBase.Face.down || faceFlag == VoxelBase.Face.back);
            IntVector3 combinePos = basePos;
            {
                if (faceFlag == VoxelBase.Face.forward) combinePos.z++;
                if (faceFlag == VoxelBase.Face.up) combinePos.y++;
                if (faceFlag == VoxelBase.Face.right) combinePos.x++;
                if (faceFlag == VoxelBase.Face.left) combinePos.x--;
                if (faceFlag == VoxelBase.Face.down) combinePos.y--;
                if (faceFlag == VoxelBase.Face.back) combinePos.z--;
            }

            if (animationObject.ignoreCavity)
            {
                if (animationObject.voxelData.VoxelTableContains(combinePos) < 0 &&
                    !animationObject.voxelData.OutsideTableContains(combinePos))
                    return true;
            }

            var baseWeights = boneWeightTable.Get(basePos);
            var combineWeights = boneWeightTable.Get(combinePos);
            if (baseWeights == null && combineWeights == null)
                return true;
            if (baseWeights == null)
                baseWeights = BoneWeightTableDefault;
            if (combineWeights == null)
                combineWeights = BoneWeightTableDefault;

            switch (faceFlag)
            {
            case VoxelBase.Face.forward:
                {
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z]) return false;
                }
                break;
            case VoxelBase.Face.up:
                {
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z]) return false;
                }
                break;
            case VoxelBase.Face.right:
                {
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XYZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z]) return false;
                }
                break;
            case VoxelBase.Face.left:
                {
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._XYZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XYZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z]) return false;
                }
                break;
            case VoxelBase.Face.down:
                {
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XYZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XYZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z]) return false;
                }
                break;
            case VoxelBase.Face.back:
                {
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.XYZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._XY_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._XYZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex.X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex.X_YZ]) return false;
                    if (baseWeights[(int)VoxelBase.VoxelVertexIndex._X_Y_Z] != combineWeights[(int)VoxelBase.VoxelVertexIndex._X_YZ]) return false;
                }
                break;
            }

            return true;
        }
        protected override void CreateMeshBefore()
        {
            base.CreateMeshBefore();

            UpdateBoneWeight();
        }
        protected override void CreateMeshAfter()
        {
            UpdateSkinnedMeshBounds();
            SetRendererCompornent();

            base.CreateMeshAfter();
        }
        public override void SetRendererCompornent()
        {
            base.SetRendererCompornent();

            {
                var renderer = animationObject.GetComponent<SkinnedMeshRenderer>();
                Undo.RecordObject(renderer, "Inspector");
                {//Unity Bug? force update.
                    renderer.enabled = !renderer.enabled;
                    renderer.enabled = !renderer.enabled;
                }
                if (animationObject.mesh != null && animationObject.mesh.vertexCount > 0)
                    renderer.sharedMesh = animationObject.mesh;
            }

            if (animationObject.updateAnimatorAvatar)
            {
                var animator = voxelBase.GetComponent<Animator>();
                if (animator != null)
                {
                    Undo.RecordObject(animator, "Inspector");
                    #region TransformSave
                    TransformSave[] transformsSave = null;
                    if (animator.isHuman)
                    {
                        if (!animator.isInitialized)
                            animator.Rebind();
                        transformsSave = new TransformSave[(int)HumanBodyBones.LastBone];
                        for (var i = 0; i < (int)HumanBodyBones.LastBone; i++)
                        {
                            var t = animator.GetBoneTransform((HumanBodyBones)i);
                            if (t == null) continue;
                            transformsSave[i] = new TransformSave(t);
                        }
                    }
                    #endregion
                    animator.avatar = animationObject.avatar;   //Since the Transform will return to its original position, the position is fixed before and after this
                    #region TransformLoad
                    if (animator.isHuman && transformsSave != null)
                    {
                        if (!animator.isInitialized)
                            animator.Rebind();
                        for (var i = 0; i < (int)HumanBodyBones.LastBone; i++)
                        {
                            var t = animator.GetBoneTransform((HumanBodyBones)i);
                            if (t == null || transformsSave[i] == null) continue;
                            transformsSave[i].LoadLocal(t);
                        }
                    }
                    #endregion
                }
            }
        }
        public void UpdateSkinnedMeshBounds()
        {
            var renderer = animationObject.GetComponent<SkinnedMeshRenderer>();
            if (animationObject.skinnedMeshBoundsUpdate)
            {
                Undo.RecordObject(renderer, "Update Skinned Mesh Bounds");
                var bounds = animationObject.mesh.bounds;
                if (animationObject.rootBone != null)
                    bounds.center = Matrix4x4.TRS(animationObject.rootBone.transform.localPosition, animationObject.rootBone.transform.localRotation, animationObject.rootBone.transform.localScale).inverse.MultiplyPoint3x4(bounds.center);
                bounds.size = Vector3.Scale(bounds.size, animationObject.skinnedMeshBoundsUpdateScale);
                if (float.IsNaN(bounds.center.x) || float.IsNaN(bounds.center.y) || float.IsNaN(bounds.center.z))
                    bounds.center = Vector3.zero;
                renderer.localBounds = bounds;
            }
        }
        #endregion

        #region BoneWeight
        private static readonly BoneWeight BoneWeightDefault = new BoneWeight() { boneIndex0 = 0, weight0 = 1f };
        private static readonly BoneWeight[] BoneWeightTableDefault = new BoneWeight[(int)VoxelBase.VoxelVertexIndex.Total] { BoneWeightDefault, BoneWeightDefault, BoneWeightDefault, BoneWeightDefault, BoneWeightDefault, BoneWeightDefault, BoneWeightDefault, BoneWeightDefault };
        private VoxelSkinnedAnimationObjectBoneCore[] bonesCore;
        private DataTable3<BoneWeight[]> boneWeightTable;
        public override bool isHaveBoneWeight { get { return true; } }
        public override Matrix4x4[] GetBindposes() { return animationObject.bindposes; }
        public override BoneWeight GetBoneWeight(IntVector3 pos, VoxelBase.VoxelVertexIndex index)
        {
            var boneWeights = boneWeightTable.Get(pos);
            if (boneWeights == null || boneWeights[(int)index].weight0 <= 0f)
            {
                return BoneWeightDefault;
            }
            else
            {
                return boneWeights[(int)index];
            }
        }
        public void UpdateBoneWeight()
        {
            Undo.RecordObject(animationObject, "Update Bone Weight");

            ReadyVoxelData();

            UpdateBoneBindposes();
            
            for (int i = 0; i < bonesCore.Length; i++)
            {
                bonesCore[i].UpdateBoneWeight(i);
            }

            UpdateBoneWeightTable();

            UpdateAvatar();
        }
        public VoxelSkinnedAnimationObjectBone[] GetBones()
        {
            List<VoxelSkinnedAnimationObjectBone> list = new List<VoxelSkinnedAnimationObjectBone>();
            {
                Action<Transform> GetChildrenBones = null;
                GetChildrenBones = (trans) =>
                {
                    var comp = trans.GetComponent<VoxelSkinnedAnimationObjectBone>();
                    if (comp != null)
                    {
                        list.Add(comp);
                        for (int i = 0; i < trans.childCount; i++)
                        {
                            GetChildrenBones(trans.GetChild(i));
                        }
                    }
                };
                {
                    var transformCache = animationObject.transform;
                    for (int i = 0; i < transformCache.childCount; i++)
                    {
                        GetChildrenBones(transformCache.GetChild(i));
                    }
                }
            }
            return list.ToArray();
        }
        public void UpdateBoneBindposes()
        {
            #region Bone
            animationObject.bones = GetBones();
            {
                bonesCore = new VoxelSkinnedAnimationObjectBoneCore[animationObject.bones.Length];
                for (int i = 0; i < animationObject.bones.Length; i++)
                {
                    bonesCore[i] = new VoxelSkinnedAnimationObjectBoneCore(animationObject.bones[i], this);
                }
            }
            #endregion

            #region MirrorBone
            for (int i = 0; i < animationObject.bones.Length; i++)
            {
                if (animationObject.bones[i].mirrorBone != null) continue;

                string mirrorName = null;
                if (animationObject.bones[i].name.IndexOf("Left") >= 0)
                    mirrorName = animationObject.bones[i].name.Replace("Left", "Right");
                else if (animationObject.bones[i].name.IndexOf("Right") >= 0)
                    mirrorName = animationObject.bones[i].name.Replace("Right", "Left");
                else
                    continue;

                for (int j = 0; j < animationObject.bones.Length; j++)
                {
                    if (i == j) continue;
                    if (animationObject.bones[j].name == mirrorName)
                    {
                        animationObject.bones[i].mirrorBone = animationObject.bones[j];
                        break;
                    }
                }
            }
            #endregion

            Transform[] boneTransforms = new Transform[animationObject.bones.Length];
            for (int i = 0; i < animationObject.bones.Length; i++)
            {
                boneTransforms[i] = animationObject.bones[i].transform;
            }

            #region UpdateBonePosition
            if (animationObject.editMode == VoxelSkinnedAnimationObject.Edit_Mode.BonePosition)
            {
                Undo.RecordObjects(animationObject.bones, "Update Bone Positions");
                for (int i = 0; i < animationObject.bones.Length; i++)
                {
                    animationObject.bones[i].bonePosition = animationObject.bones[i].transform.localPosition;
                    animationObject.bones[i].boneRotation = animationObject.bones[i].transform.localRotation;
                    animationObject.bones[i].bonePositionSave = true;
                }
            }
            else
            {
                Undo.RecordObjects(animationObject.bones, "Update Bone Positions");
                for (int i = 0; i < animationObject.bones.Length; i++)
                {
                    if (!animationObject.bones[i].bonePositionSave)
                    {
                        animationObject.bones[i].bonePosition = animationObject.bones[i].transform.localPosition;
                        animationObject.bones[i].boneRotation = animationObject.bones[i].transform.localRotation;
                        animationObject.bones[i].bonePositionSave = true;
                    }
                }
            }
            #endregion

            {
                var renderer = animationObject.GetComponent<SkinnedMeshRenderer>();
                renderer.bones = boneTransforms;
                if (boneTransforms.Length > 0)
                    renderer.rootBone = boneTransforms[0];
                else
                    renderer.rootBone = null;
                animationObject.bindposes = new Matrix4x4[boneTransforms.Length];
                {
                    var world = animationObject.transform.localToWorldMatrix;

                    List<Vector3> savePosition = new List<Vector3>(boneTransforms.Length);
                    List<Quaternion> saveRot = new List<Quaternion>(boneTransforms.Length);
                    List<Vector3> saveScale = new List<Vector3>(boneTransforms.Length);
                    for (int i = 0; i < boneTransforms.Length; i++)
                    {
                        savePosition.Add(boneTransforms[i].localPosition);
                        boneTransforms[i].localPosition = animationObject.bones[i].bonePosition;
                        saveRot.Add(boneTransforms[i].localRotation);
                        boneTransforms[i].localRotation = animationObject.bones[i].boneRotation;
                        saveScale.Add(boneTransforms[i].localScale);
                        boneTransforms[i].localScale = Vector3.one;
                    }
                    for (int i = 0; i < boneTransforms.Length; i++)
                    {
                        animationObject.bindposes[i] = boneTransforms[i].worldToLocalMatrix * world;
                    }
                    for (int i = 0; i < boneTransforms.Length; i++)
                    {
                        boneTransforms[i].localPosition = savePosition[i];
                        boneTransforms[i].localRotation = saveRot[i];
                        boneTransforms[i].localScale = saveScale[i];
                    }
                }
            }
        }
        public void UpdateBoneWeightTable()
        {
            #region Weight Update
            if(animationObject.voxelData != null)
            {
                boneWeightTable = new DataTable3<BoneWeight[]>(animationObject.voxelData.voxelSize.x, animationObject.voxelData.voxelSize.y, animationObject.voxelData.voxelSize.z);
                for (int i = 0; i < animationObject.bones.Length; i++)
                {
                    animationObject.bones[i].weightData.AllAction((pos, weights) =>
                    {
                        var boneWeights = boneWeightTable.Get(pos);
                        if (boneWeights == null)
                            boneWeights = new BoneWeight[(int)VoxelBase.VoxelVertexIndex.Total];
                        for (int k = 0; k < (int)VoxelBase.VoxelVertexIndex.Total; k++)
                        {
                            var weight = weights.GetWeight((VoxelBase.VoxelVertexIndex)k);
                            if (weight == 0f) continue;
                            if (boneWeights[k].weight0 == 0f)
                            {
                                boneWeights[k].boneIndex0 = i;
                                boneWeights[k].weight0 = weight;
                            }
                            else if (boneWeights[k].weight1 == 0f)
                            {
                                boneWeights[k].boneIndex1 = i;
                                boneWeights[k].weight1 = weight;
                            }
                            else if (boneWeights[k].weight2 == 0f)
                            {
                                boneWeights[k].boneIndex2 = i;
                                boneWeights[k].weight2 = weight;
                            }
                            else if (boneWeights[k].weight3 == 0f)
                            {
                                boneWeights[k].boneIndex3 = i;
                                boneWeights[k].weight3 = weight;
                            }
                        }
                        boneWeightTable.Set(pos, boneWeights);
                    });
                }
                for (int i = 0; i < animationObject.bones.Length; i++)
                {
                    animationObject.bones[i].weightData.AllAction((pos, weights) =>
                    {
                        var boneWeights = boneWeightTable.Get(pos);
                        for (int k = 0; k < (int)VoxelBase.VoxelVertexIndex.Total; k++)
                        {
                            if (boneWeights[k].weight3 > 0f)
                            {
                                float power = boneWeights[k].weight0 + boneWeights[k].weight1 + boneWeights[k].weight2 + boneWeights[k].weight3;
                                boneWeights[k].weight0 = boneWeights[k].weight0 / power;
                                boneWeights[k].weight1 = boneWeights[k].weight1 / power;
                                boneWeights[k].weight2 = boneWeights[k].weight2 / power;
                                boneWeights[k].weight3 = boneWeights[k].weight3 / power;
                            }
                            else if (boneWeights[k].weight2 > 0f)
                            {
                                float power = boneWeights[k].weight0 + boneWeights[k].weight1 + boneWeights[k].weight2;
                                if (power >= 1f)
                                {
                                    boneWeights[k].weight0 = boneWeights[k].weight0 / power;
                                    boneWeights[k].weight1 = boneWeights[k].weight1 / power;
                                    boneWeights[k].weight2 = boneWeights[k].weight2 / power;
                                }
                                else
                                {
                                    boneWeights[k].boneIndex3 = 0;
                                    boneWeights[k].weight3 = 1f - power;
                                }
                            }
                            else if (boneWeights[k].weight1 > 0f)
                            {
                                float power = boneWeights[k].weight0 + boneWeights[k].weight1;
                                if (power >= 1f)
                                {
                                    boneWeights[k].weight0 = boneWeights[k].weight0 / power;
                                    boneWeights[k].weight1 = boneWeights[k].weight1 / power;
                                }
                                else
                                {
                                    boneWeights[k].boneIndex2 = 0;
                                    boneWeights[k].weight2 = 1f - power;
                                }
                            }
                            else if (boneWeights[k].weight0 > 0f)
                            {
                                float power = boneWeights[k].weight0;
                                if (power >= 1f)
                                {
                                    boneWeights[k].weight0 = 1f;
                                }
                                else
                                {
                                    boneWeights[k].boneIndex1 = 0;
                                    boneWeights[k].weight1 = 1f - power;
                                }
                            }
                            else
                            {
                                boneWeights[k] = BoneWeightDefault;
                            }
                            #region Sort
                            do
                            {
                                if (boneWeights[k].weight1 > 0 && boneWeights[k].weight0 < boneWeights[k].weight1)
                                {
                                    var boneIndex = boneWeights[k].boneIndex0;
                                    var weight = boneWeights[k].weight0;
                                    boneWeights[k].boneIndex0 = boneWeights[k].boneIndex1;
                                    boneWeights[k].weight0 = boneWeights[k].weight1;
                                    boneWeights[k].boneIndex1 = boneIndex;
                                    boneWeights[k].weight1 = weight;
                                    continue;
                                }
                                if (boneWeights[k].weight2 > 0 && boneWeights[k].weight1 < boneWeights[k].weight2)
                                {
                                    var boneIndex = boneWeights[k].boneIndex1;
                                    var weight = boneWeights[k].weight1;
                                    boneWeights[k].boneIndex1 = boneWeights[k].boneIndex2;
                                    boneWeights[k].weight1 = boneWeights[k].weight2;
                                    boneWeights[k].boneIndex2 = boneIndex;
                                    boneWeights[k].weight2 = weight;
                                    continue;
                                }
                                if (boneWeights[k].weight3 > 0 && boneWeights[k].weight2 < boneWeights[k].weight3)
                                {
                                    var boneIndex = boneWeights[k].boneIndex2;
                                    var weight = boneWeights[k].weight2;
                                    boneWeights[k].boneIndex2 = boneWeights[k].boneIndex3;
                                    boneWeights[k].weight2 = boneWeights[k].weight3;
                                    boneWeights[k].boneIndex3 = boneIndex;
                                    boneWeights[k].weight3 = weight;
                                    continue;
                                }
                            } while (false);
                            #endregion
                        }
                        boneWeightTable.Set(pos, boneWeights);
                    });
                }
            }
            else
            {
                boneWeightTable = new DataTable3<BoneWeight[]>();
            }
            #endregion
        }
        #endregion
        
        #region Voxel
        public void GetVoxelsFeetArea(out IntVector3 areaMin, out IntVector3 areaMax)
        {
            areaMin = new IntVector3(int.MaxValue, 0, int.MaxValue);
            areaMax = new IntVector3(int.MinValue, voxelData.voxelSize.y - 1, int.MinValue);

            #region Step1
            {
                int minY = voxelData.voxelSize.y - 1;
                for (int i = 0; i < voxelData.voxels.Length; i++)
                {
                    minY = Math.Min(minY, voxelData.voxels[i].y);
                }
                for (int i = 0; i < voxelData.voxels.Length; i++)
                {
                    if (voxelData.voxels[i].y != minY) continue;
                    areaMin.x = Math.Min(areaMin.x, voxelData.voxels[i].x);
                    areaMin.z = Math.Min(areaMin.z, voxelData.voxels[i].z);
                    areaMax.x = Math.Max(areaMax.x, voxelData.voxels[i].x);
                    areaMax.z = Math.Max(areaMax.z, voxelData.voxels[i].z);
                }
                areaMin.y = minY;
                areaMax.y = minY;

                IntVector3 footAreaCenter = areaMin + (areaMax - areaMin) / 2;
                footAreaCenter.y = minY;
                if (voxelData.VoxelTableContains(footAreaCenter) < 0)
                {
                    int maxY = voxelData.voxelSize.y - 1;
                    for (int i = 0; i < voxelData.voxelSize.y; i++)
                    {
                        if (voxelData.VoxelTableContains(new IntVector3(footAreaCenter.x, i, footAreaCenter.z)) >= 0)
                        {
                            maxY = i - 1;
                            break;
                        }
                    }
                    areaMax.y = maxY;
                }
            }
            #endregion

            #region Step2
            {
                bool enable = false;
                var min = new IntVector3(int.MaxValue, areaMin.y, int.MaxValue);
                var max = new IntVector3(int.MinValue, areaMax.y, int.MinValue);
                for (int x = areaMin.x; x <= areaMax.x; x++)
                {
                    for (int z = areaMin.z; z <= areaMax.z; z++)
                    {
                        if (voxelData.VoxelTableContains(x, areaMin.y, z) < 0) continue;
                        bool e = true;
                        for (int y = areaMin.y; y <= areaMax.y; y++)
                        {
                            if (voxelData.VoxelTableContains(x, y, z) < 0)
                            {
                                e = false;
                                break;
                            }
                        }
                        if(e)
                        {
                            enable = true;
                            min = IntVector3.Min(min, new IntVector3(x, areaMin.y, z));
                            max = IntVector3.Max(max, new IntVector3(x, areaMax.y, z));
                        }
                    }
                }
                if(enable)
                {
                    areaMin = min;
                    areaMax = max;
                }
            }
            #endregion
        }
        public Vector3 GetVoxelsFeet()
        {
            Vector3 center = Vector3.zero;

            IntVector3 feetAreaMin, feetAreaMax;
            GetVoxelsFeetArea(out feetAreaMin, out feetAreaMax);

            Dictionary<int, List<Vector3>> centers = new Dictionary<int, List<Vector3>>();
            for (int i = 0; i < voxelData.voxels.Length; i++)
            {
                if (voxelData.voxels[i].x < feetAreaMin.x || voxelData.voxels[i].x > feetAreaMax.x ||
                    voxelData.voxels[i].y < feetAreaMin.y || voxelData.voxels[i].y > feetAreaMax.y ||
                    voxelData.voxels[i].z < feetAreaMin.z || voxelData.voxels[i].z > feetAreaMax.z)
                    continue;

                Vector3 pos;
                {
                    Vector3 posV3 = new Vector3(voxelData.voxels[i].position.x, voxelData.voxels[i].position.y, voxelData.voxels[i].position.z);
                    pos = voxelBase.localOffset + new Vector3(0.5f, 0f, 0.5f) + posV3;
                }
                if (!centers.ContainsKey(voxelData.voxels[i].y))
                    centers.Add(voxelData.voxels[i].y, new List<Vector3>());
                centers[voxelData.voxels[i].y].Add(pos);
            }

            Dictionary<int, Vector3> centerPositions = new Dictionary<int, Vector3>();
            foreach (var pair in centers)
            {
                Vector3 value = Vector3.zero;
                for (int i = 0; i < pair.Value.Count; i++)
                    value += pair.Value[i];
                value /= (float)pair.Value.Count;
                centerPositions.Add(pair.Key, value);
            }

            foreach (var pair in centerPositions)
            {
                center += pair.Value;
            }
            center /= (float)centerPositions.Count;
            center.x = Mathf.Round(center.x * 2f) / 2f;
            {
                Vector3 posV3 = new Vector3(feetAreaMin.x, feetAreaMin.y, feetAreaMin.z);
                center.y = (voxelBase.localOffset + posV3).y;
            }
            center.z = Mathf.Round(center.z * 2f) / 2f;

            return center;
        }
        #endregion

        #region Avatar
        protected void UpdateAvatar()
        {
            Undo.RecordObject(animationObject, "Update Avatar");

            string assetPath = "";
            if (EditorCommon.IsMainAsset(animationObject.avatar))
            {
                assetPath = AssetDatabase.GetAssetPath(animationObject.avatar);
            }
            if (EditorCommon.IsSubAsset(animationObject.avatar))
            {
                animationObject.avatar.name = animationObject.avatar.name + "_Destroyed";
                //Destroyed by "DestroyUnusedObjectInPrefabObject" to be called later
            }
            animationObject.avatar = null;

            var parent = animationObject.transform.parent;
            var localPosition = animationObject.transform.localPosition;
            var localRotation = animationObject.transform.localRotation;
            var localScale = animationObject.transform.localScale;
            animationObject.transform.SetParent(null);
            animationObject.transform.localPosition = Vector3.zero;
            animationObject.transform.localRotation = Quaternion.identity;
            animationObject.transform.localScale = Vector3.one;
            switch (animationObject.rigAnimationType)
            {
            case VoxelSkinnedAnimationObject.RigAnimationType.Generic:
                if (animationObject.rootBone != null)
                {
                    Dictionary<Transform, Transform> saveList = new Dictionary<Transform, Transform>();
                    List<Transform> findList = new List<Transform>();
                    for (int j = 0; j < animationObject.transform.childCount; j++)
                    {
                        findList.Add(animationObject.transform.GetChild(j));
                    }
                    for (int i = 0; i < findList.Count; i++)
                    {
                        for (int j = 0; j < findList[i].childCount; j++)
                        {
                            findList.Add(findList[i].GetChild(j));
                        }
                        if (findList[i].GetComponent<VoxelSkinnedAnimationObjectBone>() == null)
                        {
                            saveList.Add(findList[i], findList[i].parent);
                            findList[i].SetParent(null);
                        }
                    }
                    animationObject.avatar = AvatarBuilder.BuildGenericAvatar(animationObject.gameObject, animationObject.rootBone.gameObject.name);
                    {
                        var enu = saveList.GetEnumerator();
                        while (enu.MoveNext())
                        {
                            enu.Current.Key.SetParent(enu.Current.Value);
                        }
                    }
                }
                break;
            case VoxelSkinnedAnimationObject.RigAnimationType.Humanoid:
                if (animationObject.rootBone != null)
                {
                    HumanDescription humanDescription = new HumanDescription()
                    {
                        upperArmTwist = animationObject.humanDescription.upperArmTwist,
                        lowerArmTwist = animationObject.humanDescription.lowerArmTwist,
                        upperLegTwist = animationObject.humanDescription.upperLegTwist,
                        lowerLegTwist = animationObject.humanDescription.lowerLegTwist,
                        armStretch = animationObject.humanDescription.armStretch,
                        legStretch = animationObject.humanDescription.legStretch,
                        feetSpacing = animationObject.humanDescription.feetSpacing,
                        hasTranslationDoF = animationObject.humanDescription.hasTranslationDoF,
                    };
                    #region CreateHumanAndSkeleton
                    {
                        List<HumanBone> humanBones = new List<HumanBone>();
                        List<SkeletonBone> skeletonBones = new List<SkeletonBone>();

                        if (!animationObject.humanDescription.firstAutomapDone)
                        {
                            AutomapHumanDescriptionHuman();
                            animationObject.humanDescription.firstAutomapDone = true;
                        }
                        for (int i = 0; i < animationObject.humanDescription.bones.Length; i++)
                        {
                            var index = VoxelSkinnedAnimationObject.HumanTraitBoneNameTable[i];
                            if (animationObject.humanDescription.bones[(int)index] == null) continue;

                            humanBones.Add(new HumanBone()
                            {
                                boneName = animationObject.humanDescription.bones[(int)index].name,
                                humanName = HumanTrait.BoneName[i],
                                limit = new HumanLimit() { useDefaultValues = true },
                            });
                        }

                        #region FindBones
                        {
                            for (var bone = animationObject.rootBone; bone != animationObject.transform.parent; bone = bone.parent)
                            {
                                skeletonBones.Add(new SkeletonBone()
                                {
                                    name = bone.name,
                                    position = bone.localPosition,
                                    rotation = bone.localRotation,
                                    scale = bone.localScale,
                                });
                            }
                            skeletonBones.Reverse();
                            for (int i = 0; i < animationObject.humanDescription.bones.Length; i++)
                            {
                                var index = VoxelSkinnedAnimationObject.HumanTraitBoneNameTable[i];
                                var bone = animationObject.humanDescription.bones[(int)index];
                                if (bone == null) continue;
                                if (bone.transform == animationObject.rootBone)
                                    continue;

                                skeletonBones.Add(new SkeletonBone()
                                {
                                    name = bone.name,
                                    position = bone.transform.localPosition,
                                    rotation = bone.transform.localRotation,
                                    scale = Vector3.one,
                                });
                            }
                        }
                        #endregion
                        humanDescription.human = humanBones.ToArray();
                        humanDescription.skeleton = skeletonBones.ToArray();
                    }
                    #endregion
                    animationObject.avatar = AvatarBuilder.BuildHumanAvatar(animationObject.gameObject, humanDescription);
                }
                break;
            }
            animationObject.transform.SetParent(parent);
            animationObject.transform.localPosition = localPosition;
            animationObject.transform.localRotation = localRotation;
            animationObject.transform.localScale = localScale;
            if (animationObject.avatar != null)
            {
                if (!string.IsNullOrEmpty(assetPath))
                {
                    var tmpPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
                    AssetDatabase.CreateAsset(animationObject.avatar, tmpPath);
                    File.Copy(tmpPath, assetPath, true);
                    AssetDatabase.DeleteAsset(tmpPath);
                    animationObject.avatar = AssetDatabase.LoadAssetAtPath<Avatar>(assetPath);
                }
                if (!AssetDatabase.Contains(animationObject.avatar))
                {
                    AddObjectToPrefabAsset(animationObject.avatar, "avatar");
                }
            }
        }
        public void ResetHumanDescriptionHuman()
        {
            Undo.RecordObject(animationObject, "Reset Human Description");

            for (int i = 0; i < animationObject.humanDescription.bones.Length; i++)
            {
                animationObject.humanDescription.bones[i] = null;
            }
        }
        public void AutomapHumanDescriptionHuman()
        {
            Undo.RecordObject(animationObject, "Reset Human Description");

            ResetHumanDescriptionHuman();

            Func<string, VoxelSkinnedAnimationObjectBone> FindBone = (name) =>
            {
                VoxelSkinnedAnimationObjectBone bone = null;
                string nameS = name.Replace(" ", "");
                for (int i = 0; i < animationObject.bones.Length; i++)
                {
                    if (animationObject.bones[i] == null) continue;

                    if (animationObject.bones[i].name.IndexOf(name) >= 0 ||
                        animationObject.bones[i].name.IndexOf(nameS) >= 0)
                    {
                        bone = animationObject.bones[i];
                        break;
                    }
                }
                return bone;
            };

            #region FindBones
            {
                var BoneName = HumanTrait.BoneName;
                for (int i = 0; i < BoneName.Length; i++)
                {
                    var bone = FindBone(BoneName[i]);
                    if (bone != null)
                    {
                        var index = VoxelSkinnedAnimationObject.HumanTraitBoneNameTable[i];
                        animationObject.humanDescription.bones[(int)index] = bone;
                    }
                }
            }
            #endregion
        }
        public void ResetBoneTransform()
        {
            if (animationObject.bones == null) return;
            for (int i = 0; i < animationObject.bones.Length; i++)
            {
                if (animationObject.bones[i] == null) continue;
                Undo.RecordObject(animationObject.bones[i].transform, "Reset Bone Transform");
                if (animationObject.bones[i].bonePositionSave)
                {
                    animationObject.bones[i].transform.localPosition = animationObject.bones[i].bonePosition;
                    animationObject.bones[i].transform.localRotation = animationObject.bones[i].boneRotation;
                }
                animationObject.bones[i].transform.localScale = Vector3.one;
            }
        }
        private class TransformSave
        {
            public TransformSave()
            {
            }
            public TransformSave(Transform t)
            {
                Save(t);
            }
            public void Save(Transform t)
            {
                localPosition = t.localPosition;
                localRotation = t.localRotation;
                localScale = t.localScale;
            }
            public void LoadLocal(Transform t)
            {
                t.localPosition = localPosition;
                t.localRotation = localRotation;
                t.localScale = localScale;
            }

            public Vector3 localPosition;
            public Quaternion localRotation;
            public Vector3 localScale;
        }
        #endregion

        #region Animation
        public void FixMissingAnimation()
        {
            if (animationObject.rigAnimationType == VoxelSkinnedAnimationObject.RigAnimationType.Humanoid) return;

            var animator = animationObject.GetComponent<Animator>();
            if (animator != null && animator.runtimeAnimatorController != null)
            {
                Undo.RecordObject(animator, "Fix Missing Animation");
                Undo.RecordObject(animator.runtimeAnimatorController, "Fix Missing Animation");
                Undo.RecordObjects(animator.runtimeAnimatorController.animationClips, "Fix Missing Animation");
                for (int i = 0; i < animationObject.bones.Length; i++)
                {
                    var boneCore = new VoxelSkinnedAnimationObjectBoneCore(animationObject.bones[i], this);
                    boneCore.FixMissingAnimation();
                }
            }
        }
        #endregion

        #region Export
        public bool ExportDaeFileWithAnimation(string path, bool exportMesh, bool exportAnimation, bool enableFootIK)
        {
            Dictionary<Transform, SaveTransform> saveTransforms = new Dictionary<Transform, SaveTransform>();
            {
                saveTransforms.Add(animationObject.transform, new SaveTransform(animationObject.transform));
                animationObject.transform.localPosition = Vector3.zero;
                animationObject.transform.localRotation = Quaternion.identity;
                animationObject.transform.localScale = Vector3.one;
                for (int i = 0; i < animationObject.bones.Length; i++)
                {
                    saveTransforms.Add(animationObject.bones[i].transform, new SaveTransform(animationObject.bones[i].transform));
                    if (animationObject.bones[i].bonePositionSave)
                    {
                        animationObject.bones[i].transform.localPosition = animationObject.bones[i].bonePosition;
                        animationObject.bones[i].transform.localRotation = animationObject.bones[i].boneRotation;
                    }
                    animationObject.bones[i].transform.localScale = Vector3.one;
                }
            }
            bool result = false;
            try
            {
                var clips = exportAnimation ? AnimationUtility.GetAnimationClips(voxelBase.gameObject).Distinct().ToArray() : null;

                List<Transform> transforms = new List<Transform>();
                ExportDaeFile_AddTransform(transforms);
                DaeExporter exporter = new DaeExporter()
                {
                    settings_exportMesh = exportMesh,
                    settings_iKOnFeet = enableFootIK,
                };
                switch (animationObject.rigAnimationType)
                {
                case VoxelSkinnedAnimationObject.RigAnimationType.Legacy:
                    exporter.settings_animationType = ModelImporterAnimationType.Legacy;
                    break;
                case VoxelSkinnedAnimationObject.RigAnimationType.Humanoid:
                    exporter.settings_animationType = ModelImporterAnimationType.Human;
                    exporter.settings_avatar = animationObject.avatar;
                    break;
                default:
                    exporter.settings_animationType = ModelImporterAnimationType.Generic;
                    exporter.settings_avatar = animationObject.avatar;
                    exporter.settings_motionNodePath = animationObject.rootBone.gameObject.name;
                    break;
                }
                result = exporter.Export(path, transforms, clips);
                if (result)
                {
                    foreach (var p in exporter.exportedFiles)
                    {
                        if (p.IndexOf(Application.dataPath) < 0) continue;
                        var pTmp = FileUtil.GetProjectRelativePath(p);
                        var importer = AssetImporter.GetAtPath(pTmp);
                        if (importer is TextureImporter)
                        {
                            SetTextureImporterSetting(pTmp);
                            importer.SaveAndReimport();
                        }
                    }
                    AssetDatabase.Refresh();
                }
            }
            finally
            {
                foreach (var pair in saveTransforms)
                {
                    pair.Value.Load(pair.Key);
                }
            }
            return result;
        }
        protected override void ExportDaeFile_AddTransform(List<Transform> transforms)
        {
            base.ExportDaeFile_AddTransform(transforms);

            for (int i = 0; i < animationObject.bones.Length; i++)
            {
                transforms.Add(animationObject.bones[i].transform);
            }
        }
        #endregion

        #region Asset
        public override void ResetAllAssets()
        {
            #region Mesh
            animationObject.mesh = null;
            #endregion

            #region Material
            if (animationObject.materials != null)
            {
                for (int i = 0; i < animationObject.materials.Count; i++)
                {
                    if (animationObject.materials[i] == null)
                        continue;
                    animationObject.materials[i] = EditorCommon.Instantiate(animationObject.materials[i]);
                }
            }
            #endregion

            #region Texture
            animationObject.atlasTexture = null;
            #endregion

            #region Structure
            animationObject.voxelStructure = null;
            #endregion

            #region Avatar
            animationObject.avatar = null;
            #endregion
        }
        #endregion

        #region Undo
        protected override void RefreshCheckerCreate() { animationObject.refreshChecker = new VoxelSkinnedAnimationObject.RefreshCheckerSkinnedAnimation(animationObject); }
        #endregion
    }
}
