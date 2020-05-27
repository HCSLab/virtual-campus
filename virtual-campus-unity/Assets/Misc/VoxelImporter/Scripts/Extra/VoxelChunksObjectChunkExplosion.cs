using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [RequireComponent(typeof(VoxelChunksObjectChunk))]
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class VoxelChunksObjectChunkExplosion : MonoBehaviour
    {
        protected VoxelChunksObjectExplosion explosionObject { get; private set; }

        protected Transform transformCache { get; private set; }
        protected Transform parentCache { get; private set; }
        protected Renderer rendererCache { get; private set; }

        public List<VoxelBaseExplosion.MeshData> meshes;
        public List<Material> materials;
        public Vector3 chunkBasicOffset;

        void Awake()
        {
            if (transform.parent == null) return;
            explosionObject = transform.parent.GetComponent<VoxelChunksObjectExplosion>();
            if (explosionObject == null) return;
            transformCache = transform;
            parentCache = explosionObject.transform;
            rendererCache = GetComponent<Renderer>();

            explosionObject.SetEnableExplosionObject(false);
        }

        public void DrawMesh()
        {
            if (explosionObject == null || meshes == null) return;

            if (explosionObject.materialMode == VoxelChunksObject.MaterialMode.Combine && explosionObject.materials != null)
            {
                var world = parentCache.localToWorldMatrix;
                for (int i = 0; i < meshes.Count; i++)
                {
                    if (meshes[i].mesh == null) continue;
                    var local = Matrix4x4.TRS(transformCache.localPosition, transformCache.localRotation, transformCache.localScale);
                    var basic = Matrix4x4.TRS(chunkBasicOffset, Quaternion.identity, Vector3.one);
                    var offset = local * basic.inverse;
                    for (int j = 0; j < meshes[i].materialIndexes.Count; j++)
                    {
                        var matIndex = meshes[i].materialIndexes[j];
                        if (matIndex < explosionObject.materials.Count)
                        {
                            if (j < meshes[i].mesh.subMeshCount)
                                Graphics.DrawMesh(meshes[i].mesh, world * offset, explosionObject.materials[matIndex], 0, null, j, explosionObject.materialPropertyBlock);
                        }
                    }
                }
            }
            else if (explosionObject.materialMode == VoxelChunksObject.MaterialMode.Individual && materials != null)
            {
                var world = transformCache.localToWorldMatrix;
                for (int i = 0; i < meshes.Count; i++)
                {
                    if (meshes[i].mesh == null) continue;
                    var basic = Matrix4x4.TRS(chunkBasicOffset, Quaternion.identity, Vector3.one);
                    for (int j = 0; j < meshes[i].materialIndexes.Count; j++)
                    {
                        var matIndex = meshes[i].materialIndexes[j];
                        if (matIndex < materials.Count)
                        {
                            if (j < meshes[i].mesh.subMeshCount)
                                Graphics.DrawMesh(meshes[i].mesh, world * basic.inverse, materials[matIndex], 0, null, j, explosionObject.materialPropertyBlock);
                        }
                    }
                }
            }
        }

        public void SetEnableRenderer(bool enable)
        {
            if (rendererCache != null)
            {
                if (rendererCache != null && rendererCache.enabled != enable)
                    rendererCache.enabled = enable;
            }
        }

        #region Editor
        public bool edit_objectFoldout = true;
        #endregion
    }
}
