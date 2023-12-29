using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public abstract partial class BasedComponent : ZObject
    {
        private Entity entity;
        private bool isEnable;
        public Entity Entity
        {
            get
            {
                ThrowIfDisposed();
                return entity;
            }
        }
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
        protected internal sealed override void BeginDispose(bool isImmediate)
        {
            Game.DestoryHandler(this, isImmediate);
        }
        protected internal sealed override void EndDispose()
        {
            Entity.RemoveComponentDependencies(entity, this);
            ClearInstanceID();
        }

        protected internal virtual void AddEntityDependenceies(Entity entity)
        {
            this.entity = entity;
            isEnable = true;
        }
        protected internal virtual void RemoveEntityDependenceies()
        {
            entity = null;
        }
    }

}
