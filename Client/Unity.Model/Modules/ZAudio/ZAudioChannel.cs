/**
 *  ZAudioChannel.cs
 *  音频系统的声道      音频系统可以有多个自定义声道  使用声道可对播放源进行分组并且集中控制 比如背景 特效 人声 UI声道等    (本质上就是给声音分组)
 *  决定用int来标记声道数  放弃使用枚举  通过外部文件配置 不要在脚本里面改枚举
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZFramework {
    /// <summary> 声道 </summary>
    internal sealed class ZAudioChannel
    {
        internal ZAudioChannel(ChannelType? type, bool isExclusive)
        {
            Type = type;
            IsExclusive = isExclusive;
            _isMute = false;
            _volume = 1f;
            _isPaused = false;

            if (!isExclusive)
            {
                PlayerPool = new List<ZAudioPlayer>();
            }
        }
        private ZAudioPlayer _exclusiveTarget;
        private List<ZAudioPlayer> PlayerPool;
        /// <summary> 声道类型 </summary>
        internal ChannelType? Type { get; }
        /// <summary> 独占 </summary>
        internal bool IsExclusive { get; }

        private bool _isMute;
        private float _volume;
        private bool _isPaused;
        internal bool IsMute
        {
            get {
                return _isMute;
            }
            set {
                _isMute = value;
                if (IsExclusive)
                {
                    if (_exclusiveTarget != null)
                        _exclusiveTarget.Mute = value;
                }
                else
                {
                    if (PlayerPool != null)
                    {
                        foreach (var item in PlayerPool)
                        {
                            if (item != null)
                                item.Mute = value;
                        }
                    }
                    
                }
            }
        }
        internal float Volume
        {
            get {
                return IsMute ? 0f : _volume;
            }
            set {
                _volume = value;
            }
        }
        internal bool IsPaused
        {
            get
            {
                return _isPaused;
            }
            set {
                _isPaused = value;
            }
        }

        internal ZAudioPlayer Play(ZAudioLibraryData data, bool loop = false, float factor = 1) {
            if (IsExclusive)
            {
                if (_exclusiveTarget != null)
                {
                    _exclusiveTarget.Stop();
                }//重新实例  引用释放掉了
                _exclusiveTarget = new ZAudioPlayer(data, Type, factor, loop);
                return _exclusiveTarget;
            }
            else
            {
                ZAudioPlayer player = new ZAudioPlayer(data, Type, factor, loop);
                PlayerPool.Add(player);
                return player;
            }
        }
        internal void Continue() { }
        internal void Paused() { }
        internal void Stop() {
            if (PlayerPool != null)
            {
                foreach (var item in PlayerPool)
                {
                    if (item != null)
                    {
                        item.Stop();
                    }
                }
            }
            if (_exclusiveTarget != null)
            {
                _exclusiveTarget.Stop();
            }
        }

        void Update()
        {
            if (IsExclusive)
            {
                var player = _exclusiveTarget;//未完成
                if (player != null && player.IsALive)
                {
                    player.Update();
                }
                else
                {

                }
            }
            else
            {
                int len = PlayerPool.Count;
                for (int i = 0; i < len; i++)
                {
                    if (PlayerPool[i] == null) continue;
                    if (PlayerPool[i].IsALive)
                    {
                        PlayerPool[i].Update();
                    }
                    else
                    {
                        PlayerPool[i] = null;
                    }
                }
                for (int i = len - 1; i >= 0; i--)
                {
                    if (PlayerPool[i] == null)
                    {
                        PlayerPool.RemoveAt(i);
                    }
                }
            }
        }

    }

}