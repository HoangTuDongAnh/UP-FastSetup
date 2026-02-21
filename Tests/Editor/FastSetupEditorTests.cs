#if UNITY_EDITOR
using NUnit.Framework;

namespace HTDA.Framework.FastSetup.Editor.Tests
{
    public class FastSetupEditorTests
    {
        [Test]
        public void ConfigPath_IsNotEmpty()
        {
            Assert.IsFalse(string.IsNullOrEmpty(FastSetupPaths.ConfigFullPath));
        }
    }
}
#endif