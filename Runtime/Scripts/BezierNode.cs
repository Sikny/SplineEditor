using System;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    [Serializable]
    public class BezierNode : MonoBehaviour
    {
        public BezierSettings settings;
        [Space(15f)]
        [SerializeField] private Transform tangentStart;
        [SerializeField] private Transform tangentEnd;
        public float roll;


        public Vector3 GlobalTangent1
        {
            get => tangentStart.position;
            set => tangentStart.position = value;
        }

        public Vector3 GlobalTangent2
        {
            get => tangentEnd.position;
            set => tangentEnd.position = value;
        }

        public Vector3 LocalTangent1 {
            get => tangentStart.localPosition;
            set => tangentStart.localPosition = value;
        }

        public Vector3 LocalTangent2 {
            get => tangentEnd.localPosition;
            set => tangentEnd.localPosition = value;
        }

        public void UpdateMirrorPos(Transform controlPoint)
        {
            var target = controlPoint == tangentStart ? tangentEnd : tangentStart;

            Vector3 tPos = transform.position;
            Vector3 direction = controlPoint.position - tPos;
            target.position = tPos - direction;
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