using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelImporter
{
    public class VoxelChunksObjectExplosionCore : VoxelBaseExplosionCore
    {
        public VoxelChunksObjectExplosion explosionObject { get; private set; }

        public VoxelChunksObject voxelObject { get; private set; }
        public VoxelChunksObjectCore voxelObjectCore { get; private set; }

        public VoxelChunksObjectExplosionCore(VoxelBaseExplosion target) : base(target)
        {
            explosionObject = target as VoxelChunksObjectExplosion;

            voxelBase = voxelObject = target.GetComponent<VoxelChunksObject>();
            voxelBaseCore = voxelObjectCore = new VoxelChunksObjectCore(voxelObject);

            voxelBaseCore.Initialize();

            if (voxelObject != null)
            {
                var chunkObjects = voxelObject.chunks;
                explosionObject.chunksExplosion = new VoxelChunksObjectChunkExplosion[chunkObjects.Length];
                for (int i = 0; i < chunkObjects.Length; i++)
                {
                    if (chunkObjects[i] == null)
                    {
                        explosionObject.chunksExplosion[i] = null;
                        continue;
                    }
                    explosionObject.chunksExplosion[i] = chunkObjects[i].gameObject.GetComponent<VoxelChunksObjectChunkExplosion>();
                    if (explosionObject.chunksExplosion[i] == null)
                        explosionObject.chunksExplosion[i] = Undo.AddComponent<VoxelChunksObjectChunkExplosion>(chunkObjects[i].gameObject);
                }
            }
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
            List<Vector4> tangents = new List<Vector4>();
            List<int>[] triangles = new List<int>[voxelBase.materialData.Count];
            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = new List<int>();
            }

            #region Mesh
            Func<VoxelBaseExplosion.MeshData, VoxelBaseExplosion.MeshData> CreateMesh = (data) =>
            {
                if (data == null)
                    data = new VoxelObjectExplosion.MeshData();
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
                for (int i = 0; i < voxelBase.materialData.Count; i++)
                {
                    triangles[i].Clear();
                }
                return data;
            };
            #endregion

            {
                var chunkObjects = voxelObject.chunks;
                FlagTable3 doneTable = new FlagTable3(voxelData.voxelSize.x, voxelData.voxelSize.y, voxelData.voxelSize.z);
                for (int chunkIndex = 0; chunkIndex < chunkObjects.Length; chunkIndex++)
                {
                    var chunkObject = chunkObjects[chunkIndex];
                    if (chunkObject == null)
                    {
                        explosionObject.chunksExplosion[chunkIndex] = null;
                        continue;
                    }
                    var explosionChunk = explosionObject.chunksExplosion[chunkIndex] = chunkObject.gameObject.GetComponent<VoxelChunksObjectChunkExplosion>();
                    if (explosionChunk == null)
                        explosionChunk = explosionObject.chunksExplosion[chunkIndex] = Undo.AddComponent<VoxelChunksObjectChunkExplosion>(chunkObject.gameObject);

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
                            for (int i = explosionChunk.meshes.Count; i <= meshIndex; i++)
                                explosionChunk.meshes.Add(null);
                            explosionChunk.meshes[meshIndex] = CreateMesh(explosionChunk.meshes[meshIndex]);
                            if (!AssetDatabase.Contains(explosionChunk.meshes[meshIndex].mesh))
                            {
                                voxelBaseCore.AddObjectToPrefabAsset(explosionChunk.meshes[meshIndex].mesh, string.Format("{0}_explosion_mesh", explosionChunk.name), meshIndex);
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
                    };

                    if (explosionChunk.meshes == null)
                        explosionChunk.meshes = new List<VoxelBaseExplosion.MeshData>();
                    for (int i = 1; i < voxelBase.materialData.Count; i++)
                    {
                        voxelBase.materialData[i].AllAction((pos) =>
                        {
                            if (doneTable.Get(pos)) return;
                            if (voxelData.chunkTable.Get(pos) != chunkObject.position) return;
                            doneTable.Set(pos, true);
                            var index = voxelData.VoxelTableContains(pos);
                            if (index < 0) return;
                            AddVertex(i, index);
                        });
                    }
                    for (int index = 0; index < voxelData.voxels.Length; index++)
                    {
                        var pos = voxelData.voxels[index].position;
                        if (doneTable.Get(pos)) continue;
                        if (voxelData.chunkTable.Get(pos) != chunkObject.position) continue;
                        doneTable.Set(pos, true);
                        AddVertex(0, index);
                    }
                    if (vertices.Count > 0)
                    {
                        for (int i = explosionChunk.meshes.Count; i <= meshIndex; i++)
                            explosionChunk.meshes.Add(null);
                        explosionChunk.meshes[meshIndex] = CreateMesh(explosionChunk.meshes[meshIndex]);
                        if (!AssetDatabase.Contains(explosionChunk.meshes[meshIndex].mesh))
                        {
                            voxelBaseCore.AddObjectToPrefabAsset(explosionChunk.meshes[meshIndex].mesh, string.Format("{0}_explosion_mesh", explosionChunk.name), meshIndex);
                        }
                        meshIndex++;
                    }
                    explosionChunk.meshes.RemoveRange(meshIndex, explosionChunk.meshes.Count - meshIndex);

                    explosionChunk.chunkBasicOffset = chunkObject.basicOffset;
                }
            }
            #endregion

            #region Material
            explosionObject.materialMode = voxelObject.materialMode;
            if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
            {
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
                for (int i = 0; i < explosionObject.chunksExplosion.Length; i++)
                {
                    explosionObject.chunksExplosion[i].materials = null;
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
            }
            else if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
            {
                explosionObject.materials = null;
                for (int chunkIndex = 0; chunkIndex < explosionObject.chunksExplosion.Length; chunkIndex++)
                {
                    var explosionChunk = explosionObject.chunksExplosion[chunkIndex];
                    if (explosionChunk.materials == null)
                        explosionChunk.materials = new List<Material>();
                    if (explosionChunk.materials.Count < voxelBase.materialData.Count)
                    {
                        for (int i = explosionChunk.materials.Count; i < voxelBase.materialData.Count; i++)
                            explosionChunk.materials.Add(null);
                    }
                    else if (explosionChunk.materials.Count > voxelBase.materialData.Count)
                    {
                        explosionChunk.materials.RemoveRange(voxelBase.materialData.Count, explosionChunk.materials.Count - voxelBase.materialData.Count);
                    }
                    for (int i = 0; i < voxelBase.materialData.Count; i++)
                    {
                        if (!voxelObject.chunks[chunkIndex].materialIndexes.Contains(i))
                        {
                            if (explosionChunk.materials[i] != null)
                            {
                                explosionChunk.materials[i] = null;
                                voxelBaseCore.DestroyUnusedObjectInPrefabObject();
                            }
                            continue;
                        }
                        if (explosionChunk.materials[i] == null)
                            explosionChunk.materials[i] = new Material(GetStandardShader(voxelBase.materialData[i].transparent));
                        else
                            explosionChunk.materials[i].shader = GetStandardShader(voxelBase.materialData[i].transparent);
                        if (!AssetDatabase.Contains(explosionChunk.materials[i]))
                            explosionChunk.materials[i].name = explosionChunk.materials[i].shader.name;
                        if (!AssetDatabase.Contains(explosionChunk.materials[i]))
                        {
                            voxelBaseCore.AddObjectToPrefabAsset(explosionChunk.materials[i], string.Format("{0}_explosion_mat", explosionChunk.name), i);
                        }
                    }
                }
            }
            else
            {
                Assert.IsTrue(false);
            }
            #endregion
        }

        public override void SetExplosionCenter()
        {
            if (explosionObject.edit_autoSetExplosionCenter)
            {
                Vector3 center = Vector3.zero;
                int count = 0;
                if (explosionObject.chunksExplosion != null)
                {
                    for (int i = 0; i < explosionObject.chunksExplosion.Length; i++)
                    {
                        if (explosionObject.chunksExplosion[i] == null || explosionObject.chunksExplosion[i].meshes == null) continue;
                        for (int j = 0; j < explosionObject.chunksExplosion[i].meshes.Count; j++)
                        {
                            if (explosionObject.chunksExplosion[i].meshes[j].mesh == null) continue;
                            center += explosionObject.chunksExplosion[i].meshes[j].mesh.bounds.center;
                            count++;
                        }
                    }
                }
                if (count > 0)
                    center /= (float)count;
                explosionObject.explosionCenter = center;
            }
            explosionObject.SetExplosionCenter(explosionObject.explosionCenter);
        }

        public override void CopyMaterialProperties()
        {
            if (voxelObject == null) return;

            if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
            {
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
            else if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
            {
                if (explosionObject.chunksExplosion != null)
                {
                    for (int j = 0; j < explosionObject.chunksExplosion.Length; j++)
                    {
                        if (voxelObject.chunks[j] == null || explosionObject.chunksExplosion[j] == null) continue;
                        if (explosionObject.chunksExplosion[j].materials != null && voxelObject.chunks[j].materials != null)
                        {
                            for (int i = 0; i < voxelObject.chunks[j].materials.Count; i++)
                            {
                                if (explosionObject.chunksExplosion[j].materials[i] != null && voxelObject.chunks[j].materials[i] != null)
                                {
                                    if (voxelObject.chunks[j].materials[i].HasProperty("_Color"))
                                        explosionObject.chunksExplosion[j].materials[i].color = voxelObject.chunks[j].materials[i].color;
                                    if (voxelObject.chunks[j].materials[i].HasProperty("_Glossiness"))
                                        explosionObject.chunksExplosion[j].materials[i].SetFloat("_Glossiness", voxelObject.chunks[j].materials[i].GetFloat("_Glossiness"));
                                    if (voxelObject.chunks[j].materials[i].HasProperty("_Metallic"))
                                        explosionObject.chunksExplosion[j].materials[i].SetFloat("_Metallic", voxelObject.chunks[j].materials[i].GetFloat("_Metallic"));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Assert.IsTrue(false);
            }
        }

        public override void ResetAllAssets()
        {
            #region Mesh
            if (explosionObject.chunksExplosion != null)
            {
                for (int i = 0; i < explosionObject.chunksExplosion.Length; i++)
                {
                    if (explosionObject.chunksExplosion[i] == null) continue;
                    explosionObject.chunksExplosion[i].meshes = null;
                    if (explosionObject.chunksExplosion[i].materials == null) continue;
                    for (int j = 0; j < explosionObject.chunksExplosion[i].materials.Count; j++)
                    {
                        if (explosionObject.chunksExplosion[i].materials[j] == null)
                            continue;
                        explosionObject.chunksExplosion[i].materials[j] = EditorCommon.Instantiate(explosionObject.chunksExplosion[i].materials[j]);
                    }
                }
            }
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
