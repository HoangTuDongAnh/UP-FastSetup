using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace HoangTuDongAnh.FastSetup.Editor
{
    public static class GitCommandRunner
    {
        public static bool Run(string arguments, out string output, out string error)
        {
            output = "";
            error = "";

            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "git"; // Gọi lệnh git
                process.StartInfo.Arguments = arguments;
                
                // Thiết lập để chạy ngầm không hiện cửa sổ CMD đen xì
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true; // Bắt lấy kết quả thành công
                process.StartInfo.RedirectStandardError = true;  // Bắt lấy thông báo lỗi
                process.StartInfo.CreateNoWindow = true;
                
                // Quan trọng: Chạy lệnh từ thư mục gốc của Project (chứa folder Assets)
                // Application.dataPath trả về .../MyProject/Assets, ta cần lùi lại 1 cấp
                process.StartInfo.WorkingDirectory = Directory.GetParent(Application.dataPath).FullName;

                process.Start();

                // Đọc kết quả (Hàm này sẽ chờ git chạy xong mới đi tiếp - Synchronous)
                output = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
                
                process.WaitForExit();

                // Nếu ExitCode = 0 tức là thành công, khác 0 là lỗi
                return process.ExitCode == 0;
            }
            catch (System.Exception e)
            {
                error = $"Lỗi hệ thống: {e.Message}. Có thể máy bạn chưa cài Git?";
                return false;
            }
        }
        
        public static bool IsGitInstalled()
        {
            return Run("--version", out _, out _);
        }
    }
}
