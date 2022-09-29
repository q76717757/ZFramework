using System;

namespace ZFramework
{
    public interface IGameLoop
    {
        Type GameLoopType { get; }
        Type ComponentType { get; }
    }

    public interface IAwake : IGameLoop
    {
        void OnAwake(ComponentData self);
    }
    public interface IAwake<A> : IGameLoop
    {
        void OnAwake(ComponentData self, A a);
    }
    public interface IAwake<A, B> : IGameLoop
    {
        void OnAwake(ComponentData self, A a, B b);
    }
    public interface IAwake<A, B, C> : IGameLoop
    {
        void OnAwake(ComponentData self, A a, B b, C c);
    }
    public interface IReLoad : IGameLoop
    {
        void OnReload(ComponentData self);
    }
    public interface IEnable : IGameLoop
    {
        void OnEnable(ComponentData self);
    }
    public interface IDisable : IGameLoop
    {
        void OnDisable(ComponentData self);
    }
    public interface IUpdate : IGameLoop
    {
        void OnUpdate(ComponentData self);
    }
    public interface ILateUpdate : IGameLoop
    {
        void OnLateUpdate(ComponentData self);
    }
    public interface IDestory : IGameLoop
    {
        void OnDestory(ComponentData self);
    }

}