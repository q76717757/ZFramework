using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ZFramework
{
    public class BootFileLoader
    {
        [InitializeOnLoad]
        public static class CheckBootFile
        {
            static CheckBootFile()
            {
                var config = Resources.Load<BootFile>("BootFile");
                
                if (config == null)
                {
                    //AssetDatabase.CreateAsset(new GlobalConfig(), "Assets/ZFramework/Resources/GlobalConfig.asset");
                    //GlobalConfigEditorWindow.Open();
                    //Debug.Log("global config is null");
                }
            }
        }
    }
}
