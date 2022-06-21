/** Header
 *  CollisionEventChannel.cs
 *  碰撞器事件交互的入口
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public sealed class CollisionEventChannel : ZEventChannelBase<CollisionEventHandler>
    {
        internal CollisionEventChannel(CollisionEventHandler handler) : base(handler) { }

        #region 注册

        public void AddListener(GameObject target, Action<CollisionEventData> listener, bool autoRemoveInEnter = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<CollisionEventData>().SetData();
            var newListener = ZEvent.GetNewListener<CollisionEventListener<CollisionEventData>>().SetData(target, listener, newData, autoRemoveInEnter);
            _handler.AddListener(newListener);
        }
        public void AddListener(UnityEngine.Component target, Action<CollisionEventData> listener, bool autoRemoveInEnter = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, autoRemoveInEnter);
        }
        public void AddListener<D0>(GameObject target, Action<CollisionEventData<D0>> listener, D0 data0 = default, bool autoRemoveInEnter = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<CollisionEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<CollisionEventListener<CollisionEventData<D0>>>().SetData(target, listener, newData, autoRemoveInEnter);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(UnityEngine.Component target, Action<CollisionEventData<D0>> listener, D0 data0 = default, bool autoRemoveInEnter = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, autoRemoveInEnter);
        }
        public void AddListener<D0, D1>(GameObject target, Action<CollisionEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInEnter = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<CollisionEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<CollisionEventListener<CollisionEventData<D0, D1>>>().SetData(target, listener, newData, autoRemoveInEnter);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1>(UnityEngine.Component target, Action<CollisionEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInEnter = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, data1, autoRemoveInEnter);
        }
        public void AddListener<D0, D1, D2>(GameObject target, Action<CollisionEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInEnter = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<CollisionEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = ZEvent.GetNewListener<CollisionEventListener<CollisionEventData<D0, D1, D2>>>().SetData(target, listener, newData, autoRemoveInEnter);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1, D2>(UnityEngine.Component target, Action<CollisionEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInEnter = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, data1, data2, autoRemoveInEnter);
        }

        #endregion

        #region 注销

        public void RemoveListener(GameObject target, Action<CollisionEventData> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CollisionEventListener<CollisionEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(GameObject target, Action<CollisionEventData<D0>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CollisionEventListener<CollisionEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(GameObject target, Action<CollisionEventData<D0, D1>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CollisionEventListener<CollisionEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(GameObject target, Action<CollisionEventData<D0, D1, D2>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CollisionEventListener<CollisionEventData<D0, D1, D2>>>().SetData(target, listener));
        }
        public void ClearListener(GameObject target)
        {
            if (target == null) return;
            _handler.ClearListener(target);
        }
        public void ClearAllListener()
        {
            _handler.ClearAllListener();
        }

        #endregion
    }

}
