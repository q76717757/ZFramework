using System.Collections;
using System.Collections.Generic;
using System;

namespace ZFramework
{
    [Flags]
    public enum EntityStatus : byte
    {
        None = 0,
        IsFromPool = 1,
        IsRegister = 1 << 1,
        IsComponent = 1 << 2,
        IsCreated = 1 << 3,
        IsNew = 1 << 4,
    }

    public class Entity : Object
    {
        private Entity parent;
        private readonly Dictionary<Type, Entity> components = new Dictionary<Type, Entity>();//当前entity上的组件 唯一
        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();//当前entity下挂的子entity
        //private readonly Dictionary<Type, List<Entity>> componentss = new Dictionary<Type, List<Entity>>();//按类型分类
        //private readonly Dictionary<long, Entity> componentsss = new Dictionary<long, Entity>();//所有child entity 按id索引

        public Entity() : base(IdGenerater.Instance.GenerateInstanceId())
        {

        }

        private bool enable;
        public bool Enable
        {
            get => enable;
            set
            {
                enable = value;
                if (enable)
                {
                    Game.GameLoop.CallEnable(this);
                }
                else
                {
                    Game.GameLoop.CallDisable(this);
                }
            }
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

        public Entity CreateEntity(Type type)
        {
            Entity com = (Entity)Activator.CreateInstance(type);
            if (this is VirtualProcess vp)
            {
                com.Process = vp.Process;
            }
            else
            {
                com.Process = Process;
            }
            com.Parent = this;
            return com;
        }
        public T CreateEntity<T>() where T : Entity
        {
            return (T)CreateEntity(typeof(T)); ;
        }
        public Entity AddComponent(Type type)
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

        public T AddComponent<T>() where T : Entity
        {
            return (T)AddComponent(typeof(T));
        }
        public T AddComponent<T, A>(A a) where T : Entity
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
        public T AddComponent<T, A, B>(A a, B b) where T : Entity
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

        public Entity GetComponent(Type type)
        {
            if (components.TryGetValue(type, out Entity component))
            {
                return component;
            }
            return null;
        }
        public T GetComponent<T>() where T : Entity
        {
            var output = GetComponent(typeof(T));
            if (output != null)
            {
                return output as T;
            }
            return null;
        }
        public T GetComponentInParent<T>(bool includSelf = true) where T : Entity
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
        public T GetComponentInChildren<T>(bool includSelf = true) where T : Entity
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
        public T[] GetComponentsInChilren<T>(bool includSelf = true) where T : Entity
        {
            List<T> ts = new List<T>();
            GetComponentsInChildren_private(ts, includSelf);
            return ts.ToArray();
        }
        private void GetComponentsInChildren_private<T>(List<T> list, bool includSelf) where T : Entity
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
        public void RemoveComponent(Entity component)
        {
            if (components.TryGetValue(component.GetType(), out Entity target))
            {
                Game.GameLoop.CallDestory(target);
            }
        }
        public void RemoveComponent<T>() where T : Entity
        {
            if (components.TryGetValue(typeof(T),out Entity component))
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
            Game.GameLoop.CallDestory(this);

            base.Dispose();
        }

    }

}