using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace VoxelImporter
{
    [Serializable]
    public class WeightData : ISerializationCallbackReceiver
    {
        [Serializable, System.Diagnostics.DebuggerDisplay("Weight({weightXYZ}, {weight_XYZ}, {weightX_YZ}, {weightXY_Z}, {weight_X_YZ}, {weight_XY_Z}, {weightX_Y_Z}, {weight_X_Y_Z})")]
        public class VoxelWeight
        {
            public float weightXYZ;
            public float weight_XYZ;
            public float weightX_YZ;
            public float weightXY_Z;
            public float weight_X_YZ;
            public float weight_XY_Z;
            public float weightX_Y_Z;
            public float weight_X_Y_Z;
            
            public bool HasValue()
            {
                return weightXYZ != 0f || weight_XYZ != 0f || weightX_YZ != 0f || weightXY_Z != 0f || weight_X_YZ != 0f || weight_XY_Z != 0f || weightX_Y_Z != 0f || weight_X_Y_Z != 0f;
            }

            public void SetWeight(float weight)
            {
                weightXYZ = weight;
                weight_XYZ = weight;
                weightX_YZ = weight;
                weightXY_Z = weight;
                weight_X_YZ = weight;
                weight_XY_Z = weight;
                weightX_Y_Z = weight;
                weight_X_Y_Z = weight;
            }

            public void SetWeight(VoxelSkinnedAnimationObject.VoxelVertexIndex index, float weight)
            {
                switch (index)
                {
                case VoxelBase.VoxelVertexIndex.XYZ: weightXYZ = weight; break;
                case VoxelBase.VoxelVertexIndex._XYZ: weight_XYZ = weight; break;
                case VoxelBase.VoxelVertexIndex.X_YZ: weightX_YZ = weight; break;
                case VoxelBase.VoxelVertexIndex.XY_Z: weightXY_Z = weight; break;
                case VoxelBase.VoxelVertexIndex._X_YZ: weight_X_YZ = weight; break;
                case VoxelBase.VoxelVertexIndex._XY_Z: weight_XY_Z = weight; break;
                case VoxelBase.VoxelVertexIndex.X_Y_Z: weightX_Y_Z = weight; break;
                case VoxelBase.VoxelVertexIndex._X_Y_Z: weight_X_Y_Z = weight; break;
                default: Assert.IsTrue(false); break;
                }
            }
            public float GetWeight(VoxelSkinnedAnimationObject.VoxelVertexIndex index)
            {
                switch (index)
                {
                case VoxelBase.VoxelVertexIndex.XYZ: return weightXYZ;
                case VoxelBase.VoxelVertexIndex._XYZ: return weight_XYZ;
                case VoxelBase.VoxelVertexIndex.X_YZ: return weightX_YZ;
                case VoxelBase.VoxelVertexIndex.XY_Z: return weightXY_Z;
                case VoxelBase.VoxelVertexIndex._X_YZ: return weight_X_YZ;
                case VoxelBase.VoxelVertexIndex._XY_Z: return weight_XY_Z;
                case VoxelBase.VoxelVertexIndex.X_Y_Z: return weightX_Y_Z;
                case VoxelBase.VoxelVertexIndex._X_Y_Z: return weight_X_Y_Z;
                default: Assert.IsTrue(false); return 0f;
                }
            }
        }

        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            #region repair
            if (weightKeys.Count < weightValues.Count)
            {
                weightValues.RemoveRange(weightKeys.Count, weightValues.Count - weightKeys.Count);
            }
            else if(weightKeys.Count > weightValues.Count)
            {
                weightKeys.RemoveRange(weightValues.Count, weightKeys.Count - weightValues.Count);
            }
            #endregion
            {
                IntVector3 max = IntVector3.zero;
                for (int i = 0; i < weightKeys.Count; i++)
                {
                    max = IntVector3.Max(max, weightKeys[i]);
                }
                indexTable = new DataTable3<int>(max.x, max.y, max.z);
            }
            for (int i = 0; i < weightKeys.Count; i++)
            {
                if (indexTable.Contains(weightKeys[i]))
                {
                    weightKeys.RemoveAt(i);
                    weightValues.RemoveAt(i);
                    i--;
                    continue;
                }
                indexTable.Set(weightKeys[i], i);
            }
        }

        public void SetWeight(IntVector3 pos, VoxelWeight weight)
        {
            if(!indexTable.Contains(pos))
            {
                indexTable.Set(pos, weightKeys.Count);
                weightKeys.Add(pos);
                weightValues.Add(weight);
            }
            else
            {
                var index = indexTable.Get(pos);
                Assert.IsTrue(weightKeys[index] == pos);
                weightValues[index] = weight;
            }
        }
        public void RemoveWeight(IntVector3 pos)
        {
            if (indexTable.Contains(pos))
            {
                var index = indexTable.Get(pos);
                if(index < weightKeys.Count - 1)
                {
                    weightKeys[index] = weightKeys[weightKeys.Count - 1];
                    weightValues[index] = weightValues[weightValues.Count - 1];
                    indexTable.Set(weightKeys[weightKeys.Count - 1], index);
                    weightKeys.RemoveAt(weightKeys.Count - 1);
                    weightValues.RemoveAt(weightValues.Count - 1);
                }
                else
                {
                    weightKeys.RemoveAt(index);
                    weightValues.RemoveAt(index);
                }
                indexTable.Remove(pos);
            }
        }
        public VoxelWeight GetWeight(IntVector3 pos)
        {
            if (!indexTable.Contains(pos)) return null;
            var index = indexTable.Get(pos);
            Assert.IsTrue(weightKeys[index] == pos);
            return weightValues[index];
        }
        public void ClearWeight()
        {
            indexTable.Clear();
            weightKeys.Clear();
            weightValues.Clear();
        }
        public void AllAction(Action<IntVector3, VoxelWeight> action)
        {
            for (int i = 0; i < weightKeys.Count; i++)
            {
                if (weightValues[i] != null)
                {
                    action(weightKeys[i], weightValues[i]);
                }
            }
        }

        private DataTable3<int> indexTable = new DataTable3<int>();

        [SerializeField]
        private List<IntVector3> weightKeys = new List<IntVector3>();
        [SerializeField]
        private List<VoxelWeight> weightValues = new List<VoxelWeight>();
    }
}

#endif
