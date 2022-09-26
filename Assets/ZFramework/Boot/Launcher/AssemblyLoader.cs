using System.Reflection;
using System;
using System.IO;
using UnityEngine;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        public const string TempDllPath = ".cache/";//编译程序集的缓存位置
        public static Assembly LoadCode()
        {
#if UNITY_EDITOR
            byte[] dll = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Bundles/Code/ProjectCode_Code.dll.bytes").bytes;
            byte[] pdb = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Bundles/Code/ProjectCode_Code.pdb.bytes").bytes;

#else
            byte[] dll = File.ReadAllBytes(Application.streamingAssetsPath + "/Code.dll");
            byte[] pdb = File.ReadAllBytes(Application.streamingAssetsPath + "/Code.pdb");
#endif
            //通过globalconfig 设置程序集的位置
            var assembly = Assembly.Load(dll, pdb);
            return assembly;
        }

    }

}
