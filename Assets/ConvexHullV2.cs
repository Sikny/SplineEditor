﻿using System;
using System.Collections.Generic;
using System.Linq;
using SplineEditor.Runtime;
using UnityEngine;

public class ConvexHullV2 : MonoBehaviour {
    public BezierSpline spline;

    public bool useRealBezier;
    
    public List<Vector3> hullPoints;
    public List<Vector3> sourcePositions;

    public MeshFilter meshFilter;

    private void OnValidate() {
        BuildConvexHull();
    }

    [ContextMenu("Build convex hull")]
    public void BuildConvexHull() {
        // building array
        List<Vector3> positions = new List<Vector3>();
        if (!useRealBezier) {
            foreach (var node in spline.bezierNodes) {
                positions.AddPos(node.GlobalTangentStart);
                positions.AddPos(node.transform.position);
                positions.AddPos(node.GlobalTangentEnd);
            }
        }
        else {
            List<BezierUtils.BezierPos> frames = spline.RotationMinimisingFrames;
            foreach (var frame in frames) {
                positions.AddPos(frame.GlobalOrigin);
            }
            positions.AddPos(frames[0].GlobalOrigin);
        }

        sourcePositions = positions;
        hullPoints = ComputeConvexHull();

        BuildMesh();
    }

    // https://gist.github.com/dLopreiato/7fd142d0b9728518552188794b8a750c
    private List<Vector3> ComputeConvexHull() {
        List<Vector3> result = new List<Vector3>();

        sourcePositions.Sort((a, b) => 
            Math.Abs(a.z - b.z) < 0.01f ? a.y.CompareTo(b.y) : (a.z > b.z ? 1 : -1));
        int L = 0, U = 0;   // lower and upper size

        for (int i = sourcePositions.Count - 1; i >= 0; --i) {
            Vector3 p = sourcePositions[i], p1;

            while (L >= 2 && ((p1 = result[result.Count - 1]) - result[result.Count - 2]).Cross(p - p1) >= 0) {
                result.RemoveAt(result.Count - 1);
                --L;
            }

            result.Add(p);
            ++L;

            while (U >= 2 && ((p1 = result[0]) - result[1]).Cross(p - p1) <= 0) {
                result.RemoveAt(0);
                --U;
            }
            if(U != 0)
               result.Insert(0, p);
            ++U;
        }


        return result;
    }

    private void BuildMesh() {
        Mesh mesh = new Mesh();
        Vector2[] positions = new Vector2[hullPoints.Count];
        for (var index = 0; index < hullPoints.Count; ++index) {
            var hullPoint = hullPoints[index];
            hullPoint.x = hullPoint.z;
            positions[index] = hullPoint;
        }

        Triangulator tr = new Triangulator(positions);
        int[] indices = tr.Triangulate();
        Vector3[] vertices = new Vector3[positions.Length];
        for (int i = 0; i < vertices.Length; ++i) {
            vertices[i] = new Vector3(0, positions[i].y, positions[i].x);
        }

        mesh.vertices = vertices;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        for (var index = 0; index < hullPoints.Count; index++) {
            if(index > 0)
                Gizmos.DrawLine(hullPoints[index - 1], hullPoints[index]);
        }
    }
}

public static class PosUtils {
    public static void AddPos(this List<Vector3> array, Vector3 position) {
        Vector3 pos = position;
        pos.x = 0;
        array.Add(pos);
    }

    public static float Cross(this Vector3 a, Vector3 b)
    {
        return a.z * b.y - a.y * b.z;
    }
}