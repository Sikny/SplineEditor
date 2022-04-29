using System;
using System.Collections;
using SplineEditor.Runtime;
using UnityEngine;
using UnityExtendedEditor.Attributes;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Tests {
    public class BenchmarkJobsSystem : MonoBehaviour {
        [SerializeField] private BezierPath bezier;
        [SerializeField] private Vector3[] positionsToCheck = Array.Empty<Vector3>();
        [SerializeField] private int positionsCount;
        [SerializeField] private float generateInRadius = 1;
        
        private IEnumerator Start() {
            yield return null;
            Vector3 pos = transform.position;
            float time = Time.realtimeSinceStartup;
            for (int i = positionsCount - 1; i >= 0; i--) {
                bezier.bezierSpline.GetClosestBezierPos(pos + positionsToCheck[i]);
            }
            Debug.Log("Time to run without jobs : " + (Time.realtimeSinceStartup - time) + "s");
            yield return null;
        }

        [Button]
        private void GenerateRandomPositions() {
            positionsToCheck = new Vector3[positionsCount];
            for (int i = positionsCount - 1; i >= 0; i--) {
                positionsToCheck[i] = Random.insideUnitSphere * generateInRadius;
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            var pos = transform.position;
            Gizmos.DrawWireSphere(pos, generateInRadius);
            for (var index = 0; index < positionsToCheck.Length; index++) {
                var vec = pos + positionsToCheck[index];
                Gizmos.DrawSphere(vec, 0.5f);
            }
        }
    }
}