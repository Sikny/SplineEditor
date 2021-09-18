using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime
{
    public class BezierCircuit : MonoBehaviour
    {
        [SerializeField] private List<BezierPath> circuitParts;

        [ContextMenu("Init")]
        private void Init()
        {
            var instantiatedParts = new List<BezierPath>();
            int partsCount = circuitParts.Count;
            for (int i = 0; i < partsCount; ++i)
            {
                var bezierPath = Instantiate(circuitParts[i], transform);
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
        private void Clear()
        {
            var instantiatedParts = GetComponentsInChildren<BezierPath>();
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
