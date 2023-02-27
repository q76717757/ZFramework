/** Header
 *  CustomEventChannel.cs
 *  自定义事件交互的入口
 **/

using System;

namespace ZFramework
{
    public sealed class CustomEventChannel : ZEventChannelBase<CustomEventHandler>
    {
        internal CustomEventChannel(CustomEventHandler handler) : base(handler) { }

        #region 注册
        public void AddListener(string target, Action<CustomEventData> listener, bool autoRemoveInCall = false)
        {
            if (string.IsNullOrEmpty(target) || listener == null) return;
            var newListener = ZEvent.GetNewListener<CustomEventListener<CustomEventData>>().SetData(target, listener, autoRemoveInCall);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(string target, Action<CustomEventData<D0>> listener, bool autoRemoveInCall = false)
        {
            if (string.IsNullOrEmpty(target) || listener == null) return;
            var newListener = ZEvent.GetNewListener<CustomEventListener<CustomEventData<D0>>>().SetData(target, listener, autoRemoveInCall);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1>(string target, Action<CustomEventData<D0, D1>> listener, bool autoRemoveInCall = false)
        {
            if (string.IsNullOrEmpty(target) || listener == null) return;
            var newListener = ZEvent.GetNewListener<CustomEventListener<CustomEventData<D0, D1>>>().SetData(target, listener, autoRemoveInCall);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0, D1, D2>(string target, Action<CustomEventData<D0, D1, D2>> listener, bool autoRemoveInCall = false)
        {
            if (string.IsNullOrEmpty(target) || listener == null) return;
            var newListener = ZEvent.GetNewListener<CustomEventListener<CustomEventData<D0, D1, D2>>>().SetData(target, listener, autoRemoveInCall);
            _handler.AddListener(newListener);
        }
        #endregion

        #region 派发
        //这些是立即派发的
        public void Call(string target)
        {
            if (string.IsNullOrEmpty(target)) return;
            var newData = ZEvent.GetNewData<CustomEventData>().SetData(target);
            _handler.CallGroup(newData);
        }
        public void Call<D0>(string target, D0 data0)
        {
            if (string.IsNullOrEmpty(target)) return;
            var newData = ZEvent.GetNewData<CustomEventData<D0>>().SetData(target, data0);
            _handler.CallGroup(newData);
        }
        public void Call<D0, D1>(string target, D0 data0, D1 data1)
        {
            if (string.IsNullOrEmpty(target)) return;
            var newData = ZEvent.GetNewData<CustomEventData<D0, D1>>().SetData(target, data0, data1);
            _handler.CallGroup(newData);
        }
        public void Call<D0, D1, D2>(string target, D0 data0, D1 data1, D2 data2)
        {
            if (string.IsNullOrEmpty(target)) return;
            var newData = ZEvent.GetNewData<CustomEventData<D0, D1, D2>>().SetData(target, data0, data1, data2);
            _handler.CallGroup(newData);
        }

        //这个是在下一帧Update的时候派发  由主线程来调用 一般子线程有需要Call的时候用这个
        public void CallNext(string target)
        {
            if (string.IsNullOrEmpty(target)) return;
            var newData = ZEvent.GetNewData<CustomEventData>().SetData(target);
            _handler.CallGroupNext(newData);
        }
        public void CallNext<D0>(string target, D0 data0)
        {
            if (string.IsNullOrEmpty(target)) return;
            var newData = ZEvent.GetNewData<CustomEventData<D0>>().SetData(target, data0);
            _handler.CallGroupNext(newData);
        }
        public void CallNext<D0, D1>(string target, D0 data0, D1 data1)
        {
            if (string.IsNullOrEmpty(target)) return;
            var newData = ZEvent.GetNewData<CustomEventData<D0, D1>>().SetData(target, data0, data1);
            _handler.CallGroupNext(newData);
        }
        public void CallNext<D0, D1, D2>(string target, D0 data0, D1 data1, D2 data2)
        {
            if (string.IsNullOrEmpty(target)) return;
            var newData = ZEvent.GetNewData<CustomEventData<D0, D1, D2>>().SetData(target, data0, data1, data2);
            _handler.CallGroupNext(newData);
        }
        #endregion

        #region 注销  
        //这些需要Next?  看情况
        public void RemoveListener(string target, Action<CustomEventData> listener)
        {
            if (string.IsNullOrEmpty(target) || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CustomEventListener<CustomEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(string target, Action<CustomEventData<D0>> listener)
        {
            if (string.IsNullOrEmpty(target) || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CustomEventListener<CustomEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(string target, Action<CustomEventData<D0, D1>> listener)
        {
            if (string.IsNullOrEmpty(target) || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CustomEventListener<CustomEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(string target, Action<CustomEventData<D0, D1, D2>> listener)
        {
            if (string.IsNullOrEmpty(target) || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<CustomEventListener<CustomEventData<D0, D1, D2>>>().SetData(target, listener));
        }
        public void ClearListener(string target)
        {
            if (string.IsNullOrEmpty(target)) return;
            _handler.ClearListener(target);
        }
        public void ClearAllListener()
        {
            _handler.ClearAllListener();
        }
        #endregion
    }
}
