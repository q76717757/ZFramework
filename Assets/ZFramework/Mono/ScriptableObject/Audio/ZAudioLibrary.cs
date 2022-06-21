/** Header
 *  AudioLibrary.cs
 *  在编辑器面板开发选择导入器加载模式    小于1秒的提示内存加载  大于5秒提示流式   1~5提示压缩
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ZFramework {
    /// <summary>
    /// 这个类是一个资源集  改成脚本创建 全局唯一  把全部音频放在一个地方  顺便也方便查重
    /// </summary>
    [CreateAssetMenu(fileName = "NewAudioLibrary", menuName = "ZFramework/创建音频库", order = 1)]
    public class ZAudioLibrary : ScriptableObject
    {
        //用来序列化的
        public List<ZAudioLibraryData> audioLibraryDatas = new List<ZAudioLibraryData>();
    }

    /// <summary> 音频的切片   可以是一个完整音频 也可以是音频的某一个片段 </summary>
    [Serializable]
    public class ZAudioLibraryData
    {
        /// <summary> 别名 不是文件名,是自定义的名字 音频系统play方法就用这个名字 </summary>
        public string name;
        /// <summary> 引用的音频文件 </summary>
        public AudioClip clip;
        /// <summary> 时间起点 </summary>
        public float startTime;
        /// <summary> 时间的终点 </summary>
        public float endTime;
        /// <summary> 描述 </summary>
        public string info;
        /// <summary> 是一个片段 </summary>  是片段的时候 starttime和endtime才有意义
        public bool isFragment;

        //+事件刻度
        //+分组
        public ZAudioLibraryData Clone()
        {
            var clone = new ZAudioLibraryData()
            {
                name = name,
                clip = clip,
                info = info,
                startTime = startTime,
                endTime = endTime,
                isFragment = isFragment,
            };
            return clone;
        }
    }
}
