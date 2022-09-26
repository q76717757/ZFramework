using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ZFramework
{
    [CustomEditor(typeof(BootFile))]
    public class BootFileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Draw(serializedObject);
        }

        public static void Draw(SerializedObject serializedObject)
        {
            EditorGUILayout.LabelField("PlayerSetting");
            EditorGUI.BeginDisabledGroup(true);//HyCLR要求 IL2CPP后端 关闭增量GC
            EditorGUILayout.LabelField("编译后端");
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            ScriptingImplementation banckendType = PlayerSettings.GetScriptingBackend(buildTargetGroup);
            EditorGUILayout.EnumPopup(banckendType);
            EditorGUILayout.LabelField("C++编译器配置");
            var com = PlayerSettings.GetIl2CppCompilerConfiguration(buildTargetGroup);
            EditorGUILayout.EnumPopup(com);
            EditorGUILayout.LabelField("增量GC");
            EditorGUILayout.Toggle(PlayerSettings.gcIncremental);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space(10);

            if (serializedObject == null) return;

            GUILayout.Label("根据项目实际情况配置选项");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("目标平台:");
            var platform = serializedObject.FindProperty("platform");
            platform.enumValueIndex = (int)(Platform)EditorGUILayout.EnumPopup((Platform)platform.enumValueIndex);
            var selectPlatform = (Platform)platform.enumValueIndex;
            EditorGUILayout.EndHorizontal();

            switch (selectPlatform)
            {
                case Platform.Win:
                case Platform.Mac:
                case Platform.Andorid:
                case Platform.IOS:
                    EditorGUILayout.HelpBox("支持热更新的平台", MessageType.None);
                    break;
                case Platform.WebGL:
                case Platform.UWP:
                    EditorGUILayout.HelpBox("不支持热更新的平台", MessageType.Warning);
                    break;
            }



            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("更新方式");
            var network = serializedObject.FindProperty("network");
            network.enumValueIndex = (int)(Network)EditorGUILayout.EnumPopup((Network)network.enumValueIndex);
            var selectNetwork = (Network)network.enumValueIndex;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("仅PC平台有本地模式,直接通过本地读补丁,一些内网PC项目", MessageType.None);//可以在内网部署服务器 那么内网也可以实现 有些小项目时没有服务端的 就需要本地更新

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("允许离线"); //支持离线 = 允许旧版本运行   不支持离线 = 必须保持最新
            EditorGUILayout.EndHorizontal();

            //支持更新                   不支持更新
            //本地更新 = 无网络           远程更新 = 有网络
            if (serializedObject.ApplyModifiedProperties())
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            }
        }


        protected override bool ShouldHideOpenButton() => true;
    }
}
