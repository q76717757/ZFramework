/** Header
 *  UnitEventChannel.cs
 *  单位事件(场景中的物体)交互的入口
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public sealed class UnitEventChannel:ZEventChannelBase<UnitEventHandler>
    {
        internal UnitEventChannel(UnitEventHandler handler) : base(handler) { }

        #region 注册
        public void AddListener(GameObject target, Action<UnitEventData> listener, bool autoRemoveInClick = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<UnitEventData>().SetData();
            var newListener = ZEvent.GetNewListener<UnitEventListener<UnitEventData>>().SetData(target, listener, newData, autoRemoveInClick);
            _handler.AddListener(newListener);
        }
        public void AddListener(UnityEngine.Component target, Action<UnitEventData> listener, bool autoRemoveInClick = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, autoRemoveInClick);
        }

        public void AddListener<D0>(GameObject target, Action<UnitEventData<D0>> listener, D0 data0 = default, bool autoRemoveInClick = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<UnitEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<UnitEventListener<UnitEventData<D0>>>().SetData(target, listener, newData, autoRemoveInClick);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(UnityEngine.Component target, Action<UnitEventData<D0>> listener, D0 data0 = default, bool autoRemoveInClick = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, autoRemoveInClick);
        }

        public void AddListener<D0, D1>(GameObject target, Action<UnitEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInClick = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<UnitEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<UnitEventListener<UnitEventData<D0, D1>>>().SetData(target, listener, newData, autoRemoveInClick);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1>(UnityEngine.Component target, Action<UnitEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInClick = false)
        {
            if (target) return;
            AddListener(target.gameObject, listener, data0, data1, autoRemoveInClick);
        }

        public void AddListener<D0, D1, D2>(GameObject target, Action<UnitEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInClick = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<UnitEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = ZEvent.GetNewListener<UnitEventListener<UnitEventData<D0, D1, D2>>>().SetData(target, listener, newData, autoRemoveInClick);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1, D2>(UnityEngine.Component target, Action<UnitEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInClick = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, data1, data2, autoRemoveInClick);
        }
        #endregion

        #region 注销
        public void RemoveListener(GameObject target, Action<UnitEventData> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener < UnitEventListener<UnitEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(GameObject target, Action<UnitEventData<D0>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener < UnitEventListener<UnitEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(GameObject target, Action<UnitEventData<D0, D1>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener < UnitEventListener<UnitEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(GameObject target, Action<UnitEventData<D0, D1, D2>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener < UnitEventListener<UnitEventData<D0, D1, D2>>>().SetData(target, listener));
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
