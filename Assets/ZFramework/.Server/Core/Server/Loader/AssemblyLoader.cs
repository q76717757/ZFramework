using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        private static AssemblyLoadContext assemblyLoadContext;

        public static Assembly LoadFunction()
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
    }
}
