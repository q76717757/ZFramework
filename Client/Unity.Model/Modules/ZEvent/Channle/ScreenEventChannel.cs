/** Header
 *  ScreenEventChannel.cs
 *  屏幕事件交互的入口
 **/

using System;

namespace ZFramework
{
    public sealed class ScreenEventChannel:ZEventChannelBase<ScreenEventHandler>
    {
        internal ScreenEventChannel(ScreenEventHandler handler) : base(handler) { }

        #region 注册
        public void AddListener(PointerType target, Action<ScreenEventData> listener)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<ScreenEventData>().SetData();
            var newListener = ZEvent.GetNewListener<ScreenEventListener<ScreenEventData>>().SetData(target, listener, newData);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(PointerType target, Action<ScreenEventData<D0>> listener, D0 data0 = default)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<ScreenEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<ScreenEventListener<ScreenEventData<D0>>>().SetData(target, listener, newData);
            _handler.AddListener(newListener);
        }

        public void AddListener<D0, D1>(PointerType target, Action<ScreenEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<ScreenEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<ScreenEventListener<ScreenEventData<D0, D1>>>().SetData(target, listener, newData);
            _handler.AddListener(newListener);
        }

        public void AddListener<D0, D1, D2>(PointerType target, Action<ScreenEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default)
        {
            if (listener == null) return;
            var newData = ZEvent.GetNewData<ScreenEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = ZEvent.GetNewListener<ScreenEventListener<ScreenEventData<D0, D1, D2>>>().SetData(target, listener, newData);
            _handler.AddListener(newListener);
        }
        #endregion

        #region 注销
        public void RemoveListener(PointerType target, Action<ScreenEventData> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<ScreenEventListener<ScreenEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(PointerType target, Action<ScreenEventData<D0>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<ScreenEventListener<ScreenEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(PointerType target, Action<ScreenEventData<D0, D1>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<ScreenEventListener<ScreenEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(PointerType target, Action<ScreenEventData<D0, D1, D2>> listener)
        {
            if (listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<ScreenEventListener<ScreenEventData<D0, D1, D2>>>().SetData(target, listener));
        }
        public void ClearListener(PointerType target)
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
