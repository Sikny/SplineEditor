﻿using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    public class BezierSpline : MonoBehaviour
    {
        public BezierSettings settings;

        public List<BezierNode> bezierNodes;

        public int divisionsBetweenTwoPoints = 10;

        public void AddCurve()
        {
            BezierNode newPoint = Instantiate(bezierNodes[bezierNodes.Count - 1]);
            newPoint.transform.localPosition += newPoint.GlobalTangent2.normalized * 2;
            bezierNodes.Add(newPoint);
        }

        private void OnDrawGizmos()
        {
            List<BezierUtils.BezierPos> vectorFrames = this.GenerateRotationMinimisingFrames();
            int arrayLen = vectorFrames.Count;
            for (int i = 0; i < arrayLen; ++i)
            {
                var origin = transform.TransformPoint(vectorFrames[i].Origin);

                if (i < vectorFrames.Count - 1)
                {
                    Gizmos.color = settings.bezierCurveColor;
                    Gizmos.DrawLine(origin, transform.TransformPoint(vectorFrames[i + 1].Origin));
                }

                if (settings.showNormals)
                {
                    Gizmos.color = settings.normalsColor;
                    Gizmos.DrawLine(origin, origin + vectorFrames[i].Normal);
                }

                if (settings.showVerticalNormals)
                {
                    Gizmos.color = settings.verticalNormalsColor;
                    Gizmos.DrawLine(origin, origin + vectorFrames[i].LocalUp);
                }
            }
        }
    }
}