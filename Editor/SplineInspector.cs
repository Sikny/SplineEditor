using System;
using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomEditor(typeof(Spline))]
    public class SplineInspector : UnityEditor.Editor {
        private readonly Color _red = Color.red;
        private readonly Color _entryBg = new Color(0.5f,0.5f,0.5f);

        private Color _tmp;
        public override void OnInspectorGUI() {
            serializedObject.Update();
            var list = serializedObject.FindProperty("points");

            EditorGUILayout.PropertyField(list, false);
            EditorGUI.indentLevel += 1;
            if (list.isExpanded) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Length : " + list.FindPropertyRelative("Array.size").intValue);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+", GUILayout.Width(100))) {
                    ((Spline) serializedObject.targetObject).AddPoint();
                }
                GUILayout.EndHorizontal();
                int digits = (int) Math.Floor(Math.Log10(list.arraySize-1) + 1f);
                if (digits < 0) digits = 1;
                for (int i = 0; i < list.arraySize; i++) {
                    GUIStyle style = new GUIStyle();
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, _entryBg);
                    tex.Apply();
                    style.normal.background = tex;
                    style.margin.bottom = 5;
                    EditorGUILayout.BeginHorizontal(style);
                    EditorGUILayout.LabelField("" + i, GUILayout.Width(20f + digits * 8f));
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
                    _tmp = GUI.backgroundColor;
                    GUI.backgroundColor = _red;
                    if (GUILayout.Button("-", GUILayout.Width(30))) {
                        ((Spline) serializedObject.targetObject).RemovePoint(i);
                    }
                    GUI.backgroundColor = _tmp;
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel -= 1;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}