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

            Transform t = transform;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            Transform splineT = bezierSpline.transform; 
            splineT.localPosition = Vector3.zero;

            bezierMeshExtrusion.transform.rotation = Quaternion.identity;

            bezierMeshExtrusion.bezierSpline.UpdateNodes();
            bezierMeshExtrusion.transform.localPosition = Vector3.zero;
            bezierMeshExtrusion.UpdateMesh();
        }
        #endif
    }
}
