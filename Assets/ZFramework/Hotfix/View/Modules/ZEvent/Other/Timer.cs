using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework 
{
    /// <summary> 计时器 主要用来读取计时器状态或者干涉计时器 </summary>
    public sealed class Timer  //本身没什么实际方法 就是起一个连接作用  timer和listener是一对一关系  可以看成timer是listener的壳
    {
        internal Timer(TimerEventListenerBase listener) {
            _link = listener;
        }
        private TimerEventListenerBase _link;

        /// <summary> 持续时间(一个周期的长度) </summary>
        public float Length { get => _link.Length; }
        /// <summary> 当前循环的序号 从0开始算起 </summary>
        public int LoopIndex { get => _link.LoopIndex; }
        /// <summary> 总循环次数 </summary>
        public int LoopCount { get => _link.LoopCount; }
        /// <summary> 是否真实时间(不受Time.Scale影响) </summary>
        public bool IsReadTime { get => _link.IsReadTime; }
        /// <summary> 计时器当前的状态 </summary>
        public TimerState State { get => _link.State; }
        /// <summary> 当前进度  (从0到1,仅第一个周期包含0,所有周期都包含1  即 0...1...1...1...1) </summary>
        public float Progress { get => _link.Progress; }


        /// <summary> 继续计时器(仅对暂停中的起效果,会触发continue事件) </summary>
        public void Continue() => ZEvent.TimerEvent.CallEvent(_link, TimerEventType.Continue);
        /// <summary> 暂停计时器(仅对运行中的起效果,会触发paused事件) </summary>
        public void Paused() => ZEvent.TimerEvent.CallEvent(_link, TimerEventType.Pause);
        /// <summary> 重启计时器(重新开始计时,会触发restart事件,无论当前处于什么状态下) </summary>
        public void Restart() => ZEvent.TimerEvent.CallEvent(_link, TimerEventType.Restart);
        /// <summary> 杀死计时器(仅对存活的起效果,会触发kill事件) </summary>
        public void Kill() => ZEvent.TimerEvent.CallEvent(_link, TimerEventType.Kill);
    }

}
