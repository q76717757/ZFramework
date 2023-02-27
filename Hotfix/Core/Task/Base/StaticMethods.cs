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

        public static IEnumerator ToIEnumerator() => default;//�첽תIEnumerator
        public static ATask ToATask(IEnumerator enumerator) => default;//IEnumeratorת�첽
        public static ATask ToATask(Task task) => default;//TaskתATask
        public static ATask Break() => default;//�ж����� //�������ж�����Ļ���һ��token����쳣���ݳ�ȥ //ȡ���Լ������ò���??





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
