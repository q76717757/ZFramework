/** Header
 *  UIEventListener.cs
 *  UI事件监听
 **/

using System;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    /// <summary> UI监听的容器 </summary>
    public abstract class UIEventListenerBase : ZEventListenerBase<UIEventDataBase>
    {
        protected void Reset(GameObject target, object callbackTarget, MethodInfo callBackMethodInfo, bool autoRemoveInClick) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
            TargetInstanceID = target.GetInstanceID();
            AutoRemoveInClick = autoRemoveInClick;
        }

        internal int TargetInstanceID { get; private set; }
        internal GameObject Target { get; private set; }
        internal bool AutoRemoveInClick { get; private set; }

    }

    public class UIEventListener<EventData> : UIEventListenerBase where EventData : UIEventDataBase
    {
        internal UIEventListener<EventData> SetData(GameObject target, Action<EventData> listener, EventData data = default, bool autoRemoveInClick = false) {
            base.Reset(target, listener.Target, listener.Method, autoRemoveInClick);
            Listener = listener;
            Data = data;
            return this;
        }

        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }

        public override void Call(UIEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target, eventData.EventType, eventData.UnityEventData);
            Listener(Data);
        }
    }
}
