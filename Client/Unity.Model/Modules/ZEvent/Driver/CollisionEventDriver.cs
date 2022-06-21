/** Header
 *  CollisionEventDriver.cs
 *  碰撞器事件驱动单元
 **/

using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]//从inspector面板隐藏掉
    public sealed class CollisionEventDriver : ZEventDriverBase<CollisionEventHandler>
    {

        private void OnCollisionEnter(Collision collision)
            => SendData(CollisionEventType.Enter, collision);
        private void OnCollisionStay(Collision collision)
            => SendData(CollisionEventType.Stay, collision);
        private void OnCollisionExit(Collision collision)
            => SendData(CollisionEventType.Exit, collision);
        private void OnDestroy()
            => Handler?.ClearListener(InstanceID);

        private void SendData(CollisionEventType eventType, Collision collision)
            => Handler?.CallGroup(InstanceID, gameObject, eventType, collision);

    }
}
