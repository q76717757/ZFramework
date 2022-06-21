/** Header
 *  TimerEventData.cs
 *  计时器事件数据容器
 **/

namespace ZFramework
{
    public class TimerEventDataBase : ZEventDataBase
    {
        /// <summary> 驱动的计时器 </summary>
        public Timer Target { get; private set; }
        /// <summary> 事件类型 </summary>
        public TimerEventType EventType { get; private set; }

        internal void SetStaticData(Timer target, TimerEventType eventType)
        {
            Target = target;
            EventType = eventType;
        }
        internal override void Recycle()
        {
            Target = null;
        }
    }

    public class TimerEventData : TimerEventDataBase
    {
        internal TimerEventData SetData() {
            return this;
        }
    }
    public class TimerEventData<D0> : TimerEventDataBase
    {
        internal TimerEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
        public D0 Data0 { get; private set; }
    }
    public class TimerEventData<D0, D1> : TimerEventDataBase
    {
        internal TimerEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class TimerEventData<D0, D1, D2> : TimerEventDataBase
    {
        internal TimerEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
        {
            Data0 = data0;
            Data1 = data1;
            Data2 = data2;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
        public D2 Data2 { get; private set; }
    }
}
