using System;
using System.Collections.Generic;

namespace ZFramework
{
    public class VPAwake : OnAwakeImpl<VirtualProcessManager>
    {
        public override void OnAwake(VirtualProcessManager self)
        {
            //var child = self.Entity.AddChild();
        }
    }

    public class VirtualProcessManager : Component
    {
        private List<Entity> vps = new List<Entity>();
        private Dictionary<long, Entity> vpEntity = new Dictionary<long, Entity>();


    }
}
