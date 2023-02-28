/** Header
 *  KeyEventChannel.cs
 *  按键事件交互的入口
 **/

using System;
using UnityEngine;

namespace ZFramework
{
    public sealed class KeyEventChannel:ZEventChannelBase<KeyEventHandler>
    {
        internal KeyEventChannel(KeyEventHandler handler) : base(handler) { }

        #region 注册
        public void AddListener(KeyCode target, Action<KeyEventData> listener, bool autoRemove = false)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<KeyEventData>().SetData();
            var newListener = ZEvent.GetNewListener<KeyEventListener<KeyEventData>>().SetData(target, listener, newData, autoRemove);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(KeyCode target, Action<KeyEventData<D0>> listener, D0 data0 = default, bool autoRemove = false)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<KeyEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<KeyEventListener<KeyEventData<D0>>>().SetData(target, listener, newData, autoRemove);
            _handler.AddListener(newListener);
        }

        public void AddListener<D0, D1>(KeyCode target, Action<KeyEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemove = false)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<KeyEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<KeyEventListener<KeyEventData<D0, D1>>>().SetData(target, listener, newData, autoRemove);
            _handler.AddListener(newListener);
        }

        public void AddListener<D0, D1, D2>(KeyCode target, Action<KeyEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemove = false)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<KeyEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = ZEvent.GetNewListener<KeyEventListener<KeyEventData<D0, D1, D2>>>().SetData(target, listener, newData, autoRemove);
            _handler.AddListener(newListener);
        }
        #endregion

        #region 注销
        public void RemoveListener(KeyCode target, Action<KeyEventData> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<KeyEventListener<KeyEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(KeyCode target, Action<KeyEventData<D0>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<KeyEventListener<KeyEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(KeyCode target, Action<KeyEventData<D0, D1>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<KeyEventListener<KeyEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(KeyCode target, Action<KeyEventData<D0, D1, D2>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<KeyEventListener<KeyEventData<D0, D1, D2>>>().SetData(target, listener));
        }
        public void ClearListener(KeyCode target)
        {
            _handler.ClearListener(target);
        }
        public void ClearAllListener()
        {
            _handler.ClearAllListener();
        }
        #endregion
    }
}
