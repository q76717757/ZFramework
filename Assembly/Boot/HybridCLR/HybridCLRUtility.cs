#if ENABLE_HYBRIDCLR
using System;
using System.IO;
using UnityEngine;
using HybridCLR;
using System.Reflection;

namespace ZFramework
{
    public static class HybridCLRUtility
    {
        /// <summary>
        /// 热更程序集的包名
        /// </summary>
        public static string HotfixAssemblyBundleName => "hotfixassembly.ab";//ab包的名字是全小写的

        /// <summary>
        /// AOT元数据程序集的包名
        /// </summary>
        public static string AOTMetaAssemblyBundleName => "metaassembly.ab";

        /// <summary>
        /// 随包程序集的加载路径
        /// </summary>
        public static string BuildInAssemblyBundleLoadAPath
        {
            get
            {
                return Path.Combine(Defines.BuildInAssetAPath, "Assembly");
            }
        }
        /// <summary>
        /// 持久化程序集加载路径
        /// </summary>
        public static string PersistenceAssemblyBundleLoadAPath
        {
            get
            {
                return Path.Combine(Defines.PersistenceDataAPath, "Assembly");
            }
        }


        /// <summary>
        /// 加载程序集
        /// </summary>
        internal static void LoadAssembly()
        {
            //补充元数据
            LoadMetadataForAOTAssembly(Path.Combine(BuildInAssemblyBundleLoadAPath, AOTMetaAssemblyBundleName));

            //加载热更程序集
            string hotfixDLLPath = Path.Combine(PersistenceAssemblyBundleLoadAPath, HotfixAssemblyBundleName);
            string buildinDLLPath = Path.Combine(BuildInAssemblyBundleLoadAPath, HotfixAssemblyBundleName);
            if (File.Exists(hotfixDLLPath))
            {
                LoadHotfixAssembly(hotfixDLLPath);
            }
            else
            {
                LoadHotfixAssembly(buildinDLLPath);
            }
        }


        private static void LoadMetadataForAOTAssembly(string uri)
        {
            byte[] data = DiskFilesLoadingUtility.DownLoadBytes(uri);
            AssetBundle bundle = AssetBundle.LoadFromMemory(data);
            TextAsset[] assets = bundle.LoadAllAssets<TextAsset>();
            foreach (TextAsset asset in assets)
            {
                LoadImageErrorCode error = RuntimeApi.LoadMetadataForAOTAssembly(asset.bytes, HomologousImageMode.Consistent);
                if (error != LoadImageErrorCode.OK)
                {
                    Log.Error($"{asset.name}元数据补充失败:{error}");
                }
                else
                {
                    Log.Info("补充AOT元数据->" + asset.name);
                }
            }
            bundle.Unload(true);
        }
        private static void LoadHotfixAssembly(string uri)
        {
            byte[] data = DiskFilesLoadingUtility.DownLoadBytes(uri);
            AssetBundle bundle = AssetBundle.LoadFromMemory(data);
            TextAsset[] assets = bundle.LoadAllAssets<TextAsset>();
            foreach (TextAsset asset in assets)
            {
                try
                {
                    Assembly.Load(asset.bytes);
                    Log.Info("加载热更程序集->" + asset.name);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            bundle.Unload(true);
        }
    }
}
#endif