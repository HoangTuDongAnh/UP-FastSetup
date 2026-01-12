using UnityEngine;
using UnityEditor;
using System.IO;

namespace HoangTuDongAnh.FastSetup.Editor
{
    public static class FolderGenerator
    {
        public static void Generate(SetupConfig config)
        {
            float progress = 0f;
            int total = config.folders.Length;

            EditorUtility.DisplayProgressBar("Fast Setup", "Đang tạo cấu trúc thư mục...", 0f);

            foreach (string rawPath in config.folders)
            {
                progress++;

                // 1. Xử lý Dynamic Name ({ProjectName} -> Tên thật)
                string path = rawPath.Replace("{ProjectName}", config.projectName);

                // 2. Tạo Folder vật lý (Sử dụng System.IO như yêu cầu)
                // Directory.CreateDirectory rất mạnh, nó tự tạo cả folder cha nếu chưa có.
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                // 3. Tạo file .gitkeep
                // Git không theo dõi folder rỗng. Ta cần file này để giữ chỗ.
                CreateGitKeep(path);

                // Cập nhật thanh tiến trình
                EditorUtility.DisplayProgressBar("Fast Setup", $"Đang tạo: {path}", progress / total);
            }

            // 4. Báo cho Unity biết là có file mới sinh ra để nó Refresh lại Project Window
            AssetDatabase.Refresh();

            EditorUtility.ClearProgressBar();
            Debug.Log(
                $"<color=green>[FastSetup] Đã tạo xong {total} thư mục cho dự án '{config.projectName}'!</color>");
        }

        private static void CreateGitKeep(string folderPath)
        {
            string filePath = Path.Combine(folderPath, ".gitkeep");

            // Chỉ tạo nếu chưa có file này
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, ""); // Tạo một file rỗng
            }
        }
    }
}