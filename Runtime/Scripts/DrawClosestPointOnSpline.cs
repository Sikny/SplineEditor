using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class DrawClosestPointOnSpline : MonoBehaviour {
        public BezierSpline spline;

        public Vector3 closestPosition;
        public float distToClosest;
        private void OnDrawGizmos() {
            if (spline == null) return;
            Vector3 position = transform.position;
            Gizmos.color = Color.red;
            BezierUtils.BezierPos pos = spline.GetClosestBezierPos(position);
            Gizmos.DrawSphere(pos.GlobalOrigin, 0.1f);
            Gizmos.DrawLine(pos.GlobalOrigin, transform.position);
            closestPosition = pos.GlobalOrigin;
            distToClosest = Vector3.Distance(transform.position, pos.GlobalOrigin);
        }
    }
}
