using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    /// <summary>
    /// 全局组件不支持多层继承
    /// </summary>
    public abstract class GlobalComponent<T> : ZObject, IComponent where T : GlobalComponent<T>
    {
        private static T _instance;
        public static T Instance => _instance;

        private Entity entity;
        private bool isEnable;
        public bool Enable
        {
            get
            {
                ThrowIfDisposed();
                return isEnable;
            }
            set
            {
                ThrowIfDisposed();
                if (isEnable != value)
                {
                    isEnable = value;
                    if (value)
                    {
                        Game.CallEnable(this);
                    }
                    else
                    {
                        Game.CallDisable(this);
                    }
                }
            }
        }
        protected GlobalComponent() { }



        protected internal sealed override void BeginDispose(bool isImmediate)
        {
            Game.DestoryHandler(this, isImmediate);
        }
        protected internal sealed override void EndDispose()
        {
            Entity.RemoveComponentDependencies(entity, this);
            ClearInstanceID();
        }

        void IComponent.AddEntityDependenceies(Entity entity)
        {
            _instance = this as T;
            this.entity = entity;
            isEnable = true;
        }
        void IComponent.RemoveEntityDependenceies()
        {
            _instance = null;
            entity = null;
        }
    }
}
