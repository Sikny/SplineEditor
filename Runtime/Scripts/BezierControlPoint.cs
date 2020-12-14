using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [Serializable]
    public class BezierControlPoint {
        public Vector3 position;
        public Vector3 tangent;
    }
}
