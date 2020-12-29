using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class BezierSpline : MonoBehaviour {
        public BezierSettings settings;

        public List<BezierControlPoint> controlPoints;

        public int divisionsBetweenTwoPoints = 10;

        public void AddCurve() {
            BezierControlPoint newPoint = controlPoints[controlPoints.Count - 1].Copy();
            newPoint.position += newPoint.Tangent2.normalized * 2;
            controlPoints.Add(newPoint);
        }
    }
}