using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelImporter
{
    public class VoxelObjectCore : VoxelBaseCore
    {
        public VoxelObjectCore(VoxelBase target) : base(target)
        {
            voxelObject = target as VoxelObject;
        }

        public VoxelObject voxelObject { get; protected set; }

        public virtual Mesh mesh { get { return voxelObject.mesh; } set { voxelObject.mesh = value; } }
        public virtual List<Material> materials { get { return voxelObject.materials; } set { voxelObject.materials = value; } }
        public virtual Texture2D atlasTexture { get { return voxelObject.atlasTexture; } set { voxelObject.atlasTexture = value; } }

        #region AtlasRects
        protected Rect[] atlasRects;
        protected AtlasRectTable atlasRectTable;
        #endregion

        #region FaceArea
        protected VoxelData.FaceAreaTable faceAreaTable;
        #endregion

        #region CreateVoxel
        public override string GetDefaultPath()
        {
            var path = base.GetDefaultPath();
            if (mesh != null && AssetDatabase.Contains(mesh))
            {
                var assetPath = AssetDatabase.GetAssetPath(mesh);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    path = Path.GetDirectoryName(assetPath);
                }
            }
            if (materials != null)
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i] == null)
                        continue;
                    if (AssetDatabase.Contains(materials[i]))
                    {
                        var assetPath = AssetDatabase.GetAssetPath(materials[i]);
                        if (!string.IsNullOrEmpty(assetPath))
                        {
                            path = Path.GetDirectoryName(assetPath);
                        }
                    }
                }
            }
            if (atlasTexture != null && AssetDatabase.Contains(atlasTexture))
            {
                var assetPath = AssetDatabase.GetAssetPath(atlasTexture);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    path = Path.GetDirectoryName(assetPath);
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
            const float MaxProgressCount = 7f;
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
                if (voxelBase.disableData == null)
                    voxelBase.disableData = new DisableData();
                #region Erase
                {
                    List<IntVector3> removeList = new List<IntVector3>();
                    voxelBase.disableData.AllAction((pos, face) =>
                    {
                        if (voxelData.VoxelTableContains(pos) < 0)
                        {
                            removeList.Add(pos);
                        }
                    });
                    for (int j = 0; j < removeList.Count; j++)
                    {
                        voxelBase.disableData.RemoveDisable(removeList[j]);
                    }
                }
                #endregion
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
                #region Erase
                for (int i = 0; i < voxelBase.materialData.Count; i++)
                {
                    List<IntVector3> removeList = new List<IntVector3>();
                    voxelBase.materialData[i].AllAction((pos) =>
                    {
                        if (voxelData.VoxelTableContains(pos) < 0)
                        {
                            removeList.Add(pos);
                        }
                    });
                    for (int j = 0; j < removeList.Count; j++)
                    {
                        voxelBase.materialData[i].RemoveMaterial(removeList[j]);
                    }
                }
                #endregion
                if (materials == null)
                    materials = new List<Material>();
                if (materials.Count < voxelBase.materialData.Count)
                {
                    for (int i = materials.Count; i < voxelBase.materialData.Count; i++)
                        materials.Add(null);
                }
                else if (materials.Count > voxelBase.materialData.Count)
                {
                    materials.RemoveRange(voxelBase.materialData.Count, materials.Count - voxelBase.materialData.Count);
                }
            }
            voxelBase.CreateMaterialIndexTable();
            #endregion

            CalcDataCreate(voxelData.voxels);

            faceAreaTable = CreateFaceArea(voxelData.voxels);

            DisplayProgressBar("");
            {
                var atlasTextureTmp = atlasTexture;
                if (!CreateTexture(faceAreaTable, voxelData.palettes, ref atlasRectTable, ref atlasTextureTmp, ref atlasRects))
                {
                    EditorUtility.ClearProgressBar();
                    return false;
                }
                atlasTexture = atlasTextureTmp;
                if (!AssetDatabase.Contains(atlasTexture))
                {
                    AddObjectToPrefabAsset(atlasTexture, "tex");
                }
            }

            DisplayProgressBar("");
            {
                mesh = CreateMeshOnly(mesh, faceAreaTable, atlasTexture, atlasRects, atlasRectTable, Vector3.zero, out voxelBase.materialIndexes);
            }

            DisplayProgressBar("");
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (!voxelBase.materialIndexes.Contains(i))
                    {
                        if (materials[i] != null)
                        {
                            materials[i] = null;
                            DestroyUnusedObjectInPrefabObject();
                        }
                        continue;
                    }
                    if (materials[i] == null)
                        materials[i] = EditorCommon.CreateStandardMaterial();
                    else if (!AssetDatabase.Contains(materials[i]))
                    {
                        var tmp = Material.Instantiate(materials[i]);
                        tmp.name = materials[i].name;
                        materials[i] = tmp;
                    }
                    if (!AssetDatabase.Contains(materials[i]))
                    {
                        AddObjectToPrefabAsset(materials[i], "mat", i);
                    }
                }
            }

            DisplayProgressBar("");
            {
                if (voxelBase.generateLightmapUVs && mesh.uv.Length > 0)
                {
                    Unwrapping.GenerateSecondaryUVSet(mesh, voxelBase.GetLightmapParam());
                }
                if (voxelBase.generateTangents)
                {
                    mesh.RecalculateTangents();
                }
                if (!AssetDatabase.Contains(mesh))
                {
                    AddObjectToPrefabAsset(mesh, "mesh");
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
            atlasRects = null;
            atlasRectTable = null;
            faceAreaTable = null;

            base.CreateMeshAfter();
        }
        public override void SetRendererCompornent()
        {
            {
                var meshFilter = voxelBase.GetComponent<MeshFilter>();
                if (meshFilter != null)
                {
                    Undo.RecordObject(meshFilter, "Inspector");
                    meshFilter.sharedMesh = mesh;
                }
            }
            if (voxelBase.updateMaterialTexture)
            {
                if (materials != null)
                {
                    for (int i = 0; i < materials.Count; i++)
                    {
                        if (materials[i] == null)
                            continue;
                        Undo.RecordObject(materials[i], "Inspector");
                        EditorCommon.SetMainTexture(materials[i], atlasTexture);
                    }
                }
            }
            if (voxelBase.updateMeshRendererMaterials)
            {
                var renderer = voxelBase.GetComponent<Renderer>();
                Undo.RecordObject(renderer, "Inspector");
                if (materials != null)
                {
                    Material[] tmps = new Material[voxelBase.materialIndexes.Count];
                    for (int i = 0; i < voxelBase.materialIndexes.Count; i++)
                    {
                        tmps[i] = materials[voxelBase.materialIndexes[i]];
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
                if (materials != null && materials.Count == voxelBase.materialData.Count)
                {
                    for (int i = 0; i < materials.Count; i++)
                    {
                        if (materials[i] == null)
                            continue;
                        Undo.RecordObject(materials[i], "Inspector");
                        SetMaterialData(materials[i], voxelBase.materialData[i]);
                    }
                }
            }
        }
        public override Mesh[] Edit_CreateMesh(List<VoxelData.Voxel> voxels, List<Edit_VerticesInfo> dstList = null, bool combine = true)
        {
            return new Mesh[1] { Edit_CreateMeshOnly(voxels, atlasRects, dstList, combine) };
        }
        #endregion

        #region Asset
        public override void ResetAllAssets()
        {
            #region Mesh
            voxelObject.mesh = null;
            #endregion

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

            #region Structure
            voxelObject.voxelStructure = null;
            #endregion
        }
        #endregion
    }
}
