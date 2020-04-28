using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelImporter
{
    public class VoxelChunksObjectCore : VoxelBaseCore
    {
        public VoxelChunksObjectCore(VoxelBase target) : base(target)
        {
            voxelObject = target as VoxelChunksObject;
        }

        public VoxelChunksObject voxelObject { get; protected set; }

        #region Chunk
        public string chunkNameFormat = "Chunk({0})";

        protected IntVector3 GetChunkPosition(IntVector3 position)
        {
            IntVector3 cpos = IntVector3.zero;
            cpos.x = position.x / voxelObject.chunkSize.x;
            cpos.y = position.y / voxelObject.chunkSize.y;
            cpos.z = position.z / voxelObject.chunkSize.z;
            return cpos;
        }
        
        protected class ChunkData
        {
            public IntVector3 position;
            public string name;
            public VoxelData.ChunkArea area;

            public List<int> voxels;
            public Color[] palettes;

            public VoxelChunksObjectChunk chunkObject;
            public VoxelData.FaceAreaTable faceAreaTable;

            public Rect[] atlasRects;
            public AtlasRectTable atlasRectTable;
        }
        protected List<ChunkData> chunkDataList;

        protected override void CreateChunkData()
        {
            if (voxelObject.splitMode != VoxelChunksObject.SplitMode.ChunkSize)
            {
                if (voxelData.chunkDataList == null)
                    voxelObject.splitMode = VoxelChunksObject.SplitMode.ChunkSize;
            }
            if (voxelObject.splitMode == VoxelChunksObject.SplitMode.QubicleMatrix || voxelObject.splitMode == VoxelChunksObject.SplitMode.WorldEditor)
            {
                voxelData.chunkTable = new DataTable3<IntVector3>(voxelData.voxelSize.x, voxelData.voxelSize.y, voxelData.voxelSize.z);
                
                var chunkVoxels = new List<int>[voxelData.chunkDataList.Count];
                var chunkPalettes = new HashSet<Color>[voxelData.chunkDataList.Count];
                for (int i = 0; i < voxelData.chunkDataList.Count; i++)
                {
                    chunkVoxels[i] = new List<int>();
                    chunkPalettes[i] = new HashSet<Color>();
                }
                for (int i = 0; i < voxelData.voxels.Length; i++)
                {
                    var chunkIndex = voxelData.chunkIndexTable.Get(voxelData.voxels[i].position);
                    //voxel
                    chunkVoxels[chunkIndex].Add(i);
                    //palette
                    chunkPalettes[chunkIndex].Add(voxelData.palettes[voxelData.voxels[i].palette]);
                    //
                    voxelData.chunkTable.Set(voxelData.voxels[i].position, new IntVector3(chunkIndex, int.MinValue, int.MinValue));
                }
                {
                    chunkDataList = new List<ChunkData>(chunkVoxels.Length);
                    for (int i = 0; i < chunkVoxels.Length; i++)
                    {
                        chunkDataList.Add(new ChunkData()
                        {
                            position = new IntVector3(i, int.MinValue, int.MinValue),
                            name = string.Format(chunkNameFormat, voxelData.chunkDataList[i].name),
                            area = voxelData.chunkDataList[i].area,
                            voxels = chunkVoxels[i],
                            palettes = chunkPalettes[i].ToArray()
                        });
                    }
                    chunkDataList.Sort((a, b) => string.Compare(voxelData.chunkDataList[a.position.x].name, voxelData.chunkDataList[b.position.x].name));
                }
            }
            else
            {
                voxelData.chunkTable = new DataTable3<IntVector3>(voxelData.voxelSize.x, voxelData.voxelSize.y, voxelData.voxelSize.z);
                
                var chunkVoxels = new DataTable3<List<int>>(voxelData.voxelSize.x, voxelData.voxelSize.y, voxelData.voxelSize.z);
                var chunkPalettes = new DataTable3<HashSet<Color>>(voxelData.voxelSize.x, voxelData.voxelSize.y, voxelData.voxelSize.z);
                for (int i = 0; i < voxelData.voxels.Length; i++)
                {
                    var chunkPosition = GetChunkPosition(voxelData.voxels[i].position);
                    //voxel
                    if (!chunkVoxels.Contains(chunkPosition))
                        chunkVoxels.Set(chunkPosition, new List<int>());
                    chunkVoxels.Get(chunkPosition).Add(i);
                    //palette
                    if (!chunkPalettes.Contains(chunkPosition))
                        chunkPalettes.Set(chunkPosition, new HashSet<Color>());
                    chunkPalettes.Get(chunkPosition).Add(voxelData.palettes[voxelData.voxels[i].palette]);
                    //
                    voxelData.chunkTable.Set(voxelData.voxels[i].position, chunkPosition);
                }
                {
                    chunkDataList = new List<ChunkData>();
                    chunkVoxels.AllAction((x, y, z, list) =>
                    {
                        var pos = new IntVector3(x, y, z);
                        chunkDataList.Add(new ChunkData()
                        {
                            position = pos,
                            name = string.Format("Chunk({0}, {1}, {2})", x, y, z),
                            area = new VoxelData.ChunkArea() { min = pos * voxelObject.chunkSize, max = (pos + IntVector3.one) * voxelObject.chunkSize - IntVector3.one },
                            voxels = list,
                            palettes = chunkPalettes.Get(pos).ToArray()
                        });
                    });
                    chunkDataList.Sort((a, b) => a.position.x != b.position.x ? a.position.x - b.position.x : a.position.y != b.position.y ? a.position.y - b.position.y : a.position.z - b.position.z);
                }
            }
        }

        public bool removeAllChunk;
        public void RemoveAllChunk()
        {
            removeAllChunk = true;
        }
        #endregion

        #region CreateVoxel
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
            }
            return path;
        }
        #endregion

        #region CreateMesh
        protected override bool IsCombineVoxelFace(IntVector3 basePos, IntVector3 combinePos, VoxelBase.Face face)
        {
            if (!base.IsCombineVoxelFace(basePos, combinePos, face))
                return false;

            return voxelData.chunkTable.Get(basePos) == voxelData.chunkTable.Get(combinePos);
        }
        protected override bool IsHiddenVoxelFace(IntVector3 basePos, VoxelBase.Face faceFlag)
        {
            if (voxelObject.createContactChunkFaces)
            {
                Assert.IsTrue(faceFlag == VoxelBase.Face.forward || faceFlag == VoxelBase.Face.up || faceFlag == VoxelBase.Face.right || faceFlag == VoxelBase.Face.left || faceFlag == VoxelBase.Face.down || faceFlag == VoxelBase.Face.back);
                IntVector3 combinePos = basePos;
                {
                    if (faceFlag == VoxelBase.Face.forward) combinePos.z++;
                    if (faceFlag == VoxelBase.Face.up) combinePos.y++;
                    if (faceFlag == VoxelBase.Face.right) combinePos.x++;
                    if (faceFlag == VoxelBase.Face.left) combinePos.x--;
                    if (faceFlag == VoxelBase.Face.down) combinePos.y--;
                    if (faceFlag == VoxelBase.Face.back) combinePos.z--;
                }
                return voxelData.chunkTable.Get(basePos) == voxelData.chunkTable.Get(combinePos);
            }
            else
            {
                return base.IsHiddenVoxelFace(basePos, faceFlag);
            }
        }
        protected override void CreateMeshBefore()
        {
            base.CreateMeshBefore();

            #region RemoveChunk
            voxelObject.UpdateChunks();
            var chunkObjects = voxelObject.chunks;
            {
                bool chunkObjectsUpdate = false;
                bool[] enableTale = new bool[chunkObjects.Length];
                if (!removeAllChunk)
                {
                    for (int i = 0; i < chunkDataList.Count; i++)
                    {
                        for (int j = 0; j < chunkObjects.Length; j++)
                        {
                            if (chunkDataList[i].position == chunkObjects[j].position)
                            {
                                enableTale[j] = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    removeAllChunk = false;
                }
                for (int i = 0; i < enableTale.Length; i++)
                {
                    if (!enableTale[i])
                    {
                        var go = chunkObjects[i].gameObject;
                        while (go.transform.childCount > 0)
                        {
                            Undo.SetTransformParent(go.transform.GetChild(0), voxelObject.transform, "Remove Chunk");
                        }
                        Undo.DestroyObjectImmediate(go);
                        chunkObjectsUpdate = true;
                    }
                }
                if (chunkObjectsUpdate)
                {
                    voxelObject.UpdateChunks();
                    chunkObjects = voxelObject.chunks;
                }
            }
            #endregion

            #region AddChunk
            int chunkCount = 0;
            {
                bool chunkObjectsUpdate = false;
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    GameObject chunkObject = null;
                    for (int j = 0; j < chunkObjects.Length; j++)
                    {
                        if (chunkDataList[i].position == chunkObjects[j].position)
                        {
                            chunkObject = chunkObjects[j].gameObject;
                            break;
                        }
                    }
                    if (chunkObject == null)
                    {
                        chunkObject = new GameObject(chunkDataList[i].name);
                        Undo.RegisterCreatedObjectUndo(chunkObject, "Create Chunk");
                        Undo.SetTransformParent(chunkObject.transform, voxelObject.transform, "Create Chunk");
                        GameObjectUtility.SetStaticEditorFlags(chunkObject, GameObjectUtility.GetStaticEditorFlags(voxelObject.gameObject));
                        chunkObject.transform.localPosition = Vector3.Scale(voxelObject.localOffset + chunkDataList[i].area.centerf, voxelObject.importScale);
                        chunkObject.transform.localRotation = Quaternion.identity;
                        chunkObject.transform.localScale = Vector3.one;
                        chunkObject.layer = voxelObject.gameObject.layer;
                        chunkObject.tag = voxelObject.gameObject.tag;
                        chunkObjectsUpdate = true;
                    }
                    VoxelChunksObjectChunk controller = chunkObject.GetComponent<VoxelChunksObjectChunk>();
                    if (controller == null)
                        controller = Undo.AddComponent<VoxelChunksObjectChunk>(chunkObject);
                    controller.position = chunkDataList[i].position;
                    controller.chunkName = chunkDataList[i].name;
                    controller.basicOffset = Vector3.Scale(voxelObject.localOffset + chunkDataList[i].area.centerf, voxelObject.importScale);
                    chunkCount++;
                }
                if (chunkObjectsUpdate)
                {
                    voxelObject.UpdateChunks();
                    chunkObjects = voxelObject.chunks;
                }
            }
            #endregion

            #region SortChunk
            {
                List<Transform> objList = new List<Transform>();
                var childCount = voxelObject.transform.childCount;
                for (int i = 0; i < childCount; i++)
                {
                    objList.Add(voxelObject.transform.GetChild(i));
                }
                objList.Sort((obj1, obj2) => string.Compare(obj1.name, obj2.name));
                for (int i = 0; i < objList.Count; i++)
                {
                    objList[i].SetSiblingIndex(childCount - 1);
                }
                voxelObject.UpdateChunks();
                chunkObjects = voxelObject.chunks;
            }
            #endregion

            #region UpdateChunk
            for (int i = 0; i < chunkObjects.Length; i++)
            {
                for (int j = 0; j < chunkDataList.Count; j++)
                {
                    if (chunkObjects[i].position == chunkDataList[j].position)
                    {
                        chunkDataList[j].chunkObject = chunkObjects[i];
                        break;
                    }
                }
            }
            #endregion
        }
        protected override bool CreateMesh()
        {
            base.CreateMesh();

            #region ProgressBar
            const float MaxProgressCount = 6f;
            float ProgressCount = 0;
            Action<string> DisplayProgressBar = (info) =>
            {
                if (voxelData.voxels.Length > 10000)
                    EditorUtility.DisplayProgressBar("Create Mesh...", string.Format("{0} / {1}", ProgressCount, MaxProgressCount), (ProgressCount++ / MaxProgressCount));
            };
            #endregion

            DisplayProgressBar("");

            #region Disable
            {
                if (voxelObject.disableData == null)
                    voxelObject.disableData = new DisableData();
                #region Erase
                {
                    List<IntVector3> removeList = new List<IntVector3>();
                    voxelObject.disableData.AllAction((pos, face) =>
                    {
                        if (voxelData.VoxelTableContains(pos) < 0)
                        {
                            removeList.Add(pos);
                        }
                    });
                    for (int j = 0; j < removeList.Count; j++)
                    {
                        voxelObject.disableData.RemoveDisable(removeList[j]);
                    }
                }
                #endregion
            }
            #endregion

            DisplayProgressBar("");

            #region Material
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
                #region Erase
                for (int i = 0; i < voxelObject.materialData.Count; i++)
                {
                    List<IntVector3> removeList = new List<IntVector3>();
                    voxelObject.materialData[i].AllAction((pos) =>
                    {
                        if (voxelData.VoxelTableContains(pos) < 0)
                        {
                            removeList.Add(pos);
                        }
                    });
                    for (int j = 0; j < removeList.Count; j++)
                    {
                        voxelObject.materialData[i].RemoveMaterial(removeList[j]);
                    }
                }
                #endregion
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
            voxelObject.CreateMaterialIndexTable();
            #endregion

            CalcDataCreate(voxelData.voxels);

            #region CreateFaceAreaTable
            {
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    var voxels = new VoxelData.Voxel[chunkDataList[i].voxels.Count];
                    for (int j = 0; j < chunkDataList[i].voxels.Count; j++)
                    {
                        voxels[j] = voxelData.voxels[chunkDataList[i].voxels[j]];
                    }
                    chunkDataList[i].faceAreaTable = CreateFaceArea(voxels);
                    //
                    if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
                    {
                        var paletteTable = new int[voxelData.palettes.Length];
                        for (int j = 0; j < voxelData.palettes.Length; j++)
                        {
                            int newIndex = -1;
                            for (int k = 0; k < chunkDataList[i].palettes.Length; k++)
                            {
                                if (chunkDataList[i].palettes[k] == voxelData.palettes[j])
                                {
                                    newIndex = k;
                                    break;
                                }
                            }
                            paletteTable[j] = newIndex;
                        }
                        chunkDataList[i].faceAreaTable.ReplacePalette(paletteTable);
                    }
                }
            }
            #endregion

            #region CreateTexture
            if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
            {
                #region Combine
                var tmpFaceAreaTable = new VoxelData.FaceAreaTable();
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    tmpFaceAreaTable.Merge(chunkDataList[i].faceAreaTable);
                }
                {
                    var atlasTextureTmp = voxelObject.atlasTexture;
                    if (chunkDataList != null && chunkDataList.Count > 0)
                    {
                        if (!CreateTexture(tmpFaceAreaTable, voxelData.palettes, ref chunkDataList[0].atlasRectTable, ref atlasTextureTmp, ref chunkDataList[0].atlasRects))
                        {
                            EditorUtility.ClearProgressBar();
                            return false;
                        }
                    }
                    else
                    {
                        AtlasRectTable atlasRectTable = null;
                        Rect[] atlasRects = null;
                        if (!CreateTexture(tmpFaceAreaTable, voxelData.palettes, ref atlasRectTable, ref atlasTextureTmp, ref atlasRects))
                        {
                            EditorUtility.ClearProgressBar();
                            return false;
                        }
                    }
                    voxelObject.atlasTexture = atlasTextureTmp;
                    if (!AssetDatabase.Contains(voxelObject.atlasTexture))
                    {
                        AddObjectToPrefabAsset(voxelObject.atlasTexture, "tex");
                    }
                    for (int i = 0; i < chunkDataList.Count; i++)
                    {
                        chunkDataList[i].atlasRects = chunkDataList[0].atlasRects;
                        chunkDataList[i].atlasRectTable = chunkDataList[0].atlasRectTable;
                        chunkDataList[i].chunkObject.materials = null;
                        chunkDataList[i].chunkObject.atlasTexture = null;
                    }
                }
                #endregion
            }
            else if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
            {
                #region Individual
                if (voxelObject.materialData == null)
                    voxelObject.materialData = new List<MaterialData>();
                if (voxelObject.materialData.Count == 0)
                    voxelObject.materialData.Add(null);
                for (int i = 0; i < voxelObject.materialData.Count; i++)
                {
                    if (voxelObject.materialData[i] == null)
                        voxelObject.materialData[i] = new MaterialData();
                }
                voxelObject.materials = null;
                voxelObject.atlasTexture = null;
                for (int c = 0; c < chunkDataList.Count; c++)
                {
                    var atlasTextureTmp = chunkDataList[c].chunkObject.atlasTexture;
                    if (!CreateTexture(chunkDataList[c].faceAreaTable, chunkDataList[c].palettes, ref chunkDataList[c].atlasRectTable, ref atlasTextureTmp, ref chunkDataList[c].atlasRects))
                    {
                        EditorUtility.ClearProgressBar();
                        return false;
                    }
                    chunkDataList[c].chunkObject.atlasTexture = atlasTextureTmp;
                    if (!AssetDatabase.Contains(chunkDataList[c].chunkObject.atlasTexture))
                    {
                        AddObjectToPrefabAsset(chunkDataList[c].chunkObject.atlasTexture, string.Format("{0}_tex", chunkDataList[c].chunkObject.name));
                    }
                }
                #endregion
            }
            else
            {
                Assert.IsTrue(false);
            }
            #endregion

            #region CreateMesh
            DisplayProgressBar("");
            {
                if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
                {
                    #region Combine
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
                            AtlasRectTable atlasRectTable = new AtlasRectTable();
                            {
                                atlasRectTable.forward = chunkDataList[i].atlasRectTable.forward.GetRange(forward, chunkDataList[i].faceAreaTable.forward.Count);
                                forward += chunkDataList[i].faceAreaTable.forward.Count;
                                atlasRectTable.up = chunkDataList[i].atlasRectTable.up.GetRange(up, chunkDataList[i].faceAreaTable.up.Count);
                                up += chunkDataList[i].faceAreaTable.up.Count;
                                atlasRectTable.right = chunkDataList[i].atlasRectTable.right.GetRange(right, chunkDataList[i].faceAreaTable.right.Count);
                                right += chunkDataList[i].faceAreaTable.right.Count;
                                atlasRectTable.left = chunkDataList[i].atlasRectTable.left.GetRange(left, chunkDataList[i].faceAreaTable.left.Count);
                                left += chunkDataList[i].faceAreaTable.left.Count;
                                atlasRectTable.down = chunkDataList[i].atlasRectTable.down.GetRange(down, chunkDataList[i].faceAreaTable.down.Count);
                                down += chunkDataList[i].faceAreaTable.down.Count;
                                atlasRectTable.back = chunkDataList[i].atlasRectTable.back.GetRange(back, chunkDataList[i].faceAreaTable.back.Count);
                                back += chunkDataList[i].faceAreaTable.back.Count;
                            }
                            chunkDataList[i].chunkObject.mesh = CreateMeshOnly(chunkDataList[i].chunkObject.mesh, chunkDataList[i].faceAreaTable, voxelObject.atlasTexture, chunkDataList[i].atlasRects, atlasRectTable, -(voxelObject.localOffset + chunkDataList[i].area.centerf), out chunkDataList[i].chunkObject.materialIndexes);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < chunkDataList.Count; i++)
                        {
                            chunkDataList[i].chunkObject.mesh = CreateMeshOnly(chunkDataList[i].chunkObject.mesh, chunkDataList[i].faceAreaTable, voxelObject.atlasTexture, chunkDataList[i].atlasRects, chunkDataList[i].atlasRectTable, -(voxelObject.localOffset + chunkDataList[i].area.centerf), out chunkDataList[i].chunkObject.materialIndexes);
                        }
                    }
                    {
                        HashSet<int> combineMaterialIndexes = new HashSet<int>();
                        for (int c = 0; c < chunkDataList.Count; c++)
                        {
                            foreach (var index in chunkDataList[c].chunkObject.materialIndexes)
                                combineMaterialIndexes.Add(index);
                        }
                        voxelObject.materialIndexes = combineMaterialIndexes.ToList();
                    }
                    #endregion
                }
                else if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
                {
                    #region Individual
                    for (int i = 0; i < chunkDataList.Count; i++)
                    {
                        chunkDataList[i].chunkObject.mesh = CreateMeshOnly(chunkDataList[i].chunkObject.mesh, chunkDataList[i].faceAreaTable, chunkDataList[i].chunkObject.atlasTexture, chunkDataList[i].atlasRects, chunkDataList[i].atlasRectTable, -(voxelObject.localOffset + chunkDataList[i].area.centerf), out chunkDataList[i].chunkObject.materialIndexes);
                    }
                    voxelObject.materialIndexes = new List<int>();
                    #endregion
                }
                else
                {
                    Assert.IsTrue(false);
                }
            }
            #endregion

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
                if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
                {
                    #region Combine
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
                    #endregion
                }
                else if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
                {
                    #region Individual
                    for (int c = 0; c < chunkDataList.Count; c++)
                    {
                        if (chunkDataList[c].chunkObject.materials == null)
                            chunkDataList[c].chunkObject.materials = new List<Material>();
                        if (chunkDataList[c].chunkObject.materials.Count < voxelObject.materialData.Count)
                        {
                            for (int i = chunkDataList[c].chunkObject.materials.Count; i < voxelObject.materialData.Count; i++)
                                chunkDataList[c].chunkObject.materials.Add(null);
                        }
                        else if (chunkDataList[c].chunkObject.materials.Count > voxelObject.materialData.Count)
                        {
                            chunkDataList[c].chunkObject.materials.RemoveRange(voxelObject.materialData.Count, chunkDataList[c].chunkObject.materials.Count - voxelObject.materialData.Count);
                        }
                        for (int i = 0; i < chunkDataList[c].chunkObject.materials.Count; i++)
                        {
                            if (!chunkDataList[c].chunkObject.materialIndexes.Contains(i))
                            {
                                if (chunkDataList[c].chunkObject.materials[i] != null)
                                {
                                    chunkDataList[c].chunkObject.materials[i] = null;
                                    DestroyUnusedObjectInPrefabObject();
                                }
                                continue;
                            }
                            if (chunkDataList[c].chunkObject.materials[i] == null)
                                chunkDataList[c].chunkObject.materials[i] = EditorCommon.CreateStandardMaterial();
                            if (!AssetDatabase.Contains(chunkDataList[c].chunkObject.materials[i]))
                            {
                                AddObjectToPrefabAsset(chunkDataList[c].chunkObject.materials[i], string.Format("{0}_mat", chunkDataList[c].chunkObject.name), i);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    Assert.IsTrue(false);
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
                        if (chunkDataList[i].chunkObject.mesh.uv.Length > 0)
                            Unwrapping.GenerateSecondaryUVSet(chunkDataList[i].chunkObject.mesh, param);
                    }
                }
                if (voxelBase.generateTangents)
                {
                    for (int i = 0; i < chunkDataList.Count; i++)
                    {
                        chunkDataList[i].chunkObject.mesh.RecalculateTangents();
                    }
                }
                for (int i = 0; i < chunkDataList.Count; i++)
                {
                    if (!AssetDatabase.Contains(chunkDataList[i].chunkObject.mesh))
                    {
                        AddObjectToPrefabAsset(chunkDataList[i].chunkObject.mesh, string.Format("{0}_mesh", chunkDataList[i].chunkObject.name));
                    }
                }
            }

            DisplayProgressBar("");

            SetRendererCompornent();
            
            RefreshCheckerSave();

            EditorUtility.ClearProgressBar();
            
            return true;
        }
        protected override void CreateMeshAfter()
        {
            chunkDataList = null;

            base.CreateMeshAfter();
        }
        public override void SetRendererCompornent()
        {
            var chunkObjects = voxelObject.chunks;
            if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Combine)
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
                for (int i = 0; i < chunkObjects.Length; i++)
                {
                    if (chunkObjects[i] == null) continue;
                    {
                        var meshFilter = chunkObjects[i].GetComponent<MeshFilter>();
                        if (meshFilter != null)
                        {
                            Undo.RecordObject(meshFilter, "Inspector");
                            meshFilter.sharedMesh = chunkObjects[i].mesh;
                        }
                    }
                    if (voxelBase.updateMeshRendererMaterials)
                    {
                        var renderer = chunkObjects[i].GetComponent<Renderer>();
                        Undo.RecordObject(renderer, "Inspector");
                        if (voxelObject.materials != null)
                        {
                            Material[] tmps = new Material[chunkObjects[i].materialIndexes.Count];
                            for (int j = 0; j < chunkObjects[i].materialIndexes.Count; j++)
                            {
                                tmps[j] = voxelObject.materials[chunkObjects[i].materialIndexes[j]];
                            }
                            renderer.sharedMaterials = tmps;
                        }
                        else
                        {
                            renderer.sharedMaterial = null;
                        }
                    }
                    if (voxelBase.loadFromVoxelFile && voxelBase.materialData != null)
                    {
                        if (voxelObject.materials != null && voxelObject.materials.Count == voxelBase.materialData.Count)
                        {
                            for (int j = 0; j < voxelObject.materials.Count; j++)
                            {
                                if (voxelObject.materials[j] == null)
                                    continue;
                                Undo.RecordObject(voxelObject.materials[j], "Inspector");
                                SetMaterialData(voxelObject.materials[j], voxelBase.materialData[j]);
                            }
                        }
                    }
                }
            }
            else if (voxelObject.materialMode == VoxelChunksObject.MaterialMode.Individual)
            {
                if (voxelBase.updateMaterialTexture)
                {
                    for (int i = 0; i < chunkObjects.Length; i++)
                    {
                        if (chunkObjects[i].materials != null)
                        {
                            for (int j = 0; j < chunkObjects[i].materials.Count; j++)
                            {
                                if (chunkObjects[i].materials[j] == null)
                                    continue;
                                Undo.RecordObject(chunkObjects[i].materials[j], "Inspector");
                                EditorCommon.SetMainTexture(chunkObjects[i].materials[j], chunkObjects[i].atlasTexture);
                            }
                        }
                    }
                }
                for (int i = 0; i < chunkObjects.Length; i++)
                {
                    {
                        var meshFilter = chunkObjects[i].GetComponent<MeshFilter>();
                        if (meshFilter != null)
                        {
                            Undo.RecordObject(meshFilter, "Inspector");
                            meshFilter.sharedMesh = chunkObjects[i].mesh;
                        }
                    }
                    if (voxelBase.updateMeshRendererMaterials)
                    {
                        var renderer = chunkObjects[i].GetComponent<Renderer>();
                        Undo.RecordObject(renderer, "Inspector");
                        if (chunkObjects[i].materials != null)
                        {
                            Material[] tmps = new Material[chunkObjects[i].materialIndexes.Count];
                            for (int j = 0; j < chunkObjects[i].materialIndexes.Count; j++)
                            {
                                tmps[j] = chunkObjects[i].materials[chunkObjects[i].materialIndexes[j]];
                            }
                            renderer.sharedMaterials = tmps;
                        }
                        else
                        {
                            renderer.sharedMaterial = null;
                        }
                    }
                    if (voxelBase.loadFromVoxelFile && voxelBase.materialData != null)
                    {
                        if (chunkObjects[i].materials != null && chunkObjects[i].materials.Count == voxelBase.materialData.Count)
                        {
                            for (int j = 0; j < chunkObjects[i].materials.Count; j++)
                            {
                                if (chunkObjects[i].materials[j] == null)
                                    continue;
                                Undo.RecordObject(chunkObjects[i].materials[j], "Inspector");
                                SetMaterialData(chunkObjects[i].materials[j], voxelBase.materialData[j]);
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
        public override Mesh[] Edit_CreateMesh(List<VoxelData.Voxel> voxels, List<Edit_VerticesInfo> dstList = null, bool combine = true)
        {
            return new Mesh[1] { Edit_CreateMeshOnly(voxels, null, dstList, combine) };
        }
        public Mesh[] Edit_CreateMesh(List<VoxelData.Voxel>[] chunkVoxels, List<Edit_VerticesInfo>[] chunkDstList = null, bool combine = true)
        {
            Assert.IsTrue(chunkVoxels.Length == chunkDataList.Count);
            var meshs = new Mesh[chunkVoxels.Length];
            for (int i = 0; i < chunkVoxels.Length; i++)
            {
                meshs[i] = Edit_CreateMeshOnly(chunkVoxels[i], chunkDataList[i].atlasRects, chunkDstList[i], combine);
            }
            return meshs;
        }
        #endregion

        #region Edit
        public override void SetSelectedWireframeHidden(bool hidden)
        {
            if (voxelObject == null) return;
            var chunkObjects = voxelObject.chunks;
            for (int i = 0; i < chunkObjects.Length; i++)
            {
                if (chunkObjects[i] == null) continue;
                var renderer = chunkObjects[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    EditorUtility.SetSelectedRenderState(renderer, hidden ? EditorSelectedRenderState.Hidden : EditorSelectedRenderState.Wireframe | EditorSelectedRenderState.Highlight);
                }
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

            if (voxelObject.chunks != null)
            {
                for (int i = 0; i < voxelObject.chunks.Length; i++)
                {
                    if (voxelObject.chunks[i] == null) continue;
                    voxelObject.chunks[i].mesh = null;
                    #region Material
                    if (voxelObject.chunks[i].materials != null)
                    {
                        for (int j = 0; j < voxelObject.chunks[i].materials.Count; j++)
                        {
                            if (voxelObject.chunks[i].materials[j] == null)
                                continue;
                            voxelObject.chunks[i].materials[j] = EditorCommon.Instantiate(voxelObject.chunks[i].materials[j]);
                        }
                    }
                    #endregion
                    voxelObject.chunks[i].atlasTexture = null;
                }
            }

            #region Structure
            voxelObject.voxelStructure = null;
            #endregion
        }
        #endregion

        #region Export
        protected override void ExportDaeFile_AddTransform(List<Transform> transforms)
        {
            base.ExportDaeFile_AddTransform(transforms);

            for (int i = 0; i < voxelObject.chunks.Length; i++)
            {
                transforms.Add(voxelObject.chunks[i].transform);
            }
        }
        #endregion

        #region Undo
        protected override void RefreshCheckerCreate() { voxelObject.refreshChecker = new VoxelChunksObject.RefreshCheckerChunk(voxelObject); }
        #endregion
    }
}
