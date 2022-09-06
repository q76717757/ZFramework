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
        public const string UnityTempDllPath = ".cache/";//编译程序集的输出位置
        public const string AssetsSaveDllPath = "Assets/Bundles/Code/";//程序集的保存位置

        public static async void CompileAssembly_Development()
        {
            string projectName = "";
            string tick = (DateTime.Now.Ticks / 10000).ToString("X2");

            await CompileAssembly("Model", new[]
            {
                "Assets/ZFramework/.Code/Data/",
                "Assets/ZFramework/.Code/ViewData/"
            }, Array.Empty<string>());

            await CompileAssembly_Logic(projectName, tick);
        }
        public static async Task<string> CompileAssembly_Logic(string projectName,string modelTick)
        {
            string[] logicFiles = Directory.GetFiles(UnityTempDllPath, "Logic_*");
            foreach (string file in logicFiles)
            {
                File.Delete(file);
            }
            string logicFile = $"Logic_{DateTime.Now.Ticks / 10000:X2}";//不改名重载不了
            await CompileAssembly(logicFile, new[]
            {
                "Assets/ZFramework/.Code/Logic/",
                "Assets/ZFramework/.Code/ViewLogic/"
            }, new[] {Path.Combine(UnityTempDllPath, "Model.dll") });//hotfix引用model

            return logicFile;
        }

        public static async void CompileAssembly_Debug(string projectName)
        {
            await CompileAssembly("Code", new string[] { "Assets/ZFramework/.Code/" }, Array.Empty<string>(), CodeOptimization.Debug);
            CopyDllToAsssetFromTemp("Code");
        }

        private static void CopyDllToAsssetFromTemp(string assemblyName)
        {
            Directory.CreateDirectory(AssetsSaveDllPath);

            File.Copy($"{UnityTempDllPath}{assemblyName}.dll", $"{AssetsSaveDllPath}{assemblyName}.dll.bytes", true);
            File.Copy($"{UnityTempDllPath}{assemblyName}.pdb", $"{AssetsSaveDllPath}{assemblyName}.pdb.bytes", true);

            AssetDatabase.Refresh();

            AssetImporter assetImporter1 = AssetImporter.GetAtPath($"{AssetsSaveDllPath}{assemblyName}.dll.bytes");
            assetImporter1.assetBundleName = "code";
            AssetImporter assetImporter2 = AssetImporter.GetAtPath($"{AssetsSaveDllPath}{assemblyName}.pdb.bytes");
            assetImporter2.assetBundleName = "code";

            AssetDatabase.Refresh();
        }

        private static async Task CompileAssembly(string assemblyName, string[] codeDirectorys, string[] additionalReferences, CodeOptimization codeOptimization = CodeOptimization.Debug)
        {
            //查找外部CS文件
            List<string> scripts = new List<string>();
            for (int i = 0; i < codeDirectorys.Length; i++)
            {
                DirectoryInfo dti = new DirectoryInfo(codeDirectorys[i]);
                if (dti.Exists)
                {
                    FileInfo[] fileInfos = dti.GetFiles("*.cs", System.IO.SearchOption.AllDirectories);
                    for (int j = 0; j < fileInfos.Length; j++)
                    {
                        scripts.Add(fileInfos[j].FullName);
                    }
                }
            }
            Directory.CreateDirectory(UnityTempDllPath);

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
                Debug.Log($"Compile Success!  <color=green>[{assemblyName}]</color>");
            }
        }

    }
}
