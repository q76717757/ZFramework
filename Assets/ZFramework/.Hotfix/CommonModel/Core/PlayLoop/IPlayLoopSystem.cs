using System;

namespace ZFramework
{
    public interface IPlayLoopSystem
    {
        Type EntityType { get; }
        Type PlayLoopType { get; }
    }
    public interface IReLoadSystem : IPlayLoopSystem
    {
        void OnReload(Entity entity);
    }
    public interface IAwakeSystem : IPlayLoopSystem
    {
        void OnAwake(Entity entity);
    }
    public interface IUpdateSystem : IPlayLoopSystem
    {
        void OnUpdate(Entity entity);
    }
    public interface IEnableSystem : IPlayLoopSystem
    {
        void OnEnable(Entity entity);
    }
    public interface IDisableSystem : IPlayLoopSystem
    {
        void OnDisable(Entity entity);
    }
    public interface ILateUpdateSystem : IPlayLoopSystem
    {
        void OnLateUpdate(Entity entity);
    }
    public interface IDestorySystem : IPlayLoopSystem
    {
        void OnDestory(Entity entity);
    }
    [PlayLoop]
    public abstract class ReloadSystem<T> : IReLoadSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IReLoadSystem);
        void IReLoadSystem.OnReload(Entity entity) => OnReload((T)entity);
        public abstract void OnReload(T entity);
    }
    [PlayLoop]
    public abstract class AwakeSystem<T> : IAwakeSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IAwakeSystem);
        void IAwakeSystem.OnAwake(Entity entity) => OnAwake((T)entity);
        public abstract void OnAwake(T entity);
    }
    [PlayLoop]
    public abstract class EnableSystem<T> : IEnableSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IEnableSystem);
        void IEnableSystem.OnEnable(Entity entity) => OnEnable((T)entity);
        public abstract void OnEnable(T entity);
    }
    [PlayLoop]
    public abstract class DisableSystem<T> : IDisableSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IDisableSystem);
        void IDisableSystem.OnDisable(Entity entity) => OnDisable((T)entity);
        public abstract void OnDisable(T entity);
    }
    [PlayLoop]
    public abstract class UpdateSystem<T> : IUpdateSystem where T : Entity
    {
        public Type EntityType => typeof(T); 
        public Type PlayLoopType => typeof(IUpdateSystem);
        void IUpdateSystem.OnUpdate(Entity entity) => OnUpdate((T)entity);
        public abstract void OnUpdate(T entity);
    }
    [PlayLoop]
    public abstract class LateUpdateSystem<T> : ILateUpdateSystem where T : Entity
    {
        public Type EntityType => typeof(T); 
        public Type PlayLoopType => typeof(ILateUpdateSystem);
        void ILateUpdateSystem.OnLateUpdate(Entity entity)=> OnLateUpdate((T)entity);
        public abstract void OnLateUpdate(T entity);
    }
    [PlayLoop]
    public abstract class DestorySystem<T> : IDestorySystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IDestorySystem);
        void IDestorySystem.OnDestory(Entity entity) => OnDestory((T)entity);
        public abstract void OnDestory(T entity);
    }
}