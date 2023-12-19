using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class AssetBundleSettings : ZFrameworkSettingsProviderElement<AssetBundleSettings>
    {
        [SettingsProvider] public static SettingsProvider Register() => GetInstance();


        public override void OnGUI()
        {
            SerializedProperty hotUpdateScenes = EdtiorSettings.FindProperty("hotUpdateScenes");
            EditorGUILayout.HelpBox("热更模式下,仅打包引导场景,即挂载BootStrap的场景\r\n非热更模式下,则需要把所有需要的场景都添加进来", MessageType.Info);

            int len = hotUpdateScenes.arraySize;
            EditorGUI.BeginChangeCheck();//通过拖拽到标题的方式添加数组元素 检查不到变化 记录数组数量修正这个BUG
            EditorGUILayout.PropertyField(hotUpdateScenes);

            if (EditorGUI.EndChangeCheck() || len != hotUpdateScenes.arraySize)
            {
                EdtiorSettings.ApplyModifiedProperties();
                ZFrameworkEditorSettings.Save();

            }

            if (GUILayout.Button("临时输出AB包到StreamingAssets"))
            {
                BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
            }
        }

    }
}
