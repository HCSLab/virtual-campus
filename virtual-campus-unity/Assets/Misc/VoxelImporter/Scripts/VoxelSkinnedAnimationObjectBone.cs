using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
	public class VoxelSkinnedAnimationObjectBone : MonoBehaviour
    {
#if !UNITY_EDITOR        
        void Awake()
        {
            Destroy(this);
        }

#else
        public bool EditorInitialize()
        {
            return false;
        }

        private VoxelSkinnedAnimationObject _voxelObject;
        public VoxelSkinnedAnimationObject voxelObject
        {
            get
            {
                if (_voxelObject == null)
                {
                    var trans = transform.parent;
                    while (trans != null)
                    {
                        var ctl = trans.GetComponent<VoxelSkinnedAnimationObject>();
                        if (ctl != null)
                        {
                            _voxelObject = ctl;
                            break;
                        }
                        trans = trans.parent;
                    }
                }
                return _voxelObject;
            }
        }

        public VoxelSkinnedAnimationObjectBone mirrorBone;

        public int boneIndex = -1;

        public bool bonePositionSave;
        public Vector3 bonePosition;
        public Quaternion boneRotation;

        public WeightData weightData;

        #region Editor
        public Mesh[] edit_weightMesh = null;
        public Texture2D edit_weightColorTexture;

        public bool edit_disablePositionAnimation = true;
        public bool edit_disableRotationAnimation = false;
        public bool edit_disableScaleAnimation = true;
        public bool edit_mirrorSetBoneAnimation = true;
        public bool edit_mirrorSetBonePosition = true;
        public bool edit_mirrorSetBoneWeight = true;

        public bool edit_objectFoldout = true;
        #endregion
#endif
    }
}
