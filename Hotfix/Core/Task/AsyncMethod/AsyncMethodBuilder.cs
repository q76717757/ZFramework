using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace ZFramework
{
    public readonly struct AsyncTaskMethodBuilder
    {
        //注:Start方法是有点特殊的 在里面的赋值操作不会起作用 通过查看反编译代码可以知道
        //builder.start被调用前先经历了值传递 复制了一个Builder出来 再调的复制体Start方法 实际调用的builder不是原来的builder了(除非builder是引用类型)
        //所以如果builder里面有字段需要赋值等(Runner.Set)  需要将赋值操作在Create方法内实现  且该字段还必须是引用类型  不然没办法对builder里面的字段赋值
        readonly IAsyncMethodRunner<VoidResult> runner;
        private AsyncTaskMethodBuilder(IAsyncMethodRunner<VoidResult> runner) => this.runner = runner;

        //下面的实现是Builder的条件
        public ATask Task => runner.GetTask();
        public static AsyncTaskMethodBuilder Create() => new AsyncTaskMethodBuilder(new AsyncMethodSource<VoidResult>());
        [SecuritySafeCritical]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            runner.SetStateMachine(in stateMachine);
        }
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            if (typeof(TAwaiter) == typeof(ATask))
            {
                var code =  awaiter.GetHashCode(); //GetHashCode重写过   通过code作为索引  标记renner  通过索引树 建立任务链条  从而实现任务自动递归取消
                //var childRunner = GetChildRunner(code);
                //Runner.SetChild(childRunner);

                //通过code  去字典找对应的Runner   然后告诉Runner  这个就是他的新儿子  当runner终结的时候  就顺便递归通知他的儿子取消
                //当Runner.Break() -->  GetChildRunner  -> Break()
            }
            else
            {
                //awaiter.OnCompleted(runner.MoveNext);
            }
            awaiter.OnCompleted(runner.MoveNext);
        }
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine _)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(runner.MoveNext);
        }
        public void SetResult()
        {
            runner.SetResult(VoidResult.Empty);
        }
        public void SetException(Exception exception)
        {
            runner.SetException(exception);
        }
        public void SetStateMachine(IAsyncStateMachine _) { }//没用上 0结构体转接口会装箱
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
