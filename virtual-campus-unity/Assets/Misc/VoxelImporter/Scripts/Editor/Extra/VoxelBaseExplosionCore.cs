using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelImporter
{
    public abstract class VoxelBaseExplosionCore
    {
        public VoxelBaseExplosion explosionBase { get; protected set; }

        public VoxelBase voxelBase { get; protected set; }
        public VoxelBaseCore voxelBaseCore { get; protected set; }

        public VoxelBaseExplosionCore(VoxelBaseExplosion target)
        {
            explosionBase = target;
            voxelBase = target.GetComponent<VoxelBase>();
        }

        public void Generate()
        {
            if (explosionBase == null || voxelBaseCore.voxelData == null) return;

            voxelBaseCore.DestroyUnusedObjectInPrefabObject();

            GenerateOnly();

            SetMaterialProperties();

            voxelBaseCore.CheckPrefabAssetReImport();

            explosionBase.edit_fileRefreshLastTimeTicks = voxelBase.fileRefreshLastTimeTicks;
        }
        public abstract void GenerateOnly();

        protected void CreateBasicCube(out Vector3 cubeCenter, out List<Vector3> cubeVertices, out List<Vector3> cubeNormals, out List<int> cubeTriangles)
        {
            cubeVertices = new List<Vector3>();
            cubeNormals = new List<Vector3>();
            cubeTriangles = new List<int>();
            {
                var offsetPosition = voxelBase.localOffset + voxelBase.importOffset;
                cubeCenter = Vector3.Scale(voxelBase.importScale, offsetPosition) + voxelBase.importScale / 2f;
                #region forward
                {
                    var pOffset = Vector3.Scale(voxelBase.importScale, offsetPosition);
                    var vOffset = cubeVertices.Count;
                    cubeVertices.Add(new Vector3(0, voxelBase.importScale.y, voxelBase.importScale.z) + pOffset);
                    cubeVertices.Add(new Vector3(0, 0, voxelBase.importScale.z) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, 0, voxelBase.importScale.z) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, voxelBase.importScale.y, voxelBase.importScale.z) + pOffset);
                    cubeTriangles.Add(vOffset + 0); cubeTriangles.Add(vOffset + 1); cubeTriangles.Add(vOffset + 2);
                    cubeTriangles.Add(vOffset + 0); cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 3);
                    for (int j = 0; j < 4; j++)
                    {
                        cubeNormals.Add(Vector3.forward);
                    }
                }
                #endregion
                #region up
                {
                    var pOffset = Vector3.Scale(voxelBase.importScale, offsetPosition);
                    var vOffset = cubeVertices.Count;
                    cubeVertices.Add(new Vector3(0, voxelBase.importScale.y, 0) + pOffset);
                    cubeVertices.Add(new Vector3(0, voxelBase.importScale.y, voxelBase.importScale.z) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, voxelBase.importScale.y, voxelBase.importScale.z) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, voxelBase.importScale.y, 0) + pOffset);
                    cubeTriangles.Add(vOffset + 0); cubeTriangles.Add(vOffset + 1); cubeTriangles.Add(vOffset + 2);
                    cubeTriangles.Add(vOffset + 0); cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 3);
                    for (int j = 0; j < 4; j++)
                    {
                        cubeNormals.Add(Vector3.up);
                    }
                }
                #endregion
                #region right
                {
                    var pOffset = Vector3.Scale(voxelBase.importScale, offsetPosition);
                    var vOffset = cubeVertices.Count;
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, 0, 0) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, voxelBase.importScale.y, 0) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, voxelBase.importScale.y, voxelBase.importScale.z) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, 0, voxelBase.importScale.z) + pOffset);
                    cubeTriangles.Add(vOffset + 0); cubeTriangles.Add(vOffset + 1); cubeTriangles.Add(vOffset + 2);
                    cubeTriangles.Add(vOffset + 0); cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 3);
                    for (int j = 0; j < 4; j++)
                    {
                        cubeNormals.Add(Vector3.right);
                    }
                }
                #endregion
                #region left
                {
                    var pOffset = Vector3.Scale(voxelBase.importScale, offsetPosition);
                    var vOffset = cubeVertices.Count;
                    cubeVertices.Add(new Vector3(0, 0, voxelBase.importScale.z) + pOffset);
                    cubeVertices.Add(new Vector3(0, 0, 0) + pOffset);
                    cubeVertices.Add(new Vector3(0, voxelBase.importScale.y, 0) + pOffset);
                    cubeVertices.Add(new Vector3(0, voxelBase.importScale.y, voxelBase.importScale.z) + pOffset);
                    cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 1); cubeTriangles.Add(vOffset + 0);
                    cubeTriangles.Add(vOffset + 3); cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 0);
                    for (int j = 0; j < 4; j++)
                    {
                        cubeNormals.Add(Vector3.left);
                    }
                }
                #endregion
                #region down
                {
                    var pOffset = Vector3.Scale(voxelBase.importScale, offsetPosition);
                    var vOffset = cubeVertices.Count;
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, 0, 0) + pOffset);
                    cubeVertices.Add(new Vector3(0, 0, 0) + pOffset);
                    cubeVertices.Add(new Vector3(0, 0, voxelBase.importScale.z) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, 0, voxelBase.importScale.z) + pOffset);
                    cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 1); cubeTriangles.Add(vOffset + 0);
                    cubeTriangles.Add(vOffset + 3); cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 0);
                    for (int j = 0; j < 4; j++)
                    {
                        cubeNormals.Add(Vector3.down);
                    }
                }
                #endregion
                #region back
                {
                    var pOffset = Vector3.Scale(voxelBase.importScale, offsetPosition);
                    var vOffset = cubeVertices.Count;
                    cubeVertices.Add(new Vector3(0, 0, 0) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, 0, 0) + pOffset);
                    cubeVertices.Add(new Vector3(voxelBase.importScale.x, voxelBase.importScale.y, 0) + pOffset);
                    cubeVertices.Add(new Vector3(0, voxelBase.importScale.y, 0) + pOffset);
                    cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 1); cubeTriangles.Add(vOffset + 0);
                    cubeTriangles.Add(vOffset + 3); cubeTriangles.Add(vOffset + 2); cubeTriangles.Add(vOffset + 0);
                    for (int j = 0; j < 4; j++)
                    {
                        cubeNormals.Add(Vector3.back);
                    }
                }
                #endregion
            }
        }

        protected Shader GetStandardShader(bool transparent)
        {
#if UNITY_2018_1_OR_NEWER
            if (EditorCommon.IsLightweightRenderPipeline())
            {
				//ToDo
            }
            else if (EditorCommon.IsHighDefinitionRenderPipeline())
            {
				//ToDo
            }
#endif
            if (!transparent)
                return Shader.Find("Voxel Importer/Explosion/VoxelExplosion-Opaque");
            else
                return Shader.Find("Voxel Importer/Explosion/VoxelExplosion-Transparent");
        }

        public abstract void SetExplosionCenter();

        public abstract void CopyMaterialProperties();
        public void SetMaterialProperties()
        {
            CopyMaterialProperties();
            SetExplosionCenter();
            explosionBase.SetExplosionRotate(explosionBase.explosionRotate);
            explosionBase.SetExplosionRate(explosionBase.edit_explosionRate);
        }

        public abstract void ResetAllAssets();

        #region StaticForceGenerate
        public static void StaticForceGenerate(VoxelBaseExplosion voxelBase)
        {
            VoxelBaseExplosionCore voxelCore = null;
            if (voxelBase is VoxelObjectExplosion)
            {
                voxelCore = new VoxelObjectExplosionCore(voxelBase);
            }
            else if (voxelBase is VoxelChunksObjectExplosion)
            {
                voxelCore = new VoxelChunksObjectExplosionCore(voxelBase);
            }
            else if (voxelBase is VoxelFrameAnimationObjectExplosion)
            {
                voxelCore = new VoxelFrameAnimationObjectExplosionCore(voxelBase);
            }
            else if (voxelBase is VoxelSkinnedAnimationObjectExplosion)
            {
                voxelCore = new VoxelSkinnedAnimationObjectExplosionCore(voxelBase);
            }
            else
            {
                Assert.IsTrue(false);
            }
            if (voxelCore == null) return;

            voxelCore.ResetAllAssets();
            voxelCore.Generate();

            #region Extra
            if (voxelCore is VoxelSkinnedAnimationObjectExplosionCore)
            {
                var voxelCoreEx = voxelCore as VoxelSkinnedAnimationObjectExplosionCore;
                if (voxelCoreEx.explosionObject.meshes != null &&
                    voxelCoreEx.explosionObject.meshes.Count > 0 &&
                    voxelCoreEx.explosionObject.meshes[0].bakeMesh != null)
                {
                    voxelCoreEx.Bake();
                }
            }
            #endregion
        }
        #endregion
    }
}
