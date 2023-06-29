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
    [FilePath("ProjectSettings/ZFrameworkEditorSettings.asset", FilePathAttribute.Location.ProjectFolder)]
    public class ZFrameworkEditorSettings : ScriptableSingleton<ZFrameworkEditorSettings>
    {
        public static void Save()
        {
            instance.Save(true);
        }

        //AB包的输出路径
        public static string AssetBundleOutputDir => $"{Application.dataPath}/AssetBundle/{Defines.TargetRuntimePlatform}";

        //打包配置
        [Header("随包场景(引导场景)")]
        [SerializeField]
        public SceneAsset[] buildInScenes;//随包构建的场景

        //AB包配置
        [Header("热更场景")]
        [SerializeField]
        public SceneAsset[] hotUpdateScenes;//热更场景

    }

}
