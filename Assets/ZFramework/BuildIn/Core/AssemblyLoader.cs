using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        public static IEntry GetEntry(BootFile boot)
        {
#if UNITY_EDITOR
            return GetEntryDevelopment();
#else
            return GetEntryRelease();
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

        static IEntry GetEntryRelease(BootFile boot)
        {
            var codesAssembly = LoadCode(boot);
            var entry = Activator.CreateInstance(codesAssembly.GetType("ZFramework.PlayLoop")) as IEntry;
            entry.Start(codesAssembly);
            return entry;
        }


        //热重载/开发模式 分2个dll进行加载
        static Assembly LoadModel()
        {
            var pathA = "Temp/Bin/Debug/Model.dll";
            var pathB = "Temp/Bin/Debug/Model.pdb";
            var a = File.ReadAllBytes(pathA);
            var b = File.ReadAllBytes(pathB);
            return Assembly.Load(a, b);
        }
        static Assembly LoadLogic()
        {
            string[] hotfixFiles = Directory.GetFiles("Temp/Bin/Debug/", "Logic_*.dll");
            if (hotfixFiles.Length != 1)
            {
                throw new Exception("logic.dll count != 1");
            }
            string hotfix = Path.GetFileNameWithoutExtension(hotfixFiles[0]);
            var c = File.ReadAllBytes($"Temp/Bin/Debug/{hotfix}.dll");
            var d = File.ReadAllBytes($"Temp/Bin/Debug/{hotfix}.pdb");
            return Assembly.Load(c, d);
        }


        //发布模式  打包成一个热更dll->code.dll  客户端更新要求重启应用
        static Assembly LoadCode(BootFile boot)
        {
#if UNITY_EDITOR
            var dll = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Bundles/Code/Code.dll.bytes");
            var pdb = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Bundles/Code/Code.pdb.bytes");
             return Assembly.Load(dll.bytes, pdb.bytes);
#else
            var dll = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "Code.dll.bytes"));
            var pdb = File.ReadAllBytes(Path.Combine(Application.streamingAssetsPath, "Code.pdb.bytes"));
            return Assembly.Load(dll, pdb);
#endif
            //这个要分模式 本地内置模式直接反射(针对不能使用loadAssembly的平台 如UWP,其他平台可以用wolong热更) 远程模式(包含web下载/本地文件夹)

        }

    }

}
