using UnityEditor;
using UnityEngine;
using System.IO;

namespace HoangTuDongAnh.FastSetup.Editor
{
    public class FastSetupMenu
    {
        // 1. Đường dẫn file Config người dùng sẽ sửa (Nằm trong Assets để ghi được)
        private const string UserConfigPath = "Assets/Editor/FastSetup/setup-config.json";
        
        // 2. Đường dẫn file Mẫu trong Package (Chỉ để đọc)
        private static string GetTemplatePath()
        {
            // Tìm file template trong dự án (dựa vào tên file gốc)
            string[] guids = AssetDatabase.FindAssets("setup-config-template"); 
            if (guids.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(guids[0]);
            }
            return null;
        }

        [MenuItem("FastSetup/0. Create Config File")]
        public static void CreateConfig()
        {
            // Nếu file config người dùng đã tồn tại thì mở nó lên
            if (File.Exists(UserConfigPath))
            {
                Object configObj = AssetDatabase.LoadAssetAtPath<Object>(UserConfigPath);
                Selection.activeObject = configObj;
                Debug.Log("File config đã tồn tại. Hãy chỉnh sửa nó.");
                return;
            }

            // Nếu chưa có, tiến hành Copy từ Template
            string templatePath = GetTemplatePath();
            if (string.IsNullOrEmpty(templatePath))
            {
                Debug.LogError("Không tìm thấy file mẫu 'setup-config-template' trong Package!");
                return;
            }

            // Tạo thư mục chứa nếu chưa có
            string dir = Path.GetDirectoryName(UserConfigPath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            // Copy file
            File.Copy(templatePath, UserConfigPath);
            AssetDatabase.Refresh();

            // Highlight file vừa tạo cho người dùng thấy
            Object newConfig = AssetDatabase.LoadAssetAtPath<Object>(UserConfigPath);
            Selection.activeObject = newConfig;
            Debug.Log($"<color=green>Đã tạo file cấu hình tại: {UserConfigPath}</color>");
        }
        
        [MenuItem("FastSetup/1. Initialize Structure")]
        public static void InitStructure()
        {
            // Đọc file từ UserConfigPath thay vì file gốc
            if (!File.Exists(UserConfigPath))
            {
                bool copy = EditorUtility.DisplayDialog("Thiếu Config", 
                    "Chưa tìm thấy file cấu hình trong Assets. Bạn có muốn tạo mới từ mẫu không?", "Tạo ngay", "Hủy");
                
                if (copy) CreateConfig();
                return;
            }

            string jsonContent = File.ReadAllText(UserConfigPath);
            SetupConfig config = JsonUtility.FromJson<SetupConfig>(jsonContent);

            if (config == null)
            {
                Debug.LogError("[FastSetup] File JSON lỗi!");
                return;
            }
            
            // Giai đoạn 2: Tạo Folder
            FolderGenerator.Generate(config);
    
            // Giai đoạn 3: Tải Module
            ModuleInstaller.InstallModules(config);

            // Refresh để đảm bảo Unity nhận diện được các Prefab vừa tải về ở bước 3
            AssetDatabase.Refresh(); 

            // Giai đoạn 4: Tạo Scene & Spawn Prefab
            // Lưu ý: Code chạy đồng bộ, nhưng AssetDatabase.Refresh() đôi khi cần độ trễ.
            // Tuy nhiên trong Editor script đơn giản, ta cứ gọi trực tiếp.
            SceneComposer.Compose(config);

            AssetDatabase.Refresh();
        }
    }
}

