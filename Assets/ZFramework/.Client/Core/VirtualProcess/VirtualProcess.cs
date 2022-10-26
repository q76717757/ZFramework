using System;

namespace ZFramework
{
    [ProcessType]
    public abstract class VirtualProcess
    {
        private Entity root;
        public Entity Root { get => root; }

        public string Parms;

        internal void Init(Entity entity,string parms)
        {
            root = entity;
            Parms = parms;
        }
        public abstract void Start();
    }

}
