#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    public static class FastSetupPaths
    {
        public const string ConfigRelativePath = "ProjectSettings/HTDA/FastSetup/setup-config.json";
        public const string ProjectTemplatesRelativeDir = "ProjectSettings/HTDA/FastSetup/Templates";

        public static string ProjectRoot
            => Directory.GetParent(Application.dataPath)!.FullName.Replace('\\', '/');

        public static string ConfigFullPath
            => Path.Combine(ProjectRoot, ConfigRelativePath).Replace('\\', '/');

        public static string ConfigDirectoryFullPath
            => Path.GetDirectoryName(ConfigFullPath)!.Replace('\\', '/');

        public static string ProjectTemplatesDirFullPath
            => Path.Combine(ProjectRoot, ProjectTemplatesRelativeDir).Replace('\\', '/');

        public static void EnsureConfigDirectory()
        {
            var dir = ConfigDirectoryFullPath;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        public static void EnsureProjectTemplatesDirectory()
        {
            var dir = ProjectTemplatesDirFullPath;
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
#endif