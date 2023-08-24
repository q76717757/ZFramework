using System.Collections;
using System.Collections.Generic;
using System;

namespace ZFramework
{
    public sealed partial class Entity : ZObject
    {
        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();//子物体
        private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();//组件

        private Domain domain;
        private Entity parent;
        public Domain Domain
        {
            get
            {
                return GetDomain();
            }
        }
        public Entity Parent
        {
            get
            {
                return GetParent();
            }
            set
            {
                SetParent(value);
            }
        }

        public int ChildCount
        {
            get
            {
                ThrowIfDisposed();
                return childrens.Count;
            }
        }
        internal bool IsDomainRoot
        {
            get
            {
                return parent is null;
            }
        }

        private Entity() { }
        public Entity(Domain domain)
        {
            if (domain == null)
            {
                throw new Exception("domain is null");
            }
            this.domain = domain;
            if (domain.IsLoaded)//domain的第一级实体
            {
                this.parent = domain.Root;
            }
            else//是根节点entity
            {
                this.parent = null;
            }
        }
        public Entity(Entity parent)
        {
            if (parent == null)
            {
                throw new Exception("parent is null");
            }
            this.domain = parent.Domain;
            this.parent = parent;
        }


        //Add
        public Component AddComponent(Type type)
        {
            if (components.TryGetValue(type, out Component value))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return value;
            }
            var component = Component.Create(type, this);
            components.Add(component.GetType(), component);
            Game.CallAwake(component);
            Game.WaitingAdd(component);
            return component;
        }
        public T AddComponent<T>() where T : Component
        {
            if (components.TryGetValue(typeof(T), out Component value))
            {
                Log.Error("一个entity下 每种component只能挂一个 已返回现有的组件");
                return value as T;
            }
            T component = Component.Create<T>(this);
            components.Add(component.GetType(), component);
            Game.CallAwake(component);
            Game.WaitingAdd(component);
            return component;
        }


        //Remove
        internal void RemoveComponent(Component component)
        {
            components.Remove(component.GetType());
        }
        internal void RemoveChild(Entity child)
        {
            childrens.Remove(child.InstanceID);
        }
        protected sealed override void BeginDispose()
        {
            //递归子物体
            foreach (Entity child in childrens.Values)
            {
                Destory(child);
            }
            //释放自身的组件
            foreach (Component component in components.Values)
            {
                Destory(component);
            }
            Game.WaitingDestory(this);
        }
        protected sealed override void EndDispose()
        {
            parent.RemoveChild(this);
            parent = null;
            domain = null;
        }
    }
}