using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZFramework
{
    public enum HotUpdateType : byte
    {
        /// <summary>
        /// 需要平台支持热更+项目有网络环境+部署对应的热更服务 版本查询,资源下载等
        /// </summary>
        Online,
        /// <summary>
        /// 仅针对WinPC项目  不需要部署热更服务 从本地load资源  因为windows系统操作文件方便
        /// </summary>
        Offline,
        /// <summary>
        /// 不支持热更的平台/没有网络环境的项目/临时开发不想走热更流程
        /// </summary>
        Not
    }

    [Flags]
    public enum PlatformType : byte
    {
        Unsupported          = 1 << 0,
        Windows              = 1 << 1,
        Windows64            = 1 << 2,
        iOS                  = 1 << 3,
        Android              = 1 << 4,

        OnlineSupported = Windows | Windows64 | Android | iOS,
        OfflineSupported = Windows | Windows64,
    }

    /// <summary>
    /// 定义框架常量  基本每个工程都是一致的 不太会改动的
    /// </summary>
    public static class Defines
    {
        //热更程序集默认值
        public static string[] DefaultAssemblyNames => new string[]
        {
            "Unity.Hotfix.Core",
            "Unity.Hotfix.Data",
            "Unity.Hotfix.Func",
            "Unity.Hotfix.View",
            "Assembly-CSharp",
        };
        //补充元数据默认值
        public static string[] DefaultAOTMetaAssemblyNames => new string[]
        {
            "mscorlib",
            "System",
            "System.Core",
            "Unity.Boot",
            "Unity.Package.Runtime",
        };
        //补充元数据程序集的加载路径
        public static string AOTMetaAssemblyLoadDir => Application.streamingAssetsPath + "/AOT";
        
        public static bool HybridCLRPackageIsInstalled
        {
            get
            {
#if ENABLE_HYBRIDCLR
                return true;
#else
                return false;
#endif
            }
        }
        
        public static bool IsEditor
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }
        //目标平台
        public static PlatformType TargetRuntimePlatform
        {
            get
            {
#if UNITY_EDITOR
                return EditorUserBuildSettings.activeBuildTarget switch
                {
                    BuildTarget.StandaloneWindows => PlatformType.Windows,
                    BuildTarget.StandaloneWindows64 => PlatformType.Windows64,
                    BuildTarget.iOS => PlatformType.iOS,
                    BuildTarget.Android => PlatformType.Android,
                    _ => PlatformType.Unsupported,
                };

#else
                return Application.platform switch
                {
                    RuntimePlatform.WindowsPlayer => IntPtr.Size == 4? PlatformType.Windows : PlatformType.Windows64,
                    RuntimePlatform.IPhonePlayer => PlatformType.iOS,
                    RuntimePlatform.Android => PlatformType.Android,
                    _=> PlatformType.Unsupported,
                };
#endif
            }
        }
        //持久化目录
        public static string PersistenceDataDir//win平台放在根目录
        {
            get
            {
                if (IsEditor)
                {
                    return Application.streamingAssetsPath + "/Temp";
                }
                else
                {
                    if (TargetRuntimePlatform == PlatformType.Windows || TargetRuntimePlatform == PlatformType.Windows64)
                    {
                        return Directory.GetParent(Application.dataPath).FullName;
                    }
                    else
                    {
                        return Application.persistentDataPath;
                    }
                }
            }
        }

    }
}
