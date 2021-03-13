using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [CreateAssetMenu(fileName = "BezierSettings", menuName = "ScriptableObjects/BezierSettings")]
    public class BezierSettings : ScriptableObject {
        public Color bezierCurveColor = Color.red;
        public Color bezierPointColor = Color.gray;
        public Color bezierControlPointColor = Color.gray;
        public float controlsHandleSize = 0.1f;
        public Color tangentLinesColor = Color.white;
        public bool showNormals;
        public Color normalsColor = Color.yellow;
        public bool showVerticalNormals;
        public Color verticalNormalsColor = Color.blue;
    }
}
