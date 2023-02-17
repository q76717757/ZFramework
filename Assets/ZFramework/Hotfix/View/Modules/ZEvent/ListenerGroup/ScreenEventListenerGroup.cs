/** Header
 *  ScreenEventListenerGroup.cs
 *  屏幕点击事件的监听组
 **/

namespace ZFramework
{
    public class ScreenEventListenerGroup : ZEventListenerGroupBase<ScreenEventDataBase, ScreenEventListenerBase>
    {
        public PointerType Target { get; private set; }

        internal ScreenEventListenerGroup SetTarget(PointerType target) {
            Target = target;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
        }

        internal override bool AutoRemoveJob(ScreenEventListenerBase listener, ScreenEventDataBase eventData)
        {
            return false;
        }
    }

}
