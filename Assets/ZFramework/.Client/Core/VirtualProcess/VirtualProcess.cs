using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public class VirtualProcess
    {
        public ProcessType ProcessType { get; set; }
        public Entity RootEntity { get; private set; }

        internal void SetRoot(Entity vpRoot)
        {
            RootEntity = vpRoot;
        }
    }
}
