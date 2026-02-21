#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    internal sealed class SetupTemplate
    {
        public string id;
        public string displayName;
        public string description;
        public string sourcePath;       // asset path (package) OR full path (project)
        public bool isProjectTemplate;

        // For applying template:
        public string configJson;       // JSON string for SetupConfig (ready to write)
        public bool hasMeta;
        public string[] tags;
        public string version;
    }

    internal static class SetupTemplateProvider
    {
        // Package templates naming: fs-template-*.json
        private const string PackageTemplateNamePrefix = "fs-template-";

        public static List<SetupTemplate> LoadAllTemplates()
        {
            var list = new List<SetupTemplate>(16);
            LoadPackageTemplates(list);
            LoadProjectTemplates(list);

            list.Sort((a, b) =>
            {
                var s = a.isProjectTemplate.CompareTo(b.isProjectTemplate);
                if (s != 0) return s; // package first
                return string.Compare(a.displayName, b.displayName, StringComparison.OrdinalIgnoreCase);
            });

            return list;
        }

        private static void LoadPackageTemplates(List<SetupTemplate> list)
        {
            var guids = AssetDatabase.FindAssets($"{PackageTemplateNamePrefix} t:TextAsset");
            if (guids == null) return;

            foreach (var guid in guids)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(assetPath)) continue;

                var fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (!fileName.StartsWith(PackageTemplateNamePrefix, StringComparison.OrdinalIgnoreCase))
                    continue;

                var ta = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);
                if (ta == null) continue;

                if (TryParseTemplateJson(ta.text, fileName, out var t))
                {
                    t.sourcePath = assetPath;
                    t.isProjectTemplate = false;
                    list.Add(t);
                }
            }
        }

        private static void LoadProjectTemplates(List<SetupTemplate> list)
        {
            FastSetupPaths.EnsureProjectTemplatesDirectory();
            var dir = FastSetupPaths.ProjectTemplatesDirFullPath;
            if (!Directory.Exists(dir)) return;

            foreach (var file in Directory.GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly))
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                if (string.IsNullOrEmpty(fileName)) continue;

                var json = File.ReadAllText(file);
                if (TryParseTemplateJson(json, fileName, out var t))
                {
                    t.sourcePath = file; // full path
                    t.isProjectTemplate = true;
                    t.displayName = t.hasMeta ? $"(Project) {t.displayName}" : $"(Project) {t.displayName}";
                    list.Add(t);
                }
            }
        }

        /// <summary>
        /// Parse template JSON:
        /// - New format: { "meta": {...}, "config": {SetupConfig...} }
        /// - Legacy format: {SetupConfig...}
        /// </summary>
        private static bool TryParseTemplateJson(string json, string fallbackFileNameNoExt, out SetupTemplate template)
        {
            template = null;
            if (string.IsNullOrWhiteSpace(json))
                return false;

            // Heuristic: detect container format
            var isContainer = json.Contains("\"meta\"") && json.Contains("\"config\"");
            if (isContainer)
            {
                try
                {
                    var container = JsonUtility.FromJson<SetupTemplateContainer>(json);
                    if (container == null || container.config == null)
                        return false;

                    var meta = container.meta ?? new TemplateMeta();
                    var id = !string.IsNullOrEmpty(meta.id) ? meta.id : fallbackFileNameNoExt;

                    var name = !string.IsNullOrEmpty(meta.name)
                        ? meta.name
                        : ToDisplayName(fallbackFileNameNoExt);

                    template = new SetupTemplate
                    {
                        id = id,
                        displayName = name,
                        description = meta.description ?? "",
                        tags = meta.tags ?? Array.Empty<string>(),
                        version = meta.version ?? "",
                        hasMeta = true,
                        configJson = JsonUtility.ToJson(container.config, true)
                    };
                    return true;
                }
                catch
                {
                    // fallthrough to legacy attempt
                }
            }

            // Legacy: treat json as SetupConfig directly
            try
            {
                var cfg = JsonUtility.FromJson<SetupConfig>(json);
                if (cfg == null) return false;

                template = new SetupTemplate
                {
                    id = fallbackFileNameNoExt,
                    displayName = ToDisplayName(fallbackFileNameNoExt),
                    description = "",
                    tags = Array.Empty<string>(),
                    version = "",
                    hasMeta = false,
                    configJson = json
                };
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string ToDisplayName(string fileNameNoExt)
        {
            // fs-template-puzzle -> Puzzle
            var s = fileNameNoExt;

            if (s.StartsWith(PackageTemplateNamePrefix, StringComparison.OrdinalIgnoreCase))
                s = s.Substring(PackageTemplateNamePrefix.Length);

            s = s.Replace('_', '-').Replace('.', '-');
            var parts = s.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i].ToLowerInvariant();
                parts[i] = char.ToUpperInvariant(p[0]) + p.Substring(1);
            }
            return string.Join(" ", parts);
        }
    }
}
#endif