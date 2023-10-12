using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal class EntryAttribute : BaseAttribute
    {
    }

    [Entry]
    public abstract class Entry
    {
        public abstract void OnStart();
    }
}
