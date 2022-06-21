using System;

namespace ZFramework
{
    public interface IPlayLoopSystem
    {
        Type ComponentType { get; }
        Type PlayLoopType { get; }
    }

    [Live]
    public abstract class AssemblyloadSystem<T> : IPlayLoopSystem, IAssemblyLoad where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type PlayLoopType => typeof(IAssemblyLoad);
        void IAssemblyLoad.AssemblyLoad(Component component) => AssemblyLoad((T)component);
        public abstract void AssemblyLoad(T component);
    }
    [Live]
    public abstract class AwakeSystem<T> : IPlayLoopSystem , IAwake where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type PlayLoopType => typeof(IAwake);
        void IAwake.Awake(Component component) => Awake((T)component);
        public abstract void Awake(T component);
    }
    [Live]
    public abstract class UpdateSystem<T> : IPlayLoopSystem,IUpdate where T : Component
    {
        public Type ComponentType => typeof(T); 
        public Type PlayLoopType => typeof(IUpdate);
        void IUpdate.Update(Component component) => Update((T)component);
        public abstract void Update(T component);
    }
    [Live]
    public abstract class LateUpdateSystem<T> : IPlayLoopSystem,ILateUpdate where T : Component
    {
        public Type ComponentType => typeof(T); 
        public Type PlayLoopType => typeof(ILateUpdate);
        void ILateUpdate.LateUpdate(Component component)=> LateUpdate((T)component);
        public abstract void LateUpdate(T component);
    }
    [Live]
    public abstract class DestorySystem<T> : IPlayLoopSystem,IDestory where T : Component
    {
        public Type ComponentType => typeof(T);
        public Type PlayLoopType => typeof(IDestory);
        void IDestory.Destory(Component component) => Destory((T)component);
        public abstract void Destory(T component);
    }
}