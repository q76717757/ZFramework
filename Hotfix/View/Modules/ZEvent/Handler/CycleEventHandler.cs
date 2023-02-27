using System.Collections.Generic;

namespace ZFramework
{
    public sealed class CycleEventHandler : ZEventHandlerBase
    {
        private CycleEventDataBase TransferContainer = new CycleEventDataBase();
        public Dictionary<CycleType, CycleEventListenerGroup> AllListenerGroups { get; } = new Dictionary<CycleType, CycleEventListenerGroup>()
        {
            [CycleType.Update] = ZEvent.GetNewGroup<CycleEventListenerGroup>().SetTarget(CycleType.Update),
            [CycleType.FixedUpdate] = ZEvent.GetNewGroup<CycleEventListenerGroup>().SetTarget(CycleType.FixedUpdate),
            [CycleType.LateUpdate] = ZEvent.GetNewGroup<CycleEventListenerGroup>().SetTarget(CycleType.LateUpdate),
        };

        public CycleEventHandler()
        {
            ZEventTemp.Instance.callback += Update;//临时用一下
        }

        internal void AddListener(CycleEventListenerBase newlistener)
        {
            AllListenerGroups[newlistener.Target].AddListenerToGroup(newlistener);
        }
        internal void RemoveListener(CycleEventListenerBase targetlistener)
        {
            AllListenerGroups[targetlistener.Target].RemoveListenerFromGroup(targetlistener);
        }
        internal void ClearListener(CycleType target)
        {
            AllListenerGroups[target].Clear();
        }
        internal void ClearAllListener()
        {
            AllListenerGroups[CycleType.Update].Clear();
            AllListenerGroups[CycleType.FixedUpdate].Clear();
            AllListenerGroups[CycleType.LateUpdate].Clear();
        }

        private void HandleCall(CycleType type)
        {
            TransferContainer.SetStaticData(type);
            AllListenerGroups[type].DispatchAll(TransferContainer);
        }

        void Update()
        {
            HandleCall(CycleType.Update);
        }

        void FixedUpdate()
        {
            HandleCall(CycleType.FixedUpdate);
        }

        void LateUpdate()
        {
            HandleCall(CycleType.LateUpdate);
        }
    }
}
