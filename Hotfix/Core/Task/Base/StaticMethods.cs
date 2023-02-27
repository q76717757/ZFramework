using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZFramework
{
    public readonly partial struct ATask
    {
        public Task ToTask(ATask self) => new Task(() => { self.GetAwaiter(); });
       

        public static ATask CompletedTask => new ATask();
        public static ATask WhenAll() => default;
        public static ATask WhenAny() => default;
        public static ATask SwitchToThreadPool() => default;
        public static ATask SwitchToMainThread() => default;

        public static IEnumerator ToIEnumerator() => default;//异步转IEnumerator
        public static ATask ToATask(IEnumerator enumerator) => default;//IEnumerator转异步
        public static ATask ToATask(Task task) => default;//Task转ATask
        public static ATask Break() => default;//中断任务 //从里面中断任务的话抛一个token标记异常传递出去 //取消自己好像用不上??





        public static ATask<TResult> FromResult<TResult>(TResult result)
        {
            return new ATask<TResult>(result);
        }
        public static ATask<TResult> FromSource<TResult>(ITaskCompletionSource<TResult> source)
        {
            return new ATask<TResult>(source);
        }
        public static ATask FromSource(ITaskCompletionSource source)
        {
            return new ATask(source);
        }

    }



}
