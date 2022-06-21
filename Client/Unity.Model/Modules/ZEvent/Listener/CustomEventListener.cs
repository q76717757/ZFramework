/** Header
 *  CustomEventListener.cs
 *  自定义事件监听
 **/

using System;
using System.Reflection;

namespace ZFramework
{
    public abstract class CustomEventListenerBase : ZEventListenerBase<CustomEventDataBase>
    {
        protected void Reset(string target, object callbackTarget, MethodInfo callBackMethodInfo, bool autoRemoveInCall) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
            AutoRemoveInCall = autoRemoveInCall;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Target = string.Empty;
        }

        internal string Target { get; private set; }
        public bool AutoRemoveInCall { get; private set; }
    }
    public class CustomEventListener<EventData> : CustomEventListenerBase where EventData : CustomEventDataBase
    {
        internal CustomEventListener<EventData> SetData(string target, Action<EventData> listener, bool autoRemoveInCall = false) {
            base.Reset(target, listener.Target, listener.Method, autoRemoveInCall);
            this.listener = listener;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            listener = null;
        }

        private Action<EventData> listener;
        public override void Call(CustomEventDataBase eventData)
        {
            if (eventData is EventData data)
            {
                data.SetStaticData(Target);
                listener(data);
            }
        }

    }
}
