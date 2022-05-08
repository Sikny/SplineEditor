// ReSharper disable once CheckNamespace

using UnityEngine;

namespace SplineEditor.Runtime {
    public struct NativeBezierPos {
        public Vector3 localOrigin;
        public Vector3 tangent;
        public Vector3 localUp;
        public Vector3 normal;
        public Quaternion rotation;
        public float bezierDistance;
        public float t;

        public NativeBezierPos(BezierUtils.BezierPos pos) {
            localOrigin = pos.LocalOrigin;
            tangent = pos.Tangent;
            localUp = pos.LocalUp;
            normal = pos.Normal;
            rotation = pos.Rotation;
            bezierDistance = pos.BezierDistance;
            t = pos.T;
        }
    }
}