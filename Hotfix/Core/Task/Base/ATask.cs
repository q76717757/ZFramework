using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ZFramework
{
    //[IL2CPP Bug]从结构体中调用外部类的方法时，结果不正确  修复于2021.1.24f1
    //暂时修复方法 将外部类方法注册在委托上 并将委托从结构体中传出 给外部类对象去调用委托
    //Issue is tracked on https://issuetracker.unity3d.com/issues/il2cpp-incorrect-results-when-calling-a-method-from-outside-class-in-a-struct

    //ATask的构造方法分两种
    //一种是Async方法 被编译器编译成Builder的实例方法 在Builder中调Create然后返回Task
    //另一种是自己new ATask出来的 常见于静态ATask任务, 如等时间 等帧 等回调等..  大多数扩展的都是这个,并不会走Builder路径,关键在GetAwaiter()怎么实现
    //new Atask()等于空任务  source == null  IsCompleted = true  直接完成

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

        //下面的实现是成为Awaiter的条件    //注意 如果分布等待  就会多次调下面的方法了  确保查询方法是无害的  回调注册用+=方式  以满足多方等待同一任务的需要?
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