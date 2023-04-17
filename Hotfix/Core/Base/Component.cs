using System;
using System.Collections;

namespace ZFramework
{
    public abstract class Component : ZObject
    {
        private bool isEnable = true;
        private Entity entity;

        public bool Enable
        {
            get => isEnable;
            set => SetEnable(value);
        }
        public Entity Entity
        {
            get => entity;
            set => SetEntity(value);
        }

        internal void SetEnable(bool enable)
        {
            if (this.isEnable == enable)
            {
                //无变化
                return;
            }
            this.isEnable = enable;
            if (enable)
            {
                Game.GameLoopSystem.CallEnable(this);
            }
            else
            {
                Game.GameLoopSystem.CallDisable(this);
            }
        }
        internal void SetEntity(Entity entity)
        {
            if (this.entity == entity)
            {
                //无变化
                return;
            }
            if (entity == null)
            {
                //component必须挂在entity上
                return;
            }
        }


        //CREATE
        internal static Component Create(Type type,Entity entity)
        {
            Component component = (Component)Activator.CreateInstance(type);
            component.entity = entity;
            return component;
        }
        internal static T Create<T>(Entity entity) where T : Component
        {
            var component = Activator.CreateInstance<T>();
            component.entity = entity;
            return component;
        }


        //GET
        public Component GetComponent(Type type) => Entity.GetComponent(type);
        public T GetComponent<T>() where T : Component => Entity.GetComponent<T>();
        public T GetComponentInParent<T>(bool includSelf = true) where T : Component => Entity.GetComponentInParent<T>(includSelf);
        public T[] GetComponentsInParent<T>(bool iscludSelf = true) where T : Component => Entity.GetComponentsInParent<T>(iscludSelf);
        public T GetComponentInChildren<T>(bool includSelf = true) where T : Component => Entity.GetComponentInChildren<T>(includSelf);
        public T[] GetComponentsInChilren<T>(bool includSelf = true) where T : Component => Entity.GetComponentsInChilren<T>(includSelf);

        //Remove
        protected internal sealed override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            entity.RemoveComponent(this);
            //Game.instance.GameLoopSystem.DestoryComponent(this);
            //instanceID = 0;
        }
    }
}
