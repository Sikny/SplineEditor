#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class BezierCurve : MonoBehaviour {
        public BezierSettings settings;
        public Vector3 startPoint = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 endPoint = new Vector3(-2.0f, 2.0f, 0.0f);
        public Vector3 startTangent = new Vector3(-0.5f, 0.5f, 0.0f);
        public Vector3 endTangent = new Vector3(-1.5f, 1.5f, 0.0f);

        public int divisions = 10;

        private readonly List<Vector3> _positions = new List<Vector3>();
        private readonly List<Vector3> _tangents = new List<Vector3>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        private readonly List<Vector3> _vNormals = new List<Vector3>();

        [HideInInspector] public Vector3 lastPosition;

        private void OnValidate() {
            RecalculatePositions();
        }

        public void RecalculatePositions() {
            _positions.Clear();
            _tangents.Clear();
            _normals.Clear();
            _vNormals.Clear();
            for (float t = 0; t <= 1; t += 1f / divisions) {
                _positions.Add(transform.position + BezierUtils.ComputeBezier(t, startPoint, startTangent, endTangent, endPoint));
                _tangents.Add(BezierUtils.BezierTangent(this, t));
                _normals.Add(BezierUtils.Normal(this, t));
                _vNormals.Add(BezierUtils.VerticalNormal(this, t));
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos() {
            Handles.color = settings.bezierCurveColor;
            Handles.DrawAAPolyLine(settings.bezierCurveWidth, _positions.ToArray());
            if (settings.showNormals) {
                Handles.color = settings.normalsColor;
                for (int i = 0; i < _normals.Count; ++i) {
                    Handles.DrawLine(_positions[i], _positions[i] + _normals[i]);
                }
            }

            if (settings.showVerticalNormals) {
                Handles.color = settings.verticalNormalsColor;
                for (int i = 0; i < _vNormals.Count; ++i) {
                    Handles.DrawLine(_positions[i], _positions[i] + _vNormals[i]);
                }
            }
        }
#endif
    }
}