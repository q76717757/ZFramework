using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class ZFrameworkRuntimeSettings : ScriptableObject
    {
        public static string AssetName => "PROJECT SETTINGS";
        public static ZFrameworkRuntimeSettings Get() => Resources.Load<ZFrameworkRuntimeSettings>(AssetName);


        [Header("热更新模式")]
        [SerializeField] private Defines.UpdateType updateType = Defines.UpdateType.Not;
        [Header("加载程序集,AssemblyLoader将从上到下依次加载")]
        [SerializeField] private string[] assemblyNames = Defines.DefaultAssemblyNames;
        [Header("补充元数据程序集 (HybridCLR)")]
        [SerializeField] private string[] aotMetaAssemblyNames = Defines.DefaultAOTMetaAssemblyNames;


        [Header("版本对比链接")]
        [SerializeField] private string versionUrl;
        [Header("资源下载地址")]
        [SerializeField] private string downloadUrl;
        [Header("强版本同步")]
        [SerializeField] private bool mustNew;//弱同步项目则可以手动更新


        public Defines.UpdateType AssemblyLoadType => updateType;
        public string[] AssemblyNames => assemblyNames;
        public string[] AotMetaAssemblyNames => aotMetaAssemblyNames;

        public string VersionUrl => versionUrl;
        public string DownloadUrl => downloadUrl;
        public bool MustNew => mustNew;

        //其他模式的配置值也留下
    }
}