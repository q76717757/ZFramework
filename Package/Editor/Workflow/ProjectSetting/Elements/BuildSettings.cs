using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if ENABLE_HYBRIDCLR
using HybridCLR.Editor.Installer;
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
#if ENABLE_HYBRIDCLR
            if (CheckUnityBuildSettings())
            {
                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

                EditorGUILayout.LabelField("步骤一:打包前的准备工作", EditorStyles.boldLabel);
                if (GUILayout.Button("GenerateAll"))//这里编译AOT时按BuildSetting的设置来的
                {
                    PrebuildCommand.GenerateAll();
                }
                EditorGUILayout.HelpBox("1.编译JIT;2.生成IL2CPP宏;3.生成Link.xml;4.仅脚本构建编译AOT;5.生成桥接函数;6.生成反向回调;7.生成AOT启发文档", MessageType.Info);


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("步骤二:补全AOT引用", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("自动生成的启发文档");
                var aotGeneric = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/HybridCLRGenerate/AOTGenericReferences.cs");
                EditorGUILayout.ObjectField(aotGeneric, typeof(MonoScript), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("手动实现的AOT引用");
                var aorRef = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/ZFramework/Boot/Base/AOTReferences.cs");
                EditorGUILayout.ObjectField(aorRef, typeof(MonoScript), false);
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.HelpBox("补全AOT引用(根据注释文档的提示手动添加AOT引用,再重新编译AOT可以提升性能)", MessageType.Warning);
                if (GUILayout.Button("重新编译AOT,并将裁剪AOT复制到StreamingAssets/AOT"))
                {
                    //只要是新打包  AOT都要重新构建一下
                    //待补充  ***  将配置中的场景设置到buildsettings上  编译完aot再还原
                    StripAOTDllCommand.GenerateStripedAOTDlls(target, targetGroup);
                    CopyAOTAssembliesToStreamingAssets();
                }
                if (GUILayout.Button("构建随包资源(AOT,loading场景,入口场景等)"))
                {

                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("步骤三:正式打包", EditorStyles.boldLabel);

                if (GUILayout.Button("打正式包"))
                {
                    string name = PlayerSettings.productName;

                    SceneAsset[] values = ZFrameworkEditorSettings.instance.buildInScenes;
                    var scenes = values.Where(value => value != null).Select(value => AssetDatabase.GetAssetPath(value)).ToArray();

                    string outputPath = $"{Directory.GetParent(Application.dataPath)}/Build/{target}_{DateTime.Now:MMdd}";
                    BuildBootScene(name, scenes, outputPath);
                }

                EditorGUILayout.Space();

                if (GUILayout.Button("热更新:编译JIT程序集+复制到工程内+构建JIT.AB包"))
                {
                    ComplitJIT();
                }
                if (GUILayout.Button("热更新:构建增量资产AB包"))
                {
                }
            }

#endif



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

#if ENABLE_HYBRIDCLR
        void CopyAOTAssembliesToStreamingAssets()
        {

            //复制aot到工程内
            var target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = $"Assets/AssetBundle/{Defines.TargetRuntimePlatform}/MetaAssembly";
            Directory.CreateDirectory(aotAssembliesDstDir);
            List<string> assemblyNames = new List<string>();
            string[] names = BootConfig.Instance.AotMetaAssemblyNames;
            foreach (var dll in names)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError("AOT DLL:" + srcDllPath + "不存在");
                    continue;
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.dll.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                assemblyNames.Add(dllBytesPath);
            }
            AssetDatabase.Refresh();

            //打ab包
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = "metaassembly.ab";
            build.assetNames = assemblyNames.ToArray();
            AssetBundleManifest aotabm = BuildPipeline.BuildAssetBundles(aotAssembliesDstDir, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, target);
            AssetDatabase.Refresh();

            for (int i = 0; i < assemblyNames.Count; i++)
            {
                if (File.Exists(assemblyNames[i]))
                {
                    File.Delete(assemblyNames[i]);
                }
            }
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(AssetImporter.GetAtPath(aotAssembliesDstDir).GetInstanceID());
            Log.Info("AOT HASH ->" + aotabm.GetAssetBundleHash("metaassembly.ab"));
            //aot的文件hash   把这个写进bootconfig中,在jit加载之前  先检查aot是不是正确的
        }
        void ComplitJIT()
        {
            //编译jit
            var target = EditorUserBuildSettings.activeBuildTarget;
            CompileDllCommand.CompileDll(target);
            var buildDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);

            //复制到工程内
            var jitBytesDir = $"Assets/AssetBundle/{Defines.TargetRuntimePlatform}/Assembly";
            Directory.CreateDirectory(jitBytesDir);

            var assetNames = new List<string>();
            var jitNames = BootConfig.Instance.AssemblyNames;
            for (int i = 0; i < jitNames.Length; i++)
            {
                var jitName = jitNames[i] + ".dll";
                var jitPath = Path.Combine(buildDir, jitName);
                if (File.Exists(jitPath))
                {
                    File.Copy(jitPath, $"{jitBytesDir}/{jitName}.bytes", true);
                    assetNames.Add($"{jitBytesDir}/{jitName}.bytes");
                }
                else
                {
                    //配置中设置了程序集  但实际编译出来缺不存在  检查热更程序集的设置对不对
                    Log.Error(jitPath + ":Missing");
                }
            }
            AssetDatabase.Refresh();

            //构建ab包
            string jitabPath = $"Assets/AssetBundle/{Defines.TargetRuntimePlatform}/Assembly";
            string jitabName = "assembly.ab";
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = jitabName;
            build.assetNames = assetNames.ToArray();
            Directory.CreateDirectory(jitabPath);
            BuildPipeline.BuildAssetBundles(jitabPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, target);
            AssetDatabase.Refresh();

            for (int i = 0; i < jitNames.Length; i++)
            {
                var jitName = jitNames[i] + ".dll";
                var jitPath = Path.Combine(buildDir, jitName);
                if (File.Exists(jitPath))
                {
                    File.Delete($"{jitBytesDir}/{jitName}.bytes");
                }
            }
            AssetDatabase.Refresh();

            EditorGUIUtility.PingObject(AssetImporter.GetAtPath($"{jitabPath}/{jitabName}").GetInstanceID());
        }
#endif

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
