using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [Serializable]
    public class BezierControlPoint
    {
        public Vector3 position;
        [SerializeField] private Vector3 tangent1;
        [SerializeField] private Vector3 tangent2;

        public Vector3 Tangent1
        {
            get => tangent1 + position;
            set
            {
                var val = value - position;
                tangent1 = val;
                tangent2 = -val;
            }
        }

        public Vector3 Tangent2
        {
            get => tangent2 + position;
            set {
                var val = value - position;
                tangent2 = val;
                tangent1 = -val;
            }
        }

        public BezierControlPoint Copy() {
            BezierControlPoint result = new BezierControlPoint();
            result.position = position;
            result.tangent1 = tangent1;
            result.tangent2 = tangent2;
            return result;
        }
    }
}
