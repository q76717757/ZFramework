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
        /// <summary>
        /// 如果是True,则框架会自动执行OnStart方法,否则忽略这个Entry实现,默认为True
        /// </summary>
        public virtual bool IsActivate { get; } = true;
        public abstract void OnStart();
    }
}
