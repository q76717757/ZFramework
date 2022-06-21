using System;

namespace ZFramework
{
    public static class Log
    {
        public static ILog ILog { get; set; }

        public static void Info(object obj)
        {
            ILog.Info(obj);
        }

        public static void Warning(object obj)
        {
            ILog.Warning(obj);
        }

        public static void Error(object obj)
        {
            ILog.Error(obj);
        }
    }
}