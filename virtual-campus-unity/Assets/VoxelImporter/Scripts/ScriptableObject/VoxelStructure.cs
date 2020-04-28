using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
	public class VoxelStructure : ScriptableObject
	{
        [Serializable, System.Diagnostics.DebuggerDisplay("Position({x}, {y}, {z}), Palette({palette})")]
        public struct Voxel
        {
#if UNITY_EDITOR
            public Voxel(VoxelData.Voxel voxel)
            {
                this.x = voxel.x;
                this.y = voxel.y;
                this.z = voxel.z;
                this.palette = voxel.palette;
                this.visible = voxel.visible;
            }
#endif

            public Vector3 position { get { return new Vector3(x, y, z); } }

            public int x;
            public int y;
            public int z;
            public int palette;
            public VoxelBase.Face visible;
        }

#if UNITY_EDITOR
        public void Set(VoxelData voxelData)
        {
            voxels = new Voxel[voxelData.voxels.Length];
            for (int i = 0; i < voxelData.voxels.Length; i++)
            {
                voxels[i] = new Voxel(voxelData.voxels[i]);
            }
            palettes = new Color[voxelData.palettes.Length];
            voxelData.palettes.CopyTo(palettes, 0);
            voxelSize = voxelData.voxelSize;
        }
#endif

        public Voxel[] voxels;
        public Color[] palettes;
        public IntVector3 voxelSize;
    }
}
