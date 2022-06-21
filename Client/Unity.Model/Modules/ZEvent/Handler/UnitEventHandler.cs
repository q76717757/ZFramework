/** Header
 * UnitEventHandler.cs
 * 单位事件处理程序
 **/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZFramework
{
    public sealed class UnitEventHandler : ZEventHandlerBase
    {
        private UnitEventDataBase TransferContainer = new UnitEventDataBase();//传递容器
        public Dictionary<int, UnitEventListenerGroup> AllListenerGroups { get; } = new Dictionary<int, UnitEventListenerGroup>();

        internal void AddListener(UnitEventListenerBase newlistener)
        {
            if (!AllListenerGroups.TryGetValue(newlistener.TargetInstanceID, out UnitEventListenerGroup group))
            {
                group = ZEvent.GetNewGroup<UnitEventListenerGroup>().SetTarget(newlistener.Target);
                AllListenerGroups.Add(newlistener.TargetInstanceID, group);
                var driver = newlistener.Target.GetComponent<UnitEventDriver>();
                if (driver == null)
                    driver = newlistener.Target.AddComponent<UnitEventDriver>();
                driver.Init(this);
            }
            group.AddListenerToGroup(newlistener);
        }

        internal void RemoveListener(UnitEventListenerBase targetlistener)
        {
            if (AllListenerGroups.TryGetValue(targetlistener.TargetInstanceID, out UnitEventListenerGroup group))
            {
                group.RemoveListenerFromGroup(targetlistener);
            }
        }

        internal void ClearListener(GameObject target)
        {
            AllListenerGroups.Remove(target.GetInstanceID());
        }
        internal void ClearAllListener()
        {
            AllListenerGroups.Clear();
        }

        internal void CallGroup(int instanceID ,GameObject target, UnitEventType eventType, PointerEventData unityEventData) {
            if (AllListenerGroups.TryGetValue(instanceID, out UnitEventListenerGroup value))
            {
                TransferContainer.SetStaticData(target, eventType, unityEventData);
                value.DispatchAll(TransferContainer);
            }
        }
    }
}
