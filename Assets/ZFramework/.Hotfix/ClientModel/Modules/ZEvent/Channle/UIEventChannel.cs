/** Header
 *  UIEventChannel.cs
 *  UI事件交互的入口
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public sealed class UIEventChannel:ZEventChannelBase<UIEventHandler>
    {
        internal UIEventChannel(UIEventHandler handler) : base(handler) { }

        #region 注册
        public void AddListener(GameObject target, Action<UIEventData> listener, bool autoRemoveInClick = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<UIEventData>().SetData();
            var newListener = ZEvent.GetNewListener<UIEventListener<UIEventData>>().SetData(target, listener, newData, autoRemoveInClick);
            _handler.AddListener(newListener);
        }

        public void AddListener(UnityEngine.Component target, Action<UIEventData> listener, bool autoRemoveInClick = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, autoRemoveInClick);
        }

        public void AddListener<D0>(GameObject target, Action<UIEventData<D0>> listener, D0 data0 = default, bool autoRemoveInClick = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<UIEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<UIEventListener<UIEventData<D0>>>().SetData(target, listener, newData, autoRemoveInClick);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(UnityEngine.Component target, Action<UIEventData<D0>> listener, D0 data0 = default, bool autoRemoveInClick = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, autoRemoveInClick);
        }

        public void AddListener<D0, D1>(GameObject target, Action<UIEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInClick = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<UIEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<UIEventListener<UIEventData<D0, D1>>>().SetData(target, listener, newData, autoRemoveInClick);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1>(UnityEngine.Component target, Action<UIEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInClick = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, data1, autoRemoveInClick);
        }

        public void AddListener<D0, D1, D2>(GameObject target, Action<UIEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInClick = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<UIEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = ZEvent.GetNewListener<UIEventListener<UIEventData<D0, D1, D2>>>().SetData(target, listener, newData, autoRemoveInClick);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1, D2>(UnityEngine.Component target, Action<UIEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInClick = false)
        {
            if (target == null) return;
            AddListener(target.gameObject, listener, data0, data1, data2, autoRemoveInClick);
        }
        #endregion

        #region 注销
        public void RemoveListener(GameObject target, Action<UIEventData> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<UIEventListener<UIEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(GameObject target, Action<UIEventData<D0>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<UIEventListener<UIEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(GameObject target, Action<UIEventData<D0, D1>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<UIEventListener<UIEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(GameObject target, Action<UIEventData<D0, D1, D2>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<UIEventListener<UIEventData<D0, D1, D2>>>().SetData(target, listener));
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
