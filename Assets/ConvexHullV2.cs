using System;
using System.Collections.Generic;
using System.Linq;
using SplineEditor.Runtime;
using UnityEngine;

public class ConvexHullV2 : MonoBehaviour {
    public BezierSpline spline;


    public List<Vector3> hullPoints;
    public List<Vector3> sourcePositions;

    private void OnValidate() {
        BuildConvexHull();
    }

    [ContextMenu("Build convex hull")]
    public void BuildConvexHull() {
        // building array
        List<Vector3> positions = new List<Vector3>();
        foreach (var node in spline.bezierNodes) {
            positions.AddPos(node.GlobalTangentStart);
            positions.AddPos(node.transform.position);
            positions.AddPos(node.GlobalTangentEnd);
        }

        /* todo frames */

        sourcePositions = positions;
        hullPoints = ComputeConvexHull();
        Debug.Log(hullPoints.Count + " points");
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
                result.PopLast();
                --L;
            }

            result.Add(p);
            ++L;

            while (U >= 2 && ((p1 = result[0]) - result[1]).Cross(p - p1) <= 0) {
                result.PopFirst();
                --U;
            }
            if(U != 0)
               result.Insert(0, p);
            ++U;
        }


        return result;
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
    
    public static Vector3 PopLast(this List<Vector3> list)
    {
        Vector3 retVal = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return retVal;
    }
    
    public static Vector3 PopFirst(this List<Vector3> list)
    {
        Vector3 retVal = list[0];
        list.RemoveAt(0);
        return retVal;
    }
}