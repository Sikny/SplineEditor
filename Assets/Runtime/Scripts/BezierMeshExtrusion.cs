﻿using System.Collections.Generic;
using UnityEngine;
using UnityExtendedEditor.PrefabUtility;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public class BezierMeshExtrusion : MonoBehaviour, IPrefabStageListener {
        public MeshFilter meshFilter;
        public bool generateCollider;
        public BezierSpline bezierSpline;
        public float roadWidth = 1;
        public float roadThickness = 1;

        [Header("tiling"), SerializeField] private Vector2 upTiling = new Vector2(1, 1);
        [SerializeField] private Vector2 leftTiling = new Vector2(1, 1);
        [SerializeField] private Vector2 rightTiling = new Vector2(1, 1);
        [SerializeField] private Vector2 bottomTiling = new Vector2(1, 1);

        [SerializeField, HideInInspector] private MeshCollider meshCollider;

        [ContextMenu("Update Mesh")]
        public void UpdateMesh() {
            Mesh mesh = new Mesh();
            
            List<BezierUtils.BezierPos> vectorFrames = bezierSpline.RotationMinimisingFrames;
            int arrayLen = vectorFrames.Count;
            
            var vertices = new Vector3[arrayLen * 2 * 4 + 8];    // 2 vertices per bezier vertex * (2 faces + 2 sides) + 2 extremities 
            var normals = new Vector3[arrayLen * 2 * 4 + 8];
            var triangles = new int[(arrayLen * 6 - 6) * 4 + 2 * 6];    // 2 faces + 2 sides + 2 extremities
            var uvs = new Vector2[vertices.Length];

            int indexUp = 0;
            int indexBottom = arrayLen * 2;
            int indexLeftSide = arrayLen * 2 * 2;
            int indexRightSide = arrayLen * 2 * 3;
            int indexStartFace = arrayLen * 2 * 4;
            int indexEndFace = arrayLen * 2 * 4 + 4;
            int indexTriangles = 0;
            
            for (int i = 0; i < arrayLen; ++i) {
                
                var bezierCenter = vectorFrames[i].GlobalOrigin - transform.position;
                var normal = vectorFrames[i].Normal;
                var rotAxis = vectorFrames[i].LocalUp;
                var bezierDist = vectorFrames[i].BezierDistance;
                var uv1 = new Vector2(0, bezierDist);
                var uv2 = new Vector2(1, bezierDist);

                // up face
                vertices[indexUp] = bezierCenter + normal * roadWidth;
                vertices[indexUp + 1] = bezierCenter - normal * roadWidth;
                normals[indexUp] = normals[indexUp + 1] = rotAxis;
                uvs[indexUp] = uv1 * upTiling;
                uvs[indexUp+1] = uv2 * upTiling;
                
                // bottom face
                vertices[indexBottom] = vertices[indexUp] - rotAxis * roadThickness;
                vertices[indexBottom + 1] = vertices[indexUp + 1] - rotAxis * roadThickness;
                normals[indexBottom] = normals[indexBottom + 1] = -rotAxis;
                uvs[indexBottom] = uv1 * bottomTiling;
                uvs[indexBottom+1] = uv2 * bottomTiling;
                
                // left side
                vertices[indexLeftSide] = vertices[indexUp + 1];
                vertices[indexLeftSide + 1] = vertices[indexBottom + 1];
                normals[indexLeftSide] = normals[indexLeftSide + 1] = -normal;
                uvs[indexLeftSide] = uv1 * leftTiling;
                uvs[indexLeftSide+1] = uv2 * leftTiling;
                
                // right side
                vertices[indexRightSide] = vertices[indexUp];
                vertices[indexRightSide + 1] = vertices[indexBottom];
                normals[indexRightSide] = normals[indexRightSide + 1] = normal;
                uvs[indexRightSide] = uv1 * rightTiling;
                uvs[indexRightSide+1] = uv2 * rightTiling;

                if (indexUp > 1) {
                    // up face
                    triangles[indexTriangles++] = indexUp + 1;
                    triangles[indexTriangles++] = indexUp;
                    triangles[indexTriangles++] = indexUp - 1;
                    
                    triangles[indexTriangles++] = indexUp - 1;
                    triangles[indexTriangles++] = indexUp;
                    triangles[indexTriangles++] = indexUp - 2;
                    
                    // bottom face
                    triangles[indexTriangles++] = indexBottom - 1;
                    triangles[indexTriangles++] = indexBottom;
                    triangles[indexTriangles++] = indexBottom + 1;
                    
                    triangles[indexTriangles++] = indexBottom - 1;
                    triangles[indexTriangles++] = indexBottom - 2;
                    triangles[indexTriangles++] = indexBottom;
                    
                    // side left face
                    triangles[indexTriangles++] = indexLeftSide;
                    triangles[indexTriangles++] = indexLeftSide - 2;
                    triangles[indexTriangles++] = indexLeftSide + 1;

                    triangles[indexTriangles++] = indexLeftSide + 1;
                    triangles[indexTriangles++] = indexLeftSide - 2;
                    triangles[indexTriangles++] = indexLeftSide - 1;
                    
                    // side right face
                    triangles[indexTriangles++] = indexRightSide - 2;
                    triangles[indexTriangles++] = indexRightSide;
                    triangles[indexTriangles++] = indexRightSide + 1;

                    triangles[indexTriangles++] = indexRightSide + 1;
                    triangles[indexTriangles++] = indexRightSide - 1;
                    triangles[indexTriangles++] = indexRightSide - 2;
                }
                indexUp += 2;
                indexBottom += 2;
                indexLeftSide += 2;
                indexRightSide += 2;
            }
            // start face
            vertices[indexStartFace] = vertices[0];
            vertices[indexStartFace + 1] = vertices[1];
            vertices[indexStartFace + 2] = vertices[arrayLen * 2];
            vertices[indexStartFace + 3] = vertices[arrayLen * 2 + 1];
            normals[indexStartFace] = normals[indexStartFace + 1] = normals[indexStartFace + 2]
                = normals[indexStartFace + 3] = -vectorFrames[0].Tangent;

            triangles[indexTriangles++] = indexStartFace + 1;
            triangles[indexTriangles++] = indexStartFace;
            triangles[indexTriangles++] = indexStartFace + 2;
            
            triangles[indexTriangles++] = indexStartFace + 2;
            triangles[indexTriangles++] = indexStartFace + 3;
            triangles[indexTriangles++] = indexStartFace + 1;
            
            // end face
            vertices[indexEndFace] = vertices[arrayLen * 2 - 1];
            vertices[indexEndFace + 1] = vertices[arrayLen * 2 - 2];
            vertices[indexEndFace + 2] = vertices[arrayLen * 2 * 2 - 1];
            vertices[indexEndFace + 3] = vertices[arrayLen * 2 * 2 - 2];
            normals[indexEndFace] = normals[indexEndFace + 1] = normals[indexEndFace + 2]
                = normals[indexEndFace + 3] = -vectorFrames[0].Tangent;

            triangles[indexTriangles++] = indexEndFace + 1;
            triangles[indexTriangles++] = indexEndFace;
            triangles[indexTriangles++] = indexEndFace + 2;
            
            triangles[indexTriangles++] = indexEndFace + 2;
            triangles[indexTriangles++] = indexEndFace + 3;
            triangles[indexTriangles] = indexEndFace + 1;

            // finally set mesh
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.triangles = triangles;

            mesh.uv = uvs;
            meshFilter.mesh = mesh;

            if (generateCollider)
            {
                if (!meshCollider)
                {
                    meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
                    if(!meshCollider) meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                }

                meshCollider.sharedMesh = mesh;
            }
        }

        public void OnPrefabOpened()
        {
            UpdateMesh();
        }

        public void OnPrefabClosing()
        {
            DestroyImmediate(meshFilter.sharedMesh);
            meshFilter.sharedMesh = null;
            if (meshCollider != null)
                meshCollider.sharedMesh = null;
        }
    }
}
