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

        public static List<VectorFrame> GenerateRotationMinimisingFrames(this BezierSpline be) {
            List<VectorFrame> frames = new List<VectorFrame>();
            for (int i = 0; i < be.bezierNodes.Count - 1; ++i)
            {
                VectorFrame firstFrame = null;
                if (i > 0)
                    firstFrame = GetFrenetFrame(be.bezierNodes[i - 1], be.bezierNodes[i], 1);
                frames.AddRange(GenerateRotationMinimisingFrames(be.bezierNodes[i],
                    be.bezierNodes[i+1], be.divisionsBetweenTwoPoints, firstFrame));
            }
            return frames;
        }

        private static List<VectorFrame> GenerateRotationMinimisingFrames(BezierNode startPoint, BezierNode endPoint, int divisions, VectorFrame firstFrame = null) {
            int steps = divisions;
            var frames = new List<VectorFrame>();
            float step = 1.0f / steps;
            float t0, t1, c1, c2;
            Vector3 v1, v2, riL, tiL;
            VectorFrame x0, x1;

            if(firstFrame != null)
                frames.Add(firstFrame);
            else
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
        
        private static VectorFrame GetFrenetFrame(BezierNode startPoint, BezierNode endPoint, float t) {
            return new VectorFrame(GetBezierPos(startPoint, endPoint, t), Tangent(startPoint, endPoint, t).normalized, 
                RotationAxis(startPoint, endPoint, t).normalized, Normal(startPoint, endPoint, t).normalized);
        }

        private static Vector3 ComputeBezier (float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d) {
            return a * Mathf.Pow(1 - t, 3) + b * (3 * Mathf.Pow(1 - t, 2) * t) + c * (3 * (1 - t) * (t * t)) +
                   d * (t * t * t);
        }

        private static Vector3 GetBezierPos(BezierNode startPoint, BezierNode endPoint, float t) {
            return ComputeBezier(t, startPoint.transform.position, startPoint.GlobalTangent2, endPoint.GlobalTangent1, endPoint.transform.position);
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
            float x = ComputeBezierDerivative(t, startPos.x, startPoint.GlobalTangent2.x, endPoint.GlobalTangent1.x, endPos.x);
            float y = ComputeBezierDerivative(t, startPos.y, startPoint.GlobalTangent2.y, endPoint.GlobalTangent1.y, endPos.y);
            float z = ComputeBezierDerivative(t, startPos.z, startPoint.GlobalTangent2.z, endPoint.GlobalTangent1.z, endPos.z);
            return new Vector3(x, y, z).normalized;
        }

        private static Vector3 ComputeBezierDoubleDerivative(BezierNode startPoint, BezierNode endPoint, float t) {
            return 6 * (1 - t) * (endPoint.GlobalTangent1 - 2 * startPoint.GlobalTangent2 + startPoint.transform.position) +
                   6 * t * (endPoint.transform.position - 2 * endPoint.GlobalTangent1 + startPoint.GlobalTangent2);
        }

        private static Vector3 Normal(BezierNode startPoint, BezierNode endPoint, float t) {
            Vector3 a = Tangent(startPoint, endPoint, t);
            Vector3 r = RotationAxis(startPoint, endPoint, t);
            return Vector3.Cross(r, a).normalized;
        }

        private static Vector3 RotationAxis(BezierNode startPoint, BezierNode endPoint, float t) {
            Vector3 a = Tangent(startPoint, endPoint, t);
            Vector3 b = (a + ComputeBezierDoubleDerivative(startPoint, endPoint, t)).normalized;
            var result =  Vector3.Cross(b, a).normalized;
            if (result == Vector3.zero) result = Vector3.up;
            return result;
        }

        private static Vector3 GetClosestPointRecursive(Vector3 position, BezierNode nodeBegin, 
            BezierNode nodeEnd, float t0, float t1, float step, float precision) {
            float newStep = step / 2;
            float minDistance = float.MaxValue;
            float newT0 = t0, newT1 = t1;

            for (float t = t0; t <= t1; t += newStep) {
                float distance = Vector3.Distance(position, GetBezierPos(nodeBegin, nodeEnd, t));
                if (distance < minDistance) {
                    minDistance = distance;
                    newT0 = t;
                    newT1 = t + newStep;
                }
            }
            if(newStep <= precision)
                return GetBezierPos(nodeBegin, nodeEnd, newT0);
            return GetClosestPointRecursive(position, nodeBegin, nodeEnd, newT0, newT1, newStep,
                precision);
        }

        public static Vector3 GetClosestPoint(this BezierSpline be, Vector3 position) {
            // f(t) = 0.5 * Vector3.Dot(p(t)-X,p(t)-X)
            // Minimization can be achieved by finding all of the roots of the derivative
            // f'(t)=dot(p'(t), p(t)-X)
            // inside the interval and comparing the function values of the roots and at
            // the end points of the interval

            // 1. find extremities control points of position
            // 2. search closest point between interval and recursively reduce interval and step
            // return result when step <= wanted precision (dichotomy like research)

            float minDistance = float.MaxValue;
            int indexControlPointEnd = be.bezierNodes.Count;
            for (int i = be.bezierNodes.Count - 1; i > 0; --i) {
                float distance = Vector3.Distance(be.transform.TransformPoint(be.bezierNodes[i].transform.position), position);
                if (distance < minDistance) {
                    minDistance = distance;
                    indexControlPointEnd = i;
                }
            }

            return GetClosestPointRecursive(position, be.bezierNodes[indexControlPointEnd-1], 
                be.bezierNodes[indexControlPointEnd], 0, 1, 0.1f, 0.01f);
        }
    }
}
