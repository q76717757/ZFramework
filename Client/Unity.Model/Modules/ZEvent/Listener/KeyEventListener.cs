/** Header
 *  KeyEventListener.cs
 *  按键事件监听
 **/

using System;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    /// <summary> 按键监听的容器 </summary>
    public abstract class KeyEventListenerBase : ZEventListenerBase<KeyEventDataBase>
    {
        protected void Reset(KeyCode target, object callbackTarget, MethodInfo callBackMethodInfo) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
        }
        internal KeyCode Target { get; private set; }
    }

    public class KeyEventListener<EventData> : KeyEventListenerBase where EventData : KeyEventDataBase
    {
        internal KeyEventListener<EventData> SetData(KeyCode target, Action<EventData> listener, EventData data = default, bool autoRemove = false) {
            base.Reset(target, listener.Target, listener.Method);
            Listener = listener;
            Data = data;
            return this;
        }

        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }
        public override void Call(KeyEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target, eventData.EventType);
            Listener(Data);
        }

    }
}
