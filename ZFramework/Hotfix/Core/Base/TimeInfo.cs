using System;

namespace ZFramework
{
    public class TimeInfo
    {
        private readonly DateTime dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);//EcoTime

        public long ServerMinusClientTime { private get; set; }

        public long FrameTime;

        public TimeInfo()
        {
            FrameTime = ClientNow();
        }

        public void Update()
        {
            FrameTime = ClientNow();
        }

        /// <summary> 
        /// 时间戳 -> 时间 (13位)
        /// </summary>  
        public DateTime ToDateTime(long timeStamp)
        {
            return dt.AddTicks(timeStamp * 10000);
        }
        /// <summary>
        /// 时间 -> 时间戳 (13位)
        /// </summary>
        /// <returns></returns>
        public long ToTimeStamp(DateTime time)
        {
            return time.Ticks / 10000;
        }

        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - dt1970.Ticks) / 10000;
        }

        public long ServerNow()
        {
            return ClientNow() + ServerMinusClientTime;//从心跳来 实时性不够但流量少  从同步来实时性高 流量占用多
        }

    }
}
