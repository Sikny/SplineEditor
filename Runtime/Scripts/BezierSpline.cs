using System.Collections.Generic;
using UnityEditor;
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
#if UNITY_EDITOR
            Handles.color = settings.bezierCurveColor;
            for (int i = 0; i < bezierNodes.Count; i++)
            {
                Vector3 currentPos = bezierNodes[i].transform.position;
                Vector3 currentTan2 = bezierNodes[i].GlobalTangent2;
                if (i < bezierNodes.Count - 1)
                    Handles.DrawBezier(currentPos,
                        transform.TransformPoint(bezierNodes[i + 1].transform.position), currentTan2,
                        transform.TransformPoint(bezierNodes[i + 1].GlobalTangent1), settings.bezierCurveColor, 
                        null, settings.bezierCurveWidth);
            }
            
            List<BezierUtils.VectorFrame> vectorFrames = this.GenerateRotationMinimisingFrames();
            int arrayLen = vectorFrames.Count;
            for (int i = 0; i < arrayLen; ++i) {
                var origin = transform.TransformPoint(vectorFrames[i].Origin);
                if (settings.showNormals) {
                    Handles.color = settings.normalsColor;
                    Handles.DrawLine(origin, origin + vectorFrames[i].Normal);
                }

                if (settings.showVerticalNormals) {
                    Handles.color = settings.verticalNormalsColor;
                    Handles.DrawLine(origin, origin + vectorFrames[i].RotationAxis);
                }
            }
#endif
        }
    }
}