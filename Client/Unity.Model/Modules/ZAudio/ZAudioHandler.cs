/**
 *  ZAudioHandler.cs
 *  
 **/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework 
{
    internal sealed class ZAudioHandler
    {
        internal ZAudioHandler() {
            InnerChannel = new ZAudioChannel(null, false);
            foreach (ChannelType item in Enum.GetValues(typeof(ChannelType)))
            {
                bool isEx = false;
                switch (item)
                {
                    case ChannelType.音效:
                        isEx = false;
                        break;
                    case ChannelType.BGM:
                        isEx = true;
                        break;
                    case ChannelType.解说:
                        isEx = true;
                        break;
                }
                var newChannel = new ZAudioChannel(item, isEx);
                AudioChannelDic.Add(item, newChannel);
            }
            var lib = Resources.Load<ZAudioLibrary>("AL");//临时用的
            if (lib != null)
            {
                foreach (var item in lib.audioLibraryDatas)
                {
                    if (!Datas.ContainsKey(item.name))
                    {
                        Datas.Add(item.name, item);
                    }
                }
            }
        }
        private ZAudioChannel InnerChannel { get; }//内置
        private Dictionary<ChannelType, ZAudioChannel> AudioChannelDic { get; } = new Dictionary<ChannelType, ZAudioChannel>();//改成配置文件定义int
        private static Dictionary<string, ZAudioLibraryData> Datas { get; } = new Dictionary<string, ZAudioLibraryData>();//引用的音频全部在这了  以后再想资源管理 先用着

        internal ZAudioPlayer Play(string name, ChannelType? channelType = null, bool loop = false, float factor = 1)
        {
            if (!Datas.TryGetValue(name, out ZAudioLibraryData data) || data == null || data.clip == null) return null;

            if (channelType == null)//内置声道
                return InnerChannel.Play(data, loop, factor);
            else//指定声道
                return AudioChannelDic[channelType.Value].Play(data, loop, factor);
        }

        private bool _globalMute = false;
        private float _globalVolume = 1f;
        private bool _globalPaused = false;
        internal bool GlobalMute
        {
            get
            {
                return _globalMute;
            }
            set
            {
                _globalMute = value;
            }
        }
        internal float GlobalVolume
        {
            get
            {
                return GlobalMute ? 0f : _globalVolume;
            }
            set
            {
                if (GlobalMute)
                    GlobalMute = false;
                value = Mathf.Clamp01(value);
            }
        }
        internal bool GlobalPaused
        {
            get
            {
                return _globalPaused;
            }
            set
            {
                _globalPaused = value;
            }
        }

        internal bool GetMute(ChannelType? type) => type == null ? InnerChannel.IsMute : AudioChannelDic[type.Value].IsMute;
        internal float GetVolume(ChannelType? type) => type == null ? InnerChannel.Volume : AudioChannelDic[type.Value].Volume;
        internal bool GetPaused(ChannelType? type) => type == null ? InnerChannel.IsPaused : AudioChannelDic[type.Value].IsPaused;

        internal void SetMute(bool mute, ChannelType? type)
        {
            if (type == null)
            {
                InnerChannel.IsMute = mute;
            }
            else
            {
                AudioChannelDic[type.Value].IsMute = mute;
            }
        }
        internal void SetVolume(float volume, ChannelType? type)
        {

        }
        internal void SetPaused(bool paused, ChannelType? type)
        {
        }
        internal void StopChannel(ChannelType? type) {
            if (type == null)
                InnerChannel.Stop();
            else
                AudioChannelDic[type.Value].Stop();
        }
        internal void StopAll() {
            InnerChannel.Stop();
            foreach (var item in AudioChannelDic)
            {
                item.Value.Stop();
            }
        }

        // 临时用的组件池子
        private static Queue<AudioSource> AudioSourcePool = new Queue<AudioSource>();//组件池 auduiplayer给他做壳  保护audiosource不被引用  队列里面是空闲的
        internal static AudioSource GetAudioSource()
        {
            if (AudioSourcePool.Count > 0) {
                var temp = AudioSourcePool.Dequeue();
                if (temp != null) {
                    temp.gameObject.SetActive(true);
                    return temp;
                }
                return GetAudioSource();
            }
            var ass = new GameObject("AudioSource").AddComponent<AudioSource>();
            UnityEngine.Object.DontDestroyOnLoad(ass.gameObject);
            ass.playOnAwake = false;
            ass.loop = false;
            return ass;
        }
        internal static void 回收AudioSource(AudioSource audioSource) {
            if (audioSource != null)
            {
                audioSource.clip = null;
                AudioSourcePool.Enqueue(audioSource);
                audioSource.gameObject.SetActive(false);
            }
        }
    }
}

