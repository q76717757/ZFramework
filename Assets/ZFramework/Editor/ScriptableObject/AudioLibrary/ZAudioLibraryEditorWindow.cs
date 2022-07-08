/** Header
 *  ZAudioLibraryEditorWindow.cs
 *  音频库的切片编辑界面
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace ZFramework {
using UEvent = UnityEngine.Event;

    internal class ZAudioLibraryEditorWindow : EditorWindow
    {
        #region 重定向
        static Vector2 windowMinSize = new Vector2(800, 500);
        [UnityEditor.Callbacks.OnOpenAsset(0)]//Open重定向  双击lib文件也可以打开这个面板
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var instance = EditorUtility.InstanceIDToObject(instanceID);
            if (instance != null && instance is ZAudioLibrary)
            {
                Open(instance as ZAudioLibrary);
                return true;
            }
            return false;
        }
        internal static void Open(ZAudioLibrary library)
        {
            var window = EditorWindow.GetWindow<ZAudioLibraryEditorWindow>("ZAudio", true);
            if (window == null) return;
            window.minSize = windowMinSize;
            window.Show();
            window.SetTarget(library);
        }
        #endregion

        ZAudioLibrary TargetLib;
        List<AudioClip> audioClips = new List<AudioClip>();//Assets下的全部AudioClip  先用静态的存着先?
        List<ZAudioLibraryData> tempLibraryDatas = new List<ZAudioLibraryData>();//实际编辑的副本
        AudioClip SelectClip;//从左边选中的文件
        ZAudioLibraryData SelectData;//从右边选中的条目

        Vector2 scrollLeft;
        Vector2 scrollRight;
        private void SetTarget(ZAudioLibrary library) {
            TargetLib = library;
            tempLibraryDatas = new List<ZAudioLibraryData>();//不能clear  保存的时候他和target同引用  clear会把target也clear了

            if (TargetLib == null || TargetLib.audioLibraryDatas == null) return;

            for (int i = 0; i < TargetLib.audioLibraryDatas.Count; i++)//克隆一个副本 以供编辑
            {
                var clone = library.audioLibraryDatas[i].Clone();
                tempLibraryDatas.Add(clone);
            }
            OnEnable();
        }

        private void OnEnable()
        {
            SelectClip = null;
            SelectData = null;
            holeType = -1;
            startP = 0;
            endP = 1;
            Repaint();
            //GetAllAudioClipInAssset();
        }
        private void OnDisable()
        {
            ZAudioUtility.StopAllClips();
            SelectClip = null;
            SelectData = null;
        }
        private void OnProjectChange()
        {
            SelectClip = null;
            SelectData = null;
            //GetAllAudioClipInAssset();
        }
        private void OnGUI()
        {
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawLeft();
            DrawRight();
            GUILayout.EndHorizontal();
        }
        private void Update()
        {
            if (SelectClip == null && (SelectData == null || (SelectData != null && SelectData.clip == null)))
            {
                return;//没选
            }

            if (SelectData != null && SelectData.isFragment)
            {
                if (ZAudioUtility.IsClipPlaying(SelectData.clip))
                {
                    if (ZAudioUtility.GetClipPosition(SelectData.clip) > SelectData.endTime)
                    {
                        ZAudioUtility.StopAllClips();
                    }
                }
            }
        }

        void GetAllAudioClipInAssset() {//后面做成分帧读取 有的工程资源太多 会卡住
            audioClips.Clear();
            var asssets = AssetDatabase.GetAllAssetPaths();
            foreach (var item in asssets)
            {
                var t = AssetDatabase.GetMainAssetTypeAtPath(item);
                if (t != null && t == typeof(AudioClip))
                {
                    var audioclip = AssetDatabase.LoadAssetAtPath<AudioClip>(item);
                    if (audioclip != null)
                    {
                        audioClips.Add(audioclip);
                    }
                }
            }
        }
        void DrawLeft() {
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.ExpandHeight(true), GUILayout.Width(300));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Assets下的全部音频");
            if (GUILayout.Button("全部添加"))
            {
                for (int i = 0; i < audioClips.Count; i++)
                {
                    tempLibraryDatas.Add(new ZAudioLibraryData()
                    {
                        name = audioClips[i].name,
                        clip = audioClips[i],
                        startTime = 0,
                        endTime = audioClips[i].length,
                        isFragment = false,
                    });
                }
            }
            if (GUILayout.Button("刷新全部"))
            {
                GetAllAudioClipInAssset();
            }
            EditorGUILayout.EndHorizontal();

            scrollLeft = GUILayout.BeginScrollView(scrollLeft, GUILayout.ExpandHeight(true), GUILayout.Width(350));
            for (int i = 0; i < audioClips.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("HelpBox");
                EditorGUILayout.LabelField(SelectClip == audioClips[i] ? "√" : (i + 1).ToString(), GUILayout.Width(25));
                EditorGUILayout.ObjectField(audioClips[i], typeof(AudioClip), false, GUILayout.Width(100));
                if (GUILayout.Button("play"))
                {
                    ZAudioUtility.StopAllClips();
                    ZAudioUtility.PlayClip(audioClips[i]);
                }
                if (GUILayout.Button("选择"))
                {
                    SetClip(audioClips[i]);
                }
                if (GUILayout.Button("删除"))
                {
                    if (AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(audioClips[i])))
                    {
                        EditorGUILayout.EndHorizontal();
                        AssetDatabase.Refresh();
                        break;
                    }
                }
                EditorGUILayout.LabelField("using", GUILayout.Width(40));
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
        void DrawRight() {
            GUILayout.BeginVertical("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            DrawRightUp();

            DrawRightMid();

            DrawRightDown();
            GUILayout.EndVertical();
        }


        void DrawRightUp() {
            //标题
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.LabelField("序号", GUILayout.Width(25));
            EditorGUILayout.LabelField("name", GUILayout.Width(200));
            EditorGUILayout.LabelField("clip", GUILayout.Width(200));
            EditorGUILayout.LabelField("片段", GUILayout.Width(30));
            EditorGUILayout.LabelField("start", GUILayout.Width(100));
            EditorGUILayout.LabelField("end", GUILayout.Width(100));
            EditorGUILayout.LabelField("Info", GUILayout.MinWidth(200));
            EditorGUILayout.LabelField("", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            //内容
            scrollRight = EditorGUILayout.BeginScrollView(scrollRight, "HelpBox", GUILayout.ExpandHeight(true));
            if (tempLibraryDatas == null)
            {
                EditorGUILayout.HelpBox("Lib is Null", MessageType.Info);
            }
            else
            {
                EditorGUILayout.BeginVertical();
                var oldColor = GUI.backgroundColor;
                for (int i = 0; i < tempLibraryDatas.Count; i++)
                {
                    if (SelectData != null && SelectData == tempLibraryDatas[i])
                        GUI.backgroundColor = Color.green;

                    EditorGUILayout.BeginHorizontal("HelpBox");
                    EditorGUILayout.LabelField((i + 1).ToString(), GUILayout.Width(25));
                    tempLibraryDatas[i].name = EditorGUILayout.TextField(tempLibraryDatas[i].name, GUILayout.Width(200));
                    tempLibraryDatas[i].clip = (AudioClip)EditorGUILayout.ObjectField(tempLibraryDatas[i].clip, typeof(AudioClip), false, GUILayout.Width(200));
                    tempLibraryDatas[i].isFragment = EditorGUILayout.Toggle(tempLibraryDatas[i].isFragment, GUILayout.Width(30));

                    EditorGUI.BeginDisabledGroup(!tempLibraryDatas[i].isFragment);//片段才能编辑
                    tempLibraryDatas[i].startTime = EditorGUILayout.FloatField(tempLibraryDatas[i].startTime, GUILayout.Width(100));
                    tempLibraryDatas[i].endTime = EditorGUILayout.FloatField(tempLibraryDatas[i].endTime, GUILayout.Width(100));
                    EditorGUI.EndDisabledGroup();

                    tempLibraryDatas[i].info = EditorGUILayout.TextField(tempLibraryDatas[i].info, GUILayout.MinWidth(200));

                    if (GUILayout.Button("选择", GUILayout.Width(50)))
                    {
                        ZAudioUtility.StopAllClips();
                        Select(tempLibraryDatas[i]);
                    }

                    if (GUILayout.Button("删除", GUILayout.Width(50)))
                    {
                        tempLibraryDatas.RemoveAt(i);
                        EditorGUILayout.EndHorizontal();
                        break;
                    }
                    EditorGUILayout.EndHorizontal();

                    GUI.backgroundColor = oldColor;
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndScrollView();
        }
        void DrawRightMid() {
            EditorGUILayout.BeginHorizontal("HelpBox");

            if (GUILayout.Button("添加"))
            {
                var newData = new ZAudioLibraryData();
                tempLibraryDatas.Add(newData);
                Select(newData);
            }
            if (GUILayout.Button("保存"))
            {
                TargetLib.audioLibraryDatas = tempLibraryDatas;
                EditorUtility.SetDirty(TargetLib);
                AssetDatabase.SaveAssets();

                SetTarget(TargetLib);//为了刷新templist 不然保存后引用一样了
            }

            if (SelectData != null)//选了片段
            {
                if (GUILayout.Button($"试听"))
                {
                    ZAudioUtility.StopAllClips();
                    if (SelectData.clip != null)
                    {
                        ZAudioUtility.PlayClip(SelectData.clip);
                        ZAudioUtility.SetClipSamplePosition(SelectData.clip, (int)(startP * SelectData.clip.samples));
                    }
                }
            }
            if (SelectClip != null)//选了文件
            {
                if (GUILayout.Button("插入"))
                {
                    tempLibraryDatas.Add(new ZAudioLibraryData()
                    {
                        name = SelectClip.name,
                        clip = SelectClip,
                        startTime = (int)startP* SelectClip.length,
                        endTime = (int)endP* SelectClip.length,
                        isFragment = true,
                    });
                }
            }

            EditorGUILayout.EndHorizontal();
        }
        void DrawRightDown() {
            if (SelectClip == null && (SelectData == null || (SelectData != null && SelectData.clip == null)))
            {
                EditorGUILayout.HelpBox("没有选择", MessageType.Info);
            }
            else
            {
                Rect r = GUILayoutUtility.GetRect(GUIContent.none, "HelpBox", GUILayout.ExpandWidth(true), GUILayout.MaxHeight(180));
                var evt = UEvent.current;
                if (evt.type == UnityEngine.EventType.Repaint)
                {
                    new GUIStyle("HelpBox").Draw(r, false, false, false, false);
                    DoRenderPreview(r);
                }

                //左刻度线
                var startCtrlRect = new Rect(r.x + (int)(r.width * startP) - 2, r.y, 2 + 4, r.height);
                EditorGUIUtility.AddCursorRect(startCtrlRect, MouseCursor.ResizeHorizontal);
                var startRect = new Rect(r.x + (int)(r.width * startP), r.y, 2, r.height);
                GUI.DrawTexture(startRect, EditorGUIUtility.whiteTexture);
                GUI.Label(new Rect(startRect.position, new Vector2(50, 20)), startP.ToString());
                //右刻度线
                var endCtrlRect = new Rect(r.x + (int)(r.width * endP) - 2, r.y, 2 + 4, r.height);
                EditorGUIUtility.AddCursorRect(endCtrlRect, MouseCursor.ResizeHorizontal);
                var endRect = new Rect(r.x + (int)(r.width * endP), r.y, 2, r.height);
                GUI.DrawTexture(endRect, EditorGUIUtility.whiteTexture);
                GUI.Label(new Rect(endRect.position, new Vector2(50, 20)), endP.ToString());
                //事件
                if (evt.type != UnityEngine.EventType.Repaint && evt.type != UnityEngine.EventType.Layout && evt.type != UnityEngine.EventType.Used)
                {
                    switch (evt.type)
                    {
                        case UnityEngine.EventType.MouseDown:
                            if (startCtrlRect.Contains(evt.mousePosition))
                            {
                                holeType = 0;
                                evt.Use();
                            }
                            if (endCtrlRect.Contains(evt.mousePosition))
                            {
                                holeType = 1;
                                evt.Use();
                            }
                            break;
                        case UnityEngine.EventType.MouseUp:
                            if (holeType != -1)
                            {
                                holeType = -1;
                                evt.Use();
                                if (!ZAudioUtility.IsClipPlaying(SelectData.clip))//松手就播放
                                    ZAudioUtility.PlayClip(SelectData.clip);
                                ZAudioUtility.SetClipSamplePosition(SelectData.clip, (int)(startP * SelectData.clip.samples));
                            }
                            break;
                        case UnityEngine.EventType.MouseDrag:
                            switch (holeType)
                            {
                                case 0:
                                    startP = Mathf.Clamp01((evt.mousePosition.x - r.x) / r.width);
                                    evt.Use();
                                    if (startP > endP)
                                    {
                                        var t = endP;
                                        endP = startP;
                                        startP = t;
                                        holeType = 1;
                                    }
                                    UpdateP();
                                    break;
                                case 1:
                                    endP = Mathf.Clamp01((evt.mousePosition.x - r.x) / r.width);
                                    evt.Use();
                                    if (startP > endP)
                                    {
                                        var t = endP;
                                        endP = startP;
                                        startP = t;
                                        holeType = 0;
                                    }
                                    UpdateP();
                                    break;
                            }
                            break;
                        case UnityEngine.EventType.ContextClick:
                            if (r.Contains(evt.mousePosition))
                            {
                                GenericMenu menu = new GenericMenu();
                                menu.AddItem(new GUIContent("设置起点"), false, SetStartP, Mathf.Clamp01((evt.mousePosition.x - r.x) / r.width));
                                menu.AddItem(new GUIContent("设置终点"), false, SetEndP, Mathf.Clamp01((evt.mousePosition.x - r.x) / r.width));
                                menu.AddSeparator("");
                                menu.AddItem(new GUIContent("重置"), false, ResetP);
                                menu.ShowAsContext();
                                evt.Use();
                            }
                            break;
                    }
                }
            }
        }
        void SetClip(AudioClip clip) {
            SelectClip = clip;
            SelectData = null;
            if (clip != null)
            {
                startP = 0;
                endP = 1;
                samples = ZAudioUtility.GetMinMaxData(ZAudioUtility.GetImporterFromClip(SelectClip));//波形是被压缩过的  不能做放大细节
                channelCount = ZAudioUtility.GetChannelCount(SelectClip);
                samplesLength = samples == null ? 0 : samples.Length / (2 * channelCount);
            }
        }
        void Select(ZAudioLibraryData data) {
            SelectData = data;
            SelectClip = null;
            
            if (data.clip != null)
            {
                if (data.isFragment)
                {
                    startP = data.startTime / data.clip.length;
                    endP = data.endTime / data.clip.length;
                }
                else
                {
                    startP = 0;
                    endP = 1;
                }
                samples = ZAudioUtility.GetMinMaxData(ZAudioUtility.GetImporterFromClip(data.clip));//波形是被压缩过的 不能做放大细节
                channelCount = ZAudioUtility.GetChannelCount(data.clip);
                samplesLength = samples == null ? 0 : samples.Length / (2 * channelCount);
            }
        }

        int holeType = -1;//抓住左刻度=0 右刻度=1 没有=-1
        void SetStartP(object obj) {
            startP = (float)obj;
            if (startP > endP)
            {
                var t = endP;
                endP = startP;
                startP = t;
            }
            UpdateP();
        }
        void SetEndP(object obj) {
            endP = (float)obj;
            if (startP > endP)
            {
                var t = endP;
                endP = startP;
                startP = t;
            }
            UpdateP();
        }
        void ResetP() {
            startP = 0;
            endP = 1;
            SelectData.startTime = 0;
            SelectData.endTime = SelectData.clip.length;
            SelectData.isFragment = false;
        }

        void UpdateP() {
            if (SelectData != null && SelectData.clip != null)
            {
                SelectData.startTime = startP * SelectData.clip.length;
                SelectData.endTime = endP * SelectData.clip.length;
                SelectData.isFragment = true;
            }
        }

        int samplesLength;
        int channelCount;
        float[] samples;
        int channel;
        void DoRenderPreview(Rect wantedRect)
        {
            float h = wantedRect.height / channelCount;
            for (channel = 0; channel < channelCount; channel++)
            {
                Rect channelRect = new Rect(wantedRect.x, wantedRect.y + h * channel, wantedRect.width, h);
                AudioCurveRendering.DrawMinMaxFilledCurve(channelRect, Evaluator);
            }
        }
        float startP;//起点的位置
        float endP;//终点的位置
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
            if (x < startP || x > endP)
            {
                col = new Color(x, 1 - x, 0.2f, 0.3f);
            }
            else
            {
                col = new Color(x, 1 - x, 0.2f, 1);
            }
        }

    }
}
