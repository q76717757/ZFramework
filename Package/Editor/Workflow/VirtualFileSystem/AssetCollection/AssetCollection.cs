using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace ZFramework.Editor
{
    /// <summary>
    /// 收集VFS上配置的资源
    /// </summary>
    public class AssetCollection
    {
        List<AssetGroup> groups;

        public AssetCollection(IList<VFSTreeElement> elements)
        {
            Collect(elements);
        }

        void Collect(IList<VFSTreeElement> elements)
        {
            AssetGroup defaultBundle = new AssetGroup();
            groups = new List<AssetGroup>() { defaultBundle };
            AssetGroup customBundle = null;
            for (int i = 1, count = elements.Count; i < count; i++)//0是root 跳过
            {
                if (elements[i].depth == 0)//bundle固定第一层  遇到非bundle同时是第一层  可以明确不是bundle
                {
                    if (elements[i].data.IsBundle)
                    {
                        customBundle = new AssetGroup(elements[i].DisplayName);
                        groups.Add(customBundle);
                    }
                    else
                    {
                        customBundle = null;
                    }
                }
                if (elements[i].data.IsAsset || elements[i].data.type == VFSMetaData.MetaType.ReferenceFolderSubAsset)
                {
                    if (customBundle != null)//手动指定的包
                    {
                        customBundle.Datas.Add(elements[i].data);
                    }
                    else//没有指定分包的离散资源
                    {
                        defaultBundle.Datas.Add(elements[i].data);
                    }
                }
            }
        }

        public List<VFSMetaData> GetAllMetaData()
        {
            List<VFSMetaData> allDatas = new List<VFSMetaData>();
            foreach (AssetGroup group in groups)
            {
                allDatas.AddRange(group.Datas);
            }
            return allDatas;
        }
        public AssetGroup[] GetGroups()
        {
            return groups.ToArray();
        }


    }
}
