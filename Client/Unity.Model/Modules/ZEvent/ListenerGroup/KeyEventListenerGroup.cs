/** Header
 *  KeyEventListenerGroup.cs
 *  按键事件的监听组
 **/

using UnityEngine;

namespace ZFramework
{
    public class KeyEventListenerGroup : ZEventListenerGroupBase<KeyEventDataBase, KeyEventListenerBase>
    {
        public KeyCode Target { get; private set; }
        internal bool LastKeyIsDown { get; set; } = false;//记录这个按键上一帧是不是被按着的
        internal KeyEventListenerGroup SetTarget(KeyCode target)  { 
            Target = target;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Target = KeyCode.None;
        }
        internal override bool AutoRemoveJob(KeyEventListenerBase listener, KeyEventDataBase eventData)
        {
            return false;
        }
    }
}
