#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    internal static class FolderGenerator
    {
        public static void Generate(SetupConfig config)
        {
            var all = CollectFolders(config);
            if (all.Count == 0)
            {
                Debug.Log("[FastSetup] No folders configured.");
                return;
            }

            try
            {
                EditorUtility.DisplayProgressBar("HTDA FastSetup", "Creating folders...", 0f);

                for (int i = 0; i < all.Count; i++)
                {
                    var rel = all[i];
                    var full = ToFullPath(rel);

                    EditorUtility.DisplayProgressBar("HTDA FastSetup", $"Creating: {rel}", (i + 1f) / all.Count);

                    if (!Directory.Exists(full))
                        Directory.CreateDirectory(full);

                    EnsureGitKeep(full);
                }

                AssetDatabase.Refresh();
                Debug.Log($"<color=green>[FastSetup] Folder generation completed ({all.Count}).</color>");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static List<string> CollectFolders(SetupConfig config)
        {
            var result = new List<string>(256);

            // 1) legacy flat list
            if (config.folders != null)
            {
                foreach (var f in config.folders)
                {
                    var path = Normalize(ReplaceTokens(f, config));
                    if (!string.IsNullOrEmpty(path))
                        result.Add(path);
                }
            }

            // 2) new grouped layout
            if (config.folderGroups != null)
            {
                foreach (var g in config.folderGroups)
                {
                    if (g == null) continue;

                    var root = Normalize(ReplaceTokens(g.root, config));
                    if (string.IsNullOrEmpty(root)) continue;

                    // always include group root itself
                    result.Add(root);

                    if (g.paths == null) continue;
                    foreach (var p in g.paths)
                    {
                        var child = Normalize(ReplaceTokens(p, config));
                        if (string.IsNullOrEmpty(child)) continue;

                        // Join root + child (child can contain nested subfolders via "/")
                        result.Add($"{root}/{child}".Replace("\\", "/"));
                    }
                }
            }

            // deduplicate (case-insensitive on Windows)
            var seen = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            var finalList = new List<string>(result.Count);
            foreach (var p in result)
            {
                if (seen.Add(p))
                    finalList.Add(p);
            }

            return finalList;
        }

        private static string ReplaceTokens(string input, SetupConfig config)
        {
            return (input ?? "")
                .Replace("{ProjectName}", config.projectName ?? "Project")
                .Replace("\\", "/");
        }

        private static string Normalize(string path)
        {
            var p = (path ?? "").Trim().Replace("\\", "/");
            while (p.EndsWith("/")) p = p.Substring(0, p.Length - 1);
            return p;
        }

        private static string ToFullPath(string projectRelativeOrAssetsPath)
        {
            var p = projectRelativeOrAssetsPath;

            // Absolute
            if (Path.IsPathRooted(p)) return p;

            // project-root relative
            return Path.Combine(FastSetupPaths.ProjectRoot, p).Replace('\\', '/');
        }

        private static void EnsureGitKeep(string folderFullPath)
        {
            var f = Path.Combine(folderFullPath, ".gitkeep");
            if (!File.Exists(f))
                File.WriteAllText(f, "");
        }
    }
}
#endif