using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class ZFrameworkRuntimeSettings : ScriptableObject
    {
        public static string AssetName => "PROJECT SETTINGS";
        public static ZFrameworkRuntimeSettings Get()
        {
            return Resources.Load<ZFrameworkRuntimeSettings>(AssetName);
        }




        [Header("热更新模式")]
        [SerializeField] private HotUpdateType hotUpdateType;
        [Header("加载程序集,AssemblyLoader将从上到下依次加载")]
        [SerializeField] private string[] assemblyNames = Defines.DefaultAssemblyNames;
        [Header("补充元数据程序集")]
        [SerializeField] private string[] aotMetaAssemblyNames = Defines.DefaultAOTMetaAssemblyNames;

        [Header("版本对比链接")]
        [SerializeField] private string versionUrl;
        [Header("资源下载地址")]
        [SerializeField] private string downloadUrl;

        [Header("必须保持最新")]
        [SerializeField] private bool mustNew;




        public HotUpdateType AssemblyLoadType => hotUpdateType;
        public string VersionUrl => versionUrl;
        public string DownloadUrl => downloadUrl;
        public string[] AssemblyNames => assemblyNames;
        public string[] AotMetaAssemblyNames => aotMetaAssemblyNames;

        //其他模式的配置值也留下
    }
}