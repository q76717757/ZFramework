using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public interface IDispose
    {
        bool IsDisposed { get; }
        internal void BeginDispose(bool isImmediate);
        internal void EndDispose();
    }
}
