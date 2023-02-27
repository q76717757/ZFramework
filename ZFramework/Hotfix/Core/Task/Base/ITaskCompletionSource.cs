using System;

namespace ZFramework
{
    interface ITaskCompletionSource
    {
        void Invoke();//��������
        void OnCompleted(Action continuation);//����������� continuation == �ϲ������MoveNext
        internal ATaskStatus GetStatus();
    }

    // �����첽����Ļ�����λ    TResult = VoidResult => �޷��ص��첽����
    interface ITaskCompletionSource<TResult> : ITaskCompletionSource
    {
        TResult GetResult();
    }
}
