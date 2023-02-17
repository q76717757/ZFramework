#if !SERVER
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZFramework
{
    internal class ResourceRequestSource : ITaskCompletionSource<Object>
    {
        ResourceRequest request;
        Action MoveNext;

        internal ResourceRequestSource(ResourceRequest request)
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
        public static ATask<Object> GetAwaiter(this ResourceRequest asyncOperation)
        {
            if (asyncOperation == null)
            {
                throw new ArgumentNullException(nameof(asyncOperation));
            }
            return new ATask<Object>(new ResourceRequestSource(asyncOperation));
        }
    }

}

#endif