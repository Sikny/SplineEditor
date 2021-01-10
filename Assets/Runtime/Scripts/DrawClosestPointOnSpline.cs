using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class DrawClosestPointOnSpline : MonoBehaviour {
        public BezierSpline spline;

        private void OnDrawGizmos() {
            if (spline == null) return;
            Vector3 position = transform.position;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(spline.GetClosestPoint(position), 0.1f);
        }
    }
}
