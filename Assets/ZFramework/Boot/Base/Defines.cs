using UnityEngine;

namespace ZFramework
{
    public static class Defines
    {
#if UNITY_EDITOR
        public static string TempDllPath => @".cache\";//编译程序集的缓存位置 (.dll)
        public static string AssetsSaveDllPath => @"Assets\Bundles\Code\";//程序集bytes文件的保存位置 (.ab)
        public static string HideCSPath => @"Assets\ZFramework\.Client\";//实际编译脚本的隐藏目录  (.cs)
        public static string EmptyCSPath => @"Assets\ZFramework\Hotfix\";//程序集定义的目录位置 (.asmdef)
        public static string[] HotfixAssemblyNames => new string[]//程序集的名字
        {
            "Unity.Hotfix.Core",//框架层
            "Unity.Hotfix.Data",//公共数据层   
            "Unity.Hotfix.Func",//公共逻辑层  用data+function的方式 服务端重复加载程序集 重定向函数指针 实现服务端逻辑热重载
            "Unity.Hotfix.View",//客户端视图层
        };
        public static string GetFolder(string assemblyName)//Unity.Hotfix.Core ->  Core 取最后一段为文件名
        {
            return assemblyName.Substring(assemblyName.LastIndexOf('.') + 1);
        }
#endif

        public static string StreamAssetsPath => Application.streamingAssetsPath;//只读目录
        public static string PersistentDataPath => Application.persistentDataPath;//持久化目录

        public static bool IsEditor
        {
            get
            {
#if UNITY_EDITOR
                return true;
#else
                return false;
#endif
            }
        }
    }
}
