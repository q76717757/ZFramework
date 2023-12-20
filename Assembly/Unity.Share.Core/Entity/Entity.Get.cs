using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZFramework
{
    public partial class Entity
    {
        //child
        public Entity GetChild(int index)
        {
            ThrowIfDisposed();
            return GetChildren()[index];
        }
        public Entity[] GetChildren()
        {
            ThrowIfDisposed();
            return childrens.Values.ToArray();
        }
        public void GetChildren(List<Entity> result)
        {
            if (result == null)
                throw new NullReferenceException();
            ThrowIfDisposed();
            result.AddRange(childrens.Values);
        }


        //获取同级组件  //TODO Entity.GetComponent 目前不能拿接口,抽象类型
        public Component GetComponent(Type type)
        {
            ThrowIfDisposed();
            if (components.TryGetValue(type, out IComponent component))
            {
                return component as Component;
            }
            else
            {
                return null;
            }
        }
        public T GetComponent<T>()
        {
            Component component = GetComponent(typeof(T));
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
            if (components.TryGetValue(type, out IComponent _component))
            {
                if (_component is Component component1)
                {
                    component = component1;
                    return true;
                }
            }
            component = null;
            return false;
        }
        public bool TryGetComponent<T>(out T component)
        {
            ThrowIfDisposed();
            if (components.TryGetValue(typeof(T), out IComponent com) &&  com is T t)
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
            return components.Values.Cast<Component>().ToArray();
        }
        public void GetComponents(List<Component> results)
        {
            ThrowIfDisposed();
            if (results != null)
            {
                results.AddRange(components.Values.Cast<Component>());
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