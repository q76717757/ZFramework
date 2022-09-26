using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class EntityRootFunc
    {
        public static void CreateVR(this EntityRoot self, object parm)
        {
            self.VirtualProcess = new Dictionary<int, VirtualProcess>();
        }
    }
}
