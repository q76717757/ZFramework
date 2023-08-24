using System;
using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public partial class Entity
    {
        //parent
        Entity GetParent()
        {
            ThrowIfDisposed();
            if (IsDomainRoot || parent.IsDomainRoot)
            {
                return null;
            }
            return parent;
        }
        void SetParent(Entity parent)
        {
            ThrowIfDisposed();
            if (IsDomainRoot)
            {
                throw new Exception("dont set root entity.parent");
            }
            if (parent == this)
            {
                throw new Exception("entity.parent can not be set self");
            }
            if (parent.Domain != this.Domain)
            {
                throw new Exception("You cannot move an entity to a tree in another domain");
            }

            this.parent.childrens.Remove(this.InstanceID);
            if (parent == null)
            {
                domain.Root.childrens.Add(this.InstanceID, this);
                this.parent = domain.Root;
            }
            else
            {
                parent.childrens.Add(this.InstanceID, this);
                this.parent = parent;
            }
        }

        //domain
        Domain GetDomain()
        {
            ThrowIfDisposed();
            return domain;
        }

        //child
        public Entity GetChild(int index)
        {
            ThrowIfDisposed();
            throw new NotImplementedException();
        }
        public Entity[] GetEntities()
        {
            ThrowIfDisposed();
            throw new NotImplementedException();
        }
        public void GetEntities(List<Entity> result)
        {
            ThrowIfDisposed();
            throw new NotImplementedException();
        }


        //获取同级组件
        public Component GetComponent(Type type)
        {
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
            if (components.TryGetValue(typeof(T),out Component component) && component is T t)
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
            return components.TryGetValue(type, out component);
        }
        public bool TryGetComponent<T>(out T component)
        {
            if (components.TryGetValue(typeof(T), out Component com) &&  com is T t)
            {
                component= t;
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
            throw new NotImplementedException();
        }
        public void GetComponents(List<Component> results)
        {
            throw new NotImplementedException();
        }

        //获取子级的组件
        public Component GetComponentInChildren(Type type)
        {
            throw new NotImplementedException();
        }
        public T GetComponentInChildren<T>()
        {
            throw new NotImplementedException();
        }
        public Component[] GetComponentsInChildren(Type type)
        {
            throw new NotImplementedException();
        }
        public T[] GetComponentsInChildren<T>()
        {
            throw new NotImplementedException();
        }
        public void GetComponentsInChildren<T>(List<T> results)
        {
            throw new NotImplementedException();
        }
        public void GetComponentsInChildren(Type type, List<Component> results)
        {
            throw new NotImplementedException();
        }

        //获取父级组件
        public Component GetComponentInParent(Type type)
        {
            throw new NotImplementedException();
        }
        public T GetComponentInParent<T>()
        {
            throw new NotImplementedException();
        }
        public Component[] GetComponentsInParent(Type type)
        {
            throw new NotImplementedException();
        }
        public T[] GetComponentsInParent<T>()
        {
            throw new NotImplementedException();
        }
        public void GetComponentsInParent<T>(List<T> results)
        {
            throw new NotImplementedException();
        }
        public void GetComponentsInParent(Type type, List<Component> results)
        {
            throw new NotImplementedException();
        }

        //获取域级组件
        public Component GetComponentInDomain(Type type)
        {
            return Domain.GetComponentInDomain(type);
        }
        public T GetComponentInDomain<T>()
        {
            return Domain.GetComponentInDomain<T>();
        }
    }
}
