using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace VoxelImporter
{
    [Serializable]
    public class DisableData : ISerializationCallbackReceiver
    {
        public void OnBeforeSerialize()
        {
        }
        public void OnAfterDeserialize()
        {
            IntVector3 max = IntVector3.zero;
            for (int i = 0; i < disableList.Count; i++)
            {
                max = IntVector3.Max(max, disableList[i]);
            }
            indexTable = new DataTable3<VoxelBase.Face>(max.x + 1, max.y + 1, max.z + 1);

            for (int i = 0; i < disableList.Count; i++)
            {
                indexTable.Set(disableList[i], faceList[i]);
            }
        }
        
        public DisableData Clone()
        {
            DisableData clone = new DisableData();
            clone.disableList = new List<IntVector3>(disableList);
            clone.faceList = new List<VoxelBase.Face>(faceList);
            clone.OnAfterDeserialize();
            return clone;
        }

        public bool IsEqual(DisableData src)
        {
            if (disableList != null && src.disableList != null)
            {
                if (disableList.Count != src.disableList.Count) return false;
                for (int i = 0; i < disableList.Count; i++)
                {
                    if (disableList[i] != src.disableList[i]) return false;
                }
            }
            else if (disableList != null && src.disableList == null)
            {
                return false;
            }
            else if (disableList == null && src.disableList != null)
            {
                return false;
            }

            if (faceList != null && src.faceList != null)
            {
                if (faceList.Count != src.faceList.Count) return false;
                for (int i = 0; i < faceList.Count; i++)
                {
                    if (faceList[i] != src.faceList[i]) return false;
                }
            }
            else if (faceList != null && src.faceList == null)
            {
                return false;
            }
            else if (faceList == null && src.faceList != null)
            {
                return false;
            }

            return true;
        }

        public void SetDisable(IntVector3 pos, VoxelBase.Face face)
        {
            if (!indexTable.Contains(pos))
            {
                indexTable.Set(pos, face);
                disableList.Add(pos);
                faceList.Add(face);
            }
            else
            {
                indexTable.Set(pos, face);
                int index = disableList.IndexOf(pos);
                faceList[index] = face;
            }
        }
        public void RemoveDisable(IntVector3 pos)
        {
            if (indexTable.Contains(pos))
            {
                indexTable.Remove(pos);
                var index = disableList.IndexOf(pos);
                disableList.RemoveAt(index);
                faceList.RemoveAt(index);
            }
        }
        public VoxelBase.Face GetDisable(IntVector3 pos)
        {
            return indexTable.Get(pos);
        }
        public void ClearDisable()
        {
            indexTable.Clear();
            disableList.Clear();
            faceList.Clear();
        }
        public int Count
        {
            get { return disableList.Count; }
        }

        public void AllAction(Action<IntVector3, VoxelBase.Face> action)
        {
            for (int i = 0; i < disableList.Count; i++)
            {
                action(disableList[i], faceList[i]);
            }
        }
        private DataTable3<VoxelBase.Face> indexTable = new DataTable3<VoxelBase.Face>();

        [SerializeField]
        private List<IntVector3> disableList = new List<IntVector3>();
        [SerializeField]
        private List<VoxelBase.Face> faceList = new List<VoxelBase.Face>();
    }
}

#endif
