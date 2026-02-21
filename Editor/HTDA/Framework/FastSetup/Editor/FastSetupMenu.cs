#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    public static class FastSetupMenu
    {
        private const string DefaultTemplateId = "general"; // meta.id of General template

        [MenuItem("HTDA/FastSetup/0 - Create Config (Default)", priority = 0)]
        public static void CreateConfigDefault()
        {
            CreateConfigFromTemplateId(DefaultTemplateId);
        }

        [MenuItem("HTDA/FastSetup/0.1 - Create Config From Template...", priority = 1)]
        public static void CreateConfigFromTemplate()
        {
            var templates = SetupTemplateProvider.LoadAllTemplates();
            if (templates.Count == 0)
            {
                EditorUtility.DisplayDialog("FastSetup", "No templates found.", "OK");
                return;
            }

            // DisplayCustomMenu requires GUIContent[]
            var contents = new GUIContent[templates.Count];
            for (int i = 0; i < templates.Count; i++)
            {
                var t = templates[i];
                var tooltip = string.IsNullOrEmpty(t.description) ? t.sourcePath : t.description;
                contents[i] = new GUIContent(t.displayName, tooltip);
            }

            EditorUtility.DisplayCustomMenu(
                new Rect(100, 100, 0, 0),
                contents,
                0,
                // IMPORTANT: callback signature is (object userData, string[] options, int selected)
                (userData, options, selected) =>
                {
                    var list = (System.Collections.Generic.List<SetupTemplate>)userData;
                    if (selected < 0 || selected >= list.Count) return;

                    var chosen = list[selected];
                    CreateConfigFromJson(chosen.configJson, reveal: true);
                },
                templates
            );
        }

        [MenuItem("HTDA/FastSetup/0.2 - Export Current Config As Template...", priority = 2)]
        public static void ExportCurrentConfigAsTemplate()
        {
            var configPath = FastSetupPaths.ConfigFullPath;
            if (!File.Exists(configPath))
            {
                EditorUtility.DisplayDialog("FastSetup", "Config not found. Create config first.", "OK");
                return;
            }

            var json = File.ReadAllText(configPath);
            ExportTemplateWindow.Open(json);
        }

        [MenuItem("HTDA/FastSetup/1 - Run Setup", priority = 10)]
        public static void RunSetup()
        {
            var configPath = FastSetupPaths.ConfigFullPath;

            if (!File.Exists(configPath))
            {
                var create = EditorUtility.DisplayDialog("FastSetup", "Config not found. Create it now?", "Create", "Cancel");
                if (create) CreateConfigDefault();
                return;
            }

            if (!JsonFile.TryRead<SetupConfig>(configPath, out var config))
            {
                EditorUtility.DisplayDialog("FastSetup", "Config JSON invalid. Please fix it and try again.", "OK");
                return;
            }

            FolderGenerator.Generate(config);
            ModuleInstaller.Install(config);
            SceneComposer.Compose(config);

            AssetDatabase.Refresh();
            Debug.Log("<color=green>[FastSetup] Completed.</color>");
        }

        [MenuItem("HTDA/FastSetup/Open Config Folder", priority = 50)]
        public static void OpenConfigFolder()
        {
            FastSetupPaths.EnsureConfigDirectory();
            EditorUtility.RevealInFinder(Path.GetDirectoryName(FastSetupPaths.ConfigFullPath));
        }

        [MenuItem("HTDA/FastSetup/Open Templates Folder", priority = 51)]
        public static void OpenTemplatesFolder()
        {
            FastSetupPaths.EnsureProjectTemplatesDirectory();
            EditorUtility.RevealInFinder(FastSetupPaths.ProjectTemplatesDirFullPath);
        }

        private static void CreateConfigFromTemplateId(string templateId)
        {
            var templates = SetupTemplateProvider.LoadAllTemplates();
            foreach (var t in templates)
            {
                if (t.hasMeta && t.id == templateId)
                {
                    CreateConfigFromJson(t.configJson, reveal: true);
                    return;
                }
            }

            // fallback: open picker
            CreateConfigFromTemplate();
        }

        private static void CreateConfigFromJson(string json, bool reveal)
        {
            FastSetupPaths.EnsureConfigDirectory();

            var configPath = FastSetupPaths.ConfigFullPath;
            if (File.Exists(configPath))
            {
                var overwrite = EditorUtility.DisplayDialog("FastSetup", "Config already exists. Overwrite?", "Overwrite", "Cancel");
                if (!overwrite)
                {
                    if (reveal) EditorUtility.RevealInFinder(configPath);
                    return;
                }
            }

            File.WriteAllText(configPath, json);
            AssetDatabase.Refresh();

            Debug.Log($"<color=green>[FastSetup] Config written:</color> {configPath}");
            if (reveal) EditorUtility.RevealInFinder(configPath);
        }
    }
}
#endif