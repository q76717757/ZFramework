/** Header
 *  UnitEventListener.cs
 *  Unit事件驱动单元
 **/

using System;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    /// <summary> Unit监听的容器 </summary>
    public abstract class UnitEventListenerBase : ZEventListenerBase<UnitEventDataBase>
    {
        protected void Reset(GameObject target, object callbackTarget, MethodInfo callBackMethodInfo, bool autoRemoveInClick) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
            AutoRemoveInClick = autoRemoveInClick;
            TargetInstanceID = target.GetInstanceID();
        }
        internal int TargetInstanceID { get; private set; }
        internal GameObject Target { get; private set; }
        internal bool AutoRemoveInClick { get; private set; }

    }

    public class UnitEventListener<EventData> : UnitEventListenerBase where EventData : UnitEventDataBase
    {
        internal UnitEventListener<EventData> SetData(GameObject target, Action<EventData> listener, EventData data = default, bool autoRemoveInClick = false) {
            base.Reset(target, listener.Target, listener.Method, autoRemoveInClick);
            Listener = listener;
            Data = data;
            return this;
        }

        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }

        public override void Call(UnitEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target, eventData.EventType, eventData.UnityEventData);
            Listener(Data);
        }
    }
}
