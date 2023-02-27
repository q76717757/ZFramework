using System;

namespace ZFramework
{
    /// <summary>
    /// 无返回的任务源
    /// </summary>
    public interface ITaskCompletionSource  //从有返回的源抽象出来  为了让有返回的源可以隐式转换到无返回源, 但反过来不行
    {
        ushort Ver { get; set; }
        //void Recycle();
        Exception GetException();

        void TryStart();
        /// <summary>
        /// 用于注册本任务完成后的回调 continuation == 父任务的MoveNext  存在分布等待的情况   一个任务可能有多个父 用+=?
        /// </summary>
        void OnCompleted(Action continuation);
        TaskProcessStatus GetStatus();
        /// <summary>
        /// 调用方是父任务  本任务完成会Call父任务MoveNext   父任务的MoveNext反过来Call this.GetResult
        /// </summary>
        void GetResultWithNotReturn();
        /// <summary>
        /// 通过异常强制完成任务
        /// </summary>
        void Break(Exception exception);//中断任务 如果是超时错误就传超时异常  如果是方法异常就把具体的异常传进去?
    }

    /// <summary>
    /// 有返回的任务源
    /// </summary>
    public interface ITaskCompletionSource<out TResult> : ITaskCompletionSource
    {
        /// <summary>
        /// 调用方是父任务  本任务完成会Call父任务MoveNext   父任务的MoveNext反过来Call this.GetResult
        /// </summary>
        TResult GetResult();
    }
}
