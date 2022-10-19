using System;
using System.Collections.Generic;

namespace ZFramework
{
    internal class VirtualProcessManager
    {
        private readonly Dictionary<long, VirtualProcess> vps = new Dictionary<long, VirtualProcess>();

        internal void Init(ProcessConfig[] configs)//根据配置表   建立本实例的虚拟进程
        {
            foreach (var config in configs)
            {
                var vp = new VirtualProcess();
                vps.Add(0, vp);
                //vp.rootEntity = new Entity(vp);
            }
        }
    }
}
