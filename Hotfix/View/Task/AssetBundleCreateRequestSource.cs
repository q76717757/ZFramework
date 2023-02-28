#if !SERVER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ZFramework
{
    internal class AssetBundleCreateRequestSource : ITaskCompletionSource<AssetBundle>
    {
        AssetBundleCreateRequest request;
        Action moveNext;

        ushort ITaskCompletionSource.Ver { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        internal AssetBundleCreateRequestSource(AssetBundleCreateRequest request)
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
        AssetBundle ITaskCompletionSource<AssetBundle>.GetResult()
        {
            if (request != null)
            {
                var outut = request.assetBundle;
                request = null;
                return outut;
            }
            throw new Exception("Request is Null, 不能调用两次GetResult");
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

    static partial class GetAwaiterExpansions
    {
        public static ATask<AssetBundle> GetAwaiter(this AssetBundleCreateRequest asyncOperation)
        {
            if (asyncOperation == null)
            {
                throw new ArgumentNullException(nameof(asyncOperation));
            }
            return ATask.FromSource(new AssetBundleCreateRequestSource(asyncOperation));
        }
    }
}
#endif