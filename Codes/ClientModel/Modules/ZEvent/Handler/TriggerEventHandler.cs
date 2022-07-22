/** Header
 *  TriggerEventHandle.cs
 *  触发器事件处理程序
 **/

using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public sealed class TriggerEventHandler : ZEventHandlerBase
    {
        /// <summary> 传递容器 </summary>
        private TriggerEventDataBase TransferContainer = new TriggerEventDataBase();
        public Dictionary<int, TriggerEventListenerGroup> AllListenerGroups { get; } = new Dictionary<int, TriggerEventListenerGroup>();

        internal void AddListener(TriggerEventListenerBase newlistener)
        {
            if (!AllListenerGroups.TryGetValue(newlistener.TargetInstanceID, out TriggerEventListenerGroup group))
            {
                group = ZEvent.GetNewGroup<TriggerEventListenerGroup>().SetTarget(newlistener.Target);
                AllListenerGroups.Add(newlistener.TargetInstanceID, group);
                var driver = newlistener.Target.GetComponent<TriggerEventDriver>();
                if (driver == null)
                    driver = newlistener.Target.AddComponent<TriggerEventDriver>();
                driver.Init(this);
            }
            group.AddListenerToGroup(newlistener);
        }

        internal void RemoveListener(TriggerEventListenerBase targetlistener)
        {
            if (AllListenerGroups.TryGetValue(targetlistener.TargetInstanceID, out TriggerEventListenerGroup group))
            {
                group.RemoveListenerFromGroup(targetlistener);
            }
        }

        internal void ClearListener(GameObject target)
        { 
            if (AllListenerGroups.TryGetValue(target.GetInstanceID(), out TriggerEventListenerGroup group))
            {
                group.Recycle();
                AllListenerGroups.Remove(target.GetInstanceID());
            }
        }
        internal void ClearAllListener()
        {
            foreach (var item in AllListenerGroups)
            {
                item.Value.Recycle();
            }
            AllListenerGroups.Clear();
        }

        internal void CallGroup(int instanceID,GameObject target, TriggerEventType eventType, Collider other) {
            if (AllListenerGroups.TryGetValue(instanceID, out TriggerEventListenerGroup value))
            {
                TransferContainer.SetStaticData(target, eventType, other);
                value.DispatchAll(TransferContainer);
            }
        }
    }

}
