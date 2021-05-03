using System;
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

        private List<BezierUtils.BezierPos> _rotationMinimisingFrames;

        public List<BezierUtils.BezierPos> RotationMinimisingFrames
        {
            get
            {
                if (_rotationMinimisingFrames == null)
                {
                    this.GenerateRotationMinimisingFrames();
                }
                return _rotationMinimisingFrames;
            }
            set => _rotationMinimisingFrames = value;
        }

        public void UpdateNodes() {
            bezierNodes = new List<BezierNode>(GetComponentsInChildren<BezierNode>());
            if(loop) bezierNodes.Add(bezierNodes[0]);
        }

        private void OnValidate()
        {
            this.GenerateRotationMinimisingFrames();
        }

        private void OnDrawGizmos()
        {
            List<BezierUtils.BezierPos> vectorFrames = RotationMinimisingFrames;
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