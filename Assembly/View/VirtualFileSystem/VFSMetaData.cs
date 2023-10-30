using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class VFSMetaData
    {
        public string name;//VFS浏览器上的名字  拼接路径也是使用的这个
        public string fileName;//实际文件的名字 assetbundle读文件是通过文件名读的
        public string guid;

        public string filehash;//文件夹没有
        public string metahash;//文件夹和资源都有   

        [JsonIgnore]
        public bool IsAsset
        {
            get
            {
                return !string.IsNullOrEmpty(filehash);//只有资源有文件hash
            }
        }
        [JsonIgnore]
        public bool IsVirtualFolder //只有虚拟文件夹  没有guid
        {
            get
            {
                return string.IsNullOrEmpty(guid);
            }
        }
        [JsonIgnore]
        public bool IsReferenceFolder //引用文件夹  有guid 没有文件hash 没有matehash 
        {
            get
            {
                return !IsAsset && !IsVirtualFolder;
            }
        }

    }
}
