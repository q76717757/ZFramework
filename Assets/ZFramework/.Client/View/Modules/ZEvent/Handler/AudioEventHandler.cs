/** Header
 *  AudioEventHandler.cs
 *  
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public sealed class AudioEventHandler : ZEventHandlerBase
    {
        public List<AudioEventListenerBase> AllListeners { get; } = new List<AudioEventListenerBase>(32);
        private List<AudioEventListenerBase> WaitngForAdd { get; } = new List<AudioEventListenerBase>();
        public List<AudioEventDataBase> EventDataCaches { get; } = new List<AudioEventDataBase>(16);

        internal void AddListener(AudioEventListenerBase listener) {

        }
        internal void RemoveListener(AudioEventListenerBase targetlistener) {

        }
        internal void ClearListener(ZAudioPlayer target) {

        }

        internal void ClearAllListener() { }

        void Update()
        {


        }
    }

}