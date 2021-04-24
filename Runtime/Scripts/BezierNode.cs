using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    public enum NodeMode{
        Free,
        Aligned,
        Mirrored
    }
    
    public class BezierNode : MonoBehaviour
    {
        public BezierSettings settings;
        [Space(15f)] [SerializeField] private NodeMode nodeMode;
        [SerializeField] private Transform tangentStart;
        [SerializeField] private Transform tangentEnd;

        [ReadOnly] public float bezierDistance;


        public Vector3 GlobalTangentStart
        {
            get => tangentStart.position;
            set => tangentStart.position = value;
        }

        public Vector3 GlobalTangentEnd
        {
            get => tangentEnd.position;
            set => tangentEnd.position = value;
        }

        public Vector3 LocalTangentStart {
            get => tangentStart.localPosition;
            set => tangentStart.localPosition = value;
        }

        public Vector3 LocalTangentEnd {
            get => tangentEnd.localPosition;
            set => tangentEnd.localPosition = value;
        }

        public void UpdateMirrorPos(Transform controlPoint)
        {
            if(nodeMode == NodeMode.Free) return;
            var target = controlPoint == tangentStart ? tangentEnd : tangentStart;

            Vector3 tPos = transform.position;
            Vector3 direction = controlPoint.position - tPos;

            float distance;
            if (nodeMode == NodeMode.Mirrored) distance = direction.magnitude;
            else distance = Vector3.Distance(target.position, transform.position);  // Aligned
            
            target.position = tPos - distance * direction.normalized;
        }

        private void OnDrawGizmos()
        {
            if (settings == null) return;

            Transform t = transform;
            Vector3 pos = t.position;
            Quaternion rot = t.rotation;

            Gizmos.color = settings.tangentLinesColor;
            Gizmos.DrawLine(tangentStart.position, pos);
            Gizmos.DrawLine(pos, tangentEnd.position);

            Gizmos.color = settings.bezierPointColor;
            var gizmoSize = settings.controlsHandleSize;
            Gizmos.DrawCube(pos, gizmoSize * Vector3.one);
            
            if (!(rot.eulerAngles == Vector3.zero && rot.w == 0)) 
                Gizmos.matrix = Matrix4x4.TRS(pos, Quaternion.Normalize(rot), t.lossyScale);

            Gizmos.color = Color.green;
            Gizmos.DrawCube( new Vector3(0, 2.5f, 0) * gizmoSize, new Vector3(0.1f, 5, 0.1f) * gizmoSize);
            #if UNITY_EDITOR
            Handles.matrix = Gizmos.matrix;
            Handles.color = Color.white;
            Handles.DrawWireDisc(Vector3.zero, Vector3.forward, 5 * gizmoSize);
            #endif
        }
    }
}