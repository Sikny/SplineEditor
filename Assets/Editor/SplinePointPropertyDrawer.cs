﻿using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomPropertyDrawer(typeof(SplinePoint))]
    public class SplinePointPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float labelWidth = 100f;
            
            var vectorRect = new Rect(position.x, position.y, position.width, position.height);
            
            var ctrl1LblRect = new Rect(position.x, position.y, labelWidth, position.height);
            var ctrl1Rect = new Rect(position.x + labelWidth, position.y + 22f, position.width - labelWidth, position.height);
            
            var ctrl2LblRect = new Rect(position.x, position.y + 22f, labelWidth, position.height);
            var ctrl2Rect = new Rect(position.x + labelWidth, position.y + 44f, position.width - labelWidth, position.height);
            
            SplinePoint splinePoint = (SplinePoint) property.objectReferenceValue;
            if(splinePoint == null)
                EditorGUI.LabelField(vectorRect, "Null Reference");
            else {
                splinePoint.transform.localPosition =
                    EditorGUI.Vector3Field(vectorRect, GUIContent.none, splinePoint.transform.localPosition);
                
                EditorGUI.LabelField(ctrl1LblRect, "Control point 1");
                splinePoint.controlPoint1.localPosition =
                    EditorGUI.Vector3Field(ctrl1Rect, GUIContent.none, splinePoint.controlPoint1.localPosition);
                EditorGUI.LabelField(ctrl2LblRect, "Control point 2");
                splinePoint.controlPoint2.localPosition =
                    EditorGUI.Vector3Field(ctrl2Rect, GUIContent.none, splinePoint.controlPoint2.localPosition);
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return base.GetPropertyHeight(property, label) * 3f + 10f;
        }
    }
}