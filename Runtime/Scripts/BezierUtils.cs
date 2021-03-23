using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TooWideLocalVariableScope
// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public static class BezierUtils
    {
        public class BezierPos {
            public Vector3 GlobalOrigin { get; set; }

            public Vector3 LocalOrigin => Start.transform.parent.InverseTransformPoint(GlobalOrigin);

            public Vector3 Tangent { get; }

            public Vector3 LocalUp { get; }

            public Vector3 Normal { get; }

            public BezierNode Start { get; }
            public BezierNode End { get; }
            public float T { get; }

            public BezierPos(BezierNode start, BezierNode end, float t) {
                Start = start;
                End = end;
                T = t;
                GlobalOrigin = GetBezierPos(start, end, t);
                Tangent = Tangent(start, end, t).normalized;
                Quaternion rotation = Quaternion.Lerp(start.transform.rotation, end.transform.rotation, t); 
                Normal = rotation * Vector3.right;
                LocalUp = rotation * Vector3.up;
            }
        }

        public static List<BezierPos> GenerateRotationMinimisingFrames(this BezierSpline be) {
            List<BezierPos> frames = new List<BezierPos>();
            for (int i = 0; i < be.bezierNodes.Count - 1; ++i)
            {
                var vFrames = GenerateRotationMinimisingFrames(be.bezierNodes[i],
                    be.bezierNodes[i + 1], be.divisionsBetweenTwoPoints);
                foreach (var vFrame in vFrames)
                {
                    frames.Add(vFrame);
                }
            }
            return frames;
        }

        private static List<BezierPos> GenerateRotationMinimisingFrames(BezierNode startPoint, BezierNode endPoint, int divisions) {
            int steps = divisions;
            var frames = new List<BezierPos>();
            float step = 1.0f / steps;
            float t;
            BezierPos x;

            for (t = 0; t < 1.0f; t += step) {
                x = new BezierPos(startPoint, endPoint, t);
                frames.Add(x);
            }

            return frames;
        }

        private static Vector3 ComputeBezier (float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
            return a * Mathf.Pow(1 - t, 3) + b * (3 * Mathf.Pow(1 - t, 2) * t) + c * (3 * (1 - t) * (t * t)) +
                   d * (t * t * t);
        }

        private static Vector3 GetBezierPos(BezierNode startPoint, BezierNode endPoint, float t) {
            return ComputeBezier(t, startPoint.transform.position, startPoint.GlobalTangentEnd, endPoint.GlobalTangentStart, endPoint.transform.position);
        }
        
        private static float ComputeBezierDerivative (float t, float a, float b, float c, float d) {
            a = 3 * (b - a);
            b = 3 * (c - b);
            c = 3 * (d - c);
            return a * Mathf.Pow(1-t, 2) + 2 * b * (1 - t) * t + c * (t*t);
        }

        private static Vector3 Tangent(BezierNode startPoint, BezierNode endPoint, float t)
        {
            Vector3 startPos = startPoint.transform.position, endPos = endPoint.transform.position;
            float x = ComputeBezierDerivative(t, startPos.x, startPoint.GlobalTangentEnd.x, endPoint.GlobalTangentStart.x, endPos.x);
            float y = ComputeBezierDerivative(t, startPos.y, startPoint.GlobalTangentEnd.y, endPoint.GlobalTangentStart.y, endPos.y);
            float z = ComputeBezierDerivative(t, startPos.z, startPoint.GlobalTangentEnd.z, endPoint.GlobalTangentStart.z, endPos.z);
            return new Vector3(x, y, z).normalized;
        }

        public static BezierPos GetClosestPoint(this BezierSpline be, Vector3 position) {
            float minDistance = float.MaxValue;
            var frames = be.GenerateRotationMinimisingFrames();
            var closestFrame = frames[0];

            for (int i = frames.Count - 1; i >= 0; --i) {
                float distance = Vector3.Distance(frames[i].GlobalOrigin, position);
                if (distance < minDistance) {
                    minDistance = distance;
                    closestFrame = frames[i];
                }
            }
            return closestFrame;
        }
    }
}
