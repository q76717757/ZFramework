using System;
using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public abstract partial class Component : ZObject
    {
        private Entity entity;
        private bool isEnable = true;

        public Domain Domain
        {
            get
            {
                return entity.Domain;
            }
        }
        public Entity Entity
        {
            get
            {
                return GetEntity(true);
            }
        }
        public bool Enable
        {
            get
            { 
                return isEnable;
            }
            set
            {
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
        private bool IsDomainComopnent
        {
            get
            {
                return entity.IsDomainRoot;
            }
        }
        
        //CREATE
        internal static Component Create(Type type, Entity entity)
        {
            Component component = (Component)Activator.CreateInstance(type);
            component.entity = entity;
            return component;
        }
        internal static T Create<T>(Entity entity) where T : Component
        {
            T component = (T)Activator.CreateInstance(typeof(T));
            component.entity = entity;
            return component;
        }


        //Remove
        protected sealed override void BeginDispose()
        {
            Game.WaitingDestory(this);
        }
        protected sealed override void EndDispose()
        {
            entity.RemoveComponent(this);
            entity = null;
        }

    }
}
