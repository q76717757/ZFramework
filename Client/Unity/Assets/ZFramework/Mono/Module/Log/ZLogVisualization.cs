using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZFramework
{
    public enum LogType
    {
        Console,
        Memory,
        Screen,
        Quality,
        System,
        Project
    }

    public struct LogData
    {
        public LogData(string message, string stackTrace, UnityEngine.LogType type)
        {
            time = System.DateTime.Now.ToString("[HH:mm:ss]");
            this.message = message;
            this.stackTrace = stackTrace;
            this.type = type;
        }
        public string time;
        public string message;
        public string stackTrace;
        public UnityEngine.LogType type;
    }


    public class ZLogVisualization : MonoBehaviour
    {
        private static bool _initialized = false;
        private static int _maxLogCount = 512;
        private static readonly List<LogData> _logDatas = new List<LogData>(_maxLogCount);
        private static ZLogVisualization _visua;

        //确保在场景载入之前就注册,这样可以收集到awake的debug
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitBeforeAwake()
        {
            if (!_initialized)
            {
                _initialized = true;
                Application.logMessageReceived += LogCallback;
            }
        }

        /// <summary> OnGui效率偏低 限制绘制的日志数量 初始长度512 </summary>
        public static int MaxCount
        {
            get { return _maxLogCount; }
            set { _maxLogCount = Mathf.Clamp(value, 0, 9999); }
        }

        /// <summary> 可视化 </summary>
        public static bool Visua
        {
            get
            {
                return _visua != null;
            }
            set
            {
                if (value)
                {
                    if (_visua == null)
                    {
                        var go = new GameObject("[Log]");
                        _visua = go.AddComponent<ZLogVisualization>().Init(_logDatas);
                        UnityEngine.Object.DontDestroyOnLoad(go);
                    }
                }
                else
                {
                    if (_visua != null)
                    {
                        UnityEngine.Object.Destroy(_visua.gameObject);
                        _visua = null;
                    }
                }
            }
        }

        private static void LogCallback(string message, string stackTrace, UnityEngine.LogType type)
        {
            if (type == UnityEngine.LogType.Warning)
                return;

            if (_logDatas.Count >= _maxLogCount)
                _logDatas.RemoveRange(0, _logDatas.Count - _maxLogCount);
            _logDatas.Add(new LogData(message, stackTrace, type));
        }








        private LogType _debugType = LogType.Console;
        private List<LogData> _logInfos;
        private int _logIndex = -1;
        private bool _showInfo = true;
        private bool _showWarning = true;
        private bool _showError = true;
        private bool _showFatal = true;
        private Vector2 _scrollLogValue = Vector2.zero;
        private Vector2 _scrollTraceValue = Vector2.zero;
        private Vector2 _scrollSystemValue = Vector2.zero;
        private Vector2 _scrollScreenValue = Vector2.zero;
        private Vector2 _scrollQualityValue = Vector2.zero;
        private Vector2 _scrollProjectValue = Vector2.zero;
        private Rect _windowRect;

        private float _windowWidth;
        private float _windowHeight;
        private float _leftBtnWidth;
        private float _leftBtnHight;

        private bool _isOn = false;

        private int _fps = 0;
        private Color _fpsColor = Color.white;
        private int _frameNumber = 0;
        private float _lastShowFPSTime = 0f;

        internal ZLogVisualization Init(List<LogData> logDatas)
        {
            _logInfos = logDatas;
            return this;
        }

        private void Awake()
        {
            ResetWidthHight(Screen.width, Screen.height);
            _windowRect = new Rect(0, 0, _leftBtnWidth + 20, _leftBtnHight + 30);
        }
        private void Start()
        {
            if (_logInfos == null)
            {
                Destroy(gameObject);
            }
        }

        void ResetWidthHight(int width, int height)
        {
#if UNITY_STANDALONE
            _windowWidth = width * 0.5f;
            _windowHeight = height * 0.5f;
#else
            _windowWidth = width;
            _windowHeight = height;
#endif
            _leftBtnWidth = _windowWidth * 0.15f;
            _leftBtnHight = (_windowHeight - ((7 - 1) * 5 + 20)) / 9f;
        }

        private void OnDestroy()
        {
            if (_logInfos != null)
            {
                Visua = false;
            }
        }

        void Update()
        {
            _frameNumber += 1;
            float time = Time.realtimeSinceStartup - _lastShowFPSTime;
            if (time >= 1)
            {
                _fps = (int)(_frameNumber / time);
                _frameNumber = 0;
                _lastShowFPSTime = Time.realtimeSinceStartup;
            }
            _fpsColor = _fps > 50 ? Color.green : _fps > 25 ? Color.yellow : Color.red;
        }

        private void OnGUI()
        {
            if (_isOn)
            {
                _windowRect = GUI.Window(0, _windowRect, ExpansionGUIWindow, $"[ZLog] {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            }
            else
            {
                _windowRect = GUI.Window(0, _windowRect, ShrinkGUIWindow, "[ZLog]");
            }
        }

        private void ExpansionGUIWindow(int windowId)
        {
            GUILayout.BeginHorizontal();

            DrawTitle();

            switch (_debugType)
            {
                case LogType.Console:
                    DrawConsole();
                    break;
                case LogType.Memory:
                    DrawMemory();
                    break;
                case LogType.Screen:
                    DrawScreen();
                    break;
                case LogType.Quality:
                    DrawQuality();
                    break;
                case LogType.System:
                    DrawSystem();
                    break;
                case LogType.Project:
                    DrawProject();
                    break;
                default:
                    break;
            }
            GUILayout.EndHorizontal();
        }
        private void ShrinkGUIWindow(int windowId)
        {

            GUI.contentColor = _fpsColor;
            if (GUILayout.Button("FPS:" + _fps, GUILayout.Width(_leftBtnWidth), GUILayout.Height(_leftBtnHight)))
            {
                _isOn = true;
                _windowRect.x = 0;
                _windowRect.y = 0;
                _windowRect.width = _windowWidth;
                _windowRect.height = _windowHeight;
            }
            GUI.contentColor = Color.white;

            GUI.DragWindow();//拖动窗口
        }

        void DrawTitle()
        {
            GUILayout.BeginVertical();
            GUI.contentColor = _fpsColor;
            if (GUILayout.Button("FPS:" + _fps, GUILayout.Height(_leftBtnHight), GUILayout.Width(_leftBtnWidth)))
            {
                _isOn = false;
                _windowRect.width = _leftBtnWidth + 20;
                _windowRect.height = _leftBtnHight + 30;
            }
            GUI.contentColor = _debugType == LogType.Console ? Color.white : Color.gray;
            if (GUILayout.Button("调试输出", GUILayout.Height(_leftBtnHight), GUILayout.Width(_leftBtnWidth)))
            {
                _debugType = LogType.Console;
            }
            GUI.contentColor = _debugType == LogType.Memory ? Color.white : Color.gray;
            if (GUILayout.Button("内存状态", GUILayout.Height(_leftBtnHight), GUILayout.Width(_leftBtnWidth)))
            {
                _debugType = LogType.Memory;
            }
            GUI.contentColor = _debugType == LogType.Screen ? Color.white : Color.gray;
            if (GUILayout.Button("屏幕分辨率", GUILayout.Height(_leftBtnHight), GUILayout.Width(_leftBtnWidth)))
            {
                _debugType = LogType.Screen;
            }
            GUI.contentColor = _debugType == LogType.Quality ? Color.white : Color.gray;
            if (GUILayout.Button("质量设置", GUILayout.Height(_leftBtnHight), GUILayout.Width(_leftBtnWidth)))
            {
                _debugType = LogType.Quality;
            }
            GUI.contentColor = _debugType == LogType.System ? Color.white : Color.gray;
            if (GUILayout.Button("系统信息", GUILayout.Height(_leftBtnHight), GUILayout.Width(_leftBtnWidth)))
            {
                _debugType = LogType.System;
            }
            GUI.contentColor = _debugType == LogType.Project ? Color.white : Color.gray;
            if (GUILayout.Button("项目信息", GUILayout.Height(_leftBtnHight), GUILayout.Width(_leftBtnWidth)))
            {
                _debugType = LogType.Project;
            }
            GUI.contentColor = Color.white;
            GUILayout.EndVertical();

        }

        void DrawConsole()
        {
            GUILayout.BeginVertical("Box", GUILayout.Width(_windowWidth - _leftBtnWidth - 20), GUILayout.Height(_windowHeight - 25));

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear", GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                _logInfos.Clear();
                _logIndex = -1;
                _fpsColor = Color.white;
            }
            GUI.contentColor = _showInfo ? Color.white : Color.gray;
            if (GUILayout.Button("Info", GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                _showInfo = !_showInfo; _logIndex = -1;
            }
            GUI.contentColor = _showWarning ? Color.yellow : Color.gray;
            if (GUILayout.Button("Warning", GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                _showWarning = !_showWarning; _logIndex = -1;
            }
            GUI.contentColor = _showError ? Color.red : Color.gray;
            if (GUILayout.Button("Error", GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                _showError = !_showError; _logIndex = -1;
            }
            GUI.contentColor = _showFatal ? Color.magenta : Color.gray;
            if (GUILayout.Button("Fatal", GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                _showFatal = !_showFatal; _logIndex = -1;
            }
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();

            _scrollLogValue = GUILayout.BeginScrollView(_scrollLogValue, "Box", GUILayout.Height((_windowHeight - _leftBtnHight * 0.5f - 30) * 0.7f));
            for (int i = 0; i < _logInfos.Count; i++)
            {
                bool show = false;
                Color color;
                switch (_logInfos[i].type)
                {
                    case UnityEngine.LogType.Exception:
                        show = _showFatal;
                        color = Color.red;
                        break;
                    case UnityEngine.LogType.Error:
                        show = _showError;
                        color = Color.red;
                        break;
                    case UnityEngine.LogType.Log:
                        show = _showInfo;
                        color = Color.white;
                        break;
                    case UnityEngine.LogType.Warning:
                        show = _showWarning;
                        color = Color.yellow;
                        break;
                    default:
                        color = Color.white;
                        break;
                }

                if (show)
                {
                    GUI.contentColor = color;
                    string btnText = $"{_logInfos[i].time} {_logInfos[i].message.Split('\n')[0]}";
                    if (GUILayout.Button(btnText, GUILayout.Height(_leftBtnHight * 0.5f)))
                    {
                        _logIndex = i;
                    }
                    GUI.contentColor = Color.white;
                }
            }
            GUILayout.EndScrollView();

            _scrollTraceValue = GUILayout.BeginScrollView(_scrollTraceValue, "Box", GUILayout.Height((_windowHeight - _leftBtnHight * 0.5f - 30) * 0.3f - 10));
            if (_logIndex != -1)
            {
                GUILayout.Label(_logInfos[_logIndex].message + "\r\n" + _logInfos[_logIndex].stackTrace);
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        void DrawMemory()
        {
            GUILayout.BeginVertical("Box", GUILayout.Width(_windowWidth - _leftBtnWidth - 20), GUILayout.Height(_windowHeight - 25));

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("总分配内存：");
            GUILayout.Label("已占用内存：");
            GUILayout.Label("空闲中内存：");
            GUILayout.Label("总Mono堆内存：");
            GUILayout.Label("已占用Mono堆内存：");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1000000 + "MB");
            GUILayout.Label(UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1000000 + "MB");
            GUILayout.Label(UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong() / 1000000 + "MB");
            GUILayout.Label(UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong() / 1000000 + "MB");
            GUILayout.Label(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() / 1000000 + "MB");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            if (GUILayout.Button("卸载未使用的资源", GUILayout.Height(_leftBtnHight)))
            {
                Resources.UnloadUnusedAssets();
            }
            if (GUILayout.Button("使用GC垃圾回收", GUILayout.Height(_leftBtnHight)))
            {
                GC.Collect();
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        void DrawScreen()
        {
            _scrollScreenValue = GUILayout.BeginScrollView(_scrollScreenValue, "Box", GUILayout.Width(_windowWidth - _leftBtnWidth - 20), GUILayout.Height(_windowHeight - 25));

            GUILayout.BeginHorizontal();
            GUILayout.Label("当前屏幕分辨率：" + Screen.currentResolution);
            GUILayout.Label("当前窗口分辨率：" + Screen.width + " x " + Screen.height);
            GUILayout.EndHorizontal();

            var rls = Screen.resolutions;
            for (int i = 0; i < rls.Length; i++)
            {
                if (i % 4 == 0)
                {
                    GUILayout.BeginHorizontal();
                }
                if (GUILayout.Button(rls[i].ToString(), GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
                {
                    Screen.SetResolution(rls[i].width, rls[i].height, Screen.fullScreen);
                    ResetWidthHight(rls[i].width, rls[i].height);
                }
                if (i % 4 == 3 || i == rls.Length - 1)
                {
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.Label("自动旋屏：");
            GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));

            GUI.contentColor = Screen.autorotateToLandscapeLeft ? Color.white : Color.gray;
            if (GUILayout.Button("左横向", GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                Screen.autorotateToLandscapeLeft = !Screen.autorotateToLandscapeLeft;
            }
            GUI.contentColor = Screen.autorotateToPortrait ? Color.white : Color.gray;
            if (GUILayout.Button("竖直", GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                Screen.autorotateToPortrait = !Screen.autorotateToPortrait;
            }
            GUI.contentColor = Screen.autorotateToPortraitUpsideDown ? Color.white : Color.gray;
            if (GUILayout.Button("倒置", GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                Screen.autorotateToPortraitUpsideDown = !Screen.autorotateToPortraitUpsideDown;
            }
            GUI.contentColor = Screen.autorotateToLandscapeRight ? Color.white : Color.gray;
            if (GUILayout.Button("右横向", GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                Screen.autorotateToLandscapeRight = !Screen.autorotateToLandscapeRight;
            }
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();


            GUILayout.Label("屏幕模式：");
            GUILayout.BeginHorizontal();
            GUI.contentColor = Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen ? Color.white : Color.gray;
            if (GUILayout.Button("独占模式", GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
            }
            GUI.contentColor = Screen.fullScreenMode == FullScreenMode.FullScreenWindow ? Color.white : Color.gray;
            if (GUILayout.Button("全屏窗口".ToString(), GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
            }
            GUI.contentColor = Screen.fullScreenMode == FullScreenMode.MaximizedWindow ? Color.white : Color.gray;
            if (GUILayout.Button("最大化窗口", GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.MaximizedWindow);
            }
            GUI.contentColor = Screen.fullScreenMode == FullScreenMode.Windowed ? Color.white : Color.gray;
            if (GUILayout.Button("窗口化", GUILayout.Width((_windowWidth - _leftBtnWidth - 45) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
            }
            GUI.contentColor = Color.white;

            GUILayout.EndHorizontal();
            GUILayout.Label("屏幕逻辑方向：" + Screen.orientation);
            GUILayout.Label("屏幕DPI：" + Screen.dpi);
            GUILayout.Label("屏幕休眠时间：" + Screen.sleepTimeout);

            GUILayout.EndScrollView();

        }

        void DrawQuality()
        {
            _scrollQualityValue = GUILayout.BeginScrollView(_scrollQualityValue, "Box", GUILayout.Width(_windowWidth - _leftBtnWidth - 20), GUILayout.Height(_windowHeight - 25));

            GUILayout.Label("图形质量：");
            GUILayout.BeginHorizontal();
            int qualetiLevelCount = QualitySettings.names.Length;
            for (int i = 0; i < qualetiLevelCount; i++)
            {
                GUI.contentColor = QualitySettings.GetQualityLevel() == i ? Color.white : Color.gray;

                if (GUILayout.Button(QualitySettings.names[i], GUILayout.Width(((_windowWidth - _leftBtnWidth - 45 - (qualetiLevelCount - 1) * 5)) / qualetiLevelCount), GUILayout.Height(_leftBtnHight)))
                {
                    QualitySettings.SetQualityLevel(i);
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("各向异性过滤：");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 3; i++)
            {
                GUI.contentColor = (int)QualitySettings.anisotropicFiltering == i ? Color.white : Color.gray;
                if (GUILayout.Button(((AnisotropicFiltering)i).ToString(), GUILayout.Width(((_windowWidth - _leftBtnWidth - 45 - (3 - 1) * 5)) / 3), GUILayout.Height(_leftBtnHight * 0.5f)))
                {
                    QualitySettings.anisotropicFiltering = (AnisotropicFiltering)i;
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("抗锯齿：");
            GUILayout.BeginHorizontal();
            int _btnIndex = 0;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        _btnIndex = 0;
                        break;
                    case 1:
                        _btnIndex = 2;
                        break;
                    case 2:
                        _btnIndex = 4;
                        break;
                    case 3:
                        _btnIndex = 8;
                        break;
                }
                GUI.contentColor = QualitySettings.antiAliasing == _btnIndex ? Color.white : Color.gray;
                if (GUILayout.Button(_btnIndex == 0 ? "Disabled" : _btnIndex + "x", GUILayout.Width(((_windowWidth - _leftBtnWidth - 45 - (4 - 1) * 5)) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
                {
                    QualitySettings.antiAliasing = _btnIndex;
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("垂直同步：");
            GUILayout.BeginHorizontal();
            string vsynName = "";
            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        vsynName = "Don't Sync";
                        break;
                    case 1:
                        vsynName = "Every V Blank";
                        break;
                    case 2:
                        vsynName = "Every Second V Blank";
                        break;
                }
                GUI.contentColor = QualitySettings.vSyncCount == i ? Color.white : Color.gray;
                if (GUILayout.Button(vsynName, GUILayout.Width(((_windowWidth - _leftBtnWidth - 45 - (3 - 1) * 5)) / 3), GUILayout.Height(_leftBtnHight * 0.5f)))
                {
                    QualitySettings.vSyncCount = i;
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("阴影质量：");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 4; i++)
            {
                GUI.contentColor = (int)QualitySettings.shadowResolution == i ? Color.white : Color.gray;
                if (GUILayout.Button(((ShadowResolution)i).ToString(), GUILayout.Width(((_windowWidth - _leftBtnWidth - 45 - (4 - 1) * 5)) / 4), GUILayout.Height(_leftBtnHight * 0.5f)))
                {
                    QualitySettings.shadowResolution = (ShadowResolution)i;
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("阴影模式：");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 2; i++)
            {
                GUI.contentColor = (int)QualitySettings.shadowmaskMode == i ? Color.white : Color.gray;
                if (GUILayout.Button(((ShadowmaskMode)i).ToString(), GUILayout.Width(((_windowWidth - _leftBtnWidth - 45 - (2 - 1) * 5)) / 2), GUILayout.Height(_leftBtnHight * 0.5f)))
                {
                    QualitySettings.shadowmaskMode = (ShadowmaskMode)i;
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

            GUILayout.Label("实时阴影：");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 3; i++)
            {
                GUI.contentColor = (int)QualitySettings.shadows == i ? Color.white : Color.gray;
                if (GUILayout.Button(((ShadowQuality)i).ToString(), GUILayout.Width(((_windowWidth - _leftBtnWidth - 45 - (3 - 1) * 5)) / 3), GUILayout.Height(_leftBtnHight * 0.5f)))
                {
                    QualitySettings.shadows = (ShadowQuality)i;
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

            //Close Fit渲染较高分辨率的阴影，但是如果相机移动时，有时阴影会轻微摆动。
            //Stable Fit渲染的阴影分辨率较低，不过相机移动时不会发生摆动。
            GUILayout.Label("平行光投影方式：");
            GUILayout.BeginHorizontal();
            for (int i = 0; i < 2; i++)
            {
                GUI.contentColor = (int)QualitySettings.shadowProjection == i ? Color.white : Color.gray;
                if (GUILayout.Button(((ShadowProjection)i).ToString(), GUILayout.Width(((_windowWidth - _leftBtnWidth - 45 - (2 - 1) * 5)) / 2), GUILayout.Height(_leftBtnHight * 0.5f)))
                {
                    QualitySettings.shadowProjection = (ShadowProjection)i;
                }
                GUI.contentColor = Color.white;
            }
            GUILayout.EndHorizontal();

            GUI.contentColor = QualitySettings.softParticles ? Color.white : Color.gray;
            if (GUILayout.Button("粒子软混合", GUILayout.Width(_windowWidth - _leftBtnWidth - 45), GUILayout.Height(_leftBtnHight * 0.5f)))
            {
                QualitySettings.softParticles = !QualitySettings.softParticles;
            }
            GUI.contentColor = Color.white;

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("颜色空间：");
            GUILayout.Label("阴影距离：");
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label(QualitySettings.activeColorSpace.ToString());
            GUILayout.Label(QualitySettings.shadowDistance.ToString());
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        void DrawSystem()
        {
            _scrollSystemValue = GUILayout.BeginScrollView(_scrollSystemValue, "Box", GUILayout.Width(_windowWidth - _leftBtnWidth - 20), GUILayout.Height(_windowHeight - 25));
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            GUILayout.Label("操作系统：");
            GUILayout.Label("系统内存：");
            GUILayout.Label("处理器：");
            GUILayout.Label("处理器主频：");
            GUILayout.Label("处理器数量：");

            GUILayout.Label("显卡：");
            GUILayout.Label("显存：");
            GUILayout.Label("显卡供应商：");
            GUILayout.Label("显卡供应商标识码：");
            GUILayout.Label("显卡类型：");
            GUILayout.Label("显卡标识：");
            GUILayout.Label("显卡驱动版本");
            GUILayout.Label("显卡着色器级别");

            GUILayout.Label("设备名称：");
            GUILayout.Label("设备类型：");
            GUILayout.Label("设备模式：");
            GUILayout.Label("设备唯一标识：");

            GUILayout.Label("电量：");
            GUILayout.Label("电池状态：");
            GUILayout.Label("陀螺仪：");
            GUILayout.Label("加速度传感器：");
            GUILayout.Label("支持定位：");
            GUILayout.Label("支持触摸震动反馈：");

            GUILayout.Label("存在音频设备：");

            GUILayout.Label("支持多线程渲染：");
            //GUILayout.Label("支持Cubemap：");
            //GUILayout.Label("支持ImageEffects：");
            GUILayout.Label("支持GPU实例化：");
            GUILayout.Label("支持ComputeShaders：");
            GUILayout.Label("支持运动矢量：");
            GUILayout.Label("支持内置阴影：");
            GUILayout.Label("支持稀疏纹理：");
            GUILayout.Label("支持2D数组纹理：");
            GUILayout.Label("支持3D(体积)纹理：");
            GUILayout.Label("支持原始阴影深度采样：");
            GUILayout.Label("支持最大同步渲染数：");
            GUILayout.Label("支持的最大纹理尺寸：");

            GUILayout.EndVertical();


            GUILayout.BeginVertical();

            GUILayout.Label(SystemInfo.operatingSystem);
            GUILayout.Label(SystemInfo.systemMemorySize + "MB");
            GUILayout.Label(SystemInfo.processorType);
            GUILayout.Label((SystemInfo.processorFrequency * 0.001f).ToString("N2") + "GHz");
            GUILayout.Label(SystemInfo.processorCount.ToString());

            GUILayout.Label(SystemInfo.graphicsDeviceName);
            GUILayout.Label(SystemInfo.graphicsMemorySize + "MB");
            GUILayout.Label(SystemInfo.graphicsDeviceVendor);
            GUILayout.Label(SystemInfo.graphicsDeviceVendorID.ToString());
            GUILayout.Label(SystemInfo.graphicsDeviceType.ToString());
            GUILayout.Label(SystemInfo.graphicsDeviceID.ToString());
            GUILayout.Label(SystemInfo.graphicsDeviceVersion);
            GUILayout.Label("Shader Model " + (SystemInfo.graphicsShaderLevel));

            GUILayout.Label(SystemInfo.deviceName);
            GUILayout.Label(SystemInfo.deviceType.ToString());
            GUILayout.Label(SystemInfo.deviceModel);
            GUILayout.Label(SystemInfo.deviceUniqueIdentifier);

            GUILayout.Label(SystemInfo.batteryLevel.ToString());
            GUILayout.Label(SystemInfo.batteryStatus.ToString());
            GUILayout.Label(SystemInfo.supportsGyroscope.ToString());
            GUILayout.Label(SystemInfo.supportsAccelerometer.ToString());
            GUILayout.Label(SystemInfo.supportsLocationService.ToString());
            GUILayout.Label(SystemInfo.supportsVibration.ToString());

            GUILayout.Label(SystemInfo.supportsAudio.ToString());

            GUILayout.Label(SystemInfo.graphicsMultiThreaded.ToString());
            //GUILayout.Label(SystemInfo.supportsRenderToCubemap.ToString() + "19过期");
            //GUILayout.Label(SystemInfo.supportsImageEffects.ToString() + "19过期");
            GUILayout.Label(SystemInfo.supportsInstancing.ToString());
            GUILayout.Label(SystemInfo.supportsComputeShaders.ToString());
            GUILayout.Label(SystemInfo.supportsMotionVectors.ToString());
            GUILayout.Label(SystemInfo.supportsShadows.ToString());
            GUILayout.Label(SystemInfo.supportsSparseTextures.ToString());
            GUILayout.Label(SystemInfo.supports2DArrayTextures.ToString());
            GUILayout.Label(SystemInfo.supports3DTextures.ToString());
            GUILayout.Label(SystemInfo.supportsRawShadowDepthSampling.ToString());
            GUILayout.Label(SystemInfo.supportedRenderTargetCount.ToString());
            GUILayout.Label(SystemInfo.maxTextureSize.ToString());

            #region Temp

            //GUILayout.Label("操作系统：" + SystemInfo.operatingSystem);
            //GUILayout.Label("系统内存：" + SystemInfo.systemMemorySize + "MB");
            //GUILayout.Label("处理器：" + SystemInfo.processorType);
            //GUILayout.Label("处理器主频：" + (SystemInfo.processorFrequency*0.001f).ToString("N2") + "GHz");
            //GUILayout.Label("处理器数量：" + SystemInfo.processorCount);

            //GUILayout.Label("显卡：" + SystemInfo.graphicsDeviceName);
            //GUILayout.Label("显存：" + SystemInfo.graphicsMemorySize + "MB");
            //GUILayout.Label("显卡供应商：" + SystemInfo.graphicsDeviceVendor);
            //GUILayout.Label("显卡供应商标识码：" + SystemInfo.graphicsDeviceVendorID);
            //GUILayout.Label("显卡类型：" + SystemInfo.graphicsDeviceType);
            //GUILayout.Label("显卡标识：" + SystemInfo.graphicsDeviceID);
            //GUILayout.Label("显卡驱动版本" + SystemInfo.graphicsDeviceVersion);
            //GUILayout.Label("显卡着色器级别" + SystemInfo.graphicsShaderLevel);

            //GUILayout.Label("设备名称：" + SystemInfo.deviceName);
            //GUILayout.Label("设备类型：" + SystemInfo.deviceType);
            //GUILayout.Label("设备模式：" + SystemInfo.deviceModel);
            //GUILayout.Label("设备唯一标识：" + SystemInfo.deviceUniqueIdentifier);

            //GUILayout.Label("电量" + SystemInfo.batteryLevel);
            //GUILayout.Label("电池状态" + SystemInfo.batteryStatus);
            //GUILayout.Label("陀螺仪" + SystemInfo.supportsGyroscope);
            //GUILayout.Label("加速度传感器" + SystemInfo.supportsAccelerometer);
            //GUILayout.Label("支持定位" + SystemInfo.supportsLocationService);
            //GUILayout.Label("用户触摸震动反馈" + SystemInfo.supportsVibration);

            //GUILayout.Label("音频设备" + SystemInfo.supportsAudio);

            //GUILayout.Label("多线程渲染" + SystemInfo.graphicsMultiThreaded);
            //GUILayout.Label("支持Cubemap" + SystemInfo.supportsRenderToCubemap);
            //GUILayout.Label("支持ImageEffects" + SystemInfo.supportsImageEffects);
            //GUILayout.Label("支持GPU实例化" + SystemInfo.supportsInstancing);
            //GUILayout.Label("支持ComputeShaders" + SystemInfo.supportsComputeShaders);
            //GUILayout.Label("支持运动矢量" + SystemInfo.supportsMotionVectors);
            //GUILayout.Label("是否支持内置阴影" + SystemInfo.supportsShadows);
            //GUILayout.Label("是否支持稀疏纹理" + SystemInfo.supportsSparseTextures);
            //GUILayout.Label("是否支持2D数组纹理" + SystemInfo.supports2DArrayTextures);
            //GUILayout.Label("支持3D(体积)纹理" + SystemInfo.supports3DTextures);
            //GUILayout.Label("支持最大同步渲染数" + SystemInfo.supportedRenderTargetCount);
            //GUILayout.Label("支持的最大纹理尺寸" + SystemInfo.maxTextureSize);
            //GUILayout.Label("支持原始阴影深度采样" + SystemInfo.supportsRawShadowDepthSampling);


            #endregion

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        void DrawProject()
        {
            _scrollProjectValue = GUILayout.BeginScrollView(_scrollProjectValue, "Box", GUILayout.Width(_windowWidth - _leftBtnWidth - 20), GUILayout.Height(_windowHeight - 25));
            GUILayout.Label("项目名称：" + Application.productName);
            GUILayout.Label("项目版本：" + Application.version);
            GUILayout.Label("Unity版本：" + Application.unityVersion);
            GUILayout.Label("组织名称：" + Application.companyName);
            GUILayout.Label("包名：" + Application.identifier);

            GUILayout.Label("文件路径：" + Application.dataPath);
            GUILayout.Label("持久化路径：" + Application.persistentDataPath);
            GUILayout.Label("临时缓存路径：" + Application.temporaryCachePath);
            GUILayout.Label("控制台日志路径：" + Application.consoleLogPath);

            GUILayout.Label("尝试帧率：" + Application.targetFrameRate);
            GUILayout.Label("当前平台：" + Application.platform);
            GUILayout.Label("焦点：" + Application.isFocused);
            GUILayout.Label("GUID：" + Application.buildGUID);

            if (GUILayout.Button("退出程序", GUILayout.Height(_leftBtnHight)))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
            GUILayout.EndScrollView();
        }

    }

}
