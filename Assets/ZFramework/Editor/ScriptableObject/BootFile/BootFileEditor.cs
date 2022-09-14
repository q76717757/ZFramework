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
            var boot = (target as BootFile);

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

            //目标平台  win android ios  webgl uwp
            //         [     热更     ] [ 不热更 ]
            var targetPlatform = (Platform)serializedObject.FindProperty("platform").enumValueIndex;
            serializedObject.FindProperty("platform").enumValueIndex = (int)(Platform)EditorGUILayout.EnumPopup(targetPlatform);

            serializedObject.FindProperty("useHotfix").intValue = EditorGUILayout.Popup(serializedObject.FindProperty("useHotfix").intValue, new string[] { "不应用", "应用" });

            if (serializedObject.FindProperty("useHotfix").intValue == 1)
            {
                serializedObject.FindProperty("hotfixType").intValue = EditorGUILayout.Popup(serializedObject.FindProperty("hotfixType").intValue, new string[] { "本地", "远程" });
            }
            else
            {

            }

            //临时用
            if (true)//前瞻子项目 win平台 mono后端  not HyCLY
            {

            }

            //core层在unity工程内  在win平台下  可以和il2cpp一样的流程  打包剥离core程序集  (需要测试)?
            //但是在其他平台要剥离core  需要HyCLR的支持
            //否则Core将跟随工程 并且不能更新

            if (true)//支持热更的平台
            {
                if (true)//需要热更新
                {
                    if (true)//已经完成HyCLR库配置
                    {
                        if (true)//有远程环境
                        {

                        }
                        else//没有远程环境 本地更新
                        {

                        }
                    }
                    else//为配置热更环境
                    {

                    }
                }
                else//不需要热更新
                {

                }
            }
            else//不支持的平台
            {

            }


            if (serializedObject.ApplyModifiedProperties())
            {
                AssetDatabase.SaveAssetIfDirty(serializedObject.targetObject);
            }
        }


        protected override bool ShouldHideOpenButton() => true;

    }
}
