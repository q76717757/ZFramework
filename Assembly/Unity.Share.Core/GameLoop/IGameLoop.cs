using System;

namespace ZFramework
{
    public interface IAwake
    {
        void Awake();
    }
    public interface IAwake<A>
    {
        void Awake(A a);
    }
    public interface IAwake<A, B>
    {
        void Awake(A a, B b);
    }
    public interface IAwake<A, B, C>
    {
        void Awake(A a, B b, C c);
    }

    public interface IEnable
    {
        void OnEnable();
    }
    public interface IDisable
    {
        void OnDisable();
    }
    public interface IUpdate : IDispose//约束了IDispose接口 用于遍历时将过期组件出列
    {
        void Update();
    }
    public interface ILateUpdate : IDispose
    {
        void LateUpdate();
    }
    public interface IDestory
    {
        void OnDestory();
    }

}