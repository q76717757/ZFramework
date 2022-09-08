using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        public static async Task<IEntry> GetEntry(BootFile boot)
        {
#if UNITY_EDITOR
            return GetEntryDevelopment();
#else
            return await GetEntryRelease(boot);
#endif
        }

        //热重载/开发模式
        static IEntry GetEntryDevelopment()
        {
            var modelAssembly = LoadModel();
            var logicAssembly = LoadLogic();

            //可以考虑反射一个加载器出来 ? 在加载器里面去load文件  这样load文件逻辑也可以热更了
            var entry = modelAssembly.GetType("ZFramework.Game").GetProperty("GameLoop").GetValue(null) as IEntry;//将框架入口反射出来
            entry.Load(modelAssembly, logicAssembly); //然后初始化生命周期和事件系统
            return entry;
        }
        static Assembly LoadModel()
        {
            var pathA = ".cache/Model.dll";
            var pathB = ".cache/Model.pdb";
            var a = File.ReadAllBytes(pathA);
            var b = File.ReadAllBytes(pathB);
            return Assembly.Load(a, b);
        }
        static Assembly LoadLogic()
        {
            string[] hotfixFiles = Directory.GetFiles(".cache/", "Logic_*.dll");
            if (hotfixFiles.Length != 1)
            {
                throw new Exception("logic.dll count != 1");
            }
            string hotfix = Path.GetFileNameWithoutExtension(hotfixFiles[0]);
            var c = File.ReadAllBytes($".cache/{hotfix}.dll");
            var d = File.ReadAllBytes($".cache/{hotfix}.pdb");
            return Assembly.Load(c, d);
        }

        //发布模式  打包成一个热更dll->code.dll  客户端更新要求重启应用
        static async Task<IEntry> GetEntryRelease(BootFile boot)
        {
            var codesAssembly = await LoadCode(boot);

            var entry = codesAssembly.GetType("ZFramework.Game").GetProperty("GameLoop").GetValue(null) as IEntry;
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
