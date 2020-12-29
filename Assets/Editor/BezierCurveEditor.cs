using System.Collections.Generic;
using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveEditor : UnityEditor.Editor {
        private int _selectedPoint; // 0, 1 -> start/end point; 2, 3 -> start/end tangent
        private int _selectedTangent;

        private UnityEditor.Editor _editor;

        public override void OnInspectorGUI() {
            var settingsProperty = serializedObject.FindProperty("settings");
            CreateCachedEditor(settingsProperty.objectReferenceValue, null, ref _editor);
            if (_editor != null)
                _editor.OnInspectorGUI();
            DrawDefaultInspector();
        }

        private void OnSceneGUI() {
            BezierCurve be = target as BezierCurve;
            if (be == null) {
                Debug.LogError("Possible NullReferenceException on BezierCurve");
                return;
            }

            if (be.settings == null) {
                Debug.LogError("Missing required Bezier Settings attribute");
                return;
            }
            
            var bezierTransform = be.transform;

            Handles.color = be.settings.bezierCurveColor;

            List<BezierUtils.VectorFrame> vectorFrames = be.GenerateRotationMinimisingFrames();
            int arrayLen = vectorFrames.Count;
            for (int i = 0; i < arrayLen; ++i) {
                var origin = bezierTransform.TransformPoint(vectorFrames[i].Origin);
                if (be.settings.showNormals) {
                    Handles.color = be.settings.normalsColor;
                    Handles.DrawLine(origin, origin + vectorFrames[i].Normal);
                }

                if (be.settings.showVerticalNormals) {
                    Handles.color = be.settings.verticalNormalsColor;
                    Handles.DrawLine(origin, origin + vectorFrames[i].RotationAxis);
                }
            }

            for (int i = 0; i < be.controlPoints.Count; ++i) {
                Vector3 currentPos = bezierTransform.TransformPoint(be.controlPoints[i].position);
                Vector3 currentTan1 = bezierTransform.TransformPoint(be.controlPoints[i].Tangent1);
                Vector3 currentTan2 = bezierTransform.TransformPoint(be.controlPoints[i].Tangent2);
                if (i < be.controlPoints.Count - 1)
                    Handles.DrawBezier(currentPos,
                        bezierTransform.TransformPoint(be.controlPoints[i + 1].position),
                        currentTan2, 
                        bezierTransform.TransformPoint(be.controlPoints[i + 1].Tangent1),
                        be.settings.bezierCurveColor, null, be.settings.bezierCurveWidth);
                Handles.color = be.settings.tangentLinesColor;
                Handles.DrawLine(currentPos, currentTan1);
                Handles.DrawLine(currentPos, currentTan2);

                Handles.color = be.settings.bezierPointColor;
                var handleSize = HandleUtility.GetHandleSize(currentPos) * be.settings.controlsHandleSize;
                var handlePos = currentPos;
                if (Handles.Button(handlePos, Quaternion.identity, handleSize, 
                    handleSize, Handles.CubeHandleCap)) {
                    _selectedPoint = i;
                    _selectedTangent = 0;
                }

                Handles.color = be.settings.bezierControlPointColor;
                handleSize = HandleUtility.GetHandleSize(currentTan1) * be.settings.controlsHandleSize;
                handlePos = currentTan1;
                if (Handles.Button(handlePos, Quaternion.identity, handleSize, 
                    handleSize, Handles.SphereHandleCap)) {
                    _selectedPoint = i;
                    _selectedTangent = 1;
                }
                handleSize = HandleUtility.GetHandleSize(currentTan2) * be.settings.controlsHandleSize;
                handlePos = currentTan2;
                if (Handles.Button(handlePos, Quaternion.identity, handleSize, 
                    handleSize, Handles.SphereHandleCap)) {
                    _selectedPoint = i;
                    _selectedTangent = 2;
                }
            }

            var position = GetSelectedPoint(be);
            if (position.HasValue) {
                EditorGUI.BeginChangeCheck();
                var newPos = bezierTransform.InverseTransformPoint(Handles.PositionHandle(bezierTransform.TransformPoint(position.Value),
                    Quaternion.identity));

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(target, "Changed Bezier point");
                    SetSelectedPoint(be, newPos);
                }
            }
        }

        private Vector3? GetSelectedPoint(BezierCurve be) {
            Vector3? result;
            if (_selectedPoint > be.controlPoints.Count - 1) _selectedPoint = 0;
            if (_selectedTangent == 1)
                result = be.controlPoints[_selectedPoint].Tangent1;
            else if (_selectedTangent == 2)
                result = be.controlPoints[_selectedPoint].Tangent2;
            else result = be.controlPoints[_selectedPoint].position;
            return result;
        }

        private void SetSelectedPoint(BezierCurve be, Vector3 position) {
            if (_selectedTangent == 1)
                be.controlPoints[_selectedPoint].Tangent1 = position;
            else if (_selectedTangent == 2)
                be.controlPoints[_selectedPoint].Tangent2 = position;
            else be.controlPoints[_selectedPoint].position = position;
        }

        void OnEnable() {
            Tools.hidden = true;
            _selectedPoint = 0;
        }

        void OnDisable() {
            Tools.hidden = false;
        }
    }
}