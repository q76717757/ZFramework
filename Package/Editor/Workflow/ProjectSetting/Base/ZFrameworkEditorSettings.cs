using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ZFramework.Editor
{
    public class ZFrameworkEditorSettings : ScriptableObject
    {
        private static string DirPath = "ProjectSettings";
        private static string FilePath = DirPath + "/ZFrameworkEditorSettings.asset";
        private static ZFrameworkEditorSettings _instance;
        public static ZFrameworkEditorSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    var objs = InternalEditorUtility.LoadSerializedFileAndForget(FilePath);
                    if (objs.Length > 0 && objs[0] is ZFrameworkEditorSettings obj)
                    {
                        _instance = obj;
                    }
                    else
                    {
                        _instance = CreateInstance<ZFrameworkEditorSettings>();
                        Save();
                    }
                }
                return _instance;
            }
        }
        public static void Save()
        {
            Directory.CreateDirectory(DirPath);
            InternalEditorUtility.SaveToSerializedFileAndForget(new UnityEngine.Object[] { Instance }, FilePath, true);
        }





        //AB包的输出路径
        public static string AssetBundleOutputDir => $"{Application.dataPath}/AssetBundle/{Defines.TargetRuntimePlatform}";





        //打包配置
        [Header("随包场景(引导场景)")]
        public SceneAsset[] buildInScenes;//随包构建的场景

        //AB包配置
        [Header("热更场景")]
        public SceneAsset[] hotUpdateScenes;//热更场景


    }

}
