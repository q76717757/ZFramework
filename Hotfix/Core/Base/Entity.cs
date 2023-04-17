using System.Collections;
using System.Collections.Generic;
using System;

namespace ZFramework
{
    /// <summary>
    /// 类比UnityEngine.GameObject+Tranform的合体   可以挂组件+挂子物体
    /// </summary>
    public sealed class Entity : ZObject
    {
        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();//子物体
        private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();//组件
        private Entity parent;
        private VirtualProcess process;//根节点

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
                parent.RemoveChild(this);
                value.AddChild(this);
                parent = value;
            }
        }
        public VirtualProcess Process
        {
            get => process;
        }

        public bool IsActive { get; set; } = true;

        private Entity()
        {
        }

        //CREATE
        internal static Entity Create()
        {
            Entity entity = new Entity();
            return entity;
        }
        internal static Entity Create(VirtualProcess process)
        {
            Entity entity = new Entity();
            entity.process = process;
            return entity;
        }


        //NODE
        private void AddChild(Entity child)
        {
            childrens.Add(child.InstanceID, child);
        }
        private void RemoveChild(Entity child)
        {
            childrens.Remove(child.InstanceID);
        }

        //ADD ENTITY
        public Entity AddChild()
        {
            var child = new Entity();
            child.parent = this;
            child.process = this.process;
            AddChild(child);
            return child;
        }

        //ADD COMPONENT
        public Component AddComponent(Type type)
        {
            if (components.ContainsKey(type))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = Component.Create(type, this);
            components.Add(component.GetType(), component);
            Game.GameLoopSystem.CallAwake(component);
            return component;
        }
        public T AddComponent<T>() where T : Component
        {
            if (components.TryGetValue(typeof(T), out Component value))
            {
                //Log.Error("一个entity下 每种component只能挂一个");
                return value as T;
            }
            var component = Component.Create<T>(this);
            components.Add(component.GetType(), component);
            Game.GameLoopSystem.AddComponent(component);
            Game.GameLoopSystem.CallAwake(component);
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
                Game.GameLoopSystem.DestoryComponent(target);
            }
            
        }
        public void RemoveComponent<T>() where T : Component
        {
            if (components.TryGetValue(typeof(T),out Component component))
            {
                Game.GameLoopSystem.DestoryComponent(component);
            }
        }

        protected internal sealed override void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            if (parent != null)
            {
                parent.childrens.Remove(this.InstanceID);
            }
            DiGui();
        }

        void DiGui()
        {
            if (IsDisposed)
            {
                return;
            }
            parent = null;
            //释放自身的组件
            foreach (var component in components.Values)
            {
                component.Dispose();
            }
            components.Clear();

            //递归子物体
            foreach (var child in childrens.Values)
            {
                child.DiGui();
            }
            childrens.Clear();

            instanceID = 0;
        }
    }

}