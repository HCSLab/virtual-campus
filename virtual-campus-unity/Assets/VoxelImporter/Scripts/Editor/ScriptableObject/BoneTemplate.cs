using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
	public class BoneTemplate : ScriptableObject
	{
		public void Set(Transform root)
		{
			boneInitializeData.Clear();
            var data = new BoneInitializeData() { name = root.name, parentName = null, position = root.localPosition };
            var controller = root.GetComponent<VoxelSkinnedAnimationObjectBone>();
            if (controller != null)
            {
                data.disablePositionAnimation = controller.edit_disablePositionAnimation;
                data.disableRotationAnimation = controller.edit_disableRotationAnimation;
                data.disableScaleAnimation = controller.edit_disableScaleAnimation;
                data.mirrorSetBoneAnimation = controller.edit_mirrorSetBoneAnimation;
                data.mirrorSetBonePosition = controller.edit_mirrorSetBonePosition;
                data.mirrorSetBoneWeight = controller.edit_mirrorSetBoneWeight;
            }
            boneInitializeData.Add(data);
			SetChildren(root);
		}
		private void SetChildren(Transform parent)
		{
			for (int i = 0; i < parent.childCount; i++)
			{
				var child = parent.GetChild(i);
                var data = new BoneInitializeData() { name = child.name, parentName = parent.name, position = child.localPosition };
                var controller = child.GetComponent<VoxelSkinnedAnimationObjectBone>();
                if (controller != null)
                {
                    data.disablePositionAnimation = controller.edit_disablePositionAnimation;
                    data.disableRotationAnimation = controller.edit_disableRotationAnimation;
                    data.disableScaleAnimation = controller.edit_disableScaleAnimation;
                    data.mirrorSetBoneAnimation = controller.edit_mirrorSetBoneAnimation;
                    data.mirrorSetBonePosition = controller.edit_mirrorSetBonePosition;
                    data.mirrorSetBoneWeight = controller.edit_mirrorSetBoneWeight;
                }
                boneInitializeData.Add(data);
				SetChildren(child);
			}
		}

		[Serializable]
		public struct BoneInitializeData
		{
			public string name;
			public string parentName;
			public Vector3 position;
            public bool disablePositionAnimation;
            public bool disableRotationAnimation;
            public bool disableScaleAnimation;
            public bool mirrorSetBoneAnimation;
            public bool mirrorSetBonePosition;
            public bool mirrorSetBoneWeight;
        }
		public List<BoneInitializeData> boneInitializeData = new List<BoneInitializeData>();
	}
}
