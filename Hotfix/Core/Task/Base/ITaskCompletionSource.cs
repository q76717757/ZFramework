using System;

namespace ZFramework
{
    interface ITaskCompletionSource
    {
        void Invoke();//启动任务
        void OnCompleted(Action continuation);//本任务已完成 continuation == 上层任务的MoveNext
        internal ATaskStatus GetStatus();
    }

    // 这是异步任务的基本单位    TResult = VoidResult => 无返回的异步任务
    interface ITaskCompletionSource<TResult> : ITaskCompletionSource
    {
        TResult GetResult();
    }
}
