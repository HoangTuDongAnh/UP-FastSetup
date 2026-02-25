#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    public sealed class FastSetupWizardWindow : EditorWindow
    {
        private string _gitRef = "";          // empty => default branch
        private bool _includeSettings = false;
        private Vector2 _scroll;

        [MenuItem("HTDA/FastSetup/Setup Wizard...", priority = 0)]
        public static void Open() => GetWindow<FastSetupWizardWindow>("FastSetup Wizard");

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            EditorGUILayout.LabelField("HTDA FastSetup Wizard", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);

            EditorGUILayout.HelpBox(
                "Workflow đơn giản:\n" +
                "1) Install Framework (cài packages)\n" +
                "2) Run Setup (tạo folder + Bootstrap scene)\n\n" +
                "Git Ref để trống = dùng default branch của repo (master/main tuỳ repo).",
                MessageType.Info);

            EditorGUILayout.Space(6);

            _gitRef = EditorGUILayout.TextField("Git Ref (optional)", _gitRef);
            _includeSettings = EditorGUILayout.ToggleLeft("Include UP-Settings (optional)", _includeSettings);

            EditorGUILayout.Space(8);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Install Framework", GUILayout.Height(30)))
                {
                    FrameworkInstallerResumable.InstallAll(_gitRef, includeOptional: _includeSettings);
                }

                if (GUILayout.Button("Clear Installer State", GUILayout.Height(30)))
                {
                    FrameworkInstallerResumable.ClearInstallerState();
                }
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Project Setup", EditorStyles.boldLabel);

            if (GUILayout.Button("Run Setup (Folders + Bootstrap Scene)", GUILayout.Height(34)))
            {
                FastSetupMenu.RunSetup();
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox(
                "Nếu Bootstrap scene đang trống: mở scene và add GameBootstrap (UP-SceneFlow) để init services.\n" +
                "Prototype 1 scene thì có thể bỏ qua GameBootstrap.",
                MessageType.None);

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif