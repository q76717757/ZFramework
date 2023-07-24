using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace ZFramework
{
    public class DomainLoader : IAssemblyLoader
    {
        public Type[] LoadAssembly(string[] assemblyNames)
        {
            List<Type> allTypes = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assemblyNames.Contains(assembly.GetName().Name))
                {
                    allTypes.AddRange(assembly.GetTypes());
                }
            }
            return allTypes.ToArray();
        }
    }
}
