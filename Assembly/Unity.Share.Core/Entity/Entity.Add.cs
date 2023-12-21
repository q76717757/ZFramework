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
        public Component AddComponent(Type type)
        {
            ThrowIfTypeError(type);
            ThrowIfDisposed();
            if (!components.TryGetValue(type, out Component component))
            {
                if (Activator.CreateInstance(type) is Component instance)
                {
                    component = instance;
                    Entity.AddComponentDependencies(this, component);
                    Game.CallAwake(component);
                }
            }
            return component;
        }
        public T AddComponent<T>() where T : Component
        {
            return AddComponent(typeof(T)) as T;
        }
        public Component AddComponentInChild(Type type)
        {
            ThrowIfTypeError(type);
            var child = AddChild();
            return child.AddComponent(type);
        }
        public T AddComponentInChild<T>() where T :Component
        {
            ThrowIfTypeError(typeof(T));
            var child = AddChild();
            return child.AddComponent<T>();
        }

        //internal
        private void ThrowIfTypeError(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException();
            }
            if (type.IsAbstract)
            {
                throw new ArgumentException($"{type} Is Abstract Component");
            }
            if (!type.IsSubclassOf(typeof(Component)))
            {
                throw new ArgumentException($"{type} Is Invalid Component");
            }
        }

    }
}
