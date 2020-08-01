using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class ControlPoint : MonoBehaviour {
        [HideInInspector] public SplinePoint splinePoint;

        private void OnDrawGizmos() {
            if (Selection.activeTransform is null || !Selection.activeTransform.IsChildOf(splinePoint.transform)
                || splinePoint.isLast) return;
            
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(transform.position, SplineUtils.ControlPointSize);
        }
    }
}