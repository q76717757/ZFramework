using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ZFramework
{
    public static class Log
    {
        public static ILog ILog { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(object obj)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(obj);
#else
            ILog.Info(obj);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warning(object obj)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(obj);
#else
            ILog.Warning(obj);
#endif
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(object obj)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogError(obj);
#else
            ILog.Error(obj);
#endif
        }
    }
}