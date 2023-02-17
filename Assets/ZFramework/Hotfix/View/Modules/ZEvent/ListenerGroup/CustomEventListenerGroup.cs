/** Header
 *  CustomEventListenerGroup.cs
 *  自定义事件的监听组
 **/

using System;

namespace ZFramework
{
    public class CustomEventListenerGroup : ZEventListenerGroupBase<CustomEventDataBase, CustomEventListenerBase>
    {
        public string Target { get; private set; }

        internal CustomEventListenerGroup SetTarget(string target) {
            Target = target;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Target = null;
        }

        internal override bool AutoRemoveJob(CustomEventListenerBase listener, CustomEventDataBase eventData)
        {
            return listener.AutoRemoveInCall;
        }
    }
}
