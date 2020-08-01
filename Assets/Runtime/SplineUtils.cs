﻿using UnityEditor;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Runtime {
    public static class SplineUtils {
        public static float SplinePointSize => SceneView.currentDrawingSceneView.size / 30f;
        public static float ControlPointSize => SplinePointSize / 2f;
    }
}