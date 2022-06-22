/** Header
 * TimerEventChannel.cs
 * 计时器事件交互的入口
 **/

using System;

namespace ZFramework
{
    public sealed class TimerEventChannel:ZEventChannelBase<TimerEventHandler>
    {
        internal TimerEventChannel(TimerEventHandler handler) : base(handler) { }

        #region 注册
        //count -1 => 无限重复    0 => 新建一个监听但是并不调用(使用场景未知)  其他 => 具体重复的次数
        //最小的周期长度为到下一帧的update为止(不管设置多小的周期) 即设置成0时长的话 就相当于update的帧循环事件
        //***有一种情况  一帧内满足多个周期(周期时间非常短)  要不要多次调用?  看情况在作调整?

        public Timer AddListener(float duration, Action<TimerEventData> listener, int loopCount = -1, bool isRealTime = true)
        {
            if (listener == null || duration <= 0) return null;
            var newData = ZEvent.GetNewData<TimerEventData>().SetData();
            var newListener = ZEvent.GetNewListener<TimerEventListener<TimerEventData>>().SetData(listener, newData, duration, loopCount, isRealTime);
            _handler.AddListener(newListener);
            return newListener.Target;
        }
        public Timer AddListener<D0>(float duration, Action<TimerEventData<D0>> listener, D0 data0 = default, int loopCount = -1, bool isRealTime = true)
        {
            if (listener == null || duration <= 0) return null;
            var newData = ZEvent.GetNewData<TimerEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<TimerEventListener<TimerEventData<D0>>>().SetData(listener, newData, duration, loopCount, isRealTime);
            _handler.AddListener(newListener);
            return newListener.Target;
        }
        public Timer AddListener<D0, D1>(float duration, Action<TimerEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, int loopCount = -1, bool isRealTime = true)
        {
            if (listener == null || duration <= 0) return null;
            var newData = ZEvent.GetNewData<TimerEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<TimerEventListener<TimerEventData<D0, D1>>>().SetData(listener, newData, duration, loopCount, isRealTime);
            _handler.AddListener(newListener);
            return newListener.Target;
        }
        public Timer AddListener<D0, D1, D2>(float duration, Action<TimerEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, int loopCount = -1, bool isRealTime = true)
        {
            if (listener == null || duration <= 0) return null;
            var newData = ZEvent.GetNewData<TimerEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = ZEvent.GetNewListener<TimerEventListener<TimerEventData<D0, D1, D2>>>().SetData(listener, newData, duration, loopCount, isRealTime);
            _handler.AddListener(newListener);
            return newListener.Target;
        }
        #endregion

        #region 注销
        /// <summary>
        /// remove和timer.kill的区别是remove不会触发kill事件
        /// </summary>
        public void RemoveListener(Timer target)
        {
            if (target == null) return;
            _handler.RemoveListener(target);
        }
        public void ClearAllListener()
        {
            _handler.ClearAllListener();
        }
        #endregion

        internal void CallEvent(TimerEventListenerBase listener, TimerEventType eventType) => _handler.CallEvent(listener, eventType);
    }

}
