using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [ExecuteInEditMode]
    public class SplinePoint : MonoBehaviour {
        [HideInInspector] public Spline spline;
        public Transform controlPoint1;
        public Transform controlPoint2;

        public bool isLast;

        public void Init() {
            // Adding control points
            Transform selfTransform = transform;
            GameObject cPObj1 = new GameObject("Control Point 1");
            cPObj1.AddComponent<ControlPoint>().splinePoint = this;
            controlPoint1 = cPObj1.transform;
            controlPoint1.parent = selfTransform;
            GameObject cPObj2 = new GameObject("Control Point 2");
            cPObj2.AddComponent<ControlPoint>().splinePoint = this;
            controlPoint2 = cPObj2.transform;
            controlPoint2.parent = selfTransform;
        }

        private void OnDestroy() {
            spline.points.Remove(this);
        }

        private readonly Color _gizmoColor = Color.grey;
        private readonly Color _activeGizmoColor = Color.red;
        private void OnDrawGizmos() {
            if (Selection.activeTransform is null || !Selection.activeTransform.IsChildOf(spline.transform))
                return;
            
            Gizmos.color = Selection.activeGameObject == gameObject ? _activeGizmoColor : _gizmoColor;
            float pointSize = SplineUtils.SplinePointSize;
            var position = transform.position;
            Gizmos.DrawCube(position, Vector3.one * pointSize);
            
            if (!isLast && Selection.activeTransform.IsChildOf(transform)) {
                var ctrlPos1 = controlPoint1.position;
                var ctrlPos2 = controlPoint2.position;
                Handles.DrawDottedLine(position, ctrlPos1, 4.0f);
                Handles.DrawDottedLine(ctrlPos1, ctrlPos2, 4.0f);
                SplinePoint nextPoint = spline.points[spline.points.IndexOf(this) + 1];
                Handles.DrawDottedLine(ctrlPos2, nextPoint.transform.position, 4.0f);
            }
        }
    }
}