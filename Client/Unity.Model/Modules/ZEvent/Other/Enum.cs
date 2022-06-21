/** Header
 *  Enum.cs
 *  定义事件系统中使用的枚举
 **/

namespace ZFramework {

    /// <summary> 计时器的状态 </summary>
    public enum TimerState {
        /// <summary> 正在流逝 </summary>
        Running,
        /// <summary> 暂停中 </summary>
        Paused,
        /// <summary> 已失效 </summary>
        Death,
    }

    /// <summary> 计时器事件类型 </summary>
    public enum TimerEventType
    {
        /// <summary> 开始计时器 (每个新计时器首个周期的首次的Update前会调一次) </summary>
        Start,
        /// <summary> 计时器更新 </summary>
        Update,
        /// <summary> 计时器到期 </summary>
        Expire,
        /// <summary> 计时器暂停 </summary>
        Pause,
        /// <summary> 计时器继续 </summary>
        Continue,
        /// <summary> 计时器死亡 </summary>
        Kill,
        /// <summary> 计时器重启(相当于Start,重启后的首个周期的首次update前调一次) </summary>
        Restart
    }

    /// <summary> 碰撞事件类型 </summary>
    public enum CollisionEventType
    {
        /// <summary> 进入 </summary>
        Enter,
        /// <summary> 停留 </summary>
        Stay,
        /// <summary> 退出 </summary>
        Exit
    }

    /// <summary> Unit事件类型 </summary>
    public enum UnitEventType
    {
        /// <summary> 进入 </summary>
        Enter,
        /// <summary> 退出 </summary>
        Exit,
        /// <summary> 按下 </summary>
        Down,
        /// <summary> 弹起 </summary>
        Up,
        /// <summary> 点击 </summary>
        Click,
        /// <summary> 拖拽 </summary>
        Drag
    }

    /// <summary> 触发器事件类型 </summary>
    public enum TriggerEventType
    {
        /// <summary> 进入 </summary>
        Enter,
        /// <summary> 停留 </summary>
        Stay,
        /// <summary> 退出 </summary>
        Exit
    }

    /// <summary> UI事件类型 </summary>
    public enum UIEventType
    {
        /// <summary> 进入 </summary>
        Enter,
        /// <summary> 退出 </summary>
        Exit,
        /// <summary> 按下 </summary>
        Down,
        /// <summary> 弹起 </summary>
        Up,
        /// <summary> 点击 </summary>
        Click,
        /// <summary> 拖拽 </summary>
        Drag,
        /// <summary> 滚动 </summary>
        Scroll,
        /// <summary> 双击 </summary>
        //DoubleClick,  //双击事件有空后再加
    }

    /// <summary> 帧循环类型 </summary>
    public enum CycleType
    {
        /// <summary> 对应Unity生命周期Update </summary>
        Update,
        /// <summary> 对应Unity生命周期FixedUpdate </summary>
        FixedUpdate,
        /// <summary> 对应Unity生命周期LateUpdate </summary>
        LateUpdate
    }

    /// <summary> 键盘事件类型 </summary>
    public enum KeyEventType
    {
        /// <summary> 按下 </summary>
        Down,
        /// <summary> 按住 </summary>
        Press,
        /// <summary> 弹起 </summary>
        Up,
        /// <summary> 没有按住 </summary>
        NotPress
    }

    /// <summary> 屏幕事件类型 </summary>
    public enum ScreenEventType
    {
        /// <summary> 静止 </summary>
        Stay = 0,
        /// <summary> 移动 </summary>
        Move = 1,
        /// <summary> 按下 </summary>
        Down = 2,
        /// <summary> 弹起 </summary>
        Up = 3,
        /// <summary> 点击 </summary>
        Click = 4,
        /// <summary> 拖拽 </summary>
        Drag = 5
    }

    /// <summary> 指针类型 </summary>这个枚举值对应了Unity.EventSystem回调传递的pointerID按键编号 不能随意变动
    public enum PointerType
    {
        /// <summary> 中键 </summary>//
        Middle = -3,
        /// <summary> 右键 </summary>
        Right,
        /// <summary> 左键 </summary>
        Left,
        /// <summary> 第一个触摸点 </summary>
        Touch0,
        /// <summary> 第二个触摸点 </summary>
        Touch1,
        /// <summary> 第三个触摸点 </summary>
        Touch2,
        /// <summary> 第四个触摸点 </summary>
        Touch3,
        /// <summary> 第五个触摸点 </summary>
        Touch4,
        /// <summary> 第六个触摸点 </summary>
        Touch5
        //更多......
    }

    public enum AudioEventType
    {
        /// <summary> 开始 </summary>
        Start,
        /// <summary> 播放中 </summary>
        Playing,
        /// <summary> 暂停中 </summary>
        Pasued,
        /// <summary> 结束 </summary>
        End
    }

}

