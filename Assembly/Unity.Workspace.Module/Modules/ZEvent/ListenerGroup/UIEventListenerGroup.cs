/** Header
 *  UIEventListenerGroup.cs
 *  UI事件的监听组
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public class UIEventListenerGroup : ZEventListenerGroupBase<UIEventDataBase, UIEventListenerBase>
    {
        public int TargetInstanceID { get; private set; }
        public GameObject Target { get; private set; }

        internal UIEventListenerGroup SetTarget(GameObject target) {
            Target = target;
            TargetInstanceID = target.GetInstanceID();
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Target = null;
        }

        internal override bool AutoRemoveJob(UIEventListenerBase listener, UIEventDataBase eventData)
        {
            return listener.AutoRemoveInClick && eventData.EventType == UIEventType.Click;
        }
    }
}
