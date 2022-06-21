/** Header
 *  CollisionEventListener.cs
 *  碰撞器事件监听
 **/

using System;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ZFramework
{
    /// <summary> 碰撞器监听的容器 </summary>
    public abstract class CollisionEventListenerBase : ZEventListenerBase<CollisionEventDataBase>
    {
        protected void Reset(GameObject target, object callbackTarget, MethodInfo callBackMethodInfo, bool autoRemoveInEnter) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
            TargetInstanceID = target.GetInstanceID();
            AutoRemoveInEnter = autoRemoveInEnter;
        }

        internal int TargetInstanceID { get; private set; }
        internal GameObject Target { get; private set; }
        public bool AutoRemoveInEnter { get; private set; }
    }
    public class CollisionEventListener<EventData> : CollisionEventListenerBase where EventData : CollisionEventDataBase
    {
        internal CollisionEventListener<EventData> SetData(GameObject target, Action<EventData> listener, EventData data = default, bool autoRemoveInEnter = false) {
            base.Reset(target, listener.Target, listener.Method, autoRemoveInEnter);
            Listener = listener;
            Data = data;
            return this;
        }

        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }
        public override void Call(CollisionEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target, eventData.EventType, eventData.Collision);
            Listener(Data);
        }

    }
}
