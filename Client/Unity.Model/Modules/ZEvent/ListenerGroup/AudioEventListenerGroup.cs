/** Header
 *  AudioEventListenerGroup.cs
 *  
 **/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public class AudioEventListenerGroup : ZEventListenerGroupBase<AudioEventDataBase, AudioEventListenerBase>
    {
        public ZAudioPlayer Target { get; private set; }

        internal AudioEventListenerGroup SetTarget(ZAudioPlayer target) {
            Target = target;
            return this;
        }
        internal override void Recycle()
        {
            base.Recycle();
            Target = null;
        }

        internal override bool AutoRemoveJob(AudioEventListenerBase listener, AudioEventDataBase eventData)
        {
            return listener.AutoRemoveInPlayEnd && eventData.EventType == AudioEventType.End;
        }
    }
}