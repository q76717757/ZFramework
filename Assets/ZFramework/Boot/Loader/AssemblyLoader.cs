using System.Reflection;
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        private static Dictionary<string, Assembly> Assemblys = new Dictionary<string, Assembly>();//程序集缓存
        private static BootFile currentBoot;

        public static BootFile CurrentBoot => currentBoot;

        public static async Task<Assembly> LoadCode(BootFile boot)
        {
            //按照boot的设置  进行程序集加载
            //如果支持热更新的平台  则走文件对比流程 如果不支持热更新的 那也没必要热更资源了 资源变了 一般脚本也有变动

            var dllName = boot.GetDllName();
            if (Assemblys.TryGetValue(dllName, out Assembly assembly))
            {
                currentBoot = boot;
                return assembly;
            }

#if UNITY_EDITOR
            byte[] dll = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"{Defines.AssetsSaveDllPath}{dllName}.dll.bytes").bytes;
            byte[] pdb = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"{Defines.AssetsSaveDllPath}{dllName}.pdb.bytes").bytes;
#else
            byte[] dll = File.ReadAllBytes(Application.streamingAssetsPath + $"/{dllName}.dll");
            byte[] pdb = File.ReadAllBytes(Application.streamingAssetsPath + $"/{dllName}.pdb");
#endif
            assembly = Assembly.Load(dll, pdb);
            Assemblys.Add(dllName, assembly);
            currentBoot = boot;
            return assembly;

            //Pass 1
            //如果不支持更新的平台,则是以插件程序集的方式包进来的,
            //遍历appdomain 找到这个程序集 不再需要加载了 直接返回去给Drive去反射IEntry??
            //var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //foreach (var item in assemblies)
            //{
            //    if (item.GetName().Name == dllName)
            //    {
            //        CurrentDll = dllName;
            //        Assemblys.Add(dllName, assembly);
            //        return item;
            //    }
            //}
            //Pass 2
            //放进来被引用了  直接调??
            //return typeof(GameObject).Assembly

        }
    }

}
