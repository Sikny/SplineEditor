﻿#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class BezierCurve : MonoBehaviour {
        public BezierSettings settings;

        public List<BezierControlPoint> controlPoints;

        public int divisionsBetweenTwoPoints = 10;

        private readonly List<Vector3> _positions = new List<Vector3>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        private readonly List<Vector3> _rotAxis = new List<Vector3>();

        [HideInInspector] public Vector3 lastPosition;

        public UnityEvent onBezierChanged;

        private void OnValidate() {
            RecalculatePositions();
        }

        public void RecalculatePositions() {
            _positions.Clear();
            _normals.Clear();
            _rotAxis.Clear();
            List<BezierUtils.VectorFrame> vectorFrames = this.GenerateRotationMinimisingFrames();
            int arrayLen = vectorFrames.Count;
            for (int i = 0; i < arrayLen; ++i) {
                _positions.Add(transform.position + vectorFrames[i].Origin);
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