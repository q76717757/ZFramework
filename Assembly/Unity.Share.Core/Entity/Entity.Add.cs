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
            if (type.IsSubclassOf(typeof(Component)))
            {
                return AddComponentInner(type) as Component;
            }
            throw new ArgumentException($"{type} Is Invalid Component");
        }
        public T AddComponent<T>() where T : Component
        {
            return AddComponentInner(typeof(T)) as T;
        }

        //internal
        internal IComponent AddComponentInner(Type type)
        {
            if (type == null)
                throw new ArgumentNullException();
            ThrowIfDisposed();

            if (!components.TryGetValue(type, out IComponent component))
            {
                if (Activator.CreateInstance(type) is IComponent instance)
                {
                    component = instance;
                    Entity.AddComponentDependencies(this, component);
                    Game.CallAwake(component);
                }
            }
            return component;
        }

    }
}
