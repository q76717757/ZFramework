using System;

namespace ZFramework
{
    public interface IAwake
    {
        void Awake();
    }
    public interface IEnable
    {
        void OnEnable();
    }
    public interface IDisable
    {
        void OnDisable();
    }
    public interface IUpdate : IDispose//Լ����IDispose�ӿ� ���ڱ���ʱ�������������
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