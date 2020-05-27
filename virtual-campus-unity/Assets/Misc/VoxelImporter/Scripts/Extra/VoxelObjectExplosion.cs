using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [AddComponentMenu("Voxel Importer/Extra/Explosion/Voxel Object Explosion")]
    [RequireComponent(typeof(VoxelObject))]
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class VoxelObjectExplosion : VoxelBaseExplosion
    {
        protected VoxelObject voxelObject { get; private set; }

        public List<MeshData> meshes;
        public List<Material> materials;

        protected override void Awake()
        {
            base.Awake();

            voxelObject = GetComponent<VoxelObject>();
        }

        public override void DrawMesh()
        {
            if (materials != null && meshes != null)
            {
                var world = transformCache.localToWorldMatrix;
                for (int i = 0; i < meshes.Count; i++)
                {
                    if (meshes[i] == null || meshes[i].mesh == null) continue;
                    for (int j = 0; j < meshes[i].materialIndexes.Count; j++)
                    {
                        if (j < meshes[i].mesh.subMeshCount)
                            Graphics.DrawMesh(meshes[i].mesh, world, materials[meshes[i].materialIndexes[j]], 0, null, j, materialPropertyBlock);
                    }
                }
            }
        }

#if UNITY_EDITOR
        #region Asset
        public override bool IsUseAssetObject(UnityEngine.Object obj)
        {
            if (meshes != null)
            {
                for (int i = 0; i < meshes.Count; i++)
                {
                    if (meshes[i].mesh == obj) return true;
                }
            }
            if (materials != null)
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i] == obj) return true;
                }
            }

            return false;
        }
        #endregion
#endif
    }
}

