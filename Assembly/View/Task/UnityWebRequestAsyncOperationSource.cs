#if !SERVER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ZFramework
{
    internal class UnityWebRequestAsyncOperationSource : ITaskCompletionSource<DownloadHandler>
    {
        UnityWebRequestAsyncOperation request;
        Action moveNext;

        ushort ITaskCompletionSource.Ver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        internal UnityWebRequestAsyncOperationSource(UnityWebRequestAsyncOperation request)
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
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            moveNext = continuation;
        }
        DownloadHandler ITaskCompletionSource<DownloadHandler>.GetResult()
        {
            if (request != null)
            {
                var outut = request.webRequest.downloadHandler;
                request = null;
                return outut;
            }
            throw new Exception("Request is Null");//不能调用两次GetResult?? 那多重等待怎么办?
        }
        TaskProcessStatus ITaskCompletionSource.GetStatus()
        {
            return request.isDone ? TaskProcessStatus.Completion : TaskProcessStatus.Running;
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
        public static ATask<DownloadHandler> GetAwaiter(this UnityWebRequestAsyncOperation asyncOperation)
        {
            if (asyncOperation == null)
            {
                throw new ArgumentNullException(nameof(asyncOperation));
            }
            return ATask.FromSource(new UnityWebRequestAsyncOperationSource(asyncOperation));
        }
    }
}
#endif