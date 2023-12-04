using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Editor
{
    /// <summary>
    /// 资源包分块   1+N的构成  一个块由一个公开资源或者共享资源 + N个仅依赖于这个资源的独享资源所组成
    /// </summary>
    public class BundleBlock
    {
        /// <summary>
        /// 块的主资源
        /// </summary>
        public AssetReference MainAsset { get; }
        /// <summary>
        /// 所属的包名
        /// </summary>
        public BundleReference Bundle { get; set; }
        /// <summary>
        /// 起源Root
        /// </summary>
        public HashSet<BundleBlock> Root { get; } = new HashSet<BundleBlock>();
        /// <summary>
        /// 我直接依赖的块
        /// </summary>
        public HashSet<BundleBlock> Childs { get; } = new HashSet<BundleBlock>();

        public BundleBlock(AssetReference mainAsset)
        {
            this.MainAsset = mainAsset;
        }
    }

}
