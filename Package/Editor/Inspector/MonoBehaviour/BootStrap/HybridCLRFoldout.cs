using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering.Universal;
using UnityEditor.PackageManager.Requests;
using System.Linq;
using System;

#if ENABLE_HYBRIDCLR
using HybridCLR;
using HybridCLR.Editor;
using HybridCLR.Editor.Installer;
#endif

namespace ZFramework.Editor
{
    public partial class HybridCLRFoldout : FadeFoldout
    {
        public override string Title => "代码热更新";

        static string gitURL = "git@gitee.com:focus-creative-games/hybridclr_unity.git";
        static AddRequest addRequest;
        BootProfile Profile
        {
            get
            {
                return BootProfile.GetInstance();
            }
        }

        protected override void OnGUI()
        {
            if (IsEnableHotfix())
            {
#if ENABLE_HYBRIDCLR //已安装HybridCLR Package
                if (SettingsIsCorrect())
                {
                    DrawAssemblySettings();
                    ShowPath();
                }
#else
                IntallPackage();
#endif
            }
            else
            {
#if ENABLE_HYBRIDCLR
                IsDisable();
#endif
            }
            Profile.SaveIfDirty();
        }
        bool IsEnableHotfix()
        {
            Rect rect = EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("热更新方案:HybridCLR", EditorStyles.centeredGreyMiniLabel);
            if (EditorGUILayout.LinkButton($"官方文档: https://hybridclr.doc.code-philosophy.com"))
            {
                Application.OpenURL("https://hybridclr.doc.code-philosophy.com/docs/intro");
            }
            EditorGUILayout.EndVertical();

            bool enable = EditorGUI.ToggleLeft(new Rect(rect.x + 3, rect.y + 2, 50, EditorGUIUtility.singleLineHeight), "启用", Profile.isEnableHotfixCode, EditorStyles.miniLabel);
            if (enable != Profile.isEnableHotfixCode)
            {
                Profile.isEnableHotfixCode = enable;
                Profile.SetDirty();
            }
            return enable;
        }
        void IntallPackage()
        {
            EditorGUILayout.HelpBox("HybridCLR Package(未安装)", MessageType.Error);
            bool isInstalling = addRequest != null && addRequest.Status == StatusCode.InProgress;
            EditorGUI.BeginDisabledGroup(isInstalling);
            if (GUILayout.Button(isInstalling ? "安装中..." : "点击从Package Manager安装"))
            {
                addRequest = Client.Add(gitURL);
            }
            EditorGUI.EndDisabledGroup();
        }
    }


#if ENABLE_HYBRIDCLR
    public partial class HybridCLRFoldout
    { 
        static string packageName = "com.code-philosophy.hybridclr";
        static RemoveRequest removeRequest;
        ReorderableList _hotfixList;
        ReorderableList _aotMetaList;
        ReorderableList HotfixList
        {
            get
            {
                if (_hotfixList == null)
                {
                    _hotfixList = new ReorderableList(Profile.hotfixAssemblyNames, typeof(string), true, false, true, true);
                    _hotfixList.drawElementCallback += OnDrawHotfixElement;
                    _hotfixList.onChangedCallback += OnListChange;
                }
                return _hotfixList;
            }
        }
        ReorderableList AotMetaList
        {
            get
            {
                if (_aotMetaList == null)
                {
                    _aotMetaList = new ReorderableList(Profile.aotMetaAssemblyNames, typeof(string), true, false, true, true);
                    _aotMetaList.drawElementCallback += OnDrawAotMetaElement;
                    _aotMetaList.onChangedCallback += OnListChange;
                }
                return _aotMetaList;
            }
        }

        bool SettingsIsCorrect()
        { 
            return IsFullInstall() && IsEnable() && IsSupported() && IsIL2CPP() && IsDotNet4X() && CheckGC();//判断是有顺序的;
        }
        bool IsFullInstall()
        {
            bool isFull = new InstallerController().HasInstalledHybridCLR();
            if (!isFull)//未完整安装
            {
                EditorGUILayout.HelpBox("HybridCLR分为3部分\r\n1.Package(已安装)\r\n2.HybrldCLR(未安装)\r\n3.Il2CPP Plus(未安装)", MessageType.Error);
                if (GUILayout.Button("继续完成安装(需要有git环境)"))
                {
                    new InstallerController().InstallDefaultHybridCLR();
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
                EditorGUILayout.HelpBox("HybridCLR是关闭的,如启用代码热更流程,需要将HybridCLR开启", MessageType.Error);
                if (GUILayout.Button("启用HybridCLR"))
                {
                    SettingsUtil.Enable = true;
                }
                return false;
            }
        }
        bool IsSupported()
        {
            bool isUnsupport = Defines.TargetRuntimePlatform == Defines.PlatformType.Unsupported;
            if (isUnsupport)
            {
                EditorGUILayout.HelpBox("当前的目标运行时不被支持,请关闭代码热更新流程", MessageType.Error);
                if (GUILayout.Button("点击关闭"))
                {
                    SettingsUtil.Enable = false;
                    BootProfile profile = BootProfile.GetInstance();
                    if (profile.isEnableHotfixCode)
                    {
                        profile.isEnableHotfixCode = false;
                        profile.SetDirty();
                    }
                }
            }
            return !isUnsupport;
        }
        bool CheckGC()
        {
            if (PlayerSettings.gcIncremental)
            {
                EditorGUILayout.HelpBox("HybridCLR刚支持增量GC,如果出现问题可以尝试关闭增量GC", MessageType.Warning);
                if (GUILayout.Button("关闭增量GC功能"))
                {
                    PlayerSettings.gcIncremental = false;
                }
            }
            return true;
        }
        bool IsIL2CPP()
        {
            var isILCPP = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP;
            if (!isILCPP)
            {
                EditorGUILayout.HelpBox("HybridCLR是基于IL2CPP的,需要将编译后端改为IL2CPP", MessageType.Error);
                if (GUILayout.Button("切换到IL2CPP"))
                {
                    PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.IL2CPP);
                }
            }
            return isILCPP;
        }
        bool IsDotNet4X()
        {
#if UNITY_2021_1_OR_NEWER
            var targetAPILevel = ApiCompatibilityLevel.NET_Unity_4_8;
#else
            var targetAPILevel = ApiCompatibilityLevel.NET_4_6;
#endif
            var isDotNetFramework = PlayerSettings.GetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup) == targetAPILevel;
            if (!isDotNetFramework)
            {
                EditorGUILayout.HelpBox("HybridCLR要求目标框架APILevel需要是.Net Framework", MessageType.Error);
                if (GUILayout.Button("切换.Net Framework"))
                {
                    PlayerSettings.SetApiCompatibilityLevel(EditorUserBuildSettings.selectedBuildTargetGroup, targetAPILevel);
                }
            }
            return isDotNetFramework;
        } 
        bool IsDisable()
        {
            if (!SettingsUtil.Enable)
            {
                return true;
            }
            else
            {
                EditorGUILayout.HelpBox("HybridCLR已开启,如不启用代码热更新流程,需要将HybridCLR关闭", MessageType.Error);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("禁用HybridCLR"))
                {
                    SettingsUtil.Enable = false;
                }
                if (GUILayout.Button("移除HybridCLR Package"))
                {
                    removeRequest = Client.Remove(packageName);
                }
                EditorGUILayout.EndHorizontal();
                return false;
            }
            
        }


        void DrawAssemblySettings()
        {
            var rect = EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(true));

            EditorGUILayout.LabelField("补充元数据程序集", EditorStyles.centeredGreyMiniLabel);
            AotMetaList.DoLayoutList();
            EditorGUILayout.LabelField("热更新程序集", EditorStyles.centeredGreyMiniLabel);
            HotfixList.DoLayoutList();

            CheckAssemblyConsistency();

            EditorGUILayout.EndVertical();

            if (GUI.Button(new Rect(rect.x + rect.width - 43, rect.y + 3, 40, 15), "重置", EditorStyles.miniButton))
            {
                GUI.FocusControl(null);
                BootProfile defaultValue = new BootProfile();
                Profile.hotfixAssemblyNames.Clear();
                Profile.aotMetaAssemblyNames.Clear();
                Profile.hotfixAssemblyNames.AddRange(defaultValue.hotfixAssemblyNames);
                Profile.aotMetaAssemblyNames.AddRange(defaultValue.aotMetaAssemblyNames);
                OnListChange();
            }
        }
        void OnDrawHotfixElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawList(in Profile.hotfixAssemblyNames, rect, index);
        }
        void OnDrawAotMetaElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            DrawList(in Profile.aotMetaAssemblyNames, rect, index);
        }
        void DrawList(in List<string> list, Rect rect, int index)
        {
            string next = EditorGUI.DelayedTextField(rect, list[index]);
            if (string.IsNullOrEmpty(next))
            {
                EditorGUI.LabelField(rect, "不能为空", EditorStyles.centeredGreyMiniLabel);
            }
            if (next != list[index])
            {
                list[index] = next;
                OnListChange();
            }
        }
        void OnListChange(ReorderableList list = default)
        {
            SyncSettings(Profile.hotfixAssemblyNames.ToArray());
            Profile.SetDirty();
        }

        void CheckAssemblyConsistency()
        {
            string[] assemblyNameValues = Profile.hotfixAssemblyNames.ToArray();

            List<string> clr = new List<string>();
            if (HybridCLRSettings.Instance.hotUpdateAssemblies != null)
                clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblies);
            if (HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions != null)
                clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.Select((a) => a == null ? "" : a.name));

            if (!ComparisonArray(clr, assemblyNameValues))
            {
                EditorGUILayout.HelpBox("热更程序集列表与HybridCLR不一致", MessageType.Error);
                if (GUILayout.Button("同步至HybridCLR Settings"))
                {
                    SyncSettings(assemblyNameValues);
                }
            }
        }
        void SyncSettings(string[] assemblyNameValues)
        {
            HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = new AssemblyDefinitionAsset[0];
            HybridCLRSettings.Instance.hotUpdateAssemblies = assemblyNameValues;
            HybridCLRSettings.Save();
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

        void ShowPath()
        {
            //EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(true));
            //EditorGUILayout.LabelField(HybridCLRSettings.Instance.hotUpdateDllCompileOutputRootDir);
            //EditorGUILayout.LabelField(HybridCLRSettings.Instance.strippedAOTDllOutputRootDir);
            //if (GUILayout.Button("HybridCLR Settings"))
            //{
            //    SettingsService.OpenProjectSettings("Project/HybridCLR Settings");
            //}
            //EditorGUILayout.EndVertical();
        }

    }
#endif

}