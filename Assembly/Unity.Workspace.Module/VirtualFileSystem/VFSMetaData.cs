using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace ZFramework
{
    internal class VFSMetaData
    {
        internal enum MetaType
        {
            Unknown = 0,//新创建的
            Asset,//普通资源
            Scene,//场景资源

            Bundle,//独立资源包
            VirtualFolder,//虚拟文件夹

            ReferenceFolder,//引用文件夹
            ReferenceAsset,//引用文件夹下的普通资源
            ReferenceScene,//引用文件夹下的场景资源
            ReferenceSubFolder,//引用文件夹下的文件夹
        }
        public string Guid { get; }//唯一标识  定位用
        public MetaType type;//元数据的类型

        public string name;//VFS浏览器上的名字 拼接路径也是使用的这个
        public string path;//资产路径  Asset/../..
        public bool toggle;//预下载

        public VFSMetaData(MetaType type)
        {
            Guid = null;
            this.type = type;
        }
        public VFSMetaData(string guid)
        {
            Guid = guid;
            type = MetaType.Unknown;
        }
        public VFSMetaData(string guid, MetaType type)
        {
            Guid = guid;
            this.type = type;
        }

        public bool IsAsset => type == MetaType.Asset || type == MetaType.ReferenceAsset;
        public bool IsScene => type == MetaType.Scene || type == MetaType.ReferenceScene;
        public bool IsBundle => type == MetaType.Bundle;
        public bool IsFolder => 
            type == MetaType.VirtualFolder || 
            type == MetaType.ReferenceFolder || 
            type == MetaType.ReferenceSubFolder;
        public bool IsReferences =>
            type == MetaType.ReferenceAsset ||
            type == MetaType.ReferenceScene ||
            type == MetaType.ReferenceSubFolder;
    }
}
