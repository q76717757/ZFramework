using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZFramework
{
    public class BundleInfo
    {
        /// <summary>
        /// 包名
        /// </summary>
        public string bundleName;
        /// <summary>
        /// 包文件hash值
        /// </summary>
        public string hash;
        /// <summary>
        /// 包含的显式资源   guid
        /// </summary>
        public List<string> assetGuids = new List<string>();
        /// <summary>
        /// 依赖的包名
        /// </summary>
        public List<string> dependencies = new List<string>();

        public string FileName
        {
            get
            {
                return $"{bundleName}_{hash}";
            }
        }
    }

    public class BundleManifest
    {
        //bundleName --- bundleInfo
        private Dictionary<string, BundleInfo> bundleInfos = new Dictionary<string, BundleInfo>();

        public void AddBundleInfo(BundleInfo bundleInfo)
        {
            if (bundleInfo == null)
            {
                throw new System.Exception("bundleinfo is null");
            }
            bundleInfos.Add(bundleInfo.bundleName, bundleInfo);
        }
        public BundleInfo[] GetBundles()
        {
            return bundleInfos.Values.ToArray();
        }
        public BundleInfo GetBundleInfoByAssetGuid(string assetGuid)
        {
            foreach (BundleInfo bundleInfo in bundleInfos.Values)
            {
                if (bundleInfo.assetGuids.Contains(assetGuid))
                {
                    return bundleInfo;
                }
            }
            return null;
        }
        public BundleInfo GetBundleInfoByBundleName(string bundleName)
        {
            if (bundleInfos.TryGetValue(bundleName, out BundleInfo bundle))
            {
                return bundle;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取文件hash
        /// </summary>
        public string GetBundleHash(string bundleName)
        {
            BundleInfo bundleInfo = GetBundleInfoByBundleName(bundleName);
            if (bundleInfo != null)
            {
                return bundleInfo.hash;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获取直接依赖
        /// </summary>
        public string[] GetDirectDependencies(string bundleName)
        {
            List<string> bundleNames = new List<string>();
            GetDependencies(bundleName, ref bundleNames, false);
            return bundleNames.ToArray();
        }
        /// <summary>
        /// 获取所有依赖
        /// </summary>
        public string[] GetAllDependencies(string bundleName)
        {
            List<string> bundleNames = new List<string>();
            GetDependencies(bundleName, ref bundleNames, true);
            return bundleNames.ToArray();
        }
        private void GetDependencies(string bundleName, ref List<string> bundleNames,bool recursion)
        {
            BundleInfo bundleInfo = GetBundleInfoByBundleName(bundleName);
            if (bundleInfo != null && bundleInfo.dependencies.Count > 0)
            {
                bundleNames.AddRange(bundleInfo.dependencies);
                if (recursion)
                {
                    foreach (var item in bundleInfo.dependencies)
                    {
                        GetDependencies(item, ref bundleNames, recursion);
                    }
                }
                
            }
        }

    }
}
