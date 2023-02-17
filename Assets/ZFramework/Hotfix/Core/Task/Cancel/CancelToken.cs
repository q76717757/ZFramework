using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public struct CancelToken
    {
        //internal CancelToken(Component component)//组件销毁时取消  看需求 暂时先记着

        Action<TaskCancelType> callback;
        private CancelTokenSource source;

        internal CancelToken(CancelTokenSource source)
        {
            this = default;
            this.source = source;
        }
        internal CancelToken(float time)//基于时间
        {
            this = default;
        }
        public CancelToken OnCancel(Action<TaskCancelType> callback)
        {
            return this;
        }
        public void Cancel()
        {

        }
    }


    public readonly partial struct ATask
    {
        public ATask SetCancelToken(CancelToken token)
        {
            return this;
        }
    }
    public readonly partial struct ATask<TResult>
    {
        public ATask<TResult> SetCancelToken(CancelToken token)
        {
            return this;
        }
    }
}
