using System;

namespace ZFramework
{
    public interface IGameLoopSystem
    {
        Type EntityType { get; }
        Type PlayLoopType { get; }
    }
    public interface IReLoadSystem : IGameLoopSystem
    {
        void OnReload(Entity entity);
    }
    public interface IAwakeSystem : IGameLoopSystem
    {
        void OnAwake(Entity entity);
    }
    public interface IUpdateSystem : IGameLoopSystem
    {
        void OnUpdate(Entity entity);
    }
    public interface IEnableSystem : IGameLoopSystem
    {
        void OnEnable(Entity entity);
    }
    public interface IDisableSystem : IGameLoopSystem
    {
        void OnDisable(Entity entity);
    }
    public interface ILateUpdateSystem : IGameLoopSystem
    {
        void OnLateUpdate(Entity entity);
    }
    public interface IDestorySystem : IGameLoopSystem
    {
        void OnDestory(Entity entity);
    }
    [GameLoop]
    public abstract class ReloadSystem<T> : IReLoadSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IReLoadSystem);
        void IReLoadSystem.OnReload(Entity entity) => OnReload((T)entity);
        public abstract void OnReload(T entity);
    }
    [GameLoop]
    public abstract class AwakeSystem<T> : IAwakeSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IAwakeSystem);
        void IAwakeSystem.OnAwake(Entity entity) => OnAwake((T)entity);
        public abstract void OnAwake(T entity);
    }
    [GameLoop]
    public abstract class EnableSystem<T> : IEnableSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IEnableSystem);
        void IEnableSystem.OnEnable(Entity entity) => OnEnable((T)entity);
        public abstract void OnEnable(T entity);
    }
    [GameLoop]
    public abstract class DisableSystem<T> : IDisableSystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IDisableSystem);
        void IDisableSystem.OnDisable(Entity entity) => OnDisable((T)entity);
        public abstract void OnDisable(T entity);
    }
    [GameLoop]
    public abstract class UpdateSystem<T> : IUpdateSystem where T : Entity
    {
        public Type EntityType => typeof(T); 
        public Type PlayLoopType => typeof(IUpdateSystem);
        void IUpdateSystem.OnUpdate(Entity entity) => OnUpdate((T)entity);
        public abstract void OnUpdate(T entity);
    }
    [GameLoop]
    public abstract class LateUpdateSystem<T> : ILateUpdateSystem where T : Entity
    {
        public Type EntityType => typeof(T); 
        public Type PlayLoopType => typeof(ILateUpdateSystem);
        void ILateUpdateSystem.OnLateUpdate(Entity entity)=> OnLateUpdate((T)entity);
        public abstract void OnLateUpdate(T entity);
    }
    [GameLoop]
    public abstract class DestorySystem<T> : IDestorySystem where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IDestorySystem);
        void IDestorySystem.OnDestory(Entity entity) => OnDestory((T)entity);
        public abstract void OnDestory(T entity);
    }
}