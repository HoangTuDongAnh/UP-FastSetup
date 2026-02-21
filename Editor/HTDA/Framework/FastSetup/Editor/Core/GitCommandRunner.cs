#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;

namespace HTDA.Framework.FastSetup.Editor
{
    internal static class GitCommandRunner
    {
        public static bool IsGitInstalled()
            => Run("--version", out _, out _);

        public static bool IsGitRepo(string projectRoot)
            => Directory.Exists(Path.Combine(projectRoot, ".git"));

        public static bool Run(string arguments, out string output, out string error)
        {
            output = "";
            error = "";

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    WorkingDirectory = FastSetupPaths.ProjectRoot,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var p = new Process { StartInfo = psi };
                p.Start();
                output = p.StandardOutput.ReadToEnd();
                error = p.StandardError.ReadToEnd();
                p.WaitForExit();

                return p.ExitCode == 0;
            }
            catch (Exception e)
            {
                error = $"System error: {e.Message}";
                return false;
            }
        }
    }
}
#endif