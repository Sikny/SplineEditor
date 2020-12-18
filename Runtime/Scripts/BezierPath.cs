using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    [ExecuteInEditMode]
    public class BezierPath : MonoBehaviour
    {
        public BezierCurve bezierCurve;
        public BezierMeshExtrusion bezierMeshExtrusion;

        private void OnValidate()
        {
            if (bezierCurve == null || bezierMeshExtrusion == null) return;
            bezierMeshExtrusion.bezierCurve = bezierCurve;
        }

        public void Update()
        {
            bezierCurve.RecalculatePositions();
            bezierCurve.transform.localPosition = Vector3.zero;
            bezierMeshExtrusion.transform.localPosition = -transform.position / 2;
            bezierMeshExtrusion.UpdateMesh();
        }
    }
}
