using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomPropertyDrawer(typeof(SplinePoint))]
    public class SplinePointPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            
            var vectorRect = new Rect(position.x, position.y, position.width, position.height);
            SplinePoint splinePoint = (SplinePoint) property.objectReferenceValue;
            if(splinePoint == null)
                EditorGUI.LabelField(vectorRect, "Null Reference");
            else
                splinePoint.transform.position = EditorGUI.Vector3Field(vectorRect, GUIContent.none, splinePoint.transform.position);

            EditorGUI.EndProperty();
        }
    }
}