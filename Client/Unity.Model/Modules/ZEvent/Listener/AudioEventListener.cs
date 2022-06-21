/** Header
 *  AudioEventListener.cs
 *  
 **/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ZFramework
{
    /// <summary> 音频事件监听的容器 </summary>
    public abstract class AudioEventListenerBase : ZEventListenerBase<AudioEventDataBase>
    {
        internal void Reset(ZAudioPlayer target, object callbackTarget, MethodInfo callBackMethodInfo, bool autoRemoveInPlayEnd) {
            base.SetMethodInfo(callbackTarget, callBackMethodInfo);
            Target = target;
            AutoRemoveInPlayEnd = autoRemoveInPlayEnd;
        }

        internal ZAudioPlayer Target { get; private set; }
        internal bool AutoRemoveInPlayEnd { get; private set; }

    }

    public class AudioEventListener<EventData> : AudioEventListenerBase where EventData : AudioEventDataBase
    {
        internal AudioEventListener<EventData> SetData(ZAudioPlayer target, Action<EventData> listener, EventData data = default, bool autoRemoveInPlayEnd = false) {
            base.Reset(target, listener.Target, listener.Method, autoRemoveInPlayEnd);
            Listener = listener;
            Data = data;
            return this;
        }

        internal EventData Data { get; private set; }
        internal Action<EventData> Listener { get; private set; }
        public override void Call(AudioEventDataBase eventData)
        {
            Data.SetStaticData(eventData.Target, eventData.EventType);
            Listener(Data);
        }
    }

}