using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZFramework
{
    public partial struct ATask
    {
        public static ATask Delay(float seconds, bool ignoreTimeScale = false) => default;
        public static ATask NextFrame() => default;
        public static ATask WaitForEndOfFrame() => default;
        public static ATask WaitForFixedUpdate() => default;
        public static ATask WaitUntil(Func<bool> predicate) => default;
        public static ATask WaitWhile() => default;

        //await tween
        //await key
        //await audio/video
        //await process
        public static IEnumerator ToIEnumerator() => default;//�첽תIEnumerator
        public static ATask ToATask(IEnumerator enumerator) => default;//IEnumeratorת�첽

        public static ATask ToATask(Task task) => default;//TaskתATask
        public static ATask Break() => default;//�ж����� //�������ж�����Ļ���һ�������쳣���ݳ�ȥ

        //Unity�첽����AO
        //public static ATask WithCancelToken(this ref ATask task, int token) => default;
        //public static ATask WitchConcelCallback(this ref ATask task, Action callback) => default;

    }
}
