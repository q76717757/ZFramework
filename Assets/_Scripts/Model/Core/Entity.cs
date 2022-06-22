using System.Collections;
using System.Collections.Generic;
using System;

namespace ZFramework
{
    public sealed class Entity : Object
    {
        private Entity() { }

        private Entity parent;
        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();
        private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();
        public Entity Parent
        {
            get => parent;
            set => parent = value;
        }

        public T AddComponent<T>() where T : Component//立即创建
        {
            if (components.ContainsKey(typeof(T)))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component =  Component.CreateComponent<T>();
            components.Add(component.GetType(), component);
            component.Entity = this;
            PlayLoop.Instance.Awake(component);
            return component;
        }
        public T GetComponent<T>() where T : Component
        {
            if (components.TryGetValue(typeof(T), out Component component))
            {
                return (T)component;
            }
            return null;
        }
        public T GetComponentInParent<T>(bool includSelf) where T : Component
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
        public T GetComponentInChildren<T>(bool includSelf) where T : Component
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
                var child = item.GetComponent<T>();
                if (child != null)
                {
                    return child;
                }
            }
            foreach (var item in childrens.Values)
            {
                var a = item.GetComponentInChildren<T>(false);
                if (a != null)
                {
                    return a;
                }
            }
            return null;
        }
        public T[] GetComponentsInChilrends<T>() where T : Component
        {
            List<T> ts = new List<T>();
            GetComponentsInChildren_private(ts);
            return ts.ToArray();
        }
        private void GetComponentsInChildren_private<T>(List<T> list) where T : Component
        {
            var self = GetComponent<T>();
            if (self != null)
            {
                list.Add(self);
            }
            foreach (var item in childrens.Values)
            {
                if (item.IsDisposed)
                {
                    continue;
                }
                item.GetComponentsInChildren_private(list);
            }
        }


        public void RemoveComponent(Component component)//延迟销毁
        {
            if (components.TryGetValue(component.GetType(), out Component target))
            {
                PlayLoop.Instance.Destroy(target);
            }
        }
        public void RemoveComponent<T>() where T : Component
        {
            if (components.TryGetValue(typeof(T),out Component component))
            {
                PlayLoop.Instance.Destroy(component);
            }
        }


        public void Destory()
        {
            foreach (var component in components.Values)
            {
                if (component.IsDisposed)
                    continue;
                PlayLoop.Instance.Destroy(component);
            }
        }










        public static Entity CreateEntity()
        {
            var e = new Entity();
            return e;
        }
        public override string ToString()
        {
            if (IsDisposed) return "This Entity Is Disposed";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"Entity [{InstanceID}]");
            sb.Append("--ChildCount:" + childrens.Count);
            sb.Append("--ComponentCount:" + components.Count);
            foreach (var component in components)
            {
                sb.Append($"{component.Value}:{component.Key}");
            }
            return sb.ToString();
        }
    }

}