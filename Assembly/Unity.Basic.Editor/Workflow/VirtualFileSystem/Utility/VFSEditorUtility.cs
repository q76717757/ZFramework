using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ZFramework.Editor
{
    internal static class VFSEditorUtility
    {
        //根据guid  刷新信息  主要两个用途  一是初始化的时候填充其余的信息, 二是在资源发生更名|移动后,根据guid修正名称和路径(目前没有做资源监听,所以手动刷新一下)
        internal static VFSMetaData Refresh(this VFSMetaData data)//根据guid  刷新路径信息
        {
            if (data.type == VFSMetaData.MetaType.VirtualFolder || data.type == VFSMetaData.MetaType.Bundle)
            {
                return data;  //虚拟文件夹或者包只有name  没有guid 
            }
            if (string.IsNullOrEmpty(data.Guid))//其他的都是有guid的资产
            {
                throw new Exception("VFS元数据缺少GUID,和当前Type的类型不匹配,说明VFSProfile不是正确的");
            }
            var assetPath = AssetDatabase.GUIDToAssetPath(data.Guid);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (obj == null)//有一个无效的guid 可能是引用文件夹 可能是资产 但是从工程中被删除了
            {
                data.name = $"[missing object]->[{data.Guid}]";
            }
            else
            {
                data.name = obj.name;
                if (data.type == VFSMetaData.MetaType.Unknown) //是新的
                {
                    if (AssetDatabase.IsValidFolder(assetPath))
                    {
                        data.path = null;
                        data.type = VFSMetaData.MetaType.ReferenceFolder;
                    }
                    else
                    {
                        data.path = assetPath;
                        if (obj.GetType() == typeof(SceneAsset))
                        {
                            data.type = VFSMetaData.MetaType.Scene;
                        }
                        else
                        {
                            data.type = VFSMetaData.MetaType.Asset;
                        }
                    }
                }
            }
            return data;
        }
        internal static AssetBundleBuild[] GetAssetBundleBuilds(this BundleManifest manifest)//根据清单内容 构建ab打包的配置
        {
            var output = new List<AssetBundleBuild>();
            foreach (var bundleInfo in manifest.GetBundles())
            {
                var build = new AssetBundleBuild()
                {
                    assetBundleName = bundleInfo.bundleName,
                    assetNames = bundleInfo.assetGuids.Select(x => AssetDatabase.GUIDToAssetPath(x)).ToArray(),
                };
                output.Add(build);
            }
            return output.ToArray();
        }

        
    }
}
