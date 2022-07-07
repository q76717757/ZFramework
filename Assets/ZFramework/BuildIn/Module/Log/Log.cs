using System;

namespace ZFramework
{
    public static class Log
    {
        public static ILog ILog { get; set; }

        public static void Info(object obj)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.Log(obj);
#else
            ILog.Info(obj);
#endif
        }

        public static void Warning(object obj)
        {
#if UNITY_EDITOR
            UnityEngine.Debug.LogWarning(obj);
#else
            ILog.Warning(obj);
#endif
        }

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