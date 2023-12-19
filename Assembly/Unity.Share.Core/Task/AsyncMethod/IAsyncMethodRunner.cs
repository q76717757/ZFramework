using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    interface IAsyncMethodRunner<TResult> : ITaskCompletionSource<TResult>
    {
        internal ATask<TResult> GetTask();
        internal void SetStateMachine<TStateMachine>(in TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine;
        /// <summary>
        /// 其他awaiter的完成时发起调用  调用的来源并不完全可控 特别是原生Task
        /// </summary>
        internal void MoveNext();
        /// <summary>
        /// 只有本级异常 没有子级异常!!  并且中断状态机的执行  想要拿到子级异常  需要子级强制完成任务 并且在本级尝试拿子级结果的时候 子级抛异常出来
        /// </summary>
        internal void SetException(Exception exception);
        /// <summary>
        /// 本级任务完成最后一个调的方法 
        /// </summary>
        internal void SetResult(TResult result);
    }
}
