using System;
using System.Collections;
using System.Linq;
using SplineEditor.Runtime;
using Unity.Collections;
using Unity.Jobs;
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
            float errorDistance = 0;
            yield return null;
            Vector3 pos = transform.position;
            float time = Time.realtimeSinceStartup;
            var results1 = new Vector3[positionsCount];
            var results2 = new Vector3[positionsCount];
            for (int i = positionsCount - 1; i >= 0; i--) {
                results1[i] = bezier.bezierSpline.GetClosestBezierPos(pos + positionsToCheck[i]).LocalOrigin;
            }
            Debug.Log("Time to run without jobs : " + (Time.realtimeSinceStartup - time) + "s");
            yield return null;

            var frames = bezier.bezierSpline.RotationMinimisingFrames;
            var jobClosestBezierPos = new ComputeClosestBezierPosJob {
                frames = new NativeArray<NativeBezierPos>(frames.Count, Allocator.TempJob),
                bezierMatrix = bezier.bezierSpline.transform.worldToLocalMatrix
            };
            for (int i = frames.Count - 1; i >= 0; i--) {
                jobClosestBezierPos.frames[i] = new NativeBezierPos(frames[i]);
            }
            time = Time.realtimeSinceStartup;
            for (int i = positionsCount - 1; i >= 0; i--) {
                jobClosestBezierPos.inPos = pos + positionsToCheck[i];
                jobClosestBezierPos.Schedule().Complete();
                results2[i] = jobClosestBezierPos.output.localOrigin;
            }
            Debug.Log("Time to run with jobs : " + (Time.realtimeSinceStartup - time) + "s");
            jobClosestBezierPos.frames.Dispose();
            yield return null;

            for (int i = positionsCount - 1; i >= 0; i--) {
                errorDistance += (results1[i] - results2[i]).magnitude;
            }
            Debug.Log("Error distance : " + errorDistance);
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