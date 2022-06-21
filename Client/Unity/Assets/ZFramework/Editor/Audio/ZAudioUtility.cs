/** Header
 *  ZAudioUtility.cs
 *  反射编辑器下音频相关API  用来做画示波器和播放声音
 **/

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;

namespace ZFramework
{
    internal class ZAudioUtility
    {
        static bool _inited = false;
        static Dictionary<string, MethodInfo> Dic = new Dictionary<string, MethodInfo>();

        [InitializeOnLoadMethod]
        static void Init() {
            if (_inited) return;
            var audioUtilClass = typeof(Editor).Assembly.GetType("UnityEditor.AudioUtil");
            var methods = audioUtilClass.GetMethods();
            //一次性把全部都反射出来
            foreach (MethodInfo method in methods)
            {
                int index = 0;
                string methodName = method.Name;
                if (!method.IsStatic || !method.IsPublic) continue;
                while (true)
                {
                    if (Dic.ContainsKey(methodName))//可能有重载  name后面加数字
                    {
                        methodName = method.Name + ++index;
                    }
                    else
                    {
                        var parmeters = method.GetParameters();
                        Type[] types = new Type[parmeters.Length];
                        for (int i = 0; i < parmeters.Length; i++)
                        {
                            types[i] = parmeters[i].ParameterType;
                        }

                        MethodInfo info;
                        if (parmeters.Length == 0)
                        {
                            info = audioUtilClass.GetMethod(
                            method.Name, BindingFlags.Static | BindingFlags.Public);
                        }
                        else
                        {
                            info = audioUtilClass.GetMethod(
                            method.Name, BindingFlags.Static | BindingFlags.Public, null, types, null);
                        }
                        Dic.Add(methodName, info);
                        break;
                    }
                }
            }
            _inited = true;

            return;

            //打印出来看一下
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var item in Dic)
            {
                var methodInfo = item.Value;
                sb.AppendLine(item.Key + "<-->" + methodInfo.Name);

                var paramsInof = methodInfo.GetParameters();
                sb.AppendLine("<color=green>Parameters--></color>");
                foreach (var p in paramsInof)
                {
                    sb.Append(p.ParameterType.Name);
                    sb.Append(" ");
                    sb.Append(p.Name);
                    sb.Append("<color=green>|</color>");
                }
                sb.AppendLine("");
                sb.AppendLine("<color=green>Return--></color>");
                sb.AppendLine(methodInfo.ReturnType.Name);
                Log.Info(sb.ToString());
                sb.Clear();
            }
        }

        /// <summary> 编辑器Play时重置全部Clip </summary>
        public static bool ResetAllAudioClipPlayCountsOnPlay{
            get => (bool)Dic["get_resetAllAudioClipPlayCountsOnPlay"].Invoke(null, new object[] { });
            set => Dic["set_resetAllAudioClipPlayCountsOnPlay"].Invoke(null, new object[] { value });
        }

        //* 2018版本的PlayClip有3个重载  2019版本的两个重载没有了 只保留3个参数的API
        //public static void PlayClip(AudioClip clip) => PlayClip(clip, 0, false); 
        //public static void PlayClip(AudioClip clip, int startSample) => PlayClip(clip, startSample, false);
        /// <summary> 播放声音 </summary>   第二个参数没有用 不知道原因 总是从0开始  要跳播需要再调setClipSamplePosition
        public static void PlayClip(AudioClip clip,/* int startSample = 0, */bool loop = false) => Dic["PlayClip"].Invoke(null, new object[] { clip, 0, loop });
        /// <summary> 停止播放 </summary> 
        public static void StopClip(AudioClip clip) => Dic["StopClip"].Invoke(null, new object[] { clip });
        /// <summary> 暂停播放 </summary>
        public static void PauseClip(AudioClip clip) => Dic["PauseClip"].Invoke(null, new object[] { clip });
        /// <summary> 继续播放 </summary>
        public static void ResumeClip(AudioClip clip) => Dic["ResumeClip"].Invoke(null, new object[] { clip });
        /// <summary> 设置循环 </summary>
        public static void LoopClip(AudioClip clip, bool on) => Dic["LoopClip"].Invoke(null, new object[] { clip, on });
        /// <summary> 是否播放中 </summary>
        public static bool IsClipPlaying(AudioClip clip) => (bool)Dic["IsClipPlaying"].Invoke(null, new object[] { clip });
        /// <summary> 停止全部播放 </summary>
        public static void StopAllClips() => Dic["StopAllClips"].Invoke(null, new object[] { });
        /// <summary> Get进度 0~1 </summary>
        public static float GetClipPosition(AudioClip clip) => (float)Dic["GetClipPosition"].Invoke(null, new object[] { clip });
        /// <summary> Get采样位置 </summary>
        public static int GetClipSamplePostion(AudioClip clip) => (int)Dic["GetClipSamplePostion"].Invoke(null, new object[] { clip });
        /// <summary> Set采样位置 </summary>
        public static void SetClipSamplePosition(AudioClip clip, int samplePosition) => Dic["SetClipSamplePosition"].Invoke(null, new object[] { clip, samplePosition });
        /// <summary> 采样数量 </summary>
        public static int GetSampleCount(AudioClip clip) => (int)Dic["GetSampleCount"].Invoke(null, new object[] { clip });
        /// <summary> 通道数 </summary>
        public static int GetChannelCount(AudioClip clip) => (int)Dic["GetChannelCount"].Invoke(null, new object[] { clip });
        /// <summary> 比特率 </summary>
        public static int GetBitRate(AudioClip clip) => (int)Dic["GetBitRate"].Invoke(null, new object[] { clip });
        /// <summary> 每个样本的比特数 </summary>
        public static int GetBitsPerSample(AudioClip clip) => (int)Dic["GetBitsPerSample"].Invoke(null, new object[] { clip });
        /// <summary> 频率 </summary>
        public static int GetFrequency(AudioClip clip) => (int)Dic["GetFrequency"].Invoke(null, new object[] { clip });
        /// <summary> 声音大小 </summary>
        public static int GetSoundSize(AudioClip clip) => (int)Dic["GetSoundSize"].Invoke(null, new object[] { clip });
        /// <summary> 声音压缩格式(编辑器下) </summary>
        public static AudioCompressionFormat GetSoundCompressionFormat(AudioClip clip) => (AudioCompressionFormat)Dic["GetSoundCompressionFormat"].Invoke(null, new object[] { clip });
        /// <summary> 目标平台声音压缩格式 </summary>
        public static AudioCompressionFormat GetTargetPlatformSoundCompressionFormat(AudioClip clip) => (AudioCompressionFormat)Dic["GetTargetPlatformSoundCompressionFormat"].Invoke(null, new object[] { clip });
        /// <summary> 使用声场定位技术 </summary>
        public static bool GetCanUseSpatializerEffect() => (bool)Dic["get_canUseSpatializerEffect"].Invoke(null, new object[] { });
        /// <summary> 高保真立体声解码器插件名称 </summary>
        public static string[] GetAmbisonicDecoderPluginNames() => (string[])Dic["GetAmbisonicDecoderPluginNames"].Invoke(null, new object[] { });
        /// <summary> 有预览 </summary>
        public static bool HasPreview(AudioClip clip) => (bool)Dic["HasPreview"].Invoke(null, new object[] { clip });
        /// <summary> 获取Clip的导入器 </summary>
        public static AudioImporter GetImporterFromClip(AudioClip clip) => (AudioImporter)Dic["GetImporterFromClip"].Invoke(null, new object[] { clip });
        /// <summary> 获取导入器的波形 </summary>
        public static float[] GetMinMaxData(AudioImporter importer) => (float[])Dic["GetMinMaxData"].Invoke(null, new object[] { importer });
        /// <summary> 持续时间 </summary>
        public static double GetDuration(AudioClip clip) => (double)Dic["GetDuration"].Invoke(null, new object[] { clip });
        /// <summary> FMOD(音频引擎) 内存分配 </summary>
        public static int GetFMODMemoryAllocated() => (int)Dic["GetFMODMemoryAllocated"].Invoke(null, new object[] { });
        /// <summary> FMOD(音频引擎) CPU使用率 </summary>
        public static float GetFMODCPUUsage() => (float)Dic["GetFMODCPUUsage"].Invoke(null, new object[] { });
        /// <summary> 是视频声音 </summary>
        public static bool IsMovieAudio(AudioClip clip) => (bool)Dic["IsMovieAudio"].Invoke(null, new object[] { clip });
        /// <summary> 是堆栈文件 </summary>
        public static bool IsTrackerFile(AudioClip clip) => (bool)Dic["IsTrackerFile"].Invoke(null, new object[] { clip });
        /// <summary> 获取音乐声道数 </summary>
        public static int GetMusicChannelCount(AudioClip clip) => (int)Dic["GetMusicChannelCount"].Invoke(null, new object[] { clip });
        /// <summary> 获取低通曲线 </summary>
        public static AnimationCurve GetLowpassCurve(AudioLowPassFilter lowPassFilter) => (AnimationCurve)Dic["GetLowpassCurve"].Invoke(null, new object[] { lowPassFilter });
        /// <summary> 耳朵的位置 </summary>
        public static Vector3 GetListenerPos() => (Vector3)Dic["GetListenerPos"].Invoke(null, new object[] { });
        /// <summary> 更新 </summary>
        public static void UpdateAudio() => Dic["UpdateAudio"].Invoke(null, new object[] { });
        /// <summary> 设置耳朵位置 </summary>
        public static void SetListenerTransform(Transform t) => Dic["SetListenerTransform"].Invoke(null, new object[] { t });
        /// <summary> 有回调 </summary>
        public static bool HasAudioCallback(MonoBehaviour behaviour) => (bool)Dic["HasAudioCallback"].Invoke(null, new object[] { behaviour });
        /// <summary> 自定义滤波器通道数 </summary>
        public static int GetCustomFilterChannelCount(MonoBehaviour behaviour) => (int)Dic["GetCustomFilterChannelCount"].Invoke(null, new object[] { behaviour });
        /// <summary> 自定义滤波器处理时间 </summary>
        public static int GetCustomFilterProcessTime(MonoBehaviour behaviour) => (int)Dic["GetCustomFilterProcessTime"].Invoke(null, new object[] { behaviour });
        /// <summary> 自定义滤波器MaxIn </summary>
        public static float GetCustomFilterMaxIn(MonoBehaviour behaviour, int channel) => (float)Dic["GetCustomFilterMaxIn"].Invoke(null, new object[] { behaviour, channel });
        /// <summary> 自定义滤波器MaxOut </summary>
        public static float GetCustomFilterMaxOut(MonoBehaviour behaviour, int channel) => (float)Dic["GetCustomFilterMaxOut"].Invoke(null, new object[] { behaviour, channel });

    }
}

