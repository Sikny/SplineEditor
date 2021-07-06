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

        private void OnValidate()
        {
            if (spline == null) return;
            BezierUtils.BezierPos bezierPos = spline.GetBezierPos(distance);
            Transform t = transform;
            t.position = bezierPos.GlobalOrigin + bezierPos.Normal * horizontalOffset + bezierPos.LocalUp * verticalOffset;
            t.rotation = bezierPos.Rotation;
        }
        
        [ContextMenu("Compute Distance From Position")]
        private void ComputeDistanceFromPosition(){
            distance = spline.GetClosestBezierPos(transform.position).BezierDistance;
            OnValidate();
        }
    }
}
