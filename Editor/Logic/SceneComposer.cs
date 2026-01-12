using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement; 
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;

namespace HoangTuDongAnh.FastSetup.Editor
{
    public static class SceneComposer
    {
        public static void Compose(SetupConfig config)
        {
            if (config.bootstrap == null) return;

            string folderPath = config.bootstrap.savePath.Replace("{ProjectName}", config.projectName);
            string sceneName = config.bootstrap.sceneName;
            string fullPath = Path.Combine(folderPath, $"{sceneName}.unity");

            EditorUtility.DisplayProgressBar("Fast Setup", "Đang khởi tạo Bootstrap Scene...", 0.9f);

            // 1. Tạo Scene mới
            // NewSceneSetup.DefaultGameObjects: Tạo sẵn Camera và Directional Light
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            newScene.name = sceneName;

            // 2. Spawn các System Prefabs
            if (config.bootstrap.systemPrefabs != null)
            {
                // Tạo một object cha để gom nhóm cho gọn
                GameObject systemsParent = new GameObject("--- SYSTEMS ---");
                
                foreach (string prefabPath in config.bootstrap.systemPrefabs)
                {
                    // Load Prefab từ ổ cứng
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                    
                    if (prefab != null)
                    {
                        // Instantiate Prefab và giữ nguyên kết nối (Prefab Link)
                        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                        instance.transform.SetParent(systemsParent.transform);
                        Debug.Log($"[Scene] Đã spawn: {prefab.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"[Scene] Không tìm thấy Prefab tại: {prefabPath}. (Có thể Module chưa tải xong?)");
                    }
                }
            }

            // 3. Lưu Scene ra file
            // Phải đảm bảo folder tồn tại trước
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            
            EditorSceneManager.SaveScene(newScene, fullPath);
            Debug.Log($"<color=green>[Scene] Đã lưu Scene tại: {fullPath}</color>");

            // 4. Tự động thêm vào Build Settings (Quan trọng!)
            AddSceneToBuildSettings(fullPath);

            EditorUtility.ClearProgressBar();
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            // Lấy danh sách scene hiện tại trong Build Settings
            var original = EditorBuildSettings.scenes.ToList();

            // Kiểm tra xem scene này đã có chưa
            if (original.Any(s => s.path == scenePath)) return;

            // Tạo mới EditorBuildSettingsScene
            var sceneToAdd = new EditorBuildSettingsScene(scenePath, true);

            // Thêm vào đầu danh sách (Index 0) để nó chạy đầu tiên
            original.Insert(0, sceneToAdd);

            // Cập nhật lại settings
            EditorBuildSettings.scenes = original.ToArray();
            Debug.Log("[Scene] Đã thêm Bootstrap vào Build Settings (Index 0).");
        }
    }
}
