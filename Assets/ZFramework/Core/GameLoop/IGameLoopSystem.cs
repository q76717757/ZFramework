using System;

namespace ZFramework
{
    public interface IGameLoopSystem
    {
        Type EntityType { get; }
        Type GameLoopType { get; }
    }

    public interface IAwakeSystem : IGameLoopSystem
    {
        void OnAwake(Entity entity);
    }
    public interface IAwakeSystem<A> : IGameLoopSystem
    {
        void OnAwake(Entity entity, A a);
    }
    public interface IAwakeSystem<A, B> : IGameLoopSystem
    {
        void OnAwake(Entity entity, A a, B b);
    }

    public interface IReLoadSystem : IGameLoopSystem
    {
        void OnReload(Entity entity);
    }
    public interface IEnableSystem : IGameLoopSystem
    {
        void OnEnable(Entity entity);
    }
    public interface IDisableSystem : IGameLoopSystem
    {
        void OnDisable(Entity entity);
    }
    public interface IUpdateSystem : IGameLoopSystem
    {
        void OnUpdate(Entity entity);
    }
    public interface ILateUpdateSystem : IGameLoopSystem
    {
        void OnLateUpdate(Entity entity);
    }
    public interface IDestorySystem : IGameLoopSystem
    {
        void OnDestory(Entity entity);
    }

}