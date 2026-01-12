using UnityEngine;
using UnityEditor;
using System.IO;

namespace HoangTuDongAnh.FastSetup.Editor
{
    public static class ModuleInstaller
    {
        public static void InstallModules(SetupConfig config)
        {
            // 1. Kiểm tra máy có Git chưa
            if (!GitCommandRunner.IsGitInstalled())
            {
                EditorUtility.DisplayDialog("Lỗi", "Máy tính chưa cài đặt Git! Vui lòng cài Git và khởi động lại Unity.", "OK");
                return;
            }

            // 2. Kiểm tra danh sách rỗng
            if (config.modules == null || config.modules.Length == 0)
            {
                Debug.Log("[FastSetup] Không có module nào được khai báo trong config.");
                return;
            }

            int total = config.modules.Length;
            float current = 0;

            foreach (var module in config.modules)
            {
                current++;
                EditorUtility.DisplayProgressBar("Fast Setup", $"Đang cài đặt Module: {module.name}...", current / total);

                // Kiểm tra xem folder module đã tồn tại chưa để tránh lỗi
                if (Directory.Exists(module.path))
                {
                    Debug.LogWarning($"[Skip] Module '{module.name}' đã tồn tại tại {module.path}");
                    continue;
                }

                // --- LỆNH QUAN TRỌNG NHẤT ---
                // git submodule add <url> <path>
                string args = $"submodule add {module.url} {module.path}";
                
                string output, error;
                bool success = GitCommandRunner.Run(args, out output, out error);

                if (success)
                {
                    Debug.Log($"<color=green>[Success] Đã cài đặt: {module.name}</color>");
                }
                else
                {
                    Debug.LogError($"[Fail] Lỗi khi cài {module.name}:\n{error}");
                    // Có thể thêm logic: Nếu lỗi mạng thì retry? Ở đây ta tạm dừng báo lỗi.
                }
            }

            EditorUtility.ClearProgressBar();
        }
    }
}