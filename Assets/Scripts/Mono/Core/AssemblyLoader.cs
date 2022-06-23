using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        private static Assembly modelAssembly;//数据模型程序集
        private static Assembly hotfixAssembly;//逻辑方法程序集
        private readonly static Dictionary<Type, List<Type>> attributeMap = new Dictionary<Type, List<Type>>();//特性-类型映射表

        private static void BuildAttributeMap(Type[] allTypes)
        {
            attributeMap.Clear();
            foreach (Type classType in allTypes)
            {
                if (classType.IsAbstract)
                {
                    continue;
                }
                object[] attributesObj = classType.GetCustomAttributes(typeof(BaseAttribute), true);
                if (attributesObj.Length == 0)
                {
                    continue;
                }

                foreach (BaseAttribute baseAttribute in attributesObj)
                {
                    Type AttributeType = baseAttribute.AttributeType;
                    if (!attributeMap.ContainsKey(AttributeType))
                    {
                        attributeMap.Add(AttributeType, new List<Type>());
                    }
                    attributeMap[AttributeType].Add(classType);
                }
            }
        }
        public static Type[] GetTypesByAttribute(Type AttributeType)
        {
            if (!attributeMap.ContainsKey(AttributeType))
            {
                attributeMap.Add(AttributeType, new List<Type>());
            }
            return attributeMap[AttributeType].ToArray();
        }


        public static IEntry GetEntry(RunMode mode)
        {
            switch (mode)
            {
                case RunMode.HotReload_EditorRuntime://热重载模式 editor runtime
                    {
                        var a = File.ReadAllBytes(Path.Combine(Define.UnityTempDllDirectory, "Model.dll"));
                        var b = File.ReadAllBytes(Path.Combine(Define.UnityTempDllDirectory, "Model.pdb"));
                        modelAssembly = Assembly.Load(a, b);
                        hotfixAssembly = ReloadHotfixAssembly();

                        return GetEntry();
                    }
                case RunMode.Mono_RealMachine://发布模式
                    {
                        //检查持久化目录
                        //检查发包目录
                        //复制到持久化目录
                        //载入程序集
                        var code = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "Bundle/code.unity3d"));
                        var a = code.LoadAsset<TextAsset>("Model.dll").bytes;
                        var b = code.LoadAsset<TextAsset>("Model.pdb").bytes;
                        modelAssembly = Assembly.Load(a, b);

                        var c = code.LoadAsset<TextAsset>("Hotfix.dll").bytes;
                        var d = code.LoadAsset<TextAsset>("Hotfix.pdb").bytes;
                        hotfixAssembly = Assembly.Load(c, d);

                        return GetEntry();
                    }
            }
            return null;
        }

        private static IEntry GetEntry()
        {
            List<Type> types = new List<Type>();
            types.AddRange(modelAssembly.GetTypes());
            types.AddRange(hotfixAssembly.GetTypes());
            BuildAttributeMap(types.ToArray());
            return Activator.CreateInstance(modelAssembly.GetType("ZFramework.PlayLoop")) as IEntry;
        }
        public static void RunLauncher()
        {
            var met = hotfixAssembly.GetType("ZFramework.Launcher").GetMethod("Start");
            met.Invoke(null, new object[met.GetParameters().Length]);
        }


        public static Assembly ReloadHotfixAssembly()
        {
            string[] hotfixFiles = Directory.GetFiles(Define.UnityTempDllDirectory, "Hotfix_*.dll");
            if (hotfixFiles.Length != 1)
            {
                throw new Exception("hotfix.dll count != 1");
            }
            string hotfix = Path.GetFileNameWithoutExtension(hotfixFiles[0]);
            var c = File.ReadAllBytes(Path.Combine(Define.UnityTempDllDirectory, $"{hotfix}.dll"));
            var d = File.ReadAllBytes(Path.Combine(Define.UnityTempDllDirectory, $"{hotfix}.pdb"));
            hotfixAssembly = Assembly.Load(c, d);

            List<Type> types = new List<Type>();
            types.AddRange(modelAssembly.GetTypes());
            types.AddRange(hotfixAssembly.GetTypes());
            BuildAttributeMap(types.ToArray());
            return hotfixAssembly;
        }

    }

}
