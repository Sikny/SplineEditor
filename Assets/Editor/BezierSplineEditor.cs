using SplineEditor.Runtime;
using UnityEditor;

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

            EditorGUILayout.PropertyField(serializedObject.FindProperty("settings"));
            _showSettings = EditorGUILayout.Foldout(_showSettings, "Show Settings");
            if (_showSettings) {
                if (_isEditorNull) {
                    CreateCachedEditor(serializedObject.FindProperty("settings").objectReferenceValue, null,
                        ref _editor);
                    _isEditorNull = _editor == null;
                }

                _editor.OnInspectorGUI();
            }
            EditorGUILayout.Space(10);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("loop"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bezierNodes"));
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty("constantSizeForDivisions"));
            //bSpline.useResolution = EditorGUILayout.Toggle("Use Resolution", bSpline.useResolution);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("divisionsBetweenTwoPoints"));

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bezierLength"));
            
            serializedObject.ApplyModifiedProperties();
            //DrawDefaultInspector();
        }
    }
}