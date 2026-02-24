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
    /// <summary>
    /// Resumable framework installer:
    /// - Persists queue + index in EditorPrefs
    /// - Continues after domain reload/recompile
    /// </summary>
    public static class FrameworkInstallerResumable
    {
        // ====== EDIT THESE ======
        // Use .git URLs. Use "master" if your repos default branch is master.
        private static readonly PackageSpec[] Packages =
        {
            new PackageSpec("UP-Core",      "com.htda.framework.core",      "https://github.com/HoangTuDongAnh/UP-Core.git"),
            new PackageSpec("UP-Events",    "com.htda.framework.events",    "https://github.com/HoangTuDongAnh/UP-Events.git"),
            new PackageSpec("UP-Pooling",   "com.htda.framework.pooling",   "https://github.com/HoangTuDongAnh/UP-Pooling.git"),
            new PackageSpec("UP-FSM",       "com.htda.framework.fsm",       "https://github.com/HoangTuDongAnh/UP-FSM.git"),
            new PackageSpec("UP-SceneFlow", "com.htda.framework.sceneflow", "https://github.com/HoangTuDongAnh/UP-SceneFlow.git"),
            new PackageSpec("UP-Settings",  "com.htda.framework.settings",  "https://github.com/HoangTuDongAnh/UP-Settings.git", optional: true),
        };

        // ====== STORAGE KEYS ======
        private const string Key_IsRunning = "HTDA.FastSetup.Install.IsRunning";
        private const string Key_GitRef = "HTDA.FastSetup.Install.GitRef";
        private const string Key_Index = "HTDA.FastSetup.Install.Index";
        private const string Key_Queue = "HTDA.FastSetup.Install.Queue"; // serialized as lines

        private static ListRequest _listReq;
        private static AddRequest _addReq;

        private static Queue<PackageSpec> _queue;
        private static int _currentIndex;
        private static string _gitRef;
        private static HashSet<string> _installedNames;

        // Auto resume after reload
        [InitializeOnLoadMethod]
        private static void AutoResumeIfNeeded()
        {
            if (!EditorPrefs.GetBool(Key_IsRunning, false))
                return;

            // Delay until editor is stable
            EditorApplication.delayCall += Resume;
        }

        [MenuItem("HTDA/FastSetup/Install Framework (Resumable)")]
        public static void InstallMenu()
        {
            // Default: master for your repos (you can change)
            InstallAll(gitRef: "master", includeOptional: false);
        }

        [MenuItem("HTDA/FastSetup/Install Framework + Optional (Resumable)")]
        public static void InstallMenuWithOptional()
        {
            InstallAll(gitRef: "master", includeOptional: true);
        }

        public static void InstallAll(string gitRef, bool includeOptional)
        {
            if (EditorPrefs.GetBool(Key_IsRunning, false))
            {
                Debug.LogWarning("[FastSetup] Installer is already running. Trying to resume...");
                Resume();
                return;
            }

            _gitRef = (gitRef ?? "").Trim();
            if (string.IsNullOrEmpty(_gitRef))
                _gitRef = "master";

            var selected = Packages.Where(p => includeOptional || !p.Optional).ToArray();

            SaveQueue(selected, _gitRef);

            Debug.Log($"[FastSetup] Starting install ({selected.Length} packages) with ref: {_gitRef}");
            EditorPrefs.SetBool(Key_IsRunning, true);

            // Start by listing installed packages
            _listReq = Client.List(true);
            EditorApplication.update += PollList;
        }

        private static void Resume()
        {
            if (!TryLoadQueue(out var specs, out _gitRef, out _currentIndex))
            {
                Debug.LogWarning("[FastSetup] Nothing to resume.");
                EditorPrefs.SetBool(Key_IsRunning, false);
                return;
            }

            Debug.Log($"[FastSetup] Resuming install at index {_currentIndex} (ref: {_gitRef})");

            _queue = new Queue<PackageSpec>(specs.Skip(_currentIndex));

            _listReq = Client.List(true);
            EditorApplication.update += PollList;
        }

        private static void PollList()
        {
            if (_listReq == null || !_listReq.IsCompleted) return;

            EditorApplication.update -= PollList;

            if (_listReq.Status != StatusCode.Success)
            {
                Debug.LogError($"[FastSetup] Client.List failed: {_listReq.Error.message}");
                StopAndClear();
                return;
            }

            _installedNames = new HashSet<string>(_listReq.Result.Select(p => p.name));

            // Skip already installed packages (but still advance index correctly)
            InstallNextSkippingInstalled();
        }

        private static void InstallNextSkippingInstalled()
        {
            while (_queue.Count > 0)
            {
                var next = _queue.Peek();
                if (_installedNames.Contains(next.PackageName))
                {
                    Debug.Log($"[FastSetup] Already installed: {next.DisplayName} ({next.PackageName}) - skipping");
                    _queue.Dequeue();
                    _currentIndex++;
                    EditorPrefs.SetInt(Key_Index, _currentIndex);
                    continue;
                }

                break;
            }

            if (_queue.Count == 0)
            {
                Debug.Log("[FastSetup] Install done.");
                StopAndClear(keepQueue: false);
                AssetDatabase.Refresh();
                return;
            }

            InstallNext();
        }

        private static void InstallNext()
        {
            if (_queue.Count == 0)
            {
                Debug.Log("[FastSetup] Install done.");
                StopAndClear(keepQueue: false);
                AssetDatabase.Refresh();
                return;
            }

            var p = _queue.Peek();
            var id = BuildGitId(p.GitUrl, _gitRef);

            Debug.Log($"[FastSetup] Installing {_currentIndex + 1}: {p.DisplayName} -> {id}");

            _addReq = Client.Add(id);
            EditorApplication.update += PollAdd;
        }

        private static void PollAdd()
        {
            if (_addReq == null || !_addReq.IsCompleted) return;

            EditorApplication.update -= PollAdd;

            if (_addReq.Status == StatusCode.Success)
            {
                Debug.Log($"[FastSetup] Installed: {_addReq.Result.name} ({_addReq.Result.version})");
            }
            else
            {
                Debug.LogError($"[FastSetup] Install failed: {_addReq.Error.message}\n" +
                               $"You can fix the issue and it will resume automatically next reload, or run the menu again.");
                // Do NOT clear queue: allow resume after user fixes and recompile
                // We still advance index? -> No. Keep same package to retry.
                // Just keep running state true and return.
                return;
            }

            // Success: advance index and persist
            _queue.Dequeue();
            _currentIndex++;
            EditorPrefs.SetInt(Key_Index, _currentIndex);

            // Trigger list refresh occasionally? Not required; but safe to refresh installed set
            // to ensure skip logic works even if dependencies auto installed.
            _listReq = Client.List(true);
            EditorApplication.update += PollList;
        }

        [MenuItem("HTDA/FastSetup/Cancel Framework Install")]
        public static void Cancel()
        {
            Debug.LogWarning("[FastSetup] Cancel install requested.");
            StopAndClear(keepQueue: true);
        }

        private static void StopAndClear(bool keepQueue = true)
        {
            EditorPrefs.SetBool(Key_IsRunning, false);

            _queue = null;
            _installedNames = null;
            _addReq = null;
            _listReq = null;

            if (!keepQueue)
            {
                EditorPrefs.DeleteKey(Key_Queue);
                EditorPrefs.DeleteKey(Key_Index);
                EditorPrefs.DeleteKey(Key_GitRef);
            }
        }

        private static string BuildGitId(string url, string gitRef)
        {
            url = (url ?? "").Trim();
            if (!url.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                url += ".git";

            gitRef = (gitRef ?? "").Trim();
            if (string.IsNullOrEmpty(gitRef))
                return url;

            return $"{url}#{gitRef}";
        }

        private static void SaveQueue(PackageSpec[] specs, string gitRef)
        {
            // Serialize as lines: DisplayName|PackageName|GitUrl|Optional
            var lines = specs.Select(s => $"{s.DisplayName}|{s.PackageName}|{s.GitUrl}|{(s.Optional ? "1" : "0")}");
            EditorPrefs.SetString(Key_Queue, string.Join("\n", lines));
            EditorPrefs.SetString(Key_GitRef, gitRef);
            EditorPrefs.SetInt(Key_Index, 0);
        }

        private static bool TryLoadQueue(out PackageSpec[] specs, out string gitRef, out int index)
        {
            specs = Array.Empty<PackageSpec>();
            gitRef = EditorPrefs.GetString(Key_GitRef, "master");
            index = EditorPrefs.GetInt(Key_Index, 0);

            var raw = EditorPrefs.GetString(Key_Queue, "");
            if (string.IsNullOrEmpty(raw)) return false;

            var lines = raw.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<PackageSpec>(lines.Length);

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length < 4) continue;

                var display = parts[0];
                var name = parts[1];
                var url = parts[2];
                var opt = parts[3] == "1";

                list.Add(new PackageSpec(display, name, url, opt));
            }

            if (list.Count == 0) return false;
            specs = list.ToArray();

            if (index < 0) index = 0;
            if (index > specs.Length) index = specs.Length;

            return true;
        }

        private readonly struct PackageSpec
        {
            public readonly string DisplayName;
            public readonly string PackageName;
            public readonly string GitUrl;
            public readonly bool Optional;

            public PackageSpec(string displayName, string packageName, string gitUrl, bool optional = false)
            {
                DisplayName = displayName;
                PackageName = packageName;
                GitUrl = gitUrl;
                Optional = optional;
            }
        }
    }
}
#endif