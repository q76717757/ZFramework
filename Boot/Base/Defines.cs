using UnityEngine;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZFramework
{
    /// <summary>
    /// 定义框架常量
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
            /// 仅针对WinPC项目  不需要部署热更服务 从本地load资源  因为windows系统操作文件方便  适用于一些内网/无网PC项目
            /// </summary>
            Offline = 2,
        }

        [Flags]
        public enum PlatformType : byte
        {
            Unsupported,
            Windows       = 1 << 0,
            Windows64     = 1 << 1,
            iOS           = 1 << 2,
            Android       = 1 << 3,
            //WebGL       = 1 << 4,webgl热更有特殊的处理,时间关系暂不考虑

            OfflineSupported = Windows | Windows64,
            OnlineSupported = OfflineSupported | Android | iOS,
        }

        /// <summary>
        /// 目标运行时平台
        /// </summary>
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
                    _ => PlatformType.Unsupported,
                };
#endif
            }
        }

        /// <summary>
        /// 默认热更新程序集 (HybridCLR)
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
        /// 默认补充元数据程序集 (HybridCLR)
        /// </summary>
        public static string[] DefaultAOTMetaAssemblyNames => new string[]
        {
            "mscorlib",
            "System",
            "System.Core",
            "UnityEngine.CoreModule",
            "Unity.Package.Runtime",
        };

        /// <summary>
        /// 随包资产的路径  ./StreamingAssets/BuildInAssets/{Platform}
        /// </summary>
        public static string BuildInAssetAPath
        {
            get
            {
                return GetBuildInAssetsAPath(TargetRuntimePlatform);
            }
        }

        /// <summary>
        /// 数据持久化根目录  windows平台./Res/   其他平台返回./PersistentDataPath/Res
        /// </summary>
        public static string PersistenceDataAPath
        {
            get
            {
                return GetPresistenceDataAPath(TargetRuntimePlatform);
            }
        }

        /// <summary>
        /// 随包配置文件所在的文件夹
        /// </summary>
        public static string ProfilesAPath
        {
            get
            {
                return new DirectoryInfo(Path.Combine(BuildInAssetAPath, "Profiles")).FullName;
            }
        }

        /// <summary>
        /// 元数据程序集的加载路径 (HybridCLR)
        /// </summary>
        public static string AOTMetaAssemblyAPath
        {
            get
            {
                return new DirectoryInfo(Path.Combine(BuildInAssetAPath, "Assembly")).FullName;
            }
        }

        /// <summary>
        /// 获取指定平台下的随包资源的存放路径(只读目录)
        /// </summary>
        /// <param name="platform">目标运行时平台</param>
        /// <returns>返回./StreamingAssets/BuildInAssets/{platform}/</returns>
        public static string GetBuildInAssetsAPath(PlatformType platform)
        {
            return new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "ZFramework", platform.ToString())).FullName;
        }

        /// <summary>
        /// 获取指定平台下的持久化资源的存放路径(可读写目录)
        /// </summary>
        /// <param name="platform">目标运行时平台</param>
        /// <returns>windows平台./Res/   其他平台返回./PersistentDataPath/Res/ </returns>
        public static string GetPresistenceDataAPath(PlatformType platform)
        {
#if UNITY_EDITOR
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "AssetBundleCache", platform.ToString()));
#else
            DirectoryInfo directory;
            if ((platform & PlatformType.OfflineSupported) == platform)
            {
                //离线模式仅针对winpc 资产放在安装目录 让用户自己可以替换ab包进行更新
                directory = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "Res"));
            }
            else
            {
                directory = new DirectoryInfo(Path.Combine(Application.persistentDataPath, "Res"));
            }
#endif
            return directory.FullName;
        }
    }
}
