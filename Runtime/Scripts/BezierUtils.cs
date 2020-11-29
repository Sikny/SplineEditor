using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public static class BezierUtils
    {
        public static Vector3 ComputeBezier (float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
            return a * Mathf.Pow(1 - t, 3) + 3 * b * Mathf.Pow(1 - t, 2) * t + 3 * c * (1 - t) * (t * t) +
                   d * (t * t * t);
        }
        
        private static float ComputeBezierDerivative (float t, float a, float b, float c, float d) {
            a = 3 * (b - a);
            b = 3 * (c - b);
            c = 3 * (d - c);
            return a * Mathf.Pow(1-t, 2) + 2 * b * (1 - t) * t + c * (t*t);
        }

        public static Vector3 BezierTangent(BezierCurve be, float t) {
            float x = ComputeBezierDerivative(t, be.startPoint.x, be.startTangent.x, be.endTangent.x, be.endPoint.x);
            float y = ComputeBezierDerivative(t, be.startPoint.y, be.startTangent.y, be.endTangent.y, be.endPoint.y);
            float z = ComputeBezierDerivative(t, be.startPoint.z, be.startTangent.z, be.endTangent.z, be.endPoint.z);
            return new Vector3(x, y, z).normalized;
        }

        private static Vector3 ComputeBezierDoubleDerivative(BezierCurve be, float t) {
            return 6 * (1 - t) * (be.endTangent - 2 * be.startTangent + be.startPoint) +
                   6 * t * (be.endPoint - 2 * be.endTangent + be.startTangent);
        }

        public static Vector3 Normal(BezierCurve be, float t) {
            Vector3 a = BezierTangent(be, t);
            Vector3 r = -VerticalNormal(be, t);
            return Vector3.Cross(r, a).normalized;
        }

        public static Vector3 VerticalNormal(BezierCurve be, float t) {
            Vector3 a = BezierTangent(be, t);
            Vector3 b = (a + ComputeBezierDoubleDerivative(be, t)).normalized;
            Vector3 r = Vector3.Cross(b, a).normalized;
            return -r;
        }
    }
}
