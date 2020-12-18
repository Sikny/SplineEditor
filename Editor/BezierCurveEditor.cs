using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveEditor : UnityEditor.Editor {
        private int _selectedPoint; // 0, 1 -> start/end point; 2, 3 -> start/end tangent
        private int _selectedTangent;

        private void OnSceneGUI() {
            BezierCurve be = target as BezierCurve;
            if (be == null) {
                Debug.LogError("Possible NullReferenceException on BezierCurve");
                return;
            }
            var bezierPosition = be.transform.position;

            for (int i = 0; i < be.controlPoints.Count; ++i) {
                Vector3 currentPos = be.controlPoints[i].position;
                Vector3 currentTan1 = be.controlPoints[i].Tangent1;
                Vector3 currentTan2 = be.controlPoints[i].Tangent2;
                Handles.color = be.settings.tangentLinesColor;
                Handles.DrawLine(currentPos + bezierPosition, currentTan1 + bezierPosition);
                Handles.DrawLine(currentPos + bezierPosition, currentTan2 + bezierPosition);

                Handles.color = be.settings.bezierPointColor;
                if (Handles.Button(currentPos + bezierPosition, Quaternion.identity, 0.1f, 0.1f,
                    Handles.CubeHandleCap)) {
                    _selectedPoint = i;
                    _selectedTangent = 0;
                }

                Handles.color = be.settings.bezierControlPointColor;
                if (Handles.Button(currentTan1 + bezierPosition, Quaternion.identity, 0.1f, 0.1f,
                    Handles.SphereHandleCap)) {
                    _selectedPoint = i;
                    _selectedTangent = 1;
                }
                
                if (Handles.Button(currentTan2 + bezierPosition, Quaternion.identity, 0.1f, 0.1f,
                    Handles.SphereHandleCap)) {
                    _selectedPoint = i;
                    _selectedTangent = 2;
                }
            }

            var position = GetSelectedPoint(be);
            if (position.HasValue) {
                EditorGUI.BeginChangeCheck();
                var newPos = Handles.PositionHandle(position.Value + bezierPosition, Quaternion.identity) - bezierPosition;

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(target, "Changed Bezier point");
                    SetSelectedPoint(be, newPos);
                    be.RecalculatePositions();
                }
            }

            if (be.lastPosition != be.transform.position) {
                be.lastPosition = be.transform.position;
                be.RecalculatePositions();
            }
        }

        private Vector3? GetSelectedPoint(BezierCurve be) {
            if (_selectedPoint > be.controlPoints.Count - 1) _selectedPoint = 0;
            if (_selectedTangent == 1)
                return be.controlPoints[_selectedPoint].Tangent1;
            if (_selectedTangent == 2)
                return be.controlPoints[_selectedPoint].Tangent2;
            return be.controlPoints[_selectedPoint].position;
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