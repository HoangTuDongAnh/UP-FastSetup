#if UNITY_EDITOR
using System;

namespace HTDA.Framework.FastSetup.Editor
{
    [Serializable]
    public sealed class TemplateMeta
    {
        public string id = "";              // stable id (e.g. "general", "puzzle")
        public string name = "";            // display name
        public string description = "";     // shown as tooltip
        public string[] tags = Array.Empty<string>();
        public string version = "1.0.0";
    }

    [Serializable]
    public sealed class SetupTemplateContainer
    {
        public TemplateMeta meta = new TemplateMeta();

        // The actual config to write to ProjectSettings config file
        public SetupConfig config = new SetupConfig();
    }
}
#endif