using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VoxelImporter
{
    [RequireComponent(typeof(VoxelBase))]
#if UNITY_2018_3_OR_NEWER
    [ExecuteAlways]
#else
    [ExecuteInEditMode]
#endif
    public class EditMesh : MonoBehaviour
    {
#if UNITY_EDITOR
        public enum EditType
        {
            SphereCommonNormal,
            SphereNormal,
            ChangeFrontVertex,
        }
        public EditType editType;

        private void OnEnable()
        {
            var voxelBase = GetComponent<VoxelBase>();
            if (voxelBase == null) return;
            voxelBase.onBeforeCreateMesh += OnBeforeCreateMesh;
        }
        private void OnDisable()
        {
            var voxelBase = GetComponent<VoxelBase>();
            if (voxelBase == null) return;
            voxelBase.onBeforeCreateMesh -= OnBeforeCreateMesh;
        }

        private void OnBeforeCreateMesh(VoxelBase.OnBeforeCreateMeshData data)
        {
            var voxelBase = GetComponent<VoxelBase>();
            if (voxelBase == null || voxelBase.structureData == null) return;

            switch (editType)
            {
            case EditType.SphereCommonNormal:
                {
                    var center = new Vector3(voxelBase.voxelData.voxelSize.x / 2f, voxelBase.voxelData.voxelSize.y / 2f, voxelBase.voxelData.voxelSize.z / 2f);
                    for (int i = 0; i < voxelBase.structureData.voxels.Length; i++)
                    {
                        var normal = (new Vector3(voxelBase.voxelData.voxels[i].x, voxelBase.voxelData.voxels[i].y, voxelBase.voxelData.voxels[i].z) + Vector3.one / 2f) - center;
                        normal.Normalize();
                        foreach (var index in voxelBase.structureData.voxels[i].indices)
                        {
                            data.normals[index.vertexIndex] = normal;
                        }
                    }
                }
                break;
            case EditType.SphereNormal:
                {
                    Vector3 center;
                    {
                        Bounds bounds = new Bounds(data.vertices[0], Vector3.zero);
                        foreach (var vertex in data.vertices)
                        {
                            bounds.Encapsulate(vertex);
                        }
                        center = bounds.center;
                    }
                    for (int i = 0; i < data.vertices.Count; i++)
                    {
                        var normal = data.vertices[i] - center;
                        data.normals[i] = normal.normalized;
                    }
                }
                break;
            case EditType.ChangeFrontVertex:
                for (int i = 0; i < voxelBase.structureData.voxels.Length; i++)
                {
                    foreach (var index in voxelBase.structureData.voxels[i].indices)
                    {
                        switch (index.voxelPosition)
                        {
                        case VoxelBase.VoxelVertexIndex.XYZ:
                        case VoxelBase.VoxelVertexIndex._XYZ:
                        case VoxelBase.VoxelVertexIndex.X_YZ:
                        case VoxelBase.VoxelVertexIndex._X_YZ:
                            {
                                var pos = data.vertices[index.vertexIndex];
                                pos.z = voxelBase.voxelData.voxelSize.z / 2;
                                data.vertices[index.vertexIndex] = pos;
                            }
                            break;
                        }
                    }
                }
                break;
            default:
                break;
            }
        }
#else
        private void Awake()
        {
            enabled = false;
        }
#endif
    }
}
