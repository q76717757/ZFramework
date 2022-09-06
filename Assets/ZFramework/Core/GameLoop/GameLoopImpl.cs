using System;

namespace ZFramework
{
    [GameLoop]
    public abstract class OnAwakeImpl<T> : IAwake where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IAwake);
        void IAwake.OnAwake(ComponentData self) => OnAwake((T)self);
        public abstract void OnAwake(T self);
    }
    [GameLoop]
    public abstract class OnAwakeImpl<T, A> : IAwake<A> where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IAwake<A>);
        void IAwake<A>.OnAwake(ComponentData self, A a) => OnAwake((T)self, a);
        public abstract void OnAwake(T self, A a);
    }
    [GameLoop]
    public abstract class OnAwakeImpl<T, A, B> : IAwake<A, B> where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IAwake<A, B>);
        void IAwake<A, B>.OnAwake(ComponentData self, A a, B b) => OnAwake((T)self, a, b);
        public abstract void OnAwake(T self, A a, B b);
    }
    [GameLoop]
    public abstract class OnAwakeImpl<T, A, B, C> : IAwake<A, B, C> where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IAwake<A, B, C>);
        void IAwake<A, B, C>.OnAwake(ComponentData self, A a, B b, C c) => OnAwake((T)self, a, b, c);
        public abstract void OnAwake(T self, A a, B b, C c);
    }
    [GameLoop]
    public abstract class OnReloadImpl<T> : IReLoad where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IReLoad);
        void IReLoad.OnReload(ComponentData self) => OnReload((T)self);
        public abstract void OnReload(T self);
    }
    [GameLoop]
    public abstract class OnEnableImpl<T> : IEnable where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IEnable);
        void IEnable.OnEnable(ComponentData self) => OnEnable((T)self);
        public abstract void OnEnable(T self);
    }
    [GameLoop]
    public abstract class OnDisableImpl<T> : IDisable where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IDisable);
        void IDisable.OnDisable(ComponentData self) => OnDisable((T)self);
        public abstract void OnDisable(T self);
    }
    [GameLoop]
    public abstract class OnUpdateImpl<T> : IUpdate where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IUpdate);
        void IUpdate.OnUpdate(ComponentData self) => OnUpdate((T)self);
        public abstract void OnUpdate(T self);
    }
    [GameLoop]
    public abstract class OnLateUpdateImpl<T> : ILateUpdate where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(ILateUpdate);
        void ILateUpdate.OnLateUpdate(ComponentData self) => OnLateUpdate((T)self);
        public abstract void OnLateUpdate(T self);
    }
    [GameLoop]
    public abstract class OnDestoryImpl<T> : IDestory where T : ComponentData
    {
        public Type ComponentType => typeof(T);
        public Type GameLoopType => typeof(IDestory);
        void IDestory.OnDestory(ComponentData self) => OnDestory((T)self);
        public abstract void OnDestory(T self);
    }

}
