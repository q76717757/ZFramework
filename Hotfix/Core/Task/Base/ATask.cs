using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZFramework
{
    //[IL2CPP Bug]�ӽṹ���е����ⲿ��ķ���ʱ���������ȷ  �޸���2021.1.24f1
    //��ʱ�޸����� ���ⲿ�෽��ע����ί���� ����ί�дӽṹ���д��� ���ⲿ�����ȥ����ί��
    //Issue is tracked on https://issuetracker.unity3d.com/issues/il2cpp-incorrect-results-when-calling-a-method-from-outside-class-in-a-struct

    //ATask�Ĺ��췽��������
    //һ����Async���� �������������Builder��ʵ������ ��Builder�е�CreateȻ�󷵻�Task
    //��һ�����Լ�new ATask������ �����ھ�̬ATask����, ���ʱ�� ��֡ �Ȼص���..  �������չ�Ķ������,��������Builder·��,�ؼ���GetAwaiter()��ôʵ��
    //new Atask()���ڿ�����  source == null  IsCompleted = true  ֱ�����

    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct ATask : ICriticalNotifyCompletion
    {
        private readonly ITaskCompletionSource source;
        private readonly ushort ver;


        internal ATask(ITaskCompletionSource source)
        {
            this = default;
            this.source = source;
            ver = source.Ver;
        }

        bool CheckSource() => source != null && source.Ver == ver;

        public void Invoke() => GetAwaiter();
        public ATask GetAwaiter()
        {
            if (CheckSource())
            {
                source.TryStart();
            }
            return this;
        }

        //�����ʵ���ǳ�ΪAwaiter������    //ע�� ����ֲ��ȴ�  �ͻ��ε�����ķ�����  ȷ����ѯ�������޺���  �ص�ע����+=��ʽ  ������෽�ȴ�ͬһ�������Ҫ?
        public bool IsCompleted => CheckSource() == false || source.GetStatus() == TaskProcessStatus.Completion;
        public void GetResult()
        {
            if (CheckSource())
            {
                source.GetResultWithNotReturn();
            }
        }
        void INotifyCompletion.OnCompleted(Action continuation) => source.OnCompleted(continuation);
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => source.OnCompleted(continuation);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct ATask<TResult> : ICriticalNotifyCompletion
    {
        private readonly ITaskCompletionSource<TResult> source;
        private readonly ushort ver;
        private readonly TResult result;

        internal ATask(TResult result)
        {
            this = default;
            this.result = result;
        }
        internal ATask(ITaskCompletionSource<TResult> source)
        {
            this = default;
            this.source = source;
            ver = source.Ver;
        }

        bool CheckSource() => source != null && source.Ver == ver;

        public void Invoke() => GetAwaiter();
        public ATask<TResult> GetAwaiter()
        {
            if (CheckSource())
            {
                source.TryStart();
            }
            return this;
        }

        public bool IsCompleted => CheckSource() == false || source.GetStatus() == TaskProcessStatus.Completion;
        public TResult GetResult()
        {
            if (CheckSource())
            {
                return source.GetResult();
            }
            else
            {
                return result;
            }
        }
        void INotifyCompletion.OnCompleted(Action continuation) => source.OnCompleted(continuation);
        void ICriticalNotifyCompletion.UnsafeOnCompleted(Action continuation) => source.OnCompleted(continuation);

        public ATask ToATask() => new ATask(source);
    }

}