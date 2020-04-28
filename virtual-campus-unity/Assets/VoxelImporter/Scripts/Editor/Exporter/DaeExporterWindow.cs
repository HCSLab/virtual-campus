using UnityEngine;
using UnityEditor;
using System;
using System.Collections;

namespace VoxelImporter
{
    public class DaeExporterWindow : EditorWindow
    {
        public static DaeExporterWindow instance { get; private set; }

        public static bool exportMesh = true;
        public static bool exportAnimation = true;
        public static bool enableFootIK = true;
        private static bool human;
        private static Action onExport;

        public static void Open(bool human, Action onExport)
        {
            DaeExporterWindow.human = human;
            DaeExporterWindow.onExport = onExport;

            if (instance == null)
            {
                instance = CreateInstance<DaeExporterWindow>();
            }
            instance.titleContent = new GUIContent("Collada Exporter");
            instance.minSize = instance.maxSize = new Vector2(180, 80);
            instance.ShowAuxWindow();
        }

        void OnGUI()
        {
            {
                EditorGUI.BeginChangeCheck();
                var flag = EditorGUILayout.Toggle("Export Mesh", exportMesh);
                if (EditorGUI.EndChangeCheck())
                {
                    exportMesh = flag;
                }
            }
            {
                EditorGUI.BeginChangeCheck();
                var flag = EditorGUILayout.Toggle("Export Animation", exportAnimation);
                if (EditorGUI.EndChangeCheck())
                {
                    exportAnimation = flag;
                }
            }
            if (human)
            {
                EditorGUI.indentLevel++;
                EditorGUI.BeginDisabledGroup(!exportAnimation);
                EditorGUI.BeginChangeCheck();
                var flag = EditorGUILayout.Toggle(new GUIContent("Foot IK", "Activates feet IK bake."), enableFootIK);
                if (EditorGUI.EndChangeCheck())
                {
                    enableFootIK = flag;
                }
                EditorGUI.EndDisabledGroup();
                EditorGUI.indentLevel--;
            }
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Export"))
                {
                    EditorApplication.delayCall += () =>
                    {
                        if (onExport != null)
                            onExport.Invoke();
                        onExport = null;
                    };
                    Close();
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
            }

            if (Event.current.keyCode == KeyCode.Escape)
            {
                Close();
                return;
            }
        }
    }
}
