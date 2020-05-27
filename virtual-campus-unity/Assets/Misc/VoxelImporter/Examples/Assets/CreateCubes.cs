using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

namespace VoxelImporter
{
    public class CreateCubes : MonoBehaviour
    {
        public VoxelStructure voxelStructure;

        private Material[] materials;

        public void Awake()
        {
            if (voxelStructure == null) return;

            materials = new Material[voxelStructure.palettes.Length];
            for (int i = 0; i < voxelStructure.palettes.Length; i++)
            {
                Shader shader = null;
#if UNITY_2019_1_OR_NEWER
                if (GraphicsSettings.renderPipelineAsset != null)
                    shader = GraphicsSettings.renderPipelineAsset.defaultShader;
#elif UNITY_2018_1_OR_NEWER
                if (GraphicsSettings.renderPipelineAsset != null)
                    shader = GraphicsSettings.renderPipelineAsset.GetDefaultShader();
#endif
                if (shader == null)
                    shader = Shader.Find("Standard");
                materials[i] = new Material(shader);
                materials[i].name = string.Format("Palette {0}", i);
                materials[i].color = voxelStructure.palettes[i];
                if (materials[i].HasProperty("_BaseColor"))  //HDRP
                    materials[i].SetColor("_BaseColor", voxelStructure.palettes[i]);
            }

            for (int i = 0; i < voxelStructure.voxels.Length; i++)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = string.Format("{0} : ({1}, {2}, {3})", i, voxelStructure.voxels[i].x, voxelStructure.voxels[i].y, voxelStructure.voxels[i].z);
                go.transform.localPosition = voxelStructure.voxels[i].position;
                go.transform.SetParent(transform);
                {
                    var renderer = go.GetComponent<Renderer>();
                    renderer.sharedMaterial = materials[voxelStructure.voxels[i].palette];
                }
                {
                    var rigidbody = go.AddComponent<Rigidbody>();
                    rigidbody.isKinematic = true;
                    rigidbody.Sleep();
                }
            }

            enabled = false;
        }
    }
}
