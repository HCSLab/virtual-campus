using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelImporter
{
    public class VoxelChunksObjectChunkExplosionCore
    {
        public VoxelChunksObjectChunkExplosion chunkObject { get; private set; }

        public VoxelChunksObjectChunkExplosionCore(VoxelChunksObjectChunkExplosion target)
        {
            chunkObject = target;
        }
    }
}
