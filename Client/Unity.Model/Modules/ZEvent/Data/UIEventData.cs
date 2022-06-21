/** Header
 *  UIEventData.cs
 *  UI事件数据容器
 **/

using UnityEngine;
using UnityEngine.EventSystems;

namespace ZFramework
{
    public class UIEventDataBase : ZEventDataBase
    {
        /// <summary> 载体 </summary>
        public GameObject Target { get; private set; }
        /// <summary> 事件类型 </summary>
        public UIEventType EventType { get; private set; }
        /// <summary> 指针类型 </summary>
        public PointerType PointerType { get; private set; }
        /// <summary> 触发时的指针坐标 </summary>
        public Vector2 Position { get; private set; }
        /// <summary> Unity.EventSystems发送的数据 </summary>
        public PointerEventData UnityEventData { get; private set; }

        internal void SetStaticData(GameObject target, UIEventType eventType, PointerEventData unityEventData)
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

    public class UIEventData : UIEventDataBase
    {
        internal UIEventData SetData() {
            return this;
        }
    }
    public class UIEventData<D0> : UIEventDataBase
    {
        internal UIEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
        public D0 Data0 { get; private set; }
    }
    public class UIEventData<D0, D1> : UIEventDataBase
    {
        internal UIEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class UIEventData<D0, D1, D2> : UIEventDataBase
    {
        internal UIEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
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
    //more...
}
