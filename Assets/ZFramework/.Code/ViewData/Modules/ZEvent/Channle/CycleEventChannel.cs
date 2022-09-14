/** Header
 *  CycleEventChannel.cs
 *  帧循环事件交互的入口
 **/

using System;

namespace ZFramework
{
    public sealed class CycleEventChannel:ZEventChannelBase<CycleEventHandler>
    {
        internal CycleEventChannel(CycleEventHandler handler) : base(handler) { }

        #region 注册
        public void AddListener(CycleType target, Action<CycleEventData> listener)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<CycleEventData>().SetData();
            var newListener = ZEvent.GetNewListener<CycleEventListener<CycleEventData>>().SetData(target, listener, newData);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(CycleType target, Action<CycleEventData<D0>> listener, D0 data0 = default)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<CycleEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<CycleEventListener<CycleEventData<D0>>>().SetData(target, listener, newData);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1>(CycleType target, Action<CycleEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<CycleEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<CycleEventListener<CycleEventData<D0, D1>>>().SetData(target, listener, newData);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1, D2>(CycleType target, Action<CycleEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<CycleEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = ZEvent.GetNewListener<CycleEventListener<CycleEventData<D0, D1, D2>>>().SetData(target, listener, newData);
            _handler.AddListener(newListener);
        }
        #endregion

        #region 注销
        public void RemoveListener(CycleType target, Action<CycleEventData> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CycleEventListener<CycleEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(CycleType target, Action<CycleEventData<D0>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CycleEventListener<CycleEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(CycleType target, Action<CycleEventData<D0, D1>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CycleEventListener<CycleEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(CycleType target, Action<CycleEventData<D0, D1, D2>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CycleEventListener<CycleEventData<D0, D1, D2>>>().SetData(target, listener));
        }
        public void ClearListener(CycleType target)
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
