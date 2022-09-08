using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using UnityEditor;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        public const string TempDllPath = ".cache/";//编译程序集的输出位置

        public static async Task<IEntry> GetEntry(BootFile boot)
        {

#if UNITY_EDITOR  //开发模式还对比个毛的版本?  直接load最新的
            return GetEntryDevelopment(boot);
#endif


            //版本判断
            var localVer = VersionControl.GetLocalAssemblyVersion();
            var remoteVer = VersionControl.GetRemoteAssemblyVersion();
            if (remoteVer > localVer)
            {
                //有更新就下载新的dll


                //下载完成  修改本地版本号
            }
            //加载dll
            return await GetEntryRelease(boot);
        }

        static IEntry GetEntryDevelopment(BootFile boot)
        {
            //热更core+mono  不管更不更新core 编辑器下都有core
            //发布模式下  在用HyCLR的前提下 core和mono也可以热更(热更mono要求所有挂脚本的都在ab包里面load出来)

            var modelAssembly = LoadModel();
            var logicAssembly = LoadLogic();

            //可以考虑反射一个加载器出来 ? 在加载器里面去load文件  这样load文件逻辑也可以热更了
            var entry = modelAssembly.GetType("ZFramework.Game").GetProperty("GameLoop").GetValue(null) as IEntry;//将框架入口反射出来
            entry.Load(modelAssembly, logicAssembly); //然后初始化生命周期和事件系统
            return entry;
        }
        static Assembly LoadModel()
        {
            var pathA = $"{TempDllPath}Model.dll";
            var pathB = $"{TempDllPath}Model.pdb";
            var a = File.ReadAllBytes(pathA);
            var b = File.ReadAllBytes(pathB);
            return Assembly.Load(a, b);
        }
        static Assembly LoadLogic()
        {
            string[] hotfixFiles = Directory.GetFiles(TempDllPath, "Logic_*.dll");
            if (hotfixFiles.Length != 1)
            {
                throw new Exception("logic.dll count != 1");
            }
            string hotfix = Path.GetFileNameWithoutExtension(hotfixFiles[0]);
            var c = File.ReadAllBytes($"{TempDllPath}{hotfix}.dll");
            var d = File.ReadAllBytes($"{TempDllPath}{hotfix}.pdb");
            return Assembly.Load(c, d);
        }




        static async Task<IEntry> GetEntryRelease(BootFile boot)
        {
            //发布模式下  在用HyCLR的前提下 core和mono也可以热更(热更mono要求所有挂脚本的都在ab包里面load出来)
            //发布模式把全部脚本都编译在一起  只是开发的时候为了和服务器共享代码以及热重载才分的程序集

            //有时候需要在编辑器下测试发布模式的代码  这时候load的程序集是不包括mono和core的 要通过appdomain反射一下 把mono和core反射出来(mono和core有自动事件)

            var codesAssembly = await LoadCode(boot);
            var entry = codesAssembly.GetType("ZFramework.Game").GetProperty("GameLoop").GetValue(null) as IEntry;
            //框架的入口反射出来 然后load dll
            entry.Load(codesAssembly);
            return entry;
        }
        static async Task<Assembly> LoadCode(BootFile boot)
        {
            byte[] dll = File.ReadAllBytes(Application.streamingAssetsPath + "/Code.dll");
            byte[] pdb = File.ReadAllBytes(Application.streamingAssetsPath + "/Code.pdb");
            var assembly = Assembly.Load(dll, pdb);
            return assembly;
        }

    }

}
