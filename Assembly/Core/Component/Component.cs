using System;
using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public abstract partial class Component : ZObject
    {
        private Entity entity;
        private bool isEnable;

        public Entity Entity
        {
            get
            {
                return GetEntity();
            }
        }
        public bool Enable
        {
            get
            {
                return GetEnable();
            }
            set
            {
                SetEnable(value);
            }
        }

        protected Component()
        {
        }


        protected sealed override void BeginDispose(bool isImmediate)
        {
            Game.DestoryObject(this, isImmediate);
        }
        protected sealed override void EndDispose()
        {
            Entity.RemoveComponentFromDictionary(entity, this);
            entity = null;
        }

        internal static Component CreateInstance(Type type, Entity entity)
        {
            Component component = (Component)Activator.CreateInstance(type);
            component.entity = entity;
            component.isEnable = true;
            return component;
        }
    }
}
