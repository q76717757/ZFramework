using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    public class GlobalConfigLoader
    {
        [InitializeOnLoad]
        public static class CheckGlobalConfig
        {
            static CheckGlobalConfig()
            {
                var config = Resources.Load<GlobalConfig>("GlobalConfig");
                
                if (config == null)
                {
                    //AssetDatabase.CreateAsset(new GlobalConfig(), "Assets/ZFramework/Resources/GlobalConfig.asset");
                    //GlobalConfigEditorWindow.Open();
                    Debug.Log("global config is null");
                }
            }
        }
    }
}
