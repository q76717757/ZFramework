using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
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

        static IEntry GetEntryDevelopment()
        {
            var modelAssembly = LoadModel();
            var logicAssembly = LoadLogic();
            var entry = Activator.CreateInstance(modelAssembly.GetType("ZFramework.PlayLoop")) as IEntry;
            entry.Start(modelAssembly, logicAssembly);
            return entry;
        }

        static async Task<IEntry> GetEntryRelease(BootFile boot)
        {
            var codesAssembly = await LoadCode(boot);
            var entry = Activator.CreateInstance(codesAssembly.GetType("ZFramework.PlayLoop")) as IEntry;
            entry.Start(codesAssembly);
            return entry;
        }


        //热重载/开发模式
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
        static async Task<Assembly> LoadCode(BootFile boot)
        {
            //获取远程版本
            //对比本地版本
            //检查本地文件完整性



            //这个要分模式 本地内置模式直接反射(针对不能使用loadAssembly的平台 如UWP,其他平台可以用wolong热更) 远程模式(包含web下载/本地文件夹)
            return null;
        }

    }

}
