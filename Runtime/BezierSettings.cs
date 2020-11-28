using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [CreateAssetMenu(fileName = "BezierSettings", menuName = "ScriptableObjects/BezierSettings")]
    public class BezierSettings : ScriptableObject {
        public Color bezierCurveColor;
        public Color bezierPointColor;
        public Color bezierControlPointColor;
        public Color tangentLinesColor;
    }
}
