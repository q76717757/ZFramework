/** Header
 *  KeyEventDataBase.cs
 *  按键事件数据容器
 **/

using UnityEngine;


namespace ZFramework
{
    public class KeyEventDataBase : ZEventDataBase
    {
        /// <summary> 载体 </summary>
        public KeyCode Target { get; private set; }
        /// <summary> 触发类型 </summary>
        public KeyEventType EventType { get; private set; }

        internal void SetStaticData(KeyCode target, KeyEventType eventType)
        {
            Target = target;
            EventType = eventType;
        }
        internal override void Recycle()
        {
        }
    }

    public class KeyEventData : KeyEventDataBase
    {
        internal KeyEventData SetData() {
            return this;
        }
    }
    public class KeyEventData<D0> : KeyEventDataBase
    {
        internal KeyEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
        public D0 Data0 { get; private set; }
    }
    public class KeyEventData<D0, D1> : KeyEventDataBase
    {
        internal KeyEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class KeyEventData<D0, D1, D2> : KeyEventDataBase
    {
        internal KeyEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
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
