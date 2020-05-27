using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
	public class VoxelChunksObjectChunkCore
    {
        public VoxelChunksObjectChunkCore(VoxelChunksObjectChunk target)
        {
            voxelChunk = target;
            voxelObject = target.transform.parent.GetComponent<VoxelChunksObject>();
            objectCore = new VoxelChunksObjectCore(voxelObject);
        }

        public VoxelChunksObjectChunk voxelChunk { get; protected set; }
        public VoxelChunksObject voxelObject { get; protected set; }
        public VoxelChunksObjectCore objectCore { get; protected set; }

        public void Initialize()
        {
            voxelChunk.EditorInitialize();
        }
        
        #region CreateVoxel
        public string GetDefaultPath()
        {
            var path = objectCore.GetDefaultPath();
            if (voxelObject != null)
            {
                if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
                {
                    if (voxelChunk.mesh != null && AssetDatabase.Contains(voxelChunk.mesh))
                    {
                        var assetPath = AssetDatabase.GetAssetPath(voxelChunk.mesh);
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            path = Path.GetDirectoryName(assetPath);
                        }
                    }
                }
                else if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
                {
                    if (voxelChunk.mesh != null && AssetDatabase.Contains(voxelChunk.mesh))
                    {
                        var assetPath = AssetDatabase.GetAssetPath(voxelChunk.mesh);
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            path = Path.GetDirectoryName(assetPath);
                        }
                    }
                    if (voxelChunk.materials != null)
                    {
                        for (int i = 0; i < voxelChunk.materials.Count; i++)
                        {
                            if (voxelChunk.materials[i] == null)
                                continue;
                            if (AssetDatabase.Contains(voxelChunk.materials[i]))
                            {
                                var assetPath = AssetDatabase.GetAssetPath(voxelChunk.materials[i]);
                                if (!string.IsNullOrEmpty(assetPath))
                                {
                                    path = Path.GetDirectoryName(assetPath);
                                }
                            }
                        }
                    }
                    if (voxelChunk.atlasTexture != null && AssetDatabase.Contains(voxelChunk.atlasTexture))
                    {
                        var assetPath = AssetDatabase.GetAssetPath(voxelChunk.atlasTexture);
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            path = Path.GetDirectoryName(assetPath);
                        }
                    }
                }
                else
                {
                    Assert.IsTrue(false);
                }
            }
            return path;
        }
        #endregion
    }
}
