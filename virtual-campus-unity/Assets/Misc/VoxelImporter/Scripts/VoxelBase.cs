using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [DisallowMultipleComponent]
    public abstract class VoxelBase : MonoBehaviour
    {
        [Flags]
        public enum Face
        {
            forward = 1 << 0,
            up = 1 << 1,
            right = 1 << 2,
            left = 1 << 3,
            down = 1 << 4,
            back = 1 << 5,
        }

#if UNITY_EDITOR
        public bool advancedMode;
        //Voxel
        public string voxelFilePath;
        public string voxelFileGUID;    
        public UnityEngine.Object voxelFileObject;
        public int voxelFileSubIndex;
        public enum FileType
        {
            vox,
            qb,
            png,
        }
        public FileType fileType;
        public long fileRefreshLastTimeTicks;
        public int dataVersion;
        public bool legacyVoxImport;
        public enum ImportMode
        {
            LowTexture,
            LowPoly,
        }
        public ImportMode importMode = ImportMode.LowPoly;
        [Flags]
        public enum ImportFlag
        {
            FlipX = 1 << 0,
            FlipY = 1 << 1,
            FlipZ = 1 << 2,
        }
        public ImportFlag importFlags;
        public Vector3 importScale = Vector3.one;
        public Vector3 importOffset;
        public Vector3 localOffset;

        public const Face FaceAllFlags = (Face)(-1);
        public Face enableFaceFlags = FaceAllFlags;
        public DisableData disableData;

        public bool combineFaces = true;
        public bool ignoreCavity = true;
        public bool shareSameFace = true;
        public bool removeUnusedPalettes = true;

        public VoxelStructure voxelStructure;

        //Mesh
        public bool generateLightmapUVs;
        public float generateLightmapUVsAngleError = 8f;
        public float generateLightmapUVsAreaError = 15f;
        public float generateLightmapUVsHardAngle = 88f;
        public float generateLightmapUVsPackMargin = 4f;
        public bool generateTangents;
        public float meshFaceVertexOffset;
        public UnityEditor.UnwrapParam GetLightmapParam()
        {
            UnityEditor.UnwrapParam param;
            UnityEditor.UnwrapParam.SetDefaults(out param);
            param.angleError = generateLightmapUVsAngleError * 0.01f;
            param.areaError = generateLightmapUVsAreaError * 0.01f;
            param.hardAngle = generateLightmapUVsHardAngle;
            param.packMargin = generateLightmapUVsPackMargin / 1024f;
            return param;
        }

        //Material
        public bool generateMipMaps;
        public bool updateMeshRendererMaterials = true;
        public bool updateMaterialTexture = true;
        public bool loadFromVoxelFile = true;
        public List<MaterialData> materialData;
        public List<int> materialIndexes;

        //Voxel
        public enum VoxelVertexIndex
        {
            XYZ,
            _XYZ,
            X_YZ,
            XY_Z,
            _X_YZ,
            _XY_Z,
            X_Y_Z,
            _X_Y_Z,
            Total
        }
        [Flags]
        public enum VoxelVertexFlags
        {
            XYZ = (1 << 0),
            _XYZ = (1 << 1),
            X_YZ = (1 << 2),
            XY_Z = (1 << 3),
            _X_YZ = (1 << 4),
            _XY_Z = (1 << 5),
            X_Y_Z = (1 << 6),
            _X_Y_Z = (1 << 7),
        }
        public struct VoxelVertices
        {
            public Vector3 vertexXYZ;
            public Vector3 vertex_XYZ;
            public Vector3 vertexX_YZ;
            public Vector3 vertexXY_Z;
            public Vector3 vertex_X_YZ;
            public Vector3 vertex_XY_Z;
            public Vector3 vertexX_Y_Z;
            public Vector3 vertex_X_Y_Z;

            public void SetVertex(VoxelVertexIndex index, Vector3 vertex)
            {
                switch (index)
                {
                case VoxelVertexIndex.XYZ: vertexXYZ = vertex; break;
                case VoxelVertexIndex._XYZ: vertex_XYZ = vertex; break;
                case VoxelVertexIndex.X_YZ: vertexX_YZ = vertex; break;
                case VoxelVertexIndex.XY_Z: vertexXY_Z = vertex; break;
                case VoxelVertexIndex._X_YZ: vertex_X_YZ = vertex; break;
                case VoxelVertexIndex._XY_Z: vertex_XY_Z = vertex; break;
                case VoxelVertexIndex.X_Y_Z: vertexX_Y_Z = vertex; break;
                case VoxelVertexIndex._X_Y_Z: vertex_X_Y_Z = vertex; break;
                default: Assert.IsTrue(false); break;
                }
            }
            public Vector3 GetVertex(VoxelVertexIndex index)
            {
                switch (index)
                {
                case VoxelVertexIndex.XYZ: return vertexXYZ;
                case VoxelVertexIndex._XYZ: return vertex_XYZ;
                case VoxelVertexIndex.X_YZ: return vertexX_YZ;
                case VoxelVertexIndex.XY_Z: return vertexXY_Z;
                case VoxelVertexIndex._X_YZ: return vertex_X_YZ;
                case VoxelVertexIndex._XY_Z: return vertex_XY_Z;
                case VoxelVertexIndex.X_Y_Z: return vertexX_Y_Z;
                case VoxelVertexIndex._X_Y_Z: return vertex_X_Y_Z;
                default: Assert.IsTrue(false); return Vector3.zero;
                }
            }
        }

        //Calc
        [NonSerialized]
        public VoxelData voxelData;
        [NonSerialized]
        public long voxelDataCreatedVoxelFileTimeTicks;

        #region MaterialIndexTable
        [NonSerialized]
        protected DataTable3<int> materialIndexTable;
        public void CreateMaterialIndexTable()
        {
            materialIndexTable = null;
            if (voxelData == null || materialData == null) return;
            materialIndexTable = new DataTable3<int>(voxelData.voxelSize.x, voxelData.voxelSize.y, voxelData.voxelSize.z);
            for (int i = 1; i < materialData.Count; i++)
            {
                materialData[i].AllAction((pos) =>
                {
                    materialIndexTable.Set(pos, i);
                });
            }
        }
        public int GetMaterialIndexTable(IntVector3 pos)
        {
            if (materialIndexTable == null)
                CreateMaterialIndexTable();
            return materialIndexTable.Get(pos);
        }
        public int GetMaterialIndexTable(int x, int y, int z)
        {
            if (materialIndexTable == null)
                CreateMaterialIndexTable();
            return materialIndexTable.Get(x, y, z);
        }
        #endregion

        #region Editor
        public const int EditorDataVersion = 114;
        public virtual bool EditorInitialize()
        {
            //ver1.0.4
            if (fileRefreshLastTimeTicks == 0)
            {
                fileRefreshLastTimeTicks = DateTime.Now.Ticks;
            }
            //ver1.0.5
            if (!string.IsNullOrEmpty(voxelFileGUID) && voxelFileObject == null)
            {
                {
                    var assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(voxelFileGUID);
                    if (!string.IsNullOrEmpty(assetPath))
                    {
                        voxelFileObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                    }
                }
            }
            voxelFileGUID = null;
            //ver1.1.2
            if (voxelFileObject == null && !string.IsNullOrEmpty(voxelFilePath) && voxelFilePath.Contains("Assets/"))
            {
                var assetPath = voxelFilePath.Substring(voxelFilePath.IndexOf("Assets/"));
                var fullPath = Application.dataPath + "/" + assetPath.Remove(0, "Assets/".Length);
                if (File.Exists(fullPath))
                {
                    voxelFileObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                }
            }
            //ver1.1.3
            if (dataVersion < 113 && !string.IsNullOrEmpty(voxelFilePath))
            {
                switch (fileType)
                {
                case FileType.vox:
                    legacyVoxImport = true;
                    break;
                }
            }
            //ver1.1.4
            if (dataVersion < 114 && enableFaceFlags != FaceAllFlags && voxelData != null)
            {
                disableData = new DisableData();
                var face = ~enableFaceFlags;
                foreach (var voxel in voxelData.voxels)
                {
                    var visible = voxel.visible & face;
                    if (visible == 0) continue;
                    disableData.SetDisable(voxel.position, visible);
                }
            }

            #region DefaultScale
            if (voxelFileObject == null && string.IsNullOrEmpty(voxelFilePath))
            {
                var x = UnityEditor.EditorPrefs.GetFloat("VoxelImporter_DefaultScaleX", 1f);
                var y = UnityEditor.EditorPrefs.GetFloat("VoxelImporter_DefaultScaleY", 1f);
                var z = UnityEditor.EditorPrefs.GetFloat("VoxelImporter_DefaultScaleZ", 1f);
                importScale = new Vector3(x, y, z);
            }
            #endregion

            return false;
        }
        public void EditorInitializeDone()
        {
            enableFaceFlags = FaceAllFlags;
            dataVersion = EditorDataVersion;
        }

        public bool edit_importFoldout = true;
        public bool edit_objectFoldout = true;
        public bool edit_generateLightmapUVsAdvancedFoldout = false;

        [NonSerialized]
        public bool edit_afterRefresh = false;

        public bool edit_helpEnable = false;

        public enum Edit_ConfigureMode
        {
            None,
            Material,
            Disable,
        }
        public Edit_ConfigureMode edit_configureMode;

        public int edit_configureMaterialIndex;
        public enum Edit_ConfigureMaterialMainMode
        {
            Add,
            Remove,
        }
        public Edit_ConfigureMaterialMainMode edit_configureMaterialMainMode;
        public enum Edit_ConfigureMaterialSubMode
        {
            Voxel,
            Fill,
            Rect,
        }
        public Edit_ConfigureMaterialSubMode edit_configureMaterialSubMode;

        public enum Edit_ConfigureDisableMainMode
        {
            Add,
            Remove,
        }
        public Edit_ConfigureDisableMainMode edit_configureDisableMainMode;
        public enum Edit_ConfigureDisableSubMode
        {
            Face,
            Fill,
            Rect,
        }
        public Edit_ConfigureDisableSubMode edit_configureDisableSubMode;

        public enum Edit_ConfigurePreviewMode
        {
            Opaque,
            Transparent,
        }
        public Edit_ConfigurePreviewMode edit_ConfigurePreviewMode;

        public bool edit_snapToHalfVoxel = true;

        public Mesh[] edit_enableMesh = null;
        public virtual void SaveEditTmpData() { }
        #endregion

        #region Asset
        public virtual bool IsUseAssetObject(UnityEngine.Object obj)
        {
            return false;
        }
        #endregion

        #region Undo
        public class RefreshChecker
        {
            public RefreshChecker(VoxelBase voxelBase)
            {
                controller = voxelBase;
            }

            public VoxelBase controller;

            public string voxelFilePath;
            public string voxelFileGUID;
            public UnityEngine.Object voxelFileObject;
            public int voxelFileSubIndex;
            public bool legacyVoxImport;
            public ImportMode importMode;
            public ImportFlag importFlags;
            public Vector3 importScale;
            public Vector3 importOffset;
            public Face enableFaceFlags;
            public DisableData disableData;
            public bool generateLightmapUVs;
            public float generateLightmapUVsAngleError;
            public float generateLightmapUVsAreaError;
            public float generateLightmapUVsHardAngle;
            public float generateLightmapUVsPackMargin;
            public bool generateTangents;
            public float meshFaceVertexOffset;
            public bool generateMipMaps;
            public MaterialData[] materialData;
            public int[] materialIndexes;

            public virtual void Save()
            {
                voxelFilePath = controller.voxelFilePath;
                voxelFileGUID = controller.voxelFileGUID;
                voxelFileObject = controller.voxelFileObject;
                voxelFileSubIndex = controller.voxelFileSubIndex;
                legacyVoxImport = controller.legacyVoxImport;
                importMode = controller.importMode;
                importFlags = controller.importFlags;
                importScale = controller.importScale;
                importOffset = controller.importOffset;
                enableFaceFlags = controller.enableFaceFlags;
                generateLightmapUVs = controller.generateLightmapUVs;
                generateLightmapUVsAngleError = controller.generateLightmapUVsAngleError;
                generateLightmapUVsAreaError = controller.generateLightmapUVsAreaError;
                generateLightmapUVsHardAngle = controller.generateLightmapUVsHardAngle;
                generateLightmapUVsPackMargin = controller.generateLightmapUVsPackMargin;
                generateTangents = controller.generateTangents;
                meshFaceVertexOffset = controller.meshFaceVertexOffset;
                generateMipMaps = controller.generateMipMaps;
                if (controller.disableData != null)
                    disableData = controller.disableData.Clone();
                else
                    disableData = null;
                if (controller.materialData != null)
                {
                    materialData = new MaterialData[controller.materialData.Count];
                    for (int i = 0; i < controller.materialData.Count; i++)
                    {
                        if (controller.materialData[i] != null)
                            materialData[i] = controller.materialData[i].Clone();
                    }
                }
                else
                {
                    materialData = null;
                }
                materialIndexes = controller.materialIndexes != null ? controller.materialIndexes.ToArray() : null;
            }
            public virtual bool Check()
            {
                if (voxelFilePath != controller.voxelFilePath ||
                    voxelFileObject != controller.voxelFileObject ||
                    voxelFileSubIndex != controller.voxelFileSubIndex ||
                    legacyVoxImport != controller.legacyVoxImport ||
                    importMode != controller.importMode ||
                    importFlags != controller.importFlags ||
                    importScale != controller.importScale ||
                    importOffset != controller.importOffset ||
                    enableFaceFlags != controller.enableFaceFlags ||
                    generateLightmapUVs != controller.generateLightmapUVs ||
                    generateLightmapUVsAngleError != controller.generateLightmapUVsAngleError ||
                    generateLightmapUVsAreaError != controller.generateLightmapUVsAreaError ||
                    generateLightmapUVsHardAngle != controller.generateLightmapUVsHardAngle ||
                    generateLightmapUVsPackMargin != controller.generateLightmapUVsPackMargin ||
                    generateTangents != controller.generateTangents ||
                    meshFaceVertexOffset != controller.meshFaceVertexOffset ||
                    generateMipMaps != controller.generateMipMaps)
                    return true;
                #region disableData
                if (disableData != null && controller.disableData != null)
                {
                    if (!disableData.IsEqual(controller.disableData))
                        return true;
                }
                else if (disableData != null && controller.disableData == null)
                {
                    return true;
                }
                else if (disableData == null && controller.disableData != null)
                {
                    return true;
                }
                #endregion
                #region materialData
                if (materialData != null && controller.materialData != null)
                {
                    if (materialData.Length != controller.materialData.Count)
                        return true;
                    for (int i = 0; i < materialData.Length; i++)
                    {
                        if (!materialData[i].IsEqual(controller.materialData[i]))
                            return true;
                    }
                }
                else if (materialData != null && controller.materialData == null)
                {
                    return true;
                }
                else if (materialData == null && controller.materialData != null)
                {
                    return true;
                }
                #endregion
                #region materialIndexes
                if (materialIndexes != null && controller.materialIndexes != null)
                {
                    if (materialIndexes.Length != controller.materialIndexes.Count)
                        return true;
                    for (int i = 0; i < materialIndexes.Length; i++)
                    {
                        if (materialIndexes[i] != controller.materialIndexes[i])
                            return true;
                    }
                }
                else if (materialIndexes != null && controller.materialIndexes == null)
                {
                    return true;
                }
                else if (materialIndexes == null && controller.materialIndexes != null)
                {
                    return true;
                }
                #endregion

                return false;
            }
        }
        [NonSerialized]
        public RefreshChecker refreshChecker;
        #endregion

        #region Event
        public bool IsNeedStructureData()
        {
            return (onBeforeCreateMesh != null);
        }

        [NonSerialized]
        public StructureData structureData;

        public class OnBeforeCreateMeshData
        {
            public List<Vector3> vertices;
            public List<Vector2> uv;
            public List<Vector3> normals;
            public List<BoneWeight> boneWeights;
            public List<int>[] triangles;
        }

        [NonSerialized]
        public Action<OnBeforeCreateMeshData> onBeforeCreateMesh;
        #endregion
#endif
    }
}
