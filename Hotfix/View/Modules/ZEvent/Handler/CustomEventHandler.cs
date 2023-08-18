/** Header
 *  CustomEventHandler.cs
 *  自定义事件处理程序
 **/

using System.Collections.Generic;

namespace ZFramework
{
    public sealed class CustomEventHandler : ZEventHandlerBase
    {
        public Dictionary<string, CustomEventListenerGroup> AllListenerGroups { get; } = new Dictionary<string, CustomEventListenerGroup>();
        private List<CustomEventDataBase> eventDataCache = new List<CustomEventDataBase>();//next Call cache 换了ECS架构生命周期没改 暂时失效了

        internal void AddListener(CustomEventListenerBase newlistener)//channel保证了newlistener正确
        {
            if (!AllListenerGroups.TryGetValue(newlistener.Target, out CustomEventListenerGroup group))
            {
                group = ZEvent.GetNewGroup<CustomEventListenerGroup>().SetTarget(newlistener.Target);
                AllListenerGroups.Add(newlistener.Target, group);
            }
            group.AddListenerToGroup(newlistener);
        }
        internal void RemoveListener(CustomEventListenerBase targetlistener)
        {
            if (AllListenerGroups.TryGetValue(targetlistener.Target, out CustomEventListenerGroup group))
            {
                group.RemoveListenerFromGroup(targetlistener);
            }
        }
        internal void ClearListener(string target)
        {
            if (AllListenerGroups.TryGetValue(target, out CustomEventListenerGroup group))
            {
                group.Recycle();
                AllListenerGroups.Remove(target);
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

        internal void CallGroup(CustomEventDataBase eventData) {
            if (AllListenerGroups.TryGetValue(eventData.Target, out CustomEventListenerGroup value))
            {
                value.DispatchAll(eventData);
            }
        }
        internal void CallGroupNext(CustomEventDataBase eventData) {
            eventDataCache.Add(eventData);
        }
        void Update()
        {
            if (eventDataCache.Count > 0)
            {
                for (int i = 0; i < eventDataCache.Count; i++)
                {
                    CallGroup(eventDataCache[i]);
                }
                eventDataCache.Clear();//回收eventdata
            }
        }
    }

}
