/** Header
 *  ScreenEventData.cs
 *  屏幕指针事件数据容器
 **/


namespace ZFramework
{
    public class ScreenEventDataBase : ZEventDataBase
    {
        /// <summary> 指针的类型 </summary>
        public PointerType Target { get; private set; }
        /// <summary> 触发类型 </summary>
        public ScreenEventType EventType { get; private set; }


        internal void SetStaticData(PointerType target, ScreenEventType eventType)
        {
            Target = target;
            EventType = eventType;
        }
        internal override void Recycle()
        {
        }
    }

    public class ScreenEventData : ScreenEventDataBase
    {
        internal ScreenEventData SetData() {
            return this;
        }
    }
    public class ScreenEventData<D0> : ScreenEventDataBase
    {
        internal ScreenEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
        public D0 Data0 { get; private set; }
    }
    public class ScreenEventData<D0, D1> : ScreenEventDataBase
    {
        internal ScreenEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class ScreenEventData<D0, D1, D2> : ScreenEventDataBase
    {
        internal ScreenEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
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
