#if ENABLE_HYBRIDCLR
using System;
using System.IO;
using UnityEngine;
using HybridCLR;
using System.Reflection;
using System.Xml;

namespace ZFramework
{
    public static class HybridCLRUtility
    {
        /// <summary>
        /// 热更程序集的包名
        /// </summary>
        public static string HotfixAssemblyBundleName => "hotfixassembly.ab";//ab包的名字是全小写的
        /// <summary>
        /// 热更程序集摘要记录文件名
        /// </summary>
        public static string HotfixAssemblyXmlName => "hotfixassemblyInfo.xml";//记录编译时间和ab包的hash,用于版本对比
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
        public static string Hash { get; private set; }

        /// <summary>
        /// 加载程序集
        /// </summary>
        internal static void LoadAssembly()
        {
            //补充元数据
            LoadMetadataForAOTAssembly(Path.Combine(BuildInAssemblyBundleLoadAPath, AOTMetaAssemblyBundleName));

            //加载程序集配置单
            string xmlPath = Path.Combine(PersistenceAssemblyBundleLoadAPath, HotfixAssemblyXmlName);//持久化路径
            string xmlPath2 = Path.Combine(BuildInAssemblyBundleLoadAPath, HotfixAssemblyXmlName);//内置路径

            if (File.Exists(xmlPath))
            {
                LoadAssemblyInfoXML(xmlPath);
                LoadHotfixAssembly(Path.Combine(PersistenceAssemblyBundleLoadAPath, HotfixAssemblyBundleName));
            }
            else
            {
                LoadAssemblyInfoXML(xmlPath2);
                LoadHotfixAssembly(Path.Combine(BuildInAssemblyBundleLoadAPath, HotfixAssemblyBundleName));
            }
        }


        private static void LoadAssemblyInfoXML(string uri)
        {
            var xmlStr = DiskFilesLoadingUtility.DownLoadText(uri);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);
            Hash = xmlDoc.SelectSingleNode("HotfixAssemblyInfo").Attributes["Hash"].Value;
        }
        private static void LoadMetadataForAOTAssembly(string uri)
        {
            if (!File.Exists(uri))
            {
                Log.Warning("AOT Assembly Not Exists,存在没有AOT的情况,但通常情况下都会有 可以检查一下打包设置是不是漏了");
                return;
            }

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