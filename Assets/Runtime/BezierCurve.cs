using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class BezierCurve : MonoBehaviour {
        public List<BezierPoint> points = new List<BezierPoint>();

        public void AddPoint() {
            GameObject splinePointObject = new GameObject("SplinePoint");
            splinePointObject.transform.parent = transform;
            BezierPoint bezierPoint = splinePointObject.AddComponent<BezierPoint>();
            bezierPoint.bezierCurve = this;
            bezierPoint.Init();
            bezierPoint.isLast = true;
            UpdateLast();
            points.Add(bezierPoint);
        }

        public void RemovePoint(int index) {
            if (points[index] != null) {
                if (Application.isPlaying) Destroy(points[index].gameObject);
                else DestroyImmediate(points[index].gameObject);
            }
            UpdateLast();
        }

        public void UpdateLast() {
            if(points.Count > 0)
                points[points.Count - 1].isLast = false;
        }
        
        private Color _bezierCurveColor = Color.white;
        private void OnDrawGizmos() {
            int pointCount = points.Count;
            for (int i = 0; i < pointCount; ++i) {
                if (i < pointCount - 1) {
                    Handles.DrawBezier(points[i].transform.position, 
                        points[i+1].transform.position, points[i].controlPoint1.position, 
                        points[i].controlPoint2.position, _bezierCurveColor, null, 2f);
                }
            }
        }
    }
}