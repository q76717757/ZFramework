using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ZFramework
{
    public sealed partial class Entity : ZObject
    {
        private readonly Dictionary<int, Entity> childrens = new Dictionary<int, Entity>();//子物体
        private readonly Dictionary<Type, BasedComponent> components = new Dictionary<Type, BasedComponent>();//组件
        private Entity parent;
        private bool IsRoot => parent is null;

        public Entity Parent
        {
            get
            {
                ThrowIfDisposed();
                if (IsRoot || parent.IsRoot)//隐藏根节点
                {
                    return null;
                }
                return parent;
            }
            set
            {
                ThrowIfDisposed();
                if (parent == value)
                {
                    return;
                }
                if (IsRoot)
                {
                    throw new ArgumentException("RootEntity can't set parent");
                }
                if (value == this)
                {
                    throw new ArgumentException("Entity.Parent can't set self");
                }
                if (value == null)
                {
                    throw new ArgumentException("Entity.Parent can't set null");
                }
                RemoveParentEntityDependencies(parent);
                AddParentEntityDependenceies(value);
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

        internal Entity(Entity parent)//内部使用的构造方法
        {
            AddParentEntityDependenceies(parent);
        }

        protected internal sealed override void BeginDispose(bool isImmediate)
        {
            //递归子物体
            foreach (Entity child in childrens.Values.ToArray())
            {
                DestoryInner(child, isImmediate);
            }
            //释放自身的组件
            foreach (BasedComponent component in components.Values.ToArray())
            {
                DestoryInner(component, isImmediate);
            }
            Game.DestoryHandler(this, isImmediate);
        }
        protected internal sealed override void EndDispose()
        {
            RemoveParentEntityDependencies(parent);
            ClearInstanceID();
        }


        //entity  两个方法都从入口排除了root
        private void AddParentEntityDependenceies(Entity parent)
        {
            this.parent = parent;
            parent?.childrens.Add(this.InstanceID, this);
        }
        private void RemoveParentEntityDependencies(Entity parent)
        {
            this.parent = null;
            parent?.childrens.Remove(this.InstanceID);
        }
        //component
        internal static void AddComponentDependencies(Entity entity, BasedComponent component)
        {
            entity.components.Add(component.GetType(), component);
            component.AddEntityDependenceies(entity);
        }
        internal static void RemoveComponentDependencies(Entity entity, BasedComponent component)
        {
            entity.components.Remove(component.GetType());
            component.RemoveEntityDependenceies();
        }

        private void ThrowIfTypeError(Type type, bool global)
        {
            if (type == null)
            {
                throw new NullReferenceException();
            }
            if (type.IsAbstract)
            {
                throw new ArgumentException($"{type} Is Abstract Component");
            }
            if (global)
            {
                if (type.GetInterface(typeof(IGlobalComponent).Name) == null)
                {
                    throw new ArgumentException($"{type} Is Invalid GlobalcComponent");
                }
            }
            else
            {
                if (!type.IsSubclassOf(typeof(Component)))
                {
                    throw new ArgumentException($"{type} Is Invalid Component");
                }
            }
        }

    }
}