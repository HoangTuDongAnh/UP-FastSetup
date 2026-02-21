#if UNITY_EDITOR
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HTDA.Framework.FastSetup.Editor
{
    internal static class SceneComposer
    {
        public static void Compose(SetupConfig config)
        {
            var b = config.bootstrap;
            if (b == null || !b.enabled) return;

            var sceneName = string.IsNullOrEmpty(b.sceneName) ? "Bootstrap" : b.sceneName;
            var folder = string.IsNullOrEmpty(b.savePath) ? "Assets/Scenes" : b.savePath;
            folder = folder.Replace("{ProjectName}", config.projectName ?? "Project").Replace("\\", "/");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var scenePath = Path.Combine(folder, $"{sceneName}.unity").Replace('\\', '/');

            try
            {
                EditorUtility.DisplayProgressBar("HTDA FastSetup", "Creating bootstrap scene...", 0.95f);

                var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                scene.name = sceneName;

                SpawnPrefabs(b.systemPrefabs);

                EditorSceneManager.SaveScene(scene, scenePath);
                Debug.Log($"<color=green>[FastSetup] Bootstrap scene saved:</color> {scenePath}");

                if (b.addToBuildSettings)
                    AddToBuildSettings(scenePath);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void SpawnPrefabs(string[] prefabPaths)
        {
            if (prefabPaths == null || prefabPaths.Length == 0) return;

            var parent = new GameObject("--- SYSTEMS ---");

            foreach (var p in prefabPaths)
            {
                var path = (p ?? "").Replace("\\", "/").Trim();
                if (string.IsNullOrEmpty(path)) continue;

                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null)
                {
                    Debug.LogWarning($"[FastSetup] Prefab not found: {path}");
                    continue;
                }

                var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (instance != null)
                    instance.transform.SetParent(parent.transform);
            }
        }

        private static void AddToBuildSettings(string scenePath)
        {
            var list = EditorBuildSettings.scenes.ToList();
            if (list.Any(s => s.path == scenePath))
                return;

            list.Insert(0, new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = list.ToArray();

            Debug.Log("[FastSetup] Added bootstrap scene to Build Settings (index 0).");
        }
    }
}
#endif