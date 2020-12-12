using System.Collections.Generic;
using UnityEngine;
// ReSharper disable TooWideLocalVariableScope

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public static class BezierUtils
    {
        public class VectorFrame {
            public Vector3 Origin { get; }

            public Vector3 Tangent { get; }

            public Vector3 RotationAxis { get; set; }

            public Vector3 Normal { get; set; }

            public VectorFrame(Vector3 origin, Vector3 tangent, Vector3 rotAxis, Vector3 normal) {
                Origin = origin;
                Tangent = tangent;
                RotationAxis = rotAxis;
                Normal = normal;
            }
        }

        public static List<VectorFrame> GenerateRotationMinimisingFrames(this BezierCurve be) {
            int steps = be.divisions;
            var frames = new List<VectorFrame>();
            float step = 1.0f / steps;
            float t0, t1, c1, c2;
            Vector3 v1, v2, riL, tiL;
            VectorFrame x0, x1;
                
            frames.Add(be.GetFrenetFrame(0));
            for (t0 = 0; t0 < 1.0f; t0 += step) {
                // start with previous frame
                x0 = frames[frames.Count - 1];
                    
                // get the next frame -> throw away its axis and normal
                t1 = t0 + step;
                x1 = be.GetFrenetFrame(t1);
                    
                // we reflect x0 tangent & axis onto x1, through the plane of reflection at the point between x0, x1
                v1 = x1.Origin - x0.Origin;
                c1 = v1.sqrMagnitude;    // square magnitude ?
                riL = x0.RotationAxis - v1 * 2 / c1 * Vector3.Dot(v1, x0.RotationAxis);
                tiL = x0.Tangent - v1 * 2 / c1 * Vector3.Dot(v1, x0.Tangent);
                    
                // 2nd time reflection, over a plane at x1 so that the frame is aligned with the curve tangent
                v2 = x1.Tangent - tiL;
                c2 = v2.sqrMagnitude;
                x1.RotationAxis = riL - v2 * 2 / c2 * Vector3.Dot(v2, riL);
                x1.Normal = Vector3.Cross(x1.RotationAxis, x1.Tangent);
                frames.Add(x1);
            }

            frames.RemoveAt(frames.Count - 1);

            return frames;
        }
        
        private static VectorFrame GetFrenetFrame(this BezierCurve be, float t) {
            return new VectorFrame(be.GetBezierPos(t), be.Tangent(t).normalized, be.RotationAxis(t).normalized, be.Normal(t).normalized);
        }

        private static Vector3 ComputeBezier (float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
            return a * Mathf.Pow(1 - t, 3) + 3 * b * Mathf.Pow(1 - t, 2) * t + 3 * c * (1 - t) * (t * t) +
                   d * (t * t * t);
        }

        private static Vector3 GetBezierPos(this BezierCurve be, float t) {
            return ComputeBezier(t, be.startPoint, be.startTangent, be.endTangent, be.endPoint);
        }
        
        private static float ComputeBezierDerivative (float t, float a, float b, float c, float d) {
            a = 3 * (b - a);
            b = 3 * (c - b);
            c = 3 * (d - c);
            return a * Mathf.Pow(1-t, 2) + 2 * b * (1 - t) * t + c * (t*t);
        }

        private static Vector3 Tangent(this BezierCurve be, float t) {
            float x = ComputeBezierDerivative(t, be.startPoint.x, be.startTangent.x, be.endTangent.x, be.endPoint.x);
            float y = ComputeBezierDerivative(t, be.startPoint.y, be.startTangent.y, be.endTangent.y, be.endPoint.y);
            float z = ComputeBezierDerivative(t, be.startPoint.z, be.startTangent.z, be.endTangent.z, be.endPoint.z);
            return new Vector3(x, y, z).normalized;
        }

        private static Vector3 ComputeBezierDoubleDerivative(BezierCurve be, float t) {
            return 6 * (1 - t) * (be.endTangent - 2 * be.startTangent + be.startPoint) +
                   6 * t * (be.endPoint - 2 * be.endTangent + be.startTangent);
        }

        private static Vector3 Normal(this BezierCurve be, float t) {
            Vector3 a = be.Tangent(t);
            Vector3 r = be.RotationAxis(t);
            return Vector3.Cross(r, a).normalized;
        }

        private static Vector3 RotationAxis(this BezierCurve be, float t) {
            Vector3 a = be.Tangent(t);
            Vector3 b = (a + ComputeBezierDoubleDerivative(be, t)).normalized;
            return Vector3.Cross(b, a).normalized;
        }
    }
}
