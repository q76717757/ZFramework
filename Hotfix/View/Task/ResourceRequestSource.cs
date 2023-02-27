#if !SERVER
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZFramework
{
    internal class ResourceRequestSource : ITaskCompletionSource<Object>
    {
        ResourceRequest request;
        Action moveNext;

        ushort ITaskCompletionSource.Ver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        internal ResourceRequestSource(ResourceRequest request)
        {
            this.request = request;
            this.request.completed += Completed;
        }

        void ITaskCompletionSource.TryStart()
        {
        }

        void Completed(AsyncOperation operation)
        {
            request.completed -= Completed;
            //回收Source

            var temp = moveNext;
            moveNext = null;
            temp.Invoke();
        }
        Object ITaskCompletionSource<Object>.GetResult()
        {
            if (request != null)
            {
                var outut = request.asset;
                request = null;
                return outut;
            }
            throw new Exception("Request is Null, 不能调用两次GetResult");
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
        }

        void ITaskCompletionSource.GetResultWithNotReturn()
        {
        }

        Exception ITaskCompletionSource.GetException()
        {
            throw new NotImplementedException();
        }
    }

    public static partial class GetAwaiterExpansions
    {
        public static ATask<Object> GetAwaiter(this ResourceRequest asyncOperation)
        {
            if (asyncOperation == null)
            {
                throw new ArgumentNullException(nameof(asyncOperation));
            }
            return ATask.FromSource(new ResourceRequestSource(asyncOperation));
        }
    }

}

#endif