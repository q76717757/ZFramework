/** Header
 *  TriggerEventListenerGroup.cs
 *  触发器事件的监听组
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public class TriggerEventListenerGroup : ZEventListenerGroupBase<TriggerEventDataBase, TriggerEventListenerBase>
    {
        public GameObject Target { get; private set; }

        internal TriggerEventListenerGroup SetTarget(GameObject target) {
            Target = target;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Target = null;
        }

        internal override bool AutoRemoveJob(TriggerEventListenerBase listener, TriggerEventDataBase eventData)
        {
            return listener.AutoRemoveInEnter && eventData.EventType == TriggerEventType.Enter;
        }
    }
}
