#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class BezierCurve : MonoBehaviour {
        public BezierSettings settings;
        public Vector3 startPoint = new Vector3(0.0f, 0.0f, 0.0f);
        public Vector3 endPoint = new Vector3(-2.0f, 2.0f, 0.0f);
        public Vector3 startTangent = new Vector3(-0.5f, 0.5f, 0.0f);
        public Vector3 endTangent = new Vector3(-1.5f, 1.5f, 0.0f);


        #if UNITY_EDITOR
        private void OnDrawGizmos() {
            Handles.DrawBezier(startPoint, endPoint, startTangent, endTangent, settings.bezierCurveColor, null, 2f);
        }
        #endif
    }
}