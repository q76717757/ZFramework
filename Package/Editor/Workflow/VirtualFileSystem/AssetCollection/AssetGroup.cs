using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework.Editor
{
    /// <summary>
    /// VFS中配置的期望分组
    /// </summary>
    public class AssetGroup
    {
        public AssetGroup() { }
        public AssetGroup(string name)
        {
            Name = name;
        }
        public string Name { get; }
        public List<VFSMetaData> Datas { get; } = new List<VFSMetaData>();
    }
}
