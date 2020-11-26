using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class MeshExtrusion : MonoBehaviour {
        public BezierCurve bezierCurve;

        private List<Vector3> _positions = new List<Vector3>();
        [ContextMenu("Init")]
        private void Init() {
            _positions.Clear();
            for (int i = 0; i < bezierCurve.points.Count; ++i) {
                if (i == bezierCurve.points.Count - 1) break;
                var point1 = bezierCurve.points[i];
                var point2 = bezierCurve.points[i+1];
                _positions.AddRange(Handles.MakeBezierPoints(point1.transform.position, 
                    point2.transform.position, point1.controlPoint1.position, point1.controlPoint2.position, 10));
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            for (int i = 0; i < _positions.Count; ++i) {
                if(i < _positions.Count - 1)
                    Gizmos.DrawLine(_positions[i], _positions[i+1]);
            }
        }
    }
}
