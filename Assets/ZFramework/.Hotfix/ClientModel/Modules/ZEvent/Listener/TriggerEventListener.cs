/** Header
 *  TriggerEventListener.cs
 *  触发器事件监听
 **/

using System;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    /// <summary> 触发器监听的容器 </summary>
    public abstract class TriggerEventListenerBase : ZEventListenerBase<TriggerEventDataBase>
    {
        protected void Reset(GameObject target, object callbackTarget, MethodInfo callBackMethodInfo, bool autoRemoveInEnter) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
            AutoRemoveInEnter = autoRemoveInEnter;
            TargetInstanceID = target.GetInstanceID();
        }
        internal int TargetInstanceID { get; private set; }
        internal GameObject Target { get; private set; }
        internal bool AutoRemoveInEnter { get; private set; }

    }

    /// <summary> 触发器监听的实体 </summary>
    public class TriggerEventListener<EventData> : TriggerEventListenerBase where EventData : TriggerEventDataBase
    {
        internal TriggerEventListener<EventData> SetData(GameObject target, Action<EventData> listener, EventData data = default, bool autoRemoveInEnter = false) {
            base.Reset(target, listener.Target, listener.Method, autoRemoveInEnter);
            Listener = listener;
            Data = data;
            return this;
        }


        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }

        public override void Call(TriggerEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target, eventData.EventType, eventData.Other);
            Listener(Data);
        }
    }
}
