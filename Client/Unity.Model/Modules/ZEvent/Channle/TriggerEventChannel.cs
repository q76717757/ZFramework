/** Header
 *  TriggerEventChannel.cs
 *  触发器事件交互的入口
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public sealed class TriggerEventChannel:ZEventChannelBase<TriggerEventHandler>
    {
        internal TriggerEventChannel(TriggerEventHandler handler) : base(handler) { }

        #region 注册
        public void AddListener(GameObject target, Action<TriggerEventData> listener, bool autoRemoveInEnter = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<TriggerEventData>().SetData();
            var newListener = ZEvent.GetNewListener<TriggerEventListener<TriggerEventData>>().SetData(target, listener, newData, autoRemoveInEnter);
            _handler.AddListener(newListener);
        }
        public void AddListener(UnityEngine.Component target, Action<TriggerEventData> listener, bool autoRemoveInEnter = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, autoRemoveInEnter);
        }

        public void AddListener<D0>(GameObject target, Action<TriggerEventData<D0>> listener, D0 data0 = default, bool autoRemoveInEnter = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<TriggerEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<TriggerEventListener<TriggerEventData<D0>>>().SetData(target, listener, newData, autoRemoveInEnter);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(UnityEngine.Component target, Action<TriggerEventData<D0>> listener, D0 data0 = default, bool autoRemoveInEnter = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, autoRemoveInEnter);
        }

        public void AddListener<D0, D1>(GameObject target, Action<TriggerEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInEnter = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<TriggerEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<TriggerEventListener<TriggerEventData<D0, D1>>>().SetData(target, listener, newData, autoRemoveInEnter);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1>(UnityEngine.Component target, Action<TriggerEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInEnter = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, data1, autoRemoveInEnter);
        }

        public void AddListener<D0, D1, D2>(GameObject target, Action<TriggerEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInEnter = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<TriggerEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = ZEvent.GetNewListener<TriggerEventListener<TriggerEventData<D0, D1, D2>>>().SetData(target, listener, newData, autoRemoveInEnter);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1, D2>(UnityEngine.Component target, Action<TriggerEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInEnter = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, data1, data2, autoRemoveInEnter);
        }
        #endregion

        #region 注销
        public void RemoveListener(GameObject target, Action<TriggerEventData> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<TriggerEventListener<TriggerEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(GameObject target, Action<TriggerEventData<D0>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<TriggerEventListener<TriggerEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(GameObject target, Action<TriggerEventData<D0, D1>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<TriggerEventListener<TriggerEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(GameObject target, Action<TriggerEventData<D0, D1, D2>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<TriggerEventListener<TriggerEventData<D0, D1, D2>>>().SetData(target, listener));
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
