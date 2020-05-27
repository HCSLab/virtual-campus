using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

#if UNITY_2018_3_OR_NEWER
using UnityEditor.Experimental.SceneManagement;
#endif

namespace VoxelImporter
{
    public sealed class EditorCommon
    {
        public static void SaveInsideAssetsFolderDisplayDialog()
        {
            EditorUtility.DisplayDialog("Need to save in the Assets folder", "You need to save the file inside of the project's assets floder", "ok");
        }

        public static string GetHelpStrings(List<string> helpList)
        {
            if (helpList.Count > 0)
            {
                string text = "";
                if (helpList.Count >= 3)
                {
                    int i = 0;
                    var enu = helpList.GetEnumerator();
                    while (enu.MoveNext())
                    {
                        if (i == helpList.Count - 1)
                            text += ", and ";
                        else if (i != 0)
                            text += ", ";
                        text += enu.Current;
                        i++;
                    }
                }
                else if (helpList.Count == 2)
                {
                    var enu = helpList.GetEnumerator();
                    enu.MoveNext();
                    text += enu.Current;
                    text += " and ";
                    enu.MoveNext();
                    text += enu.Current;
                }
                else if (helpList.Count == 1)
                {
                    var enu = helpList.GetEnumerator();
                    enu.MoveNext();
                    text += enu.Current;
                }
                return string.Format("If it is not Prefab you need to save the file.\nPlease create a Prefab for this GameObject.\nIf you do not want to Prefab, please save {0}.", text);
            }
            return null;
        }

        public static bool IsMainAsset(UnityEngine.Object obj)
        {
            return (obj != null && AssetDatabase.Contains(obj) && AssetDatabase.IsMainAsset(obj));
        }
        public static bool IsSubAsset(UnityEngine.Object obj)
        {
            return (obj != null && AssetDatabase.Contains(obj) && AssetDatabase.IsSubAsset(obj));
        }
        public static string GetProjectRelativePath2FullPath(string assetPath)
        {
            return Application.dataPath + assetPath.Remove(0, "Assets".Length);
        }
        public static string GenerateUniqueAssetFullPath(string fullPath)
        {
            var assetPath = AssetDatabase.GenerateUniqueAssetPath(FileUtil.GetProjectRelativePath(fullPath));
            return GetProjectRelativePath2FullPath(assetPath);
        }
        public static T Instantiate<T>(T obj) where T : UnityEngine.Object
        {
            if (obj == null)
                return null;
            var inst = UnityEngine.Object.Instantiate(obj) as T;
            var index = inst.name.LastIndexOf("(Clone)");
            if (index >= 0)
                inst.name = inst.name.Remove(index);
            return inst;
        }

        public static bool IsLightweightRenderPipeline()
        {
#if UNITY_2018_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset == null)
                return false;
#if UNITY_2019_1_OR_NEWER
            var shader = GraphicsSettings.renderPipelineAsset.defaultShader;
#else
            var shader = GraphicsSettings.renderPipelineAsset.GetDefaultShader();
#endif
            if (shader == Shader.Find("LightweightPipeline/Standard (Physically Based)") ||
                shader == Shader.Find("Lightweight Render Pipeline/Lit"))
                return true;
#endif
            return false;
        }
        public static bool IsHighDefinitionRenderPipeline()
        {
#if UNITY_2018_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset == null)
                return false;
#if UNITY_2019_1_OR_NEWER
            var shader = GraphicsSettings.renderPipelineAsset.defaultShader;
#else
            var shader = GraphicsSettings.renderPipelineAsset.GetDefaultShader();
#endif
            if (shader == Shader.Find("HDRenderPipeline/Lit") ||
                shader == Shader.Find("HDRP/Lit"))
                return true;
#endif
            return false;
        }
        public static Shader GetStandardShader()
        {
#if UNITY_2018_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
#if UNITY_2019_1_OR_NEWER
                var shader = GraphicsSettings.renderPipelineAsset.defaultShader;
#else
                var shader = GraphicsSettings.renderPipelineAsset.GetDefaultShader();
#endif
                if (shader != null)
                    return shader;
            }
#endif
            return Shader.Find("Standard");
        }
        public static Material CreateStandardMaterial()
        {
            var material = new Material(GetStandardShader());
            material.color = Color.white;
            if (material.HasProperty("_BaseColor"))  //LWRP
                material.SetColor("_BaseColor", Color.white);
            return material;
        }
        public static void SetMainTexture(Material material, Texture2D texture)
        {
            material.mainTexture = texture;
            if (material.HasProperty("_BaseColorMap"))  //HDRP
                material.SetTexture("_BaseColorMap", texture);
            else if (material.HasProperty("_BaseMap"))  //LWRP
                material.SetTexture("_BaseMap", texture);
        }

        public static bool IsComponentEditable(Component comp)
        {
#if UNITY_2018_3_OR_NEWER
            var prefabType = PrefabUtility.GetPrefabAssetType(comp);
            if (prefabType == PrefabAssetType.NotAPrefab || prefabType == PrefabAssetType.MissingAsset)
                return true;
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
                return false;
            var selectPrefab = PrefabUtility.GetCorrespondingObjectFromSource(comp);
            if (selectPrefab != prefabStage.prefabContentsRoot)
                return false;
            return true;
#else
            return true;
#endif
        }
    }
}
