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
        public static async Task BuildCodeDebug()
        {
            string[] logicFiles = Directory.GetFiles(Define.UnityTempDllDirectory, "Model.*");
            foreach (string file in logicFiles)
            {
                File.Delete(file);
            }
            logicFiles = Directory.GetFiles(Define.UnityTempDllDirectory, "Hotfix.*");
            foreach (string file in logicFiles)
            {
                File.Delete(file);
            }

            await BuildAssembly("Model", new string[] { "../Unity.Model/", }, Array.Empty<string>());

            await BuildAssembly("Hotfix", new string[] { "../Unity.Hotfix/", }, new string[] { Path.Combine(Define.UnityTempDllDirectory, "Model.dll") });

            CopyDllToAssset("Model");
            CopyDllToAssset("Hotfix");
        }

        private static void CopyDllToAssset(string assemblyName)
        {
            string resourcesPath = "Assets/_Bundles/Code/";
            Directory.CreateDirectory(resourcesPath);

            File.Copy(Path.Combine(Define.UnityTempDllDirectory, $"{assemblyName}.dll"), Path.Combine(resourcesPath, $"{assemblyName}.dll.bytes"), true);
            File.Copy(Path.Combine(Define.UnityTempDllDirectory, $"{assemblyName}.pdb"), Path.Combine(resourcesPath, $"{assemblyName}.pdb.bytes"), true);

            AssetDatabase.Refresh();

            AssetImporter assetImporter1 = AssetImporter.GetAtPath($"{resourcesPath}{assemblyName}.dll.bytes");
            assetImporter1.assetBundleName = "code";
            assetImporter1.assetBundleVariant = "unity3d";
            AssetImporter assetImporter2 = AssetImporter.GetAtPath($"{resourcesPath}{assemblyName}.pdb.bytes");
            assetImporter2.assetBundleName = "code";
            assetImporter2.assetBundleVariant = "unity3d";

            AssetDatabase.Refresh();
            Debug.Log($"Bundle {assemblyName} Success!");
        }

        private static async Task BuildAssembly(string assemblyName, string[] codeDirectorys, string[] additionalReferences)
        {
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

            string dllPath = Path.Combine(Define.UnityTempDllDirectory, $"{assemblyName}.dll");
            string pdbPath = Path.Combine(Define.UnityTempDllDirectory, $"{assemblyName}.pdb");
            File.Delete(dllPath);
            File.Delete(pdbPath);
            Directory.CreateDirectory(Define.UnityTempDllDirectory);

            AssemblyBuilder assemblyBuilder = new AssemblyBuilder(dllPath, scripts.ToArray());

            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            assemblyBuilder.buildTarget = EditorUserBuildSettings.activeBuildTarget;
            assemblyBuilder.buildTargetGroup = buildTargetGroup;

            //assemblyBuilder.compilerOptions.CodeOptimization = codeOptimization;//19版本没有 20版本有
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
                Debug.Log("Build Finish:" + assemblyPath);
            };

            if (!assemblyBuilder.Build())
            {
                Debug.LogErrorFormat("Build fail：" + assemblyBuilder.assemblyPath);
            }
            else
            {
                EditorApplication.update += WaitCompiling;
                while (EditorApplication.isCompiling)
                {
                    await Task.Delay(1000);
                }
            }
        }

        static void WaitCompiling() {
            if (EditorApplication.isCompiling)
            {
                EditorUtility.DisplayCancelableProgressBar("编译中", "编译中", 0);
            }
            else
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= WaitCompiling;
            }
        }

        public static async void BuildHotReloadAssembly()//model + hotfix
        {
            await BuildAssembly("Model", new[]
            {
                "../Unity.Model/",
            }, Array.Empty<string>());

            await BuildHotfix();
        }

        public static async Task BuildHotfix()//only hotfix
        {
            string[] logicFiles = Directory.GetFiles(Define.UnityTempDllDirectory, "Hotfix_*");
            foreach (string file in logicFiles)
            {
                File.Delete(file);
            }
            string logicFile = $"Hotfix_{DateTime.Now.Ticks/10000:X2}";//编辑器运行时加载同名的assemble不会返回结果,会返回之前的缓存,导致重载无效

            await BuildAssembly(logicFile, new[]
            {
                "../Unity.Hotfix/",
            }, new[] { Path.Combine(Define.UnityTempDllDirectory, "Model.dll") });//hotfix引用model
        }

    }
}
