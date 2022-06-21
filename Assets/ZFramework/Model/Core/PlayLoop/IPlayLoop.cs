using System;

namespace ZFramework
{
    public interface IPlayLoop
    {
    }
    public interface IAssemblyLoad : IPlayLoop
    {
        void AssemblyLoad(Component component);
    }
    public interface IAwake: IPlayLoop
    {
        void Awake(Component component);
    }
    public interface IUpdate : IPlayLoop
    {
        void Update(Component component);
    }
    public interface ILateUpdate : IPlayLoop
    {
        void LateUpdate(Component component);
    }
    public interface IDestory : IPlayLoop
    {
        void Destory(Component component);
    }
}