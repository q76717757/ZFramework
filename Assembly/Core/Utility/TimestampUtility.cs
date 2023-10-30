using System;

namespace ZFramework
{
    public enum TimeUnit
    {
        Milliseconds = 0,

        Seconds,

        Minutes,

        Hours,

        Days,
    }

    public sealed class TimestampUtility
    {
        // utc start time
        public static readonly DateTime UTC_START_TIME = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
       
        public static long GetCurrentTimestamp(TimeUnit timeUnit)
        {
            TimeSpan timeSpan = DateTime.UtcNow - UTC_START_TIME;
            long result = -1L;

            switch (timeUnit)
            {
                case TimeUnit.Days:
                    result = (long)timeSpan.TotalDays;
                    break;
                case TimeUnit.Hours:
                    result = (long)timeSpan.TotalHours;
                    break;
                case TimeUnit.Minutes:
                    result = (long)timeSpan.TotalMinutes;
                    break;
                case TimeUnit.Seconds:
                    result = (long)timeSpan.TotalSeconds;
                    break;
                case TimeUnit.Milliseconds:
                    result = (long)timeSpan.TotalMilliseconds;
                    break;
            }
            return result;
        }

        public static string GetFormatTime(long timestamp, TimeUnit timeUnit, string format)
        {
            DateTime start = UTC_START_TIME;
            DateTime end = DateTime.MinValue;

            switch (timeUnit)
            {
                case TimeUnit.Days:
                    end = start.AddDays(timestamp);
                    break;
                case TimeUnit.Hours:
                    end = start.AddHours(timestamp);
                    break;
                case TimeUnit.Minutes:
                    end = start.AddMinutes(timestamp);
                    break;
                case TimeUnit.Seconds:
                    end = start.AddSeconds(timestamp);
                    break;
                case TimeUnit.Milliseconds:
                    end = start.AddMilliseconds(timestamp);
                    break;
            }
            end = TimeZone.CurrentTimeZone.ToLocalTime(end);
            return end.ToString(format);
        }
    }

}
