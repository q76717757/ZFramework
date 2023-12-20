using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace ZFramework
{
    public abstract partial class Component : ZObject , IComponent
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
        protected Component() { }


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
            this.entity = entity;
            isEnable = true;
        }
        void IComponent.RemoveEntityDependenceies()
        {
            entity = null;
        }
    }
}