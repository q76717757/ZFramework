/** Header
 *  CollisionEventData.cs
 *  碰撞器事件数据容器
 **/

using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZFramework
{  
    public class CollisionEventDataBase : ZEventDataBase
    {
        /// <summary> 载体 </summary>
        public GameObject Target { get; private set; }
        /// <summary> 碰撞类型 </summary>
        public CollisionEventType EventType { get; private set; }
        /// <summary> 碰撞器 </summary>
        public Collision Collision { get; private set; }

        internal void SetStaticData(GameObject target, CollisionEventType eventType, Collision collision)
        {
            Target = target;
            EventType = eventType;
            Collision = collision;
        }
        internal override void Recycle()
        {
            Target = null;
        }
    }

    public class CollisionEventData : CollisionEventDataBase
    {
        internal CollisionEventData SetData() {
            return this;
        }
    }
    public class CollisionEventData<D0> : CollisionEventDataBase
    {
        internal CollisionEventData<D0> SetData(D0 data0)
        {
            Data0 = data0;
            return this;
        }
        public D0 Data0 { get; private set; }
    }
    public class CollisionEventData<D0, D1> : CollisionEventDataBase
    {
        internal CollisionEventData<D0, D1> SetData(D0 data0, D1 data1)
        {
            Data0 = data0;
            Data1 = data1;
            return this;
        }
        public D0 Data0 { get; private set; }
        public D1 Data1 { get; private set; }
    }
    public class CollisionEventData<D0, D1, D2> : CollisionEventDataBase
    {
        internal CollisionEventData<D0, D1, D2> SetData(D0 data0, D1 data1, D2 data2)
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


