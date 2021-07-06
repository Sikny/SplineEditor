using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    public class ObjectAlongSpline : MonoBehaviour
    {
        public BezierSpline spline;
        public float distance;
        public float horizontalOffset;
        public float verticalOffset;
        public bool computeDistanceFromPosition;

        [ContextMenu("Validate")]
        private void OnValidate()
        {
            if (spline == null) return;
            BezierUtils.BezierPos bezierPos;
            if (computeDistanceFromPosition) {
                bezierPos = ComputeBezierPosFromPosition();
                distance = bezierPos.BezierDistance;
            }
            else bezierPos = spline.GetBezierPos(distance);
            Transform t = transform;
            t.position = bezierPos.GlobalOrigin + bezierPos.Normal * horizontalOffset + bezierPos.LocalUp * verticalOffset;
            t.rotation = bezierPos.Rotation;
        }
        
        private BezierUtils.BezierPos ComputeBezierPosFromPosition(){
            return spline.GetClosestBezierPos(transform.position);
        }
    }
}
