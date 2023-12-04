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
    public static class VFSUtility
    {
        public static VFSMetaData Refresh(this VFSMetaData data)//根据guid  补全其他信息
        {
            if (data.type == VFSMetaData.MetaType.VirtualFolder || data.type == VFSMetaData.MetaType.Bundle)
            {
                //虚拟文件夹或者包只有name  没有guid
                return data;
            }
            if (string.IsNullOrEmpty(data.guid))//其他的都是有guid的资产
            {
                Log.Error("guid is null");
                return data;
            }
            var assetPath = AssetDatabase.GUIDToAssetPath(data.guid);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
            if (obj == null)//is missing asset = 有一个无效的guid  可能是引用文件夹 可能是资产 但是从工程中被删除了
            {
                return data;
            }
            else
            {
                //var metaPath = AssetDatabase.GetTextMetaFilePathFromAssetPath(assetPath);
                var isFolder = AssetDatabase.IsValidFolder(assetPath);
                //var isScene = obj.GetType() == typeof(SceneAsset);
                data.path = isFolder ? null : assetPath;
                //data.hash = isFolder ? null : ComputeHash(assetPath, metaPath);//文件夹没有hash
                if (data.type == VFSMetaData.MetaType.None)
                {
                    data.name = obj.name;
                    data.type = isFolder ? VFSMetaData.MetaType.ReferenceFolder : VFSMetaData.MetaType.Asset;
                }
                if (data.type == VFSMetaData.MetaType.ReferenceFolderSunbFolder || data.type == VFSMetaData.MetaType.ReferenceFolderSubAsset)
                {
                    data.name = obj.name;
                }
            }
            return data;
        }
        static string ComputeHash(string filePath, string metaPath)
        {
            byte[] retVal = MD5Helper.FileMD5ToBytes(filePath);
            byte[] value2 = MD5Helper.FileMD5ToBytes(metaPath);

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                var a = retVal[i];
                var b = value2[i];
                stringBuilder.Append((a ^ b).ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        public static AssetBundleBuild[] GetAssetBundleBuilds(this BundleManifest manifest)//根据清单内容 构建ab打包的配置
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
