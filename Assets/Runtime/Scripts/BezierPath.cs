using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    [ExecuteInEditMode]
    public class BezierPath : MonoBehaviour
    {
        public BezierSpline bezierSpline;
        public BezierMeshExtrusion bezierMeshExtrusion;

        private void OnValidate()
        {
            if (bezierSpline == null || bezierMeshExtrusion == null) return;
            bezierMeshExtrusion.bezierSpline = bezierSpline;
        }

        #if UNITY_EDITOR
        public void Update() {
            if (Application.isPlaying) return;
            bezierSpline.transform.localPosition = Vector3.zero;
            bezierMeshExtrusion.transform.localPosition = -transform.position / 2;
            bezierMeshExtrusion.UpdateMesh();
        }
        #endif
    }
}
