using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
    public class VoxelChunksObjectChunk : MonoBehaviour
    {
#if !UNITY_EDITOR
        void Awake()
        {
            Destroy(this);
        }
#else
        public bool EditorInitialize()
        {
            bool result = false;

            //ver1.021 -> ver1.0.3
            if (material != null)
            {
                materials = new List<Material>();
                materials.Add(material);
                materialIndexes = new List<int>();
                materialIndexes.Add(0);
                material = null;
                result = true;
            }
            //ver1.0.4
            if(basicOffset.sqrMagnitude <= 0f)
            {
                basicOffset = transform.localPosition;
            }

            return result;
        }

        public IntVector3 position;
        public string chunkName;
        public Vector3 basicOffset;

        public Mesh mesh;
        [SerializeField]
        protected Material material;        //ver1.021 old
        public List<Material> materials;    //ver1.0.3 new
        public Texture2D atlasTexture;
        public List<int> materialIndexes;

        #region Editor
        public bool edit_objectFoldout = true;
        #endregion
#endif
    }
}
