using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    public class BezierCircuit : MonoBehaviour
    {
        [SerializeField] private List<BezierPath> circuitParts;

        public void Init()
        {
            Clear();
            var instantiatedParts = new List<BezierPath>();
            int partsCount = circuitParts.Count;
            for (int i = 0; i < partsCount; ++i) {
                BezierPath bezierPath;
#if UNITY_EDITOR
                if (Application.isPlaying)
#endif
                    bezierPath = Instantiate(circuitParts[i], transform);
#if UNITY_EDITOR
                else
                    bezierPath = PrefabUtility.InstantiatePrefab(circuitParts[i], transform) as BezierPath;
#endif
                instantiatedParts.Add(bezierPath);
                if (i > 0)
                {
                    var previousPartLastNode = instantiatedParts[i - 1].bezierSpline.bezierNodes.Last();
                    
                    bezierPath.transform.position = previousPartLastNode.transform.position;
                    bezierPath.transform.rotation = previousPartLastNode.transform.rotation *
                            Quaternion.Inverse(bezierPath.bezierSpline.bezierNodes[0].transform.rotation);
                }
            }
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            var instantiatedParts = GetComponentsInChildren<BezierPath>(true);
            foreach (var instantiatedPart in instantiatedParts)
            {
                if (instantiatedPart == null) return;
#if UNITY_EDITOR
                if(Application.isPlaying)
#endif
                    Destroy(instantiatedPart.gameObject);
#if UNITY_EDITOR
                else
                    DestroyImmediate(instantiatedPart.gameObject);
#endif
            }
        }
    }
}
