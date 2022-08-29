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
    public class AssemblyLoader
    {
        private AssemblyLoadContext assemblyLoadContext;

        private Assembly hotfix;

        public void Start()
        {
            byte[] dllBytes = File.ReadAllBytes("./Model.dll");
            byte[] pdbBytes = File.ReadAllBytes("./Model.pdb");
            var modelAsselbmy = AssemblyLoadContext.Default.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

            this.LoadHotfix();
            //Entry.Start();
        }

        public void LoadHotfix()
        {
            assemblyLoadContext?.Unload();
            GC.Collect();
            assemblyLoadContext = new AssemblyLoadContext("Hotfix", true);
            byte[] dllBytes = File.ReadAllBytes("./Hotfix.dll");
            byte[] pdbBytes = File.ReadAllBytes("./Hotfix.pdb");
            this.hotfix = assemblyLoadContext.LoadFromStream(new MemoryStream(dllBytes), new MemoryStream(pdbBytes));

            //Dictionary<string, Type> types = AssemblyHelper.GetAssemblyTypes(typeof(Init).Assembly, typeof(Game).Assembly, typeof(Entry).Assembly, this.hotfix);
            //EventSystem.Instance.Add(types);
        }
    }
}
