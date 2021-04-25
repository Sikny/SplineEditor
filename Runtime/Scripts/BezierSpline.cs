using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    public class BezierSpline : MonoBehaviour {
        public BezierSettings settings;

        public bool loop;

        public List<BezierNode> bezierNodes;

        public int divisionsBetweenTwoPoints = 10;

        public void AddCurve()
        {
            BezierNode newPoint = Instantiate(bezierNodes[bezierNodes.Count - 1]);
            newPoint.transform.localPosition += newPoint.GlobalTangentEnd.normalized * 2;
            UpdateNodes();
        }

        public void UpdateNodes() {
            bezierNodes = new List<BezierNode>(GetComponentsInChildren<BezierNode>());
            if(loop) bezierNodes.Add(bezierNodes[0]);
        }

        private void OnDrawGizmos()
        {
            List<BezierUtils.BezierPos> vectorFrames = this.GenerateRotationMinimisingFrames();
            int arrayLen = vectorFrames.Count;
            for (int i = 0; i < arrayLen; ++i)
            {
                var origin = vectorFrames[i].GlobalOrigin;

                if (i < vectorFrames.Count - 1)
                {
                    Gizmos.color = settings.bezierCurveColor;
                    Gizmos.DrawLine(origin, vectorFrames[i + 1].GlobalOrigin);
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