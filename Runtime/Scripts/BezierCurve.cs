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
        private readonly List<Vector3> _rotAxis = new List<Vector3>();

        [HideInInspector] public Vector3 lastPosition;

        private void OnValidate() {
            RecalculatePositions();
        }

        public void RecalculatePositions() {
            _positions.Clear();
            _tangents.Clear();
            _normals.Clear();
            _rotAxis.Clear();
            List<BezierUtils.VectorFrame> vectorFrames = this.GenerateRotationMinimisingFrames();
            int arrayLen = vectorFrames.Count;
            for (int i = 0; i < arrayLen; ++i) {
                _positions.Add(transform.position + vectorFrames[i].Origin);
                _tangents.Add(vectorFrames[i].Tangent);
                _normals.Add(vectorFrames[i].Normal);
                _rotAxis.Add(vectorFrames[i].RotationAxis);
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
                for (int i = 0; i < _rotAxis.Count; ++i) {
                    Handles.DrawLine(_positions[i], _positions[i] + _rotAxis[i]);
                }
            }
        }
#endif
    }
}