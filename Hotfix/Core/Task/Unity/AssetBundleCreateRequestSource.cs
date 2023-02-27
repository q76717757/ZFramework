#if !SERVER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ZFramework
{
    internal class AssetBundleCreateRequestSource : ITaskCompletionSource<AssetBundle>
    {
        AssetBundleCreateRequest request;
        Action MoveNext;

        internal AssetBundleCreateRequestSource(AssetBundleCreateRequest request)
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
        ATaskStatus ITaskCompletionSource.GetStatus()
        {
            return request.isDone ? ATaskStatus.Success : ATaskStatus.Running;
        }
    }

    public static partial class GetAwaiterExpansions
    {
        public static ATask<AssetBundle> GetAwaiter(this AssetBundleCreateRequest asyncOperation)
        {
            if (asyncOperation == null)
            {
                throw new ArgumentNullException(nameof(asyncOperation));
            }
            return new ATask<AssetBundle>(new AssetBundleCreateRequestSource(asyncOperation));
        }
    }
}
#endif