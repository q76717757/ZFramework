using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Editor
{
    public enum AssetType
    {
        /// <summary>
        /// 公开资源  VFS中被定义的资源 作为依赖分析的起点  可以被脚本获取
        /// </summary>
        PublicAsset,
        /// <summary>
        /// 共享资源  被多个资源所依赖
        /// </summary>
        ShareAsset,
        /// <summary>
        /// 独享资源   只被一个资源所依赖
        /// </summary>
        AloneAsset,
    }

    public class AssetReference
    {
        public string Guid { get; }
        public string AssetPath { get; }
        public AssetType Type { get; set; }

        /// <summary>
        /// 我直接依赖的资源
        /// </summary>
        public List<AssetReference> Children { get; } = new List<AssetReference>();

        public AssetReference(string guid, string assetPath, AssetType type)
        {
            Guid = guid;
            AssetPath = assetPath;
            Type = type;
        }

    }
}
