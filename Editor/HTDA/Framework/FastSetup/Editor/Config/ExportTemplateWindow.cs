#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    internal sealed class ExportTemplateWindow : EditorWindow
    {
        private string _fileName = "fs-template-custom.json";
        private string _id = "custom";
        private string _name = "Custom";
        private string _description = "";
        private string _tagsCsv = "custom";
        private string _version = "1.0.0";

        private string _sourceConfigJson = null;

        public static void Open(string sourceConfigJson)
        {
            var w = CreateInstance<ExportTemplateWindow>();
            w.titleContent = new GUIContent("Export Template");
            w._sourceConfigJson = sourceConfigJson;
            w.minSize = new Vector2(420, 260);
            w.ShowUtility();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Template Metadata", EditorStyles.boldLabel);

            _fileName = EditorGUILayout.TextField(new GUIContent("File name", "Saved under ProjectSettings/HTDA/FastSetup/Templates"), _fileName);
            if (!_fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                _fileName += ".json";

            _id = EditorGUILayout.TextField(new GUIContent("ID", "Stable id (e.g. general, puzzle, match3)"), _id);
            _name = EditorGUILayout.TextField(new GUIContent("Name", "Shown in the template menu"), _name);
            _version = EditorGUILayout.TextField(new GUIContent("Version", "Template version"), _version);

            EditorGUILayout.LabelField("Description");
            _description = EditorGUILayout.TextArea(_description, GUILayout.Height(48));

            _tagsCsv = EditorGUILayout.TextField(new GUIContent("Tags (CSV)", "Comma separated tags, e.g. puzzle,2d,mobile"), _tagsCsv);

            EditorGUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Cancel"))
                {
                    Close();
                    return;
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Save Template", GUILayout.Width(140)))
                {
                    SaveTemplate();
                }
            }
        }

        private void SaveTemplate()
        {
            if (string.IsNullOrEmpty(_sourceConfigJson))
            {
                EditorUtility.DisplayDialog("FastSetup", "Source config JSON is empty.", "OK");
                return;
            }

            var cfg = JsonUtility.FromJson<SetupConfig>(_sourceConfigJson);
            if (cfg == null)
            {
                EditorUtility.DisplayDialog("FastSetup", "Current config JSON is invalid.", "OK");
                return;
            }

            FastSetupPaths.EnsureProjectTemplatesDirectory();

            var meta = new TemplateMeta
            {
                id = (_id ?? "").Trim(),
                name = (_name ?? "").Trim(),
                description = (_description ?? "").Trim(),
                version = string.IsNullOrEmpty(_version) ? "1.0.0" : _version.Trim(),
                tags = ParseTags(_tagsCsv)
            };

            var container = new SetupTemplateContainer
            {
                meta = meta,
                config = cfg
            };

            var json = JsonUtility.ToJson(container, true);
            var fullPath = Path.Combine(FastSetupPaths.ProjectTemplatesDirFullPath, _fileName).Replace('\\', '/');

            File.WriteAllText(fullPath, json);
            AssetDatabase.Refresh();

            EditorUtility.DisplayDialog("FastSetup", $"Template saved:\n{fullPath}", "OK");
            Close();
        }

        private static string[] ParseTags(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return Array.Empty<string>();
            var parts = csv.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Trim();
            return parts;
        }
    }
}
#endif