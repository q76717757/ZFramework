/** Header
 *  CollisionEventHandler.cs
 *  碰撞器事件处理程序
 **/

using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZFramework
{
    public sealed class CollisionEventHandler : ZEventHandlerBase
    {
        private CollisionEventDataBase TransferContainer = new CollisionEventDataBase();
        public Dictionary<int, CollisionEventListenerGroup> AllListenerGroups { get; } = new Dictionary<int, CollisionEventListenerGroup>();

        internal void AddListener(CollisionEventListenerBase newlistener)
        {
            if (!AllListenerGroups.TryGetValue(newlistener.TargetInstanceID, out CollisionEventListenerGroup group))
            {
                group = ZEvent.GetNewGroup<CollisionEventListenerGroup>().SetTarget(newlistener.Target);
                AllListenerGroups.Add(newlistener.TargetInstanceID, group);

                var driver = newlistener.Target.GetComponent<CollisionEventDriver>();
                if (driver == null)
                    driver = newlistener.Target.AddComponent<CollisionEventDriver>();
                driver.Init(this);
            }
            group.AddListenerToGroup(newlistener);
        }

        internal void RemoveListener(CollisionEventListenerBase targetlistener)
        {
            if (AllListenerGroups.TryGetValue(targetlistener.TargetInstanceID, out CollisionEventListenerGroup group))
            {
                group.RemoveListenerFromGroup(targetlistener);
            }
        }

        internal void ClearListener(int instanceID) {
            if (AllListenerGroups.TryGetValue(instanceID, out CollisionEventListenerGroup group))
            {
                group.Recycle();
                AllListenerGroups.Remove(instanceID);
            }
        }
        internal void ClearListener(GameObject target)
        {
            ClearListener(target.GetInstanceID());
        }
        internal void ClearAllListener()
        {
            foreach (var item in AllListenerGroups)
            {
                item.Value.Recycle();
            }
            AllListenerGroups.Clear();
        }

        internal void CallGroup(int instanceID, GameObject target, CollisionEventType eventType, Collision collision) {
            if (AllListenerGroups.TryGetValue(instanceID, out CollisionEventListenerGroup value))
            {
                TransferContainer.SetStaticData(target, eventType, collision);
                value.DispatchAll(TransferContainer);
            }
        }

    }
}
