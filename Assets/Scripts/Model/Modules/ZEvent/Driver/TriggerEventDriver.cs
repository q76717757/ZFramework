/** Header
 *  TriggerEventDriver.cs
 *  触发器事件驱动单元
 **/

using UnityEngine;

namespace ZFramework
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]//从inspector面板隐藏掉
    public sealed class TriggerEventDriver : ZEventDriverBase<TriggerEventHandler>
    {

        private void OnTriggerEnter(Collider other)
            => SendData(TriggerEventType.Enter, other);
        private void OnTriggerStay(Collider other)
            => SendData(TriggerEventType.Stay, other);
        private void OnTriggerExit(Collider other)
            => SendData(TriggerEventType.Exit, other);

        private void SendData(TriggerEventType eventType, Collider other)
            => Handler?.CallGroup(InstanceID, gameObject, eventType, other);

    }
}
