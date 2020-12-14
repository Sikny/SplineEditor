using SplineEditor.Runtime;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveEditor : UnityEditor.Editor {
        private int _selectedPoint; // 0, 1 -> start/end point; 2, 3 -> start/end tangent

        private void OnSceneGUI() {
            BezierCurve be = target as BezierCurve;
            if (be == null) {
                Debug.LogError("Possible NullReferenceException on BezierCurve");
                return;
            }
            var bezierPosition = be.transform.position;

            Handles.color = be.settings.tangentLinesColor;
            Handles.DrawDottedLine(be.startPoint.position + bezierPosition, be.startPoint.tangent + bezierPosition, 2);
            Handles.DrawDottedLine(be.endPoint.position + bezierPosition, be.endPoint.tangent + bezierPosition, 2);
            
            Handles.color = be.settings.bezierPointColor;
            if (Handles.Button(be.startPoint.position + bezierPosition, Quaternion.identity, 0.1f, 0.1f, Handles.CubeHandleCap)) {
                _selectedPoint = 0;
            }
            if (Handles.Button(be.endPoint.position + bezierPosition, Quaternion.identity, 0.1f, 0.1f, Handles.CubeHandleCap)) {
                _selectedPoint = 1;
            }
            Handles.color = be.settings.bezierControlPointColor;
            if (Handles.Button(be.startPoint.tangent + bezierPosition, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap)) {
                _selectedPoint = 2;
            }
            if (Handles.Button(be.endPoint.tangent + bezierPosition, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap)) {
                _selectedPoint = 3;
            }
            
            var position = GetSelectedPoint(be);
            if (position.HasValue) {
                EditorGUI.BeginChangeCheck();
                var newPos = Handles.PositionHandle(position.Value + bezierPosition, Quaternion.identity)
                             - bezierPosition;

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
            switch (_selectedPoint) {
                case 0:
                    return be.startPoint.position;
                case 1:
                    return be.endPoint.position;
                case 2:
                    return be.startPoint.tangent;
                case 3:
                    return be.endPoint.tangent;
                default:
                    return null;
            }
        }

        private void SetSelectedPoint(BezierCurve be, Vector3 position) {
            switch (_selectedPoint) {
                case 0:
                    be.startPoint.position = position;
                    break;
                case 1:
                    be.endPoint.position = position;
                    break;
                case 2:
                    be.startPoint.tangent = position;
                    break;
                case 3:
                    be.endPoint.tangent = position;
                    break;
            }
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