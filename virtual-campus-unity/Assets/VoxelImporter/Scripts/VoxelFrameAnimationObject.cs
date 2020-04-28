using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [AddComponentMenu("Voxel Importer/Voxel Frame Animation Object")]
    [RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class VoxelFrameAnimationObject : VoxelBase
    {
        [Serializable]
        public class FrameData
        {
#if UNITY_EDITOR
            public string voxelFilePath;
            public UnityEngine.Object voxelFileObject;
            public int voxelFileSubIndex;
            public FileType fileType;
            public Vector3 localOffset;

            [NonSerialized]
            public VoxelData voxelData;
            [NonSerialized]
            public long voxelDataCreatedVoxelFileTimeTicks;

            public DisableData disableData;
            public List<MaterialData> materialData;

            [NonSerialized]
            public Texture2D icon;
#endif
            public string name;
            public Mesh mesh;
            public List<int> materialIndexes;
        }

        public List<FrameData> frames;

        public Mesh mesh;
        public List<Material> materials;
        public Texture2D atlasTexture;

        public Material playMaterial0;
        public Material playMaterial1;
        public Material playMaterial2;
        public Material playMaterial3;
        public Material playMaterial4;
        public Material playMaterial5;
        public Material playMaterial6;
        public Material playMaterial7;

        //Play
        private MeshFilter meshFilterCache;
        private Mesh filterMesh;
        private MeshRenderer meshRendererCache;
        private Material[] rendererMaterials;
        private Material rendererMaterial0;
        private Material rendererMaterial1;
        private Material rendererMaterial2;
        private Material rendererMaterial3;
        private Material rendererMaterial4;
        private Material rendererMaterial5;
        private Material rendererMaterial6;
        private Material rendererMaterial7;

        void Awake()
        {
            meshFilterCache = GetComponent<MeshFilter>();
            filterMesh = null;
            meshRendererCache = GetComponent<MeshRenderer>();
            rendererMaterials = null;
        }

        void LateUpdate()
        {
            #region Mesh
            if (filterMesh != mesh)
                meshFilterCache.sharedMesh = filterMesh = mesh;
            #endregion

            #region Material
            if (rendererMaterial0 != playMaterial0 ||
                rendererMaterial1 != playMaterial1 ||
                rendererMaterial2 != playMaterial2 ||
                rendererMaterial3 != playMaterial3 ||
                rendererMaterial4 != playMaterial4 ||
                rendererMaterial5 != playMaterial5 ||
                rendererMaterial6 != playMaterial6 ||
                rendererMaterial7 != playMaterial7)
            {
                int count = 0;
                if (playMaterial0 != null) count++;
                if (playMaterial1 != null) count++;
                if (playMaterial2 != null) count++;
                if (playMaterial3 != null) count++;
                if (playMaterial4 != null) count++;
                if (playMaterial5 != null) count++;
                if (playMaterial6 != null) count++;
                if (playMaterial7 != null) count++;
                //
                if (rendererMaterials == null || rendererMaterials.Length != count)
                    rendererMaterials = new Material[count];
                //
                int i = 0;
                if (playMaterial0 != null) rendererMaterials[i++] = playMaterial0;
                if (playMaterial1 != null) rendererMaterials[i++] = playMaterial1;
                if (playMaterial2 != null) rendererMaterials[i++] = playMaterial2;
                if (playMaterial3 != null) rendererMaterials[i++] = playMaterial3;
                if (playMaterial4 != null) rendererMaterials[i++] = playMaterial4;
                if (playMaterial5 != null) rendererMaterials[i++] = playMaterial5;
                if (playMaterial6 != null) rendererMaterials[i++] = playMaterial6;
                if (playMaterial7 != null) rendererMaterials[i++] = playMaterial7;
                //
                if (rendererMaterials.Length == 0)
                    meshRendererCache.sharedMaterial = null;
                else
                    meshRendererCache.sharedMaterials = rendererMaterials;
                //
                rendererMaterial0 = playMaterial0;
                rendererMaterial1 = playMaterial1;
                rendererMaterial2 = playMaterial2;
                rendererMaterial3 = playMaterial3;
                rendererMaterial4 = playMaterial4;
                rendererMaterial5 = playMaterial5;
                rendererMaterial6 = playMaterial6;
                rendererMaterial7 = playMaterial7;
            }
            #endregion
        }

        public void ClearFrame()
        {
            mesh = null;
            playMaterial0 = null;
            playMaterial1 = null;
            playMaterial2 = null;
            playMaterial3 = null;
            playMaterial4 = null;
            playMaterial5 = null;
            playMaterial6 = null;
            playMaterial7 = null;
        }
        public bool ChangeFrame(string frameName)
        {
            if (!string.IsNullOrEmpty(frameName))
            {
                foreach (var frame in frames)
                {
                    if (frame.name == frameName)
                    {
                        ClearFrame();
                        mesh = frame.mesh;
                        {
                            for (int i = 0; i < frame.materialIndexes.Count; i++)
                            {
                                var index = frame.materialIndexes[i];
                                switch (i)
                                {
                                case 0: playMaterial0 = materials[index]; break;
                                case 1: playMaterial1 = materials[index]; break;
                                case 2: playMaterial2 = materials[index]; break;
                                case 3: playMaterial3 = materials[index]; break;
                                case 4: playMaterial4 = materials[index]; break;
                                case 5: playMaterial5 = materials[index]; break;
                                case 6: playMaterial6 = materials[index]; break;
                                case 7: playMaterial7 = materials[index]; break;
                                default:
                                    Debug.LogWarningFormat("<color=green>[Voxel Importer]</color> Play material count over. <color=red>{0}/{1}</color>", frame.materialIndexes.Count, 8);
                                    break;
                                }
                            }
                        }
                        return true;
                    }
                }
            }
            Debug.LogWarningFormat("<color=green>[Voxel Importer]</color> Frame not found. <color=red>{0}</color>", frameName);
            return false;
        }

#if UNITY_EDITOR
        public override bool EditorInitialize()
        {
            var result = base.EditorInitialize();

            //ver1.1.2
            if (frames != null)
            {
                foreach (var frame in frames)
                {
                    if (frame.voxelFileObject == null && !string.IsNullOrEmpty(frame.voxelFilePath) && frame.voxelFilePath.Contains("Assets/"))
                    {
                        var assetPath = frame.voxelFilePath.Substring(frame.voxelFilePath.IndexOf("Assets/"));
                        var fullPath = Application.dataPath + "/" + assetPath.Remove(0, "Assets/".Length);
                        if (File.Exists(fullPath))
                        {
                            frame.voxelFileObject = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                        }
                    }
                }
            }
            //ver1.1.4
            if (dataVersion < 114)
            {
                if (frames != null)
                {
                    foreach (var frame in frames)
                    {
                        if (string.IsNullOrEmpty(frame.name))
                        {
                            if (frames.FindIndex((x) => x != frame && x.voxelFilePath == frame.voxelFilePath) >= 0)
                            {
                                frame.name = Edit_GetUniqueFrameName(string.Format("{0}_{1}", Path.GetFileNameWithoutExtension(frame.voxelFilePath), frame.voxelFileSubIndex));
                            }
                            else
                            {
                                frame.name = Edit_GetUniqueFrameName(Path.GetFileNameWithoutExtension(frame.voxelFilePath));
                            }
                        }
                        if (enableFaceFlags != FaceAllFlags && voxelData != null)
                        {
                            frame.disableData = new DisableData();
                            var face = ~enableFaceFlags;
                            foreach (var voxel in frame.voxelData.voxels)
                            {
                                var visible = voxel.visible & face;
                                if (visible == 0) continue;
                                frame.disableData.SetDisable(voxel.position, visible);
                            }
                        }
                    }
                }
            }

            return result;
        }

        [NonSerialized]
        public bool edit_animationFoldout = true;

        [NonSerialized]
        public int edit_frameIndex = -1;

        public bool edit_frameEnable { get { return frames != null && edit_frameIndex >= 0 && edit_frameIndex < frames.Count && frames[edit_frameIndex] != null; } }
        public FrameData edit_currentFrame { get { return edit_frameEnable ? frames[edit_frameIndex] : null; } }

        public enum Edit_CameraMode
        {
            forward,
            back,
            up,
            down,
            right,
            left,
        }
        [NonSerialized]
        public Edit_CameraMode edit_previewCameraMode;

        public void Edit_SetFrameCurrentVoxelOtherData()
        {
            if (!edit_frameEnable) return;

            voxelData = edit_currentFrame.voxelData;
            disableData = edit_currentFrame.disableData;
            materialData = edit_currentFrame.materialData;
            materialIndexes = edit_currentFrame.materialIndexes;
        }

        public void Edit_ClearPlayMaterials()
        {
            playMaterial0 = null;
            playMaterial1 = null;
            playMaterial2 = null;
            playMaterial3 = null;
            playMaterial4 = null;
            playMaterial5 = null;
            playMaterial6 = null;
            playMaterial7 = null;
        }
        public void Edit_SetPlayMaterials(Material[] mats)
        {
            Edit_ClearPlayMaterials();
            if (mats.Length > 0) playMaterial0 = mats[0];
            if (mats.Length > 1) playMaterial1 = mats[1];
            if (mats.Length > 2) playMaterial2 = mats[2];
            if (mats.Length > 3) playMaterial3 = mats[3];
            if (mats.Length > 4) playMaterial4 = mats[4];
            if (mats.Length > 5) playMaterial5 = mats[5];
            if (mats.Length > 6) playMaterial6 = mats[6];
            if (mats.Length > 7) playMaterial7 = mats[7];
            if (mats.Length > 8)
            {
                Debug.LogWarningFormat("<color=green>[Voxel Importer]</color> Play material count over. <color=red>{0}/{1}</color>", mats.Length, 8);
            }
        }

        public string Edit_GetUniqueFrameName(string name)
        {
            var result = name;
            bool unique;
            do
            {
                unique = true;
                foreach (var frame in frames)
                {
                    if (frame.name == result)
                    {
                        result += " 0";
                        unique = false;
                        break;
                    }
                }
            } while (!unique);
            return result;
        }

        #region Asset
        public override bool IsUseAssetObject(UnityEngine.Object obj)
        {
            if (mesh == obj) return true;
            if (materials != null)
            {
                for (int i = 0; i < materials.Count; i++)
                {
                    if (materials[i] == obj) return true;
                }
            }
            if (atlasTexture == obj) return true;

            if (frames != null)
            {
                for (int i = 0; i < frames.Count; i++)
                {
                    if (frames[i] == null) continue;
                    if (frames[i].mesh == obj) return true;
                }
            }

            return false;
        }
        #endregion

        #region Undo
        public class RefreshCheckerFrameAnimation : RefreshChecker
        {
            public RefreshCheckerFrameAnimation(VoxelFrameAnimationObject voxelObject) : base(voxelObject)
            {
                controllerFrameAnimation = voxelObject;
            }

            public VoxelFrameAnimationObject controllerFrameAnimation;

            public override void Save()
            {
                base.Save();
            }
            public override bool Check()
            {
                if (base.Check())
                    return true;

                return false;
            }
        }
        #endregion
#endif
    }
}
