using System;
using System.Collections;
using System.Collections.Generic;

namespace ZFramework
{
    public partial class Entity
    {
        //Entity
        public Entity AddChild()
        {
            return AddChild(string.Empty);
        }
        public Entity AddChild(string name)
        {
            ThrowIfDisposed();
            return new Entity(this) { Name = name };
        }

        //Component
        public T AddComponent<T>() where T : Component
            => AddComponentInner(typeof(T), false) as T;
        public T AddComponent<T, A>(A a) where T : Component
            => AddComponentInner(typeof(T), false, a) as T;
        public T AddComponent<T, A, B>(A a, B b) where T : Component
            => AddComponentInner(typeof(T), false, a, b) as T;
        public T AddComponent<T, A, B, C>(A a, B b, C c) where T : Component
            => AddComponentInner(typeof(T), false, a, b, c) as T;
        public Component AddComponent(Type type)
            => AddComponentInner(type, false) as Component;
        public Component AddComponent<A>(Type type, A a)
            => AddComponentInner(type, false, a) as Component;
        public Component AddComponent<A, B>(Type type, A a, B b)
            => AddComponentInner(type, false, a, b) as Component;
        public Component AddComponent<A, B, C>(Type type, A a, B b, C c)
            => AddComponentInner(type, false, a, b, c) as Component;

        //Child Component
        public T AddComponentInChild<T>() where T : Component
            => AddChild(typeof(T).Name).AddComponentInner(typeof(T), false) as T;
        public T AddComponentInChild<T, A>(A a) where T : Component
            => AddChild(typeof(T).Name).AddComponentInner(typeof(T), false, a) as T;
        public T AddComponentInChild<T, A, B>(A a, B b) where T : Component
            => AddChild(typeof(T).Name).AddComponentInner(typeof(T), false, a, b) as T;
        public T AddComponentInChild<T, A, B, C>(A a, B b, C c) where T : Component
            => AddChild(typeof(T).Name).AddComponentInner(typeof(T), false, a, b, c) as T;
        public Component AddComponentInChild(Type type)
            => AddChild(type.Name).AddComponentInner(type, false) as Component;
        public Component AddComponentInChild<A>(Type type, A a)
            => AddChild(type.Name).AddComponentInner(type, false, a) as Component;
        public Component AddComponentInChild<A, B>(Type type, A a, B b)
            => AddChild(type.Name).AddComponentInner(type, false, a, b) as Component;
        public Component AddComponentInChild<A, B, C>(Type type, A a, B b, C c)
            => AddChild(type.Name).AddComponentInner(type, false, a, b, c) as Component;


        //internal
        private bool CreateComponentInner(Type type, bool global,out BasedComponent component)
        {
            ThrowIfTypeError(type, global);
            ThrowIfDisposed();
            if (!components.TryGetValue(type, out component))
            {
                if (Activator.CreateInstance(type) is BasedComponent instance)
                {
                    component = instance;
                    Entity.AddComponentDependencies(this, component);
                    return true;//Ö´ÐÐawake
                }
            }
            return false;//²»Ö´ÐÐawake
        }
        internal BasedComponent AddComponentInner(Type type, bool global)
        {
            if (CreateComponentInner(type,global,out BasedComponent component))
            {
                Game.CallAwake(component);
            }
            return component;
        }
        internal BasedComponent AddComponentInner<A>(Type type, bool global, A a)
        {
            if (CreateComponentInner(type, global, out BasedComponent component))
            {
                Game.CallAwake(component, a);
                Game.CallAwake(component);
            }
            return component;
        }
        internal BasedComponent AddComponentInner<A, B>(Type type, bool global, A a, B b)
        {
            if (CreateComponentInner(type, global, out BasedComponent component))
            {
                Game.CallAwake(component, a, b);
                Game.CallAwake(component);
            }
            return component;
        }
        internal BasedComponent AddComponentInner<A, B, C>(Type type, bool global, A a, B b, C c)
        {
            if (CreateComponentInner(type, global, out BasedComponent component))
            {
                Game.CallAwake(component, a, b, c);
                Game.CallAwake(component);
            }
            return component;
        }

    }
}
