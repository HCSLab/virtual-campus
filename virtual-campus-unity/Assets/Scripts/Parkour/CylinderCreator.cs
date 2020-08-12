using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CylinderCreator : MonoBehaviour
{
    static Vector3 ptStart = new Vector3(-10.0f, 0f, 0f);
    static Vector3 ptEnd = new Vector3(80.0f, 0f, 0f);
    static float innerRadius = 10.0f, outterRadius = 11.0f;
    static string meshPrefabPath = "Assets/Models/Mesh/";//圆锥Mesh保存路径
    static string meshName = "HollowClinder.asset";//圆锥
    [MenuItem("GameObject/3D Object/HollowClinder", false, priority = 8)]
    public static void CreateHollowClinder()
    {
        SpawnHollowClinderInHierarchy();
    }

    public static GameObject SpawnHollowClinderInHierarchy()
    {
        Mesh chunkMesh = new Mesh();
        if (!File.Exists(meshPrefabPath + meshName))
        {
            //计算垂直于轴的起始向量
            Vector3 vec1 = ptEnd - ptStart;
            Vector3 vec2 = Vector3.up;
            float a = Vector3.Angle(vec1, vec2);
            if (Mathf.Approximately(a, 0.0f))
            {
                vec2 = Vector3.right;
            }
            Vector3 vecStart = Vector3.Cross(vec1, vec2);

            //计算开始面内圆点、外圆点，结束面内圆点、外圆点
            List<Vector3> pointsStartInner = new List<Vector3>();
            List<Vector3> pointsStartOutter = new List<Vector3>();
            List<Vector3> pointsEndtInner = new List<Vector3>();
            List<Vector3> pointsEndOutter = new List<Vector3>();

            GameObject objStartInner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject objStartOutter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject objEndInner = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            GameObject objEndOutter = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objStartInner.transform.position = ptStart + innerRadius * vecStart.normalized;
            objStartOutter.transform.position = ptStart + outterRadius * vecStart.normalized;
            objEndInner.transform.position = ptEnd + innerRadius * vecStart.normalized;
            objEndOutter.transform.position = ptEnd + outterRadius * vecStart.normalized;

            int devide = 30;//圆划分为多少等分
            float angleStep = 360.0f / (float)devide;

            float ang = 0.0f;
            for (ang = 0.0f; ang < 360.0f; ang += angleStep)
            {
                objStartInner.transform.RotateAround(ptStart, vec1, angleStep);
                objStartOutter.transform.RotateAround(ptStart, vec1, angleStep);
                objEndInner.transform.RotateAround(ptEnd, vec1, angleStep);
                objEndOutter.transform.RotateAround(ptEnd, vec1, angleStep);

                pointsStartInner.Add(objStartInner.transform.position);
                pointsStartOutter.Add(objStartOutter.transform.position);
                pointsEndtInner.Add(objEndInner.transform.position);
                pointsEndOutter.Add(objEndOutter.transform.position);
            }
            GameObject.DestroyImmediate(objStartInner);
            GameObject.DestroyImmediate(objStartOutter);
            GameObject.DestroyImmediate(objEndInner);
            GameObject.DestroyImmediate(objEndOutter);

            //构建曲面
            List<Vector3> vertexs = new List<Vector3>();
            vertexs.AddRange(pointsStartInner);//开始面内圆点
            vertexs.AddRange(pointsEndtInner); //结束面内圆点
            vertexs.AddRange(pointsStartOutter);//开始面外圆点
            vertexs.AddRange(pointsEndOutter);////结束面外圆点

            List<int> triangles = new List<int>();
            //构建内表面
            int startIndex = 0 * devide;
            int EndIndex = 0 * devide + devide;
            for (int i = startIndex; i < EndIndex; i++)
            {
                //边界面处
                int iNext = i + 1;
                int dNext = i + devide + 1;
                if (iNext >= startIndex + devide)
                    iNext = startIndex;

                if (dNext >= startIndex + 2 * devide)
                    dNext = startIndex + devide;

                triangles.Add(i);
                triangles.Add(i + devide);
                triangles.Add(iNext);

                triangles.Add(iNext);
                triangles.Add(i + devide);
                triangles.Add(dNext);
            }

            //构建外表面
            startIndex = 2 * devide;
            EndIndex = 2 * devide + devide;
            for (int i = startIndex; i < EndIndex; i++)
            {
                //边界面处
                int iNext = i + 1;
                int dNext = i + devide + 1;
                if (iNext >= startIndex + devide)
                    iNext = startIndex;

                if (dNext >= startIndex + 2 * devide)
                    dNext = startIndex + devide;

                triangles.Add(i);
                triangles.Add(iNext);
                triangles.Add(i + devide);

                triangles.Add(iNext);
                triangles.Add(dNext);
                triangles.Add(i + devide);
            }

            //构建上表面
            startIndex = 0 * devide;
            EndIndex = 0 * devide + devide;
            for (int i = startIndex; i < EndIndex; i++)
            {
                //边界面处
                int iNext = i + 1;
                int dNext = i + 2 * devide + 1;
                if (iNext >= startIndex + devide)
                    iNext = startIndex;

                if (dNext >= startIndex + 3 * devide)
                    dNext = startIndex + 2 * devide;

                triangles.Add(i);
                triangles.Add(iNext);
                triangles.Add(i + 2 * devide);

                triangles.Add(iNext);
                triangles.Add(dNext);
                triangles.Add(i + 2 * devide);
            }

            //构建下表面
            startIndex = 1 * devide;
            EndIndex = 1 * devide + devide;
            for (int i = startIndex; i < EndIndex; i++)
            {
                //边界面处
                int iNext = i + 1;
                int dNext = i + 2 * devide + 1;
                if (iNext >= startIndex + devide)
                    iNext = startIndex;

                if (dNext >= startIndex + 3 * devide)
                    dNext = startIndex + 2 * devide;

                triangles.Add(i);
                triangles.Add(i + 2 * devide);
                triangles.Add(iNext);

                triangles.Add(iNext);
                triangles.Add(i + 2 * devide);
                triangles.Add(dNext);
            }
            chunkMesh.vertices = vertexs.ToArray();
            chunkMesh.triangles = triangles.ToArray();

            chunkMesh.RecalculateNormals();
            chunkMesh.RecalculateBounds();
            if (!Directory.Exists(meshPrefabPath))
                Directory.CreateDirectory(meshPrefabPath);
            AssetDatabase.CreateAsset(chunkMesh, meshPrefabPath + meshName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            chunkMesh = AssetDatabase.LoadAssetAtPath<Mesh>(meshPrefabPath + meshName);
        }

        GameObject obj = new GameObject("HollowClinder");
        MeshFilter mf = obj.AddComponent<MeshFilter>();
        MeshRenderer mr = obj.AddComponent<MeshRenderer>();
        mr.material = new Material(Shader.Find("Standard"));
        mf.sharedMesh = chunkMesh;
        return obj;
    }
}