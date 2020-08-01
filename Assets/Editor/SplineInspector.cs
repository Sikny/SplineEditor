using System;
using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomEditor(typeof(Spline))]
    public class SplineInspector : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            var list = serializedObject.FindProperty("points");
            
            EditorGUILayout.PropertyField(list, false);
            EditorGUI.indentLevel += 1;
            if (list.isExpanded) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Length : " + list.FindPropertyRelative("Array.size").intValue);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button(new GUIContent("+", "Add"), EditorStyles.miniButton, 
                    GUILayout.Width(30))) {
                    //++list.arraySize;
                    ((Spline) serializedObject.targetObject).AddPoint();
                }
                GUILayout.EndHorizontal();
                int digits = (int) Math.Floor(Math.Log10(list.arraySize-1) + 1f);
                if (digits < 0) digits = 1;
                for (int i = 0; i < list.arraySize; i++) {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("" + i, GUILayout.Width(20f + digits * 8f));
                    EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), GUIContent.none);
                    if (GUILayout.Button(new GUIContent("-", "Remove element"), EditorStyles.miniButton,
                        GUILayout.Width(30))) {
                        ((Spline) serializedObject.targetObject).RemovePoint(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUI.indentLevel -= 1;
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}