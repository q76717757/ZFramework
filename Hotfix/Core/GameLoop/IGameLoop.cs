using System;

namespace ZFramework
{
    internal interface IGameLoop
    {
        Type GameLoopType { get; }
        Type ComponentType { get; }
    }

    internal interface IAwake : IGameLoop
    {
        void OnAwake(Component self);
    }
    internal interface IReLoad : IGameLoop
    {
        void OnReload(Component self);
    }
    internal interface IEnable : IGameLoop
    {
        void OnEnable(Component self);
    }
    internal interface IDisable : IGameLoop
    {
        void OnDisable(Component self);
    }
    internal interface IUpdate : IGameLoop
    {
        void OnUpdate(Component self);
    }
    internal interface ILateUpdate : IGameLoop
    {
        void OnLateUpdate(Component self);
    }
    internal interface IDestory : IGameLoop
    {
        void OnDestory(Component self);
    }

}