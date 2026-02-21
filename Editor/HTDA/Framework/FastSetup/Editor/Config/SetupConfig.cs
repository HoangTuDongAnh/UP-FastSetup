#if UNITY_EDITOR
using System;

namespace HTDA.Framework.FastSetup.Editor
{
    [Serializable]
    public sealed class SetupConfig
    {
        public string projectName = "MyProject";

        // (Legacy) Flat list: path relative ProjectRoot or Assets/...
        public string[] folders = Array.Empty<string>();

        // (New) Groups: root + list of relative child paths
        public FolderGroup[] folderGroups = Array.Empty<FolderGroup>();

        public ModuleConfig[] modules = Array.Empty<ModuleConfig>();

        public BootstrapConfig bootstrap = new BootstrapConfig();
    }

    [Serializable]
    public sealed class FolderGroup
    {
        // e.g. "Assets/__{ProjectName}" or "Assets/_MyCore"
        public string root = "Assets/__{ProjectName}";

        // e.g. "Art/Texture", "Code/Scripts/_Common/Util"
        public string[] paths = Array.Empty<string>();
    }

    [Serializable]
    public sealed class ModuleConfig
    {
        public string name = "HTDA Framework Core";
        public string url = "https://github.com/<YOUR_ORG>/UP-Core.git";
        public string path = "Packages/com.htda.framework.core";
    }

    [Serializable]
    public sealed class BootstrapConfig
    {
        public bool enabled = true;

        public string sceneName = "Bootstrap";
        public string savePath = "Assets/__{ProjectName}/Design/Scene";

        public string[] systemPrefabs = Array.Empty<string>();

        public bool addToBuildSettings = true;
    }
}
#endif