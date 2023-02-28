using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace ZFramework
{
    public readonly struct AsyncTaskMethodBuilder
    {
        //注:Start方法是有点特殊的 在里面的赋值操作不会起作用 通过查看反编译代码可以知道
        //builder.start被调用前先经历了值传递 复制了一个Builder出来 再调的复制体Start方法 实际调用的builder不是原来的builder了(除非builder是引用类型)
        //所以如果builder里面有字段需要赋值  需要将赋值操作在Create方法内实现  且该字段还必须是引用类型  不然没办法对builder里面的字段赋值
        readonly IAsyncMethodRunner<VoidResult> runner;
        private AsyncTaskMethodBuilder(IAsyncMethodRunner<VoidResult> runner) => this.runner = runner;

        //下面的实现是Builder的条件
        public ATask Task => runner.GetTask().ToATask();
        public static AsyncTaskMethodBuilder Create() => new AsyncTaskMethodBuilder(new AsyncMethodSource<VoidResult>());
        [SecuritySafeCritical]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            runner.SetStateMachine(in stateMachine);
        }
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(runner.MoveNext);
        }
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(runner.MoveNext);   //awaiter就是子任务 Runner需要保存子任务  当runner被调取消的时候 通知子任务也取消  除非ATask不作为Awaiter 用类做Awaiter
        }
        public void SetResult()
        {
            runner.SetResult(VoidResult.Empty);
        }
        public void SetException(Exception exception)
        {
            runner.SetException(exception);
        }
        public void SetStateMachine(IAsyncStateMachine _) { }//没用上 结构体直转接口会进行一次自动装箱  上面的泛型接口则不会装箱
    }

    public readonly struct AsyncTaskMethodBuilder<TResult>
    {
        readonly IAsyncMethodRunner<TResult> runner;
        private AsyncTaskMethodBuilder(IAsyncMethodRunner<TResult> runner) => this.runner = runner;

        public ATask<TResult> Task => runner.GetTask();
        public static AsyncTaskMethodBuilder<TResult> Create() => new AsyncTaskMethodBuilder<TResult>(new AsyncMethodSource<TResult>());
        [SecuritySafeCritical]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            runner.SetStateMachine(in stateMachine);
        }
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(runner.MoveNext);
        }
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(runner.MoveNext);
        }
        public void SetResult(TResult value)
        {
            runner.SetResult(value);
        }
        public void SetException(Exception exception)
        {
            runner.SetException(exception);
        }
        public void SetStateMachine(IAsyncStateMachine _) { }

    }
}
