using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class VFSBuildPipeline
    {
        public static void BuildAssetBundles(BundleManifest manifest)
        {
            var builds = manifest.GetAssetBundleBuilds();
            var dic = BuildAssetBundles(builds);
            foreach (var bundle in dic)
            {
                string bundleName = bundle.Key;
                string bundleHash = bundle.Value;
                BundleInfo bundleInfo = manifest.GetBundleInfoByBundleName(bundleName);
                bundleInfo.hash = bundleHash;
            }
        }

        static Dictionary<string,string> BuildAssetBundles(AssetBundleBuild[] builds)
        {
            //创建输出目录
            string outputPath = $"Assets/07.Bundles/{Defines.TargetRuntimePlatform}/AssetsPackingCache";
            Directory.CreateDirectory(outputPath);
            AssetBundleManifest abm = BuildPipeline.BuildAssetBundles(outputPath, builds, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

            Dictionary<string,string> output = new Dictionary<string,string>();//bundleName -- bundleHash

            var allBundles = abm.GetAllAssetBundles();
            foreach (var item in allBundles)
            {
                var hash = abm.GetAssetBundleHash(item).ToString();
                output.Add(item, hash);
                Log.Info($"<color=yellow>Bundle->{item}</color>\r\nHASH->{hash}");
            }
            return output;
        }

    }
}
