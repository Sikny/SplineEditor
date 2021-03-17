using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    [Serializable]
    public class BezierSplineData {
        public int instanceID;
        public List<BezierNodeData> nodes;
        public int divisions;

        public BezierSplineData(BezierSpline spline) {
            instanceID = spline.GetInstanceID();
            nodes = new List<BezierNodeData>();
            divisions = spline.divisionsBetweenTwoPoints;
            foreach (var bezierNode in spline.bezierNodes) {
                BezierNodeData nodeData = new BezierNodeData(bezierNode);
                nodes.Add(nodeData);
            }
        }

        public void Load(BezierSpline spline) {
            spline.divisionsBetweenTwoPoints = divisions;
            
            // clean all nodes except first (prefab base)
            var oldNodes = spline.GetComponentsInChildren<BezierNode>();
            for (int i = oldNodes.Length - 1; i > 0; --i) {
                Object.DestroyImmediate(oldNodes[i].gameObject);
            }
            
            spline.bezierNodes = new List<BezierNode>();

            int nodeCount = nodes.Count;
            for (int i = 0; i < nodeCount; i++) {
                var newNode = i == 0 ? oldNodes[0] : Object.Instantiate(oldNodes[0], oldNodes[0].transform.parent);
                nodes[i].Init(newNode);
            }

            spline.UpdateNodes();
        }
    }

    [Serializable]
    public class BezierNodeData {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 tangentStart;
        public Vector3 tangentEnd;
        public float roll;

        public BezierNodeData(BezierNode node) {
            Transform nodeT = node.transform;
            localPosition = nodeT.localPosition;
            localRotation = nodeT.localRotation;
            tangentStart = node.LocalTangent1;
            tangentStart = node.LocalTangent2;
            roll = node.roll;
        }

        public void Init(BezierNode node) {
            Transform nodeT = node.transform;
            nodeT.localPosition = localPosition;
            nodeT.localRotation = localRotation;
            node.LocalTangent1 = tangentStart;
            node.LocalTangent2 = tangentEnd;
            node.roll = roll;
        }
    }
    
    public class BezierData : ScriptableObject {
        [SerializeField] private List<BezierSplineData> splinesData;

        public void SaveSceneBezierSplines() {
            BezierSpline[] splines = FindObjectsOfType<BezierSpline>();
            splinesData = new List<BezierSplineData>();
            foreach (var spline in splines) {
                BezierSplineData splineData = new BezierSplineData(spline);
                splinesData.Add(splineData);
            }
        }

        public void LoadSceneBezierSplines() {
            BezierSpline[] splines = FindObjectsOfType<BezierSpline>();
            foreach (var spline in splines) {
                foreach (var splineData in splinesData) {
                    if (spline.GetInstanceID() == splineData.instanceID) {
                        splineData.Load(spline);
                    }
                }
            }
        }
    }
}