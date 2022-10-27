using System;

namespace ZFramework
{
    /// <summary>
    /// 分布式的最小单位  Entity节点树的根节点
    /// </summary>
    [ProcessType]
    public abstract class VirtualProcess
    {
        private Entity root;
        private ProcessConfig config;

        public Entity Root => root;
        public ProcessConfig Config => config;

        internal void Init(Entity entity, ProcessConfig config)
        {
            root = entity;
            this.config = config;
        }

        public abstract void Start();
    }

}
