using System.Collections.Generic;
using UnityEngine;

// ReSharper disable TooWideLocalVariableScope
// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    public static class BezierUtils
    {
        public class BezierPos
        {
            public Vector3 GlobalOrigin { get; set; }

            public Vector3 LocalOrigin => Start.transform.parent.InverseTransformPoint(GlobalOrigin);

            public Vector3 Tangent { get; }

            public Vector3 LocalUp { get; }

            public Vector3 Normal { get; }

            public Quaternion Rotation => Quaternion.LookRotation(Tangent, LocalUp);

            public BezierNode Start { get; }
            public BezierNode End { get; }
            public float T { get; }
            public float BezierDistance { get; set; }

            public BezierPos(BezierNode start, BezierNode end, float t, float bezierDistance)
            {
                Start = start;
                End = end;
                T = t;
                BezierDistance = bezierDistance;
                GlobalOrigin = GetBezierPos(start, end, t);
                Tangent = ComputeTangent(start, end, t).normalized;
                if (Tangent == Vector3.zero) {
                    if (Vector3.Distance(GlobalOrigin, start.transform.position) <
                        Vector3.Distance(GlobalOrigin, end.transform.position)) {
                        Tangent = (start.GlobalTangentEnd - GlobalOrigin).normalized;
                    }
                    else {
                        Tangent = (end.GlobalTangentStart - GlobalOrigin).normalized;
                    }
                }
                Quaternion rotation = Quaternion.Lerp(start.transform.rotation, end.transform.rotation, t);
                Normal = rotation * Vector3.right;
                LocalUp = -Vector3.Cross(Normal, Tangent).normalized;
                Normal = Vector3.Cross(LocalUp, Tangent).normalized;
            }

            public BezierPos(BezierPos source) : this(source.Start, source.End, source.T, source.BezierDistance) {
                
            }
        }

        public static void GenerateRotationMinimisingFrames(this BezierSpline be)
        {
            List<BezierPos> frames = new List<BezierPos>();
            be.bezierNodes[0].bezierDistance = 0;
            for (int i = 0; i < be.bezierNodes.Count - 1; ++i) {
                List<BezierPos> vFrames;
                if (be.constantSizeForDivisions) {
                    vFrames = GenerateFixedDistanceFrames(be.bezierNodes[i], be.bezierNodes[i + 1],
                        be.divisionsBetweenTwoPoints);
                }
                else {
                    vFrames = GenerateRotationMinimisingFrames(be.bezierNodes[i],
                        be.bezierNodes[i + 1], be.divisionsBetweenTwoPoints);
                }

                frames.AddRange(vFrames);
            }

            be.RotationMinimisingFrames = frames;
            be.bezierLength = be.bezierNodes[be.bezierNodes.Count - 1].bezierDistance - be.bezierNodes[0].bezierDistance;
        }

        private static List<BezierPos> GenerateRotationMinimisingFrames(BezierNode startPoint, BezierNode endPoint,
            int divisions)
        {
            int steps = divisions;
            var frames = new List<BezierPos>();
            float step = 1.0f / steps;
            float t;
            BezierPos x;

            float distance = startPoint.bezierDistance;

            for (t = 0; t < 1.0f; t += step)
            {
                x = new BezierPos(startPoint, endPoint, t, distance);
                if (t > 0) {
                    distance += Vector3.Distance(x.GlobalOrigin, frames[frames.Count - 1].GlobalOrigin);
                    x.BezierDistance = distance;
                }

                frames.Add(x);
            }

            x = new BezierPos(startPoint, endPoint, 1, endPoint.bezierDistance);
            frames.Add(x);

            endPoint.bezierDistance = distance;

            return frames;
        }

        private static List<BezierPos> GenerateFixedDistanceFrames(BezierNode startPoint, BezierNode endPoint, int divisions) {
            var frames = GenerateRotationMinimisingFrames(startPoint, endPoint, Mathf.Max(divisions * 5, 250));
            int framesCount = frames.Count;
            var result = new List<BezierPos>();
            float step = (endPoint.bezierDistance - startPoint.bezierDistance) / divisions;
            for (float d = startPoint.bezierDistance; d < endPoint.bezierDistance; d += step) {
                // find closest
                float distToFrame = float.MaxValue;
                int index = 0;
                for (int i = 0; i < framesCount; ++i) {
                    float curDist = Mathf.Abs(d - frames[i].BezierDistance);
                    if (curDist < distToFrame) {
                        index = i;
                        distToFrame = curDist;
                    }
                    else {
                        break;
                    }
                }
                result.Add(frames[index]);
            }

            if (Mathf.Abs(result[result.Count - 1].BezierDistance - frames[framesCount - 1].BezierDistance) < 3 * step / 4) {
                result.RemoveAt(result.Count - 1);
            }
            result.Add(frames[framesCount-1]);
            return result;
        }

        private static Vector3 ComputeBezier(float t, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            return a * Mathf.Pow(1 - t, 3) + b * (3 * Mathf.Pow(1 - t, 2) * t) + c * (3 * (1 - t) * (t * t)) +
                   d * (t * t * t);
        }

        private static Vector3 GetBezierPos(BezierNode startPoint, BezierNode endPoint, float t)
        {
            return ComputeBezier(t, startPoint.transform.position, startPoint.GlobalTangentEnd,
                endPoint.GlobalTangentStart, endPoint.transform.position);
        }

        private static float ComputeBezierDerivative(float t, float a, float b, float c, float d)
        {
            return 3 * Mathf.Pow(1 - t, 2) * (b - a) + 6 * (1 - t) * t * (c - b) + 3 * (t * t) * (d - c);
        }

        private static Vector3 ComputeTangent(BezierNode startPoint, BezierNode endPoint, float t)
        {
            Vector3 startPos = startPoint.transform.position, endPos = endPoint.transform.position,
                startEndTan = startPoint.GlobalTangentEnd, endStartTan = endPoint.GlobalTangentStart;
            float x = ComputeBezierDerivative(t, startPos.x, startEndTan.x, endStartTan.x, endPos.x);
            float y = ComputeBezierDerivative(t, startPos.y, startEndTan.y, endStartTan.y, endPos.y);
            float z = ComputeBezierDerivative(t, startPos.z, startEndTan.z, endStartTan.z, endPos.z);
            return new Vector3(x, y, z).normalized;
        }

        public static BezierPos GetBezierPos(this BezierSpline be, float distance)
        {
            var dist = distance < 0 ? 0 : distance;
            var nodesCount = be.bezierNodes.Count;
            BezierNode startNode = null;
            BezierNode endNode = null;
            float t;
            for (int i = 0; i < nodesCount; ++i)
            {
                if (i < nodesCount - 1 && be.bezierNodes[i].bezierDistance > dist 
                    || i == nodesCount - 1 && be.bezierLength > dist)
                {
                    if (i == 0)
                    {
                        startNode = be.bezierNodes[0];
                        endNode = be.bezierNodes[1];
                        t = 0;
                        return new BezierPos(startNode, endNode, t, dist);
                    }
                    endNode = be.bezierNodes[i];
                    startNode = be.bezierNodes[i - 1];
                    break;
                }
            }
            if (startNode == null)  // reached end of bezier spline
            {
                startNode = be.bezierNodes[nodesCount - 2];
                endNode = be.bezierNodes[nodesCount - 1];
                t = 1;
                return new BezierPos(startNode, endNode, t, dist);
            }

            var frames = GenerateRotationMinimisingFrames(startNode, endNode, 50);
            for (int i = frames.Count - 1; i >= 0; --i) {
                if (frames[i].BezierDistance < dist) {
                    return frames[i];
                }
            }
            if (dist <= frames[0].BezierDistance) return frames[0];
            return frames[frames.Count - 1];
        }

        public static BezierPos GetClosestBezierPos(this BezierSpline be, Vector3 pos)
        {
            Vector3 localPos = be.transform.worldToLocalMatrix.MultiplyPoint3x4(pos);
            float dist = float.PositiveInfinity;
            float newDist;
            BezierPos bP = null;
            Vector3 diff;

            BezierPos lineStart = null;
            BezierPos lineEnd = null;

            var frames = be.RotationMinimisingFrames;
            var framesCount = frames.Count;

            for (int j = 0; j < framesCount; ++j)
            {
                diff = localPos - frames[j].LocalOrigin;
                if (Vector3.Angle(diff, frames[j].Normal) < 90) newDist = diff.sqrMagnitude;
                else newDist = diff.sqrMagnitude;
                if (newDist < dist)
                {
                    dist = newDist;
                    bP = frames[j];
                    lineStart = frames[j];
                    lineEnd = j < frames.Count - 1 ? frames[j + 1] : lineStart;
                }
            }

            Vector3 a = lineStart.GlobalOrigin;
            Vector3 b = lineEnd.GlobalOrigin;
            BezierPos result = new BezierPos(bP);
            /*Vector3 projected = a + Vector3.Project(pos - a, b - a);
            
            // check on segment
            float toStart = (projected - a).sqrMagnitude;
            float toEnd = (projected - b).sqrMagnitude;
            float ab = (a - b).sqrMagnitude;
            
            // TODO TOFIX
            if (toStart + toEnd > ab)
                projected = toStart > toEnd ? b : a;

            result.GlobalOrigin = projected;*/
            return result;
        }
    }
}