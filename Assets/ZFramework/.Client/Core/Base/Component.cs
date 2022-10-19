using System;
using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public abstract class Component : Object
    {
        private bool isEnable = true;
        private Entity entity;

        public Type Type => GetType();
        public bool Enable
        {
            get => isEnable;
            set
            {
                if (isEnable != value)
                {
                    SetEnable(value);
                }
            }
        }
        public Entity Entity
        {
            get => entity;
            set 
            {
                if (entity != value)
                {
                    SetEntity(value);
                }
            }
        }

        public Component() : base(Game.instance.IdGenerater.GenerateInstanceId())
        {
        }
        public Component(Entity entity) : base(Game.instance.IdGenerater.GenerateId())
        {
            Entity = entity;
        }
        internal void SetEnable(bool enable)
        {
            isEnable = enable;
            if (enable)
            {
                Game.instance.GameLoopSystem.CallEnable(this);
            }
            else
            {
                Game.instance.GameLoopSystem.CallDisable(this);
            }
        }
        internal void SetEntity(Entity entity)
        {
            Log.Error("未实现");
        }




        //GET
        public Component GetComponent(Type type) => Entity.GetComponent(type);
        public T GetComponent<T>() where T : Component => Entity.GetComponent<T>();
        public T GetComponentInParent<T>(bool includSelf = true) where T : Component => Entity.GetComponentInParent<T>(includSelf);
        public T[] GetComponentsInParent<T>(bool iscludSelf = true) where T : Component => Entity.GetComponentsInParent<T>(iscludSelf);
        public T GetComponentInChildren<T>(bool includSelf = true) where T : Component => Entity.GetComponentInChildren<T>(includSelf);
        public T[] GetComponentsInChilren<T>(bool includSelf = true) where T : Component => Entity.GetComponentsInChilren<T>(includSelf);

    }
}
