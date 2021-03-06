﻿using System.Collections.Generic;
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
                Tangent = Tangent(start, end, t).normalized;
                Quaternion rotation = Quaternion.Lerp(start.transform.rotation, end.transform.rotation, t);
                Normal = rotation * Vector3.right;
                LocalUp = rotation * Vector3.up;
            }

            public BezierPos(BezierPos source) : this(source.Start, source.End, source.T, source.BezierDistance) {
                
            }
        }

        public static void GenerateRotationMinimisingFrames(this BezierSpline be)
        {
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

            be.RotationMinimisingFrames = frames;
            be.bezierLength = be.bezierNodes[0].bezierDistance;
            be.bezierNodes[0].bezierDistance = 0;
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

            endPoint.bezierDistance = distance;

            return frames;
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
            a = 3 * (b - a);
            b = 3 * (c - b);
            c = 3 * (d - c);
            return a * Mathf.Pow(1 - t, 2) + 2 * b * (1 - t) * t + c * (t * t);
        }

        private static Vector3 Tangent(BezierNode startPoint, BezierNode endPoint, float t)
        {
            Vector3 startPos = startPoint.transform.position, endPos = endPoint.transform.position;
            float x = ComputeBezierDerivative(t, startPos.x, startPoint.GlobalTangentEnd.x,
                endPoint.GlobalTangentStart.x, endPos.x);
            float y = ComputeBezierDerivative(t, startPos.y, startPoint.GlobalTangentEnd.y,
                endPoint.GlobalTangentStart.y, endPos.y);
            float z = ComputeBezierDerivative(t, startPos.z, startPoint.GlobalTangentEnd.z,
                endPoint.GlobalTangentStart.z, endPos.z);
            return new Vector3(x, y, z).normalized;
        }

        public static BezierPos GetBezierPos(this BezierSpline be, float distance)
        {
            var dist = distance < 0 ? 0 : distance;
            var nodesCount = be.bezierNodes.Count;
            BezierNode startNode = null;
            BezierNode endNode = null;
            float t = 0;
            for (int i = 0; i < nodesCount; ++i)
            {
                if (i < nodesCount - 1 && be.bezierNodes[i].bezierDistance > dist 
                    || i == nodesCount - 1 && be.bezierLength > dist)
                {
                    if (i == 0)
                    {
                        startNode = endNode = be.bezierNodes[0];
                        t = 0;
                        break;
                    }
                    endNode = be.bezierNodes[i];
                    startNode = be.bezierNodes[i - 1];
                    if(i == nodesCount - 1)
                        t = (dist - startNode.bezierDistance) / (be.bezierLength - startNode.bezierDistance);
                    else
                        t = (dist - startNode.bezierDistance) / (endNode.bezierDistance - startNode.bezierDistance);
                    break;
                }
            }
            if (startNode == null)  // reached end of bezier spline
            {
                startNode = be.bezierNodes[nodesCount - 1];
                endNode = be.bezierNodes[nodesCount - 1];
                t = 1;
            }

            return new BezierPos(startNode, endNode, t, distance);
        }

        public static BezierPos GetClosestBezierPos(this BezierSpline be, Vector3 pos, float prioritySideFactor = 1)
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
                else newDist = diff.sqrMagnitude * prioritySideFactor;
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