/** Header
 *  CollisionEventListenerGroup.cs
 *  碰撞器事件的监听组
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public class CollisionEventListenerGroup : ZEventListenerGroupBase<CollisionEventDataBase, CollisionEventListenerBase>
    {
        public GameObject Target { get; private set; }

        internal CollisionEventListenerGroup SetTarget(GameObject target)
        {
            Target = target;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Target = null;
        }

        internal override bool AutoRemoveJob(CollisionEventListenerBase listener, CollisionEventDataBase eventData)
        {
            return listener.AutoRemoveInEnter && eventData.EventType == CollisionEventType.Enter;
        }
    }
}
