using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZFramework
{
    public static class ATaskUtility //�ֲ��಻�ܿ����
    {
        public static ATask WaitForEndOfFrame() => default;
        public static ATask WaitForFixedUpdate() => default;
        public static ATask WaitUntil(Func<bool> predicate) => default;
        public static ATask WaitWhile() => default;

        //await tween
        //await key
        //await audio/video
        //await process

        //Unity�첽����AO
        //public static ATask WithCancelToken(this ref ATask task, int token) => default;
        //public static ATask WitchConcelCallback(this ref ATask task, Action callback) => default;

    }
}
