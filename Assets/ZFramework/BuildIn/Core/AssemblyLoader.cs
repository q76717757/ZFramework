using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        public static IEntry GetEntry(CompileMode mode)
        {
#if !UNITY_EDITOR
            mode = CompileMode.Release;
#endif
            try
            {
                switch (mode)
                {
                    case CompileMode.Development:
                        {
                            var modelAssembly = LoadModel();
                            var logicAssembly = LoadLogic();

                            var entry = Activator.CreateInstance(modelAssembly.GetType("ZFramework.PlayLoop")) as IEntry;
                            entry.Start(modelAssembly, logicAssembly);
                            return entry;
                        }
                    case CompileMode.Release:
                        {
                            var codesAssembly = LoadCode();
                            var entry = Activator.CreateInstance(codesAssembly.GetType("ZFramework.PlayLoop")) as IEntry;
                            entry.Start(codesAssembly);
                            return entry;
                        }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
            return null;
        }

        public static Assembly LoadModel()
        {
            var pathA = "Temp/Bin/Debug/Model.dll";
            var pathB = "Temp/Bin/Debug/Model.pdb";
            var a = File.ReadAllBytes(pathA);
            var b = File.ReadAllBytes(pathB);
            return Assembly.Load(a, b);
        }
        public static Assembly LoadLogic()
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

        static Assembly LoadCode()
        {
#if UNITY_EDITOR
            var dll = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Bundles/Code/Code.dll.bytes");
            var pdb = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Bundles/Code/Code.pdb.bytes");
            return Assembly.Load(dll.bytes, pdb.bytes);
#endif
            //判断本地目录有没有缓存
            //没有就去默认目录直接下载最新dll
             
            //var code = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "Bundle/code.unity3d"));
            //var a = code.LoadAsset<TextAsset>("Model.dll").bytes;
            //var b = code.LoadAsset<TextAsset>("Model.pdb").bytes;
            //modelAssembly = Assembly.Load(a, b);
        }
    }

}
