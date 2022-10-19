using System.Collections;
using System.Collections.Generic;
using System;

namespace ZFramework
{
    public sealed class Entity : Object //可以类比Unity的Gameboject合并Tranform 除去空间坐标系的相关内容 主要负责管理层级关系和组件挂载
    {
        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();//子物体
        private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();//组件
        private Entity parent;

        private Entity() : base(Game.instance.IdGenerater.GenerateInstanceId())
        {
        }
        public Entity(Entity parent) : base(Game.instance.IdGenerater.GenerateInstanceId())
        {
            Parent = parent;
            Process = parent.Process;
        }

        public VirtualProcess Process { get; }
        public Entity Parent
        {
            get => parent;
            set
            {
                if (value == null)//parent dont null
                {
                    throw new Exception("parent can not be null");
                }
                if (parent == null)// is RootEntity
                {
                    throw new Exception("dont set root.Parent");
                }
                if (parent == value)//is this
                {
                    return;
                }
                //normal entity
                parent.childrens.Remove(this.InstanceID);
                value.childrens.Add(this.InstanceID, this);
                parent = value;
            }
        }
        public bool IsActive { get; set; } = true;


        //CREATE
        internal static Entity CreateRoot()
        { 
            Entity entity = new Entity();
            return entity;
        }
        private Component CreateEntity(Type type)
        {
            Component com = (Component)Activator.CreateInstance(type);
            return com;
        }
        private T CreateEntity<T>() where T : Component
        {
            return Activator.CreateInstance<T>();
        }

        //ADD
        public Component AddComponent(Type type)
        {
            if (components.ContainsKey(type))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity(type);
            components.Add(component.GetType(), component);
            Game.instance.GameLoopSystem.CallAwake(component);
            return component;
        }
        public T AddComponent<T>() where T : Component
        {
            if (components.ContainsKey(typeof(T)))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity<T>();
            components.Add(component.GetType(), component);
            Game.instance.GameLoopSystem.CallAwake(component);
            return component;
        }
        public T AddComponent<T, A>(A a) where T : Component
        {
            if (components.ContainsKey(typeof(T)))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity<T>();
            components.Add(component.GetType(), component);
            Game.instance.GameLoopSystem.CallAwake(component, a);
            return component;
        }
        public T AddComponent<T, A, B>(A a, B b) where T : Component
        {
            if (components.ContainsKey(typeof(T)))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity<T>();
            components.Add(component.GetType(), component);
            Game.instance.GameLoopSystem.CallAwake(component, a, b);
            return component;
        }
        public T AddComponent<T, A, B, C>(A a, B b, C c) where T : Component
        {
            if (components.ContainsKey(typeof(T)))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity<T>();
            components.Add(component.GetType(), component);
            Game.instance.GameLoopSystem.CallAwake(component, a, b, c);
            return component;
        }
        public T AddComponentWithNewChild<T>()
        {
            return default;
        }

        //GET
        public Component GetComponent(Type type)
        {
            if (components.TryGetValue(type, out Component component))
            {
                return component;
            }
            return null;
        }
        public T GetComponent<T>() where T : Component
        {
            var output = GetComponent(typeof(T));
            if (output != null)
            {
                return output as T;
            }
            return null;
        }
        public T GetComponentInParent<T>(bool includSelf = true) where T : Component
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
        public T[] GetComponentsInParent<T>(bool includSelf = true)
        {
            return default;
        }
        public T GetComponentInChildren<T>(bool includSelf = true) where T : Component
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
        public T[] GetComponentsInChilren<T>(bool includSelf = true) where T : Component
        {
            List<T> ts = new List<T>();
            GetComponentsInChildren_private(ts, includSelf);
            return ts.ToArray();
        }
        private void GetComponentsInChildren_private<T>(List<T> list, bool includSelf) where T : Component
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

        //REMOVE
        public void RemoveComponent(Component component)
        {
            if (components.TryGetValue(component.GetType(), out Component target))
            {
                Game.instance.GameLoopSystem.CallDestory(target);
            }
        }
        public void RemoveComponent<T>() where T : Component
        {
            if (components.TryGetValue(typeof(T),out Component component))
            {
                Game.instance.GameLoopSystem.CallDestory(component);
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
            parent.childrens.Remove(this.InstanceID);
            parent = null;

            base.Dispose();
        }

    }

}