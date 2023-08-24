using System;

namespace ZFramework
{
    interface IAssemblyLoader
    {
        public Type[] LoadAssembly(string[] assemblyNames);
    }
}
