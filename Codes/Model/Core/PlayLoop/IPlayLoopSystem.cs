using System;

namespace ZFramework
{
    public interface IPlayLoopSystem
    {
        Type ComponentType { get; }
        Type PlayLoopType { get; }
    }
    public interface IReLoad : IPlayLoopSystem
    {
        void Reload(Component component);
    }
    public interface IAwake : IPlayLoopSystem
    {
        void Awake(Component component);
    }
    public interface IUpdate : IPlayLoopSystem
    {
        void Update(Component component);
    }
    public interface ILateUpdate : IPlayLoopSystem
    {
        void LateUpdate(Component component);
    }
    public interface IDestory : IPlayLoopSystem
    {
        void Destory(Component component);
    }
    [Live]
    public abstract class AssemblyloadSystem<T> : IReLoad where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type PlayLoopType => typeof(IReLoad);
        void IReLoad.Reload(Component component) => Reload((T)component);
        public abstract void Reload(T component);
    }
    [Live]
    public abstract class AwakeSystem<T> :  IAwake where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type PlayLoopType => typeof(IAwake);
        void IAwake.Awake(Component component) => Awake((T)component);
        public abstract void Awake(T component);
    }
    [Live]
    public abstract class UpdateSystem<T> : IUpdate where T : Component
    {
        public Type ComponentType => typeof(T); 
        public Type PlayLoopType => typeof(IUpdate);
        void IUpdate.Update(Component component) => Update((T)component);
        public abstract void Update(T component);
    }
    [Live]
    public abstract class LateUpdateSystem<T> : ILateUpdate where T : Component
    {
        public Type ComponentType => typeof(T); 
        public Type PlayLoopType => typeof(ILateUpdate);
        void ILateUpdate.LateUpdate(Component component)=> LateUpdate((T)component);
        public abstract void LateUpdate(T component);
    }
    [Live]
    public abstract class DestorySystem<T> : IDestory where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type PlayLoopType => typeof(IDestory);
        void IDestory.Destory(Component component) => Destory((T)component);
        public abstract void Destory(T component);
    }
}