using System;
using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public partial class Entity
    {
        //Entity
        public Entity AddChild()
        {
            ThrowIfDisposed();
            return new Entity(this);
        }

        //Component
        public Component AddComponent(Type type)
        {
            ThrowIfDisposed();
            if (components.TryGetValue(type, out Component value))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return value;
            }
            Component component = Component.CreateInstance(type, this);
            components.Add(type, component);
            Game.CallAwake(component);
            //Game.CallEnable(component);//TODO  Enable目前没用上 Entity.SetActive()考虑实现 有需求再继续实现 
            return component;
        }
        public T AddComponent<T>() where T : Component
        {
            return (T)AddComponent(typeof(T));
        }

    }
}
