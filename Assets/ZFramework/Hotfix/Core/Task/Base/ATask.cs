using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ZFramework
{
    //[IL2CPP Bug]从结构体中调用外部类的方法时，结果不正确  修复于2021.1.24f1
    //暂时修复方法 将外部类方法注册在委托上 并将委托从结构体中传出 给外部类对象去调用委托
    //Issue is tracked on https://issuetracker.unity3d.com/issues/il2cpp-incorrect-results-when-calling-a-method-from-outside-class-in-a-struct

    //ATask的构造方法分两种
    //一种是Async方法 被编译器编译成Builder的实例方法 在Builder中调Create然后返回Task
    //另一种是自己new ATask出来的 常见于静态ATask任务, 如等时间 等帧 等回调等..  大多数扩展的都是这个,并不会走Builder路径,关键在GetAwaiter()怎么实现
    //new Atask()等于空任务  source == null  IsCompleted = true  直接完成

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