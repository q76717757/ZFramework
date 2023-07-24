using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ZFramework
{
    public static class Log
    {
        public static ILogger ILog { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Info(object obj)
        {
            ILog.Info(obj);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warning(object obj)
        {
            ILog.Warning(obj);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Error(object obj)
        {
            ILog.Error(obj);
        }
    }
}