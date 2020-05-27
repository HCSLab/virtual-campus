using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [AddComponentMenu("Voxel Importer/Voxel Chunks Object")]
    public class VoxelChunksObject : VoxelBase
    {
        public enum MaterialMode
        {
            Combine,
            Individual,
        }

#if !UNITY_EDITOR   
        void Awake()
        {
            Destroy(this);
        }
#else
        public override bool EditorInitialize()
        {
            var result = base.EditorInitialize();

            //ver1.021 -> ver1.0.3
            if (material != null)
            {
                materials = new List<Material>();
                materials.Add(material);
                materialData = new List<MaterialData>();
                materialData.Add(new MaterialData());
                materialIndexes = new List<int>();
                materialIndexes.Add(0);
                material = null;
                result = true;
            }
            //ver1.0.4 new
            UpdateChunks();

            return result;
        }

        public VoxelChunksObjectChunk[] chunks; //ver1.0.4 new

        [SerializeField]
        protected Material material;        //ver1.021 old
        public List<Material> materials;    //ver1.0.3 new
        public Texture2D atlasTexture;

        public enum SplitMode
        {
            ChunkSize,
            QubicleMatrix = 100,
            WorldEditor = 100,
        }

        public SplitMode splitMode = SplitMode.QubicleMatrix;
        public IntVector3 chunkSize = new IntVector3(16, 16, 16);
        public bool createContactChunkFaces;

        public MaterialMode materialMode;

        public void UpdateChunks()
        {
            List<VoxelChunksObjectChunk> list = new List<VoxelChunksObjectChunk>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var chunk = transform.GetChild(i).GetComponent<VoxelChunksObjectChunk>();
                if (chunk != null)
                    list.Add(chunk);
            }
            chunks = list.ToArray();

            if (updatedChunks != null)
                updatedChunks.Invoke();
        }
        public delegate void UpdatedChunks();
        public UpdatedChunks updatedChunks;

        #region Editor
        public Vector3 edit_importScale;
        public Vector3 edit_importOffset;
        public IntVector3 edit_chunkSize;
        public bool IsEditorChanged()
        {
            return edit_importScale != importScale ||
                    edit_importOffset != importOffset ||
                    edit_chunkSize != chunkSize;
        }
        public void RevertEditorParam()
        {
            edit_importScale = importScale;
            edit_importOffset = importOffset;
            edit_chunkSize = chunkSize;
        }
        public void ApplyEditorParam()
        {
            importScale = edit_importScale;
            importOffset = edit_importOffset;
            chunkSize = edit_chunkSize;
        }

        public override void SaveEditTmpData()
        {
            base.SaveEditTmpData();

            RevertEditorParam();
        }
        #endregion

        #region Asset
        public override bool IsUseAssetObject(UnityEngine.Object obj)
        {
            if (materialMode == VoxelChunksObject.MaterialMode.Combine)
            {
                if (materials != null)
                {
                    for (int j = 0; j < materials.Count; j++)
                    {
                        if (materials[j] == obj) return true;
                    }
                }
                if (atlasTexture == obj) return true;
            }

            if (chunks != null)
            {
                for (int i = 0; i < chunks.Length; i++)
                {
                    var c = chunks[i];
                    if (c == null) continue;
                    if (c.mesh == obj) return true;
                    if (materialMode == VoxelChunksObject.MaterialMode.Individual)
                    {
                        if (c.materials != null)
                        {
                            for (int j = 0; j < c.materials.Count; j++)
                            {
                                if (c.materials[j] == obj) return true;
                            }
                        }
                        if (c.atlasTexture == obj) return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region Undo
        public class RefreshCheckerChunk : RefreshChecker
        {
            public RefreshCheckerChunk(VoxelChunksObject voxelObject) : base(voxelObject)
            {
                controllerChunk = voxelObject;
            }

            public VoxelChunksObject controllerChunk;

            public VoxelChunksObject.SplitMode splitMode;
            public IntVector3 chunkSize;
            public bool createContactChunkFaces;
            public VoxelChunksObject.MaterialMode materialMode;

            public override void Save()
            {
                base.Save();

                splitMode = controllerChunk.splitMode;
                chunkSize = controllerChunk.chunkSize;
                createContactChunkFaces = controllerChunk.createContactChunkFaces;
                materialMode = controllerChunk.materialMode;
            }
            public override bool Check()
            {
                if (base.Check())
                    return true;

                return splitMode != controllerChunk.splitMode ||
                        chunkSize != controllerChunk.chunkSize ||
                        createContactChunkFaces != controllerChunk.createContactChunkFaces ||
                        materialMode != controllerChunk.materialMode;
            }
        }
        #endregion
#endif
    }
}
