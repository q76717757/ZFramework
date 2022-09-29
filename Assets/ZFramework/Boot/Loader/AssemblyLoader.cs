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
        public static Dictionary<string, Assembly> Assemblys = new Dictionary<string, Assembly>();//程序集缓存
        public static async Task<Assembly> LoadCode(BootFile boot)
        {
            var dllName = $"{boot.ProjectCode}_{boot.AssemblyVersion}";
            if (Assemblys.TryGetValue(dllName,out Assembly assembly))
            {
                return assembly;
            }

#if UNITY_EDITOR
            byte[] dll = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"{Defines.AssetsSaveDllPath}{dllName}.dll.bytes").bytes;
            byte[] pdb = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>($"{Defines.AssetsSaveDllPath}{dllName}.pdb.bytes").bytes;
#else
            byte[] dll = File.ReadAllBytes(Application.streamingAssetsPath + "/Code.dll");
            byte[] pdb = File.ReadAllBytes(Application.streamingAssetsPath + "/Code.pdb");
#endif

            assembly = Assembly.Load(dll, pdb);
            Assemblys.Add(dllName, assembly);
            return assembly;
        }
    }

}
