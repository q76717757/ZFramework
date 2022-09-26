/** Header
 *  UnitEventListenerGroup.cs
 *  Unit事件监听组
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public class UnitEventListenerGroup : ZEventListenerGroupBase<UnitEventDataBase, UnitEventListenerBase>
    {
        public GameObject Target { get; private set; }

        internal UnitEventListenerGroup SetTarget(GameObject target) { 
            Target = target;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Target = null;
        }

        internal override bool AutoRemoveJob(UnitEventListenerBase listener, UnitEventDataBase eventData)
        {
            return listener.AutoRemoveInClick && eventData.EventType == UnitEventType.Click;
        }
    }
}
