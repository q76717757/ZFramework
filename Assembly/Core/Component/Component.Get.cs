using System;
using System.Collections.Generic;

namespace ZFramework
{
    public partial class Component
    {
        private Entity GetEntity()
        {
            ThrowIfDisposed();
            return entity;
        }

        private bool GetEnable()
        {
            ThrowIfDisposed();
            return isEnable;
        }
        private void SetEnable(bool enable)
        {
            ThrowIfDisposed();
            if (isEnable != enable)
            {
                isEnable = enable;
                if (enable)
                {
                    Game.CallEnable(this);
                }
                else
                {
                    Game.CallDisable(this);
                }
            }
        }

        //获取同级组件
        public Component GetComponent(Type type)
        {
            return Entity.GetComponent(type);
        }
        public T GetComponent<T>()
        {
            return Entity.GetComponent<T>();
        }
        public bool TryGetComponent(Type type, out Component component)
        {
            return Entity.TryGetComponent(type, out component);
        }
        public bool TryGetComponent<T>(out T component)
        {
            return Entity.TryGetComponent<T>(out component);
        }
        public Component[] GetComponents()
        {
            return Entity.GetComponents();
        }
        public void GetComponents(List<Component> results)
        {
            Entity.GetComponents(results);
        }

        //获取子级的组件
        public Component GetComponentInChildren(Type type)
        {
            return Entity.GetComponentInChildren(type);
        }
        public T GetComponentInChildren<T>()
        {
            return Entity.GetComponentInChildren<T>();
        }
        public Component[] GetComponentsInChildren(Type type)
        {
            return Entity.GetComponentsInChildren(type);
        }
        public T[] GetComponentsInChildren<T>()
        {
            return Entity.GetComponentsInChildren<T>();
        }
        public void GetComponentsInChildren<T>(List<T> results)
        {
            Entity.GetComponentsInChildren(results);
        }
        public void GetComponentsInChildren(Type type, List<Component> results)
        {
            Entity.GetComponentsInChildren(type, results);
        }

        //获取父级组件
        public Component GetComponentInParent(Type type)
        {
            return Entity.GetComponentInParent(type);
        }
        public T GetComponentInParent<T>()
        {
            return Entity.GetComponentInParent<T>();
        }
        public Component[] GetComponentsInParent(Type type)
        {
            return Entity.GetComponentsInParent(type);
        }
        public T[] GetComponentsInParent<T>()
        {
            return Entity.GetComponentsInParent<T>();
        }
        public void GetComponentsInParent<T>(List<T> results)
        {
            Entity.GetComponentsInParent(results);
        }
        public void GetComponentsInParent(Type type, List<Component> results)
        {
            Entity.GetComponentsInParent(type, results);
        }

    }
}
