using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZFramework
{
    public partial class Entity
    {
        //parent
        Entity GetParent()
        {
            ThrowIfDisposed();
            if (IsRoot || parent.IsRoot)
            {
                return null;
            }
            return parent;
        }
        void SetParent(Entity parent)
        {
            ThrowIfDisposed();
            if (this.parent == parent)
            {
                return;
            }
            if (parent == this)
            {
                throw new Exception("entity.parent can not be set self");
            }
            this.parent.childrens.Remove(this.InstanceID);
            if (parent == null) //绑定到新的parent上
            {
                SetDependencies(Game.Root, this);
            }
            else
            {
                SetDependencies(parent, this);
            }
        }

        //child
        //TODO Entity.GetChild相关内容未实现
        public Entity GetChild(int index)
        {
            ThrowIfDisposed();
            throw new NotImplementedException();
        }
        public Entity[] GetChildren()
        {
            ThrowIfDisposed();
            throw new NotImplementedException();
        }
        public void GetChildren(List<Entity> result)
        {
            ThrowIfDisposed();
            throw new NotImplementedException();
        }


        //获取同级组件
        public Component GetComponent(Type type)
        {
            ThrowIfDisposed();
            if (components.TryGetValue(type, out Component component))
            {
                return component;
            }
            else
            {
                return null;
            }
        }
        public T GetComponent<T>()
        {
            Component component = GetComponent(typeof(T));//TODO Entity.GetComponent 目前不能拿接口,抽象类型
            if (component is T t)
            {
                return t;
            }
            else
            {
                return default;
            }
        }
        public bool TryGetComponent(Type type, out Component component)
        {
            ThrowIfDisposed();
            return components.TryGetValue(type, out component);
        }
        public bool TryGetComponent<T>(out T component)
        {
            ThrowIfDisposed();
            if (components.TryGetValue(typeof(T), out Component com) &&  com is T t)
            {
                component = t;
                return true;
            }
            else
            {
                component = default;
                return false;
            }
        }
        public Component[] GetComponents()
        {
            ThrowIfDisposed();
            return components.Values.ToArray();
        }
        public void GetComponents(List<Component> results)
        {
            ThrowIfDisposed();
            if (results != null)
            {
                results.AddRange(components.Values);
            }
        }

        //获取子级的组件
        public Component GetComponentInChildren(Type type)
        {
            if (TryGetComponent(type, out Component component))
            {
                return component;
            }
            else
            {
                foreach (Entity child in childrens.Values)
                {
                    component = child.GetComponentInChildren(type);
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return null;
        }
        public T GetComponentInChildren<T>()
        {
            Component component = GetComponentInChildren(typeof(T));
            if (component is T t)
            {
                return t;
            }
            else
            {
                return default;
            }
        }
        public Component[] GetComponentsInChildren(Type type)
        {
            ThrowIfDisposed();
            List<Component> results = new List<Component>();
            GetComponentsInChildren(type, results);
            return results.ToArray();
        }
        public T[] GetComponentsInChildren<T>()
        {
            ThrowIfDisposed();
            List<T> results = new List<T>();
            GetComponentsInChildren(results);
            return results.ToArray();
        }
        public void GetComponentsInChildren<T>(List<T> results)
        {
            if (results != null)
            {
                if (TryGetComponent(typeof(T),out Component component) && component is T t)
                {
                    results.Add(t);
                }
                foreach (Entity child in childrens.Values)
                {
                    child.GetComponentsInChildren(results);
                }
            }
        }
        public void GetComponentsInChildren(Type type, List<Component> results)
        {
            if (results != null)
            {
                if (TryGetComponent(type, out Component component))
                {
                    results.Add(component);
                }
                foreach (Entity child in childrens.Values)
                {
                    child.GetComponentsInChildren(type, results);
                }
            }
        }

        //获取父级组件
        public Component GetComponentInParent(Type type)
        {
            if (TryGetComponent(type,out Component component))
            {
                return component;
            }
            else
            {
                if (Parent != null)
                {
                    return parent.GetComponentInParent(type);
                }
                else
                {
                    return null;
                }
            }
        }
        public T GetComponentInParent<T>()
        {
            Component component = GetComponentInParent(typeof(T));
            if (component is T t)
            {
                return t;
            }
            else
            {
                return default;
            }
        }
        public Component[] GetComponentsInParent(Type type)
        {
            List<Component> results = new List<Component>();
            GetComponentsInParent(type, results);
            return results.ToArray();
        }
        public T[] GetComponentsInParent<T>()
        {
            List<T> results = new List<T>();
            GetComponentsInParent<T>();
            return results.ToArray();
        }
        public void GetComponentsInParent<T>(List<T> results)
        {
            if (results != null)
            {
                if (TryGetComponent(out T component))
                {
                    results.Add(component);
                }
                if (Parent != null)
                {
                    parent.GetComponentsInParent<T>();
                }
            }
        }
        public void GetComponentsInParent(Type type, List<Component> results)
        {
            if (results != null)
            {
                if (TryGetComponent(type,out Component component))
                {
                    results.Add(component);
                }
                if (Parent != null)
                {
                    parent.GetComponentsInParent(type, results);
                }
            }
        }
    }
}
