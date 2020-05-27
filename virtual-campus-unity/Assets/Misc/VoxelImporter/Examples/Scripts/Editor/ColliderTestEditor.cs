using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditorInternal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace VoxelImporter
{
	[CustomEditor(typeof(ColliderTest))]
	public class ColliderTestEditor : Editor
	{
		public ColliderTest test { get; protected set; }

		void OnEnable()
		{
			test = target as ColliderTest;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			//GameObject
			{
				EditorGUI.BeginChangeCheck();
				var addObject = (GameObject)EditorGUILayout.ObjectField("GameObject", test.addObject, typeof(GameObject), false);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(test, "Inspector");
					test.addObject = addObject;
				}
			}
			//Primitive
			if (test.addObject == null)
			{
				EditorGUI.BeginChangeCheck();
				var primitive = (ColliderTest.Primitive)EditorGUILayout.EnumPopup("Primitive", test.primitive);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(test, "Inspector");
					test.primitive = primitive;
				}
			}
			//AutoBirth
			{
				EditorGUI.BeginChangeCheck();
				var autoBirth = EditorGUILayout.Toggle("Auto Birth", test.autoBirth);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(test, "Inspector");
					test.autoBirth = autoBirth;
				}
				//Sepalate Time
				EditorGUI.indentLevel++;
				EditorGUI.BeginDisabledGroup(!test.autoBirth);
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUI.BeginChangeCheck();
					var min = test.sepalateTimeMin;
					var max = test.sepalateTimeMax;
					EditorGUILayout.MinMaxSlider(new GUIContent("Sepalate Time"), ref min, ref max, 0, 10);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(test, "Inspector");
						test.sepalateTimeMin = min;
						test.sepalateTimeMax = max;
					}
				}
				{
					EditorGUI.BeginChangeCheck();
					var sepalateTimeMin = EditorGUILayout.FloatField(test.sepalateTimeMin, GUILayout.Width(48));
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(test, "Inspector");
						test.sepalateTimeMin = sepalateTimeMin;
					}
				}
				{
					EditorGUI.BeginChangeCheck();
					var sepalateTimeMax = EditorGUILayout.FloatField(test.sepalateTimeMax, GUILayout.Width(48));
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(test, "Inspector");
						test.sepalateTimeMax = sepalateTimeMax;
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUI.EndDisabledGroup();
				EditorGUI.indentLevel--;
			}
			//Random
			{
				EditorGUILayout.LabelField("Random");
				EditorGUI.indentLevel++;
				{
					EditorGUI.BeginChangeCheck();
					var randomRadius = EditorGUILayout.FloatField("Radius", test.randomRadius);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(test, "Inspector");
						test.randomRadius = randomRadius;
					}
				}
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUI.BeginChangeCheck();
					var min = test.randomScaleMin;
					var max = test.randomScaleMax;
					EditorGUILayout.MinMaxSlider(new GUIContent("Scale"), ref min, ref max, 1f, 3f);
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(test, "Inspector");
						test.randomScaleMin = min;
						test.randomScaleMax = max;
					}
				}
				{
					EditorGUI.BeginChangeCheck();
					var randomScaleMin = EditorGUILayout.FloatField(test.randomScaleMin, GUILayout.Width(48));
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(test, "Inspector");
						test.randomScaleMin = randomScaleMin;
					}
				}
				{
					EditorGUI.BeginChangeCheck();
					var randomScaleMax = EditorGUILayout.FloatField(test.randomScaleMax, GUILayout.Width(48));
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(test, "Inspector");
						test.randomScaleMax = randomScaleMax;
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}

			//GroundY
			{
				EditorGUI.BeginChangeCheck();
				var groundY = EditorGUILayout.FloatField("GroundY", test.groundY);
				if (EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject(test, "Inspector");
					test.groundY = groundY;
				}
			}

			//AddButton
			{
				EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);
				if(GUILayout.Button("Add Object", GUILayout.Height(32)))
				{
					test.Add();
				}
				EditorGUI.EndDisabledGroup();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
