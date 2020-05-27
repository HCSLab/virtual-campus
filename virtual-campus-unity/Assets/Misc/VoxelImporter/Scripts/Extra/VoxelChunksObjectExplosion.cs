using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [AddComponentMenu("Voxel Importer/Extra/Explosion/Voxel Chunks Object Explosion")]
    [RequireComponent(typeof(VoxelChunksObject))]
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class VoxelChunksObjectExplosion : VoxelBaseExplosion
    {
#if UNITY_EDITOR
        protected VoxelChunksObject voxelObject { get; private set; }
#endif

        public VoxelChunksObjectChunkExplosion[] chunksExplosion;

        public List<Material> materials;
        public VoxelChunksObject.MaterialMode materialMode;

        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
            voxelObject = GetComponent<VoxelChunksObject>();
            if (voxelObject != null)
            {
                UpdatedChunks();
                voxelObject.updatedChunks += UpdatedChunks;
            }
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

#if UNITY_EDITOR
            if (voxelObject != null)
            {
                voxelObject.updatedChunks -= UpdatedChunks;
            }
#endif
        }
#if UNITY_EDITOR
        private void UpdatedChunks()
        {
            if (voxelObject == null || voxelObject.chunks == null) return;
            chunksExplosion = new VoxelChunksObjectChunkExplosion[voxelObject.chunks.Length];
            for (int i = 0; i < voxelObject.chunks.Length; i++)
            {
                if (voxelObject.chunks[i] == null) continue;
                chunksExplosion[i] = voxelObject.chunks[i].GetComponent<VoxelChunksObjectChunkExplosion>();
            }
        }
#endif

        public override void DrawMesh()
        {
            if (chunksExplosion != null)
            {
                for (int i = 0; i < chunksExplosion.Length; i++)
                {
                    if (chunksExplosion[i] == null) continue;
                    chunksExplosion[i].DrawMesh();
                }
            }
        }

        public override void SetEnableExplosionObject(bool enable)
        {
            enabled = enable;
            if (chunksExplosion != null)
            {
                for (int i = 0; i < chunksExplosion.Length; i++)
                {
                    if (chunksExplosion[i] == null) continue;
                    chunksExplosion[i].enabled = enable;
                }
            }
        }
        public override void SetEnableRenderer(bool enable)
        {
            if (chunksExplosion != null)
            {
                for (int i = 0; i < chunksExplosion.Length; i++)
                {
                    if (chunksExplosion[i] == null) continue;
                    chunksExplosion[i].SetEnableRenderer(enable);
                }
            }
        }

#if UNITY_EDITOR
        #region Asset
        public override bool IsUseAssetObject(UnityEngine.Object obj)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i] == obj) return true;
                }
            }
            if (chunksExplosion != null)
            {
                for (int i = 0; i < chunksExplosion.Length; i++)
                {
                    if (chunksExplosion[i] == null || chunksExplosion[i].meshes == null) continue;
                    for (int j = 0; j < chunksExplosion[i].meshes.Count; j++)
                    {
                        if (chunksExplosion[i].meshes[j] == null) continue;
                        if (chunksExplosion[i].meshes[j].mesh == obj) return true;
                    }
                }
            }

            return false;
        }
        #endregion
#endif
    }
}
