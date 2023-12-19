/** Header
 * TimerEventListener.cs
 * 计时器事件的监听
 **/

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ZFramework
{
    /// <summary> 计时器监听的容器 </summary>
    internal abstract class TimerEventListenerBase : ZEventListenerBase<TimerEventDataBase>
    {
        protected void Reset(object callbackTarget, MethodInfo callBackMethodInfo) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = new Timer(this);
        }

        internal override void Recycle()
        {
            base.Recycle();
            Target = null;
            EventDataCaches.Clear();
        }

        internal Timer Target { get; private set; }
        internal long Current { get; set; }//当前的时间(每次update都会变更)
        internal long Remaining { get; set; }//距离下次到期的剩余时间  (内部用的 仅暂停/继续事件时会刷新)
        internal long Expiration { get; set; }//到期时间
        internal float Length { get; set; }//周期
        internal int LoopIndex { get; set; }//从0开始累计
        internal int LoopCount { get; set; }//-1 = 无限
        internal bool IsReadTime { get; set; }
        internal TimerState State { get; set; }
        internal float Progress {
            get => State == TimerState.Paused ? Remaining / (Length * 1000) : (Current >= Expiration ? 1 : (1 - (Expiration - Current) / (Length * 1000)));
        }
        internal List<TimerEventType> EventDataCaches { get; set; } = new List<TimerEventType>();
    }

    internal class TimerEventListener<EventData> : TimerEventListenerBase where EventData : TimerEventDataBase
    {
        internal TimerEventListener<EventData> SetData(Action<EventData> listener, EventData data, float length, int loopCount, bool isRealTime) {
            base.Reset(listener.Target, listener.Method);

            LoopIndex = 0;
            Length = length;
            LoopCount = loopCount < 0 ? -1 : loopCount;

            State = TimerState.Running;
            IsReadTime = isRealTime;

            Listener = listener;
            Data = data;
            return this;
        }

        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }
        public override void Call(TimerEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target, eventData.EventType);
            Listener(Data);
        }
    }
}
