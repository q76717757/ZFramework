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
        Action MoveNext;

        internal UnityWebRequestAsyncOperationSource(UnityWebRequestAsyncOperation request)
        {
            this.request = request;
            this.request.completed += Completed;
        }

        void ITaskCompletionSource.Invoke()
        {
            throw new NotImplementedException();
        }
        void Completed(AsyncOperation operation)
        {
            request.completed -= Completed;

            var temp = MoveNext;
            MoveNext = null;
            temp.Invoke();
        }
        void ITaskCompletionSource.OnCompleted(Action continuation)
        {
            MoveNext = continuation;
        }
        DownloadHandler ITaskCompletionSource<DownloadHandler>.GetResult()
        {
            if (request != null)
            {
                var outut = request.webRequest.downloadHandler;
                request = null;
                return outut;
            }
            throw new Exception("Request is Null, 不能调用两次GetResult");
        }
        ATaskStatus ITaskCompletionSource.GetStatus()
        {
            return request.isDone ? ATaskStatus.Success : ATaskStatus.Running;
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
            return new ATask<DownloadHandler>(new UnityWebRequestAsyncOperationSource(asyncOperation));
        }
    }
}
#endif