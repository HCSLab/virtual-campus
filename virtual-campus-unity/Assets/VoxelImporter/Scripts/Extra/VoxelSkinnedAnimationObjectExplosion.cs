using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [AddComponentMenu("Voxel Importer/Extra/Explosion/Voxel Skinned Animation Object Explosion")]
    [RequireComponent(typeof(VoxelSkinnedAnimationObject))]
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class VoxelSkinnedAnimationObjectExplosion : VoxelBaseExplosion
    {
        protected VoxelSkinnedAnimationObject voxelObject { get; private set; }
        protected SkinnedMeshRenderer skinnedMeshRendererCache { get; private set; }

        [Serializable]
        public class SkinnedAnimationMeshData : MeshData
        {
            public Mesh bakeMesh;
        }

        public List<SkinnedAnimationMeshData> meshes;
        public List<Material> materials;
        
        protected override void Awake()
        {
            base.Awake();

            voxelObject = GetComponent<VoxelSkinnedAnimationObject>();
            skinnedMeshRendererCache = GetComponent<SkinnedMeshRenderer>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void BakeExplosionPlay(float lifeTime, Action doneAction = null)
        {
            BakeMesh();
            base.ExplosionPlay(lifeTime, doneAction);
        }

        public override void DrawMesh()
        {
            if (materials != null && meshes != null)
            {
                var world = transformCache.localToWorldMatrix;
                for (int i = 0; i < meshes.Count; i++)
                {
                    for (int j = 0; j < meshes[i].materialIndexes.Count; j++)
                    {
                        if (meshes[i].bakeMesh != null)
                        {
                            if (j < meshes[i].bakeMesh.subMeshCount)
                                Graphics.DrawMesh(meshes[i].bakeMesh, world, materials[meshes[i].materialIndexes[j]], 0, null, j, materialPropertyBlock);
                        }
                        else
                        {
                            if (j < meshes[i].mesh.subMeshCount)
                                Graphics.DrawMesh(meshes[i].mesh, world, materials[meshes[i].materialIndexes[j]], 0, null, j, materialPropertyBlock);
                        }
                    }
                }
            }
        }
      
        public void BakeMesh()
        {
            if (meshes == null || skinnedMeshRendererCache == null) return;

            var saveMesh = skinnedMeshRendererCache.sharedMesh;
            var localPosition = transformCache.localPosition;
            var localRotation = transformCache.localRotation;
            var localScale = transformCache.localScale;
            transformCache.localScale = Vector3.one;

            for (int i = 0; i < meshes.Count; i++)
            {
                if (meshes[i].mesh != null)
                {
                    skinnedMeshRendererCache.sharedMesh = meshes[i].mesh;
                    if (meshes[i].bakeMesh == null)
                    {
                        meshes[i].bakeMesh = new Mesh();
                    }
                    else
                    {
#if UNITY_EDITOR
                        if (!UnityEditor.EditorApplication.isPlaying)
                        {
                            meshes[i].bakeMesh.Clear(false);
                            meshes[i].bakeMesh.ClearBlendShapes();
                        }
                        else
                        {
                            meshes[i].bakeMesh = new Mesh();
                        }
#else
                        meshes[i].bakeMesh = new Mesh();
#endif
                    }
                    skinnedMeshRendererCache.BakeMesh(meshes[i].bakeMesh);
                    {
                        var bounds = meshes[i].bakeMesh.bounds;
                        bounds.size = meshes[i].mesh.bounds.size;
                        meshes[i].bakeMesh.bounds = bounds;
                    }
                }
                else
                {
                    meshes[i].bakeMesh = null;
                }
            }

            skinnedMeshRendererCache.sharedMesh = saveMesh;
            transformCache.localPosition = localPosition;
            transformCache.localRotation = localRotation;
            transformCache.localScale = localScale;
        }

#if UNITY_EDITOR
        public bool edit_bakeFoldout = true;

        #region Asset
        public override bool IsUseAssetObject(UnityEngine.Object obj)
        {
            if (meshes != null)
            {
                for (int i = 0; i < meshes.Count; i++)
                {
                    if (meshes[i] == null) continue;
                    if (meshes[i].mesh == obj) return true;
                    if (meshes[i].bakeMesh == obj) return true;
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

