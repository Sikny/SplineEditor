using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [Serializable]
    public class BezierControlPoint {
        public Vector3 position;
        private Vector3 _tangent1;
        private Vector3 _tangent2;

        public Vector3 Tangent1
        {
            get => _tangent1 + position;
            set
            {
                var val = value - position;
                _tangent1 = val;
                _tangent2 = -val;
            }
        }

        public Vector3 Tangent2
        {
            get => _tangent2 + position;
            set {
                var val = value - position;
                _tangent2 = val;
                _tangent1 = -val;
            }
        }
    }
}
