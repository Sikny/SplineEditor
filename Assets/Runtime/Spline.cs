using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class Spline : MonoBehaviour {
        public List<SplinePoint> points = new List<SplinePoint>();

        public void AddPoint() {
            GameObject splinePointObject = new GameObject("SplinePoint");
            splinePointObject.transform.parent = transform;
            SplinePoint splinePoint = splinePointObject.AddComponent<SplinePoint>();
            splinePoint.spline = this;
            splinePoint.Init();
            splinePoint.isLast = true;
            UpdateLast();
            points.Add(splinePoint);
        }

        public void RemovePoint(int index) {
            if (points[index] != null) {
                if (Application.isPlaying) Destroy(points[index].gameObject);
                else DestroyImmediate(points[index].gameObject);
            }
            UpdateLast();
        }

        private void UpdateLast() {
            if(points.Count > 0)
                points[points.Count - 1].isLast = false;
        }
        
        private Color _bezierCurveColor = Color.white;
        private void OnDrawGizmos() {
            if (Selection.activeTransform is null || !Selection.activeTransform.IsChildOf(transform)) return;
            
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