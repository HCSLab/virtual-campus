using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelImporter
{
    public class VoxelFrameAnimationObjectCore : VoxelBaseCore
    {
        public VoxelFrameAnimationObjectCore(VoxelBase target) : base(target)
        {
            voxelObject = target as VoxelFrameAnimationObject;
        }

        public VoxelFrameAnimationObject voxelObject { get; protected set; }

        #region AtlasRects
        protected Rect[] atlasRects;
        protected AtlasRectTable atlasRectTable;
        #endregion

        #region Chunk
        protected class ChunkData
        {
            public int voxelBegin;
            public int voxelEnd;

            public VoxelData.ChunkArea area;

            public VoxelData.FaceAreaTable faceAreaTable;
        }
        protected List<ChunkData> chunkDataList;
        #endregion

        #region CreateVoxel
        public override bool ReCreate()
        {
            ClearFramesIcon();

            #region Path
            if (voxelObject.frames != null)
            {
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    if (voxelObject.frames[i].voxelFileObject != null)
                    {
                        var assetPath = AssetDatabase.GetAssetPath(voxelObject.frames[i].voxelFileObject);
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            voxelObject.frames[i].voxelFilePath = EditorCommon.GetProjectRelativePath2FullPath(assetPath);
                        }
                    }
                }
            }
            #endregion
            
            var result = base.ReCreate();

            if (voxelObject.frames.Count > 0 && voxelObject.edit_frameIndex < 0)
            {
                voxelObject.edit_frameIndex = 0;
                SetCurrentMesh();
            }

            return result;
        }
        public override void Reset(string path, UnityEngine.Object obj)
        {
            base.Reset(path, obj);

            voxelObject.frames = new List<VoxelFrameAnimationObject.FrameData>();
            bool done = false;
            if (obj != null && obj is Texture2D)
            {
                var assetPath = AssetDatabase.GetAssetPath(obj);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath);
                    TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    if (importer != null && importer.spriteImportMode == SpriteImportMode.Multiple)
                    {
                        for (int i = 0; i < sprites.Length; i++)
                        {
                            if (sprites[i] is Sprite)
                                voxelObject.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = sprites[i], name = voxelObject.Edit_GetUniqueFrameName(sprites[i].name) });
                        }
                        done = true;
                    }
                }
            }
            else if(GetFileType(path) == VoxelBase.FileType.vox)
            {
                var subCount = GetVoxelFileSubCount(path);
                if (subCount > 1)
                {
                    for (int i = 0; i < subCount; i++)
                    {
                        voxelObject.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = obj, voxelFileSubIndex = i, name = voxelObject.Edit_GetUniqueFrameName(string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(path), i)) });
                    }
                    done = true;
                }
            }
            if(!done)
            {
                voxelObject.frames.Add(new VoxelFrameAnimationObject.FrameData() { voxelFilePath = path, voxelFileObject = obj, name = voxelObject.Edit_GetUniqueFrameName(Path.GetFileNameWithoutExtension(path)) });
            }

            voxelObject.edit_frameIndex = 0;
        }
        public override bool IsVoxelFileExists()
        {
            var fileExists = true;
            if (voxelObject.frames != null && voxelObject.frames.Count > 0)
            {
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    bool tmp = false;
                    if (!string.IsNullOrEmpty(voxelObject.frames[i].voxelFilePath))
                    {
                        tmp = File.Exists(voxelObject.frames[i].voxelFilePath);
                    }
                    if (!tmp)
                    {
                        if (voxelObject.frames[i].voxelFileObject != null && AssetDatabase.Contains(voxelObject.frames[i].voxelFileObject))
                        {
                            tmp = true;
                        }
                    }
                    if (!tmp)
                        fileExists = false;
                }
            }
            else
            {
                fileExists = false;
            }
            return fileExists;
        }
        public override void ClearVoxelData()
        {
            base.ClearVoxelData();
            if (voxelObject.frames != null)
            {
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    ClearFrameVoxelData(i);
                }
            }
        }
        public void ClearFrameVoxelData(int index)
        {
            voxelObject.frames[index].voxelData = null;
            voxelObject.frames[index].voxelDataCreatedVoxelFileTimeTicks = 0;
        }
        protected override bool LoadVoxelData()
        {
            bool result = true;
            if(voxelObject.frames != null && voxelObject.frames.Count > 0)
            {
                var voxelFilePath = voxelObject.voxelFilePath;
                var voxelFileObject = voxelObject.voxelFileObject;
                var voxelFileSubIndex = voxelObject.voxelFileSubIndex;
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    voxelObject.voxelFilePath = voxelObject.frames[i].voxelFilePath;
                    voxelObject.voxelFileObject = voxelObject.frames[i].voxelFileObject;
                    voxelObject.voxelFileSubIndex = voxelObject.frames[i].voxelFileSubIndex;
                    voxelObject.fileType = voxelObject.frames[i].fileType;
                    voxelObject.localOffset = voxelObject.frames[i].localOffset;
                    voxelObject.voxelData = voxelObject.frames[i].voxelData;
                    voxelObject.voxelDataCreatedVoxelFileTimeTicks = voxelObject.frames[i].voxelDataCreatedVoxelFileTimeTicks;
                    voxelObject.disableData = voxelObject.frames[i].disableData;
                    voxelObject.materialData = voxelObject.frames[i].materialData;
                    voxelObject.materialIndexes = voxelObject.frames[i].materialIndexes;

                    if (!base.LoadVoxelData())
                        result = false;

                    voxelObject.frames[i].voxelFilePath = voxelObject.voxelFilePath;
                    voxelObject.frames[i].voxelFileObject = voxelObject.voxelFileObject;
                    voxelObject.frames[i].voxelFileSubIndex = voxelObject.voxelFileSubIndex;
                    voxelObject.frames[i].fileType = voxelObject.fileType;
                    voxelObject.frames[i].localOffset = voxelObject.localOffset;
                    voxelObject.frames[i].voxelData = voxelObject.voxelData;
                    voxelObject.frames[i].voxelDataCreatedVoxelFileTimeTicks = voxelObject.voxelDataCreatedVoxelFileTimeTicks;
                    voxelObject.frames[i].disableData = voxelObject.disableData;
                    voxelObject.frames[i].materialData = voxelObject.materialData;
                    voxelObject.frames[i].materialIndexes = voxelObject.materialIndexes;
                }
                voxelObject.voxelFilePath = voxelFilePath;
                voxelObject.voxelFileObject = voxelFileObject;
                voxelObject.voxelFileSubIndex = voxelFileSubIndex;
                voxelObject.fileType = voxelObject.frames[0].fileType;
                voxelObject.localOffset = voxelObject.frames[0].localOffset;
                voxelObject.voxelData = voxelObject.frames[0].voxelData;
                voxelObject.voxelDataCreatedVoxelFileTimeTicks = voxelObject.frames[0].voxelDataCreatedVoxelFileTimeTicks;
                voxelObject.disableData = voxelObject.frames[0].disableData;
                voxelObject.materialData = voxelObject.frames[0].materialData;
                voxelObject.materialIndexes = voxelObject.frames[0].materialIndexes;
            }
            else
            {
                result = false;
            }
            return result;
        }
        protected bool isFrameDataCreateVoxelTable { get; set; }
        public void ReadyIndividualVoxelData()
        {
            if (voxelObject.frames.Count > 0 &&
                voxelObject.frames[0].voxelData != null && 
                voxelObject.frames[0].voxelData.vertexList != null)
            {
                return;
            }
            isFrameDataCreateVoxelTable = true;
            ReadyVoxelData(true);
            isFrameDataCreateVoxelTable = false;
        }
        protected override void CreateVoxelTable()
        {
            if (isFrameDataCreateVoxelTable)
                base.CreateVoxelTable();
        }
        protected override void UpdateVisibleFlags()
        {
            if (isFrameDataCreateVoxelTable)
                base.UpdateVisibleFlags();
        }
        public override string GetDefaultPath()
        {
            var path = base.GetDefaultPath();
            if (voxelObject != null)
            {
                if (voxelObject.materials != null)
                {
                    for (int i = 0; i < voxelObject.materials.Count; i++)
                    {
                        if (voxelObject.materials[i] == null)
                            continue;
                        if (AssetDatabase.Contains(voxelObject.materials[i]))
                        {
                            var assetPath = AssetDatabase.GetAssetPath(voxelObject.materials[i]);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                path = Path.GetDirectoryName(assetPath);
                            }
                        }
                    }
                }
                if (voxelObject.atlasTexture != null && AssetDatabase.Contains(voxelObject.atlasTexture))
                {
                    var assetPath = AssetDatabase.GetAssetPath(voxelObject.atlasTexture);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        path = Path.GetDirectoryName(assetPath);
                    }
                }
                if (voxelObject.frames != null)
                {
                    for (int i = 0; i < voxelObject.frames.Count; i++)
                    {
                        if (voxelObject.frames[i].mesh != null && AssetDatabase.Contains(voxelObject.frames[i].mesh))
                        {
                            var assetPath = AssetDatabase.GetAssetPath(voxelObject.frames[i].mesh);
                            if (!string.IsNullOrEmpty(assetPath))
                            {
                                path = Path.GetDirectoryName(assetPath);
                            }
                        }
                    }
                }
            }
            return path;
        }
        #endregion

        #region CreateMesh
        protected override bool CreateMesh()
        {
            base.CreateMesh();

            #region ProgressBar
            const float MaxProgressCount = 14f;
            float ProgressCount = 0;
            Action<string> DisplayProgressBar = (info) =>
            {
                if (voxelData.voxels.Length > 10000)
                    EditorUtility.DisplayProgressBar("Create Mesh...", string.Format("{0} / {1}", ProgressCount, MaxProgressCount), (ProgressCount++ / MaxProgressCount));
            };
            #endregion

            DisplayProgressBar("");

            {
                bool reloadVoxelData = false;
                foreach (var frame in voxelObject.frames)
                {
                    if (frame.disableData != null &&
                        frame.disableData.Count > 0)
                    {
                        reloadVoxelData = true;
                        break;
                    }
                    if (frame.materialData != null)
                    {
                        foreach (var material in frame.materialData)
                        {
                            if (material != null && material.Count > 0)
                            {
                                reloadVoxelData = true;
                                break;
                            }
                        }
                        if (reloadVoxelData)
                            break;
                    }
                }
                if (reloadVoxelData)
                {
                    ReadyIndividualVoxelData();
                }
            };

            DisplayProgressBar("");

            #region Combine VoxelData
            {
                voxelBase.voxelData = new VoxelData();
                voxelBase.voxelData.chunkTable = new DataTable3<IntVector3>(voxelBase.voxelData.voxelSize.x, voxelBase.voxelData.voxelSize.y, voxelBase.voxelData.voxelSize.z);

                chunkDataList = new List<ChunkData>(voxelObject.frames.Count);
                int totalVoxelCount = 0;
                {
                    for (int i = 0; i < voxelObject.frames.Count; i++)
                        totalVoxelCount += voxelObject.frames[i].voxelData.voxels.Length;
                }
                var voxels = new VoxelData.Voxel[totalVoxelCount];
                IntVector3 voxelSize = IntVector3.zero;
                Dictionary<Color, int> paletteTable = new Dictionary<Color, int>();
                {
                    int offset = 0;
                    int index = 0;
                    for (int i = 0; i < voxelObject.frames.Count; i++)
                    {
                        var voxelData = voxelObject.frames[i].voxelData;
                        chunkDataList.Add(new ChunkData());
                        chunkDataList[i].voxelBegin = index;
                        for (int j = 0; j < voxelData.voxels.Length; j++)
                        {
                            var voxel = voxelData.voxels[j];
                            var color = voxelData.palettes[voxel.palette];
                            if (!paletteTable.ContainsKey(color))
                                paletteTable.Add(color, paletteTable.Count);
                            voxel.palette = paletteTable[color];
                            voxel.z += offset;
                            voxels[index++] = voxel;
                            voxelBase.voxelData.chunkTable.Set(voxel.position, new IntVector3(i, 0, 0));
                        }
                        chunkDataList[i].voxelEnd = index;
                        chunkDataList[i].area = new VoxelData.ChunkArea() { min = new IntVector3(0, 0, offset), max = new IntVector3(voxelData.voxelSize.x, voxelData.voxelSize.y, offset + voxelData.voxelSize.z) };
                        voxelSize = IntVector3.Max(voxelSize, new IntVector3(voxelData.voxelSize.x, voxelData.voxelSize.y, offset + voxelData.voxelSize.z));
                        offset += voxelData.voxelSize.z + 1;
                    }
                }
                #region Create
                voxelBase.localOffset = Vector3.zero;

                voxelBase.fileType = VoxelBase.FileType.vox;

                voxelBase.voxelData.voxels = voxels;
                voxelBase.voxelData.palettes = new Color[paletteTable.Count];
                foreach (var pair in paletteTable)
                    voxelBase.voxelData.palettes[pair.Value] = pair.Key;
                voxelBase.voxelData.voxelSize = voxelSize;

                base.CreateVoxelTable();
                base.UpdateVisibleFlags();
                #endregion
            }
            #endregion

            DisplayProgressBar("");

            #region Combine DisableData
            {
                #region Erase
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    if (voxelObject.frames[i].disableData == null) continue;

                    List<IntVector3> removeList = new List<IntVector3>();
                    voxelObject.frames[i].disableData.AllAction((pos, face) =>
                    {
                        if (voxelObject.frames[i].voxelData.VoxelTableContains(pos) < 0)
                        {
                            removeList.Add(pos);
                        }
                    });
                    for (int k = 0; k < removeList.Count; k++)
                    {
                        voxelObject.frames[i].disableData.RemoveDisable(removeList[k]);
                    }
                }
                #endregion
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    if (voxelObject.frames[i].disableData == null)
                        voxelObject.frames[i].disableData = new DisableData();
                }
                {
                    voxelObject.disableData = new DisableData();
                    for (int j = 0; j < voxelObject.frames.Count; j++)
                    {
                        if (voxelObject.frames[j].disableData == null) continue;
                        voxelObject.frames[j].disableData.AllAction((pos, face) =>
                        {
                            voxelObject.disableData.SetDisable(chunkDataList[j].area.min + pos, face);
                        });
                    }
                }
            }
            #endregion

            DisplayProgressBar("");

            #region Combine MaterialData
            {
                #region Erase
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    if (voxelObject.frames[i].materialData == null) continue;
                    for (int j = 0; j < voxelObject.frames[i].materialData.Count; j++)
                    {
                        List<IntVector3> removeList = new List<IntVector3>();
                        voxelObject.frames[i].materialData[j].AllAction((pos) =>
                        {
                            if (voxelObject.frames[i].voxelData.VoxelTableContains(pos) < 0)
                            {
                                removeList.Add(pos);
                            }
                        });
                        for (int k = 0; k < removeList.Count; k++)
                        {
                            voxelObject.frames[i].materialData[j].RemoveMaterial(removeList[k]);
                        }
                    }
                }
                #endregion
                voxelObject.materialData = new List<MaterialData>();
                int materialCount = 1;
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    if (voxelObject.frames[i].materialData != null)
                        materialCount = Math.Max(materialCount, voxelObject.frames[i].materialData.Count);
                }
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    if (voxelObject.frames[i].materialData == null)
                        voxelObject.frames[i].materialData = new List<MaterialData>();
                    for (int j = voxelObject.frames[i].materialData.Count; j < materialCount; j++)
                    {
                        voxelObject.frames[i].materialData.Add(new MaterialData());
                    }
                }
                for (int i = 0; i < materialCount; i++)
                {
                    voxelObject.materialData.Add(new MaterialData());
                    voxelObject.materialData[i].name = voxelObject.frames[0].materialData[i].name;
                    voxelObject.materialData[i].transparent = voxelObject.frames[0].materialData[i].transparent;
                    voxelObject.materialData[i].material = voxelObject.frames[0].materialData[i].material;
                    for (int j = 0; j < voxelObject.frames.Count; j++)
                    {
                        if (voxelObject.frames[j].materialData[i] == null) continue;
                        voxelObject.frames[j].materialData[i].AllAction((pos) =>
                        {
                            voxelObject.materialData[i].SetMaterial(chunkDataList[j].area.min + pos);
                        });
                    }
                }
            }
            #endregion

            DisplayProgressBar("");

            #region Material
            {
                if (voxelBase.materialData == null)
                    voxelBase.materialData = new List<MaterialData>();
                if (voxelBase.materialData.Count == 0)
                    voxelBase.materialData.Add(null);
                for (int i = 0; i < voxelBase.materialData.Count; i++)
                {
                    if (voxelBase.materialData[i] == null)
                        voxelBase.materialData[i] = new MaterialData();
                }
                if (voxelObject.materials == null)
                    voxelObject.materials = new List<Material>();
                if (voxelObject.materials.Count < voxelObject.materialData.Count)
                {
                    for (int i = voxelObject.materials.Count; i < voxelObject.materialData.Count; i++)
                        voxelObject.materials.Add(null);
                }
                else if (voxelObject.materials.Count > voxelObject.materialData.Count)
                {
                    voxelObject.materials.RemoveRange(voxelObject.materialData.Count, voxelObject.materials.Count - voxelObject.materialData.Count);
                }
            }
            voxelBase.CreateMaterialIndexTable();
            #endregion

            DisplayProgressBar("");

            CalcDataCreate(voxelBase.voxelData.voxels);

            DisplayProgressBar("");

            #region CreateFaceAreaTable
            {
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    VoxelData.Voxel[] voxels = new VoxelData.Voxel[chunkDataList[i].voxelEnd - chunkDataList[i].voxelBegin];
                    Array.Copy(voxelBase.voxelData.voxels, chunkDataList[i].voxelBegin, voxels, 0, voxels.Length);
                    chunkDataList[i].faceAreaTable = CreateFaceArea(voxels);
                }
            }
            #endregion

            DisplayProgressBar("");

            #region CreateTexture
            {
                var tmpFaceAreaTable = new VoxelData.FaceAreaTable();
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    tmpFaceAreaTable.Merge(chunkDataList[i].faceAreaTable);
                }
                {
                    var atlasTextureTmp = voxelObject.atlasTexture;
                    if (!CreateTexture(tmpFaceAreaTable, voxelBase.voxelData.palettes, ref atlasRectTable, ref atlasTextureTmp, ref atlasRects))
                    {
                        EditorUtility.ClearProgressBar();
                        return false;
                    }
                    voxelObject.atlasTexture = atlasTextureTmp;
                    if (!AssetDatabase.Contains(voxelObject.atlasTexture))
                    {
                        AddObjectToPrefabAsset(voxelObject.atlasTexture, "tex");
                    }
                }
            }
            #endregion

            DisplayProgressBar("");

            #region CreateMesh
            DisplayProgressBar("");
            if (voxelObject.importMode == VoxelBase.ImportMode.LowPoly)
            {
                int forward = 0;
                int up = 0;
                int right = 0;
                int left = 0;
                int down = 0;
                int back = 0;
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    AtlasRectTable atlasRectTableTmp = new AtlasRectTable();
                    {
                        atlasRectTableTmp.forward = atlasRectTable.forward.GetRange(forward, chunkDataList[i].faceAreaTable.forward.Count);
                        forward += chunkDataList[i].faceAreaTable.forward.Count;
                        atlasRectTableTmp.up = atlasRectTable.up.GetRange(up, chunkDataList[i].faceAreaTable.up.Count);
                        up += chunkDataList[i].faceAreaTable.up.Count;
                        atlasRectTableTmp.right = atlasRectTable.right.GetRange(right, chunkDataList[i].faceAreaTable.right.Count);
                        right += chunkDataList[i].faceAreaTable.right.Count;
                        atlasRectTableTmp.left = atlasRectTable.left.GetRange(left, chunkDataList[i].faceAreaTable.left.Count);
                        left += chunkDataList[i].faceAreaTable.left.Count;
                        atlasRectTableTmp.down = atlasRectTable.down.GetRange(down, chunkDataList[i].faceAreaTable.down.Count);
                        down += chunkDataList[i].faceAreaTable.down.Count;
                        atlasRectTableTmp.back = atlasRectTable.back.GetRange(back, chunkDataList[i].faceAreaTable.back.Count);
                        back += chunkDataList[i].faceAreaTable.back.Count;
                    }
                    var extraOffset = new Vector3(0, 0f, -chunkDataList[i].area.min.z);
                    voxelBase.localOffset = voxelObject.frames[i].localOffset;
                    voxelObject.frames[i].mesh = CreateMeshOnly(voxelObject.frames[i].mesh, chunkDataList[i].faceAreaTable, voxelObject.atlasTexture, atlasRects, atlasRectTableTmp, extraOffset, out voxelObject.frames[i].materialIndexes);
                }
            }
            else
            {
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    var extraOffset = new Vector3(0, 0f, -chunkDataList[i].area.min.z);
                    voxelBase.localOffset = voxelObject.frames[i].localOffset;
                    voxelObject.frames[i].mesh = CreateMeshOnly(voxelObject.frames[i].mesh, chunkDataList[i].faceAreaTable, voxelObject.atlasTexture, atlasRects, atlasRectTable, extraOffset, out voxelObject.frames[i].materialIndexes);
                }
            }
            {
                HashSet<int> combineMaterialIndexes = new HashSet<int>();
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    foreach (var index in voxelObject.frames[i].materialIndexes)
                        combineMaterialIndexes.Add(index);
                }
                voxelObject.materialIndexes = combineMaterialIndexes.ToList();
            }
            #endregion

            DisplayProgressBar("");

            #region CreateMaterial
            {
                if (voxelObject.materialData == null)
                    voxelObject.materialData = new List<MaterialData>();
                if (voxelObject.materialData.Count == 0)
                    voxelObject.materialData.Add(null);
                for (int i = 0; i < voxelObject.materialData.Count; i++)
                {
                    if (voxelObject.materialData[i] == null)
                        voxelObject.materialData[i] = new MaterialData();
                }
                if (voxelObject.materials == null)
                    voxelObject.materials = new List<Material>();
                if (voxelObject.materials.Count < voxelObject.materialData.Count)
                {
                    for (int i = voxelObject.materials.Count; i < voxelObject.materialData.Count; i++)
                        voxelObject.materials.Add(null);
                }
                else if (voxelObject.materials.Count > voxelObject.materialData.Count)
                {
                    voxelObject.materials.RemoveRange(voxelObject.materialData.Count, voxelObject.materials.Count - voxelObject.materialData.Count);
                }

                for (int i = 0; i < voxelObject.materials.Count; i++)
                {
                    if (!voxelObject.materialIndexes.Contains(i))
                    {
                        if (voxelObject.materials[i] != null)
                        {
                            voxelObject.materials[i] = null;
                            DestroyUnusedObjectInPrefabObject();
                        }
                        continue;
                    }
                    if (voxelObject.materials[i] == null)
                        voxelObject.materials[i] = EditorCommon.CreateStandardMaterial();
                    if (!AssetDatabase.Contains(voxelObject.materials[i]))
                    {
                        AddObjectToPrefabAsset(voxelObject.materials[i], "mat", i);
                    }
                }
            }
            #endregion

            DisplayProgressBar("");
            {
                if (voxelBase.generateLightmapUVs)
                {
                    var param = voxelBase.GetLightmapParam();
                    for (int i = 0; i < chunkDataList.Count; i++)
                    {
                        if (voxelObject.frames[i].mesh.uv.Length > 0)
                            Unwrapping.GenerateSecondaryUVSet(voxelObject.frames[i].mesh, param);
                    }
                }
                if (voxelBase.generateTangents)
                {
                    for (int i = 0; i < chunkDataList.Count; i++)
                    {
                        voxelObject.frames[i].mesh.RecalculateTangents();
                    }
                }
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    if (!AssetDatabase.Contains(voxelObject.frames[i].mesh))
                    {
                        AddObjectToPrefabAsset(voxelObject.frames[i].mesh, string.Format("mesh_{0}", voxelObject.frames[i].name));
                    }
                }
            }

            DisplayProgressBar("");

            SetRendererCompornent();
            
            RefreshCheckerSave();

            EditorUtility.ClearProgressBar();

            voxelObject.Edit_SetFrameCurrentVoxelOtherData();

            return true;
        }
        protected override void CreateMeshAfter()
        {
            chunkDataList = null;

            base.CreateMeshAfter();
        }
        public override void SetRendererCompornent()
        {
            if (voxelBase.updateMaterialTexture)
            {
                if (voxelObject.materials != null)
                {
                    for (int i = 0; i < voxelObject.materials.Count; i++)
                    {
                        if (voxelObject.materials[i] == null)
                            continue;
                        Undo.RecordObject(voxelObject.materials[i], "Inspector");
                        EditorCommon.SetMainTexture(voxelObject.materials[i], voxelObject.atlasTexture);
                    }
                }
            }
            SetCurrentMesh();
        }
        public void SetCurrentMesh()
        {
            Undo.RecordObject(voxelObject, "Inspector");
            if (!voxelObject.edit_frameEnable)
                voxelObject.mesh = null;
            else
                voxelObject.mesh = voxelObject.edit_currentFrame.mesh;

            {
                var meshFilter = voxelBase.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Undo.RecordObject(meshFilter, "Inspector");
                    meshFilter.sharedMesh = voxelObject.mesh;
                }
            }
            if (voxelBase.updateMeshRendererMaterials)
            {
                var renderer = voxelBase.GetComponent<Renderer>();
                Undo.RecordObject(renderer, "Inspector");
                if (voxelObject.materials != null && voxelObject.edit_frameEnable)
                {
                    Material[] tmps = new Material[voxelObject.edit_currentFrame.materialIndexes.Count];
                    for (int i = 0; i < voxelObject.edit_currentFrame.materialIndexes.Count; i++)
                    {
                        tmps[i] = voxelObject.materials[voxelObject.edit_currentFrame.materialIndexes[i]];
                    }
                    voxelObject.Edit_SetPlayMaterials(tmps);
                    renderer.sharedMaterials = tmps;
                }
                else
                {
                    voxelObject.Edit_ClearPlayMaterials();
                    renderer.sharedMaterial = null;
                }
            }
            if (voxelBase.loadFromVoxelFile && voxelBase.materialData != null)
            {
                if (voxelObject.materials != null && voxelObject.materials.Count == voxelBase.materialData.Count)
                {
                    for (int i = 0; i < voxelObject.materials.Count; i++)
                    {
                        if (voxelObject.materials[i] == null)
                            continue;
                        Undo.RecordObject(voxelObject.materials[i], "Inspector");
                        SetMaterialData(voxelObject.materials[i], voxelBase.materialData[i]);
                    }
                }
            }
        }
        public override Mesh[] Edit_CreateMesh(List<VoxelData.Voxel> voxels, List<Edit_VerticesInfo> dstList = null, bool combine = true)
        {
            return new Mesh[1] { Edit_CreateMeshOnly(voxels, null, dstList, combine) };
        }
        #endregion

        #region Preview & Icon
        public void ClearFramesIcon()
        {
            if (voxelObject.frames == null) return;
            for (int i = 0; i < voxelObject.frames.Count; i++)
            {
                if (voxelObject.frames[i] == null) continue;
                voxelObject.frames[i].icon = null;
            }
        }
        #endregion

        #region Asset
        public override void ResetAllAssets()
        {
            #region Material
            if (voxelObject.materials != null)
            {
                for (int i = 0; i < voxelObject.materials.Count; i++)
                {
                    if (voxelObject.materials[i] == null)
                        continue;
                    voxelObject.materials[i] = EditorCommon.Instantiate(voxelObject.materials[i]);
                }
            }
            #endregion

            #region Texture
            voxelObject.atlasTexture = null;
            #endregion

            #region Mesh
            voxelObject.mesh = null;
            if (voxelObject.frames != null)
            {
                for (int i = 0; i < voxelObject.frames.Count; i++)
                {
                    voxelObject.frames[i].mesh = null;
                }
            }
            #endregion

            #region Structure
            voxelObject.voxelStructure = null;
            #endregion
        }
        #endregion

        #region Undo
        protected override void RefreshCheckerCreate() { voxelObject.refreshChecker = new VoxelFrameAnimationObject.RefreshCheckerFrameAnimation(voxelObject); }
        #endregion
    }
}
