using System;
using System.Collections.Generic;

namespace ZFramework
{
    internal class VPAwake : OnAwakeImpl<VirtualProcessManager,Entity>
    {
        public override void OnAwake(VirtualProcessManager self,Entity root)
        {
            Log.Info("CreateProcess");//根据配置

            var e = root.AddChild();
            var vp = new VirtualProcess();
            vp.SetRoot(e);

            EventSystem.Call(new EventType.LoadAssemblyFinish());
        }
    }

    internal class VirtualProcessManager : SingleComponent<VirtualProcessManager>
    {
        private List<Entity> vps = new List<Entity>();
        private Dictionary<long, Entity> vpEntity = new Dictionary<long, Entity>();
    }
}
