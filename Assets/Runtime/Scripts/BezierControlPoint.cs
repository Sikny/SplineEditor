using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    public class BezierControlPoint : MonoBehaviour
    {
        public BezierSettings bezierSettings;
        private void OnDrawGizmos()
        {
            Gizmos.color = bezierSettings.bezierControlPointColor;
            float gizmoSize = bezierSettings.controlsHandleSize / 2;
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (Selection.activeObject != gameObject || transform.parent == null) return;
            BezierNode node = GetComponentInParent<BezierNode>();
            node.UpdateMirrorPos(transform);
        }
        #endif
    }
}
