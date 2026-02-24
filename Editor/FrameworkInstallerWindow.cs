#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    public sealed class FrameworkInstallerWindow : EditorWindow
    {
        [Serializable]
        private class PackageSpec
        {
            public string DisplayName;
            public string PackageName;   // package.json -> "name" (com.xxx)
            public string GitUrl;        // https://github.com/.../repo.git
            public bool Enabled = true;

            public string BuildGitId(string gitRef)
            {
                var url = GitUrl?.Trim();
                if (string.IsNullOrEmpty(url)) return url;

                if (!url.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                    url += ".git";

                gitRef = gitRef?.Trim();
                if (string.IsNullOrEmpty(gitRef))
                    return url;

                return $"{url}#{gitRef}";
            }
        }

        // ====== CONFIG HERE ======
        // Bạn thay GitUrl + PackageName theo repo/package.json của bạn.
        private readonly List<PackageSpec> _packages = new()
        {
            new PackageSpec { DisplayName="UP-Core",      PackageName="com.htda.framework.core",      GitUrl="https://github.com/HoangTuDongAnh/UP-Core.git" },
            new PackageSpec { DisplayName="UP-Events",    PackageName="com.htda.framework.events",    GitUrl="https://github.com/HoangTuDongAnh/UP-Events.git" },
            new PackageSpec { DisplayName="UP-Pooling",   PackageName="com.htda.framework.pooling",   GitUrl="https://github.com/HoangTuDongAnh/UP-Pooling.git" },
            new PackageSpec { DisplayName="UP-FSM",       PackageName="com.htda.framework.fsm",       GitUrl="https://github.com/HoangTuDongAnh/UP-FSM.git" },
            new PackageSpec { DisplayName="UP-SceneFlow", PackageName="com.htda.framework.sceneflow", GitUrl="https://github.com/HoangTuDongAnh/UP-SceneFlow.git" },
            new PackageSpec { DisplayName="UP-Settings",  PackageName="com.htda.framework.settings",  GitUrl="https://github.com/HoangTuDongAnh/UP-Settings.git", Enabled=false },
        };

        private string _gitRef = "main"; // bạn có thể đổi thành "v0.1.0"
        private Vector2 _scroll;

        private bool _isInstalling;
        private Queue<PackageSpec> _queue;
        private HashSet<string> _installedNames;

        private ListRequest _listReq;
        private AddRequest _addReq;

        [MenuItem("HTDA/FastSetup/Install Framework...")]
        public static void Open()
        {
            GetWindow<FrameworkInstallerWindow>("HTDA Framework Installer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Install HTDA Framework Packages (Git)", EditorStyles.boldLabel);

            EditorGUILayout.Space(6);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Git Ref (branch/tag/commit)", GUILayout.Width(160));
                _gitRef = EditorGUILayout.TextField(_gitRef);
            }

            EditorGUILayout.HelpBox(
                "Git Ref examples: main, develop, v0.1.0, 1a2b3c4d (commit hash)\n" +
                "Each package repo must be a valid UPM package (package.json at root).",
                MessageType.Info);

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Packages", EditorStyles.boldLabel);

            _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.Height(220));
            for (int i = 0; i < _packages.Count; i++)
            {
                var p = _packages[i];
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        p.Enabled = EditorGUILayout.Toggle(p.Enabled, GUILayout.Width(18));
                        EditorGUILayout.LabelField(p.DisplayName, EditorStyles.boldLabel);
                    }

                    EditorGUILayout.LabelField("Package Name:", p.PackageName);
                    EditorGUILayout.LabelField("Git URL:", p.GitUrl);
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(8);

            using (new EditorGUI.DisabledScope(_isInstalling))
            {
                if (GUILayout.Button("Install Selected Packages", GUILayout.Height(32)))
                {
                    StartInstall();
                }
            }

            using (new EditorGUI.DisabledScope(!_isInstalling))
            {
                if (GUILayout.Button("Cancel (Stops after current request)"))
                {
                    CancelInstall();
                }
            }
        }

        private void StartInstall()
        {
            _isInstalling = true;

            // 1) List installed packages first
            _listReq = Client.List(true);
            EditorApplication.update += PollList;
        }

        private void PollList()
        {
            if (_listReq == null || !_listReq.IsCompleted) return;

            EditorApplication.update -= PollList;

            if (_listReq.Status != StatusCode.Success)
            {
                Debug.LogError($"[FastSetup] Client.List failed: {_listReq.Error.message}");
                Finish();
                return;
            }

            _installedNames = new HashSet<string>(_listReq.Result.Select(p => p.name));

            // 2) Build install queue in dependency order
            var toInstall = _packages
                .Where(p => p.Enabled)
                .ToList();

            // Skip already installed by package name
            toInstall.RemoveAll(p => _installedNames.Contains(p.PackageName));

            if (toInstall.Count == 0)
            {
                Debug.Log("[FastSetup] All selected packages already installed.");
                Finish();
                return;
            }

            _queue = new Queue<PackageSpec>(toInstall);
            InstallNext();
        }

        private void InstallNext()
        {
            if (_queue == null || _queue.Count == 0)
            {
                Debug.Log("[FastSetup] Install done.");
                AssetDatabase.Refresh();
                Finish();
                return;
            }

            var p = _queue.Dequeue();
            var id = p.BuildGitId(_gitRef);

            Debug.Log($"[FastSetup] Installing {p.DisplayName} -> {id}");
            _addReq = Client.Add(id);
            EditorApplication.update += PollAdd;
        }

        private void PollAdd()
        {
            if (_addReq == null || !_addReq.IsCompleted) return;

            EditorApplication.update -= PollAdd;

            if (_addReq.Status == StatusCode.Success)
            {
                Debug.Log($"[FastSetup] Installed: {_addReq.Result.name} ({_addReq.Result.version})");
            }
            else
            {
                Debug.LogError($"[FastSetup] Install failed: {_addReq.Error.message}");
                // vẫn tiếp tục cài các gói khác để bạn thấy tổng thể, hoặc bạn có thể stop tại đây
            }

            InstallNext();
        }

        private void CancelInstall()
        {
            Debug.LogWarning("[FastSetup] Cancel requested. Will stop after current request completes.");
            _queue?.Clear();
        }

        private void Finish()
        {
            _isInstalling = false;
            _queue = null;
            _listReq = null;
            _addReq = null;
        }
    }
}
#endif