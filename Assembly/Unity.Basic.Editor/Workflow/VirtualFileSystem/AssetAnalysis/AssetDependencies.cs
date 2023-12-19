using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{

    /// <summary>
    /// 所有相关资源的依赖网
    /// </summary>
    internal class AssetDependencies
    {
        //所有产生依赖的资源的集合  guid-asset
        Dictionary<string, AssetReference> assetRefenreces = new Dictionary<string, AssetReference>();//产生依赖的所有资源 包括显式和隐式依赖

        public AssetDependencies(AssetCollection collection)
        {
            //建立资源和资源之间的完整的依赖网
            Analysis(collection);
        }
        void Analysis(AssetCollection collection)
        {
            foreach (VFSMetaData metaData in collection.GetAllMetaData())
            {
                Analysis(metaData.Guid, metaData.path, AssetType.PublicAsset);
            }
        }
        AssetReference Analysis(string guid, string assetPath, AssetType type)
        {
            Type mainAssetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
            if (IsIgnore(mainAssetType))//排除掉不支持的类型
            {
                return null;
            }
            if (!assetRefenreces.TryGetValue(guid, out AssetReference self))//不在字典中  说明是还没分析过的资源
            {
                self = new AssetReference(guid, assetPath, type);
                assetRefenreces.Add(guid, self);
                foreach (string subassetPath in AssetDatabase.GetDependencies(self.AssetPath, false))//所有的直接依赖项
                {
                    string subguid = AssetDatabase.AssetPathToGUID(subassetPath);
                    AssetReference child = Analysis(subguid, subassetPath, AssetType.AloneAsset);
                    if (child != null)
                    {
                        self.Children.Add(child);
                    }
                }
            }
            else //已经被字典记录了
            {
                if (type == AssetType.PublicAsset)//被VFS指定公开的资源
                {
                    //原本是被其他资源隐式依赖  但是又被VFS公开  升级成公开资源
                    self.Type = AssetType.PublicAsset;
                }
                else
                {
                    if (self.Type == AssetType.AloneAsset)
                    {
                        //被第二个依赖了  升级成共享资源
                        self.Type = AssetType.ShareAsset;
                    }
                }
            }
            return self;
        }
        bool IsIgnore(Type mainAssetType)//忽略的类型
        {
            return mainAssetType == null //missing asset
                || mainAssetType == typeof(MonoScript)
                || mainAssetType == typeof(LightingDataAsset)
                || mainAssetType == typeof(DefaultAsset)
                || mainAssetType == typeof(Shader);//把shader也排除了 暂时使用冗余方式 以后再将着色器变体进行收集 然后打在一个包里面
        }


        public List<AssetReference> GetAllAssets()
        {
            return assetRefenreces.Values.ToList();
        }
    }
}
