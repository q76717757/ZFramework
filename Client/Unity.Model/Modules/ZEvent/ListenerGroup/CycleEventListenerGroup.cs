/** Header
 *  CycleEventListenerGroup.cs
 *  帧循环事件的监听组
 **/

namespace ZFramework
{
    public class CycleEventListenerGroup : ZEventListenerGroupBase<CycleEventDataBase, CycleEventListenerBase>
    {
        public CycleType Target { get; private set; }

        internal CycleEventListenerGroup SetTarget(CycleType target) { 
            Target = target;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
        }

        internal override bool AutoRemoveJob(CycleEventListenerBase listener, CycleEventDataBase eventData)
        {
            //循环次数自动移除????
            return false;
        }
    }
}
