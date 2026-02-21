#if UNITY_EDITOR
using System.IO;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    internal static class JsonFile
    {
        public static bool TryRead<T>(string fullPath, out T data) where T : class
        {
            data = null;
            if (!File.Exists(fullPath)) return false;

            var json = File.ReadAllText(fullPath);
            data = JsonUtility.FromJson<T>(json);
            return data != null;
        }

        public static void Write<T>(string fullPath, T data) where T : class
        {
            var dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            File.WriteAllText(fullPath, JsonUtility.ToJson(data, true));
        }
    }
}
#endif