using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class BuildAssemblieEditor
    {
        public const string UnityTempDllPath = "Temp/Bin/Debug/";//编译程序集的输出位置
        public const string AssetsSaveDllPath = "Assets/Bundles/Code/";//程序集的保存位置

        public static async void CompileAssembly_Development()
        {
            await BuildAssembly("Model", new[]
            {
                "Codes/Model/",
                "Codes/ViewModel/"
            }, Array.Empty<string>());
            await CompileAssembly_Logic();
        }
        public static async Task<string> CompileAssembly_Logic()//only UnityEditor
        {
            string[] logicFiles = Directory.GetFiles(UnityTempDllPath, "Logic_*");
            foreach (string file in logicFiles)
            {
                File.Delete(file);
            }
            string logicFile = $"Logic_{DateTime.Now.Ticks / 10000:X2}";//编辑器运行时加载同名的assemble不会返回结果,会返回之前的缓存,不会加载新程序集
            await BuildAssembly(logicFile, new[]
            {
                "Codes/Logic/",
                "Codes/ViewLogic/"
            }, new[] {Path.Combine(UnityTempDllPath, "Model.dll") });//hotfix引用model

            return logicFile;
        }

        public static async void CompileAssembly_Debug()
        {
            await BuildAssembly("Code", new string[] { "Codes/" }, Array.Empty<string>(), CodeOptimization.Debug);
            CopyDllToAssset("Code");
        }
        public static async void CompileAssembly_Release()
        {
            await BuildAssembly("Codes", new string[] { "Codes/" }, Array.Empty<string>(), CodeOptimization.Release);
            CopyDllToAssset("Codes");
        }


        private static async Task BuildAssembly(string assemblyName, string[] codeDirectorys, string[] additionalReferences, CodeOptimization codeOptimization = CodeOptimization.Debug)
        {
            //查找外部CS文件
            List<string> scripts = new List<string>();
            for (int i = 0; i < codeDirectorys.Length; i++)
            {
                DirectoryInfo dti = new DirectoryInfo(codeDirectorys[i]);

                FileInfo[] fileInfos = dti.GetFiles("*.cs", System.IO.SearchOption.AllDirectories);
                for (int j = 0; j < fileInfos.Length; j++)
                {
                    scripts.Add(fileInfos[j].FullName);
                }
            }

            //删除旧的
            string dllPath = Path.Combine(UnityTempDllPath, $"{assemblyName}.dll");
            string pdbPath = Path.Combine(UnityTempDllPath, $"{assemblyName}.pdb");
            if (File.Exists(dllPath))
            {
                File.Delete(dllPath);
            }
            if (File.Exists(pdbPath))
            {
                File.Delete(pdbPath);
            }
            Directory.CreateDirectory(UnityTempDllPath);

            //开始编译
            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(dllPath, scripts.ToArray());

            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            assemblyBuilder.buildTarget = EditorUserBuildSettings.activeBuildTarget;
            assemblyBuilder.buildTargetGroup = buildTargetGroup;
            assemblyBuilder.compilerOptions.CodeOptimization = codeOptimization;//19版本没有这项 20版本有
            assemblyBuilder.compilerOptions.ApiCompatibilityLevel = PlayerSettings.GetApiCompatibilityLevel(buildTargetGroup);
            //assemblyBuilder.compilerOptions.AllowUnsafeCode = true;
            assemblyBuilder.additionalReferences = additionalReferences;
            assemblyBuilder.referencesOptions = ReferencesOptions.UseEngineModules;
            assemblyBuilder.flags = AssemblyBuilderFlags.None;
            assemblyBuilder.buildFinished += (assemblyPath, compilerMessages) =>
            {
                if (compilerMessages.Length > 0)
                {
                    for (int i = 0; i < compilerMessages.Length; i++)
                    {
                        if (compilerMessages[i].type == CompilerMessageType.Error)
                        {
                            Debug.LogError($"{compilerMessages[i].message}");
                        }
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
                Debug.Log("Compile Success!");
            }
        }
        private static void CopyDllToAssset(string assemblyName)//从temp复制到assets下
        {
            if (!Directory.Exists(AssetsSaveDllPath))
            {
                Directory.CreateDirectory(AssetsSaveDllPath);
            }

            File.Copy($"{UnityTempDllPath}{assemblyName}.dll", $"{AssetsSaveDllPath}{assemblyName}.dll.bytes", true);
            File.Copy($"{UnityTempDllPath}{assemblyName}.pdb", $"{AssetsSaveDllPath}{assemblyName}.pdb.bytes", true);

            AssetDatabase.Refresh();

            AssetImporter assetImporter1 = AssetImporter.GetAtPath($"{AssetsSaveDllPath}{assemblyName}.dll.bytes");
            assetImporter1.assetBundleName = "code";
            AssetImporter assetImporter2 = AssetImporter.GetAtPath($"{AssetsSaveDllPath}{assemblyName}.pdb.bytes");
            assetImporter2.assetBundleName = "code";

            AssetDatabase.Refresh();
        }

    }
}
