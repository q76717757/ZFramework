using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using HybridCLR.Editor.Installer;
using UnityEditor.Build.Reporting;
using UnityEditor.WindowsStandalone;

#if ENABLE_HYBRIDCLR
using HybridCLR.Editor;
using HybridCLR.Editor.Commands;
#endif

namespace ZFramework.Editor
{
    public class BuildSettings : ZFrameworkSettingsProviderElement<BuildSettings>
    {
        [SettingsProvider] public static SettingsProvider Register() => GetInstance();

        public override void OnGUI()
        {

            SerializedProperty buildInScenes = EdtiorSettings.FindProperty("buildInScenes");
            EditorGUILayout.HelpBox("热更模式下,仅打包引导场景,即挂载BootStrap的场景\r\n非热更模式下,则需要把所有需要的场景都添加进来", MessageType.Info);

            int len = buildInScenes.arraySize;
            EditorGUI.BeginChangeCheck();//通过拖拽到标题的方式添加数组元素 检查不到变化 记录数组数量修正这个BUG
            EditorGUILayout.PropertyField(buildInScenes);
            if (EditorGUI.EndChangeCheck() || len != buildInScenes.arraySize)
            {
                ZFrameworkEditorSettings.Save();
            }

            if (CheckUnityBuildSettings())
            {
                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

                EditorGUILayout.LabelField("步骤一:打包前的准备工作", EditorStyles.boldLabel);
                if (GUILayout.Button("GenerateAll"))
                {
                    PrebuildCommand.GenerateAll();//这里编译AOT时按BuildSetting的设置来的
                }
                EditorGUILayout.HelpBox("1.编译JIT;2.生成IL2CPP宏;3.生成Link.xml;4.仅脚本构建编译AOT;5.生成桥接函数;6.生成反向回调;7.生成AOT启发文档", MessageType.Info);


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("步骤二:补全AOT引用(可选,提升性能)", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("自动生成的启发文档");
                var aotGeneric = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/HybridCLRData/Generated/AOTGenericReferences.cs");
                EditorGUILayout.ObjectField(aotGeneric, typeof(MonoScript), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("手动实现的AOT引用");
                var aorRef = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/ZFramework/Boot/Base/AOTReferences.cs");
                EditorGUILayout.ObjectField(aorRef, typeof(MonoScript), false);
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.HelpBox("补全AOT引用(根据注释文档的提示手动添加AOT引用,再重新编译AOT可以提升性能)", MessageType.Warning);
                if (GUILayout.Button("完成AOT引用添加,重新编译AOT"))
                {
                    StripAOTDllCommand.GenerateStripedAOTDlls(target, targetGroup);
                    CopyAOTAssembliesToStreamingAssets(); 
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("步骤三:正式打包", EditorStyles.boldLabel);
                
                if (GUILayout.Button("打正式包"))
                {
                    string name = PlayerSettings.productName;

                    SceneAsset[] values = ZFrameworkEditorSettings.Instance.buildInScenes;
                    var scenes = values.Where(value => value != null).Select(value => AssetDatabase.GetAssetPath(value)).ToArray();

                    string outputPath = $"{Directory.GetParent(Application.dataPath)}/Build/{target}_{DateTime.Now:MMdd}";
                    BuildBootScene(name,scenes, outputPath);
                }
            }


            


        }

        bool CheckUnityBuildSettings()
        {
            var buildInScenes = EdtiorSettings.FindProperty("buildInScenes");
            List<SceneAsset> values = new List<SceneAsset>();
            for (int i = 0; i < buildInScenes.arraySize; i++)
            {
                var scene = (SceneAsset)buildInScenes.GetArrayElementAtIndex(i).objectReferenceValue;
                if (scene != null)
                {
                    values.Add(scene);
                }
            }
            var runtimeValue = values.Select(value => AssetDatabase.GetAssetPath(value)).ToArray();
            var editorValue = EditorBuildSettings.scenes.Where(scene => (scene != null && scene.enabled)).Select(a => a.path).ToArray();

            if (!ComparisonArray(runtimeValue, editorValue))
            {
                EditorGUILayout.HelpBox("打包场景的设置和Build Setting不一致", MessageType.Error);
                if (GUILayout.Button("同步设置到Build Settings"))
                {
                    var temp = new EditorBuildSettingsScene[runtimeValue.Length];
                    for (int i = 0; i < runtimeValue.Length; i++)
                    {
                        temp[i] = new EditorBuildSettingsScene(runtimeValue[i], true);
                    }
                    EditorBuildSettings.scenes = temp;
                }
                return false;
            }
            return true;
        }
        void CopyAOTAssembliesToStreamingAssets()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = Defines.AOTMetaAssemblyLoadDir;
            Directory.CreateDirectory(aotAssembliesDstDir);
            foreach (var dll in ZFrameworkRuntimeSettings.Get().AotMetaAssemblyNames)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError("AOT DLL:" + srcDllPath + "不存在");
                    continue;
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.dll.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
            }
            AssetDatabase.Refresh();
        }

        void BuildBootScene(string name, string[] scenePaths, string outputPath)
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions()
            {
                scenes = scenePaths,
                locationPathName = $"{outputPath}/{name}",
                options = BuildOptions.CompressWithLz4,
                target = EditorUserBuildSettings.activeBuildTarget,
                targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup,  
            };


            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Application.OpenURL(outputPath);
            }
            Log.Info("构建结果" + report.summary.result);
        }



        bool ComparisonArray(IEnumerable<string> item1, IEnumerable<string> item2)
        {
            if (item1 == item2) return true;
            if (item1 == null || item2 == null) return false;
            if (item1.Count() != item2.Count()) return false;

            var enum1 = item1.GetEnumerator();
            var enum2 = item2.GetEnumerator();
            while (true)
            {
                if (enum1.MoveNext() && enum2.MoveNext())
                {
                    if (enum1.Current != enum2.Current)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }

            }
        }
    }
}
