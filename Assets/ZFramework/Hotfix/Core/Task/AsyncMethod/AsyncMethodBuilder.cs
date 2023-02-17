using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

namespace ZFramework
{
    public readonly struct AsyncTaskMethodBuilder
    {
        //注:Start方法是有点特殊的 在里面的赋值操作不会起作用 通过查看反编译代码可以知道
        //builder.start被调用前先经历了值传递 复制了一个Builder出来 再调的Start方法 这时候builder不是原来的builder了(除非builder是引用类型)
        //所以如果builder里面有字段需要赋值  需要将赋值操作在Create方法内实现  且该字段还必须是引用类型  不然没办法对builder里面的字段赋值
        readonly AsyncMethodSource runner;
        public ATask Task => runner.Task;

        private AsyncTaskMethodBuilder(AsyncMethodSource runner)
        {
            this.runner = runner;
        }
        public static AsyncTaskMethodBuilder Create()
        {
            return new AsyncTaskMethodBuilder(new AsyncMethodSource());
        }

        [SecuritySafeCritical]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            runner.Start(stateMachine.MoveNext);
        }
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }
        public void SetResult()
        {
            runner.SetResult();
        }
        public void SetException(Exception exception)
        {
            runner.SetException(exception);
        }
        public void SetStateMachine(IAsyncStateMachine stateMachine) { }//没用上
    }
    public readonly struct AsyncTaskMethodBuilder<TResult>
    {
        readonly AsyncMethodSource<TResult> runner;
        public ATask<TResult> Task => runner.Task;

        private AsyncTaskMethodBuilder(AsyncMethodSource<TResult> runner)
        {
            this.runner = runner;
        }
        public static AsyncTaskMethodBuilder<TResult> Create()
        { 
            return new AsyncTaskMethodBuilder<TResult>(new AsyncMethodSource<TResult>());
        }

        [SecuritySafeCritical]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            runner.Init(stateMachine.MoveNext);
        }
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }
        public void SetResult(TResult value)
        {
            runner.SetResult(value);
        }
        public void SetException(Exception exception)
        {
            runner.SetException(exception);
        }
        public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    }
}
