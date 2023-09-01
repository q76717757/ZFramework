using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace ZFramework
{
    public class AssemblyLoader
    {
        private AssemblyLoadContext assemblyLoadContext;

        public Assembly LoadFunction()
        {
            if (assemblyLoadContext != null)
            {
                assemblyLoadContext.Unload();
                GC.Collect();
            }

            assemblyLoadContext = new AssemblyLoadContext("HotReloadFunction", true);
            byte[] dllBytes = File.ReadAllBytes("./Server.Func.dll");
            byte[] pdbBytes = File.ReadAllBytes("./Server.Func.pdb");
            return assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));
        }

        public Type[] LoadAssembly(string[] assemblyNames)
        {
            List<Type> output = new List<Type>();
            foreach (string assembly in assemblyNames)
            {
                FileInfo fi = new FileInfo($"./{assembly}.dll");
                if (fi.Exists)
                {
                    Assembly assemblyInstance = Assembly.LoadFrom(fi.FullName);
                    Type[] types = assemblyInstance.GetTypes();
                    output.AddRange(types);
                    Log.Info("LoadAssembly:" + assembly);
                }
                else
                {
                    Log.Info(fi.FullName + "不存在");
                }
            }
            return output.ToArray();
        }
    }
}
