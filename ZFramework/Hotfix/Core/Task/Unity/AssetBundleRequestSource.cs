#if !SERVER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    internal class AssetBundleRequestSource : ITaskCompletionSource<AssetBundleRequest>
    {
        AssetBundleRequest request;
        Action MoveNext;

        internal AssetBundleRequestSource(AssetBundleRequest request)
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
        AssetBundleRequest ITaskCompletionSource<AssetBundleRequest>.GetResult()
        {
            if (request != null)
            {
                var outut = request;
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
        public static ATask<AssetBundleRequest> GetAwaiter(this AssetBundleRequest asyncOperation)
        {
            if (asyncOperation == null)
            {
                throw new ArgumentNullException(nameof(asyncOperation));
            }
            return new ATask<AssetBundleRequest>(new AssetBundleRequestSource(asyncOperation));
        }
    }
}
#endif