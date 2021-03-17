using System;
using System.IO;
using SplineEditor.Runtime;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// ReSharper disable once CheckNamespace
namespace SplineEditor.Editor {
    [InitializeOnLoad]
    public class BezierSaver {
        static BezierSaver() {
            EditorSceneManager.sceneSaving += OnSceneSave;
            EditorSceneManager.sceneOpened += OnSceneLoad;
            PrefabUtility.prefabInstanceUpdated += OnPrefabSave;
            PrefabStage.prefabStageOpened += OnPrefabLoad;
        }

        private static void OnSceneSave(Scene scene, string path) {
            OnAssetSaved(path);
        }

        private static void OnSceneLoad(Scene scene, OpenSceneMode mode) {
            OnAssetLoaded(scene.path);
        }

        private static void OnPrefabSave(GameObject instance) {
            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(instance);
            OnAssetSaved(path);
        }
        
        private static void OnPrefabLoad(PrefabStage obj) {
            OnAssetLoaded(obj.prefabAssetPath);
        }

        private static void OnAssetSaved(string path) {
            string bezierScriptableObjPath = path;
            bezierScriptableObjPath = Path.ChangeExtension(bezierScriptableObjPath, "asset");
            bezierScriptableObjPath = bezierScriptableObjPath.Replace("Assets", "Assets/Data/BezierData");

            BezierData bezierData = AssetDatabase.LoadAssetAtPath<BezierData>(bezierScriptableObjPath);
            if (bezierData == null) {   // create scriptableObject
                bezierData = ScriptableObject.CreateInstance<BezierData>();

                string directory = Path.GetDirectoryName(bezierScriptableObjPath);
                Directory.CreateDirectory(directory ?? throw new InvalidOperationException());
                AssetDatabase.CreateAsset(bezierData, bezierScriptableObjPath);
                AssetDatabase.SaveAssets();
            }
            
            bezierData.SaveSceneBezierSplines();
        }

        private static void OnAssetLoaded(string path) {
            string bezierScriptableObjPath = path;
            bezierScriptableObjPath = Path.ChangeExtension(bezierScriptableObjPath, "asset");
            bezierScriptableObjPath = bezierScriptableObjPath.Replace("Assets", "Assets/Data/BezierData");

            BezierData bezierData = AssetDatabase.LoadAssetAtPath<BezierData>(bezierScriptableObjPath);
            if (bezierData != null) {
                bezierData.LoadSceneBezierSplines();
            }
        }
    }
}