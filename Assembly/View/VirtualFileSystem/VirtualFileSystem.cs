using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace ZFramework
{
    /// <summary>
    /// 虚拟文件系统    
    /// 模拟一个运行时磁盘  要获取某些资源的时候  想从本地磁盘读文件一样  从系统中读文件出来,IO操作由文件系统在底层全部处理完 如远程下载 版本对比 加载依赖等
    /// </summary>
    public class VirtualFileSystem
    {
        public static void Load()
        {
            //从*.vfs文件读取清单
        }

        public static T GetFile<T>(string path)
        {
            return default;
        }
    }

   
}
