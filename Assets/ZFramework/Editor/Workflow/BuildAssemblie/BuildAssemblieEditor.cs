using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class BuildAssemblieEditor
    {
        public const string AssetsSaveDllPath = "Assets/Bundles/Code/";//程序集的保存位置
        public const string HideCSPath = "Assets/ZFramework/.Code/";//实际编译脚本的隐藏目录

        public static async void CompileAssembly_Development(string projectCode, CodeOptimization codeOptimization)
        {
            string assemblyName = $"{projectCode}_Code";
            await CompileAssembly(assemblyName, new string[] { HideCSPath }, Array.Empty<string>(), codeOptimization);
            CopyDllToAsssetFromTemp(assemblyName);
        }

        private static void CopyDllToAsssetFromTemp(string assemblyName)
        {
            Directory.CreateDirectory(AssetsSaveDllPath);

            File.Copy($"{AssemblyLoader.TempDllPath}{assemblyName}.dll", $"{AssetsSaveDllPath}{assemblyName}.dll.bytes", true);
            File.Copy($"{AssemblyLoader.TempDllPath}{assemblyName}.pdb", $"{AssetsSaveDllPath}{assemblyName}.pdb.bytes", true);

            AssetDatabase.Refresh();
             
            AssetImporter assetImporter1 = AssetImporter.GetAtPath($"{AssetsSaveDllPath}{assemblyName}.dll.bytes");
            assetImporter1.assetBundleName = "code";
            AssetImporter assetImporter2 = AssetImporter.GetAtPath($"{AssetsSaveDllPath}{assemblyName}.pdb.bytes");
            assetImporter2.assetBundleName = "code";

            AssetDatabase.Refresh();
        }
        private static async Task CompileAssembly(string assemblyName, string[] codeDirectorys, string[] additionalReferences, CodeOptimization codeOptimization)
        {
            //查找外部CS文件
            List<string> scripts = new List<string>();
            for (int i = 0; i < codeDirectorys.Length; i++)
            {
                DirectoryInfo dti = new DirectoryInfo(codeDirectorys[i]);
                if (dti.Exists)
                {
                    FileInfo[] fileInfos = dti.GetFiles("*.cs", SearchOption.AllDirectories);
                    for (int j = 0; j < fileInfos.Length; j++)
                    {
                        scripts.Add(fileInfos[j].FullName);
                    }
                }
            }
            Directory.CreateDirectory(AssemblyLoader.TempDllPath);

            //删除旧的
            string dllPath = Path.Combine(AssemblyLoader.TempDllPath, $"{assemblyName}.dll");
            string pdbPath = Path.Combine(AssemblyLoader.TempDllPath, $"{assemblyName}.pdb");
            if (File.Exists(dllPath))
            {
                File.Delete(dllPath);
            }
            if (File.Exists(pdbPath))
            {
                File.Delete(pdbPath);
            }

            //开始编译
            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(dllPath, scripts.ToArray());
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            assemblyBuilder.buildTarget = EditorUserBuildSettings.activeBuildTarget;
            assemblyBuilder.buildTargetGroup = buildTargetGroup;
            assemblyBuilder.compilerOptions.CodeOptimization = codeOptimization;
            assemblyBuilder.compilerOptions.ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup);
            //assemblyBuilder.compilerOptions.AllowUnsafeCode = true;
            assemblyBuilder.additionalReferences = additionalReferences;
            assemblyBuilder.referencesOptions = ReferencesOptions.UseEngineModules;
            assemblyBuilder.flags = AssemblyBuilderFlags.None;
            assemblyBuilder.buildFinished += (assemblyPath, compilerMessages) =>
            {
                for (int i = 0; i < compilerMessages.Length; i++)
                {
                    if (compilerMessages[i].type == CompilerMessageType.Error)
                    {
                        Debug.LogError($"{compilerMessages[i].message}");
                    }
                }
            };

            if (!assemblyBuilder.Build())
            {
                Debug.LogErrorFormat("Compile Fail：" + assemblyBuilder.assemblyPath);
            }
            else
            {
                while (EditorApplication.isCompiling)
                {
                    await Task.Delay(100);
                }
                Debug.Log($"Compile Success!  <color=green>[{assemblyName}]</color>");
            }
        }

    }
}
