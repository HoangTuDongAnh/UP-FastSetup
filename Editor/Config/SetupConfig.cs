using System;

namespace HoangTuDongAnh.FastSetup.Editor
{
    [Serializable]
    public class SetupConfig
    {
        public string projectName;
        
        public string rootNamespace;
        
        public string[] folders;
        
        public ModuleConfig[] modules;
        
        public BootstrapConfig bootstrap;
    }

    [Serializable]
    public class ModuleConfig
    {
        public string name; 
        public string url;  
        public string path; 
    }
    
    [Serializable]
    public class BootstrapConfig
    {
        public string sceneName;
        public string savePath;
        public string[] systemPrefabs; 
    }
}
