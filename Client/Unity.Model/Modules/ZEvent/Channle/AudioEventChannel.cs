/** Header
 *  AudioEventChannel.cs
 *  音频事件交互的入口
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZFramework
{
    public sealed class AudioEventChannel : ZEventChannelBase<AudioEventHandler>
    {
        internal AudioEventChannel(AudioEventHandler handler) : base(handler) { }

        #region 注册
        public void AddListener(ZAudioPlayer target, Action<AudioEventData> listener, bool autoRemoveInPlayEnd = false)
        {
            if (target  == null|| listener == null) return;
            var newData = ZEvent.GetNewData<AudioEventData>().SetData();
            var newListener = ZEvent.GetNewListener<AudioEventListener<AudioEventData>>().SetData(target, listener, newData, autoRemoveInPlayEnd);
            _handler.AddListener(newListener);
        }
        public void AddListener<D0>(ZAudioPlayer target, Action<AudioEventData<D0>> listener, D0 data0 = default, bool autoRemoveInPlayEnd = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<AudioEventData<D0>>().SetData(data0);
            var newListener = ZEvent.GetNewListener<AudioEventListener<AudioEventData<D0>>>().SetData(target, listener, newData, autoRemoveInPlayEnd);
            _handler.AddListener(newListener);
        }

        public void AddListener<D0, D1>(ZAudioPlayer target, Action<AudioEventData<D0, D1>> listener, D0 data0 = default, D1 data1 = default, bool autoRemoveInPlayEnd = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<AudioEventData<D0, D1>>().SetData(data0, data1);
            var newListener = ZEvent.GetNewListener<AudioEventListener<AudioEventData<D0, D1>>>().SetData(target, listener, newData, autoRemoveInPlayEnd);
            _handler.AddListener(newListener);
        }

        public void AddListener<D0, D1, D2>(ZAudioPlayer target, Action<AudioEventData<D0, D1, D2>> listener, D0 data0 = default, D1 data1 = default, D2 data2 = default, bool autoRemoveInPlayEnd = false)
        {
            if (target == null || listener == null) return;
            var newData = ZEvent.GetNewData<AudioEventData<D0, D1, D2>>().SetData(data0, data1, data2);
            var newListener = new AudioEventListener<AudioEventData<D0, D1, D2>>().SetData(target, listener, newData, autoRemoveInPlayEnd);
            _handler.AddListener(newListener);
        }
        #endregion

        #region 注销

        public void RemoveListener(ZAudioPlayer target, Action<AudioEventData> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<AudioEventListener<AudioEventData>>().SetData(target, listener));
        }
        public void RemoveListener<D0>(ZAudioPlayer target, Action<AudioEventData<D0>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<AudioEventListener<AudioEventData<D0>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1>(ZAudioPlayer target, Action<AudioEventData<D0, D1>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<AudioEventListener<AudioEventData<D0, D1>>>().SetData(target, listener));
        }
        public void RemoveListener<D0, D1, D2>(ZAudioPlayer target, Action<AudioEventData<D0, D1, D2>> listener)
        {
            if (target == null || listener == null) return;
            _handler.RemoveListener(ZEvent.GetNewListener<AudioEventListener<AudioEventData<D0, D1, D2>>>().SetData(target, listener));
        }
        public void ClearListener(ZAudioPlayer target)
        {
            if (target == null) return;
            _handler.ClearListener(target);
        }
        public void ClearAllListener()
        {
            _handler.ClearAllListener();
        }

        #endregion

    }
}

