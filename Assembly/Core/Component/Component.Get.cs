using System;
using System.Collections.Generic;

namespace ZFramework
{
    public partial class Component
    {
        private Entity GetEntity(bool ignoreRoot = false)
        {
            ThrowIfDisposed();
            if (IsDomainComopnent && ignoreRoot)//这是域组件
            {
                return null;
            }
            else
            {
                //实体组件
                return entity;
            }
        }


        //获取同级组件
        public Component GetComponent(Type type)
        {
            return GetEntity().GetComponent(type);
        }
        public T GetComponent<T>()
        {
            return GetEntity().GetComponent<T>();
        }
        public bool TryGetComponent(Type type, out Component component)
        {
            return GetEntity().TryGetComponent(type, out component);
        }
        public bool TryGetComponent<T>(out T component)
        {
            return GetEntity().TryGetComponent<T>(out component);
        }
        public Component[] GetComponents()
        {
            return GetEntity().GetComponents();
        }
        public void GetComponents(List<Component> results)
        {
            GetEntity().GetComponents(results);
        }

        //获取子级的组件
        public Component GetComponentInChildren(Type type)
        {
            return GetEntity().GetComponentInChildren(type);
        }
        public T GetComponentInChildren<T>()
        {
            return GetEntity().GetComponentInChildren<T>();
        }
        public Component[] GetComponentsInChildren(Type type)
        {
            return GetEntity().GetComponentsInChildren(type);
        }
        public T[] GetComponentsInChildren<T>()
        {
            return GetEntity().GetComponentsInChildren<T>();
        }
        public void GetComponentsInChildren<T>(List<T> results)
        {
            GetEntity().GetComponentsInChildren(results);
        }
        public void GetComponentsInChildren(Type type, List<Component> results)
        {
            GetEntity().GetComponentsInChildren(type, results);
        }

        //获取父级组件
        public Component GetComponentInParent(Type type)
        {
            return GetEntity().GetComponentInParent(type);
        }
        public T GetComponentInParent<T>()
        {
            return GetEntity().GetComponentInParent<T>();
        }
        public Component[] GetComponentsInParent(Type type)
        {
            return GetEntity().GetComponentsInParent(type);
        }
        public T[] GetComponentsInParent<T>()
        {
            return GetEntity().GetComponentsInParent<T>();
        }
        public void GetComponentsInParent<T>(List<T> results)
        {
            GetEntity().GetComponentsInParent(results);
        }
        public void GetComponentsInParent(Type type, List<Component> results)
        {
            GetEntity().GetComponentsInParent(type, results);
        }


        //获取域级组件
        public Component GetComponentInDomain(Type type)
        {
            return GetEntity().GetComponentInDomain(type);
        }
        public T GetComponentInDomain<T>()
        { 
            return GetEntity().GetComponentInDomain<T>();
        }

    }
}
