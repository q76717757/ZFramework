/** Header
 *  ScreenEventListener.cs
 *  屏幕事件的监听   这个可以做一些跟UI和Unit都没关 但是和鼠标有关的东西  比如屏幕层面上的特效 鼠标轨迹 比如鼠标推动镜头等功能
 **/

using System;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    /// <summary> 屏幕监听的容器 </summary>
    public abstract class ScreenEventListenerBase : ZEventListenerBase<ScreenEventDataBase>
    {
        protected void Reset(PointerType target, object callbackTarget, MethodInfo callBackMethodInfo) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
        }
        internal PointerType Target { get;private set; }

    }

    public class ScreenEventListener<EventData> : ScreenEventListenerBase where EventData : ScreenEventDataBase
    {
        internal ScreenEventListener<EventData> SetData(PointerType target, Action<EventData> listener, EventData data = default) {
            base.Reset(target, listener.Target, listener.Method);
            Listener = listener;
            Data = data;
            return this;
        }

        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }
        public override void Call(ScreenEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target, eventData.EventType);
            Listener(Data);
        }
    }
}
