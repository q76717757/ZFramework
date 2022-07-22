using System;

namespace ZFramework
{
    public interface IPlayLoop
    {
        Type EntityType { get; }
        Type PlayLoopType { get; }
    }
    public interface IReLoad : IPlayLoop
    {
        void Reload(Entity entity);
    }
    public interface IAwake : IPlayLoop
    {
        void Awake(Entity entity);
    }
    public interface IUpdate : IPlayLoop
    {
        void Update(Entity entity);
    }
    //public interface IEnable : IPlayLoop
    //{ 
    //    void Enable(Entity entity);
    //}
    //public interface IDisable : IPlayLoop
    //{ 
    //    void Disable(Entity entity);
    //}
    public interface ILateUpdate : IPlayLoop
    {
        void LateUpdate(Entity entity);
    }
    public interface IDestory : IPlayLoop
    {
        void Destory(Entity entity);
    }

    [PlayLoop]
    public abstract class ReloadSystem<T> : IReLoad where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IReLoad);
        void IReLoad.Reload(Entity entity) => Reload((T)entity);
        public abstract void Reload(T entity);
    }
    [PlayLoop]
    public abstract class AwakeSystem<T> : IAwake where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IAwake);
        void IAwake.Awake(Entity entity) => Awake((T)entity);
        public abstract void Awake(T entity);
    }
    [PlayLoop]
    public abstract class UpdateSystem<T> : IUpdate where T : Entity
    {
        public Type EntityType => typeof(T); 
        public Type PlayLoopType => typeof(IUpdate);
        void IUpdate.Update(Entity entity) => Update((T)entity);
        public abstract void Update(T entity);
    }
    [PlayLoop]
    public abstract class LateUpdateSystem<T> : ILateUpdate where T : Entity
    {
        public Type EntityType => typeof(T); 
        public Type PlayLoopType => typeof(ILateUpdate);
        void ILateUpdate.LateUpdate(Entity entity)=> LateUpdate((T)entity);
        public abstract void LateUpdate(T entity);
    }
    [PlayLoop]
    public abstract class DestorySystem<T> : IDestory where T : Entity
    {
        public Type EntityType => typeof(T);
        public Type PlayLoopType => typeof(IDestory);
        void IDestory.Destory(Entity entity) => Destory((T)entity);
        public abstract void Destory(T entity);
    }
}