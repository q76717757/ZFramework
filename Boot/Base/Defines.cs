using UnityEngine;
using System;
using System.IO;

namespace ZFramework
{
    /// <summary>
    /// 定义框架常量  基本每个工程都是一致的
    /// </summary>
    public static class Defines
    {
        public enum UpdateType : byte
        {
            /// <summary>
            /// 不支持热更的平台/没有网络环境的项目/临时开发不想走热更流程
            /// </summary>
            Not = 0,
            /// <summary>
            /// 需要平台支持热更+项目有网络环境+部署对应的热更服务 版本查询,资源下载等
            /// </summary>
            Online = 1,
            /// <summary>
            /// 仅针对WinPC项目  不需要部署热更服务 从本地load资源  因为windows系统操作文件方便
            /// </summary>
            Offline = 2,
        }

        [Flags]
        public enum PlatformType : byte
        {
            Unsupported = 1 << 0,
            Windows = 1 << 1,
            Windows64 = 1 << 2,
            iOS = 1 << 3,
            Android = 1 << 4,

            OnlineSupported = OfflineSupported | Android | iOS,
            OfflineSupported = Windows | Windows64,
        }


        /// <summary>
        /// 默认程序集
        /// </summary>
        public static string[] DefaultAssemblyNames => new string[]
        {
            "Unity.Hotfix.Core",
            "Unity.Hotfix.Data",
            "Unity.Hotfix.Func",
            "Unity.Hotfix.View",
            "Assembly-CSharp",
        };

        /// <summary>
        /// 目标平台
        /// </summary>
        public static PlatformType TargetRuntimePlatform
        {
            get
            {
#if UNITY_EDITOR
                return UnityEditor.EditorUserBuildSettings.activeBuildTarget switch
                {
                    UnityEditor.BuildTarget.StandaloneWindows => PlatformType.Windows,
                    UnityEditor.BuildTarget.StandaloneWindows64 => PlatformType.Windows64,
                    UnityEditor.BuildTarget.iOS => PlatformType.iOS,
                    UnityEditor.BuildTarget.Android => PlatformType.Android,
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

        /// <summary>
        /// 数据持久化根目录
        /// </summary>
        public static string PersistenceDataDir
        {
            get
            {
#if UNITY_EDITOR
                return Application.streamingAssetsPath + "/Temp";
#else

                if (TargetRuntimePlatform == PlatformType.Windows || TargetRuntimePlatform == PlatformType.Windows64)
                {
                    return Directory.GetParent(Application.dataPath).FullName; //win平台放在根目录
                }
                else
                {
                    return Application.persistentDataPath;
                }
#endif
            }
        }


        /// <summary>
        /// 默认元数据程序集 (HybridCLR)
        /// </summary>
        public static string[] DefaultAOTMetaAssemblyNames => new string[]
        {
            "mscorlib",
            "System",
            "System.Core",
            "Unity.Boot",
            "Unity.Package.Runtime",
        };

        /// <summary>
        /// 元数据程序集的加载路径 (HybridCLR)
        /// </summary>
        public static string AOTMetaAssemblyLoadDir => Application.streamingAssetsPath + "/AOT";
    }
}
