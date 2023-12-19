/**
 *  ZAudioPlayer.cs
 *  对AudioSource组件的封装  该对象可以被持有 并带了一些实用方法  内部的AudioSource不对外暴露并且被对象池管理
 *  持有这个对象就可以使用事件系统 把这个对象当做target
 *  AudioSource生命周期到play stop(循环不算)为止 播放完成就会被回收 但这个对象可以继续持有,使用Replay可以重播,内部实现是重新再池子里拿一个AudioSource 对壳的引用并不改变
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework {
    /// <summary> 音频播放器 </summary>
    public sealed class ZAudioPlayer
    {
        internal ZAudioPlayer(ZAudioLibraryData data, ChannelType? channelType, float factor, bool loop)
        {
            Data = data;
            Length = data.isFragment ? (data.endTime - data.startTime) : data.clip.length;
            ChannelType = channelType;

            Factor = Mathf.Clamp01(factor);
            IsLoop = loop;

            AudioSource = ZAudioHandler.GetAudioSource();
            AudioSource.clip = data.clip;
            AudioSource.mute = ZAudio.GetMute(channelType);
            AudioSource.volume = ZAudio.GlobalVolume * ZAudio.GetVolume(ChannelType) * factor;
            AudioSource.time = data.isFragment ? data.startTime : 0;
            AudioSource.Play();
        }
        private ZAudioLibraryData Data { get; }//不能公开这个 防止被改
        private AudioSource AudioSource { get; set; }
        private float Factor { get; }//音量修正因子
        /// <summary> 是否存活 </summary>   AudioSource就是他的心脏  没有心脏它就是死的
        internal bool IsALive => AudioSource != null;

        public bool Mute
        {
            get
            {
                if (IsALive)
                {
                    return AudioSource.mute;
                }
                return true;
            }
            set {
                if (IsALive)
                {
                    AudioSource.mute = value;
                }
            }
        }
        /// <summary> 播放的内容 </summary>
        public string Info => Data == null ? string.Empty : Data.info;
        /// <summary> 播放中 (停止/暂停 都是false) </summary>
        public bool IsPlaying => IsALive ? AudioSource.isPlaying : false;
        /// <summary> 所属声道 Null则为内置(共享型) </summary>
        public ChannelType? ChannelType { get; }
        /// <summary> 音频时长(秒) </summary>
        public float Length { get; }
        /// <summary> 是否循环 </summary>
        public bool IsLoop { get; set; }
        /// <summary> 当前播放时间 </summary>
        public float Time => IsALive ? (Data.isFragment ? (AudioSource.time - Data.startTime) : AudioSource.time) : 0f;
        /// <summary> 播放进度 </summary>
        public float Progress => Time / Length;

        #region 公开方法

        /// <summary> 指定时间播放 </summary>  这个time是相对的
        public void Play(float time)
        {
            if (time < Length)
            {
                if (time < 0) time = 0f;
                if (!IsALive) {
                    AudioSource = ZAudioHandler.GetAudioSource();
                    AudioSource.clip = Data.clip;
                    AudioSource.mute = ZAudio.GetMute(ChannelType);
                    AudioSource.volume = ZAudio.GlobalVolume * ZAudio.GetVolume(ChannelType) * Factor;
                    AudioSource.loop = IsLoop;
                }
                AudioSource.time = Data.isFragment ? (Data.startTime + time) : time;
                AudioSource.Play();
            }
            else
            {
                Stop();
            }
        }
        /// <summary> 停止播放 </summary>
        public void Stop()
        {
            if (IsALive)
            {
                var recycle = AudioSource;
                AudioSource.Stop();
                AudioSource = null;
                ZAudioHandler.回收AudioSource(recycle);
            }
        }

        /// <summary> 指定进度播放 (0~1) </summary>
        public void PlayOnProgress(float progress) => Play(Mathf.Clamp(progress, 0f, 1f) * Length);

        /// <summary> 继续播放 (如果本就播放完的 则重播)</summary>
        public void Continue()
        {
            if (IsALive)
                AudioSource.UnPause();
            else
                Replay();
        }
        /// <summary> 暂停播放 </summary>
        public void Paused()
        {
            if (IsALive)
                AudioSource.Pause();
        }
        /// <summary> 重播 </summary>
        public void Replay() => Play(0f);
        #endregion

        internal void Update()
        {
            if (!IsALive) return;


            var time = AudioSource.time;
            if (AudioSource.isPlaying)
            {
                if (Data.isFragment && time >= Data.endTime)
                {
                    if (IsLoop)
                        Replay();
                    else
                        Stop();
                }
            }
            else
            {
                if (time == 0 || (Data.isFragment && time >= Data.endTime))
                {
                    if (IsLoop)
                        Replay();
                    else
                        Stop();
                }
            }
        }
    }
}
