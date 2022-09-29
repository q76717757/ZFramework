using System.Reflection;
using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        public static Dictionary<string, Assembly> Assemblys;//程序集缓存
        public static async Task<Assembly> LoadCode(BootFile boot)
        {
            var dllName = boot.ProjectCode;
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
            var output = Assembly.Load(dll, pdb);
            Assemblys.Add(dllName, output);
            return assembly;
        }
    }

}
