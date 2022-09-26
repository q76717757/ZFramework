using System.Collections;
using System.Collections.Generic;
using System;

namespace ZFramework
{
    public sealed class Entity : Object
    {
        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();//当前entity下挂的子entity
        private readonly Dictionary<Type, ComponentData> components = new Dictionary<Type, ComponentData>();//当前entity上的组件 唯一

        private Entity parent;

        public Entity() : base(IdGenerater.Instance.GenerateInstanceId())
        {
        }

        public VirtualProcess Process
        {
            get;
            private set;
        }

        public Entity Parent
        {
            get => parent;
            set
            {
                parent = value;
            }
        }

        public ComponentData CreateEntity(Type type)
        {
            ComponentData com = (ComponentData)Activator.CreateInstance(type,this);
            return com;
        }
        public T CreateEntity<T>() where T : ComponentData
        {
            return (T)CreateEntity(typeof(T));
        }
        public ComponentData AddComponent(Type type)
        {
            if (components.ContainsKey(type))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity(type);
            components.Add(component.GetType(), component);
            Game.GameLoop.CallAwake(component);
            return component;
        }

        public T AddComponent<T>() where T : ComponentData
        {
            return (T)AddComponent(typeof(T));
        }
        public T AddComponent<T, A>(A a) where T : ComponentData
        {
            if (components.ContainsKey(typeof(T)))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity<T>();
            components.Add(component.GetType(), component);
            Game.GameLoop.CallAwake(component, a);
            return component;
        }
        public T AddComponent<T, A, B>(A a, B b) where T : ComponentData
        {
            if (components.ContainsKey(typeof(T)))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity<T>();
            components.Add(component.GetType(), component);
            Game.GameLoop.CallAwake(component, a, b);
            return component;
        }
        public T AddComponentWithNewChild<T>()
        {
            return default;
        }

        public ComponentData GetComponent(Type type)
        {
            if (components.TryGetValue(type, out ComponentData component))
            {
                return component;
            }
            return null;
        }
        public T GetComponent<T>() where T : ComponentData
        {
            var output = GetComponent(typeof(T));
            if (output != null)
            {
                return output as T;
            }
            return null;
        }
        public T GetComponentInParent<T>(bool includSelf = true) where T : ComponentData
        {
            if (includSelf)
            {
                var self = GetComponent<T>();
                if (self != null)
                {
                    return self;
                }
            }
            if (parent != null)
            {
                return parent.GetComponentInParent<T>(true);
            }
            return null;
        }
        public T GetComponentInChildren<T>(bool includSelf = true) where T : ComponentData
        {
            if (includSelf)
            {
                var self = GetComponent<T>();
                if (self != null)
                {
                    return self;
                }
            }
            foreach (var item in childrens.Values)
            {
                if (item.IsDisposed)
                {
                    continue;
                }
                var a = item.GetComponentInChildren<T>(true);
                if (a != null)
                {
                    return a;
                }
            }
            return null;
        }
        public T[] GetComponentsInChilren<T>(bool includSelf = true) where T : ComponentData
        {
            List<T> ts = new List<T>();
            GetComponentsInChildren_private(ts, includSelf);
            return ts.ToArray();
        }
        private void GetComponentsInChildren_private<T>(List<T> list, bool includSelf) where T : ComponentData
        {
            if (includSelf)
            {
                var self = GetComponent<T>();
                if (self != null)
                {
                    list.Add(self);
                }
            }
            foreach (var item in childrens.Values)
            {
                if (item.IsDisposed)
                {
                    continue;
                }
                item.GetComponentsInChildren_private(list, true);
            }
        }
        public void RemoveComponent(ComponentData component)
        {
            if (components.TryGetValue(component.GetType(), out ComponentData target))
            {
                Game.GameLoop.CallDestory(target);
            }
        }
        public void RemoveComponent<T>() where T : ComponentData
        {
            if (components.TryGetValue(typeof(T),out ComponentData component))
            {
                Game.GameLoop.CallDestory(component);
            }
        }

        public override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            foreach (var item in childrens.Values)
            {
                if (item != null)
                {
                    item.Dispose();
                }
            }
            foreach (var component in components.Values)
            {
                if (component != null)
                {
                    component.Dispose();
                }
            }
            Parent = null;
            //Game.GameLoop.CallDestory(this);

            base.Dispose();
        }

    }

}