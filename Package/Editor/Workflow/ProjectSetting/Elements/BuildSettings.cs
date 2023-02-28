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
            EditorGUILayout.HelpBox("�ȸ�ģʽ��,�������������,������BootStrap�ĳ���\r\n���ȸ�ģʽ��,����Ҫ��������Ҫ�ĳ�������ӽ���", MessageType.Info);

            int len = buildInScenes.arraySize;
            EditorGUI.BeginChangeCheck();//ͨ����ק������ķ�ʽ�������Ԫ�� ��鲻���仯 ��¼���������������BUG
            EditorGUILayout.PropertyField(buildInScenes);
            if (EditorGUI.EndChangeCheck() || len != buildInScenes.arraySize)
            {
                ZFrameworkEditorSettings.Save();
            }

            if (CheckUnityBuildSettings())
            {
                BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

                EditorGUILayout.LabelField("����һ:���ǰ��׼������", EditorStyles.boldLabel);
                if (GUILayout.Button("GenerateAll"))
                {
                    PrebuildCommand.GenerateAll();//�������AOTʱ��BuildSetting����������
                }
                EditorGUILayout.HelpBox("1.����JIT;2.����IL2CPP��;3.����Link.xml;4.���ű���������AOT;5.�����ŽӺ���;6.���ɷ���ص�;7.����AOT�����ĵ�", MessageType.Info);


                EditorGUILayout.Space();
                EditorGUILayout.LabelField("�����:��ȫAOT����(��ѡ,��������)", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("�Զ����ɵ������ĵ�");
                var aotGeneric = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/HybridCLRData/Generated/AOTGenericReferences.cs");
                EditorGUILayout.ObjectField(aotGeneric, typeof(MonoScript), false);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("�ֶ�ʵ�ֵ�AOT����");
                var aorRef = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/ZFramework/Boot/Base/AOTReferences.cs");
                EditorGUILayout.ObjectField(aorRef, typeof(MonoScript), false);
                EditorGUILayout.EndHorizontal();
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.HelpBox("��ȫAOT����(����ע���ĵ�����ʾ�ֶ����AOT����,�����±���AOT������������)", MessageType.Warning);
                if (GUILayout.Button("���AOT�������,���±���AOT"))
                {
                    StripAOTDllCommand.GenerateStripedAOTDlls(target, targetGroup);
                    CopyAOTAssembliesToStreamingAssets(); 
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("������:��ʽ���", EditorStyles.boldLabel);
                
                if (GUILayout.Button("����ʽ��"))
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
                EditorGUILayout.HelpBox("������������ú�Build Setting��һ��", MessageType.Error);
                if (GUILayout.Button("ͬ�����õ�Build Settings"))
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
                    Debug.LogError("AOT DLL:" + srcDllPath + "������");
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
            Log.Info("�������" + report.summary.result);
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
