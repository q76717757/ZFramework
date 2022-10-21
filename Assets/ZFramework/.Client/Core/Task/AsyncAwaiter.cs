using System;
using System.Runtime.CompilerServices;

namespace ZFramework
{
    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder))]
    public sealed class AsyncTask : ICriticalNotifyCompletion
    {
        internal AwaiterStatus status;
        private Action moveNext;//由父task的状态机注册进来的MoveNext委托  在本task完成之后就会执行父Task状态机MoveNext

        private AsyncTask() { }
        internal static AsyncTask CreateInstance()
        {
            var task = new AsyncTask();
            task.status = AwaiterStatus.Doing;
            return task;
        }

        public AsyncTask GetAwaiter()
        {
            return this;
        }
        public bool IsCompleted
        {
            get
            {
                return status != AwaiterStatus.Doing;
            }
        }

        //把父task的状态机MoveNext 绑定到  本task的完成回调上
        public void OnCompleted(Action continuation)
        {
            if (status != AwaiterStatus.Doing)
            {
                continuation?.Invoke();
            }
            else
            {
                moveNext = continuation;
            }
        }
        public void UnsafeOnCompleted(Action continuation)
        {
            if (status != AwaiterStatus.Doing)
            {
                continuation?.Invoke();
            }
            else
            {
                moveNext = continuation;
            }
        }


        //本task完成 设置返回值  然后执行父状态机MoveNext的委托
        public void SetResult()
        {
            status = AwaiterStatus.Finish;
            var moveNext = this.moveNext;
            this.moveNext = null;
            moveNext?.Invoke();
        }
        public void GetResult()
        {
        }

    }


    [AsyncMethodBuilder(typeof(AsyncTaskMethodBuilder<>))]
    public class AsyncTask<T> : ICriticalNotifyCompletion
    {
        private AwaiterStatus status;
        private Action moveNext;
        private T value;

        private AsyncTask() { }
        internal static AsyncTask<T> CreateInstance()
        {
            var task = new AsyncTask<T>();
            task.status = AwaiterStatus.Doing;
            return task;
        }

        public AsyncTask<T> GetAwaiter()
        {
            return this;
        }
        public bool IsCompleted
        {
            get
            {
                return status != AwaiterStatus.Doing;
            }
        }

        public void OnCompleted(Action continuation)
        {
            if (status != AwaiterStatus.Doing)
            {
                continuation?.Invoke();
            }
            else
            {
                moveNext = continuation;
            }
        }
        public void UnsafeOnCompleted(Action continuation)
        {
            if (status != AwaiterStatus.Doing)
            {
                continuation?.Invoke();
            }
            else
            {
                moveNext = continuation;
            }
        }

        public void SetResult(T value)
        {
            this.value = value;
            status = AwaiterStatus.Finish;
            var moveNext = this.moveNext;
            this.moveNext = null;
            moveNext?.Invoke();
        }
        public T GetResult()
        {
            return value;
        }

    }


    [AsyncMethodBuilder(typeof(AsyncVoidMethodBuilder))]
    public struct AsyncVoid
    {
        public void Coroutine()
        {
        }
    }
}
