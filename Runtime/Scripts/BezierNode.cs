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

        private void OnDrawGizmos()
        {
            if (settings == null) return;
            
            Vector3 pos = transform.position;

            Gizmos.color = settings.tangentLinesColor;
            Gizmos.DrawLine(tangentStart.position, pos);
            Gizmos.DrawLine(pos, tangentEnd.position);

            Gizmos.color = settings.bezierPointColor;
            var gizmoSize = settings.controlsHandleSize;
            Gizmos.DrawCube(pos, gizmoSize * Vector3.one);
        }

        public void UpdateMirrorPos(Transform controlPoint)
        {
            Transform target;
            if (controlPoint == tangentStart)
            {
                target = tangentEnd;
            }
            else target = tangentStart;

            target.position = transform.position - controlPoint.localPosition;
        }
    }
}