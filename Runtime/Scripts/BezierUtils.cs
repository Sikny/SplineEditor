using System.Collections.Generic;
using UnityEngine;
// ReSharper disable TooWideLocalVariableScope

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public static class BezierUtils
    {
        public class VectorFrame {
            public Vector3 Origin { get; }

            public Vector3 Tangent { get; set; }

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
            List<VectorFrame> frames = new List<VectorFrame>();
            for (int i = 0; i < be.controlPoints.Count - 1; ++i) {
                frames.AddRange(GenerateRotationMinimisingFrames(be.controlPoints[i], 
                    be.controlPoints[i+1], be.divisions));
            }
            return frames;
        }
        
        public static List<VectorFrame> GenerateRotationMinimisingFrames(BezierControlPoint startPoint, BezierControlPoint endPoint, int divisions) {
            int steps = divisions;
            var frames = new List<VectorFrame>();
            float step = 1.0f / steps;
            float t0, t1, c1, c2;
            Vector3 v1, v2, riL, tiL;
            VectorFrame x0, x1;

            frames.Add(GetFrenetFrame(startPoint, endPoint, 0));
            for (t0 = 0; t0 < 1.0f; t0 += step) {
                // start with previous frame
                x0 = frames[frames.Count - 1];
                    
                // get the next frame -> throw away its axis and normal
                t1 = t0 + step;
                x1 = GetFrenetFrame(startPoint, endPoint, t1);
                    
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
        
        private static VectorFrame GetFrenetFrame(BezierControlPoint startPoint, BezierControlPoint endPoint, float t) {
            return new VectorFrame(GetBezierPos(startPoint, endPoint, t), Tangent(startPoint, endPoint, t).normalized, 
                RotationAxis(startPoint, endPoint, t).normalized, Normal(startPoint, endPoint, t).normalized);
        }

        private static Vector3 ComputeBezier (float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
            return a * Mathf.Pow(1 - t, 3) + b * (3 * Mathf.Pow(1 - t, 2) * t) + c * (3 * (1 - t) * (t * t)) +
                   d * (t * t * t);
        }

        private static Vector3 GetBezierPos(BezierControlPoint startPoint, BezierControlPoint endPoint, float t) {
            return ComputeBezier(t, startPoint.position, startPoint.Tangent2, endPoint.Tangent1, endPoint.position);
        }
        
        private static float ComputeBezierDerivative (float t, float a, float b, float c, float d) {
            a = 3 * (b - a);
            b = 3 * (c - b);
            c = 3 * (d - c);
            return a * Mathf.Pow(1-t, 2) + 2 * b * (1 - t) * t + c * (t*t);
        }

        private static Vector3 Tangent(BezierControlPoint startPoint, BezierControlPoint endPoint, float t) {
            float x = ComputeBezierDerivative(t, startPoint.position.x, startPoint.Tangent2.x, endPoint.Tangent1.x, endPoint.position.x);
            float y = ComputeBezierDerivative(t, startPoint.position.y, startPoint.Tangent2.y, endPoint.Tangent1.y, endPoint.position.y);
            float z = ComputeBezierDerivative(t, startPoint.position.z, startPoint.Tangent2.z, endPoint.Tangent1.z, endPoint.position.z);
            return new Vector3(x, y, z).normalized;
        }

        private static Vector3 ComputeBezierDoubleDerivative(BezierControlPoint startPoint, BezierControlPoint endPoint, float t) {
            return 6 * (1 - t) * (endPoint.Tangent1 - 2 * startPoint.Tangent2 + startPoint.position) +
                   6 * t * (endPoint.position - 2 * endPoint.Tangent1 + startPoint.Tangent2);
        }

        private static Vector3 Normal(BezierControlPoint startPoint, BezierControlPoint endPoint, float t) {
            Vector3 a = Tangent(startPoint, endPoint, t);
            Vector3 r = RotationAxis(startPoint, endPoint, t);
            return Vector3.Cross(r, a).normalized;
        }

        private static Vector3 RotationAxis(BezierControlPoint startPoint, BezierControlPoint endPoint, float t) {
            Vector3 a = Tangent(startPoint, endPoint, t);
            Vector3 b = (a + ComputeBezierDoubleDerivative(startPoint, endPoint, t)).normalized;
            return Vector3.Cross(b, a).normalized;
        }
    }
}
