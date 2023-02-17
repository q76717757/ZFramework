using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    public enum TaskCancelType
    {
        /// <summary>
        /// 手动取消
        /// </summary>
        Manual,
        /// <summary>
        /// 超时
        /// </summary>
        Timeout,
        /// <summary>
        /// 错误
        /// </summary>
        Faulted,
    }

    internal class CancelTokenSource
    {
        Action<TaskCancelType> callback;

        public bool IsCanceled;//已取消
        public bool CanBeCanceled;//可取消


        public void Cancel()
        {
        }
        public void CancelAfter(TimeSpan delay)
        { 
        }
        public void Register(Action callback)
        {
        }
    }
}
