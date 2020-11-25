using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class ControlPoint : MonoBehaviour {
        [HideInInspector] public BezierPoint bezierPoint;

        private readonly Color _gizmoColor = Color.white;
        private void OnDrawGizmos() {
            if (Selection.activeTransform is null || !Selection.activeTransform.IsChildOf(bezierPoint.transform)
                || bezierPoint.isLast) return;
            
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(transform.position, BezierUtils.ControlPointSize);
        }
    }
}