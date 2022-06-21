/**
 * ZAudio.cs
 * 基于AudioSource封装的音频系统
 * 全局设置和声道的关系参考windows音量合成器->设备音量和应用音量之间的关系
 * 设计的期望是更方便的控制音量 以及匹配一套音频的事件帧系统 用来裁长音频和做语音字幕  音频切换的淡入淡出效果后面有时间再考虑做
 **/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    /// <summary> 音频系统的信道类型</summary>  //不希望让用的人从脚本改  考虑做成配置文件   映射到int上   Api使用Int  不使用枚举
    public enum ChannelType
    {
        音效,
        BGM,
        解说
    }

    /// <summary> 音频系统 </summary>
    public sealed class ZAudio
    {
        private ZAudio() { }
        private static ZAudio _instance;
        private static ZAudio Instance {
            get {
                if (_instance == null) {
                    _instance = new ZAudio();
                    _instance._handler = new ZAudioHandler();
                }
                return _instance;
            }
        }

        private ZAudioHandler _handler;
        private static ZAudioHandler Handler => Instance._handler;

        #region 公开静态方法  实际使用音频系统的入口
        public static ZAudioPlayer Play(string clipName, ChannelType? channelType = null, bool loop = false, float factor = 1) {
            if (string.IsNullOrEmpty(clipName)) return null;
            return Handler.Play(clipName, channelType, loop, factor);
        }

        /// <summary> 全局静音 </summary>
        public static bool GlobalMute { get => Handler.GlobalMute; set => Handler.GlobalMute = value; }
        /// <summary> 全局音量(取值0~1) </summary>
        public static float GlobalVolume { get => Handler.GlobalVolume; set => Handler.GlobalVolume = value; }
        /// <summary> 全局暂停 </summary>
        public static bool GlobalPaused { get => Handler.GlobalPaused; set => Handler.GlobalPaused = value; }

        //指定声道的
        public static bool GetMute(ChannelType? type = null) => Handler.GetMute(type);
        public static void SetMute(bool mute, ChannelType? type = null) => Handler.SetMute(mute, type);
        public static float GetVolume(ChannelType? type = null) => Handler.GetVolume(type);
        public static void SetVolume(float volume, ChannelType? type = null) => Handler.SetVolume(volume, type);
        public static bool GetPaused(ChannelType? type = null) => Handler.GetPaused(type);
        public static void SetPaused(bool paused, ChannelType? type = null) => Handler.SetPaused(paused, type);
        public static void StopChannel(ChannelType? type = null) => Handler.StopChannel(type);
        public static void StopAll() => Handler.StopAll();

        #endregion

    }
}

