using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ZFramework
{
    public class VFSMetaData
    {
        public enum MetaType
        {
            None,//未初始化
            Asset,//实际资源
            Bundle,//独立资源包
            VirtualFolder,//虚拟文件夹
            ReferenceFolder,//引用文件夹
            ReferenceFolderSubAsset,//引用文件夹下的资源
            ReferenceFolderSunbFolder,//引用文件夹下的文件夹
        }

        public string guid;//唯一标识  定位用
        public MetaType type;//元数据的类型

        public string name;//VFS浏览器上的名字 拼接路径也是使用的这个 默认和资产名一样 可以自定义
        public string path;//资产路径  Asset/../..
        //public string hash;//混合file和meta的hash  暂时用不上

        public VFSMetaData(MetaType type, string guid = null)
        {
            this.type = type;
            this.guid = guid;
        }

        public bool IsAsset => TypeIs(MetaType.Asset);
        public bool IsBundle => TypeIs(MetaType.Bundle);
        public bool IsVirtualFolder => TypeIs(MetaType.VirtualFolder);
        public bool IsReferenceFolder => TypeIs(MetaType.ReferenceFolder);
        private bool TypeIs(MetaType type) => this.type == type;

    }
}
