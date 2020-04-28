using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace VoxelImporter
{
    public class StructureData
    {
        public struct Voxel
        {
            public List<Index> indices;
        }

        public struct Index
        {
            public Index(int vertexIndex, VoxelBase.VoxelVertexIndex voxelPosition)
            {
                this.vertexIndex = vertexIndex;
                this.voxelPosition = voxelPosition;
            }

            public int vertexIndex;
            public VoxelBase.VoxelVertexIndex voxelPosition;
        }

        public StructureData(VoxelData voxelData)
        {
            voxels = new Voxel[voxelData.voxels.Length];
            for (int i = 0; i < voxels.Length; i++)
            {
                voxels[i].indices = new List<Index>();
            }
        }

        public Voxel[] voxels;
    }
}

#endif
