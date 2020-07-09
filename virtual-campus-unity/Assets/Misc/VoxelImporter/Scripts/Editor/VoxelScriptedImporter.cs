#if UNITY_2017_1_OR_NEWER

using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using UnityEditor.Experimental.AssetImporters;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace VoxelImporter
{
    [ScriptedImporter(EditorDataVersion, new string[] { "vox", "qb" })]
    public class VoxelScriptedImporter : ScriptedImporter
    {
        public const int EditorDataVersion = 5;
        public int dataVersion;
        public VoxelBase.FileType fileType;
        public string gameObjectName;

        public enum MeshMode
        {
            Combine,
            Individual,
        }
        public MeshMode meshMode;
        public bool legacyVoxImport;
        public VoxelBase.ImportMode importMode = VoxelBase.ImportMode.LowPoly;
        public Vector3 importScale = Vector3.one;
        public Vector3 importOffset;
        public bool combineFaces = true;
        public bool ignoreCavity = true;
        public bool shareSameFace = true;
        public bool removeUnusedPalettes = true;
        public bool outputStructure;
        public bool generateLightmapUVs;
        public float generateLightmapUVsAngleError = 8f;
        public float generateLightmapUVsAreaError = 15f;
        public float generateLightmapUVsHardAngle = 88f;
        public float generateLightmapUVsPackMargin = 4f;
        public bool generateTangents;
        public float meshFaceVertexOffset;
        public bool loadFromVoxelFile = true;
        public bool generateMipMaps;
        public enum ColliderType
        {
            None,
            Box,
            Sphere,
            Capsule,
            Mesh,
        }
        public ColliderType colliderType;

        public bool createContactChunkFaces;
        public VoxelChunksObject.MaterialMode materialMode;

        public Material[] materials;
        public string[] materialNames;
        [Serializable]
        public class MaterialRemap
        {
            public string name;
            public Material material;
        }
        public MaterialRemap[] remappedMaterials;

        public bool export;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            {
                var ext = Path.GetExtension(ctx.assetPath).ToLower();
                if (ext == ".vox") fileType = VoxelBase.FileType.vox;
                else if (ext == ".qb") fileType = VoxelBase.FileType.qb;
                else return;
            }

            #region DefaultScale
            if (dataVersion == 0 &&
                importScale == Vector3.one &&
                AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ctx.assetPath) == null)
            {
                var x = EditorPrefs.GetFloat("VoxelImporter_DefaultScaleX", 1f);
                var y = EditorPrefs.GetFloat("VoxelImporter_DefaultScaleY", 1f);
                var z = EditorPrefs.GetFloat("VoxelImporter_DefaultScaleZ", 1f);
                importScale = new Vector3(x, y, z);
            }
            #endregion

            Action<string> LogImportError = (log) =>
            {
#if UNITY_2018_1_OR_NEWER
                ctx.LogImportError(log);
#else
                Debug.LogError(log);
#endif
            };
            Action<VoxelBase> SetBasicOptions = (voxelObject) =>
            {
                voxelObject.legacyVoxImport = legacyVoxImport;
                voxelObject.importMode = importMode;
                voxelObject.importScale = importScale;
                voxelObject.importOffset = importOffset;
                voxelObject.combineFaces = combineFaces;
                voxelObject.ignoreCavity = ignoreCavity;
                voxelObject.shareSameFace = shareSameFace;
                voxelObject.removeUnusedPalettes = removeUnusedPalettes;
                voxelObject.voxelStructure = outputStructure ? ScriptableObject.CreateInstance<VoxelStructure>() : null;
                voxelObject.generateLightmapUVs = generateLightmapUVs;
                voxelObject.generateLightmapUVsAngleError = generateLightmapUVsAngleError;
                voxelObject.generateLightmapUVsAreaError = generateLightmapUVsAreaError;
                voxelObject.generateLightmapUVsHardAngle = generateLightmapUVsHardAngle;
                voxelObject.generateLightmapUVsPackMargin = generateLightmapUVsPackMargin;
                voxelObject.generateTangents = generateTangents;
                voxelObject.meshFaceVertexOffset = meshFaceVertexOffset;
                voxelObject.loadFromVoxelFile = loadFromVoxelFile;
                voxelObject.generateMipMaps = generateMipMaps;
            };

            Action<VoxelBaseCore> Export = (core) =>
            {
                if (export)
                {
                    var fullPath = Application.dataPath + ctx.assetPath.Remove(0, "Assets".Length);
                    fullPath = fullPath.Remove(fullPath.LastIndexOf('.')) + ".dae";
                    core.ExportDaeFile(fullPath, false);
                }
            };

            var gameObject = new GameObject(Path.GetFileNameWithoutExtension(ctx.assetPath));
            if (string.IsNullOrEmpty(gameObjectName))
            {
                gameObjectName = gameObject.name;
            }
            if (meshMode == MeshMode.Combine)
            {
                #region Combine
                var voxelObject = gameObject.AddComponent<VoxelObject>();
                SetBasicOptions(voxelObject);
                var objectCore = new VoxelObjectCore(voxelObject);
                try
                {
                    if (!objectCore.Create(ctx.assetPath, null))
                    {
                        LogImportError(string.Format("<color=green>[Voxel Importer]</color> ScriptedImporter error. file:{0}", ctx.assetPath));
                        DestroyImmediate(gameObject);
                        return;
                    }
                }
                catch
                {
                    LogImportError(string.Format("<color=green>[Voxel Importer]</color> ScriptedImporter error. file:{0}", ctx.assetPath));
                    DestroyImmediate(gameObject);
                    return;
                }

                #region Correspondence in Issue ID 947055 Correction in case before correction is applied
                foreach (var material in voxelObject.materials)
                {
                    if (material != null)
                        material.hideFlags |= HideFlags.NotEditable;
                }
                if (voxelObject.atlasTexture != null)
                    voxelObject.atlasTexture.hideFlags |= HideFlags.NotEditable;
                if (voxelObject.mesh != null)
                    voxelObject.mesh.hideFlags |= HideFlags.NotEditable;
                #endregion

                #region Material
                {
                    materials = new Material[voxelObject.materialIndexes.Count];
                    materialNames = new string[voxelObject.materialIndexes.Count];
                    for (int i = 0; i < voxelObject.materialIndexes.Count; i++)
                    {
                        var index = voxelObject.materialIndexes[i];
                        var material = voxelObject.materials[index];
                        material.name = string.Format("mat{0}", index);
                        materials[i] = material;
                        materialNames[i] = material.name;
                    }
                    if (remappedMaterials != null)
                    {
                        remappedMaterials = remappedMaterials.Where(item => item.material != null && materialNames.Contains(item.name)).ToArray();
                    }
                }
                #endregion

                #region Collider
                switch (colliderType)
                {
                case ColliderType.Box:
                    gameObject.AddComponent<BoxCollider>();
                    break;
                case ColliderType.Sphere:
                    gameObject.AddComponent<SphereCollider>();
                    break;
                case ColliderType.Capsule:
                    gameObject.AddComponent<CapsuleCollider>();
                    break;
                case ColliderType.Mesh:
                    gameObject.AddComponent<MeshCollider>();
                    break;
                }
                #endregion

                Export(objectCore);

#if UNITY_2017_3_OR_NEWER
                ctx.AddObjectToAsset(gameObjectName, gameObject);
                ctx.AddObjectToAsset(voxelObject.mesh.name = "mesh", voxelObject.mesh);
                {
                    var list = new List<Material>();
                    for (int i = 0; i < voxelObject.materialIndexes.Count; i++)
                    {
                        var material = voxelObject.materials[voxelObject.materialIndexes[i]];
                        list.Add(material);
                        if (remappedMaterials != null)
                        {
                            var index = ArrayUtility.FindIndex(remappedMaterials, (t) => { return t.name == material.name; });
                            if (index >= 0)
                            {
                                list[i] = remappedMaterials[index].material;
                                continue;
                            }
                        }
                        ctx.AddObjectToAsset(material.name, material);
                    }
                    gameObject.GetComponent<MeshRenderer>().sharedMaterials = list.ToArray();
                }
                ctx.AddObjectToAsset(voxelObject.atlasTexture.name = "tex", voxelObject.atlasTexture);
                if (voxelObject.voxelStructure != null)
                {
                    ctx.AddObjectToAsset(voxelObject.voxelStructure.name = "structure", voxelObject.voxelStructure);
                }

                VoxelObject.DestroyImmediate(voxelObject);

                ctx.SetMainObject(gameObject);
#else
                ctx.SetMainAsset(gameObjectName, gameObject);
                ctx.AddSubAsset(voxelObject.mesh.name = "mesh", voxelObject.mesh);
                for (int i = 0; i < voxelObject.materialIndexes.Count; i++)
                {
                    var material = voxelObject.materials[voxelObject.materialIndexes[i]];
                    ctx.AddSubAsset(material.name, material);
                }
                ctx.AddSubAsset(voxelObject.atlasTexture.name = "tex", voxelObject.atlasTexture);
                if (voxelObject.voxelStructure != null)
                {
                    ctx.AddSubAsset(voxelObject.voxelStructure.name = "structure", voxelObject.voxelStructure);
                }

                VoxelObject.DestroyImmediate(voxelObject);
#endif
                #endregion
            }
            else if (meshMode == MeshMode.Individual)
            {
                #region Individual
                var voxelObject = gameObject.AddComponent<VoxelChunksObject>();
                SetBasicOptions(voxelObject);
                {
                    voxelObject.createContactChunkFaces = createContactChunkFaces;
                    voxelObject.materialMode = materialMode;
                }
                var objectCore = new VoxelChunksObjectCore(voxelObject);
                {
                    objectCore.chunkNameFormat = "{0}";
                }
                try
                {
                    if (!objectCore.Create(ctx.assetPath, null))
                    {
                        LogImportError(string.Format("<color=green>[Voxel Importer]</color> ScriptedImporter error. file:{0}", ctx.assetPath));
                        DestroyImmediate(gameObject);
                        return;
                    }
                }
                catch
                {
                    LogImportError(string.Format("<color=green>[Voxel Importer]</color> ScriptedImporter error. file:{0}", ctx.assetPath));
                    DestroyImmediate(gameObject);
                    return;
                }

                #region Correspondence in Issue ID 947055 Correction in case before correction is applied
                if (voxelObject.materials != null)
                {
                    foreach (var material in voxelObject.materials)
                    {
                        if (material != null)
                            material.hideFlags |= HideFlags.NotEditable;
                    }
                }
                if (voxelObject.atlasTexture != null)
                    voxelObject.atlasTexture.hideFlags |= HideFlags.NotEditable;
                foreach (var chunk in voxelObject.chunks)
                {
                    if (chunk.materials != null)
                    {
                        foreach (var material in chunk.materials)
                        {
                            if (material != null)
                                material.hideFlags |= HideFlags.NotEditable;
                        }
                    }
                    if (chunk.atlasTexture != null)
                        chunk.atlasTexture.hideFlags |= HideFlags.NotEditable;
                    if (chunk.mesh != null)
                        chunk.mesh.hideFlags |= HideFlags.NotEditable;
                }
                #endregion

                #region Material
                {
                    if (materialMode == VoxelChunksObject.MaterialMode.Combine)
                    {
                        materials = new Material[voxelObject.materialIndexes.Count];
                        materialNames = new string[voxelObject.materialIndexes.Count];
                        for (int i = 0; i < voxelObject.materialIndexes.Count; i++)
                        {
                            var index = voxelObject.materialIndexes[i];
                            var material = voxelObject.materials[index];
                            material.name = string.Format("mat{0}", index);
                            materials[i] = material;
                            materialNames[i] = material.name;
                        }
                    }
                    else if(materialMode == VoxelChunksObject.MaterialMode.Individual)
                    {
                        List<Material> list = new List<Material>();
                        foreach (var chunk in voxelObject.chunks)
                        {
                            for (int i = 0; i < chunk.materialIndexes.Count; i++)
                            {
                                var index = chunk.materialIndexes[i];
                                var material = chunk.materials[index];
                                material.name = chunk.gameObject.name + string.Format("_mat{0}", index);
                                if (!list.Contains(material))
                                {
                                    list.Add(material);
                                }
                            }
                        }
                        materials = list.ToArray();
                        materialNames = new string[list.Count];
                        for (int i = 0; i < list.Count; i++)
                            materialNames[i] = list[i].name;
                    }
                    if (remappedMaterials != null)
                    {
                        remappedMaterials = remappedMaterials.Where(item => item.material != null && materialNames.Contains(item.name)).ToArray();
                    }
                }
                #endregion

                #region Collider
                foreach (var chunk in voxelObject.chunks)
                {
                    switch (colliderType)
                    {
                    case ColliderType.Box:
                        chunk.gameObject.AddComponent<BoxCollider>();
                        break;
                    case ColliderType.Sphere:
                        chunk.gameObject.AddComponent<SphereCollider>();
                        break;
                    case ColliderType.Capsule:
                        chunk.gameObject.AddComponent<CapsuleCollider>();
                        break;
                    case ColliderType.Mesh:
                        chunk.gameObject.AddComponent<MeshCollider>();
                        break;
                    }
                }
                #endregion

                Export(objectCore);

#if UNITY_2017_3_OR_NEWER
                ctx.AddObjectToAsset(gameObjectName, gameObject);
                foreach (var chunk in voxelObject.chunks)
                {
                    ctx.AddObjectToAsset(chunk.mesh.name = chunk.gameObject.name + "_mesh", chunk.mesh);
                }
                {
                    if (materialMode == VoxelChunksObject.MaterialMode.Combine)
                    {
                        var materials = new List<Material>();
                        for (int i = 0; i < voxelObject.materialIndexes.Count; i++)
                        {
                            var material = voxelObject.materials[voxelObject.materialIndexes[i]];
                            materials.Add(material);
                            if (remappedMaterials != null)
                            {
                                var index = ArrayUtility.FindIndex(remappedMaterials, (t) => { return t.name == material.name; });
                                if (index >= 0)
                                {
                                    materials[i] = remappedMaterials[index].material;
                                    continue;
                                }
                            }
                            ctx.AddObjectToAsset(material.name, material);
                        }
                        foreach (var chunk in voxelObject.chunks)
                        {
                            chunk.gameObject.GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();
                        }
                        ctx.AddObjectToAsset(voxelObject.atlasTexture.name = "tex", voxelObject.atlasTexture);
                    }
                    else if (materialMode == VoxelChunksObject.MaterialMode.Individual)
                    {
                        foreach (var chunk in voxelObject.chunks)
                        {
                            var materials = new List<Material>();
                            for (int i = 0; i < chunk.materialIndexes.Count; i++)
                            {
                                var material = chunk.materials[chunk.materialIndexes[i]];
                                materials.Add(material);
                                if (remappedMaterials != null)
                                {
                                    var index = ArrayUtility.FindIndex(remappedMaterials, (t) => { return t.name == material.name; });
                                    if (index >= 0)
                                    {
                                        materials[i] = remappedMaterials[index].material;
                                        continue;
                                    }
                                }
                                ctx.AddObjectToAsset(material.name, material);
                            }
                            chunk.gameObject.GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();
                            ctx.AddObjectToAsset(chunk.atlasTexture.name = chunk.gameObject.name + "_tex", chunk.atlasTexture);
                        }
                    }
                }
                if (voxelObject.voxelStructure != null)
                {
                    ctx.AddObjectToAsset(voxelObject.voxelStructure.name = "structure", voxelObject.voxelStructure);
                }

                foreach (var chunk in voxelObject.chunks)
                {
                    VoxelChunksObjectChunk.DestroyImmediate(chunk.GetComponent<VoxelChunksObjectChunk>());
                }
                VoxelObject.DestroyImmediate(voxelObject);

                ctx.SetMainObject(gameObject);
#else
                ctx.SetMainAsset(gameObjectName, gameObject);
                foreach (var chunk in voxelObject.chunks)
                {
                    ctx.AddSubAsset(chunk.mesh.name = chunk.gameObject.name + "_mesh", chunk.mesh);
                }
                {
                    if (materialMode == VoxelChunksObject.MaterialMode.Combine)
                    {
                        var materials = new List<Material>();
                        for (int i = 0; i < voxelObject.materialIndexes.Count; i++)
                        {
                            var material = voxelObject.materials[voxelObject.materialIndexes[i]];
                            materials.Add(material);
                            if (remappedMaterials != null)
                            {
                                var index = ArrayUtility.FindIndex(remappedMaterials, (t) => { return t.name == material.name; });
                                if (index >= 0)
                                {
                                    materials[i] = remappedMaterials[index].material;
                                    continue;
                                }
                            }
                            ctx.AddSubAsset(material.name, material);
                        }
                        foreach (var chunk in voxelObject.chunks)
                        {
                            chunk.gameObject.GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();
                        }
                        ctx.AddSubAsset(voxelObject.atlasTexture.name = "tex", voxelObject.atlasTexture);
                    }
                    else if (materialMode == VoxelChunksObject.MaterialMode.Individual)
                    {
                        foreach (var chunk in voxelObject.chunks)
                        {
                            var materials = new List<Material>();
                            for (int i = 0; i < chunk.materialIndexes.Count; i++)
                            {
                                var material = chunk.materials[chunk.materialIndexes[i]];
                                materials.Add(material);
                                if (remappedMaterials != null)
                                {
                                    var index = ArrayUtility.FindIndex(remappedMaterials, (t) => { return t.name == material.name; });
                                    if (index >= 0)
                                    {
                                        materials[i] = remappedMaterials[index].material;
                                        continue;
                                    }
                                }
                                ctx.AddSubAsset(material.name, material);
                            }
                            chunk.gameObject.GetComponent<MeshRenderer>().sharedMaterials = materials.ToArray();
                            ctx.AddSubAsset(chunk.atlasTexture.name = chunk.gameObject.name + "_tex", chunk.atlasTexture);
                        }
                    }
                }
                if (voxelObject.voxelStructure != null)
                {
                    ctx.AddSubAsset(voxelObject.voxelStructure.name = "structure", voxelObject.voxelStructure);
                }

                foreach (var chunk in voxelObject.chunks)
                {
                    VoxelChunksObjectChunk.DestroyImmediate(chunk.GetComponent<VoxelChunksObjectChunk>());
                }
                VoxelObject.DestroyImmediate(voxelObject);
#endif
                #endregion
            }

            dataVersion = EditorDataVersion;
        }
    }
}
#endif