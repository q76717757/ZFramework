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
        Action moveNext;

        ushort ITaskCompletionSource.Ver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        internal AsyncOperationSource(AsyncOperation request)
        {
            this.request = request;
            this.request.completed += Completed;
        }

        void ITaskCompletionSource.TryStart()
        {
            throw new NotImplementedException();
        }

        void Completed(AsyncOperation operation)
        {
            request.completed -= Completed;

            var temp = moveNext;
            moveNext = null;
            temp.Invoke();
        }
        TaskProcessStatus ITaskCompletionSource.GetStatus()
        {
            return request.isDone ? TaskProcessStatus.Completion : TaskProcessStatus.Running;
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            moveNext = continuation;
        }

        void ITaskCompletionSource.Break(Exception exception)
        {
            throw new NotImplementedException();
        }

        void ITaskCompletionSource.GetResultWithNotReturn()
        {
            throw new NotImplementedException();
        }

        Exception ITaskCompletionSource.GetException()
        {
            throw new NotImplementedException();
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
            return ATask.FromSource(new AsyncOperationSource(asyncOperation));
        }
    }
}
#endif