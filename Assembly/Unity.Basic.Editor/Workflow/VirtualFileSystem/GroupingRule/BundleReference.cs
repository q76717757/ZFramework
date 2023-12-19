using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    public class BundleReference
    {
        /// <summary>
        /// 包名
        /// </summary>
        public string BundleName { get;}
        /// <summary>
        /// 组成的区块
        /// </summary>
        public HashSet<BundleBlock> Blocks { get; } = new HashSet<BundleBlock>();
        /// <summary>
        /// 依赖的包
        /// </summary>
        public HashSet<BundleReference> Children { get; } = new HashSet<BundleReference>();

        public BundleReference(string bundleName)
        {
            BundleName = bundleName;
        }

        public BundleInfo BuildInfo()
        {
            BundleInfo output = new BundleInfo();
            output.bundleName = BundleName;

            Log.Info($"<color=yellow>[{BundleName}]</color> 块->{Blocks.Count} 依赖->{Children.Count}");
            foreach (var asset in Blocks)
            {
                output.assetGuids.Add(asset.MainAsset.Guid);
                Log.Info($"包含块--->[{asset.MainAsset.AssetPath}]");
            }
            foreach (var asset in Children)
            {
                Log.Info($"依赖包--->{asset.BundleName}");
                output.dependencies.Add(asset.BundleName);
            }
            return output;
        }
    }
}
