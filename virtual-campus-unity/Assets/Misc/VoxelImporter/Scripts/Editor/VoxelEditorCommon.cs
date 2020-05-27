#define PreviewObjectHide

using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace VoxelImporter
{
    public class VoxelEditorCommon
    {
        public VoxelBase objectTarget { get; private set; }
        public VoxelBaseCore objectCore { get; private set; }

        public Material vertexColorMaterial { get; private set; }
        public Material vertexColorTransparentMaterial { get; private set; }
        public Material vertexColorTransparentZwriteMaterial { get; private set; }
        public Material unlitColorMaterial { get; private set; }
        public Material unlitTextureMaterial { get; private set; }
        public Texture2D blackTransparentTexture { get; private set; }
        public List<Rect> editorRectList { get; private set; }

        #region GUIStyle
        public GUIStyle guiStyleSkinBox { get; private set; }
        public GUIStyle guiStyleToggleLeft { get; private set; }
        public GUIStyle guiStyleToggleRight { get; private set; }
        public GUIStyle guiStyleLabel { get; private set; }
        #endregion

        //Animation
        private float animationTime;
        public float AnimationPower { get { return 1f - ((Mathf.Cos((Time.realtimeSinceStartup - animationTime) * Mathf.PI * 2f) + 1f) / 2f); } }

        #region SelectionRect
        public struct SelectionRect
        {
            public void Reset()
            {
                Enable = false;
                start = IntVector2.zero;
                end = IntVector2.zero;
            }
            public void SetStart(IntVector2 add)
            {
                Enable = true;
                start = add;
                end = add;
            }
            public void SetEnd(IntVector2 add)
            {
                end = add;
            }
            public bool Enable { get; private set; }
            public int SizeX { get { return max.x - min.x + 1; } }
            public int SizeY { get { return max.y - min.y + 1; } }
            public IntVector2 min { get { return IntVector2.Min(start, end); } }
            public IntVector2 max { get { return IntVector2.Max(start, end); } }
            public Rect rect { get { return new Rect(min.x, min.y, max.x - min.x, max.y - min.y); } }

            public IntVector2 start;
            public IntVector2 end;
        }
        public SelectionRect selectionRect;
        #endregion

        //Tool
        public static Tool lastTool;

        //Fill
        public long fillVoxelDataLastTimeTicks;
        public DataTable3<List<IntVector3>> fillVoxelTable;
        public DataTable3<VoxelData.FaceAreaTable> fillVoxelFaceAreaTable;
        public DataTable3<Dictionary<int, List<IntVector3>>> fillVoxelFaceTable;
        public DataTable3<Dictionary<int, VoxelData.FaceAreaTable>> fillVoxelFaceFaceAreaTable;

        //Mesh
        public Mesh[] silhouetteMesh;
        public Mesh[] previewMesh;
        public Mesh[] cursorMesh;

        #region Icon
        public GameObject iconRoot;
        public RenderTexture iconTexture;
        public GameObject iconModel;
        public MeshRenderer iconModelRenderer;
        public GameObject iconCamera;
        public Camera iconCameraCamera;
        #endregion

        public VoxelEditorCommon(VoxelBase objectTarget, VoxelBaseCore objectCore)
        {
            this.objectTarget = objectTarget;
            this.objectCore = objectCore;

            vertexColorMaterial = new Material(Shader.Find("Voxel Importer/VertexColor"));
            vertexColorMaterial.hideFlags = HideFlags.DontSave;

            vertexColorTransparentMaterial = new Material(Shader.Find("Voxel Importer/VertexColor-Transparent"));
            vertexColorTransparentMaterial.hideFlags = HideFlags.DontSave;

            vertexColorTransparentZwriteMaterial = new Material(Shader.Find("Voxel Importer/VertexColor-Transparent-Zwrite"));
            vertexColorTransparentZwriteMaterial.hideFlags = HideFlags.DontSave;

            unlitColorMaterial = new Material(Shader.Find("Voxel Importer/Unlit/Color"));
            unlitColorMaterial.hideFlags = HideFlags.DontSave;

            unlitTextureMaterial = new Material(Shader.Find("Voxel Importer/Unlit/Transparent"));
            unlitTextureMaterial.hideFlags = HideFlags.DontSave;

            blackTransparentTexture = CreateColorTexture(new Color(0, 0, 0, 0.3f));

            editorRectList = new List<Rect>();

            animationTime = Time.realtimeSinceStartup;
        }

        public void ClearCache()
        {
            ClearSilhouetteMeshMesh();
            ClearPreviewMesh();
            ClearCursorMesh();

            fillVoxelTable = null;
            fillVoxelFaceAreaTable = null;
            fillVoxelFaceTable = null;
            fillVoxelFaceFaceAreaTable = null;
        }

        public Texture2D CreateColorTexture(Color color)
        {
            Texture2D tex = new Texture2D(4, 4);
            tex.hideFlags = HideFlags.DontSave;
            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    tex.SetPixel(x, y, color);
                }
            }
            tex.Apply();
            return tex;
        }

        public Texture2D LoadTexture2DAssetAtPath(string path)
        {
            var result = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (result == null)
            {
                var fileName = Path.GetFileName(path);
                var guids = AssetDatabase.FindAssets("t:Texture2D");
                for (int i = 0; i < guids.Length; i++)
                {
                    var assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                    if (Path.GetFileName(assetPath) == fileName)
                    {
                        if (assetPath.IndexOf("VoxelImporter") >= 0)
                        {
                            result = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public void ClearSilhouetteMeshMesh()
        {
            if (silhouetteMesh != null)
            {
                for (int i = 0; i < silhouetteMesh.Length; i++)
                {
                    MonoBehaviour.DestroyImmediate(silhouetteMesh[i]);
                }
                silhouetteMesh = null;
            }
        }
        public void ClearPreviewMesh()
        {
            if (previewMesh != null)
            {
                for (int i = 0; i < previewMesh.Length; i++)
                {
                    MonoBehaviour.DestroyImmediate(previewMesh[i]);
                }
                previewMesh = null;
            }
        }
        public void ClearCursorMesh()
        {
            if (cursorMesh != null)
            {
                for (int i = 0; i < cursorMesh.Length; i++)
                {
                    MonoBehaviour.DestroyImmediate(cursorMesh[i]);
                }
                cursorMesh = null;
            }
        }

        public bool CheckMousePositionEditorRects()
        {
            for (int i = 0; i < editorRectList.Count; i++)
            {
                if (editorRectList[i].Contains(Event.current.mousePosition))
                {
                    return false;
                }
            }
            return true;
        }

        public IntVector3? GetMousePositionVoxel()
        {
            IntVector3? result = null;

            if (objectCore.voxelData == null || objectTarget.voxelData.voxels == null)
                return result;
            if (!CheckMousePositionEditorRects())
                return result;

            Ray localRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            {
                Matrix4x4 mat = objectTarget.transform.worldToLocalMatrix;
                localRay.direction = mat.MultiplyVector(localRay.direction);
                localRay.origin = mat.MultiplyPoint(localRay.origin);
            }

            {
                var boundsBase = objectCore.GetVoxelBounds(IntVector3.zero);
                float lengthMin = float.MaxValue;
                for (int i = 0; i < objectTarget.voxelData.voxels.Length; i++)
                {
                    if (objectTarget.voxelData.voxels[i].visible == 0)
                        continue;

                    Vector3 position = new Vector3(objectTarget.voxelData.voxels[i].x, objectTarget.voxelData.voxels[i].y, objectTarget.voxelData.voxels[i].z);
                    Vector3 offset = Vector3.Scale(position, objectTarget.importScale);
                    var bounds = boundsBase;
                    bounds.center += offset;
                    float length;
                    if (bounds.IntersectRay(localRay, out length))
                    {
                        if (!result.HasValue || length < lengthMin)
                        {
                            result = objectTarget.voxelData.voxels[i].position;
                            lengthMin = length;
                        }
                    }
                }
            }

            return result;
        }

        public bool GetMousePositionVoxelFace(out IntVector3 result, out VoxelBase.Face face)
        {
            result = IntVector3.zero;
            face = 0;

            if (objectCore.voxelData == null || objectTarget.voxelData.voxels == null)
                return false;
            if (!CheckMousePositionEditorRects())
                return false;

            Ray localRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            {
                Matrix4x4 mat = objectTarget.transform.worldToLocalMatrix;
                localRay.direction = mat.MultiplyVector(localRay.direction);
                localRay.origin = mat.MultiplyPoint(localRay.origin);
            }

            {
                var boundsBase = objectCore.GetVoxelBounds(IntVector3.zero);
                float lengthMin = float.MaxValue;
                for (int i = 0; i < objectTarget.voxelData.voxels.Length; i++)
                {
                    if (objectTarget.voxelData.voxels[i].visible == 0)
                        continue;

                    Vector3 position = new Vector3(objectTarget.voxelData.voxels[i].x, objectTarget.voxelData.voxels[i].y, objectTarget.voxelData.voxels[i].z);
                    Vector3 offset = Vector3.Scale(position, objectTarget.importScale);
                    var bounds = boundsBase;
                    bounds.center += offset;
                    float length;
                    if (bounds.IntersectRay(localRay, out length))
                    {
                        if (length < lengthMin)
                        {
                            result = objectTarget.voxelData.voxels[i].position;
                            lengthMin = length;
                            #region Face
                            {
                                const float Threshold = 0.001f;
                                var posP = localRay.GetPoint(length);
                                if (Mathf.Abs(bounds.min.x - posP.x) < Threshold)
                                    face = VoxelBase.Face.left;
                                else if (Mathf.Abs(bounds.max.x - posP.x) < Threshold)
                                    face = VoxelBase.Face.right;
                                else if (Mathf.Abs(bounds.min.y - posP.y) < Threshold)
                                    face = VoxelBase.Face.down;
                                else if (Mathf.Abs(bounds.max.y - posP.y) < Threshold)
                                    face = VoxelBase.Face.up;
                                else if (Mathf.Abs(bounds.min.z - posP.z) < Threshold)
                                    face = VoxelBase.Face.back;
                                else if (Mathf.Abs(bounds.max.z - posP.z) < Threshold)
                                    face = VoxelBase.Face.forward;
                                else
                                    Assert.IsTrue(false);
                            }
                            #endregion
                        }
                    }
                }
            }

            return face != 0;
        }

        private void CheckFillVoxelTableCheck()
        {
            if (fillVoxelDataLastTimeTicks != objectTarget.voxelData.updateVoxelTableLastTimeTicks)
            {
                fillVoxelTable = null;
                fillVoxelFaceAreaTable = null;
                fillVoxelFaceTable = null;
                fillVoxelFaceFaceAreaTable = null;
                fillVoxelDataLastTimeTicks = objectTarget.voxelData.updateVoxelTableLastTimeTicks;
            }
        }
        public List<IntVector3> GetFillVoxel(IntVector3 pos)
        {
            if (objectTarget.voxelData == null) return null;
            if (objectTarget.voxelData.VoxelTableContains(pos) < 0) return null;

            CheckFillVoxelTableCheck();
            if (fillVoxelTable == null)
            {
                fillVoxelTable = new DataTable3<List<IntVector3>>(objectTarget.voxelData.voxelSize.x, objectTarget.voxelData.voxelSize.y, objectTarget.voxelData.voxelSize.z);
            }
            if (!fillVoxelTable.Contains(pos))
            {
                List<IntVector3> searchList = new List<IntVector3>();
                int posPalette = 0;
                var doneTable = new FlagTable3(objectTarget.voxelData.voxelSize.x, objectTarget.voxelData.voxelSize.y, objectTarget.voxelData.voxelSize.z);
                {
                    var index = objectTarget.voxelData.VoxelTableContains(pos);
                    posPalette = objectTarget.voxelData.voxels[index].palette;
                    searchList.Clear();
                    searchList.Add(pos);
                    doneTable.Set(pos, true);
                }
                var result = new List<IntVector3>();
                for (int j = 0; j < searchList.Count; j++)
                {
                    var p = searchList[j];
                    var index = objectTarget.voxelData.VoxelTableContains(p);
                    if (index < 0) continue;
                    if (objectTarget.voxelData.voxels[index].palette == posPalette)
                    {
                        result.Add(p);
                        for (int x = p.x - 1; x <= p.x + 1; x++)
                        {
                            for (int y = p.y - 1; y <= p.y + 1; y++)
                            {
                                for (int z = p.z - 1; z <= p.z + 1; z++)
                                {
                                    if (x >= 0 && y >= 0 && z >= 0 &&
                                        x < objectTarget.voxelData.voxelSize.x && y < objectTarget.voxelData.voxelSize.y && z < objectTarget.voxelData.voxelSize.z &&
                                        !doneTable.Get(x, y, z))
                                    {
                                        doneTable.Set(x, y, z, true);
                                        var indexTmp = objectTarget.voxelData.VoxelTableContains(x, y, z);
                                        if (indexTmp >= 0 && objectTarget.voxelData.voxels[indexTmp].palette == posPalette)
                                            searchList.Add(new IntVector3(x, y, z));
                                    }
                                }
                            }
                        }
                    }
                }

                for (int j = 0; j < result.Count; j++)
                {
                    fillVoxelTable.Set(result[j], result);
                }
            }
            var fillVoxel = fillVoxelTable.Get(pos);

            return fillVoxel;
        }
        public VoxelData.FaceAreaTable GetFillVoxelFaceAreaTable(IntVector3 pos)
        {
            if (objectTarget.voxelData == null) return null;
            if (objectTarget.voxelData.VoxelTableContains(pos) < 0) return null;

            CheckFillVoxelTableCheck();
            if (fillVoxelFaceAreaTable == null)
            {
                fillVoxelFaceAreaTable = new DataTable3<VoxelData.FaceAreaTable>(objectTarget.voxelData.voxelSize.x, objectTarget.voxelData.voxelSize.y, objectTarget.voxelData.voxelSize.z);
            }
            if (!fillVoxelFaceAreaTable.Contains(pos))
            {
                var list = GetFillVoxel(pos);
                if (list == null) return null;
                var voxels = new List<VoxelData.Voxel>();
                for (int i = 0; i < list.Count; i++)
                {
                    var index = objectTarget.voxelData.VoxelTableContains(list[i]);
                    var voxel = objectTarget.voxelData.voxels[index];
                    voxel.palette = -1;
                    voxels.Add(voxel);
                }
                var faceAreaTable = objectCore.Edit_CreateMeshOnly_FaceArea(voxels, true);
                for (int i = 0; i < list.Count; i++)
                {
                    fillVoxelFaceAreaTable.Set(list[i], faceAreaTable);
                }
            }
            var fillVoxelFaceArea = fillVoxelFaceAreaTable.Get(pos);

            return fillVoxelFaceArea;
        }
        public List<IntVector3> GetFillVoxelFace(IntVector3 pos, VoxelBase.Face face)
        {
            if (objectTarget.voxelData == null) return null;
            if (objectTarget.voxelData.VoxelTableContains(pos) < 0) return null;

            CheckFillVoxelTableCheck();
            if (fillVoxelFaceTable == null)
            {
                fillVoxelFaceTable = new DataTable3<Dictionary<int, List<IntVector3>>>(objectTarget.voxelData.voxelSize.x, objectTarget.voxelData.voxelSize.y, objectTarget.voxelData.voxelSize.z);
            }
            if (!fillVoxelFaceTable.Contains(pos) ||
                !fillVoxelFaceTable.Get(pos).ContainsKey((int)face))
            {
                List<IntVector3> searchList = new List<IntVector3>();
                var doneTable = new FlagTable3(objectTarget.voxelData.voxelSize.x, objectTarget.voxelData.voxelSize.y, objectTarget.voxelData.voxelSize.z);
                {
                    searchList.Clear();
                    searchList.Add(pos);
                    doneTable.Set(pos, true);
                }
                var result = new List<IntVector3>();
                for (int j = 0; j < searchList.Count; j++)
                {
                    var p = searchList[j];
                    var index = objectTarget.voxelData.VoxelTableContains(p);
                    if (index < 0) continue;
                    if ((objectTarget.voxelData.voxels[index].visible & face) != 0)
                    {
                        result.Add(p);
                        int xOffset = (face & (VoxelBase.Face.up | VoxelBase.Face.down | VoxelBase.Face.forward | VoxelBase.Face.back)) != 0 ? 1 : 0;
                        int yOffset = (face & (VoxelBase.Face.right | VoxelBase.Face.left | VoxelBase.Face.forward | VoxelBase.Face.back)) != 0 ? 1 : 0;
                        int zOffset = (face & (VoxelBase.Face.right | VoxelBase.Face.left | VoxelBase.Face.up | VoxelBase.Face.down)) != 0 ? 1 : 0;
                        for (int x = p.x - xOffset; x <= p.x + xOffset; x++)
                        {
                            for (int y = p.y - yOffset; y <= p.y + yOffset; y++)
                            {
                                for (int z = p.z - zOffset; z <= p.z + zOffset; z++)
                                {
                                    if (x >= 0 && y >= 0 && z >= 0 &&
                                        x < objectTarget.voxelData.voxelSize.x && y < objectTarget.voxelData.voxelSize.y && z < objectTarget.voxelData.voxelSize.z &&
                                        !doneTable.Get(x, y, z))
                                    {
                                        doneTable.Set(x, y, z, true);
                                        var indexTmp = objectTarget.voxelData.VoxelTableContains(x, y, z);
                                        if (indexTmp >= 0)
                                            searchList.Add(new IntVector3(x, y, z));
                                    }
                                }
                            }
                        }
                    }
                }

                Dictionary<int, List<IntVector3>> data;
                if (fillVoxelFaceTable.Contains(pos))
                    data = fillVoxelFaceTable.Get(pos);
                else
                    data = new Dictionary<int, List<IntVector3>>();
                data[(int)face] = result;
                fillVoxelFaceTable.Set(pos, data);
                for (int j = 0; j < result.Count; j++)
                {
                    if (fillVoxelFaceTable.Contains(result[j]))
                        data = fillVoxelFaceTable.Get(result[j]);
                    else
                        data = new Dictionary<int, List<IntVector3>>();
                    data[(int)face] = result;
                    fillVoxelFaceTable.Set(result[j], data);
                }
            }
            var fillVoxel = fillVoxelFaceTable.Get(pos)[(int)face];

            return fillVoxel;
        }
        public VoxelData.FaceAreaTable GetFillVoxelFaceFaceAreaTable(IntVector3 pos, VoxelBase.Face face)
        {
            if (objectTarget.voxelData == null) return null;
            if (objectTarget.voxelData.VoxelTableContains(pos) < 0) return null;

            CheckFillVoxelTableCheck();
            if (fillVoxelFaceFaceAreaTable == null)
            {
                fillVoxelFaceFaceAreaTable = new DataTable3<Dictionary<int, VoxelData.FaceAreaTable>>(objectTarget.voxelData.voxelSize.x, objectTarget.voxelData.voxelSize.y, objectTarget.voxelData.voxelSize.z);
            }
            if (!fillVoxelFaceFaceAreaTable.Contains(pos) ||
                !fillVoxelFaceFaceAreaTable.Get(pos).ContainsKey((int)face))
            {
                var list = GetFillVoxelFace(pos, face);
                if (list == null) return null;
                var voxels = new List<VoxelData.Voxel>();
                for (int i = 0; i < list.Count; i++)
                {
                    var index = objectTarget.voxelData.VoxelTableContains(list[i]);
                    var voxel = objectTarget.voxelData.voxels[index];
                    voxel.palette = -1;
                    voxel.visible = face;
                    voxels.Add(voxel);
                }
                var faceAreaTable = objectCore.Edit_CreateMeshOnly_FaceArea(voxels, true);

                Dictionary<int, VoxelData.FaceAreaTable> data;
                if (fillVoxelFaceFaceAreaTable.Contains(pos))
                    data = fillVoxelFaceFaceAreaTable.Get(pos);
                else
                    data = new Dictionary<int, VoxelData.FaceAreaTable>();
                data[(int)face] = faceAreaTable;
                fillVoxelFaceFaceAreaTable.Set(pos, data);
                for (int i = 0; i < list.Count; i++)
                {
                    if (fillVoxelFaceFaceAreaTable.Contains(list[i]))
                        data = fillVoxelFaceFaceAreaTable.Get(list[i]);
                    else
                        data = new Dictionary<int, VoxelData.FaceAreaTable>();
                    data[(int)face] = faceAreaTable;
                    fillVoxelFaceFaceAreaTable.Set(list[i], data);
                }
            }
            var fillVoxelFaceArea = fillVoxelFaceFaceAreaTable.Get(pos)[(int)face];

            return fillVoxelFaceArea;
        }

        public struct VertexPower
        {
            public IntVector3 position;
            public float power;
        }
        public List<VertexPower> GetMousePositionVertex(float radius)
        {
            List<VertexPower> result = new List<VertexPower>();
            if (objectTarget.voxelData == null || objectTarget.voxelData.voxels == null)
                return result;
            if (!CheckMousePositionEditorRects())
                return result;

            var boundsList = new List<Bounds>();
            var vertexList = new List<VertexPower>();
            {
                Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                ray.origin = objectTarget.transform.worldToLocalMatrix.MultiplyPoint3x4(ray.origin);
                ray.direction = objectTarget.transform.worldToLocalMatrix.MultiplyVector(ray.direction);
                bool[,,] doneTable = new bool[objectTarget.voxelData.voxelSize.x + 1, objectTarget.voxelData.voxelSize.y + 1, objectTarget.voxelData.voxelSize.z + 1];
                {
                    Ray rayRadius = HandleUtility.GUIPointToWorldRay(new Vector2(Event.current.mousePosition.x + radius, Event.current.mousePosition.y));
                    rayRadius.origin = objectTarget.transform.worldToLocalMatrix.MultiplyPoint3x4(rayRadius.origin);
                    rayRadius.direction = objectTarget.transform.worldToLocalMatrix.MultiplyVector(rayRadius.direction);

                    Func<IntVector3, bool> AddVertex = (pos) =>
                    {
                        if (doneTable[pos.x, pos.y, pos.z])
                            return true;
                        doneTable[pos.x, pos.y, pos.z] = true;

                        var posL = objectCore.GetVoxelRatePosition(pos, Vector3.zero);
                        var posP = ray.origin + ray.direction * Vector3.Dot(posL - ray.origin, ray.direction);
                        var posR = rayRadius.origin + rayRadius.direction * (Vector3.Dot(posP - rayRadius.origin, rayRadius.direction));
                        var distanceL = (posL - posP).sqrMagnitude;
                        var distanceR = (posR - posP).sqrMagnitude;
                        if (distanceL < distanceR)
                        {
                            vertexList.Add(new VertexPower() { position = pos, power = distanceL / distanceR });
                            return true;
                        }
                        return false;
                    };
                    for (int i = 0; i < objectTarget.voxelData.voxels.Length; i++)
                    {
                        if (objectTarget.voxelData.voxels[i].visible == 0) continue;

                        var pos = objectTarget.voxelData.voxels[i].position;
                        bool enable = false;

                        if ((objectTarget.voxelData.voxels[i].visible & (VoxelBase.Face.left | VoxelBase.Face.down | VoxelBase.Face.back)) != 0)
                            if (AddVertex(new IntVector3(pos.x, pos.y, pos.z))) enable = true;
                        if ((objectTarget.voxelData.voxels[i].visible & (VoxelBase.Face.right | VoxelBase.Face.down | VoxelBase.Face.back)) != 0)
                            if (AddVertex(new IntVector3(pos.x + 1, pos.y, pos.z))) enable = true;
                        if ((objectTarget.voxelData.voxels[i].visible & (VoxelBase.Face.left | VoxelBase.Face.up | VoxelBase.Face.back)) != 0)
                            if (AddVertex(new IntVector3(pos.x, pos.y + 1, pos.z))) enable = true;
                        if ((objectTarget.voxelData.voxels[i].visible & (VoxelBase.Face.left | VoxelBase.Face.down | VoxelBase.Face.forward)) != 0)
                            if (AddVertex(new IntVector3(pos.x, pos.y, pos.z + 1))) enable = true;
                        if ((objectTarget.voxelData.voxels[i].visible & (VoxelBase.Face.right | VoxelBase.Face.up | VoxelBase.Face.back)) != 0)
                            if (AddVertex(new IntVector3(pos.x + 1, pos.y + 1, pos.z))) enable = true;
                        if ((objectTarget.voxelData.voxels[i].visible & (VoxelBase.Face.right | VoxelBase.Face.down | VoxelBase.Face.forward)) != 0)
                            if (AddVertex(new IntVector3(pos.x + 1, pos.y, pos.z + 1))) enable = true;
                        if ((objectTarget.voxelData.voxels[i].visible & (VoxelBase.Face.left | VoxelBase.Face.up | VoxelBase.Face.forward)) != 0)
                            if (AddVertex(new IntVector3(pos.x, pos.y + 1, pos.z + 1))) enable = true;
                        if ((objectTarget.voxelData.voxels[i].visible & (VoxelBase.Face.right | VoxelBase.Face.up | VoxelBase.Face.forward)) != 0)
                            if (AddVertex(new IntVector3(pos.x + 1, pos.y + 1, pos.z + 1))) enable = true;
                        if (enable)
                        {
                            if ((ray.direction.x < 0f && ((objectTarget.voxelData.voxels[i].visible & VoxelBase.Face.right) != 0)) ||
                                (ray.direction.x > 0f && ((objectTarget.voxelData.voxels[i].visible & VoxelBase.Face.left) != 0)) ||
                                (ray.direction.y < 0f && ((objectTarget.voxelData.voxels[i].visible & VoxelBase.Face.up) != 0)) ||
                                (ray.direction.y > 0f && ((objectTarget.voxelData.voxels[i].visible & VoxelBase.Face.down) != 0)) ||
                                (ray.direction.z < 0f && ((objectTarget.voxelData.voxels[i].visible & VoxelBase.Face.forward) != 0)) ||
                                (ray.direction.z > 0f && ((objectTarget.voxelData.voxels[i].visible & VoxelBase.Face.back) != 0)))
                            {
                                boundsList.Add(objectCore.GetVoxelBounds(pos));
                            }
                        }
                    }
                }
            }

            {
                for (int i = 0; i < vertexList.Count; i++)
                {
                    var pos = objectCore.GetVoxelRatePosition(vertexList[i].position, Vector3.zero);
                    Ray ray = HandleUtility.GUIPointToWorldRay(HandleUtility.WorldToGUIPoint(objectTarget.transform.localToWorldMatrix.MultiplyPoint3x4(pos)));
                    ray.origin = objectTarget.transform.worldToLocalMatrix.MultiplyPoint3x4(ray.origin);
                    ray.direction = objectTarget.transform.worldToLocalMatrix.MultiplyVector(ray.direction);
                    float length = (pos - ray.origin).magnitude - 0.1f;
                    bool enable = true;
                    for (int j = 0; j < boundsList.Count; j++)
                    {
                        float distance;
                        if (boundsList[j].IntersectRay(ray, out distance))
                        {
                            if (distance < length)
                            {
                                enable = false;
                                break;
                            }
                        }
                    }
                    if (enable)
                    {
                        result.Add(vertexList[i]);
                    }
                }
            }

            return result;
        }

        public List<IntVector3> GetSelectionRectVoxel()
        {
            List<IntVector3> result = new List<IntVector3>();
            if (selectionRect.Enable &&
                objectTarget.voxelData != null && objectTarget.voxelData.voxels != null)
            {
                var localToWorldMatrix = objectTarget.transform.localToWorldMatrix;
                for (int i = 0; i < objectTarget.voxelData.voxels.Length; i++)
                {
                    var local = objectCore.GetVoxelCenterPosition(objectTarget.voxelData.voxels[i].position);
                    var world = localToWorldMatrix.MultiplyPoint(local);
                    var screen = HandleUtility.WorldToGUIPoint(world);
                    if (selectionRect.rect.Contains(screen))
                    {
                        result.Add(objectTarget.voxelData.voxels[i].position);
                    }
                }
            }
            return result;
        }
        public Dictionary<IntVector3, VoxelBase.Face> GetSelectionRectVoxelFace()
        {
            Dictionary<IntVector3, VoxelBase.Face> result = new Dictionary<IntVector3, VoxelBase.Face>();
            if (selectionRect.Enable &&
                objectTarget.voxelData != null && objectTarget.voxelData.vertexList != null)
            {
                var localToWorldMatrix = objectTarget.transform.localToWorldMatrix;
                FlagTable3 containsTable = new FlagTable3(objectTarget.voxelData.voxelSize.x + 1, objectTarget.voxelData.voxelSize.y + 1, objectTarget.voxelData.voxelSize.z + 1);
                for (int i = 0; i < objectTarget.voxelData.vertexList.Count; i++)
                {
                    var local = objectCore.GetVoxelRatePosition(objectTarget.voxelData.vertexList[i], Vector3.zero);
                    var world = localToWorldMatrix.MultiplyPoint3x4(local);
                    var screen = HandleUtility.WorldToGUIPoint(world);
                    if (selectionRect.rect.Contains(screen))
                        containsTable.Set(objectTarget.voxelData.vertexList[i], true);
                }
                containsTable.AllAction((x, y, z) =>
                {
                    Action<int, int, int, VoxelBase.Face> AddFace = (xx, yy, zz, face) =>
                    {
                        var pos = new IntVector3(xx, yy, zz);
                        var index = objectTarget.voxelData.VoxelTableContains(pos);
                        if (index < 0) return;
                        if ((objectTarget.voxelData.voxels[index].visible & face) == 0) return;

                        VoxelBase.Face combineFace;
                        if (!result.TryGetValue(pos, out combineFace))
                        {
                            result.Add(pos, face);
                        }
                        else
                        {
                            combineFace |= face;
                            result[pos] = combineFace;
                        }
                    };
                    #region Left
                    if (containsTable.Get(x, y + 1, z) &&
                        containsTable.Get(x, y, z + 1) &&
                        containsTable.Get(x, y + 1, z + 1))
                    {
                        AddFace(x, y, z, VoxelBase.Face.left);
                        AddFace(x - 1, y, z, VoxelBase.Face.right);
                    }
                    #endregion
                    #region Down
                    if (containsTable.Get(x + 1, y, z) &&
                        containsTable.Get(x, y, z + 1) &&
                        containsTable.Get(x + 1, y, z + 1))
                    {
                        AddFace(x, y, z, VoxelBase.Face.down);
                        AddFace(x, y - 1, z, VoxelBase.Face.up);
                    }
                    #endregion
                    #region Back
                    if (containsTable.Get(x + 1, y, z) &&
                        containsTable.Get(x, y + 1, z) &&
                        containsTable.Get(x + 1, y + 1, z))
                    {
                        AddFace(x, y, z, VoxelBase.Face.back);
                        AddFace(x, y, z - 1, VoxelBase.Face.forward);
                    }
                    #endregion
                });
            }
            return result;
        }
        public List<IntVector3> GetSelectionRectVertex()
        {
            List<IntVector3> result = new List<IntVector3>();
            if (selectionRect.Enable &&
                objectTarget.voxelData != null && objectTarget.voxelData.vertexList != null)
            {
                var localToWorldMatrix = objectTarget.transform.localToWorldMatrix;
                for (int i = 0; i < objectTarget.voxelData.vertexList.Count; i++)
                {
                    var local = objectCore.GetVoxelRatePosition(objectTarget.voxelData.vertexList[i], Vector3.zero);
                    var world = localToWorldMatrix.MultiplyPoint3x4(local);
                    var screen = HandleUtility.WorldToGUIPoint(world);
                    if (selectionRect.rect.Contains(screen))
                    {
                        result.Add(objectTarget.voxelData.vertexList[i]);
                    }
                }
            }
            return result;
        }

        public Rect ResizeSceneViewRect(Rect rect)
        {
            var sv = SceneView.currentDrawingSceneView;
            if (rect.x + rect.width >= sv.position.width)
                rect.x -= (rect.x + rect.width) - sv.position.width;
            if (rect.y + rect.height >= sv.position.height)
                rect.y -= (rect.y + rect.height) - sv.position.height;
            if (rect.x < 0)
                rect.x -= rect.x;
            if (rect.y < 0)
                rect.y -= rect.y;
            return rect;
        }

        public void GUIStyleReady()
        {
            if (guiStyleSkinBox == null)
            {
                guiStyleSkinBox = new GUIStyle(GUI.skin.box);
                var olBox = new GUIStyle("OL box");
                guiStyleSkinBox.normal = olBox.normal;
                guiStyleSkinBox.hover = olBox.hover;
                guiStyleSkinBox.focused = olBox.focused;
                guiStyleSkinBox.active = olBox.active;
            }
            if (guiStyleToggleLeft == null)
                guiStyleToggleLeft = new GUIStyle(GUI.skin.toggle);
            guiStyleToggleLeft.normal.textColor = Color.white;
            guiStyleToggleLeft.onNormal.textColor = Color.white;
            guiStyleToggleLeft.hover.textColor = Color.white;
            guiStyleToggleLeft.onHover.textColor = Color.white;
            guiStyleToggleLeft.focused.textColor = Color.white;
            guiStyleToggleLeft.onFocused.textColor = Color.white;
            guiStyleToggleLeft.active.textColor = Color.white;
            guiStyleToggleLeft.onActive.textColor = Color.white;
            if (guiStyleToggleRight == null)
                guiStyleToggleRight = new GUIStyle(GUI.skin.toggle);
            guiStyleToggleRight.normal.textColor = Color.white;
            guiStyleToggleRight.onNormal.textColor = Color.white;
            guiStyleToggleRight.hover.textColor = Color.white;
            guiStyleToggleRight.onHover.textColor = Color.white;
            guiStyleToggleRight.focused.textColor = Color.white;
            guiStyleToggleRight.onFocused.textColor = Color.white;
            guiStyleToggleRight.active.textColor = Color.white;
            guiStyleToggleRight.onActive.textColor = Color.white;
            guiStyleToggleRight.padding.left = 2;
            guiStyleToggleRight.overflow.left = -149;
            if (guiStyleLabel == null)
                guiStyleLabel = new GUIStyle(GUI.skin.label);
            guiStyleLabel.normal.textColor = Color.white;
        }

        #region Icon
        public void InitializeIcon()
        {
            const string IconRootObjectName = "Tmp#VoxelImporterIcon";

            {
                var objects = Resources.FindObjectsOfTypeAll<GameObject>();
                for (int i = 0; i < objects.Length; i++)
                {
                    if (objects[i].name == IconRootObjectName)
                        iconRoot = objects[i];
                }
            }

            if (iconRoot == null)
            {
#if PreviewObjectHide
                iconRoot = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(IconRootObjectName, HideFlags.HideAndDontSave);
#else
                iconRoot = new GameObject(IconRootObjectName);
                iconRoot.hideFlags = HideFlags.DontSave;
#endif
            }
            iconRoot.SetActive(false);

            int blankLayer;
            for (blankLayer = 31; blankLayer > 0; blankLayer--)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(blankLayer)))
                    break;
            }
            if (blankLayer < 0)
                blankLayer = 31;
            iconRoot.layer = blankLayer;

            iconTexture = new RenderTexture(128, 128, 16, RenderTextureFormat.ARGB32);
            iconTexture.hideFlags = HideFlags.DontSave;
            iconTexture.Create();

            CreateIconObject();
        }

        public void CreateIconObject(Transform transform = null, Mesh mesh = null, Material[] materials = null)
        {
            if (iconRoot == null) return;

            if (transform != null)
            {
                iconRoot.transform.position = transform.position;
                iconRoot.transform.rotation = transform.rotation;
                iconRoot.transform.localScale = Vector3.one;
            }
            else
            {
                iconRoot.transform.position = Vector3.zero;
                iconRoot.transform.rotation = Quaternion.identity;
                iconRoot.transform.localScale = Vector3.one;
            }

            {
                const string IconModelName = "IconModel";

                var iconModelTransform = iconRoot.transform.Find(IconModelName);
                if (iconModelTransform != null)
                    iconModel = iconModelTransform.gameObject;
                if (iconModel == null)
                {
#if PreviewObjectHide
                    iconModel = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(IconModelName, HideFlags.HideAndDontSave);
#else
                    iconModel = new GameObject(IconModelName);
                    iconModel.hideFlags = HideFlags.DontSave;
#endif
                }
                iconModel.transform.SetParent(iconRoot.transform);
                iconModel.transform.localPosition = new Vector3(0f, 0f, 0f);
                iconModel.layer = iconRoot.layer;
                MeshFilter meshFilter = iconModel.GetComponent<MeshFilter>();
                if (meshFilter == null)
                    meshFilter = iconModel.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;
                iconModelRenderer = iconModel.GetComponent<MeshRenderer>();
                if (iconModelRenderer == null)
                    iconModelRenderer = iconModel.AddComponent<MeshRenderer>();
                if (materials != null)
                    iconModelRenderer.sharedMaterials = materials;
            }
            {
                const string IconCameraName = "IconCamera";

                var iconCameraTransform = iconRoot.transform.Find(IconCameraName);
                if (iconCameraTransform != null)
                    iconCamera = iconCameraTransform.gameObject;
                if (iconCamera == null)
                {
#if PreviewObjectHide
                    iconCamera = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(IconCameraName, HideFlags.HideAndDontSave);
#else
                    iconCamera = new GameObject(IconCameraName);
                    iconCamera.hideFlags = HideFlags.DontSave;
#endif
                    iconCameraCamera = iconCamera.AddComponent<Camera>();
                }
                else
                {
                    iconCameraCamera = iconCamera.GetComponent<Camera>();
                }
                iconCamera.transform.SetParent(iconRoot.transform);
                iconCamera.layer = iconRoot.layer;
                if (mesh != null)
                {
                    var rot = Quaternion.AngleAxis(180f, Vector3.up);
                    iconCamera.transform.localRotation = rot;
                    iconCamera.transform.localPosition = mesh.bounds.center + iconCamera.transform.worldToLocalMatrix.MultiplyVector(iconCamera.transform.forward) * mesh.bounds.size.z * 5f;
                }
                else
                {
                    iconCamera.transform.localPosition = Vector3.zero;
                    iconCamera.transform.localRotation = Quaternion.identity;
                }
                iconCameraCamera.orthographic = true;
                if (mesh != null)
                {
                    iconCameraCamera.orthographicSize = Mathf.Max(mesh.bounds.size.x, Mathf.Max(mesh.bounds.size.y, mesh.bounds.size.z)) * 0.6f;
                    iconCameraCamera.farClipPlane = Mathf.Max(mesh.bounds.size.x, Mathf.Max(mesh.bounds.size.y, mesh.bounds.size.z)) * 5f;
                }
                iconCameraCamera.clearFlags = CameraClearFlags.Color;
                iconCameraCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
                iconCameraCamera.cullingMask = 1 << iconRoot.layer;
                iconCameraCamera.targetTexture = iconTexture;

            }
        }

        public bool IsReadyIcon()
        {
            return iconRoot != null && iconModelRenderer != null && iconModelRenderer.sharedMaterials != null;
        }

        public Texture2D IconObjectRender()
        {
            if (iconRoot == null) return null;

            Texture2D tex = null;

            iconRoot.SetActive(true);
            iconCameraCamera.Render();
            {
                RenderTexture save = RenderTexture.active;
                RenderTexture.active = iconTexture;
                tex = new Texture2D(iconTexture.width, iconTexture.height, TextureFormat.ARGB32, iconTexture.useMipMap);
                tex.hideFlags = HideFlags.DontSave;
                tex.ReadPixels(new Rect(0, 0, iconTexture.width, iconTexture.height), 0, 0);
                tex.Apply();
                RenderTexture.active = save;
            }
            iconRoot.SetActive(false);

            return tex;
        }

        #endregion
    }
}