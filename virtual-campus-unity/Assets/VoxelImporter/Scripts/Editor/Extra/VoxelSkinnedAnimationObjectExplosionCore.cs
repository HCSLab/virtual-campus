using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelImporter
{
    public class VoxelSkinnedAnimationObjectExplosionCore : VoxelBaseExplosionCore
    {
        public VoxelSkinnedAnimationObjectExplosion explosionObject { get; protected set; }

        public VoxelSkinnedAnimationObject voxelObject { get; protected set; }
        public VoxelSkinnedAnimationObjectCore voxelObjectCore { get; protected set; }

        public VoxelSkinnedAnimationObjectExplosionCore(VoxelBaseExplosion target) : base(target)
        {
            explosionObject = target as VoxelSkinnedAnimationObjectExplosion;

            voxelBase = voxelObject = target.GetComponent<VoxelSkinnedAnimationObject>();
            voxelBaseCore = voxelObjectCore = new VoxelSkinnedAnimationObjectCore(voxelObject);

            voxelBaseCore.Initialize();
        }

        public override void GenerateOnly()
        {
            if (voxelObject == null || voxelObjectCore.voxelData == null) return;

            var voxelData = voxelObjectCore.voxelData;

            //BasicCube
            Vector3 cubeCenter;
            List<Vector3> cubeVertices;
            List<Vector3> cubeNormals;
            List<int> cubeTriangles;
            CreateBasicCube(out cubeCenter, out cubeVertices, out cubeNormals, out cubeTriangles);

            #region Voxels
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<BoneWeight> boneWeights = new List<BoneWeight>();
            List<Vector4> tangents = new List<Vector4>();
            List<int>[] triangles = new List<int>[voxelBase.materialData.Count];
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = new List<int>();
            }

            #region Mesh
            Func<VoxelSkinnedAnimationObjectExplosion.SkinnedAnimationMeshData, VoxelSkinnedAnimationObjectExplosion.SkinnedAnimationMeshData> CreateMesh = (data) =>
            {
                if (data == null)
                    data = new VoxelSkinnedAnimationObjectExplosion.SkinnedAnimationMeshData();
                if (data.mesh == null)
                {
                    data.mesh = new Mesh();
                }
                else
                {
                    data.mesh.Clear(false);
                    data.mesh.ClearBlendShapes();
                }
                data.materialIndexes.Clear();
#if UNITY_2017_3_OR_NEWER
                data.mesh.indexFormat = vertices.Count > 65000 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
#endif
                data.mesh.vertices = vertices.ToArray();
                data.mesh.normals = normals.ToArray();
                data.mesh.colors = colors.ToArray();
                data.mesh.tangents = tangents.ToArray();
                data.mesh.boneWeights = boneWeights.ToArray();
                data.mesh.bindposes = voxelObjectCore.GetBindposes();
                {
                    int materialCount = 0;
                    for (int i = 0; i < triangles.Length; i++)
                    {
                        if (triangles[i].Count > 0)
                            materialCount++;
                    }
                    data.mesh.subMeshCount = materialCount;
                    int submesh = 0;
                    for (int i = 0; i < triangles.Length; i++)
                    {
                        if (triangles[i].Count > 0)
                        {
                            data.materialIndexes.Add(i);
                            data.mesh.SetTriangles(triangles[i].ToArray(), submesh++);
                        }
                    }
                }
                data.mesh.RecalculateBounds();
                {
                    var bounds = data.mesh.bounds;
                    bounds.min -= Vector3.one * explosionBase.edit_velocityMax;
                    bounds.max += Vector3.one * explosionBase.edit_velocityMax;
                    data.mesh.bounds = bounds;
                }
                vertices.Clear();
                normals.Clear();
                colors.Clear();
                tangents.Clear();
                boneWeights.Clear();
                for (int i = 0; i < voxelBase.materialData.Count; i++)
                {
                    triangles[i].Clear();
                }
                return data;
            };
            #endregion

            {
                int meshIndex = 0;
                Action<int, int> AddVertex = (mat, index) =>
                {
                    if (explosionBase.edit_birthRate < 1f)
                    {
                        if (UnityEngine.Random.value >= explosionBase.edit_birthRate)
                            return;
                    }
                    if (explosionBase.edit_visibleOnly)
                    {
                        if (!voxelObjectCore.IsVoxelVisible(voxelData.voxels[index].position))
                            return;
                    }

#if !UNITY_2017_3_OR_NEWER
                    if (vertices.Count + cubeVertices.Count >= 65000)
                    {
                        for (int i = explosionObject.meshes.Count; i <= meshIndex; i++)
                            explosionObject.meshes.Add(null);
                        explosionObject.meshes[meshIndex] = CreateMesh(explosionObject.meshes[meshIndex]);
                        if (!AssetDatabase.Contains(explosionObject.meshes[meshIndex].mesh))
                        {
                            voxelBaseCore.AddObjectToPrefabAsset(explosionObject.meshes[meshIndex].mesh, "explosion_mesh", meshIndex);
                        }
                        meshIndex++;
                    }
#endif

                    var color = voxelData.palettes[voxelData.voxels[index].palette];
                    var vOffset = vertices.Count;
                    for (int i = 0; i < cubeVertices.Count; i++)
                    {
                        var pos = cubeVertices[i];
                        pos.x += voxelData.voxels[index].position.x * voxelBase.importScale.x;
                        pos.y += voxelData.voxels[index].position.y * voxelBase.importScale.y;
                        pos.z += voxelData.voxels[index].position.z * voxelBase.importScale.z;
                        vertices.Add(pos);
                    }
                    normals.AddRange(cubeNormals);
                    for (int j = 0; j < cubeTriangles.Count; j++)
                        triangles[mat].Add(vOffset + cubeTriangles[j]);
                    for (int j = 0; j < cubeVertices.Count; j++)
                    {
                        colors.Add(color);
                    }
                    {
                        Vector3 center = new Vector3
                        (
                            center.x = cubeCenter.x + voxelData.voxels[index].position.x * voxelBase.importScale.x,
                            center.y = cubeCenter.y + voxelData.voxels[index].position.y * voxelBase.importScale.y,
                            center.z = cubeCenter.z + voxelData.voxels[index].position.z * voxelBase.importScale.z
                        );
                        var velocity = UnityEngine.Random.Range(explosionBase.edit_velocityMin, explosionBase.edit_velocityMax);
                        for (int j = 0; j < cubeVertices.Count; j++)
                        {
                            tangents.Add(new Vector4(center.x - vertices[vOffset + j].x, center.y - vertices[vOffset + j].y, center.z - vertices[vOffset + j].z, velocity));
                        }
                    }
                    {
                        var boneWeight = voxelObjectCore.GetBoneWeight(voxelData.voxels[index].position, VoxelBase.VoxelVertexIndex._X_Y_Z);
                        for (int i = 0; i < cubeVertices.Count; i++)
                        {
                            boneWeights.Add(boneWeight);
                        }
                    }
                };

                if (explosionObject.meshes == null)
                    explosionObject.meshes = new List<VoxelSkinnedAnimationObjectExplosion.SkinnedAnimationMeshData>();
                FlagTable3 doneTable = new FlagTable3(voxelData.voxelSize.x, voxelData.voxelSize.y, voxelData.voxelSize.z);
                for (int i = 1; i < voxelBase.materialData.Count; i++)
                {
                    voxelBase.materialData[i].AllAction((pos) =>
                    {
                        doneTable.Set(pos, true);
                        var index = voxelData.VoxelTableContains(pos);
                        if (index < 0) return;
                        AddVertex(i, index);
                    });
                }
                for (int index = 0; index < voxelData.voxels.Length; index++)
                {
                    if (doneTable.Get(voxelData.voxels[index].position)) continue;
                    AddVertex(0, index);
                }
                if (vertices.Count > 0)
                {
                    for (int i = explosionObject.meshes.Count; i <= meshIndex; i++)
                        explosionObject.meshes.Add(null);
                    explosionObject.meshes[meshIndex] = CreateMesh(explosionObject.meshes[meshIndex]);
                    if (!AssetDatabase.Contains(explosionObject.meshes[meshIndex].mesh))
                    {
                        voxelBaseCore.AddObjectToPrefabAsset(explosionObject.meshes[meshIndex].mesh, "explosion_mesh", meshIndex);
                    }
                    meshIndex++;
                }
                explosionObject.meshes.RemoveRange(meshIndex, explosionObject.meshes.Count - meshIndex);
            }
            #endregion

            #region Material
            if (explosionObject.materials == null)
                explosionObject.materials = new List<Material>();
            if (explosionObject.materials.Count < voxelBase.materialData.Count)
            {
                for (int i = explosionObject.materials.Count; i < voxelBase.materialData.Count; i++)
                    explosionObject.materials.Add(null);
            }
            else if (explosionObject.materials.Count > voxelBase.materialData.Count)
            {
                explosionObject.materials.RemoveRange(voxelBase.materialData.Count, explosionObject.materials.Count - voxelBase.materialData.Count);
            }
            for (int i = 0; i < voxelBase.materialData.Count; i++)
            {
                if (!voxelBase.materialIndexes.Contains(i))
                {
                    if (explosionObject.materials[i] != null)
                    {
                        explosionObject.materials[i] = null;
                        voxelBaseCore.DestroyUnusedObjectInPrefabObject();
                    }
                    continue;
                }
                if (explosionObject.materials[i] == null)
                    explosionObject.materials[i] = new Material(GetStandardShader(voxelBase.materialData[i].transparent));
                else
                    explosionObject.materials[i].shader = GetStandardShader(voxelBase.materialData[i].transparent);
                if (!AssetDatabase.Contains(explosionObject.materials[i]))
                    explosionObject.materials[i].name = explosionObject.materials[i].shader.name;
                if (!AssetDatabase.Contains(explosionObject.materials[i]))
                {
                    voxelBaseCore.AddObjectToPrefabAsset(explosionObject.materials[i], "explosion_mat", i);
                }
            }
            #endregion
        }

        public void Bake()
        {
            Generate();
            explosionObject.BakeMesh();
            for (int i = 0; i < explosionObject.meshes.Count; i++)
            {
                if (explosionObject.meshes[i].bakeMesh != null)
                {
                    if (!AssetDatabase.Contains(explosionObject.meshes[i].bakeMesh))
                    {
                        voxelBaseCore.AddObjectToPrefabAsset(explosionObject.meshes[i].bakeMesh, "explosion_bake_mesh", i);
                    }
                }
            }
        }

        public override void SetExplosionCenter()
        {
            if (explosionObject.edit_autoSetExplosionCenter)
            {
                Vector3 center = Vector3.zero;
                if (explosionObject.meshes != null && explosionObject.meshes.Count > 0)
                {
                    for (int i = 0; i < explosionObject.meshes.Count; i++)
                    {
                        if (explosionObject.meshes[i].mesh == null) continue;
                        center += explosionObject.meshes[i].mesh.bounds.center;
                    }
                    center /= (float)explosionObject.meshes.Count;
                }
                explosionObject.explosionCenter = center;
            }
            explosionObject.SetExplosionCenter(explosionObject.explosionCenter);
        }

        public override void CopyMaterialProperties()
        {
            if (voxelObject == null) return;

            if (explosionObject.materials != null && voxelObject.materials != null)
            {
                for (int i = 0; i < voxelObject.materials.Count; i++)
                {
                    if (explosionObject.materials[i] != null && voxelObject.materials[i] != null)
                    {
                        if (voxelObject.materials[i].HasProperty("_Color"))
                            explosionObject.materials[i].color = voxelObject.materials[i].color;
                        if (voxelObject.materials[i].HasProperty("_Glossiness"))
                            explosionObject.materials[i].SetFloat("_Glossiness", voxelObject.materials[i].GetFloat("_Glossiness"));
                        if (voxelObject.materials[i].HasProperty("_Metallic"))
                            explosionObject.materials[i].SetFloat("_Metallic", voxelObject.materials[i].GetFloat("_Metallic"));
                    }
                }
            }
        }

        public override void ResetAllAssets()
        {
            #region Mesh
            explosionObject.meshes = null;
            #endregion

            #region Material
            if (explosionObject.materials != null)
            {
                for (int i = 0; i < explosionObject.materials.Count; i++)
                {
                    if (explosionObject.materials[i] == null)
                        continue;
                    explosionObject.materials[i] = EditorCommon.Instantiate(explosionObject.materials[i]);
                }
            }
            #endregion
        }
    }
}
