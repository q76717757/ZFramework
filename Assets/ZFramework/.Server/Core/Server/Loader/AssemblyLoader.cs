using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class AssemblyLoader
    {
        private static AssemblyLoadContext assemblyLoadContext;

        private static Assembly hotfix;

        public static void Start()
        {
            byte[] dllBytes = File.ReadAllBytes("./Model.dll");
            byte[] pdbBytes = File.ReadAllBytes("./Model.pdb");
            var modelAsselbmy = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

            LoadHotfix();
            //Entry.Start();
        }

        public static void LoadHotfix()
        {
            assemblyLoadContext?.Unload();
            GC.Collect();
            assemblyLoadContext = new AssemblyLoadContext("Hotfix", true);
            byte[] dllBytes = File.ReadAllBytes("./Hotfix.dll");
            byte[] pdbBytes = File.ReadAllBytes("./Hotfix.pdb");
            hotfix = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

            //Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof(Init).Assembly, typeof(Game).Assembly, typeof(Entry).Assembly, this.hotfix);
            //EventSystem.Instance.Add(types);
        }
    }
}
