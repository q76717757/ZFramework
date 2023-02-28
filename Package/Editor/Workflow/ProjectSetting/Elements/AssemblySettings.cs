using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;

#if ENABLE_HYBRIDCLR
using HybridCLR.Editor;
using HybridCLR.Editor.Installer;
#endif

namespace ZFramework.Editor
{
    public class AssemblySettings : ZFrameworkSettingsProviderElement<AssemblySettings>
    {
        [SettingsProvider] public static SettingsProvider Register() => GetInstance();
        GUILayoutOption titleWidth = GUILayout.Width(100);

        SerializedProperty hotUpdateType;
        SerializedProperty assemblyNames;
        SerializedProperty aotMetaAssemblyNames;

        public override void OnEnable()
        {
            hotUpdateType = RuntimeSettings.FindProperty("hotUpdateType");
            assemblyNames = RuntimeSettings.FindProperty("assemblyNames");
            aotMetaAssemblyNames = RuntimeSettings.FindProperty("aotMetaAssemblyNames");
        }
        public override void OnDisable()
        {
            hotUpdateType.Dispose();
            assemblyNames.Dispose();
            aotMetaAssemblyNames.Dispose();
        }

        public override void OnGUI()
        {
            EditorGUILayout.LabelField("代码热更新方案: HybridCLR", EditorStyles.boldLabel);
            if (GUILayout.Button("官方文档:https://focus-creative-games.github.io/hybridclr/"))
            {
                Application.OpenURL("https://focus-creative-games.github.io/hybridclr/");
            }
            EditorGUILayout.Space();

            HotUpdateType type = EnumPopup();
            switch (type)
            {
                case HotUpdateType.Online:
                    ShowModInfo("在线更新需要目标平台支持热更,需要有网络环境,需要部署热更服务,如版本查询,补丁下载等");
                    Remote();
                    break;
                case HotUpdateType.Offline:
                    ShowModInfo("离线更新仅支持Windows,不需要网络环境和热更服务,Windows操作文件方便,直接拷补丁进去");
                    Local();
                    break;
                case HotUpdateType.Not:
                    ShowModInfo("针对不具备热更新条件的,或者临时开发的项目不想走热更流程");
                    Not();
                    break;
            }

            if (RuntimeSettings.ApplyModifiedProperties())
            {
                AssetDatabase.SaveAssets();
            }
        }
        HotUpdateType EnumPopup()
        {
            string[] displayNames = new string[]
            {
                "在线更新",
                "离线更新",
                "不更新",
            };
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("热更新模式:", EditorStyles.boldLabel, titleWidth);
            hotUpdateType.enumValueIndex = EditorGUILayout.Popup(hotUpdateType.enumValueIndex, displayNames/* loadType.enumDisplayNames*/);
            EditorGUILayout.EndHorizontal();

            return (HotUpdateType)hotUpdateType.enumValueIndex;
        }
        void ShowModInfo(string info)
        {
            EditorGUILayout.HelpBox(info, MessageType.Info);
            EditorGUILayout.Space();
        }

        void Remote()
        {
            if (CheckHybridCLR())
            {
                CheckAssemblySetting();
            }
        }
        void Local()
        {
            if (CheckHybridCLR())
            {
                CheckAssemblySetting();
            }
        }
        void Not()
        {
            if (!Defines.HybridCLRPackageIsInstalled || IsDisable())
            {
                DrawAssembly(assemblyNames);
                if (GUILayout.Button("重置到默认"))
                {
                    ResetAssembly(assemblyNames, Defines.DefaultAssemblyNames);
                }
            }
            
        }



        bool CheckHybridCLR()
        {
            return CheckPackage() && IsFullInstall() && IsEnable() && CheckTargetPlatfrom() && CheckOtherSettings();//判断是有顺序的
        }
        bool CheckPackage()
        {
            if (Defines.HybridCLRPackageIsInstalled)
            {
                return true;
            }
            else
            {
                EditorGUILayout.LabelField("HybridCLR Package(未安装)",EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("可以通过Package Manager的Add package from git URL安装", MessageType.Error);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("URL:", GUILayout.Width(50));
                EditorGUILayout.TextField("git@gitee.com:focus-creative-games/hybridclr_unity.git");
                EditorGUILayout.EndHorizontal();

                return false;
            }
        }
        bool IsFullInstall()
        {
            var isFull = new InstallerController().HasInstalledHybridCLR();
            if (!isFull)//未完整安装
            {
                EditorGUILayout.HelpBox("HybridCLR分为3部分\r\n1.Package(已安装)\r\n2.HybrldCLR(未安装)\r\n3.Il2CPP Plus(未安装)", MessageType.Error);
                if (GUILayout.Button("打开HybridCLR Installer继续完成安装"))
                {
                    InstallerWindow window = EditorWindow.GetWindow<InstallerWindow>("HybridCLR Installer", true);
                    window.minSize = new Vector2(800f, 500f);
                }
            }
            return isFull;
        }
        bool IsEnable()
        {
            if (SettingsUtil.Enable)
            {
                return true;
            }
            else
            {
                EditorGUILayout.HelpBox("HybridCLR未开启,如选择热更模式则需要开启HybridCLR", MessageType.Error);
                if (GUILayout.Button("点击启用"))
                {
                    SettingsUtil.Enable = true;
                }
                return false;
            }
        }
        bool IsDisable()
        {
            if (!SettingsUtil.Enable)
            {
                return true;
            }
            else
            {
                EditorGUILayout.HelpBox("HybridCLR已开启,如选择不热更模式则需要关闭HybridCLR", MessageType.Error);
                if (GUILayout.Button("点击关闭"))
                {
                    SettingsUtil.Enable = false;
                }
                return false;
            }
            
        }
        bool CheckTargetPlatfrom()
        {
            var target = Defines.TargetRuntimePlatform;
            var select =  (HotUpdateType)hotUpdateType.enumValueIndex;
            switch (select)
            {
                case HotUpdateType.Online:
                    if (target == (target & PlatformType.OnlineSupported))
                    {
                        return true;
                    }
                    else
                    {
                        Show(target.ToString());
                        EditorGUILayout.HelpBox("目标平台不支持在线更新模式,默认只支持Windows/Android/iOS", MessageType.Error);
                        if (GUILayout.Button("关闭热更新功能"))
                        {
                            hotUpdateType.enumValueIndex = (int)HotUpdateType.Not;
                            SettingsUtil.Enable = false;
                        }
                    }
                    break;
                case HotUpdateType.Offline:
                    if (target == (target & PlatformType.OfflineSupported))
                    {
                        return true;
                    }
                    else
                    {
                        Show(target.ToString());
                        EditorGUILayout.HelpBox("目标平台不支持离线更新,默认只支持Windows", MessageType.Error);
                        if (GUILayout.Button("关闭热更新功能"))
                        {
                            hotUpdateType.enumValueIndex = (int)HotUpdateType.Not;
                            SettingsUtil.Enable = false;
                        }
                    }
                    break;
                case HotUpdateType.Not:
                    return true;
            }
            return false;

            void Show(string label)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目标平台:", EditorStyles.boldLabel, titleWidth);
                EditorGUILayout.LabelField(label);
                EditorGUILayout.EndHorizontal();
            }
        }
        bool CheckOtherSettings()//检查Hybrid要求的设置
        {
            CheckGC();
            CheckIL2CPP();
            //CheckAPILevel();
            return true;
        }
        bool CheckGC()
        {
            var gc = PlayerSettings.gcIncremental;
            if (gc)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("增量GC:", EditorStyles.boldLabel, titleWidth);
                EditorGUILayout.LabelField("已开启");
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("HybridCLR暂不支持增量GC,需要关闭增量GC功能", MessageType.Error);
                if (GUILayout.Button("关闭增量GC功能"))
                {
                    PlayerSettings.gcIncremental = false;
                }
            }
            return !gc;
        }
        bool CheckIL2CPP()
        {
            var il2cpp = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (il2cpp != ScriptingImplementation.IL2CPP)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("编译后端:", EditorStyles.boldLabel, titleWidth);
                EditorGUILayout.LabelField(il2cpp.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("HybridCLR是基于IL2CPP的,需要将编译后端改为IL2CPP", MessageType.Error);
                if (GUILayout.Button("切换到IL2CPP"))
                {
                    PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
                }
            }
            return il2cpp == ScriptingImplementation.IL2CPP;
        }
        //void CheckAPILevel()
        //{
        //    var apiLevel = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
        //    if (apiLevel != ApiCompatibilityLevel.NET_4_6)
        //    {
        //        EditorGUILayout.BeginHorizontal();
        //        EditorGUILayout.LabelField("目标框架:", EditorStyles.boldLabel, titleWidth);
        //        EditorGUILayout.LabelField(apiLevel.ToString());
        //        EditorGUILayout.EndHorizontal();

        //        EditorGUILayout.HelpBox("主工程可以选择Standard 2.0或者4.x,但编译热更程序集必须使用4.x,如无必要建议统一到4.x", MessageType.Warning);
        //    }
        //}

        void CheckAssemblySetting()
        {
            DrawAssembly(assemblyNames);
            CheckAssembly();
            DrawAssembly(aotMetaAssemblyNames);
            if (GUILayout.Button("重置到默认"))
            {
                ResetAssembly(assemblyNames, Defines.DefaultAssemblyNames);
                ResetAssembly(aotMetaAssemblyNames, Defines.DefaultAOTMetaAssemblyNames);
            }
        }
        void DrawAssembly(SerializedProperty assembly)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(assembly);
            if (EditorGUI.EndChangeCheck())
            {
                AssetDatabase.SaveAssets();
            }
        }
        void CheckAssembly()
        {
            string[] assemblyNameValues = new string[assemblyNames.arraySize];
            for (int i = 0; i < assemblyNameValues.Length; i++)
            {
                assemblyNameValues[i] = assemblyNames.GetArrayElementAtIndex(i).stringValue;
            }
            List<string> clr = new List<string>();
            if (HybridCLRSettings.Instance.hotUpdateAssemblies != null)
                clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblies);
            if (HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions != null)
                clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.Where((a) => a != null).Select((a) => a.name));


            if (!ComparisonArray(clr, assemblyNameValues))
            {
                EditorGUILayout.HelpBox("热更程序集列表与HybridCLR不一致", MessageType.Error);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("保存至HybridCLR"))
                {
                    HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = new UnityEditorInternal.AssemblyDefinitionAsset[0];
                    HybridCLRSettings.Instance.hotUpdateAssemblies = assemblyNameValues;
                    HybridCLRSettings.Save();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        void ResetAssembly(SerializedProperty assembly, string[] defVals)
        {
            assembly.ClearArray();
            for (int i = 0; i < defVals.Length; i++)
            {
                assembly.InsertArrayElementAtIndex(i);
                assembly.GetArrayElementAtIndex(i).stringValue = defVals[i];
            }
        }

        void AssemblyBytesLoadPath()
        {

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
