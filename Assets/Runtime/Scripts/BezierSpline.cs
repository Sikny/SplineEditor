using System.Collections.Generic;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class BezierSpline : MonoBehaviour {
        public BezierSettings settings;

        public List<BezierControlPoint> controlPoints;

        public int divisionsBetweenTwoPoints = 10;
    }
}