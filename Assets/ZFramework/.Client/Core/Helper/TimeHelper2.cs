using System;

namespace ZFramework
{
    public static class TimeHelper
    {
        private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;//单位是100纳秒/0.1微秒

        /// <summary> 现在的时间(UTC,秒) </summary>
        public static long NowSeconds()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000000;
        }

        /// <summary> 现在的时间 (UTC,毫秒 ,13位时间戳) </summary>
        public static long Now()
        {
            return (DateTime.UtcNow.Ticks - epoch) / 10000;
        }
    }

}
