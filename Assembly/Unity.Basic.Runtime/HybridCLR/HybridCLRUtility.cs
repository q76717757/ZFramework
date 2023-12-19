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
        public struct HotfixAssemblyInfo
        {
            //version
            public string path;
            public string md5;
            public long length;
        }

        /// <summary>
        /// 数据文件夹名称
        /// </summary>
        public const string DATA_FOLDER = "HybridCLR";
        /// <summary>
        /// 元数据程序集的包名
        /// </summary>
        internal const string META_ASSEMBLY_BUNDLE = "metaassembly";
        /// <summary>
        /// 热更程序集的包名
        /// </summary>
        public const string HOTFIX_ASSEMBLY_BUNDLE = "hotfixassembly";
        /// <summary>
        /// 热更程序集摘要文件名
        /// </summary>
        public const string HOTFIX_ASSEMBLY_XML = "hotfixassembly.xml";

        /// <summary>
        /// 当前加载的热更程序集的信息
        /// </summary>
        public static HotfixAssemblyInfo AssemblyInfo { get; private set; }


        /// <summary>
        /// 加载程序集
        /// </summary>
        internal static void LoadAssembly()
        {
            //补充元数据
            LoadMetadataForAOTAssembly();

            //读取当前程序集信息
            LoadAssemblyInfoXML();

            //加载热更程序集
            LoadHotfixAssembly();
        }

        private static void LoadMetadataForAOTAssembly()
        {
            string filePath = Path.Combine(Defines.BuildInAssetAPath, DATA_FOLDER, META_ASSEMBLY_BUNDLE);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
            foreach (TextAsset asset in bundle.LoadAllAssets<TextAsset>())
            {
                LoadImageErrorCode error = RuntimeApi.LoadMetadataForAOTAssembly(asset.bytes, HomologousImageMode.Consistent);
                if (error != LoadImageErrorCode.OK)
                {
                    throw new Exception($"{asset.name}元数据补充失败:{error}");
                }
            }
            bundle.Unload(true);
            Log.Info("Load MetaData Succeed");
        }
        private static void LoadAssemblyInfoXML()
        {
            HotfixAssemblyInfo assemblyInfo = new HotfixAssemblyInfo();
            string xmlPersistencePath = Path.Combine(Defines.PersistenceDataAPath, DATA_FOLDER, HOTFIX_ASSEMBLY_XML);
            string xmlBuildInPath = Path.Combine(Defines.BuildInAssetAPath, DATA_FOLDER, HOTFIX_ASSEMBLY_XML);
            string uri;
            if (File.Exists(xmlPersistencePath))
            {
                uri = xmlPersistencePath;
                assemblyInfo.path = Path.Combine(Defines.PersistenceDataAPath, DATA_FOLDER, HOTFIX_ASSEMBLY_BUNDLE);
            }
            else if (File.Exists(xmlBuildInPath))
            {
                uri = xmlBuildInPath;
                assemblyInfo.path = Path.Combine(Defines.BuildInAssetAPath, DATA_FOLDER, HOTFIX_ASSEMBLY_BUNDLE);
            }
            else
            {
                throw new FileNotFoundException(HOTFIX_ASSEMBLY_XML);
            }

            var xmlStr = DiskFilesLoadingUtility.DownLoadText(uri);
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlStr);
            var infoNode = xmlDoc.SelectSingleNode("AssemblyInfo");

            assemblyInfo.md5 = infoNode.Attributes["MD5"].Value;
            assemblyInfo.length = long.Parse(infoNode.Attributes["Length"].Value);

            AssemblyInfo = assemblyInfo;
            Log.Info("Load AssemblyInfo Succeed");
        }
        private static void LoadHotfixAssembly()
        {
            if (!File.Exists(AssemblyInfo.path))
            {
                throw new FileNotFoundException(AssemblyInfo.path);
            }
            AssetBundle bundle = AssetBundle.LoadFromFile(AssemblyInfo.path);
            foreach (TextAsset asset in bundle.LoadAllAssets<TextAsset>())
            {
                try
                {
                    Assembly.Load(asset.bytes);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            bundle.Unload(true);
            Log.Info("Load HotfixAssembly Succeed");
        }

    }
}
#endif