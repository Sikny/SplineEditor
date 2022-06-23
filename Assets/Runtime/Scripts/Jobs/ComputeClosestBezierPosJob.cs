using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [BurstCompile(CompileSynchronously = true)]
    public struct ComputeClosestBezierPosJob : IJob {
        [Unity.Collections.ReadOnly] public NativeArray<NativeBezierPos> frames;
        [Unity.Collections.ReadOnly] public Matrix4x4 bezierMatrix;
        [Unity.Collections.ReadOnly] public Vector3 inPos;
        
        public NativeArray<NativeBezierPos> output;
        
        public void Execute() {
            Vector3 localPos = bezierMatrix.MultiplyPoint3x4(inPos);
            float dist = float.PositiveInfinity;
            float newDist;
            NativeBezierPos bP = default;
            var frameCount = frames.Length;
            Vector3 diff;
            for (int i = 0; i < frameCount; ++i) {
                diff = localPos - frames[i].localOrigin;
                newDist = diff.sqrMagnitude;
                if (newDist < dist) {
                    dist = newDist;
                    bP = frames[i];
                }
            }
            output[0] = bP;
        }
    }
}