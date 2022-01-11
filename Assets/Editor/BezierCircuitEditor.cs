using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomEditor(typeof(BezierCircuit))]
    public class BezierCircuitEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("circuitParts"));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Init", "ButtonLeft")) {
                var circuit = serializedObject.targetObject as BezierCircuit;
                Debug.Assert(circuit != null, nameof(circuit) + " != null");
                circuit.Init();
                EditorUtility.SetDirty(circuit);
            }

            if (GUILayout.Button("Clear", "ButtonRight")) {
                var circuit = serializedObject.targetObject as BezierCircuit;
                Debug.Assert(circuit != null, nameof(circuit) + " != null");
                circuit.Clear();
                EditorUtility.SetDirty(circuit);
            }
            GUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
