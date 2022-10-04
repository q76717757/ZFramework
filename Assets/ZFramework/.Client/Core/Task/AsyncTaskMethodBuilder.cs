using System;
using System.Runtime.CompilerServices;
using System.Security;

namespace ZFramework
{
    public struct AsyncTaskMethodBuilder
    {
        private ZTask task;
        public ZTask Task => task;

        //遇到本await关键字 生成本task对应的状态机
        public static AsyncTaskMethodBuilder Create()
        {
            AsyncTaskMethodBuilder builder = new AsyncTaskMethodBuilder()
            {
                task = ZTask.CreateInstance()
            };
            return builder;
        }
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        //遇到子awaiter 绑定子awaiter的完成回调
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }

        //本状态机结束 给task设置返回值
        public void SetResult()
        {
            task.SetResult();
        }

        //没执行过 不知道干嘛的
        public void SetException(Exception exception)
        {
            Log.Info("AsyncTaskMethodBuilder.SetException-->" + exception.Message);
        }
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Log.Info("AsyncTaskMethodBuilder.SetStateMachine" + stateMachine.ToString());
        }
    }


    public struct AsyncTaskMethodBuilder<T>
    {
        private ZTask<T> task;
        public ZTask<T> Task => task;

        //遇到本await关键字 生成本task对应的状态机
        public static AsyncTaskMethodBuilder<T> Create()
        {
            AsyncTaskMethodBuilder<T> builder = new AsyncTaskMethodBuilder<T>()
            {
                task = ZTask<T>.CreateInstance()
            };
            return builder;
        }
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
        {
            stateMachine.MoveNext();
        }

        //遇到子awaiter 绑定子awaiter的完成回调
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }
        [SecuritySafeCritical]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine) where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine
        {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }

        //本状态机结束 给task设置返回值
        public void SetResult(T value)
        {
            task.SetResult(value);
        }

        //没执行过 不知道干嘛的
        public void SetException(Exception exception)
        {
            Log.Info("AsyncTaskMethodBuilder.SetException-->" + exception.Message);
        }
        public void SetStateMachine(IAsyncStateMachine stateMachine)
        {
            Log.Info("AsyncTaskMethodBuilder.SetStateMachine" + stateMachine.ToString());
        }
    }
}
