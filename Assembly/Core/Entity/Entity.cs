using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.CompilerServices;
using System.Linq;

namespace ZFramework
{
    public sealed partial class Entity : ZObject
    {
        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();//子物体
        private readonly Dictionary<Type, Component> components = new Dictionary<Type, Component>();//组件
        private Entity parent;
        private bool IsRoot
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return parent == null;
            }
        }

        public Entity Parent
        {
            get
            {
                return GetParent();
            }
            set
            {
                SetParent(value);
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
            if (parent != null)
            {
                //is new child
                SetDependencies(parent, this);
            }
            else
            {
                //is game.root
            }
        }

        protected sealed override void BeginDispose(bool isImmediate)
        {
            //递归子物体
            foreach (Entity child in childrens.Values.ToArray())
            {
                Destory(child, isImmediate);
            }
            //释放自身的组件
            foreach (Component component in components.Values.ToArray())
            {
                Destory(component, isImmediate);
            }
            Game.DestoryObject(this, isImmediate);
        }
        protected sealed override void EndDispose()
        {
            if (!IsRoot)
            {
                RemoveChildFromDictionary(parent, this.InstanceID);
                parent = null;
            }
        }


        internal static void RemoveChildFromDictionary(Entity entity, int instanceID)
        {
            entity.childrens.Remove(instanceID);
        }
        internal static void RemoveComponentFromDictionary(Entity entity, Component component)
        {
            entity.components.Remove(component.GetType());
        }
        private static void SetDependencies(Entity parent, Entity child)
        {
            parent.childrens.Add(child.InstanceID, child);
            child.parent = parent;
        }

    }
}