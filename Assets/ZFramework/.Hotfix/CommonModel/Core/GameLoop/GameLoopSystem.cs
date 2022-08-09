using System;

namespace ZFramework
{
    [GameLoop]
    public abstract class AwakeSystem<T> : IAwakeSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(IAwakeSystem);
        void IAwakeSystem.OnAwake(Entity entity) => OnAwake((T)entity);
        public abstract void OnAwake(T entity);
    }
    [GameLoop]
    public abstract class AwakeSystem<T, A> : IAwakeSystem<A> where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(IAwakeSystem<A>);
        void IAwakeSystem<A>.OnAwake(Entity entity, A a) => OnAwake((T)entity, a);
        public abstract void OnAwake(T entity, A a);
    }
    [GameLoop]
    public abstract class AwakeSystem<T, A, B> : IAwakeSystem<A, B> where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(IAwakeSystem<A, B>);
        void IAwakeSystem<A, B>.OnAwake(Entity entity, A a, B b) => OnAwake((T)entity, a, b);
        public abstract void OnAwake(T entity, A a, B b);
    }

    [GameLoop]
    public abstract class ReloadSystem<T> : IReLoadSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(IReLoadSystem);
        void IReLoadSystem.OnReload(Entity entity) => OnReload((T)entity);
        public abstract void OnReload(T entity);
    }
    [GameLoop]
    public abstract class EnableSystem<T> : IEnableSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(IEnableSystem);
        void IEnableSystem.OnEnable(Entity entity) => OnEnable((T)entity);
        public abstract void OnEnable(T entity);
    }
    [GameLoop]
    public abstract class DisableSystem<T> : IDisableSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(IDisableSystem);
        void IDisableSystem.OnDisable(Entity entity) => OnDisable((T)entity);
        public abstract void OnDisable(T entity);
    }
    [GameLoop]
    public abstract class UpdateSystem<T> : IUpdateSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(IUpdateSystem);
        void IUpdateSystem.OnUpdate(Entity entity) => OnUpdate((T)entity);
        public abstract void OnUpdate(T entity);
    }
    [GameLoop]
    public abstract class LateUpdateSystem<T> : ILateUpdateSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(ILateUpdateSystem);
        void ILateUpdateSystem.OnLateUpdate(Entity entity) => OnLateUpdate((T)entity);
        public abstract void OnLateUpdate(T entity);
    }
    [GameLoop]
    public abstract class DestorySystem<T> : IDestorySystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type GameLoopType => typeof(IDestorySystem);
        void IDestorySystem.OnDestory(Entity entity) => OnDestory((T)entity);
        public abstract void OnDestory(T entity);
    }

}
