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
    public static partial class Defines //公开的
    {
        [Flags]
        public enum PlatformType : byte
        {
            Unsupported,
            Windows       = 1 << 0,
            Windows64     = 1 << 1,
            iOS           = 1 << 2,
            Android       = 1 << 3,
            //WebGL       = 1 << 4,webgl热更有特殊的处理,时间关系暂不考虑
            //MiniGame    = 1 << 5

            OfflineSupported = Windows | Windows64,
            OnlineSupported = OfflineSupported | Android | iOS,
        }

        /// <summary>
        /// 项目代号  (一般用来给各种服务作为根路径,要求仅ASCII码)
        /// </summary>
        public const string PROJECT_CODE = "DaschowVitrualWorld";

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
        /// 随包资产的路径(只读)  ./StreamingAssets/ZFramework/{Platform}
        /// </summary>
        public static string BuildInAssetAPath => GetBuildInAssetsAPath(TargetRuntimePlatform);

        /// <summary>
        /// 数据持久化根目录(可读写)  windows平台./AssetFiles/   其他平台返回./PersistentDataPath/AssetFiles
        /// </summary>
        public static string PersistenceDataAPath => GetPresistenceDataAPath(TargetRuntimePlatform);

    }

    public static partial class Defines //内部的
    {
        internal static string[] AssemblyNames = new string[]
        {
            "Unity.Share.Core",
            "Unity.Share.Model",
            "Unity.Workspace.Module",
            "Unity.Workspace.View",
            "Assembly-CSharp",
        };

        /// <summary>
        /// 获取指定平台下的随包资源的存放路径(只读目录)
        /// </summary>
        /// <param name="platform">目标运行时平台</param>
        /// <returns>返回./StreamingAssets/ZFramework/{platform}/</returns>
        internal static string GetBuildInAssetsAPath(PlatformType platform)
        {
            return new DirectoryInfo(Path.Combine(Application.streamingAssetsPath, "ZFramework", platform.ToString())).FullName;
        }

        /// <summary>
        /// 获取指定平台下的持久化资源的存放路径(可读写目录)
        /// </summary>
        /// <param name="platform">目标运行时平台</param>
        /// <returns>windows平台./AssetFiles/   其他平台返回./PersistentDataPath/AssetFiles/</returns>
        internal static string GetPresistenceDataAPath(PlatformType platform)
        {
            DirectoryInfo directory;
#if UNITY_EDITOR
            directory = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "AssetFiles"));
#else
            if ((platform & PlatformType.OfflineSupported) == platform)
            {
                //windows平台放在根目录下 方便用户自己替换资源
                directory = new DirectoryInfo(Path.Combine(Application.dataPath, "..", "AssetFiles"));
            }
            else
            {
                directory = new DirectoryInfo(Application.persistentDataPath);
            }
#endif
            return directory.FullName;
        }
    }
}
