using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace VoxelImporter
{
    [Serializable]
    public class MaterialData : ISerializationCallbackReceiver
    {
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            IntVector3 max = IntVector3.zero;
            for (int i = 0; i < materialList.Count; i++)
            {
                max = IntVector3.Max(max, materialList[i]);
            }
            indexTable = new DataTable3<int>(max.x + 1, max.y + 1, max.z + 1);

            for (int i = 0; i < materialList.Count; i++)
            {
                indexTable.Set(materialList[i], i);
            }
        }
        
        public MaterialData Clone()
        {
            MaterialData clone = new MaterialData();
            clone.materialList = new List<IntVector3>(materialList);
            clone.name = name;
            clone.transparent = transparent;
            clone.OnAfterDeserialize();
            return clone;
        }

        public bool IsEqual(MaterialData src)
        {
            if(materialList != null && src.materialList != null)
            {
                if (materialList.Count != src.materialList.Count) return false;
                for (int i = 0; i < materialList.Count; i++)
                {
                    if (materialList[i] != src.materialList[i]) return false;
                }
            }
            else if(materialList != null && src.materialList == null)
            {
                return false;
            }
            else if (materialList == null && src.materialList != null)
            {
                return false;
            }
            if (name != src.name) return false;
            if (transparent != src.transparent) return false;

            return true;
        }

        public void SetMaterial(IntVector3 pos)
        {
            if (!indexTable.Contains(pos))
            {
                indexTable.Set(pos, materialList.Count);
                materialList.Add(pos);
            }
            else
            {
                var index = indexTable.Get(pos);
                Assert.IsTrue(materialList[index] == pos);
            }
        }
        public void RemoveMaterial(IntVector3 pos)
        {
            if (indexTable.Contains(pos))
            {
                var index = indexTable.Get(pos);
                if (index < materialList.Count - 1)
                {
                    materialList[index] = materialList[materialList.Count - 1];
                    indexTable.Set(materialList[materialList.Count - 1], index);
                    materialList.RemoveAt(materialList.Count - 1);
                }
                else
                {
                    materialList.RemoveAt(index);
                }
                indexTable.Remove(pos);
            }
        }
        public bool GetMaterial(IntVector3 pos)
        {
            return indexTable.Contains(pos);
        }
        public void ClearMaterial()
        {
            indexTable.Clear();
            materialList.Clear();
        }
        public int Count
        {
            get { return materialList.Count; }
        }

        public void AllAction(Action<IntVector3> action)
        {
            for (int i = 0; i < materialList.Count; i++)
            {
                action(materialList[i]);
            }
        }
        private DataTable3<int> indexTable = new DataTable3<int>();

        [SerializeField]
        private List<IntVector3> materialList = new List<IntVector3>();

        public string name;
        public bool transparent;

        #region FromVoxMaterial
        [Serializable]
        public struct Material
        {
            public enum StandardShaderRenderingMode
            {
                Opaque,
                Cutout,
                Fade,
                Transparent,
            }
            public StandardShaderRenderingMode renderingMode;
            public float metallic;
            public float smoothness;
            public float alpha;
            public Color emission;
            public float emissionPower;
        }
        public Material material;
        #endregion
    }
}

#endif
