/** Header
 *  UIEventHandler.cs
 *  UI事件处理程序
 **/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ZFramework
{
    public sealed class UIEventHandler : ZEventHandlerBase
    {
        private UIEventDataBase TransferContainer = new UIEventDataBase();//传递容器 (盛菜盘子 唯一且复用,临时减少GC用,引用会暴露 以后改成结构体回调?)
        public Dictionary<int, UIEventListenerGroup> AllListenerGroups { get; } = new Dictionary<int, UIEventListenerGroup>();//key = gameobject.instanceID

        internal void AddListenerForUIFramework(UIEventListenerBase newlistener)
        {
            if (!AllListenerGroups.TryGetValue(newlistener.TargetInstanceID, out UIEventListenerGroup group))
            {
                group = ZEvent.GetNewGroup<UIEventListenerGroup>().SetTarget(newlistener.Target);
                AllListenerGroups.Add(newlistener.TargetInstanceID, group);
            }
            group.AddListenerToGroup(newlistener);
        }
        internal void AddListener(UIEventListenerBase newlistener)
        {
            if (!AllListenerGroups.TryGetValue(newlistener.TargetInstanceID,out UIEventListenerGroup group))
            {
                group = ZEvent.GetNewGroup<UIEventListenerGroup>().SetTarget(newlistener.Target);
                AllListenerGroups.Add(newlistener.TargetInstanceID, group);
                var driver = newlistener.Target.GetComponent<UIEventDriver>();
                if (driver == null)
                    driver = newlistener.Target.AddComponent<UIEventDriver>();
                driver.Init(this);
            }
            group.AddListenerToGroup(newlistener);
        }

        internal void RemoveListener(UIEventListenerBase targetlistener)
        {
            if (AllListenerGroups.TryGetValue(targetlistener.TargetInstanceID,out UIEventListenerGroup group))
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

        internal void CallGroup(int instanceID ,GameObject target, UIEventType eventType, PointerEventData unityEventData) {
            if (AllListenerGroups.TryGetValue(instanceID, out UIEventListenerGroup value))
            {
                TransferContainer.SetStaticData(target, eventType, unityEventData);
                value.DispatchAll(TransferContainer);
            }
        }

    }
}
