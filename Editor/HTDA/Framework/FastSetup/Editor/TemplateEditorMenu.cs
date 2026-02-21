#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HTDA.Framework.FastSetup.Editor
{
    public static class TemplateEditorMenu
    {
        [MenuItem("HTDA/Framework/Template/About", priority = 1)]
        public static void About()
        {
            Debug.Log("HTDA.Framework.FastSetup (editor) is initialized. Replace this with your module tools.");
        }
    }
}
#endif