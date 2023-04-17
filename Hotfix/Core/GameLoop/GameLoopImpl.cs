using System;

namespace ZFramework
{
    [GameLoop]
    public abstract class OnAwakeImpl<T> : IAwake where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IAwake);
        void IAwake.OnAwake(Component self) => OnAwake((T)self);
        public abstract void OnAwake(T self);
    }
    [GameLoop]
    public abstract class OnReloadImpl<T> : IReLoad where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IReLoad);
        void IReLoad.OnReload(Component self) => OnReload((T)self);
        public abstract void OnReload(T self);
    } 
    [GameLoop]
    public abstract class OnEnableImpl<T> : IEnable where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IEnable);
        void IEnable.OnEnable(Component self) => OnEnable((T)self);
        public abstract void OnEnable(T self);
    }
    [GameLoop]
    public abstract class OnDisableImpl<T> : IDisable where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IDisable);
        void IDisable.OnDisable(Component self) => OnDisable((T)self);
        public abstract void OnDisable(T self);
    }
    [GameLoop]
    public abstract class OnUpdateImpl<T> : IUpdate where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IUpdate);
        void IUpdate.OnUpdate(Component self) => OnUpdate((T)self);
        public abstract void OnUpdate(T self);
    }
    [GameLoop]
    public abstract class OnLateUpdateImpl<T> : ILateUpdate where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(ILateUpdate);
        void ILateUpdate.OnLateUpdate(Component self) => OnLateUpdate((T)self);
        public abstract void OnLateUpdate(T self);
    }
    [GameLoop]
    public abstract class OnDestoryImpl<T> : IDestory where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IDestory);
        void IDestory.OnDestory(Component self) => OnDestory((T)self);
        public abstract void OnDestory(T self);
    }

}
