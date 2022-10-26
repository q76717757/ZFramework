using System;
using System.Collections.Generic;

namespace ZFramework
{
    internal class VirtualProcessManager
    {
        public Dictionary<string, Type> VPMap = new Dictionary<string, Type>();//类名 -- 类
        public SingletonProcess singleVP;
        public List<VirtualProcess> vps;

        public void Start(Type[] types)
        {
            foreach (var item in types)
            {
                VPMap.Add(item.Name, item);
            }

            ProcessConfig[] configs = ProcessConfigLoader.Load();//客户端直接写死 出一个创建一个Client进程??  服务端从某个地方读配置表 反序列化出来

            singleVP = new SingletonProcess();
            var singleEntity = Entity.Create();
            singleVP.Init(singleEntity, null);
            singleVP.Start();

            vps = new List<VirtualProcess>();
            foreach (var item in configs)
            {
                if (VPMap.TryGetValue(item.processClassName,out Type value))
                {
                    if (Activator.CreateInstance(value) is VirtualProcess vp)
                    {
                        vp.Init(Entity.Create(), item.parms);
                        vps.Add(vp);
                    }
                }
            }

            foreach (var item in vps)
            {
                item.Start();
            }
        }

    }
}
