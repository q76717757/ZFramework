using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZFramework
{
    public partial struct ATask
    {
        public static ATask CompletedTask => default;


        public static ATask Delay(float seconds, bool ignoreTimeScale = false) => DD();
        public static ATask NextFrame() => default;
        public static ATask WhenAll() => default;
        public static ATask WhenAny() => default;
        public static ATask SwitchToThreadPool() => default;
        public static ATask SwitchToMainThread() => default;

        public static IEnumerator ToIEnumerator() => default;//异步转IEnumerator
        public static ATask ToATask(IEnumerator enumerator) => default;//IEnumerator转异步
        public static ATask ToATask(Task task) => default;//Task转ATask

        static async ATask DD() => await Task.Delay(2000);


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

        //public Task ToTask(ATask self) => new Task(() => { self.GetAwaiter(); });
    }



}
