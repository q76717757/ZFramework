using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ZFramework
{
    public sealed partial class Entity : ZObject
    {
        private readonly Dictionary<int, Entity> childrens = new Dictionary<int, Entity>();//子物体
        private readonly Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();//组件
        private Entity parent;
        private bool isRoot;

        public Entity Parent
        {
            get
            {
                ThrowIfDisposed();
                if (isRoot || parent.isRoot)//隐藏根节点
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
                if (isRoot)
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
                this.RemoveParentEntityDependencies(parent);
                this.AddParentEntityDependenceies(value);
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
            isRoot = parent == null;
            if (!isRoot)
            {
                this.AddParentEntityDependenceies(parent);
            }
        }

        protected internal sealed override void BeginDispose(bool isImmediate)//释放顺序从最深的子开始 先释放组件 再释放实体
        {
            //递归子物体
            foreach (Entity child in childrens.Values.ToArray())
            {
                DestoryInner(child, isImmediate);
            }
            //释放自身的组件
            foreach (IComponent component in components.Values.ToArray())
            {
                DestoryInner(component, isImmediate);
            }
            Game.DestoryHandler(this, isImmediate);
        }
        protected internal sealed override void EndDispose()
        {
            if (!isRoot)
            {
                this.RemoveParentEntityDependencies(parent);
            }
            ClearInstanceID();
        }


        //entity  两个方法都从入口排除了root
        private void AddParentEntityDependenceies(Entity parent)
        {
            parent.childrens.Add(this.InstanceID, this);
            this.parent = parent;
        }
        private void RemoveParentEntityDependencies(Entity parent)
        {
            parent.childrens.Remove(this.InstanceID);
            this.parent = null;
        }
        //component
        internal static void AddComponentDependencies(Entity entity, IComponent component)
        {
            entity.components.Add(component.GetType(), component);
            component.AddEntityDependenceies(entity);
        }
        internal static void RemoveComponentDependencies(Entity entity, IComponent component)
        {
            entity.components.Remove(component.GetType());
            component.RemoveEntityDependenceies();
        }

    }
}