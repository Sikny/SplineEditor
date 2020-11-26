using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [ExecuteInEditMode]
    public class BezierPoint : MonoBehaviour {
        [HideInInspector] public BezierCurve bezierCurve;
        public Transform controlPoint1;
        public Transform controlPoint2;

        public bool isLast;

        public void Init() {
            // Adding control points
            Transform selfTransform = transform;
            GameObject cPObj1 = new GameObject("Control Point 1");
            cPObj1.AddComponent<ControlPoint>().bezierPoint = this;
            controlPoint1 = cPObj1.transform;
            controlPoint1.parent = selfTransform;
            GameObject cPObj2 = new GameObject("Control Point 2");
            cPObj2.AddComponent<ControlPoint>().bezierPoint = this;
            controlPoint2 = cPObj2.transform;
            controlPoint2.parent = selfTransform;
        }

        private void OnDestroy() {
            bezierCurve.points.Remove(this);
        }

        private readonly Color _gizmoColor = Color.grey;
        private readonly Color _activeGizmoColor = Color.red;
        private void OnDrawGizmos() {
            Gizmos.color = Selection.activeGameObject == gameObject ? _activeGizmoColor : _gizmoColor;
            float pointSize = BezierUtils.SplinePointSize;
            var position = transform.position;
            Gizmos.DrawCube(position, Vector3.one * pointSize);

            if (Selection.activeTransform is null) return;
            
            if (!isLast && Selection.activeTransform.IsChildOf(transform)) {
                var ctrlPos1 = controlPoint1.position;
                var ctrlPos2 = controlPoint2.position;
                Handles.DrawDottedLine(position, ctrlPos1, 4.0f);
                //Handles.DrawDottedLine(ctrlPos1, ctrlPos2, 4.0f);
                BezierPoint nextPoint = bezierCurve.points[bezierCurve.points.IndexOf(this) + 1];
                Handles.DrawDottedLine(ctrlPos2, nextPoint.transform.position, 4.0f);
            }
        }
    }
}