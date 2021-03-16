using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineEditor : UnityEditor.Editor {
        private UnityEditor.Editor _editor;

        private bool _showSettings;
        private bool _isEditorNull;

        private void Awake()
        {
            _isEditorNull = _editor == null;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings"), GUIContent.none);
            _showSettings = EditorGUILayout.Foldout(_showSettings, "Settings");
            if (_showSettings) {
                if (_isEditorNull) {
                    CreateCachedEditor(serializedObject.FindProperty("settings").objectReferenceValue, null,
                        ref _editor);
                    _isEditorNull = _editor == null;
                }

                _editor.OnInspectorGUI();
            }
            
            serializedObject.ApplyModifiedProperties();
            DrawDefaultInspector();
        }
    }
}