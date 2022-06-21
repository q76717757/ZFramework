/** Header
 *  UnitEventData.cs
 *  Unit事件数据容器
 **/

using UnityEngine;
using UnityEngine.EventSystems;


namespace ZFramework
{
    public class UnitEventDataBase : ZEventDataBase
    {
        /// <summary> 载体 </summary>
        public GameObject Target { get; private set; }
        /// <summary> 触发类型 </summary>
        public UnitEventType EventType { get; private set; }
        /// <summary> 指针类型 </summary>
        public PointerType PointerType { get; private set; }
        /// <summary> 触发时指针坐标 </summary>
        public Vector2 Position { get; private set; }
        /// <summary> Unity.EventSystems发送的数据 </summary>
        public PointerEventData UnityEventData { get; private set; }

        internal void SetStaticData(GameObject target, UnitEventType eventType, PointerEventData unityEventData)
        {
            Target = target;
            EventType = eventType;
            PointerType = (PointerType)unityEventData.pointerId;
            Position = unityEventData.position;
            UnityEventData = unityEventData;
        }
        internal override void Recycle()
        {
            Target = null;
        }
    }

    public class UnitEventData : UnitEventDataBase
    {
        internal UnitEventData SetData() {
            return this;
        }
    }
    public class UnitEventData<D0> : UnitEventDataBase
    {
        public D0 Data0 { get; private set; }
        internal UnitEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
    }
    public class UnitEventData<D0, D1> : UnitEventDataBase
    {
        internal UnitEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class UnitEventData<D0, D1, D2> : UnitEventDataBase
    {
        internal UnitEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
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
