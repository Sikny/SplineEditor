using System;
using System.Diagnostics;
using SplineEditor.Runtime;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityExtendedEditor.Attributes;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Tests {
    public class BenchmarkJobsSystem : MonoBehaviour {
        [SerializeField] private BezierPath bezier;
        [SerializeField] private Vector3[] positionsToCheck = Array.Empty<Vector3>();
        [SerializeField] private int positionsCount;
        [SerializeField] private float generateInRadius = 1;
        
        private void Start() {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            float errorDistance = 0;
            Vector3 pos = transform.position;
            var results1 = new Vector3[positionsCount];
            var results2 = new Vector3[positionsCount];
            for (int i = positionsCount - 1; i >= 0; --i) {
                results1[i] = bezier.bezierSpline.GetClosestBezierPos(pos + positionsToCheck[i]).LocalOrigin;
            }
            stopwatch.Stop();
            Debug.Log("Time to run without jobs: " + stopwatch.ElapsedMilliseconds + "ms");

            var frames = bezier.bezierSpline.RotationMinimisingFrames;
            var framesForJob = new NativeArray<NativeBezierPos>(frames.Count, Allocator.Persistent);
            for (int i = 0; i < frames.Count; ++i) {
                framesForJob[i] = new NativeBezierPos(frames[i]);
            }
            stopwatch.Restart();
            for (int i = positionsCount - 1; i >= 0; --i) {
                var output = new NativeArray<NativeBezierPos>(1, Allocator.Persistent);
                var job = new ComputeClosestBezierPosJob() {
                    frames = framesForJob,
                    bezierMatrix = bezier.bezierSpline.transform.worldToLocalMatrix,
                    inPos = pos + positionsToCheck[i],
                    output = output
                };
                job.Schedule().Complete();
                results2[i] = job.output[0].localOrigin;
                output.Dispose();
            }
            stopwatch.Stop();
            framesForJob.Dispose();
            Debug.Log("Time to run with jobs: " + stopwatch.ElapsedMilliseconds + "ms");


            for (int i = positionsCount - 1; i >= 0; i--) {
                errorDistance += (results1[i] - results2[i]).magnitude;
            }
            Debug.Log("Error distance: " + errorDistance);
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