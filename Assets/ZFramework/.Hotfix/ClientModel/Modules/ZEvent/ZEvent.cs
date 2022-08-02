/** Header
 *  ZEvent.cs
 *  事件系统 交互的入口 并持有各模块Handler和更新方法
 **/


namespace ZFramework
{
    public sealed class ZEvent
    {
        private ZEvent() { }//封闭构造
        private static ZEvent _instance;
        private static ZEvent Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ZEvent();
                return _instance;
            }
        }

        #region Handlers

        //public给编辑器画图的时候访问的
        public AudioEventHandler _audioEventHandler;
        public CollisionEventHandler _collisionEventHandler;
        public CustomEventHandler _customEventHandler;
        public CycleEventHandler _cycleEventHandler;
        public KeyEventHandler _keyEventHandler;
        public ScreenEventHandler _screenEventHandler;
        public TimerEventHandler _timerEventHandler;
        public TriggerEventHandler _triggerEventHandler;
        public UIEventHandler _uiEventHandler;
        public UnitEventHandler _unitEventHandler;

        private AudioEventHandler AudioEventHandler
        {
            get
            {
                if (_audioEventHandler == null) _audioEventHandler = CreateHandler<AudioEventHandler>();
                return _audioEventHandler;
            }
        }
        private CollisionEventHandler CollisionEventHandler
        {
            get
            {
                if (_collisionEventHandler == null) _collisionEventHandler = CreateHandler<CollisionEventHandler>();
                return _collisionEventHandler;
            }
        }
        private CustomEventHandler CustomEventHandler
        {
            get
            {
                if (_customEventHandler == null) _customEventHandler = CreateHandler<CustomEventHandler>();
                return _customEventHandler;
            }
        }
        private CycleEventHandler CycleEventHandler
        {
            get
            {
                if (_cycleEventHandler == null) _cycleEventHandler = CreateHandler<CycleEventHandler>();
                return _cycleEventHandler;
            }
        }
        private KeyEventHandler KeyEventHandler
        {
            get
            {
                if (_keyEventHandler == null) _keyEventHandler = CreateHandler<KeyEventHandler>();
                return _keyEventHandler;
            }
        }
        private ScreenEventHandler ScreenEventHandler
        {
            get
            {
                if (_screenEventHandler == null) _screenEventHandler = CreateHandler<ScreenEventHandler>();
                return _screenEventHandler;
            }
        }
        private TimerEventHandler TimerEventHandler
        {
            get
            {
                if (_timerEventHandler == null) _timerEventHandler = CreateHandler<TimerEventHandler>();
                return _timerEventHandler;
            }
        }
        private TriggerEventHandler TriggerEventHandler
        {
            get
            {
                if (_triggerEventHandler == null) _triggerEventHandler = CreateHandler<TriggerEventHandler>();
                return _triggerEventHandler;
            }
        }
        private UIEventHandler UIEventHandler
        {
            get
            {
                if (_uiEventHandler == null) _uiEventHandler = CreateHandler<UIEventHandler>();
                return _uiEventHandler;
            }
        }
        private UnitEventHandler UnitEventHandler
        {
            get
            {
                if (_unitEventHandler == null) _unitEventHandler = CreateHandler<UnitEventHandler>();
                return _unitEventHandler;
            }
        }

        #endregion

        #region Channels 和事件系统的交互从这里开始

        private static AudioEventChannel _audioEvent;
        private static CollisionEventChannel _collisionEvent;
        private static CustomEventChannel _customEvent;
        private static CycleEventChannel _cycleEvent;
        private static KeyEventChannel _keyEvent;
        private static ScreenEventChannel _screenEvent;
        private static TimerEventChannel _timerEvent;
        private static TriggerEventChannel _triggetEvent;
        private static UIEventChannel _uiEvent;
        private static UnitEventChannel _unitEvent;

        /// <summary> 音频事件 </summary>  最近比较迫切需要音频的关键帧事件 优先设计这个
        public static AudioEventChannel AudioEvent
        {
            get
            {
                if (_audioEvent == null)
                    _audioEvent = new AudioEventChannel(Instance.AudioEventHandler);
                return _audioEvent;
            }
        }

        /// <summary> 碰撞体事件 </summary>
        public static CollisionEventChannel CollisionEvent
        {
            get
            {
                if (_collisionEvent == null)
                    _collisionEvent = new CollisionEventChannel(Instance.CollisionEventHandler);
                return _collisionEvent;
            }
        }

        /// <summary> 自定义事件 </summary>
        public static CustomEventChannel CustomEvent
        {
            get
            {
                if (_customEvent == null)
                    _customEvent = new CustomEventChannel(Instance.CustomEventHandler);
                return _customEvent;
            }
        }

        /// <summary> 帧循环事件 </summary>
        public static CycleEventChannel CycleEvent
        {
            get
            {
                if (_cycleEvent == null)
                    _cycleEvent = new CycleEventChannel(Instance.CycleEventHandler);
                return _cycleEvent;
            }
        }

        /// <summary> 按键事件 </summary>
        public static KeyEventChannel KeyEvent
        {
            get
            {
                if (_keyEvent == null)
                    _keyEvent = new KeyEventChannel(Instance.KeyEventHandler);
                return _keyEvent;
            }
        }

        /// <summary> 屏幕事件 </summary>
        public static ScreenEventChannel ScreenEvent
        {
            get
            {
                if (_screenEvent == null)
                    _screenEvent = new ScreenEventChannel(Instance.ScreenEventHandler);
                return _screenEvent;
            }
        }

        /// <summary> 计时器事件</summary>
        public static TimerEventChannel TimerEvent
        {
            get
            {
                if (_timerEvent == null)
                    _timerEvent = new TimerEventChannel(Instance.TimerEventHandler);
                return _timerEvent;
            }
        }

        /// <summary> 触发器事件 </summary>
        public static TriggerEventChannel TriggerEvent
        {
            get
            {
                if (_triggetEvent == null)
                    _triggetEvent = new TriggerEventChannel(Instance.TriggerEventHandler);
                return _triggetEvent;
            }
        }

        /// <summary> UI事件</summary>
        public static UIEventChannel UIEvent
        {
            get
            {
                if (_uiEvent == null)
                    _uiEvent = new UIEventChannel(Instance.UIEventHandler);
                return _uiEvent;
            }
        }

        /// <summary> 单位事件 </summary>
        public static UnitEventChannel UnitEvent
        {
            get
            {
                if (_unitEvent == null)
                    _unitEvent = new UnitEventChannel(Instance.UnitEventHandler);
                return _unitEvent;
            }
        }

        /// <summary> 轴输入事件 </summary>
        //public static AxisEventChannel AxisEvent { }
        //通过axisName 来注册事件   axis可以通过inputmanager自己定义
        //有空可以研究下新的inputsys

        #endregion

        private T CreateHandler<T>() where T:ZEventHandlerBase,new () {
            var newHandler = new T();
            return newHandler;
        }


        //暂用
        internal static T GetNewData<T>() where T : ZEventDataBase, new() {
            return new T();
        }
        internal static T GetNewListener<T>() where T: ZEventListenerBase,new()
        {
            return new T();
        }
        internal static T GetNewGroup<T>() where T : ZEventListenerGroupBase, new() {
            return new T();
        }
    }
}