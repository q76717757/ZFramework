using System.Collections;
using System.Collections.Generic;
using System;

namespace ZFramework
{
    public class Entity : Object
    {
        private Entity parent;
        private readonly Dictionary<Type, Entity> components = new Dictionary<Type, Entity>();//当前entity上的组件 唯一
        private readonly Dictionary<long, Entity> childrens = new Dictionary<long, Entity>();//当前entity下挂的子entity

        private readonly Dictionary<Type,Dictionary<long,Entity>> componentss = new Dictionary<Type, Dictionary<long, Entity>>();

        //protected Entity()
        //{ 
        //}

        public Entity Parent
        {
            get => parent;
            set
            {
                parent = value;
            }
        }



        public static Entity CreateEntity(Type type,bool reg = true)
        {
            Entity com = (Entity)Activator.CreateInstance(type);
            com.InstanceID = IdGenerater.Instance.GenerateInstanceId();
            if (reg)
            {
                Game.PlayLoop.RegisterEntityToPlayloop(com);
            }
            return com;
        }
        public static T CreateEntity<T>() where T : Entity
        {
            return (T)CreateEntity(typeof(T)); 
        }
        public Entity AddComponent(Type type)
        {
            if (components.ContainsKey(type))
            {
                Log.Error("一个entity下 每种component只能挂一个");
                return null;
            }
            var component = CreateEntity(type);
            components.Add(component.GetType(), component);
            Game.PlayLoop.RegisterEntityToPlayloop(component);
            return component;
        }
        public T AddComponent<T>() where T : Entity
        {
            return (T)AddComponent(typeof(T));
        }
        public Entity GetComponent(Type type)
        {
            if (components.TryGetValue(type, out Entity component))
            {
                return component;
            }
            return null;
        }
        public T GetComponent<T>() where T : Entity
        {
            var output = GetComponent(typeof(T));
            if (output != null)
            {
                return output as T;
            }
            return null;
        }
        public T GetComponentInParent<T>(bool includSelf = true) where T : Entity
        {
            if (includSelf)
            {
                var self = GetComponent<T>();
                if (self != null)
                {
                    return self;
                }
            }
            if (parent != null)
            {
                return parent.GetComponentInParent<T>(true);
            }
            return null;
        }
        public T GetComponentInChildren<T>(bool includSelf = true) where T : Entity
        {
            if (includSelf)
            {
                var self = GetComponent<T>();
                if (self != null)
                {
                    return self;
                }
            }
            foreach (var item in childrens.Values)
            {
                if (item.IsDisposed)
                {
                    continue;
                }
                var a = item.GetComponentInChildren<T>(true);
                if (a != null)
                {
                    return a;
                }
            }
            return null;
        }
        public T[] GetComponentsInChilren<T>(bool includSelf = true) where T : Entity
        {
            List<T> ts = new List<T>();
            GetComponentsInChildren_private(ts, includSelf);
            return ts.ToArray();
        }
        private void GetComponentsInChildren_private<T>(List<T> list, bool includSelf) where T : Entity
        {
            if (includSelf)
            {
                var self = GetComponent<T>();
                if (self != null)
                {
                    list.Add(self);
                }
            }
            foreach (var item in childrens.Values)
            {
                if (item.IsDisposed)
                {
                    continue;
                }
                item.GetComponentsInChildren_private(list, true);
            }
        }
        public void RemoveComponent(Entity component)
        {
            if (components.TryGetValue(component.GetType(), out Entity target))
            {
                Game.PlayLoop.RemoveEntityFromPlayloop(target);
            }
        }
        public void RemoveComponent<T>() where T : Entity
        {
            if (components.TryGetValue(typeof(T),out Entity component))
            {
                Game.PlayLoop.RemoveEntityFromPlayloop(component);
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            base.Dispose();

            foreach (var item in childrens.Values)
            {
                if (item != null)
                {
                    item.Dispose();
                }
            }
            foreach (var component in components.Values)
            {
                if (component != null)
                {
                    component.Dispose();
                }
            }
            Parent = null;
            Game.PlayLoop.RemoveEntityFromPlayloop(this);
        }

      

    }

}