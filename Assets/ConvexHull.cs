using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using SplineEditor.Runtime;
using UnityEngine;
using static SplineEditor.Runtime.BezierUtils;

public class ConvexHull : MonoBehaviour
{
    public BezierSpline spline;
    private List<Vector3> splinePositions;
    private List<Vector3> convexHullPositions;

    private void Awake()
    {
        convexHullPositions = new List<Vector3>();
    }

    [ContextMenu("HAHAHAHAHA")]
    void UpdateConvexHull()
    {
        if (splinePositions == null) splinePositions = new List<Vector3>();
        splinePositions.Clear();
        convexHullPositions.Clear();
        foreach (var pos in spline.RotationMinimisingFrames)
        {
            Vector3 newPos = new Vector3(0, pos.GlobalOrigin.y, pos.GlobalOrigin.z);
            splinePositions.Add(newPos);
        }
        Debug.Log("sizeof spline points : " + splinePositions.Count);
        JarvisMarch();
    }

    private void JarvisMarch()
    {
        if (convexHullPositions == null) convexHullPositions = new List<Vector3>();
        Vector3 pointOnHull = getLeftestPoint();
        Debug.Log("pointOnHull " + pointOnHull);
        Vector3 endPoint;
        do
        {
            Debug.Log("do");
            convexHullPositions.Add(pointOnHull);
            Debug.Log("convexHullPositions" + convexHullPositions[0]);
            Debug.Log(convexHullPositions);
            foreach (var p in convexHullPositions)
            {
                Debug.Log(p);
            }
            endPoint = splinePositions[0];
            for (int j = 0; j < splinePositions.Count; j++)
            {
                if (endPoint == pointOnHull || isPointLeftToLine(splinePositions[j],convexHullPositions.Last(),endPoint))
                    endPoint = splinePositions[j];
                pointOnHull = endPoint;
            }

        } while (endPoint == convexHullPositions.First());

        /*// S is the set of points
    // P will be the set of points which form the convex hull. Final set size is i.
    pointOnHull = leftmost point in S // which is guaranteed to be part of the CH(S)
    i := 0
    repeat
        P[i] := pointOnHull
        endpoint := S[0]      // initial endpoint for a candidate edge on the hull
        for j from 0 to |S| do
            // endpoint == pointOnHull is a rare case and can happen only when j == 1 and a better endpoint has not yet been set for the loop
            if (endpoint == pointOnHull) or (S[j] is on left of line from P[i] to endpoint) then
                endpoint := S[j]   // found greater left turn, update endpoint
        i := i + 1
        pointOnHull = endpoint
    until endpoint = P[0]      // wrapped around to first hull point*/
    }

    private bool isPointLeftToLine(Vector3 point, Vector3 lineA, Vector3 lineB)
    {
        return (lineB.x - lineA.x) * (point.y - lineA.y) - (lineA.y - lineA.y) * (point.x - lineA.x) >= 0;
    }

    private Vector3 getLeftestPoint()
    {
        Vector3 leftest = splinePositions[0];
        for(int i = 1; i < splinePositions.Count; i++)
        {
            if (splinePositions[i].z < leftest.z)
            {
                leftest = splinePositions[i];
            }
        }

        return leftest;
    }
    private void OnDrawGizmos()
    {
        if (convexHullPositions != null)
        {
            for (int i = 0; i < convexHullPositions.Count; ++i)
            {
                var origin = convexHullPositions[i];

                if (i < convexHullPositions.Count - 1)
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(origin, convexHullPositions[i + 1]);
                }
            }
        }
    }
}
