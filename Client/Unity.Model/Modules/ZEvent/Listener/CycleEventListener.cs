/** Header
 *  CycleEventListener.cs
 *  帧循环事件监听
 **/

using System;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    /// <summary> 帧循环监听的容器 </summary>
    public abstract class CycleEventListenerBase : ZEventListenerBase<CycleEventDataBase>
    {
        protected void Reset(CycleType target, object callbackTarget, MethodInfo callBackMethodInfo) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
        }
        internal CycleType Target { get;private set; }

    }

    public class CycleEventListener<EventData> : CycleEventListenerBase where EventData : CycleEventDataBase
    {
        internal CycleEventListener<EventData> SetData(CycleType target, Action<EventData> listener, EventData data = default) {
            base.Reset(target, listener.Target, listener.Method);
            Listener = listener;
            Data = data;
            return this;
        }

        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }
        public override void Call(CycleEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target);
            Listener(Data);
        }
    }
}
