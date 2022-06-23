/** Header
 *  AudioLibraryEditor.cs
 *  这个界面不要了  写一个功能更多的切片编辑界面  这个界面就用来预览一下
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace ZFramework {
using Event = UnityEngine.Event;

    [CustomEditor(typeof(ZAudioLibrary))]
    [CanEditMultipleObjects]//做一个多选查重的功能
    internal class ZAudioLibraryEditor : Editor
    {
        ZAudioLibrary _lib;
        ZAudioLibrary Lib => _lib ? _lib : _lib = (ZAudioLibrary)target;
        List<ZAudioLibraryData> tempLibraryDatas;//实际编辑的副本
        List<bool> chooseStates;//编辑器下用的 标识templist的显示状态  用于搜索功能 /查重

        AudioClip _reviewClip;//预览的音频
        AudioClip ReviewClip {
            get { return _reviewClip; }
            set {
                _reviewClip = value;
                samples = ZAudioUtility.GetMinMaxData(ZAudioUtility.GetImporterFromClip(value));
                channelCount = ZAudioUtility.GetChannelCount(value);
                samplesLength = samples == null ? 0 : samples.Length / (2 * channelCount);
                duration = ZAudioUtility.GetDuration(value);
                frequency = ZAudioUtility.GetFrequency(value);
                reviewTitle = new GUIContent(value.name);
                hasPreview = ZAudioUtility.HasPreview(value);
                isTrackerFile = ZAudioUtility.IsTrackerFile(value);
                samplesCount = ZAudioUtility.GetSampleCount(value);
            }
        }
        bool IsPlaying => ZAudioUtility.IsClipPlaying(ReviewClip);
        static bool _loop;

        bool hasChange = false;
        bool _multiEditing;//是否多重编辑
        float[] samples;//波形采样集
        int samplesLength;//波形震荡数量   =  samples.Length / (2 * channelCount);  /相当于平均每声道的震荡个数 每两点为一组震荡
        int samplesCount;//全部样本点
        double duration;//持续时间
        int channel;//正在绘制的声道
        int channelCount;//声道数
        int frequency;//采样率
        bool hasPreview;//有预览
        bool isTrackerFile;//是文件
        GUIContent reviewTitle;//标题

        //一些样式资源
        static GUIContent _playOnIcon;
        static GUIContent _playOffIcon;
        static GUIContent _playOnIconNoTip;
        static GUIContent _playOffIconNoTip;
        static GUIContent _loopOnIcon;
        static GUIContent _loopOffIcon;
        static Texture2D _whiteLine;
        void InitIcon()
        {
            _playOnIcon = EditorGUIUtility.TrIconContent("preAudioPlayOn", "Play On");
            _playOffIcon = EditorGUIUtility.TrIconContent("preAudioPlayOff", "Play Off");
            _playOnIconNoTip = EditorGUIUtility.TrIconContent("preAudioPlayOn", "Play");
            _playOffIconNoTip = EditorGUIUtility.TrIconContent("preAudioPlayOff", "Play");

            _loopOnIcon = EditorGUIUtility.TrIconContent("preAudioLoopOn", "Loop On");
            _loopOffIcon = EditorGUIUtility.TrIconContent("preAudioLoopOff", "Loop Off");
            _whiteLine = EditorGUIUtility.whiteTexture;

        }

        private void OnEnable()
        {
            if (_playOnIcon == null)
                InitIcon();

            _multiEditing = targets.Length > 1;
            if (_multiEditing)
            {

            }
            else
            {
                Revert();


                for (int i = 0; i < tempLibraryDatas.Count; i++)
                {
                    if (tempLibraryDatas[i].clip != null)
                    {
                        ReviewClip = tempLibraryDatas[i].clip;
                        return;
                    }
                }
            }
        }

        private void OnDisable()
        {
            ZAudioUtility.StopAllClips();
            if (hasChange)
            {
                if (EditorUtility.DisplayDialog("提示", "修改的内容没有保存", "保存修改", "取消修改"))
                {
                    Apply();
                }
            }
        }

        #region Review面板

        public override bool HasPreviewGUI()=> ReviewClip != null && !_multiEditing;
        public override GUIContent GetPreviewTitle()=> reviewTitle;

        public override void OnPreviewSettings()
        {
            bool playing = IsPlaying;
            bool newplaying = GUILayout.Toggle(playing, playing ? _playOnIcon : _playOffIcon, EditorStyles.toolbarButton);
            if (newplaying != playing)
            {
                ZAudioUtility.StopAllClips();
                if (newplaying)
                {
                    ZAudioUtility.PlayClip(ReviewClip, _loop);
                }
            }

            bool loop = _loop;
            _loop = GUILayout.Toggle(_loop, _loop ? _loopOnIcon : _loopOffIcon, EditorStyles.toolbarButton);
            if (loop != _loop && IsPlaying)
            {
                ZAudioUtility.LoopClip(ReviewClip, _loop);
            }
        }
        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            Event evt = Event.current;
            if (evt.type != UnityEngine.EventType.Repaint && evt.type != UnityEngine.EventType.Layout && evt.type != UnityEngine.EventType.Used)
            {
                switch (evt.type)
                {
                    case UnityEngine.EventType.MouseDrag:
                    case UnityEngine.EventType.MouseDown:
                        {
                            if (r.Contains(evt.mousePosition))
                            {
                                var startSample = (int)(evt.mousePosition.x * (samplesCount / (int)r.width));
                                if (!IsPlaying) {
                                    ZAudioUtility.StopAllClips();
                                    ZAudioUtility.PlayClip(ReviewClip, _loop);
                                }
                                ZAudioUtility.SetClipSamplePosition(ReviewClip, startSample);
                                evt.Use();
                            }
                        }
                        break;
                }
                return;
            }

            if (Event.current.type == UnityEngine.EventType.Repaint)
                background.Draw(r, false, false, false, false);

            var s_WantedRect = new Rect(r.x, r.y, r.width, r.height);
            float sec2px = ((float)s_WantedRect.width / ReviewClip.length);

            if (hasPreview || !isTrackerFile)
            {
                float t = ZAudioUtility.GetClipPosition(ReviewClip);

                if (Event.current.type == UnityEngine.EventType.Repaint)
                {
                    updateColor = IsPlaying;
                    playPro = t*1000/(float)duration;
                    DoRenderPreview(s_WantedRect);
                }

                for (int i = 0; i < channelCount; ++i)
                {
                    if (channelCount > 1 && r.width > 64)
                    {
                        var labelRect = new Rect(s_WantedRect.x + 5, s_WantedRect.y + (s_WantedRect.height / channelCount) * i, 30, 20);
                        EditorGUI.DropShadowLabel(labelRect, "ch " + (i + 1));
                    }
                }


                TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)(t * 1000.0f));

                GUI.DrawTexture(new Rect(s_WantedRect.x + (int)(sec2px * t), s_WantedRect.y, 2, s_WantedRect.height), _whiteLine);
                if (r.width > 64)
                    EditorGUI.DropShadowLabel(new Rect(s_WantedRect.x, s_WantedRect.y, s_WantedRect.width, 20), string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds));
                else
                    EditorGUI.DropShadowLabel(new Rect(s_WantedRect.x, s_WantedRect.y, s_WantedRect.width, 20), string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds));

            }
            else
            {
                float labelY = (r.height > 150) ? r.y + (r.height / 2) - 10 : r.y + (r.height / 2) - 25;
                if (r.width > 64)
                {
                    if (isTrackerFile)
                        EditorGUI.DropShadowLabel(new Rect(r.x, labelY, r.width, 20), string.Format("Module file with " + channelCount + " channels."));
                    else
                        EditorGUI.DropShadowLabel(new Rect(r.x, labelY, r.width, 20), "Can not show PCM data for this file");
                }

                float t = ZAudioUtility.GetClipPosition(ReviewClip);
                System.TimeSpan ts = new System.TimeSpan(0, 0, 0, 0, (int)(t * 1000.0f));
                EditorGUI.DropShadowLabel(new Rect(s_WantedRect.x, s_WantedRect.y, s_WantedRect.width, 20), string.Format("Playing - {0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds));
            }

            if (IsPlaying) {
                Repaint();
            }
        }


        bool updateColor;
        float playPro;//播放进度
        void DoRenderPreview(Rect wantedRect)
        {
            float h = wantedRect.height / channelCount;

            for (channel = 0; channel < channelCount; channel++)
            {
                Rect channelRect = new Rect(wantedRect.x, wantedRect.y + h * channel, wantedRect.width, h);

                AudioCurveRendering.DrawMinMaxFilledCurve(channelRect, Evaluator);
            }
        }
        void Evaluator(float x, out Color col, out float minValue, out float maxValue)
        {
            if (samplesLength > 0)
            {
                float p = Mathf.Clamp(x * (samplesLength - 2), 0.0f, samplesLength - 2);
                int i = (int)Mathf.Floor(p);
                int offset1 = (i * channelCount + channel) * 2;
                int offset2 = ((i + 1) * channelCount + channel) * 2;
                minValue = Mathf.Min(samples[offset1 + 1], samples[offset2 + 1]) * 0.95f;
                maxValue = Mathf.Max(samples[offset1 + 0], samples[offset2 + 0]) * 0.95f;
                if (minValue > maxValue) { float tmp = minValue; minValue = maxValue; maxValue = tmp; }
            }
            else
            {
                minValue = 0f;
                maxValue = 0f;
            }

            if (updateColor && x > playPro)
            {
                col = new Color(x, 1 - x, 0.2f, 0.3f);
            }
            else
            {
                col = new Color(x, 1 - x, 0.2f);
            }
        }

        public override string GetInfoString()
        {
            string ch = channelCount == 1 ? "Mono" : channelCount == 2 ? "Stereo" : (channelCount - 1) + ".1";
            AudioCompressionFormat platformFormat = ZAudioUtility.GetTargetPlatformSoundCompressionFormat(ReviewClip);
            AudioCompressionFormat editorFormat = ZAudioUtility.GetSoundCompressionFormat(ReviewClip);
            string s = platformFormat.ToString();
            if (platformFormat != editorFormat)
                s += " (" + editorFormat + " in editor" + ")";
            s += ", " + frequency + " Hz, " + ch + ", ";
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, (int)duration);

            if ((uint)duration == 0xffffffff)
                s += "Unlimited";
            else
                s += string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);

            return s;
        }

        #endregion

        #region Inspector面板

        string qianzui;
        string sousuo;

        //public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)//画默认图标
        //{
        //    if (Lib.t2d == null) return null;
        //    Texture2D aa = new Texture2D(Lib.t2d.width, Lib.t2d.height);
        //    aa.SetPixels(Lib.t2d.GetPixels());
        //    return aa;
        //}

        public override void OnInspectorGUI()
        {
            if (_multiEditing)
            {
                MultiInspectorGUI();
            }
            else
            {
                OneInspectorGUI();
            }
        }
        void OneInspectorGUI()
        {
            Undo.RecordObject(Lib, "change");

            if (tempLibraryDatas == null)
                Revert();
            EditorGUILayout.LabelField("音频系统使用的资源引用集合");
            GUILayout.Space(30);
            //搜索
            EditorGUILayout.BeginHorizontal();

            var tempSousuo = sousuo;
            sousuo = EditorGUILayout.TextField(sousuo, new GUIStyle("ToolbarSeachTextField"));
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(sousuo));
            if (GUILayout.Button("clear",!string.IsNullOrEmpty(sousuo)? new GUIStyle("ToolbarSeachCancelButton"):new GUIStyle("ToolbarSeachCancelButtonEmpty")))
            {
                sousuo = string.Empty;
                EditorGUI.FocusTextInControl(string.Empty);
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            if (sousuo != tempSousuo)//有改变
            {
                if (string.IsNullOrEmpty(sousuo))
                {
                    for (int i = 0; i < chooseStates.Count; i++)
                    {
                        chooseStates[i] = true;
                    }
                }
                else
                {
                    for (int i = 0; i < tempLibraryDatas.Count; i++)
                    {
                        if (tempLibraryDatas[i] != null && !string.IsNullOrEmpty(tempLibraryDatas[i].name) && tempLibraryDatas[i].name.Contains(sousuo))
                        {
                            chooseStates[i] = true;
                        }
                        else
                        {
                            chooseStates[i] = false;
                        }
                    }
                }
            }

            //批量增加前缀
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(qianzui));

            if (GUILayout.Button("批量添加前缀->"))
            {
                foreach (var item in tempLibraryDatas)
                {
                    item.name = qianzui + item.name;
                }
                qianzui = string.Empty;
                EditorGUI.FocusTextInControl(string.Empty);
                hasChange = true;
            }
            EditorGUI.EndDisabledGroup();
            qianzui = EditorGUILayout.TextField(qianzui);
            EditorGUILayout.EndHorizontal();

            //绘制条目
            for (int i = 0; i < tempLibraryDatas.Count; i++)
            {
                if (!chooseStates[i]) continue;

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField((i + 1).ToString(), GUILayout.Width(20));//编号

                EditorGUI.BeginDisabledGroup(tempLibraryDatas[i].clip == null);
                bool a = ReviewClip != null && ZAudioUtility.IsClipPlaying(ReviewClip) && ReviewClip == tempLibraryDatas[i].clip;
                if (GUILayout.Button(a ? _playOnIconNoTip : _playOffIconNoTip, GUILayout.Height(15)))
                {
                    ReviewClip = tempLibraryDatas[i].clip;
                    ZAudioUtility.StopAllClips();
                    ZAudioUtility.PlayClip(ReviewClip, _loop);
                }
                EditorGUI.EndDisabledGroup();



                EditorGUI.BeginChangeCheck();
                tempLibraryDatas[i].name = EditorGUILayout.TextField(tempLibraryDatas[i].name);
                tempLibraryDatas[i].clip = (AudioClip)EditorGUILayout.ObjectField(tempLibraryDatas[i].clip, typeof(AudioClip), false);

                if (GUILayout.Button("删除"))
                {
                    tempLibraryDatas[i] = new ZAudioLibraryData();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    hasChange = true;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.BeginChangeCheck();

            var eventType = UnityEngine.Event.current.type;
            if (eventType == UnityEngine.EventType.DragUpdated|| eventType == UnityEngine.EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (eventType == UnityEngine.EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (var item in DragAndDrop.objectReferences)
                    {
                        if (item is AudioClip)
                        {
                            tempLibraryDatas.Add(new ZAudioLibraryData() { name = item.name, clip = (AudioClip)item });
                            chooseStates.Add(true);
                            sousuo = string.Empty;
                            hasChange = true;
                        }
                        if (item is ZAudioLibrary lib)//lib合并  临时用的
                        {
                            foreach (var libItem in lib.audioLibraryDatas)
                            {
                                tempLibraryDatas.Add(libItem);
                                chooseStates.Add(true);
                            }
                            sousuo = string.Empty;
                            hasChange = true;
                        }
                    }
                    UnityEngine.Event.current.Use();
                }
            }

            EditorGUI.BeginDisabledGroup(tempLibraryDatas.Count == 0);
            if (GUILayout.Button("删除无效条目"))
            {
                for (int i = tempLibraryDatas.Count - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(tempLibraryDatas[i].name) || tempLibraryDatas[i].clip == null)
                    {
                        tempLibraryDatas.RemoveAt(i);
                    }
                }
            }
            if (GUILayout.Button("补全空索引"))
            {
                for (int i = 0; i < tempLibraryDatas.Count; i++)
                {
                    if (string.IsNullOrEmpty(tempLibraryDatas[i].name) && tempLibraryDatas[i].clip != null)
                    {
                        tempLibraryDatas[i].name = tempLibraryDatas[i].clip.name;
                    }
                }
            }
            EditorGUI.EndDisabledGroup();




            if (EditorGUI.EndChangeCheck())
            {
                hasChange = true;
            }







            //Apply and Revert
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(!hasChange);

            if (GUILayout.Button("保存")) { 
                Apply();
                EditorGUI.FocusTextInControl(string.Empty);
            }
            if (GUILayout.Button("还原")) { 
                Revert();
                EditorGUI.FocusTextInControl(string.Empty);
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Editor"))
            {
                ZAudioLibraryEditorWindow.Open(Lib);
            }

        }

        void MultiInspectorGUI()
        {
            EditorGUILayout.LabelField("目前不支持多选");
            EditorGUILayout.LabelField("多选有什么功能可以做? 多选查重??");
        }

        #endregion

        #region IO相关

        void Apply()
        {
            Lib.audioLibraryDatas = tempLibraryDatas;
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Revert();
        }
        void Revert()
        {
            tempLibraryDatas = new List<ZAudioLibraryData>();
            chooseStates = new List<bool>();
            hasChange = false;

            if (Lib.audioLibraryDatas == null) return;

            for (int i = 0; i < Lib.audioLibraryDatas.Count; i++)
            {
                var clone = Lib.audioLibraryDatas[i].Clone();
                tempLibraryDatas.Add(clone);
                chooseStates.Add(true);
            }
        }

        #endregion

        
    }

}