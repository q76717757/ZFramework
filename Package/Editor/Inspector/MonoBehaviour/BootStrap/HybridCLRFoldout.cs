using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;

#if ENABLE_HYBRIDCLR
using HybridCLR;
using HybridCLR.Editor;
using HybridCLR.Editor.Installer;
#endif

namespace ZFramework.Editor
{
    public class HybridCLRFoldout : FadeFoldout
    {
        public override string Title => "代码热更新";

        string packageName = "com.code-philosophy.hybridclr";
        string gitURL = "git@gitee.com:focus-creative-games/hybridclr_unity.git";

        protected override void OnGUI()
        {
            EditorGUILayout.LabelField("代码热更新方案: HybridCLR", EditorStyles.centeredGreyMiniLabel);

            BootProfile profile = BootProfile.GetInstance();
            EditorGUILayout.BeginHorizontal();
            bool enable = EditorGUILayout.Toggle(profile.IsEnableHotfixCore, GUILayout.Width(15));
            profile.SetEnableHotfix(enable);
            EditorGUILayout.LabelField("启用");
            EditorGUILayout.EndHorizontal();

            //ShowDoc();

            EditorGUI.BeginDisabledGroup(!enable);
            if (enable)
            {
                CheckHybridCLR();
            }
            else
            {
                IsDisable();
            }
            //SelectAssembly();
            EditorGUI.EndDisabledGroup();

            //EditorGUILayout.HelpBox("HybridCLR已开启,如选择不热更模式则需要关闭HybridCLR", MessageType.Error);

            profile.SaveIfDirty();

            if (GUILayout.Button("TestButton"))
            {
                Log.Info("TestButton");
            }
        }


        void CheckHybridCLRPackageIsInstall()
        {
#if ENABLE_HYBRIDCLR


#else

 
#endif
        }


        void ShowDoc()
        {
            string docURL = "https://hybridclr.doc.code-philosophy.com/docs/intro";
            if (GUILayout.Button($"官方文档:{docURL}"))
            {
                Application.OpenURL(docURL);
            }
            EditorGUILayout.Space();
        }

        ReorderableList list;
        void SelectAssembly()
        {
            BootProfile profile = BootProfile.GetInstance();

            if (list == null)
            {
                list = new ReorderableList(profile.AotMetaAssemblyNames, null, true, false, true, true);
                list.drawElementCallback += A;
            }
            list.DoLayoutList();

            profile.SaveIfDirty();
        }
        void A(Rect rect, int index, bool isActive, bool isFocused)
        {
            BootProfile profile = BootProfile.GetInstance();
            //Log.Info(index + ":" + isActive + ":" + isFocused);
            EditorGUI.TextField(rect, profile.HotfixAssemblyNames[index]);
        }



        bool CheckHybridCLR()
        {
#if ENABLE_HYBRIDCLR

            if (GUILayout.Button("移除HybridCLR"))
            {
                var r =  Client.Remove(packageName);
            }

            return IsFullInstall() && IsEnable() && CheckTargetPlatfrom() && CheckGC() && CheckIL2CPP() && CheckAPILevel();//判断是有顺序的
#else
            EditorGUILayout.HelpBox("HybridCLR Package(未安装)", MessageType.Error);
            UnityEditor.PackageManager.Requests.AddRequest a = null;
            if (GUILayout.Button("点击从git安装"))
            {
                a = Client.Add(gitURL);
            }
            if (a != null)
            {
                EditorGUILayout.LabelField(a.Status.ToString());
            }
            return false;
#endif
        }

#if ENABLE_HYBRIDCLR

        bool IsFullInstall()
        {
            var isFull = new InstallerController().HasInstalledHybridCLR();
            if (!isFull)//未完整安装
            {
                EditorGUILayout.HelpBox("HybridCLR分为3部分\r\n1.Package(已安装)\r\n2.HybrldCLR(未安装)\r\n3.Il2CPP Plus(未安装)", MessageType.Error);
                if (GUILayout.Button("打开HybridCLR Installer继续完成安装"))
                {
                    EditorWindow.GetWindow<InstallerWindow>("HybridCLR Installer", true).minSize = new Vector2(500, 200);
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
            //var target = Defines.TargetRuntimePlatform;
            //var select = BootConfig.Instance.AssemblyLoadType;
            //switch (select)
            //{
            //    case Defines.UpdateType.Online:
            //        if (target == (target & Defines.PlatformType.OnlineSupported))
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            Show(target.ToString());
            //            EditorGUILayout.HelpBox("目标平台不支持在线更新模式,默认只支持Windows/Android/iOS", MessageType.Error);
            //            if (GUILayout.Button("关闭热更新功能"))
            //            {
            //                //updateType.enumValueIndex = (int)Defines.UpdateType.Not;
            //                //SettingsUtil.Enable = false;
            //            }
            //        }
            //        break;
            //    case Defines.UpdateType.Offline:
            //        if (target == (target & Defines.PlatformType.OfflineSupported))
            //        {
            //            return true;
            //        }
            //        else
            //        {
            //            Show(target.ToString());
            //            EditorGUILayout.HelpBox("目标平台不支持离线更新,默认只支持Windows", MessageType.Error);
            //            if (GUILayout.Button("关闭热更新功能"))
            //            {
            //                //updateType.enumValueIndex = (int)Defines.UpdateType.Not;
            //                //SettingsUtil.Enable = false;
            //            }
            //        }
            //        break;
            //    case Defines.UpdateType.Not:
            //        return true;
            //}

            void Show(string label)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目标平台:", EditorStyles.boldLabel, GUILayout.Width(100));
                EditorGUILayout.LabelField(label);
                EditorGUILayout.EndHorizontal();
            }
            return true;
        }
        bool CheckGC()
        {
            var gc = PlayerSettings.gcIncremental;
            if (gc)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("增量GC:", EditorStyles.boldLabel, GUILayout.Width(100));
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
                EditorGUILayout.LabelField("编译后端:", EditorStyles.boldLabel, GUILayout.Width(100));
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
        bool CheckAPILevel()
        {
            var apiLevel = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (apiLevel != ApiCompatibilityLevel.NET_4_6)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("目标框架:", EditorStyles.boldLabel, GUILayout.Width(100));
                EditorGUILayout.LabelField(apiLevel.ToString());
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.HelpBox("主工程可以选择Standard 2.0或者4.x,但编译热更程序集必须使用4.x,如无必要建议统一到4.x", MessageType.Warning);
            }
            return true;
        }
        void CheckAssemblySetting()
        {
            //DrawAssembly(BootConfig.Instance.AssemblyNames);
            CheckAssemblyConsistency();
            //DrawAssembly(BootConfig.Instance.AotMetaAssemblyNames);

            EditorGUILayout.Space();
            if (GUILayout.Button("重置到默认"))
            {
                //ResetAssembly(Defines.DefaultAssemblyNames);
                //ResetAssembly(Defines.DefaultAOTMetaAssemblyNames);
            }
        }
        void CheckAssemblyConsistency()
        {
            //string[] assemblyNameValues = BootConfig.Instance.AssemblyNames;
            //for (int i = 0; i < assemblyNameValues.Length; i++)
            //{
            //    assemblyNameValues[i] = BootConfig.Instance.AssemblyNames[i];
            //}
            //List<string> clr = new List<string>();
            //if (HybridCLRSettings.Instance.hotUpdateAssemblies != null)
            //    clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblies);
            //if (HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions != null)
            //    clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.Where((a) => a != null).Select((a) => a.name));


            //if (!ComparisonArray(clr, assemblyNameValues))
            {
                EditorGUILayout.HelpBox("热更程序集列表与HybridCLR不一致", MessageType.Error);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("保存至HybridCLR"))
                {
                    HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = new UnityEditorInternal.AssemblyDefinitionAsset[0];
                    //HybridCLRSettings.Instance.hotUpdateAssemblies = assemblyNameValues;
                    HybridCLRSettings.Save();
                }
                EditorGUILayout.EndHorizontal();
            }
        }
#endif

        void ResetAssembly(string[] defVals)
        {
            //for (int i = 0; i < defVals.Length; i++)
            //{
            //    assembly.InsertArrayElementAtIndex(i);
            //    assembly.GetArrayElementAtIndex(i).stringValue = defVals[i];
            //}
        }
        bool ComparisonArray(IEnumerable<string> item1, IEnumerable<string> item2)
        {
            if (item1 == item2) return true;
            if (item1 == null || item2 == null) return false;

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
