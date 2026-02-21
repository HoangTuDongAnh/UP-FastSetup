#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    internal static class ModuleInstaller
    {
        public static void Install(SetupConfig config)
        {
            if (config.modules == null || config.modules.Length == 0)
            {
                Debug.Log("[FastSetup] No modules configured.");
                return;
            }

            if (!GitCommandRunner.IsGitInstalled())
            {
                EditorUtility.DisplayDialog("FastSetup", "Git is not installed or not available in PATH.", "OK");
                return;
            }

            var root = FastSetupPaths.ProjectRoot;
            if (!GitCommandRunner.IsGitRepo(root))
            {
                EditorUtility.DisplayDialog("FastSetup", "Project is not a git repository (missing .git). Please run `git init` first.", "OK");
                return;
            }

            try
            {
                var total = config.modules.Length;
                for (int i = 0; i < total; i++)
                {
                    var m = config.modules[i];
                    if (m == null) continue;

                    EditorUtility.DisplayProgressBar("HTDA FastSetup", $"Installing: {m.name}", (i + 1f) / total);

                    var relPath = (m.path ?? "").Replace("\\", "/").Trim();
                    if (string.IsNullOrEmpty(relPath))
                    {
                        Debug.LogError("[FastSetup] Module path is empty.");
                        continue;
                    }

                    var fullPath = Path.Combine(root, relPath).Replace('\\', '/');
                    if (Directory.Exists(fullPath))
                    {
                        Debug.LogWarning($"[FastSetup] Skip (exists): {relPath}");
                        continue;
                    }

                    var parent = Path.GetDirectoryName(fullPath);
                    if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                        Directory.CreateDirectory(parent);

                    var args = $"submodule add {m.url} {relPath}";
                    var ok = GitCommandRunner.Run(args, out var output, out var error);

                    if (!ok)
                    {
                        Debug.LogError($"[FastSetup] Failed: {m.name}\n{error}");
                    }
                    else
                    {
                        Debug.Log($"<color=green>[FastSetup] Installed:</color> {m.name}\n{output}");
                    }
                }

                // Ensure initialized
                GitCommandRunner.Run("submodule update --init --recursive", out _, out _);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.Refresh();
            }
        }
    }
}
#endif