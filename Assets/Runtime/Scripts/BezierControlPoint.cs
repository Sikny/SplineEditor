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

        private void OnDrawGizmosSelected()
        {
            if (transform.parent == null) return;
            BezierNode node = GetComponentInParent<BezierNode>();
            node.UpdateMirrorPos(transform);
        }
    }
}
