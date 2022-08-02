/** Header
 *  TriggerEventData.cs
 *  触发器事件数据容器
 **/

using UnityEngine;


namespace ZFramework
{
    public class TriggerEventDataBase : ZEventDataBase
    {
        /// <summary> 载体 </summary>
        public GameObject Target { get; private set; }
        /// <summary> 触发类型 </summary>
        public TriggerEventType EventType { get; private set; }
        /// <summary> 触发器 </summary>
        public Collider Other { get; private set; }


        internal void SetStaticData(GameObject target, TriggerEventType eventType, Collider other)
        {
            Target = target;
            EventType = eventType;
            Other = other;
        }
        internal override void Recycle()
        {
            Target = null;
        }
    }

    public class TriggerEventData : TriggerEventDataBase
    {
        internal TriggerEventData SetData() {
            return this;
        }
    }
    public class TriggerEventData<D0> : TriggerEventDataBase
    {
        internal TriggerEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
        public D0 Data0 { get; private set; }
    }
    public class TriggerEventData<D0, D1> : TriggerEventDataBase
    {
        internal TriggerEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class TriggerEventData<D0, D1, D2> : TriggerEventDataBase
    {
        internal TriggerEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
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
