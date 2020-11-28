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
            Handles.DrawDottedLine(be.startPoint + bezierPosition, be.startTangent + bezierPosition, 2);
            Handles.DrawDottedLine(be.endPoint + bezierPosition, be.endTangent + bezierPosition, 2);
            
            Handles.color = be.settings.bezierPointColor;
            if (Handles.Button(be.startPoint + bezierPosition, Quaternion.identity, 0.1f, 0.1f, Handles.CubeHandleCap)) {
                _selectedPoint = 0;
            }
            if (Handles.Button(be.endPoint + bezierPosition, Quaternion.identity, 0.1f, 0.1f, Handles.CubeHandleCap)) {
                _selectedPoint = 1;
            }
            Handles.color = be.settings.bezierControlPointColor;
            if (Handles.Button(be.startTangent + bezierPosition, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap)) {
                _selectedPoint = 2;
            }
            if (Handles.Button(be.endTangent + bezierPosition, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap)) {
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
                }
            }
        }

        private Vector3? GetSelectedPoint(BezierCurve be) {
            switch (_selectedPoint) {
                case 0:
                    return be.startPoint;
                case 1:
                    return be.endPoint;
                case 2:
                    return be.startTangent;
                case 3:
                    return be.endTangent;
                default:
                    return null;
            }
        }

        private void SetSelectedPoint(BezierCurve be, Vector3 position) {
            switch (_selectedPoint) {
                case 0:
                    be.startPoint = position;
                    break;
                case 1:
                    be.endPoint = position;
                    break;
                case 2:
                    be.startTangent = position;
                    break;
                case 3:
                    be.endTangent = position;
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