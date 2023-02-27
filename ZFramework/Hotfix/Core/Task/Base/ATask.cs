using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZFramework
{
    //[IL2CPP Bug]�ӽṹ���е����ⲿ��ķ���ʱ���������ȷ  �޸���2021.1.24f1
    //��ʱ�޸����� ���ⲿ�෽��ע����ί���� ����ί�дӽṹ���д��� ���ⲿ�����ȥ����ί��
    //Issue is tracked on https://issuetracker.unity3d.com/issues/il2cpp-incorrect-results-when-calling-a-method-from-outside-class-in-a-struct

    //ATask�Ĺ��췽��������
    //һ����Async���� �������������Builder��ʵ������ ��Builder�е�CreateȻ�󷵻�Task
    //��һ�����Լ�new ATask������ �����ھ�̬ATask����, ���ʱ�� ��֡ �Ȼص���..  �������չ�Ķ������,��������Builder·��,�ؼ���GetAwaiter()��ôʵ��
    //new Atask()���ڿ�����  source == null  IsCompleted = true  ֱ�����

    public readonly partial struct ATask : ICriticalNotifyCompletion
    {
        private readonly ITaskCompletionSource source;
        internal ATask(ITaskCompletionSource source) => this.source = source;


        public void Invoke() => GetAwaiter();

        public ATask GetAwaiter()
        {
            source?.Invoke();
            return this;
        }
        public bool IsCompleted => source == null || source.GetStatus().IsCompeleted();
        public void GetResult() { }
        void INotifyCompletion.OnCompleted(Action continuation) => source.OnCompleted(continuation);
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => source.OnCompleted(continuation);
    }

    public readonly partial struct ATask<TResult> : ICriticalNotifyCompletion
    {
        private readonly ITaskCompletionSource<TResult> source;
        internal ATask(ITaskCompletionSource<TResult> source) => this.source = source;

        public void Invoke() => GetAwaiter();

        public ATask<TResult> GetAwaiter()
        {
            source?.Invoke();
            return this;
        }
        public bool IsCompleted => source == null || source.GetStatus().IsCompeleted();
        public TResult GetResult() => source == null ? default : source.GetResult();
        void INotifyCompletion.OnCompleted(Action continuation) => source.OnCompleted(continuation);
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => source.OnCompleted(continuation);
    }

}