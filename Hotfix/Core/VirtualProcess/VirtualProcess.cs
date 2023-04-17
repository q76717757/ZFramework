using System;
using System.Collections.Generic;

namespace ZFramework
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ProcessType : BaseAttribute
    {
    }

    /// <summary>
    /// 分布式的最小单位  Entity节点树的根节点   原则上VP和VP之间是孤岛
    /// </summary>
    [ProcessType]
    public abstract class VirtualProcess
    {
        private Entity root;
        private ProcessConfig config;

        /// <summary>
        /// 根节点  所有的Entity从这里开始挂
        /// </summary>
        public Entity Root => root;
        /// <summary>
        /// 进程的配置  带内外网端口 物理机编号等信息
        /// </summary>
        public ProcessConfig Config => config;

        internal void Init(ProcessConfig config)
        {
            root = Entity.Create();
            this.config = config;
        }
        internal void Close()
        {
            ZObject.Destory(Root);
        }

        public abstract void Start();

        internal static VirtualProcess[] Load(Type[] types)
        {
            //后面改成从中控台下载配置过来   开进程
            ProcessConfig[] configs = ProcessConfigLoader.Load();

            Dictionary<string, Type> VPMap = new Dictionary<string, Type>();
            foreach (var item in types)
            {
                VPMap.Add(item.Name, item);
            }

            var vps = new List<VirtualProcess>();

            var singleVP = new SingletonProcess();
            var config = new ProcessConfig()
            {
                processClassName = "SingletonProcess"
            };

            singleVP.Init(config);
            vps.Add(singleVP);


            foreach (var item in configs)
            {
                if (VPMap.TryGetValue(item.processClassName, out Type value))
                {
                    if (Activator.CreateInstance(value) is VirtualProcess vp)
                    {
                        vp.Init(item);
                        vps.Add(vp);
                    }
                }
            }

            return vps.ToArray();
        }

    }

}
