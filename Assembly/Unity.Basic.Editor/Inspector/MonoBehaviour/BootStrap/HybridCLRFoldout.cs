using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor.PackageManager.Requests;
using System.Linq;
using System;
using UnityEditorInternal;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Text;

#if ENABLE_HYBRIDCLR
using HybridCLR.Editor.Commands;
using HybridCLR.Editor;
using HybridCLR.Editor.Installer;
using HybridCLR.Editor.Settings;
#endif

namespace ZFramework.Editor
{
    public partial class HybridCLRFoldout : FadeFoldout
    {
        public override string Title => $"代码热更新({Defines.TargetRuntimePlatform})";

        static string gitURL = "https://gitee.com/focus-creative-games/hybridclr_unity.git";
        static string packageVersion = "4.0.12";
        static AddRequest addRequest;
        static IFileServer fileServer;

        protected override void OnGUI()
        {
            if (fileServer == null)
            {
                fileServer = new CosFileServer();
            }

            DrawDocLink();

#if ENABLE_HYBRIDCLR
            OnEnableGUI();
#else
            OnDisableGUI();
#endif
        }

        void DrawDocLink()
        {
            EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(true));

            EditorGUILayout.LabelField($"热更新方案:HybridCLR {packageVersion}", EditorStyles.centeredGreyMiniLabel);
            Rect linkRect =  EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, EditorStyles.linkLabel,GUILayout.ExpandWidth(true));
            EditorGUIUtility.AddCursorRect(linkRect, MouseCursor.Link);
            if(GUI.Button(linkRect,$"官方文档: https://hybridclr.doc.code-philosophy.com",EditorStyles.linkLabel))
            {
                Application.OpenURL("https://hybridclr.doc.code-philosophy.com/docs/intro");
            }

            EditorGUILayout.EndVertical();
        }

        void OnDisableGUI()
        {
            EditorGUILayout.HelpBox("HybridCLR Package(未安装)", MessageType.Error);
            bool isInstalling = addRequest != null && addRequest.Status == StatusCode.InProgress;
            EditorGUI.BeginDisabledGroup(isInstalling);
            if (GUILayout.Button(isInstalling ? "安装中..." : "点击从Package Manager安装"))
            {
                addRequest = Client.Add($"{gitURL}#v{packageVersion}");
            }
            EditorGUI.EndDisabledGroup();
        }
    }


#if ENABLE_HYBRIDCLR
    public partial class HybridCLRFoldout
    {
        void OnEnableGUI()
        {
            if (SettingsIsCorrect())
            {
                DrawAssemblySettings();
                PackagingProcess();
            }
        }

        bool SettingsIsCorrect()
        { 
            return PackageVersion() && IsFullInstall() && IsEnable() && IsSupported() && IsIL2CPP() && IsDotNet4X() && CheckGC();//判断是有顺序的;
        }
        bool PackageVersion()
        {
            var installVersion = new InstallerController().PackageVersion;
            bool version = installVersion == packageVersion;
            if (!version)//当前安装的package版本和指定的版本不一致
            {
                EditorGUILayout.HelpBox($"当前HybridCLER的安装版本和设定的版本不一致\r\n当前版本:{installVersion}\r\n目标版本:{packageVersion}", MessageType.Error);
                bool isInstalling = addRequest != null && addRequest.Status == StatusCode.InProgress;
                EditorGUI.BeginDisabledGroup(isInstalling);
                if (GUILayout.Button(isInstalling ? "安装中..." : "更换为指定版本"))
                {
                    addRequest = Client.Add($"{gitURL}#v{packageVersion}");
                }
                EditorGUI.EndDisabledGroup();
            }
            return version;
        }
        bool IsFullInstall()
        {
            var installer = new InstallerController();
            bool isFull = installer.HasInstalledHybridCLR();
            string ilbil2cppVer = installer.InstalledLibil2cppVersion;
            if (!isFull || string.IsNullOrEmpty(ilbil2cppVer))//未完整安装
            {
                EditorGUILayout.HelpBox("HybridCLR分为3部分\r\n1.Package(已安装)\r\n2.HybrldCLR(未安装)\r\n3.Il2CPP Plus(未安装)", MessageType.Error);
                if (GUILayout.Button("继续完成安装(需要有git环境)"))
                {
                    installer.InstallDefaultHybridCLR();
                }
            }
            return isFull;
        }
        bool IsEnable()
        {
            if (SettingsUtil.Enable)
            {
                return true;
            }
            else
            {
                EditorGUILayout.HelpBox("HybridCLR是关闭的,如启用代码热更流程,需要将HybridCLR开启", MessageType.Error);
                if (GUILayout.Button("启用HybridCLR"))
                {
                    SettingsUtil.Enable = true;
                }
                return false;
            }
        }
        bool IsSupported()
        {
            bool isUnsupport = Defines.TargetRuntimePlatform == Defines.PlatformType.Unsupported;
            if (isUnsupport)
            {
                EditorGUILayout.HelpBox("当前的目标运行时不被支持,请关闭代码热更新流程", MessageType.Error);
                if (GUILayout.Button("点击移除HybridCLR"))
                {
                }
            }
            return !isUnsupport;
        }
        bool IsIL2CPP()
        {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            var isILCPP = PlayerSettings.GetScriptingBackend(group) == ScriptingImplementation.IL2CPP;
            if (!isILCPP)
            {
                EditorGUILayout.HelpBox("HybridCLR是基于IL2CPP的,需要将编译后端改为IL2CPP", MessageType.Error);
                if (GUILayout.Button("切换到IL2CPP"))
                {
                    PlayerSettings.SetScriptingBackend(group, ScriptingImplementation.IL2CPP);
                }
            }
            return isILCPP;
        }
        bool IsDotNet4X()
        {
            var group = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
#if UNITY_2021_1_OR_NEWER
            var targetAPILevel = ApiCompatibilityLevel.NET_Unity_4_8;
#else
            var targetAPILevel = ApiCompatibilityLevel.NET_4_6;
#endif
            var isDotNetFramework = PlayerSettings.GetApiCompatibilityLevel(group) == targetAPILevel;
            if (!isDotNetFramework)
            {
                EditorGUILayout.HelpBox("HybridCLR要求目标框架APILevel需要是.Net Framework", MessageType.Error);
                if (GUILayout.Button("切换.Net Framework"))
                {
                    PlayerSettings.SetApiCompatibilityLevel(group, targetAPILevel);
                }
            }
            return isDotNetFramework;
        } 
        bool CheckGC()
        {
            if (PlayerSettings.gcIncremental)
            {
                EditorGUILayout.HelpBox("增量GC还在Beta中,如果出现问题可以先关闭增量GC", MessageType.Warning);
                if (GUILayout.Button("关闭增量GC功能"))
                {
                    PlayerSettings.gcIncremental = false;
                }
            }
            return true;
        }

        void DrawAssemblySettings()
        {
            Rect rect = EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("热更新程序集", EditorStyles.centeredGreyMiniLabel);

            if (GUILayout.Button("编译+打包热更程序集"))
            {
                CompileDllCommand.CompileDll(EditorUserBuildSettings.activeBuildTarget);
                PackagingHotfixAssembly();
            }
            if (GUILayout.Button("上传热更程序集"))
            {
                UploadHotfixBundle();
            }
            EditorGUILayout.EndVertical();

            CheckAssemblyConsistency();
        }

        void CheckAssemblyConsistency()
        {
            string[] assemblyNameValues = Defines.AssemblyNames.ToArray();

            List<string> clr = new List<string>();
            if (HybridCLRSettings.Instance.hotUpdateAssemblies != null)
                clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblies);
            if (HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions != null)
                clr.AddRange(HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions.Select((a) => a == null ? "" : a.name));

            if (!ComparisonArray(clr, assemblyNameValues))
            {
                EditorGUILayout.HelpBox("热更程序集列表与HybridCLR不一致", MessageType.Error);
                if (GUILayout.Button("同步至HybridCLR Settings"))
                {
                    SyncSettings(assemblyNameValues);
                }
            }
            //TODO  fix bug 不会自动同步 可能是对比逻辑有问题  手动点击可以重置
        }
        void SyncSettings(string[] assemblyNameValues)
        {
            HybridCLRSettings.Instance.hotUpdateAssemblyDefinitions = new AssemblyDefinitionAsset[0];
            HybridCLRSettings.Instance.hotUpdateAssemblies = assemblyNameValues;
            HybridCLRSettings.Save();
        }
        bool ComparisonArray(IEnumerable<string> item1, IEnumerable<string> item2)
        {
            if (item1 == item2) return true;
            if (item1 == null || item2 == null) return false;

            var enum1 = item1.GetEnumerator();
            var enum2 = item2.GetEnumerator();
            while (true)
            {
                if (enum1.MoveNext() && enum2.MoveNext())
                {
                    if (enum1.Current != enum2.Current)
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        void PackagingProcess()
        {
            EditorGUILayout.BeginVertical("GroupBox", GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField($"打包流程", EditorStyles.centeredGreyMiniLabel);

            //part 1
            EditorGUILayout.LabelField("步骤一:执行必要的生成操作", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("1.编译程序集;2.生成IL2CPP宏;3.生成Link.xml;4.导出工程获取AOT程序集;5.生成桥接函数;6.生成反向平台调用包装;7.生成AOT启发文档", MessageType.Info);
            if (GUILayout.Button("一键生成"))
            {
                PrebuildCommand.GenerateAll();
                GUIUtility.ExitGUI();//解决一个报错,导出工程后GUI不能闭合,中断本次GUI执行即可
            }

            //part 2
            EditorGUILayout.LabelField("步骤二:添加AOT引用(可选)", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("根据启发文档的提示手动添加AOT引用,再重新编译AOT可以提升性能", MessageType.Warning);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("自动生成的启发文档");
            EditorGUI.BeginDisabledGroup(true);
            MonoScript aotGeneric = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/" + HybridCLRSettings.Instance.outputAOTGenericReferenceFile);
            EditorGUILayout.ObjectField(aotGeneric, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("手动添加AOT的引用");
            EditorGUI.BeginDisabledGroup(true);
            MonoScript aorRef = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/ZFramework/Assembly/Boot/HybridCLR/AOTReferences.cs");
            EditorGUILayout.ObjectField(aorRef, typeof(MonoScript), false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("重新生成"))
            {
                PrebuildCommand.GenerateAll();
                GUIUtility.ExitGUI();
            }

            //part 3
            EditorGUILayout.LabelField("步骤三:打包AOT程序集并复制到StreamAssets", EditorStyles.boldLabel);
            if (GUILayout.Button("打包AOT程序集"))
            {
                //HybridCLR会生成一个启发文档,反射启发文档的内容自动填充进去
                Assembly assembly_csharp = AppDomain.CurrentDomain.GetAssemblies().First((assembly) => assembly.GetName().Name == "Assembly-CSharp");
                if (assembly_csharp != null)
                {
                    Type type = assembly_csharp.GetType("AOTGenericReferences");
                    if (type != null)
                    {
                        FieldInfo property = type.GetField("PatchedAOTAssemblyList");
                        if (property != null && property.GetValue(null) is IReadOnlyList<string> values)
                        {
                            PackagingMetaAssembly(values.ToArray());
                        }
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
        void PackagingMetaAssembly(string[] aotList)
        {
            //复制aot源文件到工程内
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = $"Assets/07.Bundles/{Defines.TargetRuntimePlatform}/MetaAssemblyCache";//TODO  到处都有路径设置 将配置统一抽到一个公共的类上
            Directory.CreateDirectory(aotAssembliesDstDir);
            List<string> assemblyNames = new List<string>();
            foreach (string dll in aotList)
            {
                Log.Info("Meta Assembly ->" + dll);
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}";
                if (!File.Exists(srcDllPath))
                {
                    throw new FileNotFoundException(srcDllPath);
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                assemblyNames.Add(dllBytesPath);
            }
            AssetDatabase.Refresh();

            //打ab包
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = HybridCLRUtility.META_ASSEMBLY_BUNDLE;
            build.assetNames = assemblyNames.ToArray();
            BuildPipeline.BuildAssetBundles(aotAssembliesDstDir, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DisableWriteTypeTree, target);

            //删除aot源文件
            for (int i = 0; i < assemblyNames.Count; i++)
            {
                AssetDatabase.DeleteAsset(assemblyNames[i]);
            }

            //复制到随包目录下
            Directory.CreateDirectory(Path.Combine(Defines.BuildInAssetAPath, HybridCLRUtility.DATA_FOLDER));
            string readPath = $"{aotAssembliesDstDir}/{HybridCLRUtility.META_ASSEMBLY_BUNDLE}";
            string writePath = Path.Combine(Defines.BuildInAssetAPath, HybridCLRUtility.DATA_FOLDER, HybridCLRUtility.META_ASSEMBLY_BUNDLE);
            File.Copy(readPath, writePath, true);

            AssetDatabase.Refresh();
            Log.Info("Meta Assembly Bundle Build Success");
        }


        void PackagingHotfixAssembly()
        {
            //复制dll源文件到缓存路径
            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            string aotAssembliesSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            string aotAssembliesDstDir = $"Assets/07.Bundles/{Defines.TargetRuntimePlatform}/HotfixAssemblyCache";
            Directory.CreateDirectory(aotAssembliesDstDir);
            List<string> assemblyNames = new List<string>();
            List<string> names = Defines.AssemblyNames.ToList();
            foreach (var dll in names)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
                if (!File.Exists(srcDllPath))
                {
                    throw new FileNotFoundException(srcDllPath);
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.dll.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                assemblyNames.Add(dllBytesPath);
            }
            AssetDatabase.Refresh();

            //打ab包到缓存路径
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = HybridCLRUtility.HOTFIX_ASSEMBLY_BUNDLE;
            build.assetNames = assemblyNames.ToArray();
            AssetBundleManifest aotabm = BuildPipeline.BuildAssetBundles(aotAssembliesDstDir, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.ForceRebuildAssetBundle | BuildAssetBundleOptions.UncompressedAssetBundle | BuildAssetBundleOptions.DisableWriteTypeTree, target);
                                                                          
            //删除jit源文件
            for (int i = 0; i < assemblyNames.Count; i++)
            {
                AssetDatabase.DeleteAsset(assemblyNames[i]);
            }
            //复制到随包目录下
            Directory.CreateDirectory(Path.Combine(Defines.BuildInAssetAPath, HybridCLRUtility.DATA_FOLDER));
            var dllReadPath = Path.Combine(aotAssembliesDstDir, HybridCLRUtility.HOTFIX_ASSEMBLY_BUNDLE);
            var dllWritePath = Path.Combine(Defines.BuildInAssetAPath, HybridCLRUtility.DATA_FOLDER, HybridCLRUtility.HOTFIX_ASSEMBLY_BUNDLE);
            File.Copy(dllReadPath, dllWritePath, true);

            //生成xml记录文件
            var xml = new XmlDocument();
            var element = xml.CreateElement("AssemblyInfo");
            var time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            element.SetAttribute("Time", time);

            var length = new FileInfo(dllWritePath).Length.ToString();
            element.SetAttribute("Length", length);

            var md5 = MD5Helper.FileMD5(dllWritePath);
            element.SetAttribute("MD5", md5);

            xml.AppendChild(element);

            var xmlPath = Path.Combine(Defines.BuildInAssetAPath, HybridCLRUtility.DATA_FOLDER, HybridCLRUtility.HOTFIX_ASSEMBLY_XML);
            var xmlString = xml.FormatString();
            File.WriteAllText(xmlPath, xmlString, new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Log.Info("Hotfix Assembly Bundle Build Success");
            Log.Info(xmlString);
        }
        void UploadHotfixBundle()
        {
            string cosPath = $"{Defines.PROJECT_CODE}/{Defines.TargetRuntimePlatform}/{HybridCLRUtility.DATA_FOLDER}";
            //上传xml
            var xmlBytes = File.ReadAllBytes(Path.Combine(Defines.BuildInAssetAPath, HybridCLRUtility.DATA_FOLDER, HybridCLRUtility.HOTFIX_ASSEMBLY_XML));
            var xmlPath = $"{cosPath}/{HybridCLRUtility.HOTFIX_ASSEMBLY_XML}";
            fileServer.UploadFile(xmlPath, xmlBytes).Invoke();
            //上传assetbundle
            var dllReadPath = Path.Combine(Defines.BuildInAssetAPath, HybridCLRUtility.DATA_FOLDER, HybridCLRUtility.HOTFIX_ASSEMBLY_BUNDLE);
            var dllBytes = File.ReadAllBytes(dllReadPath);
            var dllWritePath =  $"{cosPath}/{HybridCLRUtility.HOTFIX_ASSEMBLY_BUNDLE}";
            fileServer.UploadFile(dllWritePath, dllBytes).Invoke();
        }

    }
#endif
}