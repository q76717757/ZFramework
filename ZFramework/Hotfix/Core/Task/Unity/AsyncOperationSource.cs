#if !SERVER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    internal class AsyncOperationSource : ITaskCompletionSource
    {
        AsyncOperation request;
        Action MoveNext;

        internal AsyncOperationSource(AsyncOperation request)
        {
            this.request = request;
            this.request.completed += Completed;
        }

        void ITaskCompletionSource.Invoke()
        {
            throw new NotImplementedException("拓展的子任务并不会执行这个,和AssyncMethod上的逻辑不一致");
        }

        void Completed(AsyncOperation operation)
        {
            request.completed -= Completed;

            var temp = MoveNext;
            MoveNext = null;
            temp.Invoke();
        }
        ATaskStatus ITaskCompletionSource.GetStatus()
        {
            return request.isDone ? ATaskStatus.Success : ATaskStatus.Running;
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            MoveNext = continuation;
        }
    }

    public static partial class GetAwaiterExpansions
    {
        public static ATask GetAwaiter(this AsyncOperation asyncOperation)
        {
            if (asyncOperation == null)
            {
                throw new ArgumentNullException(nameof(asyncOperation));
            }
            return new ATask(new AsyncOperationSource(asyncOperation));
        }
    }
}
#endif